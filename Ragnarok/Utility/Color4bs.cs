using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ragnarok.Utility
{
    /// <summary>
    /// Color4bのための色一覧を定義します。
    /// </summary>
    public static class Color4bs
    {
        /// <summary>
        /// string型を大文字小文字の区別をせずに比較します。
        /// </summary>
        internal sealed class StringComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return x.Equals(y, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                return obj.ToLower().GetHashCode();
            }
        }
        /// <summary>
        /// このクラスに登録されている組み込み色を高速化のため
        /// 事前にリスト化しておきます。
        /// </summary>
        internal readonly static Dictionary<string, Color4b>
            RegisteredColorsDic;

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static Color4bs()
        {
            var flags = BindingFlags.GetProperty | BindingFlags.Public |
                        BindingFlags.Static;

            RegisteredColorsDic = typeof(Color4bs)
                .GetProperties(flags)
                .Where(_ => _ != null)
                .Where(_ => _.PropertyType == typeof(Color4b))
                .ToDictionary(
                    _ => _.Name,
                    _ => (Color4b)_.GetValue(null, null),
                    new StringComparer());
        }

        /// <summary>
        /// アリスブルー色を取得します。
        /// </summary>
        public static Color4b AliceBlue
        {
            get { return Color4b.FromValue(0xFFF0F8FF); }
        }

        /// <summary>
        /// アンティークブルーを取得します。
        /// </summary>
        public static Color4b AntiqueWhite
        {
            get { return Color4b.FromValue(0xFFFAEBD7); } 
        }

        /// <summary>
        /// アクア色を取得します。
        /// </summary>
        public static Color4b Aqua
        {
            get { return Color4b.FromValue(0xFF00FFFF); }
        }

        /// <summary>
        /// アクアマリン色を取得します。
        /// </summary>
        public static Color4b Aquamarine
        {
            get { return Color4b.FromValue(0xFF7FFFD4); }
        }

        /// <summary>
        /// アズレ色を取得します。
        /// </summary>
        public static Color4b Azure
        {
            get { return Color4b.FromValue(0xFFF0FFFF); }
        }

        /// <summary>
        /// ベージュ色を取得します。
        /// </summary>
        public static Color4b Beige
        {
            get { return Color4b.FromValue(0xFFF5F5DC); }
        }

        /// <summary>
        /// ビスチェ色を取得します。
        /// </summary>
        public static Color4b Bisque
        {
            get { return Color4b.FromValue(0xFFFFE4C4); }
        }

        /// <summary>
        /// 黒を取得します。
        /// </summary>
        public static Color4b Black
        {
            get { return Color4b.FromValue(0xFF000000); }
        }

        /// <summary>
        /// ブランシュトアーモンド色を取得します。
        /// </summary>
        public static Color4b BlanchedAlmond
        {
            get { return Color4b.FromValue(0xFFFFEBCD); }
        }

        /// <summary>
        /// 青を取得します。
        /// </summary>
        public static Color4b Blue
        {
            get { return Color4b.FromValue(0xFF0000FF); }
        }

        /// <summary>
        /// ブルーバイオレット色を取得します。
        /// </summary>
        public static Color4b BlueViolet
        {
            get { return Color4b.FromValue(0xFF8A2BE2); }
        }

        /// <summary>
        /// ブラウン色を取得します。
        /// </summary>
        public static Color4b Brown
        {
            get { return Color4b.FromValue(0xFFA52A2A); }
        }

        /// <summary>
        /// ブリーウッド色を取得します。
        /// </summary>
        public static Color4b BurlyWood
        {
            get { return Color4b.FromValue(0xFFDEB887); }
        }

        /// <summary>
        /// カデットブルー色を取得します。
        /// </summary>
        public static Color4b CadetBlue
        {
            get { return Color4b.FromValue(0xFF5F9EA0); }
        }

        /// <summary>
        /// チャトレージュ色を取得します。
        /// </summary>
        public static Color4b Chartreuse
        {
            get { return Color4b.FromValue(0xFF7FFF00); }
        }

        /// <summary>
        /// チョコレート色を取得します。
        /// </summary>
        public static Color4b Chocolate
        {
            get { return Color4b.FromValue(0xFFD2691E); }
        }

        /// <summary>
        /// コーラル色を取得します。
        /// </summary>
        public static Color4b Coral
        {
            get { return Color4b.FromValue(0xFFFF7F50); }
        }

        /// <summary>
        /// コーンフラワーブルー色を取得します。
        /// </summary>
        public static Color4b CornflowerBlue
        {
            get { return Color4b.FromValue(0xFF6495ED); }
        }

        /// <summary>
        /// コーンシルク色を取得します。
        /// </summary>
        public static Color4b Cornsilk
        {
            get { return Color4b.FromValue(0xFFFFF8DC); }
        }

        /// <summary>
        /// クリムゾン色を取得します。
        /// </summary>
        public static Color4b Crimson
        {
            get { return Color4b.FromValue(0xFFDC143C); }
        }

        /// <summary>
        /// シアン色を取得します。
        /// </summary>
        public static Color4b Cyan
        {
            get { return Color4b.FromValue(0xFF00FFFF); }
        }

        /// <summary>
        /// ダークブルー色を取得します。
        /// </summary>
        public static Color4b DarkBlue
        {
            get { return Color4b.FromValue(0xFF00008B); }
        }

        /// <summary>
        /// ダークシアン色を取得します。
        /// </summary>
        public static Color4b DarkCyan
        {
            get { return Color4b.FromValue(0xFF008B8B); }
        }

        /// <summary>
        /// ダークゴールデンロッド色を取得します。
        /// </summary>
        public static Color4b DarkGoldenrod
        {
            get { return Color4b.FromValue(0xFFB8860B); }
        }

        /// <summary>
        /// ダークグレイ色を取得します。
        /// </summary>
        public static Color4b DarkGray
        {
            get { return Color4b.FromValue(0xFFA9A9A9); }
        }

        /// <summary>
        /// ダークグリーン色を取得します。
        /// </summary>
        public static Color4b DarkGreen
        {
            get { return Color4b.FromValue(0xFF006400); }
        }

        /// <summary>
        /// ダークカーキ色を取得します。
        /// </summary>
        public static Color4b DarkKhaki
        {
            get { return Color4b.FromValue(0xFFBDB76B); }
        }

        /// <summary>
        /// ダークマゼンダ色を取得します。
        /// </summary>
        public static Color4b DarkMagenta
        {
            get { return Color4b.FromValue(0xFF8B008B); }
        }

        /// <summary>
        /// 濃いオリーブグリーン色を取得します。
        /// </summary>
        public static Color4b DarkOliveGreen
        {
            get { return Color4b.FromValue(0xFF556B2F); }
        }

        /// <summary>
        /// 濃いオレンジ色を取得します。
        /// </summary>
        public static Color4b DarkOrange
        {
            get { return Color4b.FromValue(0xFFFF8C00); }
        }

        /// <summary>
        /// 濃いオーキッド色を取得します。
        /// </summary>
        public static Color4b DarkOrchid
        {
            get { return Color4b.FromValue(0xFF9932CC); }
        }

        /// <summary>
        /// 濃い赤を取得します。
        /// </summary>
        public static Color4b DarkRed
        {
            get { return Color4b.FromValue(0xFF8B0000); }
        }

        /// <summary>
        /// 濃いサーモン色を取得します。
        /// </summary>
        public static Color4b DarkSalmon
        {
            get { return Color4b.FromValue(0xFFE9967A); }
        }

        /// <summary>
        /// 濃いシーグリーン色を取得します。
        /// </summary>
        public static Color4b DarkSeaGreen
        {
            get { return Color4b.FromValue(0xFF8FBC8F); }
        }

        /// <summary>
        /// 濃いスレートブルー色を取得します。
        /// </summary>
        public static Color4b DarkSlateBlue
        {
            get { return Color4b.FromValue(0xFF483D8B); }
        }

        /// <summary>
        /// 濃いスレートグレーを取得します。
        /// </summary>
        public static Color4b DarkSlateGray
        {
            get { return Color4b.FromValue(0xFF2F4F4F); }
        }

        /// <summary>
        /// 濃いターコイズ色を取得します。
        /// </summary>
        public static Color4b DarkTurquoise
        {
            get { return Color4b.FromValue(0xFF00CED1); }
        }

        /// <summary>
        /// 濃いバイオレット色を取得します。
        /// </summary>
        public static Color4b DarkViolet
        {
            get { return Color4b.FromValue(0xFF9400D3); }
        }

        /// <summary>
        /// 深いピンク色を取得します。
        /// </summary>
        public static Color4b DeepPink
        {
            get { return Color4b.FromValue(0xFFFF1493); }
        }

        /// <summary>
        /// 深いスカイブルー色を取得します。
        /// </summary>
        public static Color4b DeepSkyBlue
        {
            get { return Color4b.FromValue(0xFF00BFFF); }
        }

        /// <summary>
        /// 中間の濃さの灰色を取得します。
        /// </summary>
        public static Color4b DimGray
        {
            get { return Color4b.FromValue(0xFF696969); }
        }

        /// <summary>
        /// ドッジャーブルー色を取得します。
        /// </summary>
        public static Color4b DodgerBlue
        {
            get { return Color4b.FromValue(0xFF1E90FF); }
        }

        /// <summary>
        /// ファイアーブリック色を取得します。
        /// </summary>
        public static Color4b Firebrick
        {
            get { return Color4b.FromValue(0xFFB22222); }
        }

        /// <summary>
        /// フローラルな白色を取得します。
        /// </summary>
        public static Color4b FloralWhite
        {
            get { return Color4b.FromValue(0xFFFFFAF0); }
        }

        /// <summary>
        /// フォレストグリーン色を取得します。
        /// </summary>
        public static Color4b ForestGreen
        {
            get { return Color4b.FromValue(0xFF228B22); }
        }

        /// <summary>
        /// フッチャ色を取得します。
        /// </summary>
        public static Color4b Fuchsia
        {
            get { return Color4b.FromValue(0xFFFF00FF); }
        }

        /// <summary>
        /// ゲインズボロ色を取得します。
        /// </summary>
        public static Color4b Gainsboro
        {
            get { return Color4b.FromValue(0xFFDCDCDC); }
        }

        /// <summary>
        /// ゴーストホワイト色を取得します。
        /// </summary>
        public static Color4b GhostWhite
        {
            get { return Color4b.FromValue(0xFFF8F8FF); }
        }

        /// <summary>
        /// 金色を取得します。
        /// </summary>
        public static Color4b Gold
        {
            get { return Color4b.FromValue(0xFFFFD700); }
        }

        /// <summary>
        /// ゴールデンロッド色を取得します。
        /// </summary>
        public static Color4b Goldenrod
        {
            get { return Color4b.FromValue(0xFFDAA520); }
        }

        /// <summary>
        /// 灰色を取得します。
        /// </summary>
        public static Color4b Gray
        {
            get { return Color4b.FromValue(0xFF808080); }
        }

        /// <summary>
        /// 緑色を取得します。
        /// </summary>
        public static Color4b Green
        {
            get { return Color4b.FromValue(0xFF008000); }
        }

        /// <summary>
        /// 黄緑色を取得します。
        /// </summary>
        public static Color4b GreenYellow
        {
            get { return Color4b.FromValue(0xFFADFF2F); }
        }

        /// <summary>
        /// ハニーデュー色を取得します。
        /// </summary>
        public static Color4b Honeydew
        {
            get { return Color4b.FromValue(0xFFF0FFF0); }
        }

        /// <summary>
        /// ホットピンク色を取得します。
        /// </summary>
        public static Color4b HotPink
        {
            get { return Color4b.FromValue(0xFFFF69B4); }
        }

        /// <summary>
        /// インディアンレッドを取得します。
        /// </summary>
        public static Color4b IndianRed
        {
            get { return Color4b.FromValue(0xFFCD5C5C); }
        }

        /// <summary>
        /// インディゴ色を取得します。
        /// </summary>
        public static Color4b Indigo
        {
            get { return Color4b.FromValue(0xFF4B0082); }
        }

        /// <summary>
        /// アイボリー色を取得します。
        /// </summary>
        public static Color4b Ivory
        {
            get { return Color4b.FromValue(0xFFFFFFF0); }
        }

        /// <summary>
        /// カーキ色を取得します。
        /// </summary>
        public static Color4b Khaki
        {
            get { return Color4b.FromValue(0xFFF0E68C); }
        }

        /// <summary>
        /// ラベンダー色を取得します。
        /// </summary>
        public static Color4b Lavender
        {
            get { return Color4b.FromValue(0xFFE6E6FA); }
        }

        /// <summary>
        /// ラベンダーブラッシュ色を取得します。
        /// </summary>
        public static Color4b LavenderBlush
        {
            get { return Color4b.FromValue(0xFFFFF0F5); }
        }

        /// <summary>
        /// ローングリーン色を取得します。
        /// </summary>
        public static Color4b LawnGreen
        {
            get { return Color4b.FromValue(0xFF7CFC00); }
        }

        /// <summary>
        /// レモンシフォン色を取得します。
        /// </summary>
        public static Color4b LemonChiffon
        {
            get { return Color4b.FromValue(0xFFFFFACD); }
        }

        /// <summary>
        /// 薄い青色を取得します。
        /// </summary>
        public static Color4b LightBlue
        {
            get { return Color4b.FromValue(0xFFADD8E6); }
        }

        /// <summary>
        /// 薄いコーラル色を取得します。
        /// </summary>
        public static Color4b LightCoral
        {
            get { return Color4b.FromValue(0xFFF08080); }
        }

        /// <summary>
        /// 薄いシアン色を取得します。
        /// </summary>
        public static Color4b LightCyan
        {
            get { return Color4b.FromValue(0xFFE0FFFF); }
        }

        /// <summary>
        /// 薄いゴールデンロッド色を取得します。
        /// </summary>
        public static Color4b LightGoldenrodYellow
        {
            get { return Color4b.FromValue(0xFFFAFAD2); }
        }

        /// <summary>
        /// 薄い灰色を取得します。
        /// </summary>
        public static Color4b LightGray
        {
            get { return Color4b.FromValue(0xFFD3D3D3); }
        }

        /// <summary>
        /// 薄い緑色を取得します。
        /// </summary>
        public static Color4b LightGreen
        {
            get { return Color4b.FromValue(0xFF90EE90); }
        }

        /// <summary>
        /// 薄いピンク色を取得します。
        /// </summary>
        public static Color4b LightPink
        {
            get { return Color4b.FromValue(0xFFFFB6C1); }
        }

        /// <summary>
        /// 薄いサーモン色を取得します。
        /// </summary>
        public static Color4b LightSalmon
        {
            get { return Color4b.FromValue(0xFFFFA07A); }
        }

        /// <summary>
        /// 薄いシーグリーン色を取得します。
        /// </summary>
        public static Color4b LightSeaGreen
        {
            get { return Color4b.FromValue(0xFF20B2AA); }
        }

        /// <summary>
        /// 薄いスカイブルー色を取得します。
        /// </summary>
        public static Color4b LightSkyBlue
        {
            get { return Color4b.FromValue(0xFF87CEFA); }
        }

        /// <summary>
        /// 薄いスレートグレー色を取得します。
        /// </summary>
        public static Color4b LightSlateGray
        {
            get { return Color4b.FromValue(0xFF778899); }
        }

        /// <summary>
        /// 薄いスティールブルー色を取得します。
        /// </summary>
        public static Color4b LightSteelBlue
        {
            get { return Color4b.FromValue(0xFFB0C4DE); }
        }

        /// <summary>
        /// 薄い黄色を取得します。
        /// </summary>
        public static Color4b LightYellow
        {
            get { return Color4b.FromValue(0xFFFFFFE0); }
        }

        /// <summary>
        /// ライム色を取得します。
        /// </summary>
        public static Color4b Lime
        {
            get { return Color4b.FromValue(0xFF00FF00); }
        }

        /// <summary>
        /// ライムグリーン色を取得します。
        /// </summary>
        public static Color4b LimeGreen
        {
            get { return Color4b.FromValue(0xFF32CD32); }
        }

        /// <summary>
        /// リネン色を取得します。
        /// </summary>
        public static Color4b Linen
        {
            get { return Color4b.FromValue(0xFFFAF0E6); }
        }

        /// <summary>
        /// マゼンダ色を取得します。
        /// </summary>
        public static Color4b Magenta
        {
            get { return Color4b.FromValue(0xFFFF00FF); }
        }

        /// <summary>
        /// マローン色を取得します。
        /// </summary>
        public static Color4b Maroon
        {
            get { return Color4b.FromValue(0xFF800000); }
        }

        /// <summary>
        /// 中間の濃さのアクアマリン色を取得します。
        /// </summary>
        public static Color4b MediumAquamarine
        {
            get { return Color4b.FromValue(0xFF66CDAA); }
        }

        /// <summary>
        /// 中間の濃さの青色を取得します。
        /// </summary>
        public static Color4b MediumBlue
        {
            get { return Color4b.FromValue(0xFF0000CD); }
        }

        /// <summary>
        /// 中間の濃さのオーキッド色を取得します。
        /// </summary>
        public static Color4b MediumOrchid
        {
            get { return Color4b.FromValue(0xFFBA55D3); }
        }

        /// <summary>
        /// 中間の濃さの紫色を取得します。
        /// </summary>
        public static Color4b MediumPurple
        {
            get { return Color4b.FromValue(0xFF9370DB); }
        }

        /// <summary>
        /// 中間の濃さのシーグリーン色を取得します。
        /// </summary>
        public static Color4b MediumSeaGreen
        {
            get { return Color4b.FromValue(0xFF3CB371); }
        }

        /// <summary>
        /// 中間の濃さのスレートブルーを取得します。
        /// </summary>
        public static Color4b MediumSlateBlue
        {
            get { return Color4b.FromValue(0xFF7B68EE); }
        }

        /// <summary>
        /// 中間の濃さのスプリンググリーン色を取得します。
        /// </summary>
        public static Color4b MediumSpringGreen
        {
            get { return Color4b.FromValue(0xFF00FA9A); }
        }

        /// <summary>
        /// 中間の濃さのターコイズ色を取得します。
        /// </summary>
        public static Color4b MediumTurquoise
        {
            get { return Color4b.FromValue(0xFF48D1CC); }
        }

        /// <summary>
        /// 中間の濃さのバイオレットレッド色を取得します。
        /// </summary>
        public static Color4b MediumVioletRed
        {
            get { return Color4b.FromValue(0xFFC71585); }
        }

        /// <summary>
        /// ミッドナイトブルー色を取得します。
        /// </summary>
        public static Color4b MidnightBlue
        {
            get { return Color4b.FromValue(0xFF191970); }
        }

        /// <summary>
        /// ミントクリーム色を取得します。
        /// </summary>
        public static Color4b MintCream
        {
            get { return Color4b.FromValue(0xFFF5FFFA); }
        }

        /// <summary>
        /// ミスティーローズ色を取得します。
        /// </summary>
        public static Color4b MistyRose
        {
            get { return Color4b.FromValue(0xFFFFE4E1); }
        }

        /// <summary>
        /// モカシン色を取得します。
        /// </summary>
        public static Color4b Moccasin
        {
            get { return Color4b.FromValue(0xFFFFE4B5); }
        }

        /// <summary>
        /// ナヴァージョホワイト色を取得します。
        /// </summary>
        public static Color4b NavajoWhite
        {
            get { return Color4b.FromValue(0xFFFFDEAD); }
        }

        /// <summary>
        /// ネイビー色を取得します。
        /// </summary>
        public static Color4b Navy
        {
            get { return Color4b.FromValue(0xFF000080); }
        }

        /// <summary>
        /// オールドレース色を取得します。
        /// </summary>
        public static Color4b OldLace
        {
            get { return Color4b.FromValue(0xFFFDF5E6); }
        }

        /// <summary>
        /// オリーブ色を取得します。
        /// </summary>
        public static Color4b Olive
        {
            get { return Color4b.FromValue(0xFF808000); }
        }

        /// <summary>
        /// オリーブドラブ色を取得します。
        /// </summary>
        public static Color4b OliveDrab
        {
            get { return Color4b.FromValue(0xFF6B8E23); }
        }

        /// <summary>
        /// オレンジ色を取得します。
        /// </summary>
        public static Color4b Orange
        {
            get { return Color4b.FromValue(0xFFFFA500); }
        }

        /// <summary>
        /// オレンジレッド色を取得します。
        /// </summary>
        public static Color4b OrangeRed
        {
            get { return Color4b.FromValue(0xFFFF4500); }
        }

        /// <summary>
        /// オーキッド色を取得します。
        /// </summary>
        public static Color4b Orchid
        {
            get { return Color4b.FromValue(0xFFDA70D6); }
        }

        /// <summary>
        /// パーレゴールデンレッド色を取得します。
        /// </summary>
        public static Color4b PaleGoldenrod
        {
            get { return Color4b.FromValue(0xFFEEE8AA); }
        }

        /// <summary>
        /// パーレグリーン色を取得します。
        /// </summary>
        public static Color4b PaleGreen
        {
            get { return Color4b.FromValue(0xFF98FB98); }
        }

        /// <summary>
        /// ペイルターコイズ色を取得します。
        /// </summary>
        public static Color4b PaleTurquoise
        {
            get { return Color4b.FromValue(0xFFAFEEEE); }
        }

        /// <summary>
        /// ペイルヴァイオレッドレッド色を取得します。
        /// </summary>
        public static Color4b PaleVioletRed
        {
            get { return Color4b.FromValue(0xFFDB7093); }
        }

        /// <summary>
        /// パパヤウィップ色を取得します。
        /// </summary>
        public static Color4b PapayaWhip
        {
            get { return Color4b.FromValue(0xFFFFEFD5); }
        }

        /// <summary>
        /// ピーチパフ色を取得します。
        /// </summary>
        public static Color4b PeachPuff
        {
            get { return Color4b.FromValue(0xFFFFDAB9); }
        }

        /// <summary>
        /// ペルー色を取得します。
        /// </summary>
        public static Color4b Peru
        {
            get { return Color4b.FromValue(0xFFCD853F); }
        }

        /// <summary>
        /// ピンク色を取得します。
        /// </summary>
        public static Color4b Pink
        {
            get { return Color4b.FromValue(0xFFFFC0CB); }
        }

        /// <summary>
        /// プラム色を取得します。
        /// </summary>
        public static Color4b Plum
        {
            get { return Color4b.FromValue(0xFFDDA0DD); }
        }

        /// <summary>
        /// パウダーブルー色を取得します。
        /// </summary>
        public static Color4b PowderBlue
        {
            get { return Color4b.FromValue(0xFFB0E0E6); }
        }

        /// <summary>
        /// 紫色を取得します。
        /// </summary>
        public static Color4b Purple
        {
            get { return Color4b.FromValue(0xFF800080); }
        }

        /// <summary>
        /// 赤色を取得します。
        /// </summary>
        public static Color4b Red
        {
            get { return Color4b.FromValue(0xFFFF0000); }
        }

        /// <summary>
        /// ロージーブラウン色を取得します。
        /// </summary>
        public static Color4b RosyBrown
        {
            get { return Color4b.FromValue(0xFFBC8F8F); }
        }

        /// <summary>
        /// ロイヤルブルー色を取得します。
        /// </summary>
        public static Color4b RoyalBlue
        {
            get { return Color4b.FromValue(0xFF4169E1); }
        }

        /// <summary>
        /// サドルブラウン色を取得します。
        /// </summary>
        public static Color4b SaddleBrown
        {
            get { return Color4b.FromValue(0xFF8B4513); }
        }

        /// <summary>
        /// サーモン色を取得します。
        /// </summary>
        public static Color4b Salmon
        {
            get { return Color4b.FromValue(0xFFFA8072); }
        }

        /// <summary>
        /// サンディーブラウン色を取得します。
        /// </summary>
        public static Color4b SandyBrown
        {
            get { return Color4b.FromValue(0xFFF4A460); }
        }

        /// <summary>
        /// シーグリーン色を取得します。
        /// </summary>
        public static Color4b SeaGreen
        {
            get { return Color4b.FromValue(0xFF2E8B57); }
        }

        /// <summary>
        /// シーシェル色を取得します。
        /// </summary>
        public static Color4b SeaShell
        {
            get { return Color4b.FromValue(0xFFFFF5EE); }
        }

        /// <summary>
        /// シエナ色を取得します。
        /// </summary>
        public static Color4b Sienna
        {
            get { return Color4b.FromValue(0xFFA0522D); }
        }

        /// <summary>
        /// 銀色を取得します。
        /// </summary>
        public static Color4b Silver
        {
            get { return Color4b.FromValue(0xFFC0C0C0); }
        }

        /// <summary>
        /// スカイブルー色を取得します。
        /// </summary>
        public static Color4b SkyBlue
        {
            get { return Color4b.FromValue(0xFF87CEEB); }
        }

        /// <summary>
        /// スレートブルー色を取得します。
        /// </summary>
        public static Color4b SlateBlue
        {
            get { return Color4b.FromValue(0xFF6A5ACD); }
        }

        /// <summary>
        /// スレートグレー色を取得します。
        /// </summary>
        public static Color4b SlateGray
        {
            get { return Color4b.FromValue(0xFF708090); }
        }

        /// <summary>
        /// 雪色を取得します。
        /// </summary>
        public static Color4b Snow
        {
            get { return Color4b.FromValue(0xFFFFFAFA); }
        }

        /// <summary>
        /// スプリンググリーン色を取得します。
        /// </summary>
        public static Color4b SpringGreen
        {
            get { return Color4b.FromValue(0xFF00FF7F); }
        }

        /// <summary>
        /// スティールブルー色を取得します。
        /// </summary>
        public static Color4b SteelBlue
        {
            get { return Color4b.FromValue(0xFF4682B4); }
        }

        /// <summary>
        /// タン色を取得します。
        /// </summary>
        public static Color4b Tan
        {
            get { return Color4b.FromValue(0xFFD2B48C); }
        }

        /// <summary>
        /// ティール色を取得します。
        /// </summary>
        public static Color4b Teal
        {
            get { return Color4b.FromValue(0xFF008080); }
        }

        /// <summary>
        /// シストレ色を取得します。
        /// </summary>
        public static Color4b Thistle
        {
            get { return Color4b.FromValue(0xFFD8BFD8); }
        }

        /// <summary>
        /// トマト色を取得します。
        /// </summary>
        public static Color4b Tomato
        {
            get { return Color4b.FromValue(0xFFFF6347); }
        }

        /// <summary>
        /// 透明色を取得します。
        /// </summary>
        public static Color4b Transparent
        {
            get { return Color4b.FromValue(0x00FFFFFF); }
        }

        /// <summary>
        /// ターコイズ色を取得します。
        /// </summary>
        public static Color4b Turquoise
        {
            get { return Color4b.FromValue(0xFF40E0D0); }
        }

        /// <summary>
        /// ヴァイオレット色を取得します。
        /// </summary>
        public static Color4b Violet
        {
            get { return Color4b.FromValue(0xFFEE82EE); }
        }

        /// <summary>
        /// ウィート色を取得します。
        /// </summary>
        public static Color4b Wheat
        {
            get { return Color4b.FromValue(0xFFF5DEB3); }
        }

        /// <summary>
        /// 白色を取得します。
        /// </summary>
        public static Color4b White
        {
            get { return Color4b.FromValue(0xFFFFFFFF); }
        }

        /// <summary>
        /// ホワイトスモーク色を取得します。
        /// </summary>
        public static Color4b WhiteSmoke
        {
            get { return Color4b.FromValue(0xFFF5F5F5); }
        }

        /// <summary>
        /// 黄色を取得します。
        /// </summary>
        public static Color4b Yellow
        {
            get { return Color4b.FromValue(0xFFFFFF00); }
        }

        /// <summary>
        /// 黄緑色を取得します。
        /// </summary>
        public static Color4b YellowGreen
        {
            get { return Color4b.FromValue(0xFF9ACD32); }
        }
    }
}
