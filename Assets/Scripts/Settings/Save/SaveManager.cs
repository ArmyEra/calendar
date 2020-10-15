using System.Linq;
using Core;
using Data.Calendar;
using Settings.Save.Model;
using UnityEngine;
using Utils;

namespace Settings.Save
{
    public class SaveManager: Singleton<SaveManager>
    {
        [Header("Ключи хранения данных")]
        [SerializeField] private string saveInfoKey;

        [Header("Контейнер заметок")] 
        [SerializeField] private CalendarEventsContainer scheduledEventContainer;

        /// <summary>
        /// Информация, подлежащая сохранениею в PlayerPrefs
        /// </summary>
        public SaveInfo SaveInfo { get; private set; }
        
        /// <summary>
        /// Загружкает данные
        /// </summary>
        public void LoadApplicationData()
        {
            SaveInfo = DataManager.Load<SaveInfo>(saveInfoKey) ?? SaveInfo.New;
            var savedNotes = SaveInfo.notes;
            foreach (var note in savedNotes)
                SaveToContainer(note);
        }

        /// <summary>
        /// Сохраняет заметку в ScriptableObject, если она уникальна
        /// </summary>
        private void SaveToContainer(NoteInfo noteInfo)
        {
            var newNoteEvent = CalendarEventData.NewNote(noteInfo);
            SaveToContainer(newNoteEvent);
        }

        /// <summary>
        /// Сохраняет заметку в ScriptableObject, если она уникальна
        /// </summary>
        public bool SaveToContainer(CalendarEventData newNoteEvent)
        {
            if(scheduledEventContainer.datas.Any(newNoteEvent.Equals))
                return false;
            
            scheduledEventContainer.AddNew(newNoteEvent);
            return true;
        }

        /// <summary>
        /// Сохранение в PlayerPrefs
        /// </summary>
        public void SaveToPrefs()
        {
            DataManager.Save(SaveInfo, saveInfoKey);
        }
    }
}