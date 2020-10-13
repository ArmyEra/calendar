using Audio.ClipQueue;
using Audio.FlowChart.Utils;
using TreeModule;

namespace Audio.FlowChart.Model
{
    /// <summary>
    /// Корневой узел графа аудио-менеджера
    /// </summary>
    public class AudioFlowChartRootNode: TreeRootNode<ClipQueueCollection>
    {
        public readonly AudioFlowChartStates State = AudioFlowChartStates.Null;
        
        public AudioFlowChartRootNode(Tree<ClipQueueCollection> container) : base(container){ }

        public AudioFlowChartRootNode(Tree<ClipQueueCollection> container, ClipQueueCollection value) : base(container, value){ }
    }
}