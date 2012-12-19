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
        string Title { get; }

        /// <summary>
        /// アセンブリバージョンを取得します。
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 概要を取得します。
        /// </summary>
        string Description { get; }

        /// <summary>
        /// プロダクト情報を取得します。
        /// </summary>
        string Product { get; }

        /// <summary>
        /// 会社情報を取得します。
        /// </summary>
        string Company { get; }

        /// <summary>
        /// 権利情報を取得します。
        /// </summary>
        string Copyright { get; }
    }
}
