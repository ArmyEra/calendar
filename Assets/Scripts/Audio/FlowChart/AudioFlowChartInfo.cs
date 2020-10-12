using Audio.FlowChart.Utils;

namespace Audio.FlowChart
{
    /// <summary>
    /// Узел графа аудио-менеджера
    /// </summary>
    public class AudioFlowChartInfo
    {
        public readonly AudioFlowChartStates State;
        public int FrameId { get; private set; } = -1;

        public AudioFlowChartInfo(AudioFlowChartStates state)
        {
            State = state;
        }
    }
}