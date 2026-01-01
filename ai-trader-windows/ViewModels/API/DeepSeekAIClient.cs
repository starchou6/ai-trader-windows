using AITrade.Entity.AI;
using DeepSeek.Core;
using DeepSeek.Core.Models;

namespace AITrade.API
{
    public class DeepSeekAIClient : AIClient
    {
        private readonly string _apiKey;

        private readonly string _model = "deepseek-chat";

        public DeepSeekAIClient(string apiKey)
        {
            _apiKey = apiKey;
        }

        public DeepSeekAIClient(string apiKey, string model)
        {
            _apiKey = apiKey;
            _model = model;
        }

        public async Task<string> CallWithMessages(string systemPrompt, string userPrompt)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return "Deepseek api key cannot be null or empty.";
            }
            if (string.IsNullOrEmpty(systemPrompt) || string.IsNullOrEmpty(userPrompt))
            {
                return "System prompt and user prompt cannot be null or empty.";
            }
            // Create an instance using the apiKey
            var client = new DeepSeekClient(_apiKey);
            // Construct the request body
            var request = new ChatRequest
            {
                Messages = [
                    Message.NewSystemMessage(systemPrompt),
                    Message.NewUserMessage(userPrompt)
                ],
                Model = _model,
            };

            var chatResponse = await client.ChatAsync(request, new CancellationToken());
            if (chatResponse is null)
            {
                System.Diagnostics.Debug.WriteLine(client.ErrorMsg);
            }
            return chatResponse?.Choices.First().Message?.Content;
        }

        public async Task<bool> ValidApiKey()
        {
            var client = new DeepSeekClient(_apiKey);
            var request = new ChatRequest
            {
                Messages = [
                    Message.NewSystemMessage("You are a language translator"),
                    Message.NewUserMessage("Please translate 'They are scared! ' into English!")
                ],
                Model = "deepseek-chat"
            };

            var chatResponse = await client.ChatAsync(request, new CancellationToken());
            return chatResponse is not null;
        }
    }
}
