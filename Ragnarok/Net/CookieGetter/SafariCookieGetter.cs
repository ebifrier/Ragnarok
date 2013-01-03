using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace Ragnarok.Net.CookieGetter
{
	class SafariCookieGetter : CookieGetter
	{

		public SafariCookieGetter(CookieStatus status) : base(status)
		{
		}

		public override System.Net.CookieContainer GetAllCookies()
		{
			if (base.CookiePath == null || !System.IO.File.Exists(base.CookiePath)) {
				throw new CookieGetterException("Safariのクッキーパスが正しく設定されていません。");
			}

			System.Net.CookieContainer container = new System.Net.CookieContainer();

			// Safari5.1対応 07/30
			if(CheckSafariCookieBinary()) {
				// Cookies.binarycookiesの仕様がわからないので暫定対策
				// niconicoのsessionしか取得しませんのであしからず
			//	return GetNiconicoContainer();	// 07/30
				// 相変わらず仕様がわからないので暫定的にへぼい正規表現でさくっと
				return GetContainer();	// 08/08
			}

			// Safari5.0.5以下
			try {
				System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings();

				// DTDを取得するためにウェブアクセスするのを抑制する
				// (通信遅延や、アクセスエラーを排除するため)
				settings.XmlResolver = null;
#if CLR_V4
                settings.DtdProcessing = System.Xml.DtdProcessing.Ignore;
#else
				settings.ProhibitDtd = false;
#endif
				settings.CheckCharacters = false;

				using (System.Xml.XmlReader xtr = System.Xml.XmlTextReader.Create(base.CookiePath, settings)) {
					while (xtr.Read()) {
						switch (xtr.NodeType) {
							case System.Xml.XmlNodeType.Element:
								if (xtr.Name.ToLower().Equals("dict")) {
									System.Net.Cookie cookie = getCookie(xtr);
									try {
										CookieUtil.AddCookieToContainer(container, cookie);
									} catch (Exception ex){
										CookieGetter.Exceptions.Enqueue(ex);
										Console.WriteLine(string.Format("Invalid Format! domain:{0},key:{1},value:{2}", cookie.Domain, cookie.Name, cookie.Value));
									}
								}
								break;
						}
					}
				}

			} catch (Exception ex) {
				throw new CookieGetterException("Safariのクッキー取得中にエラーが発生しました。", ex);
			}

			return container;
		}

		private System.Net.Cookie getCookie(System.Xml.XmlReader xtr)
		{
			bool isEnd = false;
			System.Net.Cookie cookie = new System.Net.Cookie();
			string tagName = "";
			string kind = "";

			while (xtr.Read() && !isEnd) {
				switch (xtr.NodeType) {
					case System.Xml.XmlNodeType.Element:
						tagName = xtr.Name.ToLower();
						break;

					case System.Xml.XmlNodeType.Text:
						switch (tagName) {
							case "key":
								kind = xtr.Value.ToLower();
								break;
							case "real":
							case "string":
							case "date":
								switch (kind) {
									case "domain":
										cookie.Domain = xtr.Value;
										break;
									case "name":
										cookie.Name = xtr.Value;
										break;
									case "value":
										cookie.Value = xtr.Value;
										if (cookie.Value != null) {
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

					case System.Xml.XmlNodeType.EndElement:
						if (xtr.Name.ToLower() == "dict") {
							isEnd = true;
						}
						break;

				}
			}

			return cookie;
		}



		// Sfari5.1対応のへぼいコード

		// 08/08

		private System.Net.CookieContainer GetContainer() {
			System.Net.CookieContainer container = new System.Net.CookieContainer();

			try {
				byte[] CookieData = System.IO.File.ReadAllBytes(base.CookiePath);
			//	String CookieTxt = Encoding.GetEncoding(932).GetString(CookieData);
				String CookieTxt = Encoding.ASCII.GetString(CookieData);	// 08/09
				CookieTxt = CookieTxt.Replace("\0", "\t");

				System.Net.Cookie cookie = new System.Net.Cookie();

				// この正規表現があっていなければCookieの取得もれがあります。
				// niconicoのCookieが取得できるので問題ないかな…
				// 充分な動作確認しておりません。
				Match cookieChunk = Regex.Match(CookieTxt, @"\t\t\t.....([\w.%+-\\(\\)\[\]|]+)\t([\w.%+-\\(\\)\[\]|]+)\t([\w.+-]+)\t([\w.%+-\\(\\)\[\]|/]+)\t", RegexOptions.Multiline);
				int count = 0;
				while(cookieChunk.Success) {
					try {
						string CookieText = cookieChunk.Groups[1].Value + "\t" + cookieChunk.Groups[2].Value + "\t" + cookieChunk.Groups[3].Value + "\t" + cookieChunk.Groups[4].Value;

						cookie = getCookie(CookieText);
						try {
							CookieUtil.AddCookieToContainer(container, cookie);
						}
						catch(Exception ex) {
							CookieGetter.Exceptions.Enqueue(ex);
							Console.WriteLine(string.Format("Invalid Format! domain:{0},key:{1},value:{2}", cookie.Domain, cookie.Name, cookie.Value));
						}
					}
					catch {
					}
					cookieChunk = cookieChunk.NextMatch();
					count++;
				}
			}
			catch(Exception ex) {
				throw new CookieGetterException("Safariのクッキー取得中にエラーが発生しました。", ex);
			}

			return container;
		}



		// 07/30

		private System.Net.CookieContainer GetNiconicoContainer() {
			System.Net.CookieContainer container = new System.Net.CookieContainer();

			try {
				// 一度にすべて読み込む（何MBにもならないでしょうからいいよね？）
				// 本当は手抜きしないで1kB毎に読んだらいいと思うけど
				// あーメモリの無駄使い
				byte[] CookieData = System.IO.File.ReadAllBytes(base.CookiePath);
				String CookieTxt = Encoding.ASCII.GetString(CookieData);
				CookieTxt = CookieTxt.Replace("\0", "\t");

				System.Net.Cookie cookie = new System.Net.Cookie();

				int StartIndex = 0;
				int LastIndex;
				do {
					StartIndex = CookieTxt.IndexOf("user_session", StartIndex);
					if(StartIndex == -1) {
						break;
					}
					LastIndex = StartIndex;

					for(int i = 0; i < 4; i++) {
						LastIndex = CookieTxt.IndexOf("\t", LastIndex) + 1;
					}

					string CookieText = CookieTxt.Substring(StartIndex, LastIndex - StartIndex);
					cookie = getCookie(CookieText);

					try {
						CookieUtil.AddCookieToContainer(container, cookie);
					}
					catch(Exception ex) {
						CookieGetter.Exceptions.Enqueue(ex);
						Console.WriteLine(string.Format("Invalid Format! domain:{0},key:{1},value:{2}", cookie.Domain, cookie.Name, cookie.Value));
					}

					StartIndex = LastIndex;
				} while(true);
			}
			catch(Exception ex) {
				throw new CookieGetterException("Safariのクッキー取得中にエラーが発生しました。", ex);
			}

			return container;
		}

		private System.Net.Cookie getCookie(string CookieText) {
			System.Net.Cookie cookie = new System.Net.Cookie();
			
			string[] CookieTexts = CookieText.Split('\t');
			if(CookieTexts.Length < 4) {
				return cookie;
			}

			cookie.Name = CookieTexts[0];
			cookie.Value = CookieTexts[1];
			cookie.Domain = CookieTexts[2];
			cookie.Path = CookieTexts[3];

			return cookie;
		}

		private bool CheckSafariCookieBinary() {
			try {
				using(FileStream fileStream = new FileStream(base.CookiePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
					string Header = "cook";
					byte[] HeaderByte = Encoding.ASCII.GetBytes(Header);
					int ReadLength = HeaderByte.Length;
					byte[] HeaderTemp = new byte[ReadLength];

					if(fileStream.Length < ReadLength) {
						return false;
					}

					try {
						using(BinaryReader binaryReader = new BinaryReader(fileStream)) {
							fileStream.Seek(0x00L, SeekOrigin.Begin);
							HeaderTemp = binaryReader.ReadBytes(ReadLength);
						}
					}
					catch {
						return false;
					}

					for(int i = 0; i < ReadLength; i++) {
						if(HeaderByte[i] != HeaderTemp[i]) {
							return false;
						}
					}
				}
			}
			catch {
				return false;
			}

			return true;
		}

	}
}
