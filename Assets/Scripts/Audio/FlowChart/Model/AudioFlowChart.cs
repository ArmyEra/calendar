using System.Collections.Generic;
using System.Linq;
using Audio.CashedSounds.Default.Utils;
using Audio.ClipQueue;
using Audio.FlowChart.Utils;
using Data.Calendar;
using TreeModule;
using TreeModule.Utils;

namespace Audio.FlowChart.Model
{
    /// <summary>
    /// Граф аудио-менеджера
    /// </summary>
    public class AudioFlowChart: Tree<ClipQueueCollection>
    {
        private int FrameId { get; set; } = -1;
        
        private AudioFlowChart(TrackingDirection trackingDirection) : base(trackingDirection) { }
        
        /// <summary>
        /// Ставит в очередь   
        /// </summary>
        public void PlayQueued(int frameId, params DefaultSoundType[] soundTypes)
        {
            var clipQueueInfos = soundTypes
                .Select(st => new DefaultClipQueueInfo(frameId, st));
            
            UpdateFrame(frameId, clipQueueInfos);
        }

        public void PlayQueued(int frameId, params CalendarEventData[] calendarEvents)
        {
            var clipQueueInfos = calendarEvents
                .Select(ce => new HolidayClipQueueInfo(frameId, ce));
            
            UpdateFrame(frameId, clipQueueInfos);
        }

        private void UpdateFrame(int frameId, IEnumerable<IClipQueueInfo> clipQueueInfos)
        {
            if (FrameId != frameId)
            {
                FrameId = frameId;
                Root.ChildrenInvoke(AudioFlowChartNode.ClearClips, true);   
            }
            Root.ChildrenInvoke(AudioFlowChartNode.SetNewClips,true, clipQueueInfos.ToArray());
        }

        public IClipQueueInfo GetNextInfo()
        {
            if(Current.IsRoot)
        }
        
        /// <summary>
        /// Создает Граф аудио-менеджера
        /// </summary>
        /// <returns>Формат графа
        ///
        ///        |                         |                                             |
        ///        |                         |                                             |
        ///       \|                        \|                                            \|
        ///    Greeting    ---->    DayNotification    ---->    NoteNotification        Other
        ///                                |
        ///                                |
        ///                               \| 
        ///                        HolidayPreview
        ///                                |
        ///                                |
        ///                               \|
        ///                        HolidayNotification 
        /// </returns>
        public static AudioFlowChart Create()
        {
            var flowChart = new AudioFlowChart(TrackingDirection.ToChild);

            var greetingNode = new AudioFlowChartNode(flowChart.Root, AudioFlowChartStates.Greeting);
            var dayNotificationNode = new AudioFlowChartNode(flowChart.Root, AudioFlowChartStates.DayNotification);
            var otherNode = new AudioFlowChartNode(flowChart.Root,AudioFlowChartStates.Other);
            flowChart.Root.AddRange(greetingNode, dayNotificationNode, otherNode);

            var holidayPreview = new AudioFlowChartNode(dayNotificationNode, AudioFlowChartStates.HolidayPreview); 
            var noteNotification = new AudioFlowChartNode(dayNotificationNode, AudioFlowChartStates.NoteNotification); 
            dayNotificationNode.AddRange(holidayPreview, noteNotification);

            var holidayNotification = new AudioFlowChartNode(holidayPreview, AudioFlowChartStates.HolidayNotification);
            holidayPreview.Add(holidayNotification);
            //Поставили петлю в рассчете на то, что обработка не будет переходжить к графам с пустыми очередями
            holidayNotification.Add(dayNotificationNode);

            return flowChart;
        }
    }
}