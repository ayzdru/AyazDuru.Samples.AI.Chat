using EchoSharp.NAudio;
using EchoSharp.Onnx.SileroVad;
using EchoSharp.SpeechTranscription;
using EchoSharp.VoiceActivityDetection;
using EchoSharp.Whisper.net;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Whisper.net;

namespace AyazDuru.Samples.AI.Chat.Console
{
    public class SpeechToTextService
    {
        private readonly IVadDetectorFactory vadDetectorFactory;
        private readonly ISpeechTranscriptorFactory speechTranscriptorFactory;
        private readonly MicrophoneInputSource micAudioSource;
        private readonly IRealtimeSpeechTranscriptorFactory realTimeFactory;
        private readonly IRealtimeSpeechTranscriptor realTimeTranscriptor;
        private string _text;

        public string Text { get => _text; set => _text = value; }
        public SpeechToTextService()
        {
            vadDetectorFactory = new SileroVadDetectorFactory(
                new SileroVadOptions(
                    Path.Combine(AppContext.BaseDirectory, Program.SileroModelFileName)
                )
                {
                    Threshold = 0.5f,
                    ThresholdGap = 0.15f,
                }
            );
          
            speechTranscriptorFactory = new WhisperSpeechTranscriptorFactory(Program.WhisperModelFileName);
            micAudioSource = new MicrophoneInputSource(deviceNumber: 0);

            realTimeFactory = new EchoSharpRealtimeTranscriptorFactory(
                speechTranscriptorFactory,
                vadDetectorFactory,
                echoSharpOptions:
                new EchoSharpRealtimeOptions
                {
                    ConcatenateSegmentsToPrompt = false
                }
            );

            realTimeTranscriptor = realTimeFactory.Create(new RealtimeSpeechTranscriptorOptions
            {
                AutodetectLanguageOnce = false,
                IncludeSpeechRecogizingEvents = true,
                RetrieveTokenDetails = true,
                LanguageAutoDetect = false,
                Language = new CultureInfo("en-US"),
            });
        }

        public async Task StartMicrophoneRecording()
        {
            micAudioSource.StartRecording();
            System.Console.WriteLine("Konuşabilirsiniz, herhangi bir tuşa bastığınız mikrofon kaydı duracaktır...");

            var transcriptionTask = ShowTranscriptAsync();

            System.Console.ReadKey();
            micAudioSource.StopRecording();
            
        }
       

        public async Task ShowTranscriptAsync()
        {
            Text = null;
            await foreach (var transcription in realTimeTranscriptor.TranscribeAsync(micAudioSource))
            {
                var eventType = transcription.GetType().Name;

                if (transcription is RealtimeSegmentRecognized segmentRecognized)
                {
                    if (segmentRecognized.Segment.Text.TrimStart().StartsWith("[") == false)
                    {
                        Text += segmentRecognized.Segment.Text.TrimStart().TrimEnd() + "\n";
                        System.Console.WriteLine(segmentRecognized.Segment.Text);
                    }
                }
            }
        }
    }
}
