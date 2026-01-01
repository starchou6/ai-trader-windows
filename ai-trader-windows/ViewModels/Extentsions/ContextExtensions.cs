using AITrade.API;
using AITrade.Entity.AI;
using Microsoft.Extensions.Logging;

namespace AITrade.Extentsions
{
    public static class ContextExtensions
    {
        public static async Task FetchMarketDataForContextAsync(this ContextData ctx, ILogger logger = null)
        {
            ctx.MarketDataMap = new Dictionary<string, MarketData>();
            ctx.OITopDataMap = new Dictionary<string, OITopData>();

            var symbolSet = new HashSet<string>();

            foreach (var pos in ctx.Positions)
                symbolSet.Add(pos.Symbol);

            int maxCandidates = CalculateMaxCandidates(ctx);
            foreach (var coin in ctx.CandidateCoins.Take(maxCandidates))
                symbolSet.Add(coin.Symbol);

            var positionSymbols = ctx.Positions.Select(p => p.Symbol).ToHashSet();

            foreach (var symbol in symbolSet)
            {
                MarketData data;
                try
                {
                    data = await MarketInfoClient.GetAsync(symbol);
                }
                catch
                {
                    logger?.LogWarning("Failed to fetch market data for symbol: {Symbol}", symbol);
                    continue;
                }

                bool isExistingPosition = positionSymbols.Contains(symbol);

                if (!isExistingPosition && data?.OpenInterest != null && data.CurrentPrice > 0)
                {
                    double oiValue = data.OpenInterest.Latest * data.CurrentPrice;
                    double oiValueInMillions = oiValue / 1_000_000;

                    if (oiValueInMillions < 15)
                    {
                        logger?.LogWarning("⚠️  {Symbol} 持仓价值过低 ({Value:F2}M USD < 15M)，跳过", symbol, oiValueInMillions);
                        continue;
                    }
                }

                ctx.MarketDataMap[symbol] = data;
            }

            try
            {
                var oiPositions = await OITopClient.GetOITopPositionsAsync();
                foreach (var pos in oiPositions)
                {
                    string symbol = pos.Symbol;

                    ctx.OITopDataMap[symbol] = new OITopData
                    {
                        Rank = pos.Rank,
                        OIDeltaPercent = pos.OIDeltaPercent,
                        OIDeltaValue = pos.OIDeltaValue,
                        PriceDeltaPercent = pos.PriceDeltaPercent,
                        NetLong = pos.NetLong,
                        NetShort = pos.NetShort
                    };
                }
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "加载 OI Top 数据失败");
            }
        }

        private static int CalculateMaxCandidates(ContextData ctx)
        {
            return ctx.CandidateCoins?.Count ?? 0;
        }
    }
}
