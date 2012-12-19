using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Ragnarok.Utility.AssemblyUtility
{
    /// <summary>
    /// アセンブリ情報を保持します。
    /// </summary>
    public class AssemblyAccessor : IAssemblyAccessor
    {
        private IAssemblyAccessor internalAccessor;

        /// <summary>
        /// アセンブリ名を取得します。
        /// </summary>
        public string AssemblyTitle
        {
            get { return this.internalAccessor.AssemblyTitle; }
        }

        /// <summary>
        /// アセンブリバージョンを取得します。
        /// </summary>
        public string AssemblyVersion
        {
            get { return this.internalAccessor.AssemblyVersion; }
        }

        /// <summary>
        /// 概要を取得します。
        /// </summary>
        public string AssemblyDescription
        {
            get { return this.internalAccessor.AssemblyDescription; }
        }

        /// <summary>
        /// プロダクト情報を取得します。
        /// </summary>
        public string AssemblyProduct
        {
            get { return this.internalAccessor.AssemblyProduct; }
        }

        /// <summary>
        /// 会社情報を取得します。
        /// </summary>
        public string AssemblyCompany
        {
            get { return this.internalAccessor.AssemblyCompany; }
        }

        /// <summary>
        /// 権利情報を取得します。
        /// </summary>
        public string AssemblyCopyright
        {
            get { return this.internalAccessor.AssemblyCopyright; }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AssemblyAccessor(string assemblyName, bool bUseReflectionAccesor)
        {
            try
            {
                this.internalAccessor = (bUseReflectionAccesor ?
                    (IAssemblyAccessor)
                    new AssemblyReflectionAccessor(assemblyName) :
                    new AssemblyDiagnosticsAccessor(assemblyName));
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "アセンブリ情報の取得に失敗しました。");

                this.internalAccessor = (!bUseReflectionAccesor ?
                    (IAssemblyAccessor)
                    new AssemblyReflectionAccessor(assemblyName) :
                    new AssemblyDiagnosticsAccessor(assemblyName));
            }
        }
    }
}
