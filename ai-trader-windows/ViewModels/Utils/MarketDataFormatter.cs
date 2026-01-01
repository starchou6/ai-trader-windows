using AITrade.Entity.AI;
using System.Globalization;
using System.Text;

namespace AITrade.Utils
{
    public class MarketDataFormatter
    {
        public static string Format(MarketData data)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"current_price = {data.CurrentPrice:F2}, current_ema20 = {data.CurrentEMA20:F3}, current_macd = {data.CurrentMACD:F3}, current_rsi (7 period) = {data.CurrentRSI7:F3}\n");
            sb.AppendLine($"In addition, here is the latest {data.Symbol} open interest and funding rate for perps:\n");
            if (data.OpenInterest != null)
            {
                sb.AppendLine($"Open Interest: Latest: {data.OpenInterest.Latest:F2} Average: {data.OpenInterest.Average:F2}\n");
            }
            sb.AppendLine($"Funding Rate: {data.FundingRate:E2}\n");
            if (data.IntradaySeries != null)
            {
                sb.AppendLine("Intraday series (3‑minute intervals, oldest → latest):\n");
                if (data.IntradaySeries.MidPrices?.Any() == true)
                    sb.AppendLine($"Mid prices: {FormatFloatList(data.IntradaySeries.MidPrices)}\n");
                if (data.IntradaySeries.EMA20Values?.Any() == true)
                    sb.AppendLine($"EMA indicators (20‑period): {FormatFloatList(data.IntradaySeries.EMA20Values)}\n");
                if (data.IntradaySeries.MACDValues?.Any() == true)
                    sb.AppendLine($"MACD indicators: {FormatFloatList(data.IntradaySeries.MACDValues)}\n");
                if (data.IntradaySeries.RSI7Values?.Any() == true)
                    sb.AppendLine($"RSI indicators (7‑Period): {FormatFloatList(data.IntradaySeries.RSI7Values)}\n");
                if (data.IntradaySeries.RSI14Values?.Any() == true)
                    sb.AppendLine($"RSI indicators (14‑Period): {FormatFloatList(data.IntradaySeries.RSI14Values)}\n");
            }
            if (data.LongerTermContext != null)
            {
                sb.AppendLine("Longer‑term context (4‑hour timeframe):\n");
                sb.AppendLine($"20‑Period EMA: {data.LongerTermContext.EMA20:F3} vs. 50‑Period EMA: {data.LongerTermContext.EMA50:F3}\n");
                sb.AppendLine($"3‑Period ATR: {data.LongerTermContext.ATR3:F3} vs. 14‑Period ATR: {data.LongerTermContext.ATR14:F3}\n");
                sb.AppendLine($"Current Volume: {data.LongerTermContext.CurrentVolume:F3} vs. Average Volume: {data.LongerTermContext.AverageVolume:F3}\n");
                if (data.LongerTermContext.MACDValues?.Any() == true)
                    sb.AppendLine($"MACD indicators: {FormatFloatList(data.LongerTermContext.MACDValues)}\n");
                if (data.LongerTermContext.RSI14Values?.Any() == true)
                    sb.AppendLine($"RSI indicators (14‑Period): {FormatFloatList(data.LongerTermContext.RSI14Values)}\n");
            }
            return sb.ToString();
        }

        public static string FormatFloatList(IEnumerable<double> values)
        {
            return "[" + string.Join(", ", values.Select(v => v.ToString("F3", CultureInfo.InvariantCulture))) + "]";
        }
    }
}
