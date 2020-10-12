using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Audio.CashedSounds.Default;
using Audio.CashedSounds.Default.Utils;
using Audio.CashedSounds.Holiday;
using Audio.ClipQueue;
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
        [SerializeField] private AudioSource defaultSource;
        
        private readonly Queue<IClipQueueInfo> _clipQueueInfos = new Queue<IClipQueueInfo>();
        private bool _awaitInvoke = true;
        
        public void OnStart()
        {
            DefaultSoundManager.Instance.Initialize();
            HolidaySoundManager.Instance.Initialize();

            StartCoroutine(PlayQueued());
        }
        
        public void OnDestroy()
        {
            _awaitInvoke = false;
            StopAllCoroutines();
            _clipQueueInfos.Clear();
        }

        /// <summary>
        /// Проигрывает из очереди все звуки 
        /// </summary>
        private IEnumerator PlayQueued()
        {
            while (_awaitInvoke)
            {
                yield return new WaitUntil(_clipQueueInfos.Any);
                
                var info = _clipQueueInfos.Dequeue();
                var clip = info.CheckClip(out var success);
                if (!success)
                {
                    yield return new WaitUntil(() => WaitClipLoaded(Time.time, info));
                    clip = info.Clip;
                }

                if (clip != null)
                {
                    var clipLenght = Play(clip);
                    yield return new WaitForSeconds(clipLenght);
                }
                
                info.Dispose();
            }
        }
        
        /// <summary>
        /// Функция флага ожидания загрузки клипа 
        /// </summary>
        private bool WaitClipLoaded(float startTime, IClipQueueInfo info)
        {
            return info.ClipLoaded || Time.time > startTime + Params.TIME_SOUND_AWAIT;
        }

        #region STATIC EVENTS

        public static float Play(AudioClip clip)
        {
            Instance.defaultSource.clip = clip;
            Instance.defaultSource.Play();
            
            return clip.length;
        }

        public static void PlayQueued(params DefaultSoundType[] soundTypes)
        {
            foreach (var soundType in soundTypes)
            {
                var queuedSoundInfo = new DefaultClipQueueInfo(soundType);
                Instance._clipQueueInfos.Enqueue(queuedSoundInfo);    
            }
        }

        public static void PlayQueued(params CalendarEventData[] calendarEvents)
        {
            
            foreach (var calendarEvent in calendarEvents)
            {
                var queuedSoundInfo = new HolidayClipQueueInfo(calendarEvent);
                Instance._clipQueueInfos.Enqueue(queuedSoundInfo);
            }
        }
        
        /// <summary>
        /// Генерирует звук и сохраняет в файл 
        /// </summary>
        public static void GenerateSounds(params object[] args)
        {
            var texts = (string[]) args[0];
            var externalOptions = (SynthesisExternalOptions) args[1];
            var path = (string) args[2];
            var sourceIds = args.Length < 4 ? new string[texts.Length] : (string[]) args[3];
            GeneratingFullSoundPaths(in path, in sourceIds);
            
            var optionsArray = SynthesisOptions.Create(
                texts,
                externalOptions,
                ClientParams.YandexCloudFolderId
            ).ToArray();
            
            var dataArray = SpeechKitManager.Client.GetMultipleSpeech(optionsArray).GetAwaiter().GetResult();
            for(var i = 0; i < dataArray.Length; i++)
                WavConverter.Convert(in dataArray[i], in optionsArray[i], sourceIds[i]);
        }

        /// <summary>
        /// Генерирует массив дирректорий к файлам 
        /// </summary>
        private static void GeneratingFullSoundPaths(in string basePath, in string[] sourceIds)
        {
            for (var i = 0; i < sourceIds.Length; i++)
            {
                sourceIds[i] = string.IsNullOrEmpty(sourceIds[i])
                    ? basePath
                    : Path.Combine(basePath, $"{sourceIds[i]}.wav");
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
                    //Debug.Log($"Coroutine: clip from path \"{filePath}\" loaded");
                    callback.Invoke(DownloadHandlerAudioClip.GetContent(www));
                }
            }
        }

        #endregion
    }
}