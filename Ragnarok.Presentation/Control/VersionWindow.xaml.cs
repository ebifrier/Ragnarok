using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Ragnarok.Utility.AssemblyUtility;

namespace Ragnarok.Presentation.Control
{
    /// <summary>
    /// VersionWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class VersionWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AssemblyAccessorProperty =
            DependencyProperty.Register(
                "AssemblyAccessor",
                typeof(AssemblyAccessor),
                typeof(VersionWindow),
                new UIPropertyMetadata((AssemblyAccessor)null));

        /// <summary>
        /// アセンブリ情報を取得します。
        /// </summary>
        public AssemblyAccessor AssemblyAccessor
        {
            get { return (AssemblyAccessor)GetValue(AssemblyAccessorProperty); }
            private set { SetValue(AssemblyAccessorProperty, value); }
        }
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public VersionWindow(string assemblyName = null)
        {
            InitializeComponent();

            DialogCommands.BindCommands(CommandBindings);

            AssemblyAccessor = new AssemblyAccessor(assemblyName, true);
        }
    }
}
