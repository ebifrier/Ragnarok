using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ragnarok;
using Ragnarok.Utility;

namespace Ragnarok.Net
{
    /// <summary>
    /// 評価値修正時のイベント引数です。
    /// </summary>
    public class EvaluationEventArgs : EventArgs
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EvaluationEventArgs(double value)
        {
            Value = value;
        }

        /// <summary>
        /// 評価値を取得します。
        /// </summary>
        public double Value
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

        public event EventHandler<EvaluationEventArgs> EvaluationChanged;

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

                // 評価値は'\n'で区切られています。
                var data = this.reader.ReadUntil((byte)'\n');
                if (data == null)
                {
                    return;
                }

                // 評価値データを取得します。
                var str = Encoding.UTF8.GetString(data);
                if (string.IsNullOrEmpty(str))
                {
                    return;
                }

                var val = double.Parse(str);

                EvaluationChanged.RaiseEvent(
                    this, new EvaluationEventArgs(val));
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);

                Log.ErrorException(ex,
                    "評価値データの取得に失敗しました。");
            }
        }
    }
}
