using AITrade.Entity;

namespace AITrade.Services.Trades
{
    public interface ITrader
    {
        Task<AccountData> GetAccountInfo();
        Task<List<PositionData>> GetPositions();

        Task<long?> OpenLong(string symbol, double quantity, int leverage, decimal takeProfit, decimal stopLoss);
        Task<long?> OpenShort(string symbol, double quantity, int leverage, decimal takeProfit, decimal stopLoss);
        Task<long?> CloseLong(string symbol, double quantity);  // quantity为0 = 全部
        Task<long?> CloseShort(string symbol, double quantity); // quantity为0 = 全部
    }
}
