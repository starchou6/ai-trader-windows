using AITrade.Consts;
using AITrade.Entity.AI;
using System.IO;
using System.Text.Json;

namespace AITrade.Services
{
    public class DecisionLogger
    {
        private readonly string _dir;

        public DecisionLogger(string dir)
        {
            _dir = dir;
            Directory.CreateDirectory(_dir);
        }

        public void LogDecision(DecisionRecord record)
        {
            var file = Path.Combine(_dir, $"{DateTime.UtcNow:yyyyMMdd_HHmmss_fff}.json");
            var json = JsonSerializer.Serialize(record, CommonConstants.CachedJsonSerializerOptions);
            File.WriteAllText(file, json);
        }

        public object? AnalyzePerformance(int lastNCycles)
        {
            // why: 占位；可读取_lastN 个记录聚合
            return null;
        }
    }
}
