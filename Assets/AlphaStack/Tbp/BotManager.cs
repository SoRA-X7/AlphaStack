using System;
using System.Collections.Generic;
using System.Linq;
using AlphaStack.Game;
using AlphaStack.Game.Routing;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utf8Json;
using Debug = UnityEngine.Debug;

namespace AlphaStack.Tbp {
    public class BotManager {
        private readonly IBotCommunicator exe;
        private Field field;

        public BotStatus Status { get; private set; }
        private bool pendingSuggestion;
        private bool quitSent;

        public InfoMessage BotInfo { get; private set; }

        private UniTask runner;

        private MoveFinder moveFinder = new MoveFinder();
        private UniTask moveFindTask;

        private UniTaskCompletionSource<BotMoveResult> moveFound;

        public Queue<string> errors = new Queue<string>();

        public BotManager(IBotCommunicator exe) {
            this.exe = exe;
        }

        public void Launch() {
            quitSent = false;
            Status = BotStatus.Launching;
            if (!exe.Launch()) {
                errors.Enqueue("Failed to launch bot");
                Status = BotStatus.None;
                return;
            }

            runner = Receive();
        }

        public void SetRules() {
            Send(new RulesMessage());
        }

        public void Start(Field f) {
            if (Status != BotStatus.Ready) {
                errors.Enqueue("Bot is not ready");
                return;
            }

            field = f;
            field.OnNewPiece += NewPieceHandler;
            Send(new StartMessage(f));
            Status = BotStatus.Active;
        }

        public void Stop() {
            if (Status == BotStatus.None) {
                throw new InvalidOperationException();
            }

            Send(new FrontendMessage(FrontendMessage.KeyStop));
            field.OnNewPiece -= NewPieceHandler;
            Status = BotStatus.Ready;
        }

        public void Quit() {
            if (Status == BotStatus.None) return;
            if (Status == BotStatus.Launching) {
                exe.Kill();
                Status = BotStatus.None;
                return;
            }
            quitSent = true;
            Send(new FrontendMessage(FrontendMessage.KeyQuit));
            Status = BotStatus.None;
        }

        private void NewPieceHandler(Piece newPiece) {
            Send(new NewPieceMessage(newPiece));
        }

        private void Send<T>(T message) where T : FrontendMessage {
            Debug.Log("[SEND] " + JsonSerializer.ToJsonString(message));
            Send(JsonSerializer.Serialize(message));
        }

        private void Send(byte[] json) {
            exe.Send(json);
        }

        private async UniTask Receive() {
            while (true) {
                var line = await exe.Receive();
                if (line == null) {
                    if (!quitSent) {
                        errors.Enqueue("Bot unexpectedly quit");
                    }
                    quitSent = false;
                    Status = BotStatus.None;
                    return;
                }

                var msg = JsonUtility.FromJson<BotMessage>(line);

                switch (msg.type) {
                    case BotMessage.KeyInfo when Status == BotStatus.Launching:
                        BotInfo = JsonUtility.FromJson<InfoMessage>(line);
                        Status = BotStatus.Init;
                        break;
                    case BotMessage.KeyReady when Status == BotStatus.Init || Status == BotStatus.Ready:
                        Status = BotStatus.Ready;
                        break;
                    case BotMessage.KeyError when Status == BotStatus.Init || Status == BotStatus.Ready:
                        var err = JsonUtility.FromJson<ErrorMessage>(line);
                        errors.Enqueue(err.reason);
                        Status = BotStatus.Init;
                        break;
                    case BotMessage.KeySuggestion when pendingSuggestion:
                        ProcessSuggestion(JsonUtility.FromJson<SuggestionMessage>(line)).Forget();
                        break;
                }
            }
        }

        private async UniTask ProcessSuggestion(SuggestionMessage suggestion) {
            pendingSuggestion = false;

            // var sw = Stopwatch.StartNew();
            await moveFindTask;
            // Debug.Log(sw.ElapsedMilliseconds);

            for (var i = 0; i < suggestion.moves.Count; i++) {
                var move = suggestion.moves[i];
                var targets = move.ToFallingPiece().GetCanonicals();
                foreach (var target in targets) {
                    var route = moveFinder.BuildRoute(target, target.piece != field.CurrentPiece?.piece);
                    if (route != null) {
                        Send(new PlayMessage(move));
                        moveFound.TrySetResult(new BotMoveResult(move, route.Value));
                        // Debug.Log(route.ToString());
                        return;
                    }
                }
            }

            Send(new FrontendMessage(FrontendMessage.KeyStop));
            Status = BotStatus.Ready;
        }

        public async UniTask<BotMoveResult> NextMove() {
            pendingSuggestion = true;
            moveFound = new UniTaskCompletionSource<BotMoveResult>();
            moveFindTask = moveFinder.FindValidPlacements(
                field.CopyGrid(), field.CurrentPiece.Value.piece, field.HoldPiece ?? field.next.First(), new PieceSpawner());
            Send(new FrontendMessage(FrontendMessage.KeySuggest));
            return await moveFound.Task;
        }
    }
}