using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyazDuru.Samples.AI.Chat.Console
{
    internal class ChatService : BackgroundService
    {
        private readonly IChatClient _chatClient;
        private readonly ILogger<ChatService> _logger;
        public ChatService(IChatClient chatClient, ILogger<ChatService> logger)
        {
            _chatClient = chatClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<ChatMessage> history = [];
            while (!stoppingToken.IsCancellationRequested)
            {
                System.Console.Write("Q: ");
                history.Add(new(ChatRole.User, System.Console.ReadLine()));

                ChatResponse response = await _chatClient.GetResponseAsync(history);
                System.Console.WriteLine(response);
                _logger.LogWarning(response.ToString());
                history.AddMessages(response);
            }
        }
    }
}
