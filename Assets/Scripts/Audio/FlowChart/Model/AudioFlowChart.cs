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

        private bool _chartIsEmpty = true;
        
        private AudioFlowChart(TrackingDirection trackingDirection) : base(trackingDirection) { }
        
        /// <summary>
        /// Ставит в очередь дефолтные клипы
        /// </summary>
        public void PlayQueued(int frameId, params DefaultSoundType[] soundTypes)
        {
            var clipQueueInfos = soundTypes
                .Select(st => new DefaultClipQueueInfo(st));
            
            UpdateFrame(frameId, clipQueueInfos);
        }

        /// <summary>
        /// Ставит в очередь клипы календарных событий 
        /// </summary>
        public void PlayQueued(int frameId, params CalendarEventData[] calendarEvents)
        {   
            var clipQueueInfos = calendarEvents
                .Select(ce => new HolidayClipQueueInfo(ce));
            
            UpdateFrame(frameId, clipQueueInfos);
        }

        /// <summary>
        /// Проверят, есть ли какая-то информация для восрпоизведения 
        /// </summary>
        public bool HasAnyInfo()
        {
            if (_chartIsEmpty)
                return false;

            var currentQueuedClip = GetNextInfo();
            return !(_chartIsEmpty = currentQueuedClip == null);
        }
        
        /// <summary>
        /// Обновляет граф сообщениями текущего кадра 
        /// </summary>
        private void UpdateFrame(int frameId, IEnumerable<IClipQueueInfo> clipQueueInfos)
        {
            var clipInfosArray = clipQueueInfos.ToArray();
            if (clipInfosArray.Length == 0)
                return;
            
            if (FrameId != frameId)
            {
                FrameId = frameId;
                Root.ChildrenInvoke(AudioFlowChartNode.ClearClips, true);   
            }
            Root.ChildrenInvoke(AudioFlowChartNode.SetNewClips,true, clipInfosArray);
            _chartIsEmpty = false;
        }

        /// <summary>
        /// Обходит дерево в поиске клипа. Если клип не найден, то запускается повторный поиск 
        /// </summary>
        private IClipQueueInfo GetNextInfo(bool finalSearch = false)
        {
            if (Current.IsRoot)
                Current.Children.First.Value.SetCurrent();

            while (Current.Value.IsEmpty)
            {
                var nextNode = ((TreeNode<ClipQueueCollection>) Current).GetNext();
                if (nextNode != null)
                    continue;
                
                Current = Root;
                return finalSearch 
                    ? null 
                    : GetNextInfo(true);
            }

            return Current.Value.Dequeue();
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

            return flowChart;
        }
    }
}