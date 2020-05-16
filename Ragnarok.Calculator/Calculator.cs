using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Codehaus.Parsec;

namespace Ragnarok.Calculator
{
    using Scanner = Parser<D_>;
    using Lexer = Parser<Tok>;
    using Lexeme = Parser<Tok[]>;
    using Binary = Map<double, double, double>;
    using Unary = Map<double, double>;
    using Term = Parser<object>;
    using Grammar = Parser<double>;

    /// <summary>
    /// 定数情報を登録します。
    /// </summary>
    internal class ConstantInfo
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConstantInfo(string name, double value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// 定数名を取得します。
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// 値を取得します。
        /// </summary>
        public double Value
        {
            get;
        }
    }

    /// <summary>
    /// 関数情報を登録します。
    /// </summary>
    internal class FunctionInfo
    {
        private readonly Func<double[], double> func;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FunctionInfo(string name, int parameterCount,
                            bool isVariableLengthParameter,
                            Func<double[], double> func)
        {
            Name = name;
            ParameterCount = parameterCount;
            IsVariableLengthParameter = isVariableLengthParameter;
            this.func = func;
        }

        /// <summary>
        /// 関数名を取得します。
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// パラメータの数を取得します。
        /// </summary>
        public int ParameterCount
        {
            get;
        }

        /// <summary>
        /// 可変長パラメータかどうかを取得または設定します。
        /// </summary>
        public bool IsVariableLengthParameter
        {
            get;
        }

        /// <summary>
        /// 実際に処理を行います。
        /// </summary>
        public double Apply(double[] parameters)
        {
            return this.func(parameters);
        }
    }

    /// <summary>
    /// 角度の単位を指定します。
    /// </summary>
    public enum AngleMode
    {
        /// <summary>
        /// 角度は使いません。
        /// </summary>
        None,
        /// <summary>
        /// 角度をラジアンで指定します。
        /// </summary>
        Radian,
        /// <summary>
        /// 角度を度で指定します。
        /// </summary>
        Degree,
    }

    /// <summary>
    /// 簡単な数式を計算します。
    /// </summary>
    /// <remarks>
    /// 関数の引数は通常','で区切られますが、
    /// カンマ記号が使えない特殊な環境で使うことも考え
    /// カンマ以外にも'#'で引数を区切ることができます。
    /// </remarks>
    public class Calculator
    {
        private static Calculator defaultValue;

        /// <summary>
        /// デフォルトのインスタンスです。
        /// 静的オブジェクトの初期化順序問題が発生しないようにしています。
        /// </summary>
        public static Calculator Default
        {
            get
            {
                if (defaultValue == null)
                {
                    defaultValue = new Calculator();
                }

                return defaultValue;
            }
        }

        /// <summary>
        /// デフォルトの定数リストです。
        /// </summary>
        static readonly ConstantInfo[] DefaultConstTable =
            new ConstantInfo[]
            {
                new ConstantInfo("e", Math.E),
                new ConstantInfo("pi", Math.PI),
            };

        /// <summary>
        /// デフォルトの関数リストです。
        /// </summary>
        static readonly FunctionInfo[] DefaultFuncTable =
            new FunctionInfo[]
            {
                new FunctionInfo("log", 1, false, _ => Math.Log(_[0])),
                new FunctionInfo("log10", 1, false, _ => Math.Log(_[0])),
                new FunctionInfo("abs", 1, false, _ => Math.Abs(_[0])),
                new FunctionInfo("max", 2, true, _ => _.Max()),
                new FunctionInfo("min", 2, true, _ => _.Min()),
                new FunctionInfo("leap", 3, false, _ => (_[0] * (1.0 - _[2]) + _[1] * _[2])),
                new FunctionInfo("rand", 0, false, _ => MathEx.RandDouble()),
                new FunctionInfo("rand", 1, false, _ => MathEx.RandDouble(_[0])),
                new FunctionInfo("rand", 2, false, _ => MathEx.RandDouble(_[0], _[1])),
            };

        /// <summary>
        /// ダミー
        /// </summary>
        static readonly FunctionInfo[] EmptyFuncTable = new FunctionInfo[0];

        /// <summary>
        /// ラジアン単位の角度関数リストです。
        /// </summary>
        static readonly FunctionInfo[] RadianFuncTable =
            new FunctionInfo[]
            {
                new FunctionInfo("sin", 1, false, _ => Math.Sin(_[0])),
                new FunctionInfo("cos", 1, false, _ => Math.Cos(_[0])),
                new FunctionInfo("tan", 1, false, _ => Math.Tan(_[0])),
                new FunctionInfo("asin", 1, false, _ => Math.Asin(_[0])),
                new FunctionInfo("acos", 1, false, _ => Math.Acos(_[0])),
                new FunctionInfo("atan", 1, false, _ => Math.Atan(_[0])),
                new FunctionInfo("atan2", 2, false, _ => Math.Atan2(_[0], _[2])),
            };

        /// <summary>
        /// 度単位の角度関数リストです。
        /// </summary>
        static readonly FunctionInfo[] DegreeFuncTable =
            new FunctionInfo[]
            {
                new FunctionInfo("sin", 1, false, _ => Math.Sin(MathEx.ToRad(_[0]))),
                new FunctionInfo("cos", 1, false, _ => Math.Cos(MathEx.ToRad(_[0]))),
                new FunctionInfo("tan", 1, false, _ => Math.Tan(MathEx.ToRad(_[0]))),
                new FunctionInfo("asin", 1, false, _ => MathEx.ToDeg(Math.Asin(_[0]))),
                new FunctionInfo("acos", 1, false, _ => MathEx.ToDeg(Math.Acos(_[0]))),
                new FunctionInfo("atan", 1, false, _ => MathEx.ToDeg(Math.Atan(_[0]))),
                new FunctionInfo("atan2", 2, false, _ => MathEx.ToDeg(Math.Atan2(_[0], _[2]))),
            };

        private readonly Grammar parser;
        private readonly ConstantInfo[] constTable;
        private readonly FunctionInfo[] funcTable;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Calculator(AngleMode angleMode)
        {
            var angleFuncTable =
                (angleMode == AngleMode.Radian ? RadianFuncTable
                : (angleMode == AngleMode.Degree ? DegreeFuncTable
                  : EmptyFuncTable));

            this.parser = CreateParser();
            this.constTable = DefaultConstTable;
            this.funcTable = DefaultFuncTable.Concat(angleFuncTable).ToArray();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Calculator()
            : this(AngleMode.Radian)
        {
        }

        /// <summary>
        /// 式を計算し、その値を返します。
        /// </summary>
        public double Run(string expression)
        {
            try
            {
                return Parsers.RunParser(
                    expression, this.parser, "calculate");
            }
            catch (Exception ex)
            {
                throw new RagnarokException(
                    $"'{expression}': 計算中にエラーが発生しました。", ex);
            }
        }

        /// <summary>
        /// 関数の計算を行います。
        /// </summary>
        private double CalcFunc(string name, double[] args)
        {
            args = args ?? new double[0];

            if (args.Length == 0)
            {
                // 名前が一致する定数値を探します。
                var constInfo = this.constTable
                    .Where(_ => string.Compare(name, _.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    .FirstOrDefault();
                if (constInfo != null)
                {
                    return constInfo.Value;
                }
            }

            // 一致する関数を探します。
            var funcInfo = this.funcTable
                .Where(_ => string.Compare(name, _.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
                .Where(_ => _.IsVariableLengthParameter ?
                    args.Length >= _.ParameterCount :
                    args.Length == _.ParameterCount)
                .FirstOrDefault();
            if (funcInfo != null)
            {
                return funcInfo.Apply(args);
            }

            throw new RagnarokException(
                $"'{name}': 適切な定数or関数が見つかりません。");
        }

        /// <summary>
        /// 新規にパーサーを作成します。
        /// </summary>
        private Grammar CreateParser()
        {
            var sDelim = Scanners.IsWhitespaces().Many_();
            var OPs = Terms.GetOperatorsInstance(
                "!", "+", "-", "**", "*", "/", "%",
                "==", "!=", "<=", ">=", "<", ">", "&&", "||",
                "(", ")", ",", "#");

            var lToken = OPs.Lexer | Lexers.LexDecimal() | Lexers.LexWord();
            var lexeme = Lexers.Lexeme(sDelim, lToken).FollowedBy(Parsers.Eof());
            
            var pNumber = Terms.OnDecimal((from, len, s) => double.Parse(s, CultureInfo.InvariantCulture));
            var pWord = Terms.OnWord((from, len, s) => s);
            Terms.FromSimpleToken<string, string>((from, len, s) => s);

            var pAnd = GetOperator(OPs, "&&", new Binary((a, b) => (a != 0 && b != 0) ? 1.0 : 0.0));
            var pOr = GetOperator(OPs, "||", new Binary((a, b) => (a != 0 || b != 0) ? 1.0 : 0.0));

            var pEQ = GetOperator(OPs, "==", new Binary((a, b) => (a == b) ? 1.0 : 0.0));
            var pNQ = GetOperator(OPs, "!=", new Binary((a, b) => (a != b) ? 1.0 : 0.0));
            var pLE = GetOperator(OPs, "<=", new Binary((a, b) => (a <= b) ? 1.0 : 0.0));
            var pL = GetOperator(OPs, "<", new Binary((a, b) => (a < b) ? 1.0 : 0.0));
            var pGE = GetOperator(OPs, ">=", new Binary((a, b) => (a >= b) ? 1.0 : 0.0));
            var pG = GetOperator(OPs, ">", new Binary((a, b) => (a > b) ? 1.0 : 0.0));

            var pPlus = GetOperator(OPs, "+", new Binary((a, b) => (a + b)));
            var pMinus = GetOperator(OPs, "-", new Binary((a, b) => (a - b)));

            var pMul = GetOperator(OPs, "*", new Binary((a, b) => (a * b)));
            var pDiv = GetOperator(OPs, "/", new Binary((a, b) => (a / b)));
            var pMod = GetOperator(OPs, "%", new Binary((a, b) => (a % b)));

            var pPow = GetOperator(OPs, "**", new Binary((a, b) => Math.Pow(a, b)));

            var pNone = GetOperator(OPs, "+", new Unary(n => n));
            var pNeg = GetOperator(OPs, "-", new Unary(n => -n));
            var pNot = GetOperator(OPs, "!", new Unary(n => (n == 0 ? 1.0 : 0.0)));

            var opTable = new OperatorTable<double>()
                .Infixl(pAnd, 10).Infixl(pOr, 10)
                .Infixl(pEQ, 20).Infixl(pNQ, 20)
                .Infixl(pLE, 20).Infixl(pL, 20).Infixl(pGE, 20).Infixl(pG, 20)
                .Infixl(pPlus, 40).Infixl(pMinus, 40)
                .Infixl(pMul, 50).Infixl(pDiv, 50).Infixl(pMod, 50)
                .Infixr(pPow, 60)
                .Prefix(pNone, 70).Prefix(pNeg, 70).Prefix(pNot, 70);

            var pLParen = OPs.GetParser("(");
            var pRParen = OPs.GetParser(")");
            var pComma = OPs.GetParser(new string[] { ",", "#" });

            var lazyExpr = new Grammar[1];
            var pLazyExpr = Parsers.Lazy<double>(() => lazyExpr[0]);
            var pArg
                = pLazyExpr.SepEndBy(pComma).Between(pLParen, pRParen)
                | pLParen.Seq(pRParen).Seq(Parsers.Return(new double[0]));
            var pTerm
                = pLazyExpr.Between(pLParen, pRParen)
                | pWord.And(pArg.Optional(), new Map<string, double[], double>(CalcFunc))
                | pNumber;

            var pExpr = Expressions.BuildExpressionParser(pTerm, opTable);
            lazyExpr[0] = pExpr;
            return Parsers.ParseTokens(lexeme, pExpr.FollowedBy(Parsers.Eof()), "calculator");
        }

        /// <summary>
        /// 演算子を処理するパーサーを作成します。
        /// </summary>
        static Parser<T> GetOperator<T>(Terms ops, string op, T v)
        {
            return ops.GetParser(op).Seq(Parsers.Return(v));
        }
    }
}

