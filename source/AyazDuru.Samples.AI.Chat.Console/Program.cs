using EchoSharp.Whisper.net;
using KokoroSharp;
using KokoroSharp.Core;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using OllamaSharp;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Speech.Synthesis;
using Whisper.net;
using Whisper.net.Ggml;
using Whisper.net.Logger;
namespace AyazDuru.Samples.AI.Chat.Console
{
    public class Program
    {
        public const GgmlType WhisperGgmlType = GgmlType.Base;
        public const string WhisperModelFileName = "ggml-base.bin";
        public const string SileroModelFileName = "silero_vad.onnx";
        public const string SileroModelUrl = "https://github.com/snakers4/silero-vad/raw/master/src/silero_vad/data/silero_vad.onnx";

        private static async Task WhisperDownloadModel(string fileName, GgmlType ggmlType)
        {
            System.Console.WriteLine($"Downloading Whisper Model {fileName}");
            using var modelStream = await WhisperGgmlDownloader.Default.GetGgmlModelAsync(ggmlType);
            using var fileWriter = File.OpenWrite(fileName);
            await modelStream.CopyToAsync(fileWriter);
        }
        private static async Task SileroDownloadModel(string fileName, string modelUrl)
        {
            System.Console.WriteLine($"Downloading Silero VAD Model from {modelUrl}");
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(modelUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            using var modelStream = await response.Content.ReadAsStreamAsync();
            using var fileWriter = File.OpenWrite(fileName);
            await modelStream.CopyToAsync(fileWriter);
        }
        

        static async Task Main(string[] args)
        {
            if (!File.Exists(WhisperModelFileName))
            {
                await WhisperDownloadModel(WhisperModelFileName, WhisperGgmlType);
            }

            if (!File.Exists(SileroModelFileName))
            {
                await SileroDownloadModel(SileroModelFileName, SileroModelUrl);
            }

            if(!File.Exists("kokoro.onnx"))
            {
                System.Console.WriteLine($"Downloading Kokoro Text-to-Speech Model kokoro.onnx");
                await KokoroTTS.LoadModelAsync();
                System.Console.WriteLine($"Downloaded Kokoro Text-to-Speech Model kokoro.onnx");
            }

            var builder = Host.CreateApplicationBuilder(args);
            builder.Logging.ClearProviders();
            builder.AddServiceDefaults();
            builder.Services.AddSingleton<TextToSpeechService>();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddChatClient(new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.2"))
                .UseDistributedCache();
            builder.Services.AddHostedService<ChatService>();
            using IHost host = builder.Build();
            host.Run();
        }
    }
}
