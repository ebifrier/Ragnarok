using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Ragnarok.MathEx
{
    /// <summary>
    /// ポイントクラスです。
    /// </summary>
    [TypeConverter(typeof(PointdConverter))]
    public struct Pointd : IEquatable<Pointd>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Pointd(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// X座標を取得または設定します。
        /// </summary>
        public double X
        {
            get;
            set;
        }

        /// <summary>
        /// Y座標を取得または設定します。
        /// </summary>
        public double Y
        {
            get;
            set;
        }

        /// <summary>
        /// 距離の二乗を取得します。
        /// </summary>
        public double Distance2
        {
            get { return ((X * X) + (Y * Y)); }
        }

        /// <summary>
        /// 距離を取得します。
        /// </summary>
        public double Distance
        {
            get { return Math.Sqrt(Distance2); }
        }

        /// <summary>
        /// 座標値を指定した分だけ移動させます。
        /// </summary>
        public void Offset(double offsetX, double offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        /// <summary>
        /// ポイントの座標値を加算した新たなポイントを作成します。
        /// </summary>
        public static Pointd Add(Pointd x, Pointd y)
        {
            return new Pointd(x.X + y.X, x.Y + y.Y);
        }

        /// <summary>
        /// ポイントの座標値を減算した新たなポイントを作成します。
        /// </summary>
        public static Pointd Subtract(Pointd x, Pointd y)
        {
            return new Pointd(x.X - y.X, x.Y - y.Y);
        }

        /// <summary>
        /// ポイントの座標値を加算した新たなポイントを作成します。
        /// </summary>
        public static Pointd operator +(Pointd x, Pointd y)
        {
            return Pointd.Add(x, y);
        }

        /// <summary>
        /// ポイントの座標値を減算した新たなポイントを作成します。
        /// </summary>
        public static Pointd operator -(Pointd x, Pointd y)
        {
            return Pointd.Subtract(x, y);
        }

        /// <summary>
        /// ハッシュ値を計算します。
        /// </summary>
        public override int GetHashCode()
        {
            return (X.GetHashCode() ^ Y.GetHashCode());
        }

        /// <summary>
        /// オブジェクトが等値か検証します。
        /// </summary>
        public override bool Equals(object obj)
        {
            var status = this.PreEquals(obj);
            if (status != null)
            {
                return status.Value;
            }

            return Equals((Pointd)obj);
        }

        /// <summary>
        /// オブジェクトが等値か検証します。
        /// </summary>
        public bool Equals(Pointd other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return (
                MathUtil.Compare(X, other.X) == 0 &&
                MathUtil.Compare(Y, other.Y) == 0);
        }

        public static bool operator ==(Pointd lhs, Pointd rhs)
        {
            return Util.GenericEquals(lhs, rhs);
        }

        public static bool operator !=(Pointd lhs, Pointd rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// 文字列化します。
        /// </summary>
        public override string ToString()
        {
            return $"{X},{Y}";
        }

        /// <summary>
        /// 文字列からパースします。
        /// </summary>
        public static Pointd Parse(string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var list = source.Split(new char[] { ',' });
            if (list == null || list.Length != 2)
            {
                throw new FormatException(
                    "Point3d型への変換に失敗しました。");
            }

            var x = double.Parse(list[0], CultureInfo.InvariantCulture);
            var y = double.Parse(list[1], CultureInfo.InvariantCulture);
            return new Pointd(x, y);
        }
    }
}
