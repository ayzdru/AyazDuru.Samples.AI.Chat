using Microsoft.Extensions.AI;
using OllamaSharp;
namespace AyazDuru.Samples.AI.Chat.Console
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IChatClient client = new OllamaApiClient(
    new Uri("http://localhost:11434/"), "llama3.3");


            List<ChatMessage> history = [];
            while (true)
            {
                System.Console.Write("Q: ");
                history.Add(new(ChatRole.User, System.Console.ReadLine()));

                ChatResponse response = await client.GetResponseAsync(history);
                System.Console.WriteLine(response);

                history.AddMessages(response);
            }
        }
    }
}
