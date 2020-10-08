using UnityEngine;

namespace Audio.Utils
{
    public static class Params
    {
        public static readonly string SoundGenerateFolder = $"{Application.dataPath}\\Audio";

        public static readonly string DefaultSoundsGenerateFolder = $"{SoundGenerateFolder}\\Default";
        public static string SoundPath(string sourceId) => $"{SoundGenerateFolder}\\{sourceId}";

    }
}