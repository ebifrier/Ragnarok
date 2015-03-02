using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Markup;

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更してください。
[assembly: AssemblyTitle("Ragnarok.Extra")]
[assembly: AssemblyDescription("究極ライブラリ(補助)です。")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("co516151")]
[assembly: AssemblyProduct("Ragnarok.Extra")]
[assembly: AssemblyCopyright("Copyright えびふらい 2012-2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// ComVisible を false に設定すると、その型はこのアセンブリ内で COM コンポーネントから 
// 参照不可能になります。COM からこのアセンブリ内の型にアクセスする場合は、
// その型の ComVisible 属性を true に設定してください。
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

// 次の GUID は、このプロジェクトが COM に公開される場合の、typelib の ID です
[assembly: Guid("8ba42fd2-e23b-4a6b-a07c-6c33780107a3")]

//[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.Extra")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.Extra.Effect")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.Extra.Effect.Animation")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.Extra.Sound")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.Extra.Xaml")]

#if !MONO
//[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.Extra.Effect")]
//[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.Extra.Effect.Animation")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.Extra.Sound")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.Extra.Xaml")]
#endif

[assembly: XmlnsDefinition("http://schemas.flint-sharp/xaml", "FlintSharp.Behaviours")]
[assembly: XmlnsDefinition("http://schemas.flint-sharp/xaml", "FlintSharp.Activities")]
[assembly: XmlnsDefinition("http://schemas.flint-sharp/xaml", "FlintSharp.Counters")]
[assembly: XmlnsDefinition("http://schemas.flint-sharp/xaml", "FlintSharp.Easing")]
[assembly: XmlnsDefinition("http://schemas.flint-sharp/xaml", "FlintSharp.Emitters")]
[assembly: XmlnsDefinition("http://schemas.flint-sharp/xaml", "FlintSharp.Initializers")]
[assembly: XmlnsDefinition("http://schemas.flint-sharp/xaml", "FlintSharp.Particles")]
[assembly: XmlnsDefinition("http://schemas.flint-sharp/xaml", "FlintSharp.Zones")]

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
