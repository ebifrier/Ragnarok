using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Ragnarok.Utility.AssemblyUtility;

namespace Ragnarok.Update
{
    /// <summary>
    /// This class handles all registry values which are used from sparkle to handle 
    /// update intervalls. All values are stored in HKCU\Software\Vendor\AppName which 
    /// will be read ot from the assembly information. All values are of the REG_SZ 
    /// type, no matter what their "logical" type is. The following options are
    /// available:
    /// 
    /// CheckForUpdate  - bool    - Whether NetSparkle should check for updates
    /// LastCheckTime   - time_t  - Time of last check
    /// SkipThisVersion - string  - If the user skipped an update, then the version to ignore is stored here (e.g. "1.4.3")
    /// </summary>
    public sealed class Configuration
    {
        private IAssemblyAccessor accessor;

        /// <summary>
        /// アプリ名を取得します。
        /// </summary>
        public string ApplicationName
        {
            get { return this.accessor.AssemblyTitle; }
        }

        /// <summary>
        /// 現在インストールされているアプリのバージョンを取得します。
        /// </summary>
        public string InstalledVersion
        {
            get { return this.accessor.AssemblyVersion; }
        }

        public bool CheckForUpdate
        {
            get;
            private set;
        }

        public string SkipThisVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// This method allows to skip a specific version
        /// </summary>
        /// <param name="version"></param>
        public void SetVersionToSkip(string version)
        {
            // set the check tiem
            SkipThisVersion = version;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>        
        public Configuration(string assemblyName)
            : this(assemblyName, true)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Configuration(string assemblyName,
                             bool useReflectionBasedAssemblyAccessor)
        {
            try
            {
                CheckForUpdate = true;
                SkipThisVersion = string.Empty;

                // set some value from the binary
                this.accessor = new AssemblyAccessor(
                    assemblyName, useReflectionBasedAssemblyAccessor);
            }
            catch (Exception)
            {
                // disable update checks when exception was called 
                CheckForUpdate = false;
            }
        }
    }
}
