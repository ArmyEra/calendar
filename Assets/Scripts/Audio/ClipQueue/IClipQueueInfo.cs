using System;
using System.Collections;
using UnityEngine;

namespace Audio.ClipQueue
{
    public interface IClipQueueInfo: IDisposable
    {
        AudioClip Clip { get; }
        
        bool ClipLoaded { get; }

        AudioClip CheckClip(out bool success);
    }
}