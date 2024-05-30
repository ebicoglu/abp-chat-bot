//using Azure;
//using Azure.AI.OpenAI;
//using AzureOpenAiChatClient.Data;

//namespace AzureOpenAiChatClient.Services
//{
//    public class ChatService
//    {
//        private readonly string _openAiUrl, _openAiKey, _aiDeploymentModel;
//        private const string ChatContext = "@\"You are both a software developer and a sales assistant who works for the ABP.IO Platform and your name is Scott. The commercial ABP.IO Platform website is https://commercial.abp.io, https://commercial.abp.io/faq, https://support.abp.io/QA/Questions, https://docs.abp.io/en/abp/latest, https://docs.abp.io/en/commercial/latest. You help come up with the technical questions, licensing questions, sales questions and other questions related to ABP Framework and ABP Commercial Licenses. You write in a friendly yet professional tone but can tailor your writing style that best works for a user-specified audience. If you do not know the answer to a question, respond by saying 'I do not know the answer to your question. I can answer questions about ABP.IO Platform'";

//        public ChatService(IConfiguration configuration)
//        {
//            var azureConfig = configuration.GetSection("Azure-Open-AI");
//            _openAiUrl = azureConfig["Url"]!;
//            _openAiKey = azureConfig["Key"]!;
//            _aiDeploymentModel = azureConfig["DeploymentModel"]!;
//        }

//        public async Task<Message> GetResponse(List<Message> messages)
//        {
//            var client = new OpenAIClient(new Uri(_openAiUrl), new AzureKeyCredential(_openAiKey));

//            var options = new ChatCompletionsOptions
//            {
//                Temperature = (float)0.7,
//                MaxTokens = 800,
//                NucleusSamplingFactor = (float)0.95,
//                FrequencyPenalty = 0,
//                PresencePenalty = 0,
//                Messages = { new ChatMessage(ChatRole.System, ChatContext) }
//            };

//            foreach (var msg in messages)
//            {
//                if (msg.IsRequest)
//                {
//                    options.Messages.Add(new ChatMessage(ChatRole.User, msg.Body));
//                }
//                else
//                {
//                    options.Messages.Add(new ChatMessage(ChatRole.Assistant, msg.Body));
//                }
//            }

//            var completionResponse = await client.GetChatCompletionsAsync(_aiDeploymentModel, options);
//            var completions = completionResponse.Value;
//            var messageContent = completions.Choices[0].Message.Content;

//            var returnMessage = new Message(messageContent, false);

//            return returnMessage;
//        }

//    }
//}
