using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;

using Ragnarok.Utility;

namespace Ragnarok.Extra.Xaml
{
    /// <summary>
    /// ディレクトリ中の画像をリストアップするための拡張です。
    /// </summary>
    [MarkupExtensionReturnType(typeof(List<ImageSetInfo>))]
    public class ImageSetListExtension : MarkupExtension
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImageSetListExtension()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImageSetListExtension(string dirPath)
        {
            DirectoryPath = dirPath;
        }

        /// <summary>
        /// 基本となるディレクトリパスを取得または設定します。
        /// </summary>
        [DefaultValue("")]
        public string DirectoryPath
        {
            get;
            set;
        }

        /// <summary>
        /// を取得します。
        /// </summary>
        public override object ProvideValue(IServiceProvider service)
        {
            if (string.IsNullOrEmpty(DirectoryPath))
            {
                throw new InvalidOperationException(
                    "ディレクトリパスが設定されていません。");
            }

            return InfoBase.ReadInfoDirectory<ImageSetInfo>(DirectoryPath);
        }
    }
}
