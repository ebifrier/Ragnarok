using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Ragnarok.Net.CookieGetter
{
	/// <summary>
	/// Firefox4CCSのプロフィールを表現します。
	/// </summary>
	internal sealed class FirefoxProfile
	{
		public string Name;
		public bool IsRelative;
		public string FilePath;
		public bool IsDefault;

		/// <summary>
		/// 既定のプロファイルを取得する
		/// </summary>
        public static FirefoxProfile GetDefaultProfile(string moz_path,
                                                       string iniFileName)
        {
            FirefoxProfile[] profs = GetProfiles(moz_path, iniFileName);
            if (profs.Length == 1)
            {
                return profs[0];
            }
            else
            {
                foreach (FirefoxProfile prof in profs)
                {
                    if (prof.IsDefault)
                    {
                        return prof;
                    }
                }
            }

            return null;
        }

        public static IEnumerable<string> EachLines(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }

                yield return line;
            }
        }

		/// <summary>
		/// Firefoxのプロフィールフォルダ内のフォルダをすべて取得する
		/// </summary>
        public static FirefoxProfile[] GetProfiles(string moz_path,
                                                   string iniFileName)
        {
            string profile_path = Path.Combine(moz_path, iniFileName);
            List<FirefoxProfile> results = new List<FirefoxProfile>();

            if (!File.Exists(profile_path))
            {
                return results.ToArray();
            }

            using (StreamReader sr = new StreamReader(profile_path))
            {
                FirefoxProfile prof = null;

                foreach (string line in EachLines(sr))
                {
                    if (line.StartsWith("[Profile"))
                    {
                        prof = new FirefoxProfile();
                        results.Add(prof);
                    }

                    if (prof != null)
                    {
                        KeyValuePair<string, string> kvp = GetKVP(line);

                        switch (kvp.Key)
                        {
                            case "Name":
                                prof.Name = kvp.Value;
                                break;
                            case "IsRelative":
                                prof.IsRelative = (kvp.Value == "1");
                                break;
                            case "Path":
                                prof.FilePath = kvp.Value.Replace('/', '\\');
                                if (prof.IsRelative)
                                {
                                    prof.FilePath = Path.Combine(moz_path, prof.FilePath);
                                }
                                break;
                            case "Default":
                                prof.IsDefault = (kvp.Value == "1");
                                break;
                        }
                    }
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// Name=Value となっている行を解析します。
        /// </summary>
        public static KeyValuePair<string, string> GetKVP(string line)
        {
            string[] x = line.Split('=');
            if (x.Length == 2)
            {
                return new KeyValuePair<string, string>(x[0], x[1]);
            }

            return new KeyValuePair<string, string>();
        }
	}
}
