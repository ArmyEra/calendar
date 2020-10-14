using Core;
using OrderExecuter;
using SpeechKitApi;
using SpeechKitApi.Models.TokenResources;
using UnityAsyncHelper.Core;
using UnityEngine;
using Utils;
using YandexSpeechKit.Utils;
using EventType = Core.EventType;

namespace YandexSpeechKit
{
    public class SpeechKitManager: Singleton<SpeechKitManager>, IStartable
    {
        public static SpeechKitClient Client { get; private set; }

        [SerializeField] private bool authorize;
        
        public void OnStart()
        {
            if (!authorize)
            {
                print("Authorization turned off!");
                return;
            }

            object[] AuthorizationBegin()
            {
                return new object[]
                {
                    SpeechKitClient.Create(new OAuthToken {Key = ClientParams.OAuthKey})
                };
            }
            
            void InitializeCallback(object[] args)
            {
                Client = (SpeechKitClient) args[0];
                print("Y.SK authorization completed!");
                EventManager.RaiseEvent(EventType.YandexClientCreated);
            }
            
            ThreadManager.AsyncExecute(AuthorizationBegin, InitializeCallback);
        }

        private void OnDestroy()
        {
            Client?.Dispose();
        }
    }
}