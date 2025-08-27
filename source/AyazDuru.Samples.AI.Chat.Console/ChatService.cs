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
        private readonly TextToSpeechService _textToSpeechService;
        public ChatService(IChatClient chatClient, TextToSpeechService textToSpeechService)
        {
            _chatClient = chatClient;
            _textToSpeechService = textToSpeechService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<ChatMessage> history = [];
            System.Console.WriteLine("Canlı mikrofondan konuşmak için @ yazıp, enter'a basınız.");
            while (!stoppingToken.IsCancellationRequested)
            {
                System.Console.Write("Q: ");
                history.Add(new(ChatRole.System, "You are an AI assistant. Always give concise, to-the-point answers. Avoid unnecessary details or long explanations. Use plain English and keep responses short.\r\n"));
                var question = System.Console.ReadLine();
                if (question == "@")
                {
                    var speechToText = new SpeechToTextService();
                    var showTranscriptTask = speechToText.ShowTranscriptAsync();
                    var startMicrophoneRecordingTask = speechToText.StartMicrophoneRecording();
                    var firstReady = await Task.WhenAny(startMicrophoneRecordingTask, showTranscriptTask);

                    await firstReady;
                    question = speechToText.Text;
                }
                history.Add(new(ChatRole.User, question));

                ChatResponse response = await _chatClient.GetResponseAsync(history);
                System.Console.WriteLine(response);
                _textToSpeechService.Speak(response.ToString());                
                history.AddMessages(response);
            }
        }
    }
}
