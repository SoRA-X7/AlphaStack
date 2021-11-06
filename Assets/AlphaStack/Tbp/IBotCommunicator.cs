using System;
using Cysharp.Threading.Tasks;

namespace AlphaStack.Tbp {
    public interface IBotCommunicator : IDisposable {
        bool Launch();
        
        UniTask<string> Receive();

        void Send(byte[] json);

        void Kill();
    }
}