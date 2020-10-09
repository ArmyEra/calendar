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
        private static readonly string ResourcesFolder = $"{Application.dataPath}\\Resources";
        
        /// <summary>
        /// Путь звука конкретного праздника (без расширения)
        /// </summary>
        public static string SoundDirectory => $"{ResourcesFolder}\\Audio";
        
        /// <summary>
        /// Каталог сохранения дефолтных звуков
        /// </summary>
        public static readonly string DefaultSoundsFolder = $"{ResourcesFolder}\\{DEFAULT_SOUNDS_SUB_PATH}";
        
        /// <summary>
        /// Путь к дефолтным звукам в Resources
        /// </summary>
        public const string DEFAULT_SOUNDS_SUB_PATH = "Audio\\Default";
        
        /// <summary>
        /// Путь звука события относительно папки Resources (без расширения)
        /// </summary>
        public static string HolidaySoundSubPath(string sourceId) => $"Audio\\{sourceId}";

        /// <summary>
        /// Полный путь звука конкретного календарного события (без расширения) 
        /// </summary>
        public static string FullHolidaySoundPath(string sourceId) =>
            $"{ResourcesFolder}\\{HolidaySoundSubPath(sourceId)}";
        
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