using System.Linq;
using Audio.ClipQueue;
using Audio.Extensions;
using Audio.FlowChart.Utils;
using TreeModule;

namespace Audio.FlowChart.Model
{
    /// <summary>
    /// Узел графа аудио-менеджера
    /// </summary>
    public class AudioFlowChartNode: TreeNode<ClipQueueCollection>
    {
        private readonly AudioFlowChartStates _state;

        public AudioFlowChartNode(TreeRootNode<ClipQueueCollection> parent, AudioFlowChartStates state) 
            : base(parent)
        {
            _state = state;
            Value = new ClipQueueCollection();
        }

        /// <summary>
        /// Утсанавливает новые клипы в очередь узлов графов 
        /// </summary>
        private void SetNewClips(params IClipQueueInfo[] clipQueueInfos)
        {
            if(clipQueueInfos.Length == 0)
                return;
            
            if(Value == null)
                Value = new ClipQueueCollection();

            switch (_state)
            {
                case AudioFlowChartStates.HolidayNotification:
                case AudioFlowChartStates.Other:
                    Value.Enqueue(clipQueueInfos);
                    break;
                case AudioFlowChartStates.Null:
                    break;
                default:
                {
                    if (Value.IsEmpty)
                        Value.Enqueue(clipQueueInfos[0]);
                    break;
                }
            }
        }

        /// <summary>
        /// Сбрасывает все текущие клипы из очереди
        /// </summary>
        /// <param name="args">[0] - deep</param>
        public static void ClearClips(TreeRootNode<ClipQueueCollection> clipQueueCollectionNode, params object[] args)
        {
            if (clipQueueCollectionNode is AudioFlowChartNode treeNode)
                treeNode.Value.Clear();
        }
        
        /// <summary>
        /// Устанаваливает новые клипы в очередь
        /// </summary>
        /// <param name="args">[0] - deep; [1] - IClipQueueInfo[]</param>
        public static void SetNewClips(TreeRootNode<ClipQueueCollection> clipQueueCollectionNode, params object[] args)
        {
            if (!(clipQueueCollectionNode is AudioFlowChartNode treeNode)) 
                return;

            var current = treeNode.Container.Current;
            if(!current.IsRoot && ((AudioFlowChartNode)current).IsChildNodeOf(treeNode))
                return;
            
            var newClipInfos = ((IClipQueueInfo[]) args[1])
                .Where(cqi => cqi.FlowChartState == treeNode._state)
                .ToArray();
            
            treeNode.SetNewClips(newClipInfos);
        }
    }
}