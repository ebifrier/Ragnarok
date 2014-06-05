using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

using Ragnarok;
using Ragnarok.ObjectModel;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 持ち駒の数が変わったときに使われます。
    /// </summary>
    public class CountChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 持ち数が変わった駒の種類を取得または設定します。
        /// </summary>
        public PieceType PieceType
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CountChangedEventArgs(PieceType pieceType)
        {
            PieceType = pieceType;
        }
    }

    /// <summary>
    /// 駒箱/駒台用のビューモデルです。
    /// </summary>
    [DataContract()]
    public class CapturedPieceBox : NotifyObject, IEquatable<CapturedPieceBox>
    {
        /// <summary>
        /// 駒の種類です。
        /// </summary>
        public const int PieceCount = (int)PieceType.Hu + 1;

        private int[] pieceCountArray = new int[PieceCount];

        /// <summary>
        /// 持ち駒の数が変わったことを通知します。
        /// </summary>
        public event EventHandler<CountChangedEventArgs> CountChanged;

        /// <summary>
        /// 持ち駒の数の変更を通知します。
        /// </summary>
        void NotifyCountChanged(PieceType pieceType)
        {
            var handler = CountChanged;

            if (handler != null)
            {
                Util.CallEvent(() =>
                    handler(this, new CountChangedEventArgs(pieceType)));
            }
        }

        /// <summary>
        /// オブジェクトのコピーを作成します。
        /// </summary>
        public CapturedPieceBox Clone()
        {
            using (LazyLock())
            {
                return new CapturedPieceBox(BWType)
                {
                    pieceCountArray = (int[])this.pieceCountArray.Clone(),
                };
            }
        }

        /// <summary>
        /// 指定の駒の持ち駒オブジェクトを作成します。
        /// </summary>
        public CapturedPiece MakeCapturedPiece(PieceType pieceType)
        {
            using (LazyLock())
            {
                return new CapturedPiece(new Piece(pieceType, false, BWType))
                {
                    Count = this.pieceCountArray[(int)pieceType],
                };
            }
        }

        /// <summary>
        /// 先手側か後手側かを取得します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public BWType BWType
        {
            get;
            private set;
        }

        /// <summary>
        /// シリアライズ用のプロパティです。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        private int[] PieceCountArray
        {
            get
            {
                return this.pieceCountArray;
            }
            set
            {
                using (LazyLock())
                {
                    if (this.pieceCountArray != value)
                    {
                        this.pieceCountArray = value;

                        this.RaisePropertyChanged("PieceCountArray");
                    }
                }
            }
        }

        /// <summary>
        /// 持ち駒の数を取得します。
        /// </summary>
        public int GetCount(PieceType pieceType)
        {
            if (!Enum.IsDefined(typeof(PieceType), pieceType))
            {
                throw new ArgumentException("pieceType");
            }

            using (LazyLock())
            {
                return this.pieceCountArray[(int)pieceType];
            }
        }

        /// <summary>
        /// 持ち駒の数を設定します。
        /// </summary>
        public void SetCount(PieceType pieceType, int count)
        {
            if (!Enum.IsDefined(typeof(PieceType), pieceType))
            {
                throw new ArgumentException("pieceType");
            }

            using (LazyLock())
            {
                this.pieceCountArray[(int)pieceType] = count;
            }
        }

        /// <summary>
        /// 持ち駒の数を増やします。
        /// </summary>
        private void AddCount(PieceType pieceType, int count)
        {
            if (!Enum.IsDefined(typeof(PieceType), pieceType))
            {
                throw new ArgumentException("pieceType");
            }

            using (LazyLock())
            {
                if (this.pieceCountArray[(int)pieceType] + count < 0)
                {
                    return;
                }

                this.pieceCountArray[(int)pieceType] += count;
            }

            NotifyCountChanged(pieceType);
        }

        /// <summary>
        /// 持ち駒の数を増やします。
        /// </summary>
        public void Increment(PieceType pieceType)
        {
            AddCount(pieceType, +1);
        }

        /// <summary>
        /// 持ち駒の数を減らします。
        /// </summary>
        public void Decrement(PieceType pieceType)
        {
            AddCount(pieceType, -1);
        }

        /// <summary>
        /// 持ち駒をすべて初期化します。
        /// </summary>
        public void Clear()
        {
            using (LazyLock())
            {
                Array.Clear(
                    this.pieceCountArray, 0,
                    this.pieceCountArray.Count());
            }
        }

        /// <summary>
        /// オブジェクトの妥当性を検証します。
        /// </summary>
        public bool Validate()
        {
            if (!Enum.IsDefined(typeof(BWType), BWType))
            {
                return false;
            }

            if (this.pieceCountArray == null ||
                this.pieceCountArray.Count() != PieceCount)
            {
                return false;
            }

            // 数が負なら不正な値。
            if (this.pieceCountArray.Any(count => count < 0))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// オブジェクトの等値性を判定します。
        /// </summary>
        public override bool Equals(object obj)
        {
            var result = this.PreEquals(obj);
            if (result.HasValue)
            {
                return result.Value;
            }

            return Equals(obj as CapturedPieceBox);
        }

        /// <summary>
        /// オブジェクトの等値性を判定します。
        /// </summary>
        public bool Equals(CapturedPieceBox other)
        {
            if ((object)other == null)
            {
                return false;
            }

            for (var i = 0; i < PieceCount; ++i)
            {
                if (this.pieceCountArray[i] != other.pieceCountArray[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// ハッシュ値を返します。
        /// </summary>
        public override int GetHashCode()
        {
            var hash = 0;

            for (var i = 0; i < PieceCount; ++i)
            {
                hash ^= (this.pieceCountArray[i] * 10 + i).GetHashCode();
            }

            return hash;
        }

        /// <summary>
        /// protobuf-netの仕様(BUG?)でデシリアライズ時には
        /// 作成されたプロパティにデシリアライズされた要素を追加する。
        /// このため、デシリアライズ前に既存の配列をクリアしておかないと
        /// デシリアライズ後に配列サイズが倍になる。
        /// </summary>
        [OnDeserializing()]
        protected new void OnBeforeDeselialize(StreamingContext context)
        {
            base.OnBeforeDeselialize(context);

            this.pieceCountArray = new int[0];
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CapturedPieceBox(BWType bwType)
        {
            this.BWType = bwType;

            /*for (var i = 0; i < this.pieceCountArray.Length; ++i)
            {
                this.pieceCountArray[i] = 0;
            }*/
        }
    }
}
