using AITrade.Entity.AI;
using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace AITrade.API
{
    public static class OITopClient
    {
        private static readonly HttpClient httpClient = new HttpClient();

        // 按需配置
        private static string _apiUrl = ""; // OI Top API URL
        private static readonly string CacheDir = "coin_pool_cache";
        private static readonly string CacheFile = Path.Combine(CacheDir, "oi_top_latest.json");

        public static void SetApiUrl(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        public static async Task<List<OIPosition>> GetOITopPositionsAsync(string? apiUrl = null)
        {
            _apiUrl = apiUrl;
            if (string.IsNullOrWhiteSpace(_apiUrl))
            {
                return new List<OIPosition>();
            }

            Exception lastError = null;

            for (int attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    var positions = await FetchFromApiAsync();
                    await SaveCacheAsync(positions);
                    return positions;
                }
                catch (Exception ex)
                {
                    lastError = ex;
                    await Task.Delay(2000);
                }
            }

            // fallback: cache
            try
            {
                return await LoadCacheAsync();
            }
            catch
            {
                return new List<OIPosition>();
            }
        }

        private static async Task<List<OIPosition>> FetchFromApiAsync()
        {
            var response = await httpClient.GetStringAsync(_apiUrl);
            var apiResponse = JsonSerializer.Deserialize<OITopApiResponse>(response);

            if (apiResponse == null || !apiResponse.Success || apiResponse.Data?.Positions == null)
                throw new Exception("Invalid OI Top API response");

            return apiResponse.Data.Positions;
        }

        private static async Task SaveCacheAsync(List<OIPosition> positions)
        {
            Directory.CreateDirectory(CacheDir);

            var cache = new OITopCache
            {
                Positions = positions,
                FetchedAt = DateTime.UtcNow,
                SourceType = "api"
            };

            var json = JsonSerializer.Serialize(cache, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(CacheFile, json);
        }

        private static async Task<List<OIPosition>> LoadCacheAsync()
        {
            if (!File.Exists(CacheFile))
                throw new FileNotFoundException("OI Top cache not found");

            var json = await File.ReadAllTextAsync(CacheFile);
            var cache = JsonSerializer.Deserialize<OITopCache>(json);

            return cache?.Positions ?? new List<OIPosition>();
        }
    }
}
