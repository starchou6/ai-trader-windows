using AITrade.Consts;
using Binance.Net.Clients;
using Binance.Net.Objects.Models.Futures;
using CryptoExchange.Net.Authentication;

namespace AITrade.API
{
    public class BinanceClient
    {
        private ApiCredentials _apiCredentials;

        public BinanceClient(string key, string secret)
        {
            _apiCredentials = new ApiCredentials(
                key: key,
                secret: secret
            );
        }

        public async Task<List<BinanceFuturesUsdtTrade>> GetCompletedTrades()
        {
            var restClient = new BinanceRestClient(options =>
            {
                options.UsdFuturesOptions.ApiCredentials = _apiCredentials;
            });
            var tradesHistoryResult = new List<BinanceFuturesUsdtTrade>();
            foreach (var symbol in CommonConstants.COIN_SYMBOL_LIST)
            {
                var tradesResult = await restClient.UsdFuturesApi.Trading.GetUserTradesAsync(symbol: symbol);
                tradesHistoryResult.AddRange(tradesResult.Data);
            }
            tradesHistoryResult.RemoveAll(trade => trade.RealizedPnl == 0);
            tradesHistoryResult.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
            return tradesHistoryResult;
        }

        public async Task<BinanceFuturesAccountInfo> GetAccountInfo()
        {
            var restClient = new BinanceRestClient(options =>
            {
                options.UsdFuturesOptions.ApiCredentials = _apiCredentials;
            });
            var accountInfoResult = await restClient.UsdFuturesApi.Account.GetAccountInfoV2Async();
            return accountInfoResult.Data;
        }

        public async Task<List<string>> GetAllSymbols()
        {
            var restClient = new BinanceRestClient(options =>
            {
                options.UsdFuturesOptions.ApiCredentials = _apiCredentials;
            });
            var exchangeInfoResult = await restClient.UsdFuturesApi.ExchangeData.GetExchangeInfoAsync();
            if (!exchangeInfoResult.Success)
                return [];
            return exchangeInfoResult.Data.Symbols
                .Where(s => s.Status == Binance.Net.Enums.SymbolStatus.Trading && s.Name.EndsWith("USDT"))
                .Select(s => s.Name)
                .OrderBy(s => s)
                .ToList();
        }

        public async Task<BinancePositionDetailsUsdt[]> GetPositions()
        {
            var restClient = new BinanceRestClient(options =>
            {
                options.UsdFuturesOptions.ApiCredentials = _apiCredentials;
            });
            var accountInfoResult = await restClient.UsdFuturesApi.Account.GetPositionInformationAsync();
            return accountInfoResult.Data;
        }

        public async Task<BinanceUsdFuturesOrder[]> GetOpenOrders()
        {
            var restClient = new BinanceRestClient(options =>
            {
                options.UsdFuturesOptions.ApiCredentials = _apiCredentials;
            });
            var openOrdersResult = await restClient.UsdFuturesApi.Trading.GetOpenOrdersAsync();
            return openOrdersResult.Data;
        }

        public async Task<BinanceUsdFuturesOrder> PlaceOrder(
            string symbol,
            decimal quantity,
            decimal? price,
            Binance.Net.Enums.OrderSide side,
            Binance.Net.Enums.FuturesOrderType orderType,
            decimal? takeProfit = null,
            decimal? stopLoss = null,
            int leverage = 0)
        {
            var restClient = new BinanceRestClient(options =>
            {
                options.UsdFuturesOptions.ApiCredentials = _apiCredentials;
            });
            // 0. 设置杠杆（如果指定了）
            if (leverage > 0)
            {
                var levResult = await restClient.UsdFuturesApi.Account.ChangeInitialLeverageAsync(symbol, leverage);
                if (!levResult.Success)
                {
                    throw new Exception($"设置杠杆失败: {levResult.Error?.Message}");
                }
            }
            // 修正数量和价格到合约允许的精度
            var exchangeInfoResult = await restClient.UsdFuturesApi.ExchangeData.GetExchangeInfoAsync();
            if (!exchangeInfoResult.Success)
                throw new Exception($"获取交易对信息失败: {exchangeInfoResult.Error?.Message}");
            var symbolInfo = exchangeInfoResult.Data.Symbols.FirstOrDefault(s => s.Name == symbol);
            if (symbolInfo == null)
                throw new Exception($"未找到交易对信息: {symbol}");

            var fixedQuantity = TruncateDecimal(quantity, symbolInfo.QuantityPrecision);

            // 1. 下主单
            var orderResult = await restClient.UsdFuturesApi.Trading.PlaceOrderAsync(
                symbol: symbol,
                quantity: fixedQuantity,
                price: price,
                side: side,
                type: orderType
            );
            if (!orderResult.Success)
            {
                throw new Exception($"下主单出错: {orderResult.Error?.Message}");
            }

            // 2. 下止盈单
            if (takeProfit.HasValue)
            {
                var tpResult = await restClient.UsdFuturesApi.Trading.PlaceConditionalOrderAsync(
                    symbol: symbol,
                    side: side == Binance.Net.Enums.OrderSide.Buy ? Binance.Net.Enums.OrderSide.Sell : Binance.Net.Enums.OrderSide.Buy,
                    type: Binance.Net.Enums.ConditionalOrderType.TakeProfitMarket,
                    quantity: fixedQuantity,
                    triggerPrice: takeProfit,
                    reduceOnly: true
                );
                if (!tpResult.Success)
                {
                    throw new Exception($"下止盈单出错: {tpResult.Error?.Message}");
                }
            }

            // 3. 下止损单
            if (stopLoss.HasValue)
            {
                var slResult = await restClient.UsdFuturesApi.Trading.PlaceConditionalOrderAsync(
                    symbol: symbol,
                    side: side == Binance.Net.Enums.OrderSide.Buy ? Binance.Net.Enums.OrderSide.Sell : Binance.Net.Enums.OrderSide.Buy,
                    type: Binance.Net.Enums.ConditionalOrderType.StopMarket,
                    quantity: fixedQuantity,
                    triggerPrice: stopLoss,
                    reduceOnly: true
                );
                if (!slResult.Success)
                {
                    throw new Exception($"下止损单出错: {slResult.Error?.Message}");
                }
            }

            return orderResult.Data;
        }

        public async Task<bool> CancelOrder(string symbol, long orderId)
        {
            var restClient = new BinanceRestClient(options =>
            {
                options.UsdFuturesOptions.ApiCredentials = _apiCredentials;
            });
            var cancelResult = await restClient.UsdFuturesApi.Trading.CancelOrderAsync(
                symbol: symbol,
                orderId: orderId
            );
            return cancelResult.Success;
        }

        // 截断到合约允许的小数位数
        private static decimal TruncateDecimal(decimal value, int precision)
        {
            if (precision < 0) return value;
            decimal factor = (decimal)Math.Pow(10, precision);
            return Math.Floor(value * factor) / factor;
        }
    }
}
