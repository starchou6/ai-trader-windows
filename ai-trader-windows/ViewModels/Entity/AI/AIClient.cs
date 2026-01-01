namespace AITrade.Entity.AI
{
    public interface AIClient
    {
        Task<string> CallWithMessages(string systemPrompt, string userPrompt);
    }
}
