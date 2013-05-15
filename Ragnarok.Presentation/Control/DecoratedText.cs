using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace Ragnarok.Presentation.Control
{
    /// <summary>
    /// DecoratedText.xaml の相互作用ロジック
    /// </summary>
    /// <remarks>
    /// <see cref="System.Windows.Controls.ContentControl.Content"/>
    /// は使えないので気をつけてください。
    /// 代わりに<see cref="Text"/>を使ってください。
    /// </remarks>
    [TemplatePart(Type = typeof(ContentControl), Name = "ChildPart")]
    public partial class DecoratedText : DecoratedTextBase
    {
        /// <summary>
        /// 子コントロール名。
        /// </summary>
        private const string ElementChildName = "ChildPart";
        
        private ContentControl child;
        
        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static DecoratedText()
        {
            var depenencyPropertyList = new DependencyProperty[]
            {
                FontFamilyProperty,
                FontStyleProperty,
                FontWeightProperty,
                FontStretchProperty,
                FontSizeProperty,
                StrokeProperty,
                StrokeThicknessProperty,

                FlowDirectionProperty,
                ForegroundProperty,

                MaxWidthProperty,
                MaxHeightProperty,                
            };

            // 依存関係にあるプロパティが変更されたら、
            // コンテンツの内容を更新します。
            foreach (var property in depenencyPropertyList)
            {
                property.OverrideMetadata(
                    typeof(DecoratedText),
                    new FrameworkPropertyMetadata(OnPropertyChanged));
            }

            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DecoratedText),
                new FrameworkPropertyMetadata(typeof(DecoratedText)));
            BackgroundProperty.OverrideMetadata(
                typeof(DecoratedText),
                new FrameworkPropertyMetadata(
                    Brushes.Transparent, OnPropertyChanged));
        }

        /// <summary>
        /// Visualオブジェクトの更新をするかどうかを示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty IsUpdateVisualProperty =
            DependencyProperty.Register(
                "IsUpdateVisual", typeof(bool), typeof(DecoratedText),
                new FrameworkPropertyMetadata(true, OnPropertyChanged));

        /// <summary>
        /// 表示文字列用の依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text", typeof(string), typeof(DecoratedText),
                new FrameworkPropertyMetadata("", OnPropertyChanged,
                    OnCoerceTextCallback));

        /// <summary>
        /// 表示文字列フォーマットの依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty TextFormatProperty =
            DependencyProperty.Register(
                "TextFormat", typeof(string), typeof(DecoratedText),
                new FrameworkPropertyMetadata("", OnPropertyChanged));

        /// <summary>
        /// １行の最大文字数を示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty MaxCharCountProperty =
            DependencyProperty.Register(
                "MaxCharCount", typeof(int?), typeof(DecoratedText),
                new FrameworkPropertyMetadata(null, OnPropertyChanged)); 

        /// <summary>
        /// 最大行数を示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty MaxLineCountProperty =
            DependencyProperty.Register(
                "MaxLineCount", typeof(int), typeof(DecoratedText),
                new FrameworkPropertyMetadata(1, OnPropertyChanged));

        /// <summary>
        /// Visualオブジェクトの更新をするかどうかを取得または設定します。
        /// </summary>
        public bool IsUpdateVisual
        {
            get { return (bool)GetValue(IsUpdateVisualProperty); }
            set { SetValue(IsUpdateVisualProperty, value); }
        }

        /// <summary>
        /// 表示文字列を取得または設定します。
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 表示文字列のフォーマットを取得または設定します。
        /// </summary>
        public string TextFormat
        {
            get { return (string)GetValue(TextFormatProperty); }
            set { SetValue(TextFormatProperty, value); }
        }

        /// <summary>
        /// 1行の最大文字数を取得または設定します。
        /// </summary>
        public int? MaxCharCount
        {
            get { return (int?)GetValue(MaxCharCountProperty); }
            set { SetValue(MaxCharCountProperty, value); }
        }

        /// <summary>
        /// 最大行数を取得または設定します。
        /// </summary>
        public int MaxLineCount
        {
            get { return (int)GetValue(MaxLineCountProperty); }
            set { SetValue(MaxLineCountProperty, value); }
        }

        /// <summary>
        /// 内部で使うFormattedTextオブジェクトを取得します。
        /// </summary>
        public FormattedText FormattedText
        {
            get;
            private set;
        }

        /// <summary>
        /// フォーマット済みの表示用文字列を取得します。
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (string.IsNullOrEmpty(TextFormat))
                {
                    if (string.IsNullOrEmpty(Text))
                    {
                        return "";
                    }

                    return TrimText(Text);
                }

                return TrimText(string.Format(TextFormat, Text));
            }
        }

        /// <summary>
        /// 必要なら指定文字数で切り詰めます。
        /// </summary>
        private string TrimText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (MaxCharCount == null)
            {
                return text;
            }

            if (text.Length > MaxCharCount.Value)
            {
                return text.Substring(0, MaxCharCount.Value) + "...";
            }
            else
            {
                return text;
            }
        }

        /// <summary>
        /// 入力された文字列をチェックします。
        /// </summary>
        private static object OnCoerceTextCallback(DependencyObject d,
                                                   object baseValue)
        {
            return (baseValue as string ?? "");
        }

        /// <summary>
        /// プロパティが変更されたときに、コンテンツを変更します。
        /// </summary>
        private static void OnPropertyChanged(DependencyObject d,
                                              DependencyPropertyChangedEventArgs e)
        {
            var self = (DecoratedText)d;

            if (self.IsUpdateVisual)
            {
                self.UpdateContent();
            }
        }
        
        /// <summary>
        /// テンプレートが変わったときに呼ばれます。
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.child = GetTemplateChild(ElementChildName) as ContentControl;

            UpdateContent();
        }
        
        /// <summary>
        /// 表示文字列を更新します。
        /// </summary>
        private void UpdateContent()
        {
            // 文字列オブジェクトです。
            FormattedText = new FormattedText(
                DisplayText,
                CultureInfo.CurrentUICulture,
                FlowDirection,
                new Typeface(
                    FontFamily,
                    FontStyle,
                    FontWeight,
                    FontStretch),
                FontSize,
                Brushes.Transparent)
            {
                MaxLineCount = MaxLineCount,
                Trimming = TextTrimming.CharacterEllipsis,
            };

            if (!double.IsInfinity(MaxWidth))
            {
                FormattedText.MaxTextWidth = MaxWidth;
            }
            if (!double.IsInfinity(MaxHeight))
            {
                FormattedText.MaxTextHeight = MaxHeight;
            }

            // FormattedTextは事前に設定します。
            if (this.child == null)
            {
                return;
            }

            // 文字オブジェクトの表示用オブジェクトです。
            this.child.Content = new Border()
            {
                Background = Background,
                Child = new Path()
                {
                    Fill = Foreground,
                    Stroke = Stroke,
                    StrokeThickness = StrokeThickness,
                    Stretch = Stretch.None,
                    Data = FormattedText.BuildGeometry(new Point(0, 0)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                },
            };
        }
    }
}
