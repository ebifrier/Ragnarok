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
        public string Title
        {
            get { return this.internalAccessor.Title; }
        }

        /// <summary>
        /// アセンブリバージョンを取得します。
        /// </summary>
        public string Version
        {
            get { return this.internalAccessor.Version; }
        }

        /// <summary>
        /// 概要を取得します。
        /// </summary>
        public string Description
        {
            get { return this.internalAccessor.Description; }
        }

        /// <summary>
        /// プロダクト情報を取得します。
        /// </summary>
        public string Product
        {
            get { return this.internalAccessor.Product; }
        }

        /// <summary>
        /// 会社情報を取得します。
        /// </summary>
        public string Company
        {
            get { return this.internalAccessor.Company; }
        }

        /// <summary>
        /// 権利情報を取得します。
        /// </summary>
        public string Copyright
        {
            get { return this.internalAccessor.Copyright; }
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
