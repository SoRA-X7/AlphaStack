using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AlphaStack.Game.Util {
    public static class ThreadUtils {
        public static void CheckThread() {
            if (Thread.CurrentThread.ManagedThreadId == 1) {
                Debug.LogWarning("This method is running at the main thread");
            }
        }

        public static async UniTask SwitchToTaskPool() {
            if (Thread.CurrentThread.ManagedThreadId == 1) {
                await UniTask.SwitchToTaskPool();
            }
        }
        
        public static TResult Lock<TSource, TResult>(this TSource obj, Func<TSource, TResult> fn) where TSource : class {
            lock (obj) {
                return fn(obj);
            }
        }
    }
}