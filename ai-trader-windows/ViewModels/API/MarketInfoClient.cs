using AITrade.Entity.AI;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;

namespace AITrade.API
{
    public class MarketInfoClient
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<MarketData> GetAsync(string symbol)
        {
            symbol = Normalize(symbol);


            var klines3m = await GetKlines(symbol, "3m", 40);
            var klines4h = await GetKlines(symbol, "4h", 60);


            var currentPrice = klines3m.Last().Close;
            var currentEMA20 = CalculateEMA(klines3m, 20);
            var currentMACD = CalculateMACD(klines3m);
            var currentRSI7 = CalculateRSI(klines3m, 7);


            double priceChange1h = 0;
            if (klines3m.Count >= 21)
            {
                var price1hAgo = klines3m[^21].Close;
                priceChange1h = price1hAgo > 0 ? ((currentPrice - price1hAgo) / price1hAgo) * 100 : 0;
            }


            double priceChange4h = 0;
            if (klines4h.Count >= 2)
            {
                var price4hAgo = klines4h[^2].Close;
                priceChange4h = price4hAgo > 0 ? ((currentPrice - price4hAgo) / price4hAgo) * 100 : 0;
            }


            var oiData = await GetOpenInterestData(symbol);
            var fundingRate = await GetFundingRate(symbol);


            return new MarketData
            {
                Symbol = symbol,
                CurrentPrice = currentPrice,
                PriceChange1h = priceChange1h,
                PriceChange4h = priceChange4h,
                CurrentEMA20 = currentEMA20,
                CurrentMACD = currentMACD,
                CurrentRSI7 = currentRSI7,
                OpenInterest = oiData,
                FundingRate = fundingRate
            };
        }

        private static async Task<List<Kline>> GetKlines(string symbol, string interval, int limit)
        {
            var url = $"https://fapi.binance.com/fapi/v1/klines?symbol={symbol}&interval={interval}&limit={limit}";
            var response = await httpClient.GetStringAsync(url);
            var rawData = JsonSerializer.Deserialize<List<List<JsonElement>>>(response);

            return rawData.Select(item => new Kline
            {
                OpenTime = item[0].GetInt64(),
                Open = double.Parse(item[1].GetString(), CultureInfo.InvariantCulture),
                High = double.Parse(item[2].GetString(), CultureInfo.InvariantCulture),
                Low = double.Parse(item[3].GetString(), CultureInfo.InvariantCulture),
                Close = double.Parse(item[4].GetString(), CultureInfo.InvariantCulture),
                Volume = double.Parse(item[5].GetString(), CultureInfo.InvariantCulture),
                CloseTime = item[6].GetInt64()
            }).ToList();
        }

        private static double CalculateEMA(List<Kline> klines, int period)
        {
            if (klines.Count < period) return 0;


            var multiplier = 2.0 / (period + 1);
            double ema = klines.Take(period).Average(k => k.Close);


            foreach (var kline in klines.Skip(period))
            {
                ema = (kline.Close - ema) * multiplier + ema;
            }


            return ema;
        }
        private static double CalculateMACD(List<Kline> klines)
        {
            if (klines.Count < 26) return 0;
            return CalculateEMA(klines, 12) - CalculateEMA(klines, 26);
        }


        private static double CalculateRSI(List<Kline> klines, int period)
        {
            if (klines.Count <= period) return 0;


            double gains = 0, losses = 0;


            for (int i = 1; i <= period; i++)
            {
                var change = klines[i].Close - klines[i - 1].Close;
                if (change > 0) gains += change;
                else losses -= change;
            }


            double avgGain = gains / period;
            double avgLoss = losses / period;


            for (int i = period + 1; i < klines.Count; i++)
            {
                var change = klines[i].Close - klines[i - 1].Close;
                if (change > 0)
                {
                    avgGain = ((avgGain * (period - 1)) + change) / period;
                    avgLoss = (avgLoss * (period - 1)) / period;
                }
                else
                {
                    avgGain = (avgGain * (period - 1)) / period;
                    avgLoss = ((avgLoss * (period - 1)) + -change) / period;
                }
            }


            if (avgLoss == 0) return 100;
            double rs = avgGain / avgLoss;
            return 100 - (100 / (1 + rs));
        }


        private static async Task<OIData> GetOpenInterestData(string symbol)
        {
            var url = $"https://fapi.binance.com/fapi/v1/openInterest?symbol={symbol}";
            var response = await httpClient.GetStringAsync(url);
            var json = JsonSerializer.Deserialize<Dictionary<string, object>>(response);


            double latest = ParseDouble(json["openInterest"]);
            return new OIData
            {
                Latest = latest,
                Average = latest * 0.999
            };
        }


        private static async Task<double> GetFundingRate(string symbol)
        {
            var url = $"https://fapi.binance.com/fapi/v1/premiumIndex?symbol={symbol}";
            var response = await httpClient.GetStringAsync(url);
            var json = JsonSerializer.Deserialize<Dictionary<string, object>>(response);


            return ParseDouble(json["lastFundingRate"]);
        }


        private static string Normalize(string symbol)
        {
            symbol = symbol.ToUpperInvariant();
            return symbol.EndsWith("USDT") ? symbol : symbol + "USDT";
        }

        private static double ParseDouble(object val)
        {
            if (val is JsonElement je)
            {
                if (je.ValueKind == JsonValueKind.String)
                    return double.Parse(je.GetString(), CultureInfo.InvariantCulture);
                if (je.ValueKind == JsonValueKind.Number)
                    return je.GetDouble();
            }
            return Convert.ToDouble(val, CultureInfo.InvariantCulture);
        }
    }
}
