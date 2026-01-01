using AITrade.API;
using AITrade.Converter;
using AITrade.Entity;

namespace AITrade.Services.Trades
{
    public class BinanceFuturesTrader(string key, string secret) : ITrader
    {
        private readonly BinanceClient _binanceClient = new(key, secret);

        public async Task<AccountData> GetAccountInfo()
        {
            var accountInfo = await _binanceClient.GetAccountInfo();
            return BinanceDataConverter.ConvertToAccountData(accountInfo);
        }
        public async Task<List<PositionData>> GetPositions()
        {
            var positions = await _binanceClient.GetPositions();
            return BinanceDataConverter.ConvertToPositionDatas(positions);
        }
        public async Task<long?> OpenLong(string symbol, double quantity, int leverage, decimal takeProfit, decimal stopLoss)
        {
            var result = await _binanceClient.PlaceOrder(
                symbol,
                (decimal)quantity,
                null,
                Binance.Net.Enums.OrderSide.Buy,
                Binance.Net.Enums.FuturesOrderType.Market,
                takeProfit,
                stopLoss,
                leverage
            );
            return result?.Id;
        }
        public async Task<long?> OpenShort(string symbol, double quantity, int leverage, decimal takeProfit, decimal stopLoss)
        {
            var result = await _binanceClient.PlaceOrder(
                symbol,
                (decimal)quantity,
                null,
                Binance.Net.Enums.OrderSide.Sell,
                Binance.Net.Enums.FuturesOrderType.Market,
                takeProfit,
                stopLoss,
                leverage
            );
            return result?.Id;
        }
        public async Task<long?> CloseLong(string symbol, double quantity)
        {
            var result = await _binanceClient.PlaceOrder(
                symbol,
                (decimal)quantity,
                null,
                Binance.Net.Enums.OrderSide.Sell,
                Binance.Net.Enums.FuturesOrderType.Market
            );
            return result.Id;
        }
        public async Task<long?> CloseShort(string symbol, double quantity)
        {
            var result = await _binanceClient.PlaceOrder(
                symbol,
                (decimal)quantity,
                null,
                Binance.Net.Enums.OrderSide.Buy,
                Binance.Net.Enums.FuturesOrderType.Market
            );
            return result?.Id;
        }
        public bool SetStopLoss(string symbol, string side, double quantity, double stopLoss) => true;
        public bool SetTakeProfit(string symbol, string side, double quantity, double takeProfit) => true;
    }
}
