using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ragnarok.Presentation.Control
{
    /// <summary>
    /// コントロールサイズに自動でフィットするようなAdornerです。
    /// </summary>
    public class CustomAdorner : Adorner
    {
        /// <summary>
        /// 添付プロパティからAdornerを設定するための添付プロパティです。
        /// </summary>
        public static readonly DependencyProperty AttachedProperty =
            DependencyProperty.RegisterAttached(
                "Attached",
                typeof(ControlTemplate), typeof(CustomAdorner),
                new FrameworkPropertyMetadata(null, OnAttachedChanged));

        /// <summary>
        /// 添付プロパティからAdornerを設定します。
        /// </summary>
        public static ControlTemplate GetAttached(DependencyObject obj)
        {
            return (ControlTemplate)obj.GetValue(AttachedProperty);
        }

        /// <summary>
        /// 添付プロパティからAdornerを設定します。
        /// </summary>
        public static void SetAttached(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(AttachedProperty, value);
        }

        /// <summary>
        /// 添付プロパティAttachedの設定時に初期化処理を行います。
        /// </summary>
        private static void OnAttachedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var adorned = d as FrameworkElement;

            Attach(adorned, e.NewValue as ControlTemplate);
        }

        /// <summary>
        /// ソース上からControlTemplateをAdornerを追加します。
        /// </summary>
        public static CustomAdorner Attach(FrameworkElement adorned,
                                           ControlTemplate template)
        {
            var me = new CustomAdorner(adorned)
            {
                Template = template,
            };

            // 装飾層に登録します。
            if (adorned.IsInitialized)
            {
                me.AddToAdornerLayer();
            }
            else
            {
                // 初期化中の場合は登録処理を遅延。
                adorned.Loaded += (_, __) => me.AddToAdornerLayer();
            }

            return me;
        }

        /// <summary>
        /// 装飾層に登録します。
        /// </summary>
        private void AddToAdornerLayer()
        {
            var layer = AdornerLayer.GetAdornerLayer(AdornedElement);
            if (layer == null)
            {
                // デザイン時はエラーになります。
                if (WPFUtil.IsInDesignMode)
                {
                    return;
                }

                throw new InvalidOperationException(
                    "XAML tree must have at least one AdornerDecorator.");
            }

            // 既存の就職を除去します。
            var registered = layer.GetAdorners(AdornedElement);
            if (registered != null)
            {
                registered
                    .OfType<CustomAdorner>()
                    .ForEach(_ => layer.Remove(_));
            }

            // 装飾を新たに登録します。
            layer.Add(this);
        }

        /// <summary>
        /// AdornerのTemplateを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register(
                "Template", typeof(ControlTemplate), typeof(CustomAdorner),
                new FrameworkPropertyMetadata(null, OnTemplateChanged));

        /// <summary>
        /// AdornerのTemplateを取得または設定します。
        /// </summary>
        public ControlTemplate Template
        {
            get { return (ControlTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        /// <summary>
        /// Templateの変更時に呼ばれます。
        /// </summary>
        private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (CustomAdorner)d;

            self.child = new System.Windows.Controls.Control
            {
                Template = e.NewValue as ControlTemplate,
                IsHitTestVisible = false,
                IsTabStop = false,
                Focusable = false,
            };
            self.AddVisualChild(self.child);
            self.AddLogicalChild(self.child);
            self.InvalidateVisual();
        }

        private FrameworkElement child;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CustomAdorner(FrameworkElement adornedElement)
            : base(adornedElement)
        {
        }

        /// <summary>
        /// Visual要素の子供の数を取得します。
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Visual要素の子供を取得します。
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            return this.child;
        }

        /// <summary>
        /// Adornerの所望するサイズを取得します。
        /// </summary>
        private Size AdornerSize
        {
            get
            {
                var adorned = (FrameworkElement)AdornedElement;

                return new Size(
                    Math.Max(0.0, adorned.ActualWidth - 0),
                    Math.Max(0.0, adorned.ActualHeight - 0));
            }
        }

        /// <summary>
        /// オーバーライド
        /// </summary>
        protected override Size MeasureOverride(Size constraint)
        {
            return AdornerSize;
        }

        /// <summary>
        /// オーバーライド
        /// </summary>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.child != null)
            {
                this.child.Arrange(new Rect(AdornerSize));
            }

            return AdornerSize;
        }
    }
}
