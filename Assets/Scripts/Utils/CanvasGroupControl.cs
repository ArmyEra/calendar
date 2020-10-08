using System;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public class CanvasGroupControl
    {
        public CanvasGroup CanvasGroup;
        [Space] 
        public CanvasGroupSettings ShowSettings;
        public CanvasGroupSettings HideSettings;

        [HideInInspector] 
        public CanvasGroupSettings CashedSettings;
        
        /// <summary>
        /// Активация/Деактивация
        /// </summary>
        public void SetActive(bool value, bool cash = false, bool restore = false)
        {
            if(cash)
                Cash();
            
            CanvasGroup.Set(value 
                ? restore ? CashedSettings : ShowSettings  
                :  HideSettings);
        }

        public void Cash()
            => CashedSettings = new CanvasGroupSettings(CanvasGroup);

        public void Restore()
            => CanvasGroup.Set(CashedSettings);
    }
    
    [Serializable]
    public class CanvasGroupSettings
    {
        public float Alpha;
        public bool Interactable;
        public bool BlockRaycasts;

        public CanvasGroupSettings(float alpha, bool interactable, bool blockRaycasts)
        {
            Alpha = alpha;
            Interactable = interactable;
            BlockRaycasts = blockRaycasts;
        }

        public CanvasGroupSettings(CanvasGroup canvasGroup)
        {
            Alpha = canvasGroup.alpha;
            Interactable = canvasGroup.interactable;
            BlockRaycasts = canvasGroup.blocksRaycasts;
        }
    }
    
    public static partial class Extensions
    {
        public static void Set(this CanvasGroup canvasGroup, params object[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] is float alpha)
                {
                    canvasGroup.alpha = alpha;
                    continue;
                }
                
                if (args[i] is bool value)
                {
                    if (i == 1)
                        canvasGroup.interactable = value;
                    else
                        canvasGroup.blocksRaycasts = value;
                }
            }
        }
        
        public static void Set(this CanvasGroup canvasGroup, in CanvasGroupSettings settings)
        {
            canvasGroup.alpha = settings.Alpha;
            canvasGroup.interactable = settings.Interactable;
            canvasGroup.blocksRaycasts = settings.BlockRaycasts;
        }
    }
}

