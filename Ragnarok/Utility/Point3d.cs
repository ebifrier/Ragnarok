﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Ragnarok.Utility
{
    /// <summary>
    /// ３次元用のポイントクラスです。
    /// </summary>
    [TypeConverter(typeof(Point3dConverter))]
    public struct Point3d : IEquatable<Point3d>
    {
        private double x;
        private double y;
        private double z;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Point3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// X座標を取得または設定します。
        /// </summary>
        public double X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        /// <summary>
        /// Y座標を取得または設定します。
        /// </summary>
        public double Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        /// <summary>
        /// Z座標を取得または設定します。
        /// </summary>
        public double Z
        {
            get { return this.z; }
            set { this.z = value; }
        }

        /// <summary>
        /// 距離の二乗を取得します。
        /// </summary>
        public double Distance2
        {
            get { return ((X * X) + (Y * Y) + (Z * Z)); }
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
        public void Offset(double offsetX, double offsetY, double offsetZ)
        {
            X += offsetX;
            Y += offsetY;
            Z += offsetZ;
        }

        /// <summary>
        /// ポイントの座標値を加算した新たなポイントを作成します。
        /// </summary>
        public static Point3d Add(Point3d x, Point3d y)
        {
            return new Point3d(x.X + y.X, x.Y + y.Y, x.Z + y.Z);
        }

        /// <summary>
        /// ポイントの座標値を減算した新たなポイントを作成します。
        /// </summary>
        public static Point3d Subtract(Point3d x, Point3d y)
        {
            return new Point3d(x.X - y.X, x.Y - y.Y, x.Z - y.Z);
        }

        /// <summary>
        /// ポイントの座標値を加算した新たなポイントを作成します。
        /// </summary>
        public static Point3d operator +(Point3d x, Point3d y)
        {
            return Point3d.Add(x, y);
        }

        /// <summary>
        /// ポイントの座標値を減算した新たなポイントを作成します。
        /// </summary>
        public static Point3d operator -(Point3d x, Point3d y)
        {
            return Point3d.Subtract(x, y);
        }

        /// <summary>
        /// ハッシュ値を計算します。
        /// </summary>
        public override int GetHashCode()
        {
            return (X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode());
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

            return Equals((Point3d)obj);
        }

        /// <summary>
        /// オブジェクトが等値か検証します。
        /// </summary>
        public bool Equals(Point3d other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return (
                MathEx.Compare(X, other.X) == 0 &&
                MathEx.Compare(Y, other.Y) == 0 &&
                MathEx.Compare(Z, other.Z) == 0);
        }

        public static bool operator ==(Point3d lhs, Point3d rhs)
        {
            return Util.GenericEquals(lhs, rhs);
        }

        public static bool operator !=(Point3d lhs, Point3d rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// 文字列化します。
        /// </summary>
        public override string ToString()
        {
            return $"{X},{Y},{Z}";
        }

        /// <summary>
        /// 文字列からパースします。
        /// </summary>
        public static Point3d Parse(string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var list = source.Split(new char[] { ',' });
            if (list == null || list.Length != 3)
            {
                throw new FormatException(
                    "Point3d型への変換に失敗しました。");
            }

            var x = double.Parse(list[0], CultureInfo.InvariantCulture);
            var y = double.Parse(list[1], CultureInfo.InvariantCulture);
            var z = double.Parse(list[2], CultureInfo.InvariantCulture);
            return new Point3d(x, y, z);
        }
    }
}
