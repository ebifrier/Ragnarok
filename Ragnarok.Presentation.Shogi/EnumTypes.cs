using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ragnarok.Utility;

namespace Ragnarok.Presentation.Shogi
{
    /// <summary>
    /// 将棋ウィンドウの編集モードを識別します。
    /// </summary>
    public enum EditMode
    {
        /// <summary>
        /// 手番と一致する側の駒のみを可能な位置にのみ動かせます。
        /// </summary>
        Normal,
        /// <summary>
        /// どの駒も自由に動かせます。
        /// </summary>
        Editing,
        /// <summary>
        /// 駒を動かすことが出来ません。
        /// </summary>
        NoEdit,
    }

    /// <summary>
    /// 変化の再生状態を示します。
    /// </summary>
    public enum AutoPlayState
    {
        /// <summary>
        /// 何もしていません。
        /// </summary>
        None,
        /// <summary>
        /// 変化再生中です。
        /// </summary>
        Playing,
    }

    /// <summary>
    /// 駒画像の種類です。
    /// </summary>
    public enum KomaImageType
    {
        /// <summary>
        /// ２文字駒バージョン１
        /// </summary>
        [LabelDescription(Label = "２文字駒 その１", Description = "koma/koma_kinki.png")]
        Kinki,
        /// <summary>
        /// ２文字駒バージョン２
        /// </summary>
        [LabelDescription(Label = "２文字駒 その２", Description = "koma/koma_ryoko.png")]
        Ryoko,
        /// <summary>
        /// １文字駒
        /// </summary>
        [LabelDescription(Label = "１文字駒", Description = "koma/koma_1moji.png")]
        OneMoji,
    }

    /// <summary>
    /// 盤画像の種類です。
    /// </summary>
    public enum BanImageType
    {
        /// <summary>
        /// デフォルト
        /// </summary>
        [LabelDescription(Label = "デフォルト", Description = "ban/ban.jpg")]
        Default,
        /// <summary>
        /// カヤ柾目 その１
        /// </summary>
        [LabelDescription(Label = "カヤ柾目 その１", Description = "ban/ban_kaya1.jpg")]
        Kaya1,
        /// <summary>
        /// カヤ柾目 その２
        /// </summary>
        [LabelDescription(Label = "カヤ柾目 その２", Description = "ban/ban_kaya2.jpg")]
        Kaya2,
        /// <summary>
        /// 使い古し
        /// </summary>
        [LabelDescription(Label = "使い古し", Description = "ban/ban_dirty.jpg")]
        Dirty,
        /// <summary>
        /// 紙
        /// </summary>
        [LabelDescription(Label = "紙", Description = "ban/ban_paper.jpg")]
        Paper,
        /// <summary>
        /// 透明
        /// </summary>
        [LabelDescription(Label = "透明", Description = "ban/ban_trans.png")]
        Transparent,
    }

    /// <summary>
    /// 駒台画像の種類です。
    /// </summary>
    public enum KomadaiImageType
    {
        /// <summary>
        /// 駒台１
        /// </summary>
        [LabelDescription(Label = "駒台１", Description = "komadai/komadai1.jpg")]
        Komadai1,
        /// <summary>
        /// 駒台２
        /// </summary>
        [LabelDescription(Label = "駒台２", Description = "komadai/komadai2.jpg")]
        Komadai2,
        /// <summary>
        /// 駒台３
        /// </summary>
        [LabelDescription(Label = "駒台３", Description = "komadai/komadai3.jpg")]
        Komadai3,
    }

    /// <summary>
    /// 画像用のユーティリティクラスです。
    /// </summary>
    public static class ImageUtil
    {
        /// <summary>
        /// 画像用の基本パスです。
        /// </summary>
        public static readonly Uri BaseUri =
            new Uri("pack://application:,,,/Ragnarok.Presentation.Shogi;component/Resources/Image/xxx");

        /// <summary>
        /// 選択された画像のURIを取得します。
        /// </summary>
        public static Uri GetImageUri<T>(T value)
            where T : struct
        {
            var leaf = EnumEx.GetDescription(value);
            if (string.IsNullOrEmpty(leaf))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "{0}: 画像のパスが設定されてません。",
                        value));
            }

            return new Uri(BaseUri, leaf);
        }
    }
}
