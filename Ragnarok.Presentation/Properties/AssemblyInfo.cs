using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更してください。
[assembly: AssemblyTitle("Ragnarok.Presentation")]
[assembly: AssemblyDescription("Wpf用の究極ライブラリです。")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("co516151")]
[assembly: AssemblyProduct("Ragnarok.Presentation")]
[assembly: AssemblyCopyright("Copyright © えびふらい 2011-2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// ComVisible を false に設定すると、その型はこのアセンブリ内で COM コンポーネントから 
// 参照不可能になります。COM からこのアセンブリ内の型にアクセスする場合は、
// その型の ComVisible 属性を true に設定してください。
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

// 次の GUID は、このプロジェクトが COM に公開される場合の、typelib の ID です
[assembly: Guid("7a7d2199-818f-4d1a-959e-07baaafc9e70")]

[assembly: ThemeInfo(
    //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.None,

    //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly
)]

[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.Presentation")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.Presentation.Command")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.Presentation.Control")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.Presentation.Converter")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.Presentation.Extension")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.Presentation.Utility")]

// アセンブリのバージョン情報は、以下の 4 つの値で構成されています:
//
//      Major PbProtocolVersion
//      Minor PbProtocolVersion 
//      Build Number
//      Revision
//
// すべての値を指定するか、下のように '*' を使ってビルドおよびリビジョン番号を 
// 既定値にすることができます:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.2.0")]
//[assembly: AssemblyFileVersion("1.2.0")]
