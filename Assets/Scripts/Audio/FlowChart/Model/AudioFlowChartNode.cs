using System.Linq;
using Audio.ClipQueue;
using Audio.FlowChart.Utils;
using TreeModule;
using TreeModule.Utils;
using UnityEngine;

namespace Audio.FlowChart.Model
{
    public class AudioFlowChartNode: TreeNode<ClipQueueCollection>
    {
        private readonly AudioFlowChartStates _state;

        public AudioFlowChartNode(TreeRootNode<ClipQueueCollection> parent, AudioFlowChartStates state) 
            : base(parent)
        {
            _state = state;
            Value = new ClipQueueCollection();
        }

        public AudioFlowChartNode(TreeRootNode<ClipQueueCollection> parent, ClipQueueCollection value, AudioFlowChartStates state) 
            : base(parent, value)
        {
            _state = state;
        }

        public override TreeRootNode<ClipQueueCollection> GetNext(int step = 1, TrackingDirection preferedDirection = TrackingDirection.Null)
        {
            if (step == 0)
                return this;

            step = Mathf.Clamp(step, -1, 1);
            
            if (preferedDirection == TrackingDirection.Null)
                preferedDirection = Container.DefaultTrackingDirection;

            switch (preferedDirection)
            {
                case TrackingDirection.ToBrother:
                    return GetNextBrother(this, step);
                case TrackingDirection.ToChild:
                    return GetNextChild(this, step);
                default: return null;
            }
            
            
            // var nextTreeGenericNode = base.GetNext(step, preferedDirection);
            // switch (nextTreeGenericNode)
            // {
            //     case null:
            //         return null;
            //     case AudioFlowChartNode nextAudioNode:
            //         return !nextAudioNode.Value.IsEmpty 
            //             ? nextAudioNode 
            //             : null;
            //     default:
            //         return null;
            // }
        }

        private void ClearQueue()
        {
            Value.Clear();
        }
        
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
                treeNode.ClearQueue();
        }
        
        /// <summary>
        /// Устанаваливает новые клипы в очередь
        /// </summary>
        /// <param name="args">[0] - deep; [1] - IClipQueueInfo[]</param>
        public static void SetNewClips(TreeRootNode<ClipQueueCollection> clipQueueCollectionNode, params object[] args)
        {
            if (clipQueueCollectionNode is AudioFlowChartNode treeNode)
            {
                var newClipInfos = ((IClipQueueInfo[]) args[1])
                    .Where(cqi => cqi.State == treeNode._state)
                    .ToArray();
            
                treeNode.SetNewClips(newClipInfos);
            }
        }
    }
}