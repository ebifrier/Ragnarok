using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.ComponentModel;
using System.Globalization;

namespace Ragnarok.Presentation.Control
{
    /// <summary>
    /// ２つのボタンを持つダイアログです。
    /// </summary>
    /// <remarks>
    /// デフォルトのダイアログでは
    /// 1. 最前面じゃないウィンドウから最前面の表示指定ができない
    /// 2. 位置指定ができない
    /// ため、このようなクラスを作ります。
    /// </remarks>
    [TemplatePart(Type = typeof(Button), Name = "Button1Part")]
    [TemplatePart(Type = typeof(Button), Name = "Button2Part")]
    public partial class GenericDialog : Window
    {
        /// <summary>
        /// ボタン1のコントロール名。
        /// </summary>
        private const string ElementButton1Name = "Button1Part";

        /// <summary>
        /// ボタン2のコントロール名。
        /// </summary>
        private const string ElementButton2Name = "Button2Part";

        private Button button1;
        private Button button2;
        private int focusedButtonNum = -1;

        /// <summary>
        /// メッセージ依存プロパティ
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                "Message", typeof(string),
                typeof(GenericDialog),
                new UIPropertyMetadata(""));

        /// <summary>
        /// 結果的に押されたボタンを識別するための依存プロパティ
        /// </summary>
        public static readonly DependencyProperty ResultButtonProperty =
            DependencyProperty.Register(
                "ResultButton", typeof(MessageBoxResult),
                typeof(GenericDialog),
                new UIPropertyMetadata(MessageBoxResult.None));

        /// <summary>
        /// ボタン1のタイトル依存プロパティ
        /// </summary>
        public static readonly DependencyProperty Button1TextProperty =
            DependencyProperty.Register(
                "Button1Text", typeof(string),
                typeof(GenericDialog),
                new UIPropertyMetadata(""));

        /// <summary>
        /// ボタン1のボタン種別依存プロパティ
        /// </summary>
        public static readonly DependencyProperty Button1KindProperty =
            DependencyProperty.Register(
                "Button1Kind", typeof(MessageBoxResult),
                typeof(GenericDialog),
                new UIPropertyMetadata(MessageBoxResult.None));

        /// <summary>
        /// ボタン2のタイトル依存プロパティ
        /// </summary>
        public static readonly DependencyProperty Button2TextProperty =
            DependencyProperty.Register(
                "Button2Text", typeof(string),
                typeof(GenericDialog),
                new UIPropertyMetadata(""));

        /// <summary>
        /// ボタン2のボタン種別依存プロパティ
        /// </summary>
        public static readonly DependencyProperty Button2KindProperty =
            DependencyProperty.Register(
                "Button2Kind", typeof(MessageBoxResult),
                typeof(GenericDialog),
                new UIPropertyMetadata(MessageBoxResult.None));

        /// <summary>
        /// メッセージを取得または設定します。
        /// </summary>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        /// <summary>
        /// 結果的に押されたボタンを取得します。
        /// </summary>
        public MessageBoxResult ResultButton
        {
            get { return (MessageBoxResult)GetValue(ResultButtonProperty); }
            set { SetValue(ResultButtonProperty, value); }
        }

        /// <summary>
        /// ボタン1のタイトルを取得または設定します。
        /// </summary>
        internal string Button1Text
        {
            get { return (string)GetValue(Button1TextProperty); }
            set { SetValue(Button1TextProperty, value); }
        }

        /// <summary>
        /// ボタン1の種別を取得または設定します。
        /// </summary>
        internal MessageBoxResult Button1Kind
        {
            get { return (MessageBoxResult)GetValue(Button1KindProperty); }
            set { SetValue(Button1KindProperty, value); }
        }

        /// <summary>
        /// ボタン1のタイトルを取得または設定します。
        /// </summary>
        internal string Button2Text
        {
            get { return (string)GetValue(Button2TextProperty); }
            set { SetValue(Button2TextProperty, value); }
        }

        /// <summary>
        /// ボタン1の種別を取得または設定します。
        /// </summary>
        internal MessageBoxResult Button2Kind
        {
            get { return (MessageBoxResult)GetValue(Button2KindProperty); }
            set { SetValue(Button2KindProperty, value); }
        }

        /// <summary>
        /// 指定のボタンにフォーカスを当てます。
        /// </summary>
        internal void SetFocusButton(MessageBoxResult focusedButton)
        {
            if (focusedButton == MessageBoxResult.None)
            {
                return;
            }

            if (Button1Kind == focusedButton)
            {
                this.focusedButtonNum = 1;
            }
            else if (Button2Kind == focusedButton)
            {
                this.focusedButtonNum = 2;
            }
        }

        /// <summary>
        /// 指定のボタンにフォーカスを当てます。
        /// </summary>
        internal void SetFocusButtonNum(int n)
        {
            this.focusedButtonNum = n;
        }

        /// <summary>
        /// テンプレートが変わったときに呼ばれます。
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.button1 != null)
            {
                this.button1.Click -= button1_Click;
            }

            if (this.button2 != null)
            {
                this.button2.Click -= button2_Click;
            }

            this.button1 = GetTemplateChild(ElementButton1Name) as Button;
            this.button2 = GetTemplateChild(ElementButton2Name) as Button;

            if (this.button1 != null)
            {
                this.button1.Click += button1_Click;
            }

            if (this.button2 != null)
            {
                this.button2.Click += button2_Click;
            }
        }

        /// <summary>
        /// アイコンを非表示にします。
        /// </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            Utility.ControlUtil.RemoveIcon(this);
        }

        /// <summary>
        /// ボタン2
        /// </summary>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ResultButton = Button1Kind;
            DialogResult = true;
        }

        /// <summary>
        /// ボタン1
        /// </summary>
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            ResultButton = Button2Kind;
            DialogResult = false;
        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static GenericDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(GenericDialog),
                new FrameworkPropertyMetadata(typeof(GenericDialog)));
        }

        /// <summary>
        /// 指定のボタンが正しく機能するボタンか調べます。
        /// </summary>
        private bool CheckButton(Button button)
        {
            return (button != null && button.Visibility == Visibility.Visible);
        }

        /// <summary>
        /// ボタンのフォーカスを変更します。
        /// </summary>
        private void Dialog_Loaded(object sender, EventArgs e)
        {
            // ボタン番号は表示前に設定されます。
            if (CheckButton(this.button1) &&
                (!CheckButton(this.button2) || this.focusedButtonNum == 1))
            {
                this.button1.Focus();
                return;
            }

            if (CheckButton(this.button2) &&
                (!CheckButton(this.button1) || this.focusedButtonNum == 2))
            {
                this.button2.Focus();
                return;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GenericDialog()
        {
            Loaded += Dialog_Loaded;
        }
    }
}
