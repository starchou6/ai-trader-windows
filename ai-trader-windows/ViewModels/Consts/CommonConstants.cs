using System.Text.Encodings.Web;
using System.Text.Json;

namespace AITrade.Consts
{
    public class CommonConstants
    {
        #region language codes
        public static readonly string[] LANGUAGE_LIST =
        [
            LANGUAGE_CN,
            LANGUAGE_EN
        ];
        public const string LANGUAGE_CN = "中文";
        public const string LANGUAGE_EN = "English";
        #endregion

        public static readonly string[] COIN_SYMBOL_LIST =
        [
            "BTCUSDT",
            "ETHUSDT",
            "BNBUSDT",
            "XRPUSDT",
            "SOLUSDT",
            "DOGEUSDT",
        ];

        public static readonly JsonSerializerOptions CachedJsonSerializerOptions = new() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
    }
}
