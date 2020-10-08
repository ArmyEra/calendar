using OrderExecuter;
using SpeechKitApi;
using SpeechKitApi.Models.TokenResources;
using UnityEngine;
using YandexSpeechKit.Utils;

namespace YandexSpeechKit
{
    //ToDo: Вызов по очереди
    //ToDo: Создание записей, которых нет
    public class SpeechKitManager: MonoBehaviour, IStartable
    {
        private static SpeechKitClient _client;
        
        public void OnStart()
        {
            var _client = SpeechKitClient.Create(new OAuthToken {Key = ClientParams.OAuthKey});
        }
        
        private void OnDestroy()
        {
            _client.Dispose();
        }
    }
}