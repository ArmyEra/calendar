using SpeechKitApi.Enums;
using SpeechKitApi.Models;
using UnityEngine;

namespace Audio.Utils
{
    public static class Params
    {
        /// <summary>
        /// Время ожидания звука в очереди
        /// </summary>
        public const float TIME_SOUND_AWAIT = 3f;
        
        /// <summary>
        /// Общий каталог сохранения звука
        /// </summary>
        public static readonly string SoundGenerateFolder = $"{Application.dataPath}\\Audio";

        /// <summary>
        /// Каталог сохранения дефолтных звуков
        /// </summary>
        public static readonly string DefaultSoundsGenerateFolder = $"{SoundGenerateFolder}\\Default";
        
        /// <summary>
        /// Каталог звука конкретного события 
        /// </summary>
        public static string SoundPath(string sourceId) => $"{SoundGenerateFolder}\\{sourceId}";

        public static readonly SynthesisExternalOptions DefaultExternalOptions = new SynthesisExternalOptions
        {
            Emotion = Emotion.Good,
            Language = SynthesisLanguage.Russian,
            Quality = SynthesisQuality.High,
            Speaker = Speaker.Oksana,
            AudioFormat = SynthesisAudioFormat.Lpcm
        };
    }
}