using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Markup;

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更してください。
[assembly: AssemblyTitle("Ragnarok.NicoNico")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("co516151")]
[assembly: AssemblyProduct("Ragnarok.NicoNico")]
[assembly: AssemblyCopyright("Copyright @ ebifrier 2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// ComVisible を false に設定すると、その型はこのアセンブリ内で COM コンポーネントから 
// 参照不可能になります。COM からこのアセンブリ内の型にアクセスする場合は、
// その型の ComVisible 属性を true に設定してください。
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]

// このプロジェクトが COM に公開される場合、次の GUID が typelib の ID になります
[assembly: Guid("6aee649c-ee42-4ae5-87e3-a4f8bfc12f72")]

[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.CookieGetter")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.NicoNico")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.NicoNico.Live")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.NicoNico.Login")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.NicoNico.Provider")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.NicoNico.Video")]

#if !MONO
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.CookieGetter")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.NicoNico")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.NicoNico.Live")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.NicoNico.Login")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.NicoNico.Provider")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.NicoNico.Video")]
#endif

// アセンブリのバージョン情報は次の 4 つの値で構成されています:
//
//      メジャー バージョン
//      マイナー バージョン
//      ビルド番号
//      Revision
//
// すべての値を指定するか、下のように '*' を使ってビルドおよびリビジョン番号を 
// 既定値にすることができます:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
