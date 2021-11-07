using System;
using System.Threading;
using AlphaStack.Game;
using AlphaStack.Game.Routing;
using AlphaStack.Game.Util;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AlphaStack.Tbp {
    public class BotPlayer : Player {
        private BotManager bot;
        private Field field;
        private BotPlayerConfig config;
        private bool running;

        public BotStatus Status => bot?.Status ?? BotStatus.None;
        public bool Launched => bot != null && bot.Status != BotStatus.None;
        public InfoMessage? BotInfo => bot?.BotInfo;
        public override bool Ready => bot?.Status == BotStatus.Ready;
        public override bool HasError => bot == null || bot.Status < BotStatus.Init;
        public override Field PlayerField => field;

        private CancellationTokenSource cts = new CancellationTokenSource();

        public BotPlayer(BotPlayerConfig config, Match match) {
            this.config = config;
            match.AddPlayer(this);
        }

        public override bool SetRules(GameRules rules) {
            if (!Launched) return false;
            field = new Field(rules.pieceGenerator.Clone(), rules.attackTable, new DelaysConfig());
            bot.SetRules(rules);
            return true;
        }

        public override void Prepare() {
            bot.Start(field);
            running = true;
            field.Init();
        }

        public override void Start() {
            if (!running) {
                throw new InvalidOperationException("BotPlayer is not prepared");
            }

            field.SpawnNext();
            Run();
        }

        public override void Stop() {
            bot?.Stop();
            cts.Cancel();
            cts = new CancellationTokenSource();
            field = null;
            running = false;
        }

        public void Launch(string botExePath) {
            bot = new BotManager(new StandaloneBotCommunicator(ID, botExePath));
            bot.Launch();
        }

        public void Quit() {
            bot?.Quit();
            bot = null;
        }

        public bool TryGetError(out string error) {
            error = null;
            if (bot != null && bot.errors.Count > 0) {
                error = bot.errors.Dequeue();
                return true;
            }

            return false;
        }

        private void Run() {
            UniTask.Run(async () => {
                while (running && !field.Dead) {
                    ThreadUtils.CheckThread();
                    var move = await bot.NextMove();
                    if (move == null) {
                        running = false;
                        return;
                    }

                    await ThreadUtils.SwitchToTaskPool();

                    await ProcessRoute(move.move, move.route, cts.Token);
                }
            }, cancellationToken: cts.Token);
        }

        private async UniTask ProcessRoute(Move move, Route route, CancellationToken cancellationToken) {
            if (route.hold) {
                await field.Hold();
            }

            var prev = Instruction.None;
            foreach (var inst in route.instructions) {
                if (prev == inst) {
                    await AsyncUtils.Delay(config.inputDelay, cancellationToken);
                }

                switch (inst) {
                    case Instruction.Left:
                        field.MovePiece(new Vector2Int(-1, 0));
                        break;
                    case Instruction.Right:
                        field.MovePiece(new Vector2Int(1, 0));
                        break;
                    case Instruction.Cw:
                        field.RotatePiece(true);
                        break;
                    case Instruction.Ccw:
                        field.RotatePiece(false);
                        break;
                    case Instruction.SonicDrop:
                        while (!field.IsCurrentGrounded()) {
                            await AsyncUtils.Delay(config.dropRate, cancellationToken);
                            field.MovePiece(new Vector2Int(0, -1));
                        }

                        break;
                }

                prev = inst;

                await AsyncUtils.Delay(config.inputDelay, cancellationToken);
            }

            await field.LockPiece(cancellationToken);
        }
    }
}