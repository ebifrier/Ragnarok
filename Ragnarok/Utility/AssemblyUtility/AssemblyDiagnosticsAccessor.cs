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
        public string AssemblyTitle
        {
            get;
            private set;
        }

        /// <summary>
        /// アセンブリバージョンを取得します。
        /// </summary>
        public string AssemblyVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// 概要を取得します。
        /// </summary>
        public string AssemblyDescription
        {
            get;
            private set;
        }

        /// <summary>
        /// プロダクト情報を取得します。
        /// </summary>
        public string AssemblyProduct
        {
            get;
            private set;
        }

        /// <summary>
        /// 会社情報を取得します。
        /// </summary>
        public string AssemblyCompany
        {
            get;
            private set;
        }

        /// <summary>
        /// 権利情報を取得します。
        /// </summary>
        public string AssemblyCopyright
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

            AssemblyTitle = fileVersionInfo.ProductName;
            AssemblyVersion = fileVersionInfo.FileVersion;
            AssemblyDescription = fileVersionInfo.FileDescription;
            AssemblyCompany = fileVersionInfo.CompanyName;
            AssemblyCopyright = fileVersionInfo.LegalCopyright;
        }
    }
}
