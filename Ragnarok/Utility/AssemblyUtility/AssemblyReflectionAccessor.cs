using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;

namespace Ragnarok.Utility.AssemblyUtility
{
    /// <summary>
    /// アセンブリから情報を取得するための内部クラスです。
    /// </summary>
    internal sealed class AssemblyReflectionUtil
    {
        private readonly List<Attribute> attributes = new List<Attribute>();
        private readonly Assembly assembly;

        /// <summary>
        /// アセンブリを取得します。
        /// </summary>
        public Assembly Assembly
        {
            get { return this.assembly; }
        }

        /// <summary>
        /// アセンブリを読み込みます。
        /// </summary>
        /// <remarks>
        /// <paramref name="assemblyName"/>がnullなら起動アセンブリを読み込みます。
        /// </remarks>
        private static Assembly LoadAssembly(string assemblyName)
        {
            if (assemblyName == null)
            {
                return Assembly.GetEntryAssembly();
            }
            else
            {
                var absolutePath = Path.GetFullPath(assemblyName);
                if (!File.Exists(absolutePath))
                {
                    throw new FileNotFoundException(
                        "アセンブリファイルが見つかりません。",
                        absolutePath);
                }

                var assembly = Assembly.ReflectionOnlyLoadFrom(absolutePath);
                if (assembly == null)
                {
                    throw new RagnarokException(
                        "Unable to load assembly " + absolutePath);
                }

                return assembly;
            }
        }

        /// <summary>
        /// 属性を検索します。
        /// </summary>
        public T FindAttribute<T>()
            where T : Attribute
        {
            var result = this.attributes
                .Where(_ => _.GetType().Equals(typeof(T)))
                .FirstOrDefault();

            if (result == null)
            {
                throw new RagnarokException(
                    $"属性 {typeof(T)} が {this.assembly.FullName} に存在しません。");
            }

            return (T)result;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AssemblyReflectionUtil(string assemblyName)
        {
            this.assembly = LoadAssembly(assemblyName);

            // アセンブリの属性情報読み込み。
            this.attributes = this.assembly.GetCustomAttributes(true)
                .OfType<Attribute>()
                .Where(_ => _ != null)
                .ToList();
        }
    }

    /// <summary>
    /// リフレクションによりアセンブリ情報を取得します。
    /// </summary>
    public class AssemblyReflectionAccessor : IAssemblyAccessor
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
        public AssemblyReflectionAccessor(string assemblyName)
        {
            var util = new AssemblyReflectionUtil(assemblyName);

            var titleAttr = util.FindAttribute<AssemblyTitleAttribute>();
            Title = titleAttr.Title;

            var nameInfo = util.Assembly.GetName();
            Version = nameInfo.Version.ToString();

            var descAttr = util.FindAttribute<AssemblyDescriptionAttribute>();
            Description = descAttr.Description;

            var productAttr = util.FindAttribute<AssemblyProductAttribute>();
            Product = productAttr.Product;

            var copyAttr = util.FindAttribute<AssemblyCopyrightAttribute>();
            Copyright = copyAttr.Copyright;

            var companyAttr = util.FindAttribute<AssemblyCompanyAttribute>();
            Company = companyAttr.Company;
        }
    }
}
