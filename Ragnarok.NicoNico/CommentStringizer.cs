using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;

namespace Ragnarok.NicoNico
{
    /// <summary>
    /// コメントの関連属性を文字列に直します。
    /// </summary>
    public static class CommentStringizer
    {
        /// <summary>
        /// コメント色を文字列に直します。
        /// </summary>
        public static string GetColorString(CommentColor color)
        {
            switch (color)
            {
                case CommentColor.Default:
                    return "";
                case CommentColor.White:
                    return "white";
                case CommentColor.Red:
                    return "red";
                case CommentColor.Pink:
                    return "pink";
                case CommentColor.Orange:
                    return "orange";
                case CommentColor.Yellow:
                    return "yellow";
                case CommentColor.Green:
                    return "green";
                case CommentColor.Cyan:
                    return "cyan";
                case CommentColor.Blue:
                    return "blue";
                case CommentColor.Purple:
                    return "purple";
                case CommentColor.Black:
                    return "black";

                // プレミアム会員専用色
                case CommentColor.White2:
                    return "white2";
                case CommentColor.Red2:
                    return "red2";
                case CommentColor.Orange2:
                    return "orange2";
                case CommentColor.Yellow2:
                    return "yellow2";
                case CommentColor.Green2:
                    return "green2";
                case CommentColor.Blue2:
                    return "blue2";
                case CommentColor.Purple2:
                    return "purple2";

                // プレミアム会員専用色（ニコ動のみ）
                case CommentColor.Cyan2:
                    return "cyan2";
                case CommentColor.Pink2:
                    return "pink2";
                case CommentColor.Black2:
                    return "black2";

                // プレミアム会員は任意の色が指定できるようになったため
                case CommentColor.Custom:
                    return "";
            }

            return ""; // 色指定無しです。
        }

        /// <summary>
        /// chukeiコマンド用のコメント色を取得します。
        /// </summary>
        public static string GetColorStringForChukei(CommentColor color)
        {
            switch (color)
            {
                case CommentColor.Default:
                case CommentColor.White:
                case CommentColor.White2:
                    return "fff";
                case CommentColor.Red:
                case CommentColor.Red2:
                    return "f00";
                case CommentColor.Pink:
                case CommentColor.Pink2:
                    return "e8a";
                case CommentColor.Orange:
                case CommentColor.Orange2:
                    return "fa0";
                case CommentColor.Yellow:
                case CommentColor.Yellow2:
                    return "ff0";
                case CommentColor.Green:
                case CommentColor.Green2:
                    return "0f0";
                case CommentColor.Cyan:
                case CommentColor.Cyan2:
                    return "0ff";
                case CommentColor.Blue:
                case CommentColor.Blue2:
                    return "00f";
                case CommentColor.Purple:
                case CommentColor.Purple2:
                    return "f0f";
                case CommentColor.Black:
                case CommentColor.Black2:
                    return "000";
            }

            return "fff"; // 色指定無しです。
        }
    }
}
