namespace AITrade.Utils
{
    public class CommonUtils
    {
        public static double GetDouble(object value)
        {
            if (TryToDouble(value, out var d)) return d;
            return 0.0;
        }
        public static double GetDouble(Dictionary<string, object> dict, string key)
        {
            if (dict.TryGetValue(key, out var v) && TryToDouble(v, out var d)) return d;
            return 0.0;
        }

        public static string GetString(object value)
        {
            if (value != null) return value.ToString() ?? "";
            return "";
        }

        public static string GetString(Dictionary<string, object> dict, string key)
        {
            if (dict.TryGetValue(key, out var v) && v != null) return v.ToString() ?? "";
            return "";
        }

        public static bool TryToDouble(object? v, out double d)
        {
            switch (v)
            {
                case double dd: d = dd; return true;
                case float ff: d = ff; return true;
                case int ii: d = ii; return true;
                case long ll: d = ll; return true;
                case decimal mm: d = (double)mm; return true;
                case string s when double.TryParse(s, out var x): d = x; return true;
                default: d = 0; return false;
            }
        }

        public static bool TryToLong(object? v, out long d)
        {
            switch (v)
            {
                case long ll: d = ll; return true;
                case int ii: d = ii; return true;
                case string s when long.TryParse(s, out var x): d = x; return true;
                default: d = 0; return false;
            }
        }
    }
}
