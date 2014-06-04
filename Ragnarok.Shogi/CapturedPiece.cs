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
    public class CapturedPiece : NotifyObject, IEquatable<CapturedPiece>
    {
        /// <summary>
        /// 駒情報を取得します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public Piece Piece
        {
            get { return GetValue<Piece>("Piece"); }
            private set { SetValue("Piece", value); }
        }

        /// <summary>
        /// 駒の数を取得または設定します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public int Count
        {
            get { return GetValue<int>("Count"); }
            set { SetValue("Count", value); }
        }

        /// <summary>
        /// 持ち駒の数が０より大きいかどうかを取得します。
        /// </summary>
        [DependOnProperty("Count")]
        public bool IsVisible
        {
            get { return (Count > 0); }
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
            var result = this.PreEquals(obj);
            if (result.HasValue)
            {
                return result.Value;
            }

            return Equals(obj as CapturedPiece);
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
            return Util.GenericEquals(x, y);
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
        public CapturedPiece(Piece piece, int count)
        {
            Piece = piece;
            Count = count;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CapturedPiece(Piece piece)
            : this(piece, 0)
        {
        }
    }
}
