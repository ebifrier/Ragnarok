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
        public string Title
        {
            get { return null; }
        }

        /// <summary>
        /// アセンブリバージョンを取得します。
        /// </summary>
        public string Version
        {
            get { return null; }
        }

        /// <summary>
        /// 概要を取得します。
        /// </summary>
        public string Description
        {
            get { return null; }
        }

        /// <summary>
        /// プロダクト情報を取得します。
        /// </summary>
        public string Product
        {
            get { return null; }
        }

        /// <summary>
        /// 会社情報を取得します。
        /// </summary>
        public string Company
        {
            get { return null; }
        }

        /// <summary>
        /// 権利情報を取得します。
        /// </summary>
        public string Copyright
        {
            get { return null; }
        }
    }
}
