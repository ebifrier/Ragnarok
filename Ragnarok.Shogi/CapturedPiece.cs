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
    /// 駒台/駒箱にある各駒の情報を示します。
    /// </summary>
    [DataContract()]
    public class CapturedPiece : ILazyModel, IEquatable<CapturedPiece>
    {
        private readonly object SyncRoot = new object();
        private readonly LazyModelObject lazyModelObject = new LazyModelObject();
        private int count;

        /// <summary>
        /// プロパティの変更を通知します。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティの変更を通知します。
        /// </summary>
        void IModel.NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            Util.CallPropertyChanged(
                PropertyChanged, this, e);
        }

        /// <summary>
        /// 内部で使います。
        /// </summary>
        LazyModelObject ILazyModel.LazyModelObject
        {
            get
            {
                return this.lazyModelObject;
            }
        }

        /// <summary>
        /// 駒情報を取得します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public BoardPiece Piece
        {
            get;
            private set;
        }

        /// <summary>
        /// 駒の数を取得または設定します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public int Count
        {
            get
            {
                return this.count;
            }
            set
            {
                using (new LazyModelLock(this, SyncRoot))
                {
                    if (this.count != value)
                    {
                        this.count = value;

                        this.RaisePropertyChanged("Count");
                    }
                }
            }
        }

        /// <summary>
        /// 持ち駒の数が０より大きいかどうかを取得します。
        /// </summary>
        [DependOnProperty("Count")]
        public bool IsVisible
        {
            get
            {
                return (Count > 0);
            }
        }

        /// <summary>
        /// オブジェクトの妥当性を検証します。
        /// </summary>
        public bool Validate()
        {
            if (Piece == null || !Piece.Validate())
            {
                return false;
            }

            if (Count < 0)
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
            var other = obj as CapturedPiece;

            return Equals(other);
        }

        /// <summary>
        /// オブジェクトの等値性を判定します。
        /// </summary>
        public bool Equals(CapturedPiece other)
        {
            if ((object)other == null)
            {
                return false;
            }

            if (Piece != other.Piece)
            {
                return false;
            }

            if (Count != other.Count)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator==(CapturedPiece x, CapturedPiece y)
        {
            return Util.GenericClassEquals(x, y);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator !=(CapturedPiece x, CapturedPiece y)
        {
            return !(x == y);
        }

        /// <summary>
        /// ハッシュ値を返します。
        /// </summary>
        public override int GetHashCode()
        {
            return (Piece.GetHashCode() ^ Count.GetHashCode());
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CapturedPiece(BoardPiece piece, int count)
        {
            this.Piece = piece;
            this.count = count;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CapturedPiece(BoardPiece piece)
            : this(piece, 0)
        {
        }
    }
}
