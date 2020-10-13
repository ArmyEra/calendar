using System;
using Audio.FlowChart.Utils;
using Audio.Utils;
using UnityEngine;

namespace Audio.ClipQueue
{
    public interface IClipQueueInfo: IDisposable
    {
        /// <summary>
        /// Узел звукового менеджера
        /// </summary>
        AudioFlowChartStates FlowChartState { get; }
        
        /// <summary>
        /// Текущие состояние аудио-клипа
        /// </summary>
        QueuedClipStates ClipState { get; }
        
        /// <summary>
        /// Аудио-клип
        /// </summary>
        AudioClip Clip { get; }
        
        /// <summary>
        /// Функция, возращающия аудио-клип, если его возможно проиграть 
        /// </summary>
        AudioClip CheckClip(out bool success);
        
        /// <summary>
        /// Удален ли элемент
        /// </summary>
        bool IsDisposed { get; }
    }
}