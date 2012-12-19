using System;

namespace Ragnarok.Utility.AssemblyUtility
{
    /// <summary>
    /// アセンブリ情報にアクセスするための基本インターフェースです。
    /// </summary>
    public interface IAssemblyAccessor
    {
        /// <summary>
        /// アセンブリ名を取得します。
        /// </summary>
        string AssemblyTitle { get; }

        /// <summary>
        /// アセンブリバージョンを取得します。
        /// </summary>
        string AssemblyVersion { get; }

        /// <summary>
        /// 概要を取得します。
        /// </summary>
        string AssemblyDescription { get; }

        /// <summary>
        /// プロダクト情報を取得します。
        /// </summary>
        string AssemblyProduct { get; }

        /// <summary>
        /// 会社情報を取得します。
        /// </summary>
        string AssemblyCompany { get; }

        /// <summary>
        /// 権利情報を取得します。
        /// </summary>
        string AssemblyCopyright { get; }
    }
}
