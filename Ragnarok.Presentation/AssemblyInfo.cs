using System.Windows;
using System.Windows.Markup;

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
