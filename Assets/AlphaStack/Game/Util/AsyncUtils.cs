using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AlphaStack.Game.Util {
    public static class AsyncUtils {
        public static async UniTask Delay(int milliseconds, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested();
            if (milliseconds == 0) return;
            if (Application.platform == RuntimePlatform.WebGLPlayer) {
                await UniTask.Delay(milliseconds);
            } else {
                await Task.Delay(milliseconds);
            }
            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}