using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml;

using Ragnarok.Shogi;

namespace Ragnarok.Presentation.Shogi.Effects
{
    /// <summary>
    /// エフェクトの各引数を管理します。
    /// </summary>
    public sealed class EffectArgument
    {
        /// <summary>
        /// 引数名を取得または設定します。
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 型を取得または設定します。
        /// </summary>
        public Type Type
        {
            get;
            set;
        }

        /// <summary>
        /// デフォルト値を取得または設定します。
        /// </summary>
        public string DefaultValue
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EffectArgument(string name, Type type, string defaultValue)
        {
            Name = name;
            Type = type;
            DefaultValue = defaultValue;
        }
    }

    /// <summary>
    /// エフェクトを管理します。
    /// </summary>
    public sealed class EffectInfo
    {
        /// <summary>
        /// エフェクトファイルがある基本パスを取得または設定します。
        /// </summary>
        public static Uri EffectBaseDir = new Uri(
            new Uri(Assembly.GetEntryAssembly().CodeBase),
            "ShogiData/Effect");

        /// <summary>
        /// 背景エフェクトファイルがある基本パスを取得または設定します。
        /// </summary>
        public static Uri BackgroundBaseDir = new Uri(
            new Uri(Assembly.GetEntryAssembly().CodeBase),
            "ShogiData/Background");

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EffectInfo(string name, string dir)
        {
            Name = name;
            Directory = dir;
            Arguments = new List<EffectArgument>();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EffectInfo(string name, string dir, List<EffectArgument> arguments)
        {
            Name = name;
            Directory = dir;
            Arguments = arguments ?? new List<EffectArgument>();
        }

        /// <summary>
        /// エフェクトの名前(ファイルの名前)を取得または設定します。
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// エフェクトがあるディレクトリを取得または設定します。
        /// </summary>
        public string Directory
        {
            get;
            private set;
        }

        /// <summary>
        /// エフェクトの各引数を取得します。
        /// </summary>
        public List<EffectArgument> Arguments
        {
            get;
            private set;
        }

        /// <summary>
        /// エフェクトファイルの置いてあるパスを順次取得します。
        /// </summary>
        private IEnumerable<string> EnumeratePath(Uri baseDir)
        {
            var dirPath =
                (string.IsNullOrEmpty(Directory)
                ? baseDir.LocalPath
                : Path.Combine(baseDir.LocalPath, Directory));

            yield return Path.Combine(dirPath, Name, Name + ".xaml");
            yield return Path.Combine(dirPath, Name, "Effect.xaml");
            yield return Path.Combine(dirPath, Name + ".xaml");
        }

        /// <summary>
        /// 置換テーブルを使って、テキストを置き換えます。
        /// </summary>
        private static string ReplaceTable(string text,
                                           Dictionary<string, object> table)
        {
            if (table == null)
            {
                return text;
            }

            foreach (var pair in table)
            {
                if (string.IsNullOrEmpty(pair.Key))
                {
                    continue;
                }

                var value = (pair.Value == null ? "" : pair.Value.ToString());
                text = text.Replace("${" + pair.Key + "}", value);
            }

            return text;
        }

        /// <summary>
        /// エフェクトをファイルから読み込みます。
        /// </summary>
        /// <remarks>
        /// エフェクトファイルは以下のファイルから検索されます。
        /// 
        /// １、{基本パス}/{ディレクトリ名}/{エフェクト名}/{エフェクト名}.xaml
        /// ２、{基本パス}/{ディレクトリ名}/{エフェクト名}/Effect.xaml
        /// ３、{基本パス}/{ディレクトリ名}/{エフェクト名}.xaml
        /// </remarks>
        private EffectObject Load(Uri baseDir, Dictionary<string, object> table)
        {
            try
            {
                var path = EnumeratePath(baseDir)
                    .FirstOrDefault(_ => File.Exists(_));
                if (string.IsNullOrEmpty(path))
                {
                    throw new ArgumentException(
                        "エフェクトが見つかりません。", "effect");
                }

                byte[] bytes = null;
                if (table == null || !table.Any())
                {
                    bytes = Util.ReadFile(path);
                }
                else
                {
                    // ファイル中の変数を置き換えます。
                    var text = Util.ReadFile(path, Encoding.UTF8);
                    text = ReplaceTable(text, table);

                    bytes = Encoding.UTF8.GetBytes(text);
                }

                using (var stream = new MemoryStream(bytes))
                {
                    var context = new ParserContext()
                    {
                        BaseUri = new Uri(path, UriKind.Absolute),
                    };
                    var resource = (ResourceDictionary)XamlReader.Load(stream, context);

                    // エフェクトの名前は'Effect'であると仮定します。
                    var effect = (EffectObject)resource["Effect"];
                    if (effect != null)
                    {
                        effect.Name = Name;
                    }

                    return effect;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "エフェクト'{0}'の読み込みに失敗しました。", Name);

                return null;
            }
        }

        /// <summary>
        /// エフェクトをファイルから読み込みます。
        /// </summary>
        public EffectObject LoadEffect(Dictionary<string, object> table = null)
        {
            return Load(EffectBaseDir, table);
        }

        /// <summary>
        /// 読み込み時間短縮のための事前読み込みを行います。
        /// </summary>
        public void PreLoad()
        {
            var effect = LoadEffectDefault();

            if (effect != null)
            {
                effect.PlayStartSound(true);
            }
        }

        /// <summary>
        /// エフェクトをデフォルトの引数を使って読み込みます。
        /// </summary>
        public EffectObject LoadEffectDefault()
        {
            var dic = Arguments.ToDictionary(
                _ => _.Name,
                _ => (object)_.DefaultValue);

            return LoadEffect(dic);
        }

        /// <summary>
        /// バックグラウンドエフェクトをファイルから読み込みます。
        /// </summary>
        public EffectObject LoadBackground()
        {
            return Load(BackgroundBaseDir, null);
        }

        /// <summary>
        /// XAMLの事前読み込みを行います。
        /// </summary>
        /// <remarks>
        /// XAML読み込みは初回の読み込みに多大な時間がかかります。
        /// 例）
        /// １回目:100ms、２回目:1ms
        /// 
        /// そのため、全エフェクトを一度事前に読み込みます。
        /// </remarks>
        public static void InitializeEffect(Type effectTableType)
        {
            if (effectTableType == null)
            {
                throw new ArgumentNullException("effectTableType");
            }

            try
            {
                effectTableType.GetFields()
                    .Where(_ => _.FieldType == typeof(EffectInfo))
                    .Select(_ => (EffectInfo)_.GetValue(null))
                    .Where(_ => _ != null)
                    .ForEach(_ => _.PreLoad());
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "エフェクトの初期化に失敗しました。");
            }
        }
    }

    /*/// <summary>
    /// 読み込み可能なエフェクトのリストです。
    /// </summary>
    public static class EffectTable
    {
        #region Cell
        /// <summary>
        /// 駒の動かせるマスを表示します。
        /// </summary>
        public readonly static EffectInfo MovableCell = new EffectInfo(
            "MovableCellEffect", "Cell");

        /// <summary>
        /// 以前に移動させた駒を表示します。
        /// </summary>
        public readonly static EffectInfo PrevMovedCell = new EffectInfo(
            "PrevMovedCellEffect", "Cell");

        /// <summary>
        /// 手番のある側を表示します。
        /// </summary>
        public readonly static EffectInfo Teban = new EffectInfo(
            "TebanEffect", "Cell");
        #endregion

        #region Piece
        /// <summary>
        /// 駒が動いたときのエフェクトです。
        /// </summary>
        public readonly static EffectInfo PieceMove = new EffectInfo(
            "PieceMoveEffect", "Piece",
            new List<EffectArgument>
            {
                new EffectArgument("Color", typeof(Color), "#ffffffff"),
            });

        /// <summary>
        /// 駒を打ったときのエフェクトです。
        /// </summary>
        public readonly static EffectInfo PieceDrop = new EffectInfo(
            "PieceDropEffect", "Piece");

        /// <summary>
        /// 駒が成ったときのエフェクトです。
        /// </summary>
        public readonly static EffectInfo Promote = new EffectInfo(
            "PromoteEffect", "Piece");

        /// <summary>
        /// 駒を取ったときのエフェクトです。
        /// </summary>
        public readonly static EffectInfo PieceTook = new EffectInfo(
            "PieceTookEffect", "Piece",
            new List<EffectArgument>
            {
                new EffectArgument("StartAngle", typeof(double), "0"),
                new EffectArgument("TargetXY", typeof(Vector), "0,0"),
                new EffectArgument("Color", typeof(Color), "#ffffffff"),
            });
        #endregion

        #region 変化表示用のエフェクトです。
        /// <summary>
        /// 自動再生開始時のカットインエフェクトです。(その１)
        /// </summary>
        public readonly static EffectInfo VariationCutIn1 = new EffectInfo(
            "CutInEffect1", "Variation");

        /// <summary>
        /// 自動再生開始時のカットインエフェクトです。(その２)
        /// </summary>
        public readonly static EffectInfo VariationCutIn2 = new EffectInfo(
            "CutInEffect2", "Variation");

        /// <summary>
        /// 変化の最初のエフェクトです。
        /// </summary>
        public readonly static EffectInfo VariationFirst = new EffectInfo(
            "FirstEffect", "Variation",
            new List<EffectArgument>
            {
                new EffectArgument("Rate", typeof(double), "0"),
            });

        /// <summary>
        /// 変化の２番目のエフェクトです。
        /// </summary>
        public readonly static EffectInfo VariationSecond = new EffectInfo(
            "SecondEffect", "Variation",
            new List<EffectArgument>
            {
                new EffectArgument("Rate", typeof(double), "0"),
            });

        /// <summary>
        /// 変化のエフェクトです。
        /// </summary>
        public readonly static EffectInfo VariationLast = new EffectInfo(
            "LastEffect", "Variation");
        #endregion

        #region その他
        /// <summary>
        /// 囲い完成時のエフェクトです。
        /// </summary>
        public readonly static EffectInfo Castle = new EffectInfo(
            "CastleEffect", "Other",
            new List<EffectArgument>
            {
                new EffectArgument("CastleName", typeof(string), ""),
                new EffectArgument("CastleNameLen", typeof(int), "1"),
                new EffectArgument("StartAngle", typeof(double), "0"),
                new EffectArgument("StartXY", typeof(Vector), "0,0"),
            });

        /// <summary>
        /// 勝利時のエフェクトです。
        /// </summary>
        public readonly static EffectInfo Win = new EffectInfo(
            "WinEffect", "Other");

        /// <summary>
        /// 投票時のエフェクトです。
        /// </summary>
        public readonly static EffectInfo Vote = new EffectInfo(
            "VoteEffect", "Other");
        #endregion
    }*/
}
