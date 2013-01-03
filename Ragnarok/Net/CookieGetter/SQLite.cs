using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Data.SQLite;

namespace Ragnarok.Net.CookieGetter
{
	/// <summary>
	/// SQLiteを利用してクッキーを保存するタイプのブラウザからクッキーを取得するクラス
	/// </summary>
    internal static class SQLite
	{
		private const string CONNECTIONSTRING_FORMAT = "Data Source={0}";

		/*public override Cookie GetCookie(Uri url, string key)
		{
			CookieContainer container = GetCookies(base.CookiePath, MakeQuery(url, key));
			CookieCollection collection = container.GetCookies(Utility.AddSrashLast(url));
			return collection[key];
		}

		public override CookieCollection GetCookieCollection(Uri url)
		{
			CookieContainer container = GetCookies(base.CookiePath, MakeQuery(url));
			return container.GetCookies(Utility.AddSrashLast(url));
		}

		public override CookieContainer GetAllCookies()
		{
			return GetCookies(base.CookiePath, MakeQuery());
		}*/

        public static List<List<object>> GetCookies(string path, string query)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                throw new CookieGetterException("クッキーパスが正しく設定されていません。");
            }
            
            string temp = null;
            string tempshm = null;
            string tempwal = null;
            try
            {
                // 一時ファイルの取得ができない環境に対応
                temp = Path.GetTempFileName();
                File.Copy(path, temp, true);

                // SQLite3.7.x
                string pathshm = path + "-shm";
                string pathwal = path + "-wal";
                tempshm = Path.GetTempFileName();
                tempwal = Path.GetTempFileName();
                if (File.Exists(pathshm))
                {
                    File.Copy(pathshm, tempshm, true);
                    File.Copy(pathwal, tempwal, true);
                }

                // 5ミリ秒
                Thread.Sleep(5);

                string connStr = string.Format(CONNECTIONSTRING_FORMAT, temp);
                using (SQLiteConnection sqlConnection = new SQLiteConnection(connStr))
                {
                    sqlConnection.Open();

                    SQLiteCommand command = sqlConnection.CreateCommand();
                    command.Connection = sqlConnection;
                    command.CommandText = query;
                    SQLiteDataReader reader = command.ExecuteReader();

                    List<List<object>> result = new List<List<object>>();
                    while (reader.Read())
                    {
                        List<object> items = new List<object>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            items.Add(reader[i]);
                        }

                        result.Add(items);

                        /*Cookie cookie = DataToCookie(items.ToArray());
                        try {
                            Utility.AddCookieToContainer(container, cookie);
                        } catch (Exception ex){
                            CookieGetter.Exceptions.Enqueue(ex);
                            Console.WriteLine(string.Format("Invalid Format! domain:{0},key:{1},value:{2}", cookie.Domain, cookie.Name, cookie.Value));
                        }*/
                    }

                    sqlConnection.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new CookieGetterException(
                    "クッキーを取得中、SQLiteアクセスでエラーが発生しました。", ex);
            }
            finally
            {
                if (File.Exists(temp))
                {
                    File.Delete(temp);
                }

                if (File.Exists(tempshm))
                {
                    File.Delete(tempshm);
                }

                if (File.Exists(tempwal))
                {
                    File.Delete(tempwal);
                }
            }
        }
	}
}
