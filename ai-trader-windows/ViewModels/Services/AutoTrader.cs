using AITrade.API;
using AITrade.Consts;
using AITrade.Entity.AI;
using AITrade.Services.Trades;
using AITrade.Utils;
using System.IO;
using System.Text.Json;

namespace AITrade.Services
{
    // -------------------- Config --------------------
    /// <summary>自动交易配置（简化版 - AI全权决策）</summary>
    public sealed class AutoTraderConfig
    {
        // Trader 标识
        public string ID { get; set; } = "default_trader";
        public string Name { get; set; } = "Default Trader";
        public string AIModel { get; set; } = "deepseek"; // "qwen" / "deepseek" / "custom"

        // 交易平台选择
        public string Exchange { get; set; } = "binance"; // "binance" | "hyperliquid" | "aster"

        // 币安API配置
        public string BinanceAPIKey { get; set; } = "";
        public string BinanceSecretKey { get; set; } = "";

        // Hyperliquid配置
        public string HyperliquidPrivateKey { get; set; } = "";
        public string HyperliquidWalletAddr { get; set; } = "";
        public bool HyperliquidTestnet { get; set; } = false;

        // Aster配置
        public string AsterUser { get; set; } = "";
        public string AsterSigner { get; set; } = "";
        public string AsterPrivateKey { get; set; } = "";

        public string CoinPoolAPIURL { get; set; } = "https://api.binance.com/api/v3/exchangeInfo";

        // AI配置
        public bool UseQwen { get; set; } = false;
        public string DeepSeekKey { get; set; } = "";
        public string QwenKey { get; set; } = "";

        // 自定义AI API配置
        public string CustomAPIURL { get; set; } = "";
        public string CustomAPIKey { get; set; } = "";
        public string CustomModelName { get; set; } = "";

        // 扫描配置
        public TimeSpan ScanInterval { get; set; } = TimeSpan.FromMinutes(180);

        // 账户配置
        public double InitialBalance { get; set; } = 1000;

        // 杠杆配置
        public int BTCETHLeverage { get; set; } = 10;
        public int AltcoinLeverage { get; set; } = 10;

        // 风险控制提示
        public double MaxDailyLoss { get; set; } = 0;
        public double MaxDrawdown { get; set; } = 0;
        public TimeSpan StopTradingTime { get; set; } = TimeSpan.Zero;

        //日志路徑
        public string LogDirectory { get; set; } = Path.Combine("decision_logs", "unknown");
    }

    // ========================= AutoTrader =========================
    public class AutoTrader
    {
        private readonly string _id;
        private readonly string _name;
        private readonly string _aiModel;
        private readonly string _exchange;
        public readonly AutoTraderConfig Config;
        private readonly ITrader _trader;
        private readonly AIClient _mcpClient;
        private readonly DecisionLogger _decisionLogger;

        private double _dailyPnL = 0;
        private DateTime _lastResetTime;
        private DateTime _stopUntil = DateTime.MinValue;

        private bool _isRunning = false;
        public readonly DateTime StartTime;
        public int CallCount = 0;

        public readonly double InitialBalance;
        public readonly Dictionary<string, long> PositionFirstSeenTime = new(); // symbol_side -> ms

        private AutoTrader(
            AutoTraderConfig config,
            ITrader trader,
            AIClient mcpClient,
            DecisionLogger decisionLogger)
        {
            _id = config.ID;
            _name = config.Name;
            _aiModel = ResolveAIModel(config);
            _exchange = string.IsNullOrWhiteSpace(config.Exchange) ? "binance" : config.Exchange;
            Config = config;
            _trader = trader;
            _mcpClient = mcpClient;
            _decisionLogger = decisionLogger;
            InitialBalance = config.InitialBalance;

            _lastResetTime = DateTime.Now;
            StartTime = DateTime.Now;
        }

        public static AutoTrader Create(AutoTraderConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.ID)) config.ID = "default_trader";
            if (string.IsNullOrWhiteSpace(config.Name)) config.Name = "Default Trader";

            AIClient mcp;
            if (config.AIModel == "custom")
            {
                //todo: custom model client
                mcp = new DeepSeekAIClient(config.DeepSeekKey);
                System.Diagnostics.Debug.WriteLine($"🤖 [{config.Name}] 使用自定义AI API: {config.CustomAPIURL} (模型: {config.CustomModelName})");
            }
            else if (config.AIModel == "qwen")
            {
                //todo : Qwen client
                mcp = new DeepSeekAIClient(config.DeepSeekKey);
                System.Diagnostics.Debug.WriteLine($"🤖 [{config.Name}] 使用阿里云Qwen AI");
            }
            else if (config.AIModel == "deepseek")
            {
                mcp = new DeepSeekAIClient(config.DeepSeekKey);
                System.Diagnostics.Debug.WriteLine($"🤖 [{config.Name}] 使用DeepSeek AI");
            }
            else
            {
                throw new ArgumentException($"未指定有效的AI模型");
            }

            if (!string.IsNullOrWhiteSpace(config.CoinPoolAPIURL))
            {
                Pool.SetCoinPoolAPI(config.CoinPoolAPIURL);
            }

            var exchange = string.IsNullOrWhiteSpace(config.Exchange) ? "binance" : config.Exchange;

            ITrader trader = exchange switch
            {
                "binance" => new BinanceFuturesTrader(config.BinanceAPIKey, config.BinanceSecretKey),
                _ => throw new ArgumentException($"不支持的交易平台: {config.Exchange}")
            };

            System.Diagnostics.Debug.WriteLine(exchange switch
            {
                "binance" => $"🏦 [{config.Name}] 使用币安合约交易",
                "hyperliquid" => $"🏦 [{config.Name}] 使用Hyperliquid交易",
                "aster" => $"🏦 [{config.Name}] 使用Aster交易",
                _ => $"🏦 [{config.Name}] 未知交易平台"
            });

            if (config.InitialBalance <= 0)
                throw new ArgumentException("初始金额必须大于0，请在配置中设置 InitialBalance");

            var logger = new DecisionLogger(config.LogDirectory);

            return new AutoTrader(config, trader, mcp, logger);
        }

        private static string ResolveAIModel(AutoTraderConfig cfg)
        {
            if (!string.IsNullOrWhiteSpace(cfg.AIModel)) return cfg.AIModel;
            return cfg.UseQwen ? "qwen" : "deepseek";
        }

        // -------------------- Control --------------------
        public async Task Run()
        {
            _isRunning = true;
            System.Diagnostics.Debug.WriteLine("🚀 AI驱动自动交易系统启动");
            System.Diagnostics.Debug.WriteLine($"💰 初始余额: {InitialBalance:F2} USDT");
            System.Diagnostics.Debug.WriteLine($"⚙️  扫描间隔: {Config.ScanInterval}");
            System.Diagnostics.Debug.WriteLine("🤖 AI将全权决定杠杆、仓位大小、止损止盈等参数");

            while (_isRunning)
            {
                if (await RunCycleSafe() is Exception e)
                    System.Diagnostics.Debug.WriteLine($"❌ 执行失败: {e.Message}");
                Thread.Sleep(Config.ScanInterval);
            }
        }

        public void Stop()
        {
            _isRunning = false;
            System.Diagnostics.Debug.WriteLine("⏹ 自动交易系统停止");
        }

        private async Task<Exception?> RunCycleSafe()
        {
            try
            {
                await RunCycle();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        // -------------------- Core Cycle --------------------
        private async Task RunCycle()
        {
            CallCount++;

            System.Diagnostics.Debug.WriteLine("\n" + new string('=', 70));
            System.Diagnostics.Debug.WriteLine($"⏰ {DateTime.Now:yyyy-MM-dd HH:mm:ss} - AI决策周期 #{CallCount}");
            System.Diagnostics.Debug.WriteLine(new string('=', 70));

            var record = new DecisionRecord
            {
                ExecutionLog = new List<string>(),
                Success = true,
                ErrorMessage = "",
            };

            // 风控暂停
            if (DateTime.Now < _stopUntil)
            {
                var remaining = _stopUntil - DateTime.Now;
                System.Diagnostics.Debug.WriteLine($"⏸ 风险控制：暂停交易中，剩余 {remaining.TotalMinutes:F0} 分钟");
                record.Success = false;
                record.ErrorMessage = $"风险控制暂停中，剩余 {remaining.TotalMinutes:F0} 分钟";
                _decisionLogger.LogDecision(record);
                return;
            }

            // 日盈亏重置
            if ((DateTime.Now - _lastResetTime).TotalHours > 24)
            {
                _dailyPnL = 0;
                _lastResetTime = DateTime.Now;
                System.Diagnostics.Debug.WriteLine("📅 日盈亏已重置");
            }

            // 上下文
            ContextData ctx;
            try
            {
                ctx = await BuildContextAsync();
            }
            catch (Exception ex)
            {
                record.Success = false;
                record.ErrorMessage = $"构建交易上下文失败: {ex.Message}";
                _decisionLogger.LogDecision(record);
                throw;
            }
            record.Title = $"**时间**: {ctx.CurrentTime} | **周期**: #{ctx.CallCount} | **运行**: {ctx.RuntimeMinutes}分钟\n"; ;

            record.AccountState = new AccountSnapshot
            {
                TotalBalance = ctx.Account.TotalEquity,
                AvailableBalance = ctx.Account.AvailableBalance,
                TotalUnrealizedProfit = ctx.Account.TotalPnL,
                PositionCount = ctx.Account.PositionCount,
                MarginUsedPct = ctx.Account.MarginUsedPct
            };

            foreach (var pos in ctx.Positions)
            {
                record.Positions.Add(new PositionSnapshot
                {
                    Symbol = pos.Symbol,
                    Side = pos.Side,
                    PositionAmt = pos.Quantity,
                    EntryPrice = pos.EntryPrice,
                    MarkPrice = pos.MarkPrice,
                    UnrealizedProfit = pos.UnrealizedProfit,
                    LiquidationPrice = pos.LiquidationPrice,
                    Leverage = pos.Leverage,
                });
            }

            foreach (var coin in ctx.CandidateCoins)
                record.CandidateCoins.Add(coin.Symbol);

            System.Diagnostics.Debug.WriteLine($"📊 账户净值: {ctx.Account.TotalEquity:F2} USDT | 可用: {ctx.Account.AvailableBalance:F2} USDT | 持仓: {ctx.Account.PositionCount}");

            System.Diagnostics.Debug.WriteLine("🤖 正在请求AI分析并决策...");
            FullDecision? ai = null;
            try
            {
                ai = await DecisionService.GetFullDecision(ctx, _mcpClient);
                if (ai != null)
                {
                    record.InputPrompt = ai.UserPrompt;
                    record.CoTTrace = ai.CoTTrace;
                    if (ai.Decisions?.Count > 0)
                        record.DecisionJSON = JsonSerializer.Serialize(ai.Decisions, new JsonSerializerOptions { WriteIndented = true });
                }
            }
            catch (Exception ex)
            {
                record.Success = false;
                record.ErrorMessage = $"获取AI决策失败: {ex.Message}";
                if (ai != null && !string.IsNullOrWhiteSpace(ai.CoTTrace))
                {
                    System.Diagnostics.Debug.WriteLine("\n" + new string('-', 70));
                    System.Diagnostics.Debug.WriteLine("💭 AI思维链分析（错误情况）：");
                    System.Diagnostics.Debug.WriteLine(new string('-', 70));
                    System.Diagnostics.Debug.WriteLine(ai.CoTTrace);
                    System.Diagnostics.Debug.WriteLine(new string('-', 70) + "\n");
                }
                _decisionLogger.LogDecision(record);
                throw;
            }

            System.Diagnostics.Debug.WriteLine("\n" + new string('-', 70));
            System.Diagnostics.Debug.WriteLine("💭 AI思维链分析:");
            System.Diagnostics.Debug.WriteLine(new string('-', 70));
            System.Diagnostics.Debug.WriteLine(ai!.CoTTrace);
            System.Diagnostics.Debug.WriteLine(new string('-', 70) + "\n");

            System.Diagnostics.Debug.WriteLine($"📋 AI决策列表 ({ai.Decisions.Count} 个):\n");
            for (int i = 0; i < ai.Decisions.Count; i++)
            {
                var d = ai.Decisions[i];
                System.Diagnostics.Debug.WriteLine($"  [{i + 1}] {d.Symbol}: {d.Action} - {d.Reasoning}");
                if (d.Action is "open_long" or "open_short")
                {
                    System.Diagnostics.Debug.WriteLine($"      杠杆: {d.Leverage}x | 仓位: {d.PositionSizeUSD:F2} USDT | 止损: {d.StopLoss:F4} | 止盈: {d.TakeProfit:F4}");
                }
            }

            var sorted = SortDecisionsByPriority(ai.Decisions);
            System.Diagnostics.Debug.WriteLine("🔄 执行顺序（已优化）: 先平仓→后开仓");
            for (int i = 0; i < sorted.Count; i++)
                System.Diagnostics.Debug.WriteLine($"  [{i + 1}] {sorted[i].Symbol} {sorted[i].Action}");

            foreach (var d in sorted)
            {
                var action = new DecisionAction
                {
                    Action = d.Action,
                    Symbol = d.Symbol,
                    Quantity = 0,
                    Leverage = d.Leverage,
                    Price = 0,
                    Timestamp = DateTime.Now,
                    Success = false
                };

                Exception? ex = await ExecuteDecisionWithRecord(d, action, ctx.Positions);
                if (ex != null)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ 执行决策失败 ({d.Symbol} {d.Action}): {ex.Message}");
                    action.Error = ex.Message;
                    record.ExecutionLog.Add($"❌ {d.Symbol} {d.Action} 失败: {ex.Message}");
                }
                else
                {
                    action.Success = true;
                    record.ExecutionLog.Add($"✓ {d.Symbol} {d.Action} 成功");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }

                record.Decisions.Add(action);
            }

            try
            {
                _decisionLogger.LogDecision(record);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠ 保存决策记录失败: {ex.Message}");
            }
        }

        private async Task<ContextData> BuildContextAsync()
        {
            if (_trader == null)
            {
                throw new Exception("Trader instance is null, cannot fetch user data.");
            }
            var balance = await _trader.GetAccountInfo();
            double totalWalletBalance = CommonUtils.GetDouble(balance.Balance);
            double totalUnrealizedProfit = CommonUtils.GetDouble(balance.TotalUnrealizedProfit);
            double availableBalance = CommonUtils.GetDouble(balance.AvailableBalance);

            double totalEquity = totalWalletBalance + totalUnrealizedProfit;

            var positions = await _trader.GetPositions();
            var positionInfos = new List<PositionInfo>();
            double totalMarginUsed = 0.0;

            var currentKeys = new HashSet<string>();

            foreach (var pos in positions)
            {
                var symbol = CommonUtils.GetString(pos.Symbol);
                var side = CommonUtils.GetString(pos.Side);
                var entryPrice = CommonUtils.GetDouble(pos.EntryPrice);
                var markPrice = CommonUtils.GetDouble(pos.CurrentPrice);
                var rawQty = Math.Abs(CommonUtils.GetDouble(pos.Quantity));
                var unrealized = CommonUtils.GetDouble(pos.UnrealizedProfit);
                var liquidationPrice = CommonUtils.GetDouble(pos.LiquidationPrice);
                double marginUsed = rawQty * markPrice / Math.Max(1, pos.Leverage);
                totalMarginUsed += marginUsed;

                var key = $"{symbol}_{side}";
                currentKeys.Add(key);
                if (!PositionFirstSeenTime.ContainsKey(key))
                    PositionFirstSeenTime[key] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                var updateTime = PositionFirstSeenTime[key];

                positionInfos.Add(new PositionInfo
                {
                    Symbol = symbol,
                    Side = side,
                    EntryPrice = entryPrice,
                    MarkPrice = markPrice,
                    Quantity = rawQty,
                    Leverage = pos.Leverage,
                    UnrealizedProfit = unrealized,
                    UnrealizedPnLPct = unrealized / (entryPrice * rawQty),
                    LiquidationPrice = liquidationPrice,
                    MarginUsed = marginUsed,
                    UpdateTime = updateTime
                });
            }

            // 清理已平仓的首次时间记录
            var toRemove = PositionFirstSeenTime.Keys.Where(k => !currentKeys.Contains(k)).ToList();
            foreach (var k in toRemove) PositionFirstSeenTime.Remove(k);

            const int ai500Limit = 20;
            var mergedPool = await Pool.GetMergedCoinPoolAsync(ai500Limit);

            var candidateCoins = new List<CandidateCoin>();
            foreach (var sym in mergedPool.AllSymbols)
            {
                mergedPool.SymbolSources.TryGetValue(sym, out var sources);
                candidateCoins.Add(new CandidateCoin { Symbol = sym, Sources = sources ?? new List<string>() });
            }

            System.Diagnostics.Debug.WriteLine($"📋 合并币种池: AI500前{ai500Limit} + OI_Top20 = 总计{candidateCoins.Count}个候选币种");

            double totalPnL = totalEquity - InitialBalance;
            double totalPnLPct = InitialBalance > 0 ? totalPnL / InitialBalance * 100.0 : 0.0;
            double marginUsedPct = totalEquity > 0 ? totalMarginUsed / totalEquity * 100.0 : 0.0;

            object? performance = null;
            try
            {
                performance = _decisionLogger.AnalyzePerformance(100);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️  分析历史表现失败: {ex.Message}");
            }

            return new ContextData
            {
                CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                RuntimeMinutes = (int)(DateTime.Now - StartTime).TotalMinutes,
                CallCount = CallCount,
                BTCETHLeverage = Config.BTCETHLeverage,
                AltcoinLeverage = Config.AltcoinLeverage,
                Account = new AccountInfo
                {
                    TotalEquity = totalEquity,
                    AvailableBalance = availableBalance,
                    TotalPnL = totalPnL,
                    TotalPnLPct = totalPnLPct,
                    MarginUsed = totalMarginUsed,
                    MarginUsedPct = marginUsedPct,
                    PositionCount = positionInfos.Count
                },
                Positions = positionInfos,
                CandidateCoins = candidateCoins,
                Performance = performance
            };
        }

        // -------------------- Execute Decisions --------------------
        private async Task<Exception?> ExecuteDecisionWithRecord(Decision d, DecisionAction actionRecord, List<PositionInfo> positionInfos)
        {
            try
            {
                return d.Action switch
                {
                    "open_long" => await ExecuteOpenLongWithRecord(d, actionRecord),
                    "open_short" => await ExecuteOpenShortWithRecord(d, actionRecord),
                    "close_long" => await ExecuteCloseLongWithRecord(d, actionRecord, positionInfos),
                    "close_short" => await ExecuteCloseShortWithRecord(d, actionRecord, positionInfos),
                    "hold" or "wait" => null,
                    _ => new Exception($"未知的action: {d.Action}")
                };
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        private async Task<Exception?> ExecuteOpenLongWithRecord(Decision d, DecisionAction ar)
        {
            System.Diagnostics.Debug.WriteLine($"  📈 开多仓: {d.Symbol}");

            try
            {
                var positions = await _trader.GetPositions();
                if (positions.Any(p => CommonUtils.GetString(p.Symbol) == d.Symbol && CommonUtils.GetString(p.Side) == PositionConstants.LONG))
                    return new Exception($"❌ {d.Symbol} 已有多仓，拒绝开仓以防止仓位叠加超限。如需换仓，请先给出 close_long 决策");
            }
            catch { /* why: 读取持仓失败时不阻断，保持容错 */ }

            var md = await MarketInfoClient.GetAsync(d.Symbol);
            var qty = d.PositionSizeUSD * d.Leverage / md.CurrentPrice;
            ar.Quantity = qty;
            ar.Price = md.CurrentPrice;
            try
            {
                ar.OrderID = await _trader.OpenLong(d.Symbol, qty, d.Leverage, (decimal)d.TakeProfit, (decimal)d.StopLoss);
            }
            catch (Exception ex)
            {
                return new Exception($"开多仓失败: {ex.Message}");
            }
            if (ar.OrderID is null || ar.OrderID == 0)
            {
                return new Exception("❌ 开多仓失败，订单ID为空或无效");
            }
            System.Diagnostics.Debug.WriteLine($"  ✓ 开仓成功，订单ID: {ar.OrderID} , 数量: {qty:F4}");

            var key = $"{d.Symbol}_long";
            PositionFirstSeenTime[key] = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            return null;
        }

        private async Task<Exception?> ExecuteOpenShortWithRecord(Decision d, DecisionAction ar)
        {
            System.Diagnostics.Debug.WriteLine($"  📉 开空仓: {d.Symbol}");

            try
            {
                var positions = await _trader.GetPositions();
                if (positions.Any(p => CommonUtils.GetString(p.Symbol) == d.Symbol && CommonUtils.GetString(p.Side) == PositionConstants.SHORT))
                    return new Exception($"❌ {d.Symbol} 已有空仓，拒绝开仓以防止仓位叠加超限。如需换仓，请先给出 close_short 决策");
            }
            catch { }

            var md = await MarketInfoClient.GetAsync(d.Symbol);
            var qty = d.PositionSizeUSD * d.Leverage / md.CurrentPrice;
            ar.Quantity = qty;
            ar.Price = md.CurrentPrice;

            try
            {
                ar.OrderID = await _trader.OpenShort(d.Symbol, qty, d.Leverage, (decimal)d.TakeProfit, (decimal)d.StopLoss);
            }
            catch (Exception ex)
            {
                return new Exception($"开空仓失败: {ex.Message}");
            }
            if (ar.OrderID is null || ar.OrderID == 0)
            {
                return new Exception("❌ 开空仓失败，订单ID为空或无效");
            }
            System.Diagnostics.Debug.WriteLine($"  ✓ 开仓成功，订单ID: {ar.OrderID} , 数量: {qty:F4}");

            var key = $"{d.Symbol}_short";
            PositionFirstSeenTime[key] = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            return null;
        }

        private async Task<Exception?> ExecuteCloseLongWithRecord(Decision d, DecisionAction ar, List<PositionInfo> positionInfos)
        {
            var position = positionInfos.FirstOrDefault(p => p.Symbol == d.Symbol && p.Side == PositionConstants.LONG);
            if (position == null)
            {
                return new Exception($"❌ {d.Symbol} 无多仓可平");
            }
            System.Diagnostics.Debug.WriteLine($"  🔄 平多仓: {d.Symbol}");
            var md = await MarketInfoClient.GetAsync(d.Symbol);
            ar.Quantity = position.Quantity;
            ar.Price = md.CurrentPrice;
            try
            {
                ar.OrderID = await _trader.CloseLong(d.Symbol, position.Quantity);
            }
            catch (Exception ex)
            {
                return new Exception($"平多仓失败: {ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine("  ✓ 平仓成功");
            return null;
        }

        private async Task<Exception?> ExecuteCloseShortWithRecord(Decision d, DecisionAction ar, List<PositionInfo> positionInfos)
        {
            var position = positionInfos.FirstOrDefault(p => p.Symbol == d.Symbol && p.Side == PositionConstants.SHORT);
            if (position == null)
            {
                return new Exception($"❌ {d.Symbol} 无空仓可平");
            }
            System.Diagnostics.Debug.WriteLine($"  🔄 平空仓: {d.Symbol}");
            var md = await MarketInfoClient.GetAsync(d.Symbol);
            ar.Quantity = position.Quantity;
            ar.Price = md.CurrentPrice;
            try
            {
                ar.OrderID = await _trader.CloseShort(d.Symbol, position.Quantity);
            }
            catch (Exception ex)
            {
                return new Exception($"平空仓失败: {ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine("  ✓ 平仓成功");
            return null;
        }

        // -------------------- Public Info --------------------
        public string GetID() => _id;
        public string GetName() => _name;
        public string GetAIModel() => _aiModel;
        public DecisionLogger GetDecisionLogger() => _decisionLogger;

        public Dictionary<string, object> GetStatus()
        {
            var aiProvider = Config.UseQwen ? "Qwen" : "DeepSeek";
            return new Dictionary<string, object>
            {
                ["trader_id"] = _id,
                ["trader_name"] = _name,
                ["ai_model"] = _aiModel,
                ["exchange"] = _exchange,
                ["is_running"] = _isRunning,
                ["start_time"] = StartTime.ToString("o"),
                ["runtime_minutes"] = (int)(DateTime.Now - StartTime).TotalMinutes,
                ["call_count"] = CallCount,
                ["initial_balance"] = InitialBalance,
                ["scan_interval"] = Config.ScanInterval.ToString(),
                ["stop_until"] = _stopUntil.ToString("o"),
                ["last_reset_time"] = _lastResetTime.ToString("o"),
                ["ai_provider"] = aiProvider
            };
        }

        public async Task<Dictionary<string, object>> GetAccountInfo()
        {
            var balance = await _trader.GetAccountInfo();

            double totalWalletBalance = CommonUtils.GetDouble(balance.Balance);
            double totalUnrealizedProfit = CommonUtils.GetDouble(balance.TotalUnrealizedProfit);
            double availableBalance = CommonUtils.GetDouble(balance.AvailableBalance);

            double totalEquity = totalWalletBalance + totalUnrealizedProfit;

            var positions = await _trader.GetPositions();

            double totalMarginUsed = 0.0;
            double totalUnrealizedPnL = 0.0;
            foreach (var pos in positions)
            {
                var markPrice = CommonUtils.GetDouble(pos.CurrentPrice);
                var quantity = Math.Abs(CommonUtils.GetDouble(pos.Quantity));
                var unrealized = CommonUtils.GetDouble(pos.UnrealizedProfit);
                totalUnrealizedPnL += unrealized;

                int lev = pos.Leverage;
                double marginUsed = quantity * markPrice / Math.Max(1, lev);
                totalMarginUsed += marginUsed;
            }

            double totalPnL = totalEquity - InitialBalance;
            double totalPnLPct = InitialBalance > 0 ? totalPnL / InitialBalance * 100.0 : 0.0;
            double marginUsedPct = totalEquity > 0 ? totalMarginUsed / totalEquity * 100.0 : 0.0;

            return new Dictionary<string, object>
            {
                ["total_equity"] = totalEquity,
                ["wallet_balance"] = totalWalletBalance,
                ["unrealized_profit"] = totalUnrealizedProfit,
                ["available_balance"] = availableBalance,

                ["total_pnl"] = totalPnL,
                ["total_pnl_pct"] = totalPnLPct,
                ["total_unrealized_pnl"] = totalUnrealizedPnL,
                ["initial_balance"] = InitialBalance,
                ["daily_pnl"] = _dailyPnL,

                ["position_count"] = positions.Count,
                ["margin_used"] = totalMarginUsed,
                ["margin_used_pct"] = marginUsedPct
            };
        }

        public async Task<List<Dictionary<string, object>>> GetPositions()
        {
            var positions = await _trader.GetPositions();
            var result = new List<Dictionary<string, object>>();

            foreach (var pos in positions)
            {
                var symbol = CommonUtils.GetString(pos.Symbol);
                var side = CommonUtils.GetString(pos.Side);
                var entryPrice = CommonUtils.GetDouble(pos.EntryPrice);
                var markPrice = CommonUtils.GetDouble(pos.CurrentPrice);
                var quantity = Math.Abs(CommonUtils.GetDouble(pos.Quantity));
                var unrealized = CommonUtils.GetDouble(pos.UnrealizedProfit);
                int lev = pos.Leverage;

                double pnlPct = side == "long"
                    ? (markPrice - entryPrice) / entryPrice * lev * 100.0
                    : (entryPrice - markPrice) / entryPrice * lev * 100.0;

                double marginUsed = quantity * markPrice / Math.Max(1, lev);

                result.Add(new Dictionary<string, object>
                {
                    ["symbol"] = symbol,
                    ["side"] = side,
                    ["entry_price"] = entryPrice,
                    ["mark_price"] = markPrice,
                    ["quantity"] = quantity,
                    ["leverage"] = lev,
                    ["unrealized_pnl"] = unrealized,
                    ["unrealized_pnl_pct"] = pnlPct,
                    ["margin_used"] = marginUsed
                });
            }

            return result;
        }

        // -------------------- Utilities --------------------
        private static List<Decision> SortDecisionsByPriority(List<Decision> decisions)
        {
            if (decisions.Count <= 1) return decisions;

            int Pri(string action) => action switch
            {
                "close_long" or "close_short" => 1,
                "open_long" or "open_short" => 2,
                "hold" or "wait" => 3,
                _ => 999
            };

            return decisions.OrderBy(d => Pri(d.Action)).ToList();
        }
    }

    // ========================= Example Entry =========================
    //public static class Program
    //{
    //    public static void Main(string[] args)
    //    {
    //        // Demo：运行 1 个循环后停止（避免示例卡住），按需修改
    //        var cfg = new AutoTraderConfig
    //        {
    //            ID = "demo",
    //            Name = "Demo Trader",
    //            AIModel = "deepseek",
    //            Exchange = "binance",
    //            InitialBalance = 10000,
    //            ScanInterval = TimeSpan.FromSeconds(2),
    //            BTCETHLeverage = 10,
    //            AltcoinLeverage = 10
    //        };

    //        var trader = AutoTrader.Create(cfg);

    //        // 只跑一次周期演示
    //        var t = new Thread(() => trader.Run());
    //        t.IsBackground = true;
    //        t.Start();
    //        Thread.Sleep(TimeSpan.FromSeconds(3));
    //        trader.Stop();

    //        // 打印状态
    //        var status = trader.GetStatus();
    //        System.Diagnostics.Debug.WriteLine($"状态: {JsonSerializer.Serialize(status)}");
    //    }
    //}
}
