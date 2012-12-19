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
    /// アセンブリ情報の取得失敗時に使います。
    /// </summary>
    public class AssemblyNullAccessor : IAssemblyAccessor
    {
        /// <summary>
        /// アセンブリ名を取得します。
        /// </summary>
        public string AssemblyTitle
        {
            get { return null; }
        }

        /// <summary>
        /// アセンブリバージョンを取得します。
        /// </summary>
        public string AssemblyVersion
        {
            get { return null; }
        }

        /// <summary>
        /// 概要を取得します。
        /// </summary>
        public string AssemblyDescription
        {
            get { return null; }
        }

        /// <summary>
        /// プロダクト情報を取得します。
        /// </summary>
        public string AssemblyProduct
        {
            get { return null; }
        }

        /// <summary>
        /// 会社情報を取得します。
        /// </summary>
        public string AssemblyCompany
        {
            get { return null; }
        }

        /// <summary>
        /// 権利情報を取得します。
        /// </summary>
        public string AssemblyCopyright
        {
            get { return null; }
        }
    }
}
