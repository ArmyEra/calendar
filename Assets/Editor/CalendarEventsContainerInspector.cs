using System;
using System.Collections.Generic;
using System.Linq;
using Audio.CashedSounds.Holiday;
using Calendar.InfoPanel.Utils;
using Data.Calendar.HelpEditor;
using Extensions;
using UnityAsyncHelper.Core;
using UnityEditor;
using UnityEngine;
using YandexSpeechKit;
using AudioParams = Audio.Utils.Params;

namespace Editor
{
    [CustomEditor(typeof(CalendarEventsSpeechSynthesisTool))]
    public class CalendarEventsContainerInspector : UnityEditor.Editor
    {
        private static float _defaultLabelWidth = 100;
        private const float TEXT_AREA_LABEL_WIDTH = 50;
        
        private static bool _showSpeechMenu = true;
        private static readonly List<CalendarEventSmallInfo> NewSpeechItems = new List<CalendarEventSmallInfo>();

        private CalendarEventsSpeechSynthesisTool _controller;

        private void OnEnable()
        {
            _controller = target as CalendarEventsSpeechSynthesisTool;
        }

        public override void OnInspectorGUI()
        {
            _defaultLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = TEXT_AREA_LABEL_WIDTH;
            
            DrawSpeechMenu();
            EditorGUILayout.Space();
            
            EditorGUIUtility.labelWidth = _defaultLabelWidth;
            
            base.OnInspectorGUI();
            
        }

        /// <summary>
        /// Отрисовывает настройки озвучки
        /// </summary>
        private void DrawSpeechMenu()
        {
            _showSpeechMenu = EditorGUILayout.BeginFoldoutHeaderGroup(_showSpeechMenu,
                _showSpeechMenu ? $"Скрыть меню озвучки" : $"Открыть меню озвучки");

            if (!_showSpeechMenu)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }
            
            var newCount = Mathf.Max(0, EditorGUILayout.IntField("Size", NewSpeechItems.Count));//GUILayout.Width(100)
            while (newCount < NewSpeechItems.Count)
                NewSpeechItems.RemoveAt(NewSpeechItems.Count - 1);
            while (newCount > NewSpeechItems.Count)
                NewSpeechItems.Add(new CalendarEventSmallInfo());
            
            foreach (var newItem in NewSpeechItems)
            {
                EditorGUILayout.BeginHorizontal();
                newItem.stringDate = EditorGUILayout.TextField("Дата:", newItem.stringDate);
                newItem.type = (CalendarEventTypes)EditorGUILayout.EnumPopup("Тип:", newItem.type);
                EditorGUILayout.EndHorizontal();
            }

            GUI.enabled = newCount > 0;
            if (GUILayout.Button("Сгенерировать звуки"))
                GenerateSounds();
            GUI.enabled = true;
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void GenerateSounds()
        {
            // Debug.Log("Not ready!");
            // return;
            
            //Проверка созданного подключения к Yandex.SpeechKit
            if (SpeechKitManager.Client == null)
            {
                Debug.LogAssertion($"Подключение к Яндекс.SpeechKit не осуществлено. Запустите сцену и дождитесь пожключения!");
                return;
            }

            //Выборка валидных указателей на данные из списка для синтеза
            var speechItems = NewSpeechItems.Where(
                it => !string.IsNullOrWhiteSpace(it.stringDate) && it.type.OneOf(CalendarEventTypes.MilitaryMemoryDay, CalendarEventTypes.ScienceDay))
                .ToArray();

            if (speechItems.Length == 0)
            {
                Debug.LogAssertion("Can't select any info to voice. Each of existed has invalid fields!");
                return;
            }
            
            //Создание пар "Указатель"-"Календарное событие"
            var itemInfoPairs = speechItems.Select(
                item => new
                {
                    Item = item,
                    Infos = _controller.dataContainer.datas.Where(d =>
                        d.ThisDayAndMonth(item.Date) && d.calendarEventType == item.type).ToArray()
                });

            //Удаление из списка "Указателей", по которым найденны календарные события
            
            
            var notNullInfoPairs = itemInfoPairs
                .Where(p => p.Infos.Any());
            
            //Календарные события с "Указателей"
            var generateInfos = notNullInfoPairs
                .SelectMany(p => p.Infos)
                .ToArray();
            
            if (generateInfos.Length == 0)
            {
                Debug.Log("Don't have any info to voice!");
                return;
            }

            ThreadManager.ExecuteOnMainThread(
                () => HolidaySoundManager.Instance.DownloadSoundFromInternetOneByOne(in generateInfos));
            foreach (var item in notNullInfoPairs.Select(p => p.Item))
                NewSpeechItems.Remove(item);
        }
    }

    /// <summary>
    /// Сущность "Указатель" на календарное событие
    /// </summary>
    [Serializable]
    public class CalendarEventSmallInfo
    {
        /// <summary>
        /// Строковое представление даты
        /// </summary>
        public string stringDate;
        
        /// <summary>
        /// Тип календарного события
        /// </summary>
        public CalendarEventTypes type = CalendarEventTypes.Null;
        
        /// <summary>
        /// Кешированная дата 
        /// </summary>
        private DateTime _cashedDate = DateTime.MinValue;

        /// <summary>
        /// Дата события
        /// </summary>
        public DateTime Date => _cashedDate.IsNull()
            ? _cashedDate = stringDate.GetDateTime()
            : _cashedDate;

        public override string ToString()
            => $"Date: {stringDate}; Type: {type}";
    }
}
