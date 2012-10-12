using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Ragnarok.Presentation.Control
{
    /// <summary>
    /// Enterを押すとバインディングし直すテキストボックスです。
    /// </summary>
    public class BindOnEnterTextBox : TextBox
    {
        /// <summary>
        /// キーが押されたときに呼ばれます。
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Enter)
            {
                var bindingExpression =
                    BindingOperations.GetBindingExpression(
                        this, TextProperty);

                if (bindingExpression != null)
                {
                    bindingExpression.UpdateSource();
                }
            }
        }
    }
}
