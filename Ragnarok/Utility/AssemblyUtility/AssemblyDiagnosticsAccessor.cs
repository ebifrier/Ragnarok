using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Ragnarok.Utility.AssemblyUtility
{
    /// <summary>
    /// アセンブリファイルからアセンブリ情報を取得します。
    /// </summary>
    public class AssemblyDiagnosticsAccessor : IAssemblyAccessor
    {
        /// <summary>
        /// アセンブリ名を取得します。
        /// </summary>
        public string Title
        {
            get;
            private set;
        }

        /// <summary>
        /// アセンブリバージョンを取得します。
        /// </summary>
        public string Version
        {
            get;
            private set;
        }

        /// <summary>
        /// 概要を取得します。
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// プロダクト情報を取得します。
        /// </summary>
        public string Product
        {
            get;
            private set;
        }

        /// <summary>
        /// 会社情報を取得します。
        /// </summary>
        public string Company
        {
            get;
            private set;
        }

        /// <summary>
        /// 権利情報を取得します。
        /// </summary>
        public string Copyright
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AssemblyDiagnosticsAccessor(string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName))
            {
                throw new ArgumentNullException("assemblyName");
            }

            // 例外が返る可能性があります。
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assemblyName);

            Title = fileVersionInfo.ProductName;
            Version = fileVersionInfo.FileVersion;
            Description = fileVersionInfo.FileDescription;
            Company = fileVersionInfo.CompanyName;
            Copyright = fileVersionInfo.LegalCopyright;
        }
    }
}
