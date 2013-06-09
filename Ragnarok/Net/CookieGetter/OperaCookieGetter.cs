using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Ragnarok.Net.CookieGetter
{
    /// <summary>
    /// Operaはクッキーを独自のバイナリで管理する。
    /// </summary>
    /// <remarks>
    /// 分けわからん
    /// 
    /// 参考：http://www.opera.com/docs/operafiles/#cookies
    /// </remarks>
	internal sealed class OperaCookieGetter : CookieGetter
	{
		private const byte MSB = 0x80;

		private struct Header
		{
			public int file_version_number;
			public int app_version_number;
			public int idtag_length;
			public int length_length;
		}

		private struct Record
		{
			// application specific tag to identify content type
			public int tag_id;
			// length of payload
			public int length;
			// Payload/content of the record
			public byte[] bytepayload;
		};

		public OperaCookieGetter(CookieStatus status) : base(status)
		{
		}

        public override CookieContainer GetAllCookies()
        {
            if (base.CookiePath == null || !File.Exists(base.CookiePath))
            {
                throw new CookieGetterException(
                    "Operaのクッキーパスが正しく設定されていません。");
            }

            CookieContainer container = new CookieContainer();
            try
            {
                using (FileStream reader = new FileStream(
                    base.CookiePath, FileMode.Open, FileAccess.Read))
                {
                    Header headerData = getHeader(reader);

                    //version check
                    if ((headerData.file_version_number & 0xfffff000) == 0x00001000)
                    {
                        foreach (Cookie cookie in getCookies(headerData, reader))
                        {
                            try
                            {
                                CookieUtil.AddCookieToContainer(container, cookie);
                            }
                            catch (Exception ex)
                            {
                                CookieGetter.Exceptions.Enqueue(ex);

                                Console.WriteLine(string.Format(
                                    "Invalid Format! domain:{0},key:{1},value:{2}",
                                    cookie.Domain, cookie.Name, cookie.Value));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CookieGetterException(
                    "Operaのクッキー取得でエラーが発生しました。", ex);
            }

            return container;
        }

        private IEnumerable<Cookie> getCookies(Header headerData, Stream reader)
        {
            Stack<string> domainStack = new Stack<string>();
            Stack<string> pathStack = new Stack<string>();

            while (reader.Position < reader.Length)
            {
                Record recordData = getRecord(reader, headerData);

                switch (recordData.tag_id)
                {
                    case 0x01:  // ドメイン
                        string domain;
                        using (MemoryStream ms = new MemoryStream(recordData.bytepayload))
                        {
                            domain = getDomainRecord(ms, headerData);
                        }
                        if (domain != null)
                        {
                            domainStack.Push(domain);
                        }
                        break;
                    case 0x02:  // パス
                        string page;
                        using (MemoryStream ms = new MemoryStream(recordData.bytepayload))
                        {
                            page = getPageRecord(ms, headerData);
                        }

                        if (page != null)
                        {
                            pathStack.Push(page);
                        }
                        break;
                    case 0x03:  // クッキー
                        Cookie cookie;
                        using (MemoryStream ms = new MemoryStream(recordData.bytepayload))
                        {
                            cookie = getCookieRecord(ms, headerData);
                        }
                        cookie.Domain = '.' + string.Join(".", domainStack.ToArray());
                        cookie.Path = '/' + string.Join("/", pathStack.ToArray());

                        yield return cookie;
                        break;
                    case 0x04 + MSB: //ドメイン終了
                        if (0 < domainStack.Count)
                        {
                            domainStack.Pop();
                        }
                        break;
                    case 0x05 + MSB: //パス終了
                        if (0 < pathStack.Count)
                        {
                            pathStack.Pop();
                        }
                        break;
                }
            }
        }

        private string getDomainRecord(Stream stream, Header headerData)
        {
            Record recordData;

            while (stream.Position < stream.Length)
            {
                recordData = getRecord(stream, headerData);

                switch (recordData.tag_id)
                {
                    case 0x1e:  // Domain Name
                        return Encoding.ASCII.GetString(recordData.bytepayload);
                }
            }

            return null;
        }

        private string getPageRecord(Stream stream, Header headerData)
        {
            Record recordData;

            while (stream.Position < stream.Length)
            {
                recordData = getRecord(stream, headerData);

                switch (recordData.tag_id)
                {
                    case 0x1d:  // Page Name
                        return Encoding.ASCII.GetString(recordData.bytepayload);
                }
            }

            return null;
        }

        private Cookie getCookieRecord(Stream stream, Header headerData)
        {
            Record recordData;
            Cookie cookie = new Cookie();

            while (stream.Position < stream.Length)
            {
                recordData = getRecord(stream, headerData);

                switch (recordData.tag_id)
                {
                    case 0x10:  // Cookie Name
                        cookie.Name = Encoding.ASCII.GetString(recordData.bytepayload);
                        break;
                    case 0x11:  // Cookie Value
                        cookie.Value = Encoding.ASCII.GetString(recordData.bytepayload);
                        if (cookie.Value != null)
                        {
                            cookie.Value = Uri.EscapeDataString(cookie.Value);
                        }
                        break;
                    case 0x12:
                        long time;
                        using (MemoryStream ms = new MemoryStream(recordData.bytepayload))
                        {
                            time = getNumber(ms, 8);
                        }
                        cookie.Expires = CookieUtil.UnixTimeToDateTime((int)time);
                        break;
                }
            }

            return cookie;
        }

		private Header getHeader(Stream stream)
		{
			Header headerData = new Header();

			headerData.file_version_number = (int)getNumber(stream, 4);
			headerData.app_version_number = (int)getNumber(stream, 4);
			headerData.idtag_length = (int)getNumber(stream, 2);
			headerData.length_length = (int)getNumber(stream, 2);

			return headerData;
		}

        private Record getRecord(Stream stream, Header headerData)
        {
            Record recordData = new Record();
            int topData = stream.ReadByte();
            stream.Seek(-1, SeekOrigin.Current);
            recordData.tag_id = (int)getNumber(stream, headerData.idtag_length);

            // MSBがONのとき、Tag IDのみになる。（以降のDataLength,Dataはない）
            if ((topData & MSB) == 0)
            {
                recordData.length = (int)getNumber(stream, headerData.length_length);
                recordData.bytepayload = new byte[recordData.length];
                stream.Read(recordData.bytepayload, 0, recordData.length);
            }

            return recordData;
        }

		private long getNumber(Stream stream, int length)
		{
			long n = 0;
			for (int i = 0; i < length; i++) {
				n <<= 8;
				n += stream.ReadByte();
			}

			return n;
		}
	}
}
