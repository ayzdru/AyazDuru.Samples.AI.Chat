using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OllamaSharp;
namespace AyazDuru.Samples.AI.Chat.Console
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddChatClient(new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.2"))
                .UseDistributedCache();
            builder.AddServiceDefaults();
            builder.Services.AddHostedService<ChatService>();
            using IHost host = builder.Build();
            host.Run();          
        }
    }
}
