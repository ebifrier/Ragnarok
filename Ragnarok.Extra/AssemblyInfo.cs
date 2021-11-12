using System.Windows.Markup;

[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.Extra.Effect")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.Extra.Effect.Animation")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.Extra.Sound")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml", "Ragnarok.Extra.Xaml")]

#if !MONO
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.Extra.Effect")]
[assembly: XmlnsDefinition("http://schemas.garnet-alice.net/ragnarok/xaml/presentation", "Ragnarok.Extra.Effect.Animation")]
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
