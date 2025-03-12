using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Markup;

namespace Ragnarok.Xaml
{
    /// <summary>
    /// ファイルパスを扱います。
    /// </summary>
    public class FilePathInfo
    {
        /// <summary>
        /// ファイル名のみを取得または設定します。
        /// </summary>
        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// ファイルのフルパスを取得または設定します。
        /// </summary>
        public string FullPath
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 指定のディレクトリにあるファイル一覧を取得する拡張構文です。
    /// </summary>
    [MarkupExtensionReturnType(typeof(FilePathInfo[]))]
    public class FileListExtension : MarkupExtension
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FileListExtension()
            : this("*")
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FileListExtension(string pattern)
        {
            Pattern = pattern;
        }

        /// <summary>
        /// 基本となるディレクトリを取得または設定します。
        /// </summary>
        [DefaultValue((string)null)]
        public string BasePath
        {
            get;
            set;
        }

        /// <summary>
        /// 検索パターンを取得または設定します。
        /// </summary>
        [DefaultValue("*")]
        public string Pattern
        {
            get;
            set;
        }

        /// <summary>
        /// オプションを取得または設定します。
        /// </summary>
        [DefaultValue(SearchOption.TopDirectoryOnly)]
        public SearchOption Option
        {
            get;
            set;
        }

        /// <summary>
        /// ファイル一覧を取得します。
        /// </summary>
        public override object ProvideValue(IServiceProvider service)
        {
            var basePath = BasePath;

            if (string.IsNullOrEmpty(basePath))
            {
                basePath = AppContext.BaseDirectory;
            }

            return Directory
                .EnumerateFiles(basePath, Pattern, Option)
                .Select(_ =>
                    new FilePathInfo()
                    {
                        FullPath = _,
                        FileName = Path.GetFileName(_),
                    })
                .ToArray();
        }
    }
}
