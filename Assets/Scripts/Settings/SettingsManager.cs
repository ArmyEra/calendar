using OrderExecuter;
using Settings.Save;
using UnityEngine;
using Utils;

namespace Settings
{
    public class SettingsManager: Singleton<SettingsManager>, IStartable
    {
        [SerializeField] private SaveManager saveManger;
        
        public void OnStart()
        {
            saveManger.LoadApplicationData();
        }
    }
}