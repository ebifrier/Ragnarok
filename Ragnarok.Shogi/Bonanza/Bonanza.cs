using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

using Ragnarok;
using Ragnarok.ObjectModel;
using Ragnarok.Utility;

namespace Ragnarok.Shogi.Bonanza
{
    /// <summary>
    /// Bonanzaの使用メモリとそれを使うためのハッシュ値を保持します。
    /// </summary>
    public sealed class BonanzaHashMem
    {
        /// <summary>
        /// 使用メモリ量[MB]を取得します。
        /// </summary>
        public int MemSize
        {
            get;
            private set;
        }

        /// <summary>
        /// ハッシュ値の指定に使う値を取得します。
        /// </summary>
        public int HashValue
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BonanzaHashMem(int memSize, int hashValue)
        {
            MemSize = memSize;
            HashValue = hashValue;
        }
    }

    /// <summary>
    /// ボナンザの操作用オブジェクトです。
    /// </summary>
    public class Bonanza : NotifyObject, IDisposable
    {
        /// <summary>
        /// ボナンザの基本使用メモリ(MB)
        /// </summary>
        public const int MemUsedBase = 230;
        /// <summary>
        /// ハッシュサイズの最小値です。(24MB)
        /// </summary>
        public const int HashSizeMinimum = 24;

        private readonly LinkedList<string> writeCommands =
            new LinkedList<string>();
        private volatile bool aborted;
        private volatile bool initialized;
        private Process bonaProcess;
        private Thread readThread;
        private Thread errorReadThread;
        private Thread writeThread;
        private bool disposed = false;

        /// <summary>
        /// コマンド送信時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<BonanzaReceivedCommandEventArgs> CommandSent;

        /// <summary>
        /// コマンド受信時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<BonanzaReceivedCommandEventArgs> CommandReceived;

        /// <summary>
        /// エラー受信時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<BonanzaReceivedCommandEventArgs> ErrorReceived;

        /// <summary>
        /// ボナンザが終了したときに呼ばれます。
        /// </summary>
        public event EventHandler<BonanzaAbortedEventArgs> Aborted;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Bonanza()
        {
            ErrorReceived += Bonanza_ReceivedError;
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~Bonanza()
        {
            Dispose(false);
        }

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Abort(0);
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// 並列化サーバーに接続したかどうかを取得します。
        /// </summary>
        public bool IsConnected
        {
            get { return GetValue<bool>("IsConnected"); }
            private set { SetValue("IsConnected", value); }
        }

        /// <summary>
        /// ボナンザが終了していたら、その理由を取得します。
        /// </summary>
        public AbortReason? AbortedReason
        {
            get { return GetValue<AbortReason?>("AbortedReason"); }
            private set { SetValue("AbortedReason", value); }
        }

        /// <summary>
        /// ボナンザのハッシュサイズ指定に使う使用メモリの一覧を取得します。
        /// </summary>
        /// <param name="rate">
        /// 物理メモリの何割をボナンザが使用可能とするか指定します。(0.0～1.0)
        /// </param>
        /// <return>
        /// ボナンザのハッシュ値指定に使う値と実際の使用メモリ[MB]の
        /// ペアを返します。
        /// </return>
        public static IEnumerable<BonanzaHashMem> MemorySizeList(double rate)
        {
            var hashSize = 19;
            var mem = Bonanza.HashSizeMinimum;

            // メモリ最大値は1GBとします。
            // あまり大きいとメモリ確保に失敗します。
            var memMax = Math.Min(
                (long)(DeviceInventory.MemorySize / 1024 / 1024 * rate),
                1024 + Bonanza.MemUsedBase);

            do
            {
                yield return new BonanzaHashMem(mem + Bonanza.MemUsedBase, hashSize);

                mem *= 2;
                hashSize += 1;
            } while (mem + Bonanza.MemUsedBase <= memMax);
        }

        /// <summary>
        /// ボナンザを停止します。
        /// </summary>
        public void Abort(AbortReason reason, int millis = 500)
        {
            using (LazyLock())
            {
                this.aborted = true;

                // 初期化されていなければ、そのまま帰ります。
                if (!this.initialized)
                {
                    return;
                }

                WriteCommand("quit");

                // ボナンザプロセスを終了します。
                var process = this.bonaProcess;
                if (process != null)
                {
                    if (!process.WaitForExit(millis))
                    {
                        process.Kill();
                    }

                    this.bonaProcess = null;
                }

                if (this.writeThread != null)
                {
                    this.writeThread.Join(0);
                    this.writeThread = null;
                }

                if (this.readThread != null)
                {
                    this.readThread.Join(0);
                    this.readThread = null;
                }

                if (this.errorReadThread != null)
                {
                    this.errorReadThread.Join(0);
                    this.errorReadThread = null;
                }

                this.initialized = false;
                IsConnected = false;
                AbortedReason = reason;
            }

            Aborted.SafeRaiseEvent(this, new BonanzaAbortedEventArgs(reason));
        }
        
        /// <summary>
        /// ボナンザを起動します。
        /// </summary>
        public void Initialize(string path)
        {
            using (LazyLock())
            {
                if (this.initialized)
                {
                    throw new InvalidOperationException(
                        "ボナンザはすでに既に初期化されています。");
                }

                if (this.aborted)
                {
                    throw new InvalidOperationException(
                        "ボナンザはすでに既に終了しています。");
                }

                // まずプロセスを起動します。
                Start(path);

                // 
                if (this.aborted)
                {
                    throw new BonanzaException();
                }

                this.initialized = true;
            }
        }

        /// <summary>
        /// ボナンザプロセスを起動します。
        /// </summary>
        private void Start(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new FileNotFoundException(
                    string.Format("{0}が見つかりません。", path));
            }

            var filepath = Path.GetFullPath(path);
            var startInfo = new ProcessStartInfo
            {
                FileName = filepath,
                WorkingDirectory = Path.GetDirectoryName(filepath),
                Arguments = null,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
            };

            var process = new Process
            {
                StartInfo = startInfo,
            };

            if (!process.Start())
            {
                throw new InvalidOperationException(
                    "ボナンザの起動に失敗しました。");
            }

            process.Exited += (sender, e) =>
                Abort(AbortReason.Aborted);

            ReadStart(process.StandardOutput);
            ErrorReadStart(process.StandardError);
            WriteStart(process.StandardInput);

            this.bonaProcess = process;
        }

        #region コールバック
        void Bonanza_ReceivedError(object sender, BonanzaReceivedCommandEventArgs e)
        {
            var error = e.Command;

            if (error.StartsWith("ERROR: "))
            {
                // 再起不能なエラーの場合は、FatalErrorとします。
                if (error.StartsWith("ERROR: Can't open a file,"))
                {
                    Abort(AbortReason.FatalError);
                }
                else
                {
                    Abort(AbortReason.Error);
                }
            }
            else if (error.StartsWith("WARNING: "))
            {
                Abort(AbortReason.Error);
            }
        }
        #endregion

        #region mnj
        /// <summary>
        /// 並列化サーバーに接続します。
        /// </summary>
        public void Connect(string serverAddress, int serverPort,
                            string name, int threadNum, int hashSize, int depth,
                            bool sendPV)
        {
            using (LazyLock())
            {
                if (!this.initialized)
                {
                    throw new InvalidOperationException(
                        "ボナンザが起動していません。");
                }

                if (IsConnected)
                {
                    throw new InvalidOperationException(
                        "既に並列化サーバーに接続しています。");
                }
                
                if (string.IsNullOrEmpty(name) ||
                    !name.All(_ => char.IsLetterOrDigit(_) || _ == '_'))
                {
                    throw new ArgumentException(
                        "名前はアルファベット＋数字の組み合わせでお願いします。");
                }

                WriteCommand(string.Format("book off"));
                WriteCommand(string.Format("tlp num {0}", threadNum));
                WriteCommand(string.Format("hash {0}", hashSize));

                // 並列化サーバーへの接続コマンドを発行します。
                var command = string.Format(
                    "mnj {0} {1} {2} {3} {4} {5}",
                    serverAddress, serverPort,
                    name, threadNum, depth,
                    sendPV ? 1 : 0);
                WriteCommand(command);

                IsConnected = true;
            }
        }
        #endregion

        #region dfpn
        /// <summary>
        /// DFPNサーバーに接続します。
        /// </summary>
        public void ConnectToDfpn(string serverAddress, int serverPort, string name,
                                  int threadNum, int hashSize)
        {
            using (LazyLock())
            {
                if (!this.initialized)
                {
                    throw new InvalidOperationException(
                        "ボナンザが起動していません。");
                }

                if (IsConnected)
                {
                    throw new InvalidOperationException(
                        "既に並列化サーバーに接続しています。");
                }

                if (string.IsNullOrEmpty(name) ||
                    !name.All(_ => char.IsLetterOrDigit(_) || _ == '_'))
                {
                    throw new ArgumentException(
                        "名前はアルファベット＋数字の組み合わせでお願いします。");
                }

                WriteCommand(string.Format("tlp num {0}", threadNum));
                WriteCommand(string.Format("hash {0}", hashSize));

                // DFPNサーバーへの接続コマンドを発行します。
                var command = string.Format(
                    "dfpn connect {0} {1} {2}",
                    serverAddress,
                    serverPort,
                    name);
                WriteCommand(command);

                IsConnected = true;
            }
        }
        #endregion

        #region ボナンザ読み込み
        /// <summary>
        /// ボナンザからの読み込みを開始します。
        /// </summary>
        private void ReadStart(StreamReader reader)
        {
            this.readThread = new Thread(ReadMain)
            {
                IsBackground = true,
                Name = "Bonanza Read Thread",
                Priority = ThreadPriority.AboveNormal,
            };

            this.readThread.Start(reader);
        }

        /// <summary>
        /// ボナンザからの読み込みを行います。
        /// </summary>
        private void ReadMain(object arg)
        {
            var reader = (StreamReader)arg;

            while (!this.aborted)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }

                CommandReceived.SafeRaiseEvent(
                    this, new BonanzaReceivedCommandEventArgs(line));
            }
        }
        #endregion

        #region ボナンザエラー読み込み
        /// <summary>
        /// ボナンザからのエラー読み込みを開始します。
        /// </summary>
        private void ErrorReadStart(StreamReader reader)
        {
            this.errorReadThread = new Thread(ErrorReadMain)
            {
                IsBackground = true,
                Name = "Bonanza ErrorRead Thread",
                Priority = ThreadPriority.AboveNormal,
            };

            this.errorReadThread.Start(reader);
        }

        /// <summary>
        /// ボナンザからのエラー読み込みを行います。
        /// </summary>
        private void ErrorReadMain(object arg)
        {
            var reader = (StreamReader)arg;

            while (!this.aborted)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }

                ErrorReceived.SafeRaiseEvent(
                    this, new BonanzaReceivedCommandEventArgs(line));
            }
        }
        #endregion

        #region ボナンザ書き込み
        /// <summary>
        /// ボナンザへの書き込みコマンドを設定します。
        /// </summary>
        public void WriteCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return;
            }

            string trimmedCommand = command.Trim();

            lock (this.writeCommands)
            {
                this.writeCommands.AddLast(trimmedCommand);

                Monitor.PulseAll(this.writeCommands);
            }
        }

        /// <summary>
        /// ボナンザへの書き込みコマンドを一つ取得します。
        /// </summary>
        private string GetWriteCommand()
        {
            lock (this.writeCommands)
            {
                while (!this.aborted && !this.writeCommands.Any())
                {
                    Monitor.Wait(this.writeCommands, 500);
                }

                if (this.aborted && !this.writeCommands.Any())
                {
                    return null;
                }

                // 書き込みコマンドを一つ取り出します。
                var command = this.writeCommands.FirstOrDefault();
                this.writeCommands.RemoveFirst();
                return command;
            }
        }

        /// <summary>
        /// ボナンザへの書き込みを開始します。
        /// </summary>
        private void WriteStart(StreamWriter writer)
        {
            this.writeThread = new Thread(WriteMain)
            {
                IsBackground = true,
                Name = "Bonanza WriteCommand Thread",
                Priority = ThreadPriority.AboveNormal,
            };

            this.writeThread.Start(writer);
        }

        /// <summary>
        /// ボナンザへの書き込みを行います。
        /// </summary>
        private void WriteMain(object arg)
        {
            var writer = (StreamWriter)arg;

            while (!this.aborted)
            {
                var command = GetWriteCommand();
                if (command == null)
                {
                    continue;
                }

                writer.WriteLine(command + "\n");
                writer.Flush();

                CommandSent.SafeRaiseEvent(this,
                    new BonanzaReceivedCommandEventArgs(command));
            }
        }
        #endregion
    }
}
