using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Ragnarok;
using Ragnarok.Net;

namespace Ragnarok.Utility
{
    /// <summary>
    /// 評価値関係の例外クラスです。
    /// </summary>
    public class EvaluationException : Exception
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EvaluationException()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EvaluationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EvaluationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// 評価値サーバーのコマンドです。
    /// </summary>
    public enum EvaluationCommand
    {
        /// <summary>
        /// 評価値を送ります。
        /// </summary>
        Value,
        /// <summary>
        /// その他情報を送ります。
        /// </summary>
        Info,
    }

    /// <summary>
    /// 評価値修正時のイベント引数です。
    /// </summary>
    public class EvaluationEventArgs : EventArgs
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EvaluationEventArgs(double value, List<string> arguments)
        {
            Command = EvaluationCommand.Value;
            Value = value;
            Arguments = arguments;
        }

        /// <summary>
        /// コマンドの種類を取得します。
        /// </summary>
        public EvaluationCommand Command
        {
            get;
            private set;
        }

        /// <summary>
        /// 評価値を取得します。
        /// </summary>
        public double Value
        {
            get;
            private set;
        }

        /// <summary>
        /// コマンド引数を取得します。
        /// </summary>
        public List<string> Arguments
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// 評価値サーバーとのやり取りを行います。
    /// </summary>
    public sealed class EvaluationClient
    {
        private Connection connection = new Connection();
        private BinarySplitReader reader = new BinarySplitReader(2048);

        /// <summary>
        /// コマンド受信時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<EvaluationEventArgs> CommandReceived;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EvaluationClient()
        {
            this.connection = new Connection();
            this.connection.Received += connection_Received;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EvaluationClient(string address, int port)
            : this()
        {
            Connect(address, port);
        }

        /// <summary>
        /// サーバーに接続されているかどうかを取得します。
        /// </summary>
        public bool IsConnected
        {
            get { return this.connection.IsConnected; }
        }

        /// <summary>
        /// サーバーに接続します。
        /// </summary>
        public void Connect(string address, int port)
        {
            this.connection.Connect(address, port);
        }
        /// <summary>
        /// サーバーから切断します。
        /// </summary>
        public void Disconnect()
        {
            this.connection.Disconnect();
        }

        /// <summary>
        /// データ受信時にコールバックされます。
        /// </summary>
        private void connection_Received(object sender, DataEventArgs e)
        {
            if (e.Error != null)
            {
                Log.ErrorException(e.Error,
                    "評価値サーバーからデータの受信に失敗しました。");
                return;
            }

            try
            {
                this.reader.Write(e.Data, 0, e.DataLength);

                // コマンド行は'\0'で区切られています。
                var data = this.reader.ReadUntil((byte)'\0');
                if (data == null)
                {
                    return;
                }

                var str = Encoding.UTF8.GetString(data);
                if (string.IsNullOrEmpty(str))
                {
                    return;
                }

                // 各コマンドをパースします。
                var list = ParseCommand(str);
                if (!list.Any())
                {
                    return;
                }

                HandleCommand(list[0], list.Take(1).ToList());
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);

                Log.ErrorException(ex,
                    "評価値データの取得に失敗しました。");
            }
        }

        #region Parse Line
        // 各フィールドを取得するための正規表現
        private static readonly Regex CsvRegex = new Regex(
            @"([^""\s][^\s]*?|""(?:[^""]|"""")*?""|\s)");

        /// <summary>
        /// 1つの行からフィールドを取り出します。
        /// </summary>
        private List<string> ParseCommand(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return new List<string>();
            }

            return CsvRegex.Matches(line.Trim())
                .OfType<Match>()
                .Where(_ => _.Success)
                .Select(_ => _.Value.Trim())
                .Where(_ => !string.IsNullOrWhiteSpace(_))
                .Select(_ => Filter(_))
                .ToList();
        }

        /// <summary>
        /// フィールドが"で囲まれている場合、前後の"を取り「""」を「"」に置換します。
        /// </summary>
        private string Filter(string field)
        {
            if (field.Length >= 2 &&
                field.StartsWith("\"") && field.EndsWith("\""))
            {
                field = field.Substring(1, field.Length - 2);
                field = field.Replace("\"\"", "\"");
            }

            return field;
        }
        #endregion

        #region Handle Command
        /// <summary>
        /// コマンドを処理します。
        /// </summary>
        private void HandleCommand(string command, List<string> args)
        {
            try
            {
                switch (command)
                {
                    case "value":
                        HandleEvaluationCommand(args);
                        break;
                    default:
                        throw new EvaluationException(
                            "不正なコマンドです。");
                }
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);

                Log.ErrorException(ex,
                    "コマンドの処理に失敗しました。");
            }
        }

        /// <summary>
        /// 評価値コマンドを処理します。
        /// </summary>
        private void HandleEvaluationCommand(List<string> args)
        {
            if (args.Count() < 1)
            {
                throw new EvaluationException(
                    "評価値の値が存在しません。");
            }

            var val = double.Parse(args[0]);
            CommandReceived.RaiseEvent(
                this, new EvaluationEventArgs(val, args));
        }
        #endregion
    }
}
