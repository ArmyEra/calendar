using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class ThreadManager: MonoBehaviour
    {
        private static readonly List<Action> executeOnMainThread = new List<Action>();
        private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
        private static bool _actionToExecuteOnMainThread = false;

        private void Update()
        {
            UpdateMain();
        }

        public static void ExecuteOnMainThread([CanBeNull] Action action)
        {
            if (action == null)
            {
                Debug.Log("No action to execute on main thread!");
                return;
            }

            lock (executeOnMainThread)
            {
                executeOnMainThread.Add(action);
                _actionToExecuteOnMainThread = true;
            }
        }
        
        private void UpdateMain()
        {
            if(!_actionToExecuteOnMainThread)
                return;

            lock (executeOnMainThread)
            {
                executeCopiedOnMainThread.AddRange(executeOnMainThread);
                executeOnMainThread.Clear();
                _actionToExecuteOnMainThread = false;
            }

            foreach (var action in executeCopiedOnMainThread)
                action.Invoke();
            
            executeCopiedOnMainThread.Clear();
        }
    }
}