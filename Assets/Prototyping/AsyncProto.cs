using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Prototyping {
    public class AsyncProto : MonoBehaviour {
        private void Start() {
            Task.Run(async () => {
                var stopwatch = new Stopwatch();
                Debug.Log($"Thread: {Thread.CurrentThread.ManagedThreadId}");
                for (var i = 0; i < 10; i++) {
                    stopwatch.Restart();
                    await Delay(100);
                    Debug.Log($"Time: {stopwatch.ElapsedMilliseconds}");
                    Debug.Log($"Thread: {Thread.CurrentThread.ManagedThreadId}");
                }
                Debug.Log($"Thread: {Thread.CurrentThread.ManagedThreadId}");
            });
        }

        private async UniTask Delay(int ms) {
            await Task.Delay(ms);
        }
    }
}