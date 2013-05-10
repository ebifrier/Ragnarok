using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Ragnarok.ObjectModel;

namespace Ragnarok.Presentation.Utility
{
    /// <summary>
    /// 将棋用の持ち時間などを保持します。
    /// </summary>
    [Serializable()]
    [DataContract()]
    public sealed class SimpleTimeSpan : NotifyObject, IEquatable<SimpleTimeSpan>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SimpleTimeSpan()
        {
            IsUse = true;
        }

        /// <summary>
        /// 時間を使用するかどうかを取得または設定します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public bool IsUse
        {
            get { return GetValue<bool>("IsUse"); }
            set { SetValue("IsUse", value); }
        }

        /// <summary>
        /// 持ち時間の分を取得または設定します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public int Minutes
        {
            get { return GetValue<int>("Minutes"); }
            set { SetValue("Minutes", value); }
        }

        /// <summary>
        /// 持ち時間の秒を取得または設定します。
        /// </summary>
        [DataMember(Order = 3, IsRequired = true)]
        public int Seconds
        {
            get { return GetValue<int>("Seconds"); }
            set { SetValue("Seconds", value); }
        }

        /// <summary>
        /// 時間間隔を取得します。
        /// </summary>
        public TimeSpan TimeSpan
        {
            get { return TimeSpan.FromSeconds(Minutes * 60 + Seconds); }
        }

        /// <summary>
        /// トータルの秒数を取得します。
        /// </summary>
        public int TotalSeconds
        {
            get { return (Minutes * 60 + Seconds); }
        }

        /// <summary>
        /// nullでない and IsUseが真 ならば真を返します。
        /// </summary>
        public static bool NotNullAndUse(SimpleTimeSpan value)
        {
            return (value != null && value.IsUse);
        }

        /// <summary>
        /// 文字列化します。
        /// </summary>
        public override string ToString()
        {
            return string.Format(
                "{0}分{1}秒",
                Minutes, Seconds);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public override bool Equals(object obj)
        {
            var result = this.PreEquals(obj);
            if (result != null)
            {
                return result.Value;
            }

            return Equals(obj as SimpleTimeSpan);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public bool Equals(SimpleTimeSpan other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return (
                IsUse == other.IsUse &&
                Minutes == other.Minutes &&
                Seconds == other.Seconds);
        }

        /// <summary>
        /// ハッシュ値を計算します。
        /// </summary>
        public override int GetHashCode()
        {
            return (
                IsUse.GetHashCode() ^
                Minutes.GetHashCode() ^
                Seconds.GetHashCode());
        }
    }
}
