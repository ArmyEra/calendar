using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Audio.Utils;
using Calendar.InfoPanel.Utils;
using Core;
using Data.Calendar;
using OrderExecuter;
using SpeechKitApi.Enums;
using SpeechKitApi.Models;
using SpeechKitApi.Utils;
using SpeechKitApi.Wav;
using UnityAsyncHelper.Core;
using UnityEngine;
using UnityEngine.Networking;
using Utils;
using YandexSpeechKit;
using YandexSpeechKit.Utils;
using EventType = Core.EventType;
using Params = Audio.Utils.Params;

namespace Audio
{
    public class SoundManger : Singleton<SoundManger>, IStartable
    {
        private bool _awaitDownload = true;
        private bool _componentsInitialized;
        private WaitUntil _anyAwaitGeneration;
        
        public void OnStart()
        {
            EventManager.AddHandler(EventType.YandexClientCreated, SpeechKitClientCallback);
            _anyAwaitGeneration = new WaitUntil(()=> _defaultAwaitGeneration.Count > 0);
            
            var defaultTexts = Enum.GetValues(typeof(DefaultSoundType))
                .Cast<DefaultSoundType>()
                .Select(v => v.GetEnumString());
            DownloadDefaultSounds(defaultTexts, Params.DefaultSoundsGenerateFolder);
            
            _componentsInitialized = true;
            Debug.Log("Sound manager loaded all, that could");
        }

        public void OnDestroy()
        {
            EventManager.RemoveHandler(EventType.YandexClientCreated, SpeechKitClientCallback);
            _awaitDownload = false;
        }

        private void SpeechKitClientCallback(params object[] args)
        {
            StartCoroutine(DownloadDefaultSoundsFromInternet());
        }

        #region Default Sounds
        public readonly Dictionary<string, AudioClip> DefaultSounds = new Dictionary<string, AudioClip>();
        private readonly List<string> _defaultAwaitGeneration = new List<string>();

        /// <param name="onStart">TRUE: загружает на старте. Если нет, то кеширует имя\n
        /// FALSE: Загружает с диска и удаляет имя из кеша</param>
        private void DownloadDefaultSounds(IEnumerable<string> texts, string path, bool onStart = true)
        {
            foreach (var text in texts)
            {
                var fullPath = $"{Path.Combine(path, text.GetValidPathString())}.wav";
                if (File.Exists(fullPath))
                {
                    if (DefaultSounds.ContainsKey(text))
                        continue;
                    void SaveToDictionary(AudioClip clip)
                    {
                        if(DefaultSounds.ContainsKey(text))
                            return;
                        DefaultSounds.Add(text, clip);
                        Debug.Log($"Clip uploaded: \"{text}\"");

                        if (!onStart)
                        {
                            _defaultAwaitGeneration.Remove(text);
                            Debug.Log($"Remove from cash: \"{text}\"");
                        }
                    }
                    StartCoroutine(LoadClip(fullPath, SaveToDictionary));
                }
                else
                {
                    if (onStart)
                    {
                        _defaultAwaitGeneration.Add(text);
                        Debug.Log($"Cash text: \"{text}\"");
                    }
                }
            }
        }

        private IEnumerator DownloadDefaultSoundsFromInternet()
        {
            yield return new WaitUntil(() => _componentsInitialized);

            while (_awaitDownload)
            {
                yield return _anyAwaitGeneration;
                
                var externalOptions = new SynthesisExternalOptions
                {
                    Emotion = Emotion.Good,
                    Language = SynthesisLanguage.Russian,
                    Quality = SynthesisQuality.High,
                    Speaker = Speaker.Oksana,
                    AudioFormat = SynthesisAudioFormat.Lpcm
                };

                void CallBackDownload()
                    => DownloadDefaultSounds(_defaultAwaitGeneration, Params.DefaultSoundsGenerateFolder, false);

                ThreadManager.AsyncExecute(
                    GenerateSounds,
                    CallBackDownload,
                    _defaultAwaitGeneration,
                    externalOptions,
                    Params.DefaultSoundsGenerateFolder);
            }
        }
        
        #endregion

        #region STATIC EVENTS
        
        /// <summary>
        /// Генерирует звук и сохраняет в файл 
        /// </summary>
        public static void GenerateSounds(params object[] args)
        {
            var texts = (IEnumerable<string>) args[0];
            var externalOptions = (SynthesisExternalOptions) args[1];
            var path = (string) args[2];
            
            var optionsArray = SynthesisOptions.Create(
                texts,
                externalOptions,
                ClientParams.YandexCloudFolderId
            ).ToArray();
            
            var dataArray = SpeechKitManager.Client.GetMultipleSpeech(optionsArray).GetAwaiter().GetResult();
            for(var i = 0; i < dataArray.Length; i++)
                WavConverter.Convert(in dataArray[i], in optionsArray[i], path);
        }
        
        /// <summary>
        /// Дополняет в событие звук 
        /// </summary>
        public static void GenerateEventDataSound(CalendarEventData eventData)
        {
            switch (eventData.calendarEventType)
            {
                case CalendarEventTypes.Null:
                case CalendarEventTypes.Notes:
                    return;
            }  
            
            if(eventData.clip != null)
                return;

            var filePath = Params.SoundPath(eventData.SourceId);
            if (File.Exists(filePath))
            {
                void SetClip(AudioClip loadedClip)
                {
                    Debug.Log("Clip received!");
                    eventData.clip = loadedClip;
                }

                Instance.StartCoroutine(LoadClip(filePath, SetClip));
                return;
            }
        }

        /// <summary>
        /// Загрузка клипа с диска и передача его в Callback-функцию
        /// </summary>
        private static IEnumerator LoadClip(string filePath, Action<AudioClip> callback)
        {
            using (var www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError || !www.isDone)
                    Debug.Log(www.error);
                else
                {
                    Debug.Log($"Coroutine: clip from path \"{filePath}\" loaded");
                    callback.Invoke(DownloadHandlerAudioClip.GetContent(www));
                }
            }
        }
        
        #endregion
    }
}