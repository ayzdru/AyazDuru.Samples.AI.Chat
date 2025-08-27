using KokoroSharp;
using KokoroSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyazDuru.Samples.AI.Chat.Console
{
    public class TextToSpeechService
    {
        KokoroTTS tts;
        KokoroVoice heartVoice;

        public TextToSpeechService()
        {           
            tts = KokoroTTS.LoadModel();
            heartVoice = KokoroVoiceManager.GetVoice("af_heart");
        }

        public void Speak(string text)
        {
            tts.SpeakFast(text, heartVoice, new KokoroSharp.Processing.KokoroTTSPipelineConfig() { Speed = 1.2f});
        }
    }
}
