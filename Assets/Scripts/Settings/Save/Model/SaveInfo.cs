using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Settings.Save.Model
{
    [Serializable]
    public class SaveInfo
    {
        [JsonRequired]
        public List<NoteInfo> notes;

        public static SaveInfo New => new SaveInfo
        {
            notes = new List<NoteInfo>()
        };
    }
}