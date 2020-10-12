using Audio.FlowChart.Utils;
using TreeModule;
using TreeModule.Utils;

namespace Audio.FlowChart
{
    /// <summary>
    /// Граф аудио-менеджера
    /// </summary>
    public class AudioFlowChart: Tree<AudioFlowChartInfo>
    {
        private AudioFlowChart(TrackingDirection trackingDirection) : base(trackingDirection) { }
        

        public static AudioFlowChart Create()
        {
            var flowChart = new AudioFlowChart(TrackingDirection.ToChild);
            
            flowChart.Root.Add(new AudioFlowChartInfo(AudioFlowChartStates.Greeting));
            var dayNotificationNode = flowChart.Root.Add(new AudioFlowChartInfo(AudioFlowChartStates.DayNotification));
            flowChart.Root.Add(new AudioFlowChartInfo(AudioFlowChartStates.Other));
            
            var holidayPreview = dayNotificationNode.Add(new AudioFlowChartInfo(AudioFlowChartStates.HolidayPreview));
            dayNotificationNode.Add(new AudioFlowChartInfo(AudioFlowChartStates.NoteNotification));
            
            holidayPreview.Add(new AudioFlowChartInfo(AudioFlowChartStates.HolidayNotification));

            return flowChart;
        }
    }
}