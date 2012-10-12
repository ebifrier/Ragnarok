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
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ragnarok.Presentation.Control
{
    /// <summary>
    /// MessageStatusBar.xaml の相互作用ロジック
    /// </summary>
    //[TemplatePart(Type = typeof(TextBox), Name = "ChildItemPart")]
    public partial class MessageStatusBar : StatusBar
    {
        /*/// <summary>
        /// 子要素のコントロール名。
        /// </summary>
        private const string ChildItemName = "ChildItemPart";

        private TextBox textBox;*/

        /// <summary>
        /// ステータスメッセージ用の依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty StatusMessageProperty =
            DependencyProperty.Register(
                "StatusMessage",
                typeof(string),
                typeof(MessageStatusBar),
                new FrameworkPropertyMetadata(string.Empty, OnStatusMessageChanged));

        /// <summary>
        /// ステータスメッセージを取得または設定します。
        /// </summary>
        public string StatusMessage
        {
            get { return (string)GetValue(StatusMessageProperty); }
            set { SetValue(StatusMessageProperty, value); }
        }

        /// <summary>
        /// ステータスメッセージ変更時に呼ばれます。
        /// </summary>
        private static void OnStatusMessageChanged(DependencyObject d,
                                                   DependencyPropertyChangedEventArgs args)
        {
            var self = (MessageStatusBar)d;

            self.childItem.Content = args.NewValue;
        }

        /// <summary>
        /// メッセージの表示色を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty MessageBrushProperty =
            DependencyProperty.Register(
                "MessageBrush",
                typeof(Brush),
                typeof(MessageStatusBar),
                new FrameworkPropertyMetadata(Brushes.Blue));

        /// <summary>
        /// メッセージの表示色を取得または設定します。
        /// </summary>
        public Brush MessageBrush
        {
            get { return (Brush)GetValue(MessageBrushProperty); }
            set { SetValue(MessageBrushProperty, value); }
        }

        /// <summary>
        /// エラーメッセージの表示色を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ErrorMessageBrushProperty =
            DependencyProperty.Register(
                "ErrorMessageBrush",
                typeof(Brush),
                typeof(MessageStatusBar),
                new FrameworkPropertyMetadata(Brushes.Red));

        /// <summary>
        /// エラーメッセージの表示色を取得または設定します。
        /// </summary>
        public Brush ErrorMessageBrush
        {
            get { return (Brush)GetValue(ErrorMessageBrushProperty); }
            set { SetValue(ErrorMessageBrushProperty, value); }
        }

        /// <summary>
        /// メッセージの表示期間を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty MessageDurationProperty =
            DependencyProperty.Register(
                "MessageDuration",
                typeof(TimeSpan),
                typeof(MessageStatusBar),
                new FrameworkPropertyMetadata(TimeSpan.FromSeconds(10)));

        /// <summary>
        /// メッセージの表示期間を取得または設定します。
        /// </summary>
        public TimeSpan MessageDuration
        {
            get { return (TimeSpan)GetValue(MessageDurationProperty); }
            set { SetValue(MessageDurationProperty, value); }
        }

        /// <summary>
        /// ステータスメッセージの表示開始イベントです。
        /// </summary>
        public static readonly RoutedEvent StatusMessageShowEvent =
            EventManager.RegisterRoutedEvent(
                "StatusMessageShow",
                RoutingStrategy.Bubble,
                typeof(RoutedEvent),
                typeof(MessageStatusBar));

        /// <summary>
        /// ステータスメッセージの表示イベントです。
        /// </summary>
        public event RoutedEventHandler StatusMessageShow
        {
            add { AddHandler(StatusMessageShowEvent, value); }
            remove { RemoveHandler(StatusMessageShowEvent, value); }
        }

        /// <summary>
        /// メッセージのフェードアウト処理を行います。
        /// </summary>
        private void StartAnimation()
        {
            var endTime = MessageDuration + TimeSpan.FromSeconds(2);

            // フェードアウトが終わっても、不透明度は0に維持します。
            // 処理中に新しいアニメーションが始まったら、古いアニメーションは
            // 自動的に破棄されます。
            var anim = new DoubleAnimationUsingKeyFrames();
            anim.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, TimeSpan.Zero));
            anim.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, MessageDuration));
            anim.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, endTime));

            BeginAnimation(UIElement.OpacityProperty, anim);
        }

        /// <summary>
        /// メッセージを表示しないようにします。
        /// </summary>
        public void ResetMessage()
        {
            Foreground = Brushes.Transparent;
            StatusMessage = string.Empty;
        }

        /// <summary>
        /// 通常メッセージを設定します。
        /// </summary>
        public void SetMessage(string message)
        {
            Foreground = MessageBrush;
            StatusMessage = message;

            StartAnimation();
        }

        /// <summary>
        /// エラーメッセージを設定します。
        /// </summary>
        public void SetErrorMessage(string message)
        {
            Foreground = ErrorMessageBrush;
            StatusMessage = message;

            StartAnimation();
        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static MessageStatusBar()
        {
            UIElement.OpacityProperty.OverrideMetadata(
                typeof(MessageStatusBar),
                new FrameworkPropertyMetadata(0.0));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MessageStatusBar()
        {
            InitializeComponent();

            // コントロールの高さを設定します。
            StatusMessage = " ";
        }
    }
}
