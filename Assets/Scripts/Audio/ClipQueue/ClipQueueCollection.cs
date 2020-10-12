using System.Collections.Generic;
using System.Linq;

namespace Audio.ClipQueue
{
    public class ClipQueueCollection
    {
        public bool IsEmpty => _cashedClip == null && !_clipQueue.Any(); 
        
        private IClipQueueInfo _cashedClip;
        private readonly Queue<IClipQueueInfo> _clipQueue = new Queue<IClipQueueInfo>();

        public void Enqueue(params IClipQueueInfo[] clipQueueInfos)
        {
            foreach (var clipQueueInfo in clipQueueInfos)
                _clipQueue.Enqueue(clipQueueInfo);
        }

        public IClipQueueInfo Dequeue()
        {
            return _cashedClip = _clipQueue.Any()
                ? _clipQueue.Dequeue()
                : null;
        }

        public void Clear()
        {
            _clipQueue.Clear();
            if (_cashedClip != null)
            {
                //ToDo: check if we can drop it
            }
        }
    }
}