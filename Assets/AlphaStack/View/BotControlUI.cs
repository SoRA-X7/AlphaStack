using System;
using System.IO;
using AlphaStack.Game;
using AlphaStack.Tbp;
using SFB;
using TMPro;
using UnityEngine;

namespace AlphaStack.View {
    public class BotControlUI : MonoBehaviour {
        private BotPlayer player;
        private string path;

        [SerializeField] private GameObject selectButton;
        [SerializeField] private GameObject launchButton;
        [SerializeField] private GameObject quitButton;

        [SerializeField] private TMP_Text playerID;
        [SerializeField] private TMP_Text botName;
        [SerializeField] private TMP_Text botPath;
        [SerializeField] private TMP_Text botStatus;

        [SerializeField] private BotErrorPanelManager errorPanelManager;

        private void Start() {
            player = new BotPlayer(new BotPlayerConfig(), FindObjectOfType<Match>());
            launchButton.SetActive(false);
            quitButton.SetActive(false);
            botPath.text = "---";
            playerID.text = player.ID.ToString();

            GetComponent<FieldView>().player = player;
        }

        private void Update() {
            var botPathOk = !string.IsNullOrWhiteSpace(path);
            var botLaunched = player.Status != BotStatus.None && player.Status != BotStatus.Launching;
            botName.enabled = botLaunched;
            botPath.enabled = !botLaunched;

            launchButton.SetActive(botPathOk && !player.Launched);
            quitButton.SetActive(player.Launched);
            selectButton.SetActive(!player.Launched);

            while (player.TryGetError(out var error)) {
                errorPanelManager.ShowError(error);
            }

            if (!botPathOk) {
                botStatus.text = "No executable";
                return;
            }
            botStatus.text = StatusText(player.Status);
            botStatus.color = StatusColor(player.Status);
            botName.text = player.BotInfo?.name ?? "---";
        }

        private void OnDestroy() {
            QuitBot();
        }

        public void OpenDialog() {
            var paths = StandaloneFileBrowser.OpenFilePanel("Select bot executable", "", "", false);

            path = paths is { Length: 1 } ? paths[0] : "";
            botPath.text = Path.GetFileName(path);
            if (path == "") {
                botPath.text = "---";
            }
        }

        public void LaunchBot() {
            if (string.IsNullOrWhiteSpace(path)) return;
            player.Launch(path);
        }

        public void QuitBot() {
            player.Quit();
        }

        private static string StatusText(BotStatus status) {
            return status switch {
                BotStatus.None => "Not launched",
                BotStatus.Launching => "Launching",
                BotStatus.Init => "Initialized",
                BotStatus.Ready => "Ready",
                BotStatus.Active => "Active",
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
            };
        }

        private static Color StatusColor(BotStatus status) {
            return status switch {
                BotStatus.None => new Color(0.5f, 0.5f, 0.5f),
                BotStatus.Launching => new Color(1f, 1f, 0.5f),
                BotStatus.Init => new Color(1f, 0.5f, 0.7f),
                BotStatus.Ready => new Color(0.5f, 1f, 0.5f),
                BotStatus.Active => new Color(0.4f, 0.7f, 1f),
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
            };
        }
    }
}