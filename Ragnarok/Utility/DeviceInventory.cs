using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace Ragnarok.Utility
{
    /// <summary>
    /// 実行環境・デバイス情報を取得します。
    /// </summary>
    public static class DeviceInventory
    {
#if !MONO
        #region PInvoke
        [DllImport("kernel32.dll")]
        extern static void GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }
        #endregion
#endif

        /// <summary>
        /// OSが64bitかどうか取得します。
        /// </summary>
        public static bool Is64BitOS
        {
            get { return Environment.Is64BitOperatingSystem; }
        }

        /// <summary>
        /// CPUの論理個数を取得します。(HTは考えていません)
        /// </summary>
        public static int CPUCount
        {
            get { return Environment.ProcessorCount; }
        }

        /// <summary>
        /// 物理メモリ量をバイト単位で取得します。
        /// </summary>
        public static long MemorySize
        {
            get;
            private set;
        }

        /// <summary>
        /// OSのバージョンを取得します。
        /// </summary>
        public static string OSVersion
        {
            get;
            private set;
        }

        /*
        public string BuildRequestUrl(string baseRequestUrl)
        {
            string retValue = baseRequestUrl;
            
            // x64 
            retValue += "cpu64bit=" + (x64System ? "1" : "0") + "&";

            // cpu speed
            retValue += "cpuFreqMHz=" + ProcessorSpeed + "&";

            // ram size
            retValue += "ramMB=" + MemorySize + "&";

            // Application name (as indicated by CFBundleName)
            retValue += "appName=" + _config.ApplicationName + "&";

            // Application version (as indicated by CFBundleVersion)
            retValue += "appVersion=" + _config.InstalledVersion + "&";

            // User’s preferred language
            retValue += "lang=" + Thread.CurrentThread.CurrentUICulture.ToString() + "&";

            // Windows version
            retValue += "osVersion=" + OsVersion + "&";

            // CPU type/subtype (see mach/machine.h for decoder information on this data)
            // ### TODO: cputype, cpusubtype ###

            // Mac model
            // ### TODO: model ###

            // Number of CPUs (or CPU cores, in the case of something like a Core Duo)
            // ### TODO: ncpu ###
            retValue += "ncpu=" + CPUCount + "&";

            // sanitize url
            retValue = retValue.TrimEnd('&');            

            // go ahead
            return retValue;
        }*/

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static DeviceInventory()
        {
            // ram size
            CollectRamSize();

            // os
            OSVersion = GetOSVersion();
        }

        /// <summary>
        /// RAMサイズを取得します。
        /// </summary>
        private static void CollectRamSize()
        {
#if MONO
            MemorySize = 0;
#else
            var ms = new MEMORYSTATUSEX();
            ms.dwLength = (uint)Marshal.SizeOf(ms);
            GlobalMemoryStatusEx(ref ms);

            // MemorySizeはbyte単位
            MemorySize = (long)ms.ullAvailPhys;
#endif
        }

        /// <summary>
        /// OSのバージョンを取得します。
        /// </summary>
        private static string GetOSVersion()
        {
            var os = Environment.OSVersion;
            
            switch (os.Platform)
            {
                case PlatformID.Win32Windows:
                    if (os.Version.Major >= 4)
                    {
                        switch (os.Version.Minor)
                        {
                            case 0:
                                return "Windows 95";
                            case 10:
                                return "Windows 98";
                            case 90:
                                return "Windows Me";
                        }
                    }
                    return "Windows 95 以降";
                case PlatformID.Win32NT:
                    switch (os.Version.Major)
                    {
                        case 3:
                            switch (os.Version.Minor)
                            {
                                case 0:
                                    return "Windows NT 3";
                                case 1:
                                    return "Windows NT 3.1";
                                case 5:
                                    return "Windows NT 3.5";
                                case 51:
                                    return "Windows NT 3.51";
                            }
                            break;
                        case 4:
                            if (os.Version.Minor == 0)
                            {
                                return "Windows NT 4.0";
                            }
                            break;
                        case 5:
                            switch (os.Version.Minor)
                            {
                                case 0:
                                    return "Windows 2000";
                                case 1:
                                    return "Windows XP";
                                case 2:
                                    return "Windows Server 2003";
                            }
                            break;
                        case 6:
                            switch (os.Version.Minor)
                            {
                                case 0:
                                    return "Windows Vista";
                                    //Console.WriteLine(" または Windows Server 2008 です。");
                                case 1:
                                    return "Windows 7";
                                    //Console.WriteLine(" または Windows Server 2008 R2 です。");
                            }
                            break;
                    }
                    return "Windows NT 以降";
                case PlatformID.Win32S:
                    return "OSは Win32s";
                case PlatformID.WinCE:
                    return "Windows CE";
                case PlatformID.Unix:
                    //.NET Framework 2.0以降
                    return "Unix";
                case PlatformID.Xbox:
                    //.NET Framework 3.5以降
                    return "Xbox 360";
                case PlatformID.MacOSX:
                    //.NET Framework 3.5以降
                    return "Macintosh";
            }

            return "（不明）";
        }
    }
}
