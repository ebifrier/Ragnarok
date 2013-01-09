using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Codehaus.Parsec;

namespace Ragnarok.Utility
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
    internal struct ConstantInfo
    {
        private readonly string name;
        private readonly double value;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConstantInfo(string name, double value)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// 定数名を取得します。
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// 値を取得します。
        /// </summary>
        public double Value
        {
            get { return this.value; }
        }
    }

    /// <summary>
    /// 関数情報を登録します。
    /// </summary>
    internal struct FunctionInfo
    {
        private readonly Func<double[], double> func;
        private readonly string name;
        private readonly int parameterCount;
        private readonly bool isVariableLengthParameter;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FunctionInfo(string name, int parameterCount,
                            bool isVariableLengthParameter,
                            Func<double[], double> func)
        {
            this.name = name;
            this.parameterCount = parameterCount;
            this.isVariableLengthParameter = isVariableLengthParameter;
            this.func = func;
        }

        /// <summary>
        /// 関数名を取得します。
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// パラメータの数を取得します。
        /// </summary>
        public int ParameterCount
        {
            get { return this.parameterCount; }
        }

        /// <summary>
        /// 可変長パラメータかどうかを取得または設定します。
        /// </summary>
        public bool IsVariableLengthParameter
        {
            get { return this.isVariableLengthParameter; }
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
                new FunctionInfo("abs", 2, false, _ => Math.Abs(_[0])),
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

            this.parser = CreateParser(angleMode);
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
                    "計算中にエラーが発生しました。", ex);
            }
        }

        /// <summary>
        /// 関数の計算を行います。
        /// </summary>
        private double CalcFunc(string name, double[] args)
        {
            args = args ?? new double[0];

            foreach (var info in this.constTable)
            {
                if (string.Compare(name, info.Name, true) == 0)
                {
                    var isCorrent = (args.Count() == 0);

                    if (isCorrent)
                    {
                        return info.Value;
                    }
                }
            }

            foreach (var info in this.funcTable)
            {
                if (string.Compare(name, info.Name, true) == 0)
                {
                    var isCorrent =
                        (info.IsVariableLengthParameter ?
                         args.Count() >= info.ParameterCount :
                         args.Count() == info.ParameterCount);
                    
                    if (isCorrent)
                    {
                        return info.Apply(args);
                    }
                }
            }

            throw new RagnarokException(string.Format(
                "'{0}': 適切な定数or関数が見つかりません。", name));
        }

        /// <summary>
        /// 新規にパーサーを作成します。
        /// </summary>
        private Grammar CreateParser(AngleMode angleMode)
        {
            //var x = Patterns.Regex("[a-zA-Z_][0-9a-zA-Z_]*");
            var sDelim = Scanners.IsWhitespaces().Many_();
            var OPs = Terms.GetOperatorsInstance(
                "+", "-", "**", "*", "/", "%", "(", ")", ",");

            var lToken = OPs.Lexer | Lexers.LexDecimal() | Lexers.LexWord();
            var lexeme = Lexers.Lexeme(sDelim, lToken).FollowedBy(Parsers.Eof());
            
            var pNumber = Terms.OnDecimal((from, len, s) => double.Parse(s));
            var pWord = Terms.OnWord((from, len, s) => s);
            Terms.FromSimpleToken<string, string>((from, len, s) => s);

            var pPlus = GetOperator(OPs, "+", new Binary((a, b) => (a + b)));
            var pMinus = GetOperator(OPs, "-", new Binary((a, b) => (a - b)));
            var pMul = GetOperator(OPs, "*", new Binary((a, b) => (a * b)));
            var pDiv = GetOperator(OPs, "/", new Binary((a, b) => (a / b)));
            var pMod = GetOperator(OPs, "%", new Binary((a, b) => (a % b)));
            var pPow = GetOperator(OPs, "**", new Binary((a, b) => Math.Pow(a, b)));
            var pNeg = GetOperator(OPs, "-", new Unary(n => -n));
            var opTable = new OperatorTable<double>()
                .Infixl(pPlus, 10)
                .Infixl(pMinus, 10)
                .Infixl(pMul, 20)
                .Infixl(pDiv, 20)
                .Infixl(pMod, 20)
                .Infixr(pPow, 30)
                .Prefix(pNeg, 40);

            var pLParen = OPs.GetParser("(");
            var pRParen = OPs.GetParser(")");
            var pComma = OPs.GetParser(",");

            var lazyExpr = new Grammar[1];
            var pLazyExpr = Parsers.Lazy<double>(() => lazyExpr[0]);
            var pArg
                = pLazyExpr.SepEndBy(pComma).Between(pLParen, pRParen)
                | pLParen.Seq(pRParen).Seq(Parsers.Return(new double[0]));
            var pTerm
                = pLazyExpr.Between(pLParen, pRParen)
                | pWord.And(pArg.Optional(), (Map<string, double[], double>)CalcFunc)
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

