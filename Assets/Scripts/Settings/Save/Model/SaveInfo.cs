using System;
using Newtonsoft.Json;

namespace Settings.Save.Model
{
    [Serializable]
    public class SaveInfo
    {
        [JsonRequired]
        public NoteInfo[] notes;

        public static SaveInfo New => new SaveInfo
        {
            notes = new NoteInfo[0]
        };
    }
}