using AITrade.Entity.AI;
using System.Text;

namespace AITrade.Utils
{
    public static class PromptUtil
    {
        public const string DefaultCustomPrompt =
            "你是一名专业的加密货币合约交易员。" +
            "你的目标是尽可能赚多的钱。" +
            "你拥有的完整数据：1.原始序列：3分钟价格序列(MidPrices数组) + 4小时K线序列；2.技术序列：EMA20序列、MACD序列、RSI7序列、RSI14序列；3.资金序列：成交量序列、持仓量(OI)序列、资金费率。" +
            "你的决策流程为：1.分析完整数据：自由运用序列数据，你可以做但不限于趋势分析、形态识别、支撑阻力、技术阻力位、斐波那契、波动带计算、多维度交叉验证（价格+量+OI+指标+序列形态），用你认为最有效的方法发现高确定性机会；" +
            "2.评估持仓: 趋势是否改变，是否该止盈/止损；" +
            "3.寻找新机会: 是否有强信号，是否有多空机会；" +
            "4.输出决策: 思维链分析 + JSON。";

        private const string FixedOutputFormat =
            "你的输出格式为：1.思维链（纯文本）：简洁分析你的思考过程；" +
            "2.JSON决策数组格式为：json[{\"symbol\": \"BTCUSDT\", \"action\": \"open_short\", \"leverage\": 10, \"position_size_usd\": 100.0, \"stop_loss\": 97000, \"take_profit\": 91000, \"confidence\": 85, \"risk_usd\": 300, \"reasoning\": \"下跌趋势+MACD死叉\"},{\"symbol\": \"ETHUSDT\", \"action\": \"close_long\", \"reasoning\": \"止盈离场\"}]" +
            "字段说明：`action`: open_long | open_short | close_long | close_short | hold | wait ，`confidence`: 0-100（开仓建议≥75），开仓时必填: leverage, position_size_usd, stop_loss, take_profit, confidence, risk_usd, reasoning。";

        public static string GetSystemPrompt(string customPrompt = null)
        {
            var prompt = string.IsNullOrWhiteSpace(customPrompt) ? DefaultCustomPrompt : customPrompt;
            return prompt + FixedOutputFormat;
        }

        public static string GetUserPrompt(ContextData ctx)
        {
            var sb = new StringBuilder();

            // 系统状态
            sb.AppendLine($"**时间**: {ctx.CurrentTime} | **周期**: #{ctx.CallCount} | **运行**: {ctx.RuntimeMinutes}分钟\n");

            // BTC 市场
            if (ctx.MarketDataMap != null && ctx.MarketDataMap.TryGetValue("BTCUSDT", out var btcData))
            {
                sb.AppendLine($"**BTC**: {btcData.CurrentPrice:F2} (1h: {btcData.PriceChange1h:+0.00;-0.00}%, 4h: {btcData.PriceChange4h:+0.00;-0.00}%) | MACD: {btcData.CurrentMACD:F4} | RSI: {btcData.CurrentRSI7:F2}\n");
            }

            // 账户
            var availablePct = ctx.Account.TotalEquity > 0
                ? (ctx.Account.AvailableBalance / ctx.Account.TotalEquity) * 100
                : 0;

            sb.AppendLine($"**账户**: 净值{ctx.Account.TotalEquity:F2} | 余额{ctx.Account.AvailableBalance:F2} ({availablePct:F1}%) | 盈亏{ctx.Account.TotalPnLPct:+0.00;-0.00}% | 保证金{ctx.Account.MarginUsedPct:F1}% | 持仓{ctx.Account.PositionCount}个\n");

            // 持仓（完整市场数据）
            if (ctx.Positions != null && ctx.Positions.Any())
            {
                sb.AppendLine("## 当前持仓\n");

                for (int i = 0; i < ctx.Positions.Count; i++)
                {
                    var pos = ctx.Positions[i];
                    string holdingDuration = "";

                    if (pos.UpdateTime > 0)
                    {
                        var durationMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - pos.UpdateTime;
                        var durationMin = durationMs / (1000 * 60);

                        if (durationMin < 60)
                            holdingDuration = $" | 持仓时长{durationMin}分钟";
                        else
                            holdingDuration = $" | 持仓时长{durationMin / 60}小时{durationMin % 60}分钟";
                    }

                    sb.AppendLine($"{i + 1}. {pos.Symbol} {pos.Side.ToUpper()} | 入场价{pos.EntryPrice:F4} 当前价{pos.MarkPrice:F4} | 盈亏{pos.UnrealizedPnLPct:+0.00;-0.00}% | 杠杆{pos.Leverage}x | 保证金{pos.MarginUsed:F0} | 强平价{pos.LiquidationPrice:F4}{holdingDuration}\n");

                    if (ctx.MarketDataMap != null && ctx.MarketDataMap.TryGetValue(pos.Symbol, out var marketData))
                    {
                        sb.AppendLine(MarketDataFormatter.Format(marketData));
                        sb.AppendLine();
                    }
                }
            }
            else
            {
                sb.AppendLine("**当前持仓**: 无\n");
            }

            // 候选币种
            var candidateCount = ctx.MarketDataMap?.Count ?? 0;
            sb.AppendLine($"## 候选币种 ({candidateCount}个)\n");

            int displayedCount = 0;

            if (ctx.CandidateCoins != null)
            {
                foreach (var coin in ctx.CandidateCoins)
                {
                    if (ctx.MarketDataMap != null && ctx.MarketDataMap.TryGetValue(coin.Symbol, out var marketData))
                    {
                        displayedCount++;

                        string sourceTags = coin.Sources?.Count > 1
                            ? " (AI500+OI_Top双重信号)"
                            : coin.Sources?.FirstOrDefault() == "oi_top"
                                ? " (OI_Top持仓增长)"
                                : "";

                        sb.AppendLine($"### {displayedCount}. {coin.Symbol}{sourceTags}\n");
                        sb.AppendLine(MarketDataFormatter.Format(marketData));
                        sb.AppendLine();
                    }
                }
            }

            sb.AppendLine("---\n");
            sb.AppendLine("现在请分析并输出决策（思维链 + JSON）\n");

            return sb.ToString();
        }
    }
}
