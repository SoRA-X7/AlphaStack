using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Cysharp.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace AlphaStack.Tbp {
    public class StandaloneBotCommunicator : IBotCommunicator {
        private int id;
        private string executable;
        private Process botProcess;
        private StreamReader stdout;
        private StreamReader stderr;
        private StreamWriter stdin;

        private StreamWriter logger;

        public StandaloneBotCommunicator(int id, string executable) {
            this.id = id;
            this.executable = executable;
        }

        public bool Launch() {
            try {
                var start = new ProcessStartInfo {
                    FileName = executable,
                    WorkingDirectory = Path.GetDirectoryName(executable)!,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                botProcess = Process.Start(start);
                stdin = botProcess.StandardInput;
                stdout = botProcess.StandardOutput;
                stderr = botProcess.StandardError;

                logger?.Dispose();
                var logFilePath = Path.Combine(Environment.CurrentDirectory, $"Logs/bot{id}.log");
                if (File.Exists(logFilePath)) {
                    var prevLogFilePath = Path.Combine(Environment.CurrentDirectory, $"Logs/bot{id}_prev.log");
                    if (File.Exists(prevLogFilePath)) {
                        File.Delete(prevLogFilePath);
                    }

                    File.Move(logFilePath, prevLogFilePath);
                }

                logger = File.CreateText(logFilePath);
                logger.AutoFlush = true;

                botProcess.ErrorDataReceived += (_, err) => { logger.Write(err.Data); };
                return true;
            } catch (Win32Exception ex) {
                Debug.LogError(ex);
                return false;
            } catch (IOException ex) {
                Debug.LogError(ex);
                Kill();
                return false;
            }
        }

        public async UniTask<string> Receive() {
            var s = await stdout.ReadLineAsync();
            Debug.Log("[RECEIVE] " + s);
            if (s == null) {
                DisposeBotProcess();
            }
            return s;
        }

        public void Send(byte[] json) {
            try {
                stdin.BaseStream.Write(json, 0, json.Length);
                stdin.Write('\n');
            } catch (IOException e) {
                Debug.LogWarning(e);
            }
        }

        public void Kill() {
            try {
                botProcess.Kill();
            } catch (InvalidOperationException e) {
                Debug.LogWarning(e);
            }
            DisposeBotProcess();
        }

        public void Dispose() {
            DisposeBotProcess();
        }

        public void DisposeBotProcess() {
            stdout = null;
            botProcess?.Dispose();
            botProcess = null;
            logger?.Dispose();
            logger = null;
        }
    }
}