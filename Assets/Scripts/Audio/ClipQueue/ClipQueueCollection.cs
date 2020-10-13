using System.Collections.Generic;
using System.Linq;
using Audio.Utils;

namespace Audio.ClipQueue
{
    /// <summary>
    /// Класс коллекция клипов
    /// </summary>
    public class ClipQueueCollection
    {
        /// <summary>
        /// Флаг, является ли текущая коллекция пустой
        /// </summary>
        public bool IsEmpty => (CashedClip == null || CashedClip.IsDisposed) && !_clipQueue.Any(); 
        
        /// <summary>
        /// Кешированный клип (выбрается из очереди)
        /// </summary>
        public IClipQueueInfo CashedClip { get; private set; }
        private readonly Queue<IClipQueueInfo> _clipQueue = new Queue<IClipQueueInfo>();

        /// <summary>
        /// Добавляет в очередь новые клипы 
        /// </summary>
        public void Enqueue(params IClipQueueInfo[] clipQueueInfos)
        {
            foreach (var clipQueueInfo in clipQueueInfos)
                _clipQueue.Enqueue(clipQueueInfo);
        }

        /// <summary>
        /// Берет кешированный клип (или устанавливает его из очереди и возвращает)
        /// </summary>
        public IClipQueueInfo Dequeue()
        {
            if (CashedClip == null || CashedClip.IsDisposed)
                return CashedClip = _clipQueue.Any()
                    ? _clipQueue.Dequeue()
                    : null;

            return CashedClip;
        }

        /// <summary>
        /// Очищает всю очередь и кешированный клип по возможности
        /// </summary>
        public void Clear()
        {
            _clipQueue.Clear();
            if (CashedClip == null) 
                return;
            if (CashedClip.ClipState == QueuedClipStates.Playing)
                return;
            DisposeCash();
        }

        /// <summary>
        /// очищает кеш
        /// </summary>
        private void DisposeCash()
        {
            CashedClip.Dispose();
            CashedClip = null;
        }
    }
}