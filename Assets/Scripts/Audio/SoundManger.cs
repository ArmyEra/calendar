using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Audio.Utils;
using Calendar.InfoPanel.Utils;
using Data.Calendar;
using OrderExecuter;
using SpeechKitApi.Models;
using SpeechKitApi.Wav;
using UnityEngine;
using UnityEngine.Networking;
using Utils;
using YandexSpeechKit;
using YandexSpeechKit.Utils;

namespace Audio
{
    public class SoundManger : Singleton<SoundManger>, IStartable
    {
        public void OnStart()
        {
            DefaultSoundManager.Instance.Initialize();
        }

        
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
        public static IEnumerator LoadClip(string filePath, Action<AudioClip> callback)
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
    }
}