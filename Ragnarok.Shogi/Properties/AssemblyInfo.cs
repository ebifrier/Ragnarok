using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !MONO
using System.Windows.Markup;
#endif

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更してください。
[assembly: AssemblyTitle("Ragnarok.Shogi")]
[assembly: AssemblyDescription("将棋の究極ライブラリです。")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("co516151")]
[assembly: AssemblyProduct("Ragnarok.Shogi")]
[assembly: AssemblyCopyright("Copyright © えびふらい 2012-2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// ComVisible を false に設定すると、その型はこのアセンブリ内で COM コンポーネントから 
// 参照不可能になります。COM からこのアセンブリ内の型にアクセスする場合は、
// その型の ComVisible 属性を true に設定してください。
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

// 次の GUID は、このプロジェクトが COM に公開される場合の、typelib の ID です
[assembly: Guid("9c28a541-043c-45b0-b537-c770636f0063")]

#if !MONO
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.Shogi")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.Shogi.Bonanza")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.Shogi.Csa")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.Shogi.File")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.Shogi.Kif")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.Shogi.Sfen")]
#endif

// アセンブリのバージョン情報は、以下の 4 つの値で構成されています:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// すべての値を指定するか、下のように '*' を使ってビルドおよびリビジョン番号を 
// 既定値にすることができます:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
//[assembly: AssemblyFileVersion("1.0.0.0")]
