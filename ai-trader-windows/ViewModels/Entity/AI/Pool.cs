using AITrade.Consts;
using System.Net.Http;
using System.Text.Json;

namespace AITrade.Entity.AI
{
    public static class Pool
    {
        private static string _api = "";
        public static void SetCoinPoolAPI(string url) => _api = url;

        public static async Task<MergedPool> GetMergedCoinPoolAsync(int limit)
        {
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
