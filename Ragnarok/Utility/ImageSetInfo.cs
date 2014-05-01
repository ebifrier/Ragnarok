using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.Utility
{
    /// <summary>
    /// 評価値ウィンドウ用の画像セット情報を保持します。
    /// </summary>
    [DataContract()]
    public class ImageSetInfo : InfoBase
    {
        /// <summary>
        /// 評価値ウィンドウに使う各画像の情報を保持します。
        /// </summary>
        [DataContract()]
        internal class InternalImage
        {
            /// <summary>
            /// 画像が適用される評価値の最小値を取得または設定します。
            /// </summary>
            [DataMember(Name = "min")]
            public double MinValue
            {
                get;
                set;
            }

            /// <summary>
            /// 画像が適用される評価値の最大値を取得または設定します。
            /// </summary>
            [DataMember(Name = "max")]
            public double MaxValue
            {
                get;
                set;
            }

            /// <summary>
            /// 画像のパスを取得または設定します。
            /// </summary>
            [DataMember(Name = "image")]
            public string ImagePath
            {
                get;
                set;
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public InternalImage()
            {
                // デフォルトは無限小＆無限大です。
                MinValue = int.MinValue;
                MaxValue = int.MaxValue;
            }
        }

        /// <summary>
        /// 画像リストを取得します。
        /// </summary>
        [DataMember(Name = "imageList")]
        private List<InternalImage> ImageList
        {
            get;
            set;
        }

        /// <summary>
        /// 評価値から該当する画像ファイルのパスを取得します。
        /// </summary>
        public string GetSelectedImagePath(double point)
        {
            var selectedImage = ImageList.FirstOrDefault(
                info => info.MinValue <= point && point <= info.MaxValue);

            return (selectedImage != null
                ? Path.Combine(BasePath, selectedImage.ImagePath)
                : null);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImageSetInfo()
        {
            ImageList = new List<InternalImage>();
        }
    }
}
