using Azure;
using Azure.AI.OpenAI;
using AzureOpenAiChatClient.Data;

namespace AzureOpenAiChatClient.Services
{
    public class ChatService
    {
        private readonly string _openAiUrl, _openAiKey, _aiDeploymentModel, _searchEndPoint, _indexName, _dataApiKey;
        private const string ChatContext = "@\"You are a sales assistant who works for the ABP.IO Platform and your name is Scott. The commercial ABP.IO Platform website is https://commercial.abp.io, https://commercial.abp.io/faq. You help with licensing questions, sales questions and other questions related to ABP Commercial Licenses. You write in a friendly yet professional tone but can tailor your writing style that best works for a user-specified audience. If you do not know the answer to a question, respond by saying 'I do not know the answer, I can answer questions about ABP.IO Platform'";

        public ChatService(IConfiguration configuration)
        {
            var azureConfig = configuration.GetSection("Azure-Open-AI");
            _openAiUrl = azureConfig["Url"]!;
            _openAiKey = azureConfig["Key"]!;
            _aiDeploymentModel = azureConfig["DeploymentModel"]!;
            _searchEndPoint = azureConfig["SearchEndPoint"]!;
            _indexName = azureConfig["IndexName"]!;
            _dataApiKey = azureConfig["DataApiKey"]!;
        }

        public async Task<Message> GetResponse(List<Message> messages)
        {
            try
            {
                var client = new OpenAIClient(new Uri(_openAiUrl), new AzureKeyCredential(_openAiKey));
                var options = CreateOptions(_aiDeploymentModel, messages);
                var completionResponse = await client.GetChatCompletionsAsync(options);
                var completions = completionResponse.Value;
                var messageContent = completions.Choices[0].Message.Content;
                var returnMessage = new Message(messageContent, false);
                return returnMessage;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new Message("ERROR OCCURED!", false);
            }
        }

        private  ChatCompletionsOptions CreateOptions(string deploymentModel, List<Message> messages)
        {
            var options = new ChatCompletionsOptions
            {
                DeploymentName = deploymentModel,
                Temperature = (float)0.7,
                MaxTokens = 800,
                NucleusSamplingFactor = (float)0.95,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
                Messages =
                {
                      new ChatRequestSystemMessage(ChatContext),
                      new ChatRequestUserMessage("What's the main difference between the Team License and the Business License?" )
                },
                AzureExtensionsOptions = new AzureChatExtensionsOptions()
                {
                    Extensions =
                    {
                        new AzureSearchChatExtensionConfiguration()
                        {
                            SearchEndpoint = new Uri(_searchEndPoint),
                            IndexName = _indexName,
                            Authentication = new OnYourDataApiKeyAuthenticationOptions(_dataApiKey)
                        }
                    }
                }
            };

            AddMessages(messages, options);
            return options;
        }

        private static void AddMessages(List<Message> messages, ChatCompletionsOptions options)
        {
            foreach (var msg in messages)
            {
                options.Messages.Add(msg.IsRequest
                    ? new ChatRequestUserMessage(msg.Body)
                    : new ChatRequestAssistantMessage(msg.Body));
            }
        }
    }
}
