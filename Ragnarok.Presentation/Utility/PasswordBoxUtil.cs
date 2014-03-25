using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Ragnarok.Presentation.Utility
{
    /// <summary>
    /// PasswordBoxのPasswordプロパティにバインディングするためのビヘイビアです。
    /// </summary>
    public class PasswordBindingBehavior : Behavior<PasswordBox>
    {
        /// <summary>
        /// パスワードを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                "Password", typeof(string),
                typeof(PasswordBindingBehavior),
                new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        /// <summary>
        /// パスワードを取得または設定します。
        /// </summary>
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        private static void OnPasswordPropertyChanged(DependencyObject d,
                                                      DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as PasswordBindingBehavior;
            if (d == null)
            {
                return;
            }

            var newPassword = e.NewValue as string;
            var oldPassword = behavior.AssociatedObject.Password;
            if (newPassword == oldPassword)
            {
                return;
            }

            behavior.AssociatedObject.Password = newPassword;
        }

        /// <summary>
        /// アタッチ時に呼ばれます。
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += PasswordBox_PasswordChanged;
        }

        /// <summary>
        /// デタッチ時に呼ばれます。
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.PasswordChanged -= PasswordBox_PasswordChanged;
            base.OnDetaching();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = AssociatedObject.Password;
        }
    }
}
