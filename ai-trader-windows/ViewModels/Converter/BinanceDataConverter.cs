using AITrade.Consts;
using AITrade.Entity;
using Binance.Net.Enums;
using Binance.Net.Objects.Models.Futures;

namespace AITrade.Converter
{
    public static class BinanceDataConverter
    {
        public static AccountData ConvertToAccountData(BinanceFuturesAccountInfo account)
        {
            if (account == null)
            {
                return new AccountData
                {
                    ApiStatus = false,
                    Balance = 0,
                    AvailableBalance = 0,
                    TotalUnrealizedProfit = 0
                };
            }
            return new AccountData
            {
                ApiStatus = true,
                Balance = (decimal)account.TotalWalletBalance,
                AvailableBalance = (decimal)account.AvailableBalance,
                TotalUnrealizedProfit = (decimal)account.TotalUnrealizedProfit
            };
        }

        public static List<PositionData> ConvertToPositionDatas(BinancePositionDetailsUsdt[] positionDetails)
        {
            var result = new List<PositionData>();
            if (positionDetails == null || positionDetails.Length == 0) return result;

            foreach (var p in positionDetails)
            {
                if (p.Quantity == 0)
                    continue;

                var absQty = Math.Abs(p.Quantity);
                decimal currentPrice = p.EntryPrice + (p.UnrealizedPnl / p.Quantity);

                result.Add(new PositionData
                {
                    Side = p.Quantity > 0 ? PositionConstants.LONG : PositionConstants.SHORT,
                    Symbol = p.Symbol,
                    Leverage = p.Leverage,
                    Quantity = absQty,
                    EntryPrice = p.EntryPrice,
                    CurrentPrice = currentPrice,
                    EntryAmount = (absQty * p.EntryPrice),
                    CurrentAmount = (absQty * currentPrice),
                    UnrealizedProfit = p.UnrealizedPnl,
                    LiquidationPrice = p.LiquidationPrice
                });
            }

            return result;
        }

        public static List<CompletedTrade> CompletedTrades(List<BinanceFuturesUsdtTrade> binanceFuturesUsdtTrades)
        {
            var result = new List<CompletedTrade>();
            if (binanceFuturesUsdtTrades == null) return result;
            foreach (var t in binanceFuturesUsdtTrades)
            {
                var absQty = Math.Abs(t.Quantity);
                decimal entryPrice = t.Price + (t.RealizedPnl / t.Quantity);

                result.Add(new CompletedTrade
                {
                    Coin = t.Symbol,
                    Side = t.Side == OrderSide.Buy ? PositionConstants.SHORT : "Long",
                    Quantity = absQty,
                    EntryPrice = entryPrice,
                    EntryAmount = absQty * entryPrice,
                    CompletedPrice = t.Price,
                    CompletedAmount = absQty * t.Price,
                    RealizedProfit = t.RealizedPnl,
                    Timestamp = t.Timestamp,
                });
            }
            return result;
        }

    }

}
