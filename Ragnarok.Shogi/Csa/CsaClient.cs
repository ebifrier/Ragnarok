using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.Shogi.Csa
{
    using Utility;

    public class StringEventArgs : EventArgs
    {
        public string Line
        {
            get;
            private set;
        }

        public StringEventArgs(string line)
        {
            Line = line;
        }
    }

    public class CsaClient
    {
        //private readonly object SyncRoot = new object();
        private TcpClient tcp = new TcpClient();
        private NetworkStream stream;
        private BinarySplitReader receivedData = new BinarySplitReader(2048);

        public event EventHandler<StringEventArgs> Received;
        public event EventHandler<StringEventArgs> Sent;

        /// <summary>
        /// ログイン時の名前を取得します。
        /// </summary>
        public string UserName
        {
            get;
            private set;
        }

        public CsaGameInfo GameInfo
        {
            get;
            private set;
        }

        public CsaClient()
        {
        }

        public void Connect(string address, int port)
        {
            try
            {
                // 例外が発生しなかったら、基本的にはつながっているはず。
                this.tcp.Connect(address, port);

                this.stream = this.tcp.GetStream();
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);

                Log.ErrorException(ex,
                    string.Format(
                        "{0}:{1} への接続に失敗しました。",
                        address, port));
            }
        }

        public void Close()
        {
            UserName = null;
            GameInfo = null;

            this.stream = null;
            this.receivedData = null;

            if (this.tcp != null)
            {
                this.tcp.Close();
                this.tcp = null;
            }
        }

        public void WriteLine(string line)
        {
            if (this.stream == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            if (!line.EndsWith("\n"))
            {
                line = line + '\n';
            }

            var data = Encoding.UTF8.GetBytes(line);
            this.stream.Write(data, 0, data.Length);

            Sent.SafeRaiseEvent(this, new StringEventArgs(line));
        }

        public string ReadLine()
        {
            if (this.stream == null)
            {
                return null;
            }

            var lineData = this.receivedData.ReadUntil((byte)'\n');
            if (lineData != null)
            {
                var line = Encoding.UTF8.GetString(lineData);
                Received.SafeRaiseEvent(this, new StringEventArgs(line));

                return line.Substring(0, line.Length - 1);
            }

            var data = new byte[256];
            do
            {
                var size = this.stream.Read(data, 0, data.Length);
                if (size == 0)
                {
                    Close();
                    return null;
                }

                this.receivedData.Write(data, 0, size);
            } while (this.stream.DataAvailable);

            // データを受信したはずなので、もう一度トライ！
            return ReadLine();
        }

        private static readonly Regex LoginRegex = new Regex(
            @"^LOGIN:([\d\w_\-]+)( OK)?$");

        public bool Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            if (!string.IsNullOrEmpty(UserName))
            {
                throw new InvalidOperationException(
                    "すでにログインしています。");
            }

            if (!this.tcp.Connected)
            {
                throw new InvalidOperationException(
                    "サーバーに接続していません。");
            }

            WriteLine(string.Format(
                "LOGIN {0} {1}",
                username, password));

            var line = ReadLine();
            if (line == null)
            {
                return false;
            }

            var m = LoginRegex.Match(line);
            if (!m.Success ||
                !m.Groups[2].Success ||
                m.Groups[1].Value != username)
            {
                return false;
            }

            UserName = username;
            return true;
        }

        public void Logout()
        {
            if (this.tcp == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(UserName))
            {
                return;
            }

            WriteLine("LOGOUT");

            var line = ReadLine();
            if (line == null)
            {
                return;
            }

            UserName = null;
            GameInfo = null;
        }

        /// <summary>
        /// 手合い情報を待ちます。
        /// </summary>
        public CsaGameInfo WaitGameSummary()
        {
            var info = new CsaGameInfo();

            while (!info.IsGameSummaryEnded)
            {
                var line = ReadLine();
                if (line == null)
                {
                    return null;
                }

                if (!info.ParseLine(line))
                {
                    Log.Error(string.Format(
                        "{0}: Game_Summaryの解釈に失敗しました。",
                        line));
                }
            }

            return info;
        }

        /// <summary>
        /// 対局を開始します。
        /// </summary>
        public bool StartGame(CsaGameInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("game");
            }

            WriteLine("AGREE");

            var line = ReadLine();
            if (line == null ||
                !line.StartsWith("START:"))
            {
                return false;
            }

            GameInfo = info;
            return true;
        }

        private static TEnum? GetTargetEnum<TEnum>(string line)
            where TEnum : struct
        {
            var pair = EnumEx
                .GetValues<TEnum>()
                .Select(_ => new
                {
                    Value = _,
                    Attribute = EnumEx.GetAttribute<CsaCommandAttribute>(_),
                })
                .Where(_ => _.Attribute != null)
                .FirstOrDefault(_ => line == "#" + _.Attribute.Command);

            return (pair != null ? pair.Value : (TEnum?)null);
        }

        private static readonly Regex MoveCommandRegex = new Regex(
            @"^((?:\+|\-)\d+\w+),\s*T(\d+)$");
        private static readonly Regex SpecialMoveCommandRegex = new Regex(
            @"^(%\w+)(?:,\s*T(\d+))?$");

        /// <summary>
        /// 対局中のコマンド情報を待ちます。
        /// </summary>
        public CsaGameCommand WaitGameCommand()
        {
            if (GameInfo == null)
            {
                throw new InvalidOperationException(
                    "このメソッドは対局中のみ呼び出すことができます。");
            }

            var line = ReadLine();
            if (line == null)
            {
                Close();
                return null;
            }

            if (!line.Any())
            {
                return WaitGameCommand();
            }

            if (line.StartsWith("##[ERROR]"))
            {
                return new CsaGameCommand
                {
                    Error = line.Substring(9).Trim(),
                };
            }
            else if (line.StartsWith("#"))
            {
                var reason = GetTargetEnum<GameEndReason>(line);
                if (reason != null)
                {
                    return new CsaGameCommand
                    {
                        EndReason = reason.Value,
                    };
                }

                var result = GetTargetEnum<GameResult>(line);
                if (result != null)
                {
                    return new CsaGameCommand
                    {
                        Result = result.Value,
                    };
                }

                return null;
            }
            else
            {
                var m = MoveCommandRegex.Match(line);
                if (m.Success)
                {
                    return new CsaGameCommand
                    {
                        Move = CsaMove.Parse(m.Groups[1].Value),
                        MoveTime = int.Parse(m.Groups[2].Value),
                    };
                }

                m = SpecialMoveCommandRegex.Match(line);
                if (m.Success)
                {
                    return new CsaGameCommand
                    {
                        Move = CsaMove.Parse(m.Groups[1].Value),
                        MoveTime = (m.Groups[2].Success ? int.Parse(m.Groups[2].Value) : 0),
                    };
                }

                return null;
            }
        }
    }
}
