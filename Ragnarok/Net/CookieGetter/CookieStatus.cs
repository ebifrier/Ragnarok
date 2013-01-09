using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ragnarok.Net.CookieGetter
{
	/// <summary>
	/// PathがFileとDirectoryのどちらを示しているかを表す
	/// </summary>
	internal enum PathType
	{
		/// <summary>
		/// ファイル
		/// </summary>
		File,
		/// <summary>
		/// ディレクトリ
		/// </summary>
		Directory
	}

	/// <summary>
	/// CookieGetterの状態を表すインターフェース
	/// </summary>
	[Serializable]
	public sealed class CookieStatus : IEquatable<CookieStatus>
	{
        private string displayName;

        private CookieStatus()
        {
        }

		internal CookieStatus(string name, string path, BrowserType browserType, PathType pathType)
		{
			Name = name;
            CookiePath = path;
			BrowserType = browserType;
			PathType = pathType;

			this.displayName = null;
		}

		/// <summary>
		/// ブラウザの種類を取得する
		/// </summary>
        public BrowserType BrowserType
        {
            get;
            private set;
        }

        /// <summary>
        /// 識別名を取得する
        /// </summary>
        public string Name
        {
            get;
            internal set;
        }

        /// <summary>
        /// クッキーが保存されているフォルダを取得、設定する
        /// </summary>
        public string CookiePath
        {
            get;
            internal set;
        }

        /// <summary>
        /// CookiePathがFileを表すのか、Directoryを表すのかを取得する
        /// </summary>
        internal PathType PathType
        {
            get;
            private set;
        }

		/// <summary>
		/// 利用可能かどうかを取得する
		/// </summary>
        public bool IsAvailable
        {
            get
            {
                if (string.IsNullOrEmpty(CookiePath))
                {
                    return false;
                }

                if (PathType == PathType.File)
                {
                    return File.Exists(CookiePath);
                }
                else
                {
                    return Directory.Exists(CookiePath);
                }
            }
        }

		/// <summary>
		/// ToStringで表示される名前。nullにするとNameが表示されるようになる。
		/// </summary>
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(this.displayName))
                {
                    return Name;
                }

                return this.displayName;
            }
            set
            {
                this.displayName = value;
            }
        }

		#region Objectのオーバーライド
		/// <summary>
		/// DisplayNameを返します
		/// </summary>
		public override string ToString()
		{
			return DisplayName;
		}

		/// <summary>
		/// ブラウザ名、クッキー保存先が等しいかを調べます
		/// </summary>
        public override bool Equals(object obj)
        {
            bool? result = this.PreEquals(obj);
            if (result.HasValue)
            {
                return result.Value;
            }

            return Equals(obj as CookieStatus);
        }

        /// <summary>
        /// ブラウザ名、クッキー保存先が等しいかを調べます
        /// </summary>
        public bool Equals(CookieStatus obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (Name != obj.Name)
            {
                return false;
            }

            if (CookiePath != obj.CookiePath)
            {
                return false;
            }

            return true;
        }

		/// <summary>
		/// ハッシュコードを返します
		/// </summary>
		public override int GetHashCode()
		{
			string x = Name + CookiePath;

			return x.GetHashCode();
		}
		#endregion
	}
}
