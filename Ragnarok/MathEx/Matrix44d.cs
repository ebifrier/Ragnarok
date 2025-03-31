using System;
using System.Collections.Generic;
using System.Drawing;

namespace Ragnarok.MathEx
{
    /// <summary>
    /// 行列用の例外クラスです。
    /// </summary>
    public class MatrixException : Exception
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MatrixException()
            : base()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MatrixException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MatrixException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// 4x4のdouble型行列を定義します。
    /// </summary>
    public class Matrix44d
    {
        /// <summary>
        /// 単位行列
        /// </summary>
        public static readonly Matrix44d Identity = new();
        /// <summary>
        /// 行列の行の数を取得します。
        /// </summary>
        public static readonly int Rows = 4;
        /// <summary>
        /// 行列の列の数を取得します。
        /// </summary>
        public static readonly int Columns = 4;

        private readonly double[,] values;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Matrix44d()
        {
            this.values = new double[4, 4];
            this.values[0, 0] = 1.0;
            this.values[1, 1] = 1.0;
            this.values[2, 2] = 1.0;
            this.values[3, 3] = 1.0;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Matrix44d(double[,] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            this.values = new double[4, 4];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    this.values[i, j] = values[i, j];
                }
            }
        }

        /// <summary>
        /// オブジェクトのクローンを作成します。
        /// </summary>
        public Matrix44d Clone()
        {
            return new Matrix44d(this.values);
        }

        /// <summary>
        /// 行列の各値を取得または設定します。
        /// </summary>
        public double this[int row, int col]
        {
            get { return this.values[row, col]; }
            set { this.values[row, col] = value; }
        }

        /// <summary>
        /// 行列が単位行列かどうか取得します。
        /// </summary>
        public bool IsIdentity
        {
            get { return (this == Identity); }
        }

        /// <summary>
        /// 逆行列が存在するか取得します。
        /// </summary>
        public bool HasInverse
        {
            get { return (Math.Abs(Determinant()) > 1E-12); }
        }

        /// <summary>
        /// 各値を配列として取得します。
        /// </summary>
        public double[,] AsArray
        {
            get { return this.values; }
        }

        /// <summary>
        /// 行列を単位行列化します。
        /// </summary>
        public void SetIdentity()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this[i, j] = (i == j ? 1 : 0);
                }
            }
        }

        /// <summary>
        /// 行列の行と列を入れ替えます。
        /// </summary>
        public void Transpose()
        {
            if (ReferenceEquals(this, Identity))
            {
                throw new MatrixException(
                    "Identity matrix could not be changed.");
            }

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (i < j)
                    {
                        var tmp = this[i, j];
                        this[i, j] = this[j, i];
                        this[j, i] = tmp;
                    }
                }
            }
        }

        /// <summary>
        /// 行列の比較を行います。
        /// </summary>
        public override bool Equals(object obj)
        {
            var status = this.PreEquals(obj);
            if (status != null)
            {
                return status.Value;
            }

            return Equals((Matrix44d)obj);
        }

        /// <summary>
        /// ハッシュ値を計算します。
        /// </summary>
        public override int GetHashCode()
        {
            var result = 0;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    result ^= this[i, j].GetHashCode();
                }
            }

            return result;
        }

        /// <summary>
        /// 行列の比較を行います。
        /// </summary>
        public bool Equals(Matrix44d other)
        {
            if (other is null)
            {
                return false;
            }

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (MathUtil.Compare(this[i, j], other[i, j]) != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 行列の比較を行います。
        /// </summary>
        public static bool operator ==(Matrix44d mat1, Matrix44d mat2)
        {
            return Util.GenericEquals(mat1, mat2);
        }

        /// <summary>
        /// 行列の比較を行います。
        /// </summary>
        public static bool operator !=(Matrix44d mat1, Matrix44d mat2)
        {
            return !(mat1 == mat2);
        }

        /// <summary>
        /// X座標の値のみに座標変換を行います。他の座標は0として計算されます。
        /// </summary>
        public double TransformX(double x)
        {
            return (x * this[0, 0] + this[0, 3]);
        }

        /// <summary>
        /// Y座標の値のみに座標変換を行います。他の座標は0として計算されます。
        /// </summary>
        public double TransformY(double y)
        {
            return (y * this[1, 1] + this[1, 3]);
        }

        /// <summary>
        /// Z座標の値のみに座標変換を行います。他の座標は0として計算されます。
        /// </summary>
        public double TransformZ(double z)
        {
            return (z * this[2, 2] + this[2, 3]);
        }

        /// <summary>
        /// <paramref name="p"/>の値に座標変換を行います。
        /// </summary>
        public Pointd Transform(Pointd p)
        {
            return new Pointd(
                p.X * this[0, 0] + p.Y * this[0, 1] + this[0, 3],
                p.X * this[1, 0] + p.Y * this[1, 1] + this[1, 3]);
        }

        /// <summary>
        /// <paramref name="p"/>の値に座標変換を行います。
        /// </summary>
        public Point3d Transform(Point3d p)
        {
            return new Point3d(
                p.X * this[0, 0] + p.Y * this[0, 1] + p.Z * this[0, 2] + this[0, 3],
                p.X * this[1, 0] + p.Y * this[1, 1] + p.Z * this[1, 2] + this[1, 3],
                p.X * this[2, 0] + p.Y * this[2, 1] + p.Z * this[2, 2] + this[2, 3]);
        }

        #region Row/Column major
        /// <summary>
        /// 矩形からその変換行列を作成します。
        /// </summary>
        public static Matrix44d FromRectangle(Rectangle rect)
        {
            var matrix = new Matrix44d();
            matrix.Translate(
                rect.Left + rect.Width / 2,
                rect.Top + rect.Height / 2,
                0.0);
            matrix.Scale(rect.Width, rect.Height, 1.0);
            return matrix;
        }

        /// <summary>
        /// 矩形からその変換行列を作成します。
        /// </summary>
        public static Matrix44d FromRectangle(RectangleF rect)
        {
            var matrix = new Matrix44d();
            matrix.Translate(
                rect.Left + rect.Width / 2,
                rect.Top + rect.Height / 2,
                0.0);
            matrix.Scale(rect.Width, rect.Height, 1.0);
            return matrix;
        }

        /// <summary>
        /// 行優先の配列から行列を作成します。
        /// </summary>
        public static Matrix44d FromRowMajorArray(double[] rowMajorArray)
        {
            if (rowMajorArray == null)
            {
                throw new ArgumentNullException(nameof(rowMajorArray));
            }

            var matrix = new Matrix44d();
            int index = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    matrix[i, j] = rowMajorArray[index++];
                }
            }

            return matrix;
        }

        /// <summary>
        /// 列優先の配列から行列を作成します。
        /// </summary>
        public static Matrix44d FromColumnMajorArray(double[] columnMajorArray)
        {
            if (columnMajorArray == null)
            {
                throw new ArgumentNullException(nameof(columnMajorArray));
            }

            var matrix = new Matrix44d();
            int index = 0;
            for (int j = 0; j < Columns; j++)
            {
                for (int i = 0; i < Rows; i++)
                {
                    matrix[i, j] = columnMajorArray[index++];
                }
            }

            return matrix;
        }

        /// <summary>
        /// 行列を列優先の数字列に変換します。
        /// </summary>
        public double[] AsRowMajorArray
        {
            get
            {
                var ar = new double[Rows * Columns];
                var index = 0;

                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        ar[index++] = this[i, j];
                    }
                }

                return ar;
            }
        }

        /// <summary>
        /// 行列を行優先の数字列に変換します。
        /// </summary>
        public double[] AsColumnMajorArray
        {
            get
            {
                var ar = new double[Rows * Columns];
                var index = 0;

                for (int j = 0; j < Columns; j++)
                {
                    for (int i = 0; i < Rows; i++)
                    {
                        ar[index++] = this[i, j];
                    }
                }

                return ar;
            }
        }
        #endregion

        #region Add / Subtract / Multiply / Divide
        /// <summary>
        /// 行列を加算します。
        /// </summary>
        public void Add(Matrix44d mat)
        {
            if (mat == null)
            {
                throw new ArgumentNullException(nameof(mat));
            }

            if (ReferenceEquals(this, Identity))
            {
                throw new MatrixException(
                    "Identity matrix could not be changed.");
            }

            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    this[i, j] += mat[i, j];
                }
            }
        }

        /// <summary>
        /// 行列同士を加算した新たな行列を作成します。
        /// </summary>
        public static Matrix44d operator +(Matrix44d mat1, Matrix44d mat2)
        {
            if (mat1 == null)
            {
                throw new ArgumentNullException(nameof(mat1));
            }

            var result = mat1.Clone();
            result.Add(mat2);
            return result;
        }

        /// <summary>
        /// 行列を減算します。
        /// </summary>
        public void Subtract(Matrix44d mat)
        {
            if (mat == null)
            {
                throw new ArgumentNullException(nameof(mat));
            }

            if (ReferenceEquals(this, Identity))
            {
                throw new MatrixException(
                    "Identity matrix could not be changed.");
            }

            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    this[i, j] -= mat[i, j];
                }
            }
        }

        /// <summary>
        /// 行列同士を減算した新たな行列を作成します。
        /// </summary>
        public static Matrix44d operator -(Matrix44d mat1, Matrix44d mat2)
        {
            if (mat1 == null)
            {
                throw new ArgumentNullException(nameof(mat1));
            }

            var result = mat1.Clone();
            result.Subtract(mat2);
            return result;
        }

        /// <summary>
        /// 行列にスカラ値を積算します。
        /// </summary>
        public void Multiply(double scale)
        {
            if (ReferenceEquals(this, Identity))
            {
                throw new MatrixException(
                    "Identity matrix could not be changed.");
            }

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    this[i, j] *= scale;
                }
            }
        }

        /// <summary>
        /// 行列とスカラ値を積算した新たな行列を作成します。
        /// </summary>
        public static Matrix44d operator *(Matrix44d mat, double scala)
        {
            if (mat == null)
            {
                throw new ArgumentNullException(nameof(mat));
            }

            var result = mat.Clone();
            result.Multiply(scala);
            return result;
        }

        /// <summary>
        /// 行列とスカラ値を積算した新たな行列を作成します。
        /// </summary>
        public static Matrix44d operator *(double scala, Matrix44d mat)
        {
            if (mat == null)
            {
                throw new ArgumentNullException(nameof(mat));
            }

            var result = mat.Clone();
            result.Multiply(scala);
            return result;
        }

        /// <summary>
        /// 行列にスカラ値を除算します。
        /// </summary>
        public void Divide(double scale)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    this[i, j] /= scale;
                }
            }
        }

        /// <summary>
        /// 行列とスカラ値を除算した新たな行列を作成します。
        /// </summary>
        public static Matrix44d operator /(Matrix44d mat, double scala)
        {
            if (mat == null)
            {
                throw new ArgumentNullException(nameof(mat));
            }

            var result = mat.Clone();
            result.Divide(scala);
            return result;
        }

        /// <summary>
        /// 行列を積算します。
        /// </summary>
        public void Multiply(Matrix44d mat)
        {
            if (mat == null)
            {
                throw new ArgumentNullException(nameof(mat));
            }

            if (ReferenceEquals(this, Identity))
            {
                throw new MatrixException(
                    "Identity matrix could not be changed.");
            }

            var clone = Clone();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    var value = 0.0;
                    for (int k = 0; k < Rows; k++)
                    {
                        value += clone[i, k] * mat[k, j];
                    }

                    this[i, j] = value;
                }
            }
        }

        /// <summary>
        /// 前に行列を積算します。
        /// </summary>
        public void Prepend(Matrix44d mat)
        {
            if (mat == null)
            {
                throw new ArgumentNullException(nameof(mat));
            }

            if (ReferenceEquals(this, Identity))
            {
                throw new MatrixException(
                    "Identity matrix could not be changed.");
            }

            var clone = Clone();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    var value = 0.0;
                    for (int k = 0; k < Rows; k++)
                    {
                        value += mat[i, k] * clone[k, j];
                    }

                    this[i, j] = value;
                }
            }
        }

        /// <summary>
        /// 行列同士を積算した新たな行列を作成します。
        /// </summary>
        public static Matrix44d operator *(Matrix44d mat1, Matrix44d mat2)
        {
            if (mat1 == null)
            {
                throw new ArgumentNullException(nameof(mat1));
            }

            var result = mat1.Clone();
            result.Multiply(mat2);
            return result;
        }
        #endregion

        #region Inverse
        /// <summary>
        /// 行列式を取得します。
        /// </summary>
        /// <remarks>
        /// http://thira.plavox.info/blog/2008/06/_c.html#sthash.swdMv7Xn.dpuf
        /// </remarks>
        public double Determinant()
        {
            var clone = Clone();
            var det = 1.0;

            for (var i = 0; i < Rows; ++i)
            {
                for (var j = 0; j < Columns; ++j)
                {
                    if (i < j)
                    {
                        var tmp = clone[i, j] / clone[i, i];
                        for (var k = 0; k < Rows; ++k)
                        {
                            clone[k, j] -= clone[k, i] * tmp;
                        }
                    }
                }
            }

            // 対角部分の積
            for (var i = 0; i < Rows; ++i)
            {
                det *= clone[i, i];
            }

            return det;
        }

        /// <summary>
        /// 掃き出し法による逆行列の計算を行います。
        /// </summary>
        /// <remarks>
        /// http://www.asahi-net.or.jp/~uc3k-ymd/Lesson/Section03/invmat.html
        /// </remarks>
        public Matrix44d Invert()
        {
            if (!HasInverse)
            {
                throw new MatrixException(
                    "逆行列の計算ができません。");
            }

            var inv = new Matrix44d();
            var clone = Clone();

            for (var i = 0; i < Rows; ++i)
            {
                var tmp = 1.0 / clone[i, i];
                for (var j = 0; j < Rows; ++j)
                {
                    clone[i, j] *= tmp;
                    inv[i, j] *= tmp;
                }

                for (var j = 0; j < Rows; ++j)
                {
                    if (i != j)
                    {
                        var tmp2 = clone[j, i];
                        for (var k = 0; k < Columns; ++k)
                        {
                            clone[j, k] -= clone[i, k] * tmp2;
                            inv[j, k] -= inv[i, k] * tmp2;
                        }
                    }
                }
            }

            return inv;
        }
        #endregion

        #region Translate / Scale / Rotate
        /// <summary>
        /// 行列を平行移動します。
        /// </summary>
        public void Translate(double offsetX, double offsetY, double offsetZ)
        {
            var m = new Matrix44d();
            m[0, 3] = offsetX;
            m[1, 3] = offsetY;
            m[2, 3] = offsetZ;

            // 最後に行列を積算します。
            Multiply(m);
        }

        /// <summary>
        /// 行列を拡大縮小します。
        /// </summary>
        public void Scale(double scaleX, double scaleY, double scaleZ)
        {
            var m = new Matrix44d();
            m[0, 0] = scaleX;
            m[1, 1] = scaleY;
            m[2, 2] = scaleZ;

            // 最後に行列を積算します。
            Multiply(m);
        }

        /// <summary>
        /// 行列を拡大縮小します。
        /// </summary>
        /// <remarks>
        /// http://www.cg.info.hiroshima-cu.ac.jp/~miyazaki/knowledge/tech07.html
        /// </remarks>
        public void Rotate(double angle, double axisX, double axisY, double axisZ)
        {
            var m = new Matrix44d();
            var c = Math.Cos(angle);
            var s = Math.Sin(angle);
            var t = 1.0 - c;

            var axis = new Point3d(axisX, axisY, axisZ);
            //axis.Normalize();

            // intermediate values
            var tx = t * axis.X; var ty = t * axis.Y; var tz = t * axis.Z;
            var sx = s * axis.X; var sy = s * axis.Y; var sz = s * axis.Z;
            var txy = tx * axis.Y; var tyz = ty * axis.Z; var tzx = tz * axis.X;

            // set matrix
            m[0, 0] = tx * axis.X + c;
            m[0, 1] = txy - sz;
            m[0, 2] = tzx + sy;
            m[0, 3] = 0.0;

            m[1, 0] = txy + sz;
            m[1, 1] = ty * axis.Y + c;
            m[1, 2] = tyz - sx;
            m[1, 3] = 0.0;

            m[2, 0] = tzx - sy;
            m[2, 1] = tyz + sx;
            m[2, 2] = tz * axis.Z + c;
            m[2, 3] = 0.0;

            m[3, 0] = 0.0;
            m[3, 1] = 0.0;
            m[3, 2] = 0.0;
            m[3, 3] = 1.0;
            
            // 最後に行列を積算します。
            Multiply(m);
        }
        #endregion
    }
}
