using AITrade.Entity.AI;
using AITrade.Extentsions;
using AITrade.Utils;
using System.Text.Json;

namespace AITrade.Services
{
    public class DecisionService
    {
        public static async Task<FullDecision> GetFullDecision(ContextData ctx, AIClient mcpClient, string customPrompt = null)
        {
            // 1) market + OI
            await ctx.FetchMarketDataForContextAsync();

            // 2) prompts
            var systemPrompt = PromptUtil.GetSystemPrompt(customPrompt);
            var userPrompt = PromptUtil.GetUserPrompt(ctx);

            // 3) call AI
            string aiResponse = await mcpClient.CallWithMessages(systemPrompt, userPrompt);

            // 4) parse & validate
            var parsed = ParseFullDecisionResponse(aiResponse, ctx.Account.TotalEquity, ctx.BTCETHLeverage, ctx.AltcoinLeverage);
            parsed.Timestamp = DateTime.Now;
            parsed.UserPrompt = userPrompt;
            return parsed;
        }

        private static FullDecision ParseFullDecisionResponse(string aiResponse, double accountEquity, int btcEthLeverage, int altcoinLeverage)
        {
            string cot = ExtractCoTTrace(aiResponse);
            List<Decision> decisions;
            try
            {
                decisions = ExtractDecisions(aiResponse);
            }
            catch (Exception ex)
            {
                return new FullDecision
                {
                    CoTTrace = cot,
                    Decisions = new List<Decision>()
                };
            }

            try
            {
                ValidateDecisions(decisions, accountEquity, btcEthLeverage, altcoinLeverage);
            }
            catch (Exception ex)
            {
                return new FullDecision
                {
                    CoTTrace = cot,
                    Decisions = decisions
                };
            }

            return new FullDecision
            {
                CoTTrace = cot,
                Decisions = decisions
            };
        }

        private static string ExtractCoTTrace(string response)
        {
            int jsonStart = response.IndexOf('[');
            if (jsonStart > 0) return response[..jsonStart].Trim();
            return response.Trim();
        }

        private static List<Decision> ExtractDecisions(string response)
        {
            int start = response.IndexOf('[');
            if (start == -1) throw new Exception("无法找到JSON数组起始");

            int end = FindMatchingBracket(response, start);
            if (end == -1) throw new Exception("无法找到JSON数组结束");

            string jsonContent = response.Substring(start, end - start + 1).Trim();
            jsonContent = FixMissingQuotes(jsonContent);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var decisions = JsonSerializer.Deserialize<List<Decision>>(jsonContent, options)
                            ?? new List<Decision>();
            return decisions;
        }

        private static string FixMissingQuotes(string s)
        {
            // 替换中文引号为英文
            return s.Replace('\u201c', '"')
                    .Replace('\u201d', '"')
                    .Replace('\u2018', '\'')
                    .Replace('\u2019', '\'');
        }

        private static void ValidateDecisions(List<Decision> decisions, double accountEquity, int btcEthLeverage, int altcoinLeverage)
        {
            for (int i = 0; i < decisions.Count; i++)
            {
                ValidateDecision(decisions[i], accountEquity, btcEthLeverage, altcoinLeverage);
            }
        }

        private static int FindMatchingBracket(string s, int start)
        {
            if (start < 0 || start >= s.Length || s[start] != '[') return -1;

            int depth = 0;
            for (int i = start; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '[') depth++;
                else if (c == ']')
                {
                    depth--;
                    if (depth == 0) return i;
                }
            }
            return -1;
        }

        private static void ValidateDecision(Decision d, double accountEquity, int btcEthLeverage, int altcoinLeverage)
        {
            var valid = new HashSet<string> { "open_long", "open_short", "close_long", "close_short", "hold", "wait" };
            if (!valid.Contains(d.Action))
                throw new Exception($"无效的action: {d.Action}");

            if (d.Action is "open_long" or "open_short")
            {
                int maxLev = altcoinLeverage;
                double maxValue = accountEquity * 1.5;
                if (d.Symbol == "BTCUSDT" || d.Symbol == "ETHUSDT")
                {
                    maxLev = btcEthLeverage;
                    maxValue = accountEquity * 10.0;
                }

                if (d.Leverage <= 0 || d.Leverage > maxLev)
                    throw new Exception($"杠杆必须在1-{maxLev}之间 ({d.Symbol})，实际: {d.Leverage}");

                if (d.PositionSizeUSD <= 0)
                    throw new Exception($"仓位大小必须大于0: {d.PositionSizeUSD:0.##}");

                double tol = maxValue * 0.01;
                if (d.PositionSizeUSD > maxValue + tol)
                {
                    if (d.Symbol is "BTCUSDT" or "ETHUSDT")
                        throw new Exception($"BTC/ETH单币种仓位价值不能超过{maxValue:0} USDT（10倍账户净值），实际: {d.PositionSizeUSD:0}");
                    else
                        throw new Exception($"山寨币单币种仓位价值不能超过{maxValue:0} USDT（1.5倍账户净值），实际: {d.PositionSizeUSD:0}");
                }

                if (d.StopLoss <= 0 || d.TakeProfit <= 0)
                    throw new Exception("止损和止盈必须大于0");

                if (d.Action == "open_long")
                {
                    if (d.StopLoss >= d.TakeProfit)
                        throw new Exception("做多时止损价必须小于止盈价");
                }
                else
                {
                    if (d.StopLoss <= d.TakeProfit)
                        throw new Exception("做空时止损价必须大于止盈价");
                }

                // 风险回报
                double entryPrice = d.Action == "open_long"
                    ? d.StopLoss + (d.TakeProfit - d.StopLoss) * 0.2
                    : d.StopLoss - (d.StopLoss - d.TakeProfit) * 0.2;

                double riskPct, rewardPct, rr = 0;
                if (d.Action == "open_long")
                {
                    riskPct = (entryPrice - d.StopLoss) / entryPrice * 100.0;
                    rewardPct = (d.TakeProfit - entryPrice) / entryPrice * 100.0;
                    if (riskPct > 0) rr = rewardPct / riskPct;
                }
                else
                {
                    riskPct = (d.StopLoss - entryPrice) / entryPrice * 100.0;
                    rewardPct = (entryPrice - d.TakeProfit) / entryPrice * 100.0;
                    if (riskPct > 0) rr = rewardPct / riskPct;
                }

                if (rr < 3.0)
                    throw new Exception($"风险回报比过低({rr:0.00}:1)，必须≥3.0:1 [风险:{riskPct:0.00}% 收益:{rewardPct:0.00}%] [止损:{d.StopLoss:0.0000} 止盈:{d.TakeProfit:0.0000}]");
            }
        }
    }
}
