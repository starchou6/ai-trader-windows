using AITrade.Consts;
using System.Net.Http;
using System.Text.Json;

namespace AITrade.Entity.AI
{
    public static class Pool
    {
        private static string _api = "";
        private static List<string> _selectedCoins = new List<string>();

        public static void SetCoinPoolAPI(string url) => _api = url;
        public static void SetSelectedCoins(List<string> coins) => _selectedCoins = coins ?? new List<string>();

        public static async Task<MergedPool> GetMergedCoinPoolAsync(int limit)
        {
            // 如果设置了自定义币种列表，直接使用
            if (_selectedCoins.Count > 0)
            {
                return new MergedPool
                {
                    AllSymbols = _selectedCoins,
                    SymbolSources = _selectedCoins.ToDictionary(s => s, s => new List<string> { "user_selected" })
                };
            }

            // 否则使用默认的 API 获取
            if (string.IsNullOrWhiteSpace(_api))
                throw new InvalidOperationException("Coin pool API URL is not set.");

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(_api);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var pool = JsonSerializer.Deserialize<MergedPool>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return pool ?? new MergedPool();
        }
    }

    public class MergedPool
    {
        public List<string> AllSymbols { get; set; } = [.. CommonConstants.COIN_SYMBOL_LIST];
        public Dictionary<string, List<string>> SymbolSources { get; set; } = new();
    }
}
