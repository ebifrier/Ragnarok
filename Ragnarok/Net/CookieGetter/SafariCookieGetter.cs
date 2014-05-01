using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Ragnarok.Net.CookieGetter
{
	class SafariCookieGetter : CookieGetter
	{
		public SafariCookieGetter(CookieStatus status) : base(status)
		{
		}

		public override CookieContainer GetAllCookies()
		{
            if (base.CookiePath == null || !File.Exists(base.CookiePath))
            {
                throw new CookieGetterException(
                    "Safariのクッキーパスが正しく設定されていません。");
            }

			CookieContainer container = new CookieContainer();

			// Safari5.1対応 07/30
            if (CheckSafariCookieBinary())
            {
                // 相変わらず仕様がわからないので暫定的にへぼい正規表現でさくっと
                return GetContainer();	// 08/08
            }

			// Safari5.0.5以下
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();

                // DTDを取得するためにウェブアクセスするのを抑制する
                // (通信遅延や、アクセスエラーを排除するため)
                settings.XmlResolver = null;
#if CLR_GE_4_0
                settings.DtdProcessing = DtdProcessing.Ignore;
#else
				settings.ProhibitDtd = false;
#endif
                settings.CheckCharacters = false;

                using (XmlReader xtr = XmlTextReader.Create(base.CookiePath, settings))
                {
                    while (xtr.Read())
                    {
                        switch (xtr.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (xtr.Name.ToLower().Equals("dict"))
                                {
                                    Cookie cookie = getCookie(xtr);
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
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CookieGetterException(
                    "Safariのクッキー取得中にエラーが発生しました。", ex);
            }

			return container;
		}

        private Cookie getCookie(XmlReader xtr)
        {
            Cookie cookie = new Cookie();
            string tagName = "";
            string kind = "";
            bool isEnd = false;

            while (xtr.Read() && !isEnd)
            {
                switch (xtr.NodeType)
                {
                    case XmlNodeType.Element:
                        tagName = xtr.Name.ToLower();
                        break;

                    case XmlNodeType.Text:
                        switch (tagName)
                        {
                            case "key":
                                kind = xtr.Value.ToLower();
                                break;
                            case "real":
                            case "string":
                            case "date":
                                switch (kind)
                                {
                                    case "domain":
                                        cookie.Domain = xtr.Value;
                                        break;
                                    case "name":
                                        cookie.Name = xtr.Value;
                                        break;
                                    case "value":
                                        cookie.Value = xtr.Value;
                                        if (cookie.Value != null)
                                        {
                                            cookie.Value = Uri.EscapeDataString(cookie.Value);
                                        }
                                        break;
                                    case "expires":
                                        cookie.Expires = DateTime.Parse(xtr.Value);
                                        break;
                                    case "path":
                                        cookie.Path = xtr.Value;
                                        break;
                                }
                                break;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        if (xtr.Name.ToLower() == "dict")
                        {
                            isEnd = true;
                        }
                        break;
                }
            }

            return cookie;
        }


		// Sfari5.1対応のへぼいコード

		// 08/08

        private static readonly Regex CookieChunkRegex = new Regex(
            @"\t\t\t.....([\w.%+-\\(\\)\[\]|]+)\t([\w.%+-\\(\\)\[\]|]+)\t([\w.+-]+)\t([\w.%+-\\(\\)\[\]|/]+)\t",
            RegexOptions.Multiline);

        private CookieContainer GetContainer()
        {
            CookieContainer container = new CookieContainer();

            try
            {
                byte[] CookieData = File.ReadAllBytes(base.CookiePath);
                string CookieText = Encoding.ASCII.GetString(CookieData);	// 08/09
                CookieText = CookieText.Replace("\0", "\t");

                // この正規表現があっていなければCookieの取得もれがあります。
                // niconicoのCookieが取得できるので問題ないかな…
                // 充分な動作確認しておりません。
                for (Match cookieChunk = CookieChunkRegex.Match(CookieText);
                     cookieChunk.Success;
                     cookieChunk = cookieChunk.NextMatch())
                {
                    try
                    {
                        Cookie cookie = new Cookie
                        {
                            Name = cookieChunk.Groups[1].Value,
                            Value = cookieChunk.Groups[2].Value,
                            Domain = cookieChunk.Groups[3].Value,
                            Path = cookieChunk.Groups[4].Value,
                        };

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
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CookieGetterException(
                    "Safariのクッキー取得中にエラーが発生しました。", ex);
            }

            return container;
        }

        private bool CheckSafariCookieBinary()
        {
            try
            {
                using (FileStream fileStream = new FileStream(
                    base.CookiePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    string Header = "cook";
                    byte[] HeaderByte = Encoding.ASCII.GetBytes(Header);
                    int ReadLength = HeaderByte.Length;
                    byte[] HeaderTemp = new byte[ReadLength];

                    if (fileStream.Length < ReadLength)
                    {
                        return false;
                    }

                    try
                    {
                        using (BinaryReader binaryReader = new BinaryReader(fileStream))
                        {
                            fileStream.Seek(0x00L, SeekOrigin.Begin);
                            HeaderTemp = binaryReader.ReadBytes(ReadLength);
                        }
                    }
                    catch
                    {
                        return false;
                    }

                    for (int i = 0; i < ReadLength; i++)
                    {
                        if (HeaderByte[i] != HeaderTemp[i])
                        {
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
	}
}
