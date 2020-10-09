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


        #region RESOURCES

        /// <summary>
        /// Каталог Resources
        /// </summary>
        public static readonly string ResourcesFolder = $"{Application.dataPath}\\Resources";

        /// <summary>
        /// Путь к дефолтным звукам в Resources
        /// </summary>
        public const string DEFAULT_SOUNDS_SUB_PATH = "Audio\\Default";
        
        /// <summary>
        /// Каталог сохранения дефолтных звуков
        /// </summary>
        public static readonly string DefaultSoundsGenerateFolder = $"{ResourcesFolder}\\{DEFAULT_SOUNDS_SUB_PATH}";
        
        /// <summary>
        /// Каталог звука конкретного праздника
        /// </summary>
        public static string HolidaySoundPath(string sourceId) => $"{ResourcesFolder}\\Audio\\{sourceId}";

        #endregion
        
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