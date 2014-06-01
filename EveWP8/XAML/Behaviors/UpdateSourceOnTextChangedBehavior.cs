using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace EveWP8.XAML.Behaviors
{
    public class UpdateSourceOnTextChangedBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.TextChanged += this.OnTextChanged;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            BindingExpression be =
                this.AssociatedObject.GetBindingExpression(TextBox.TextProperty);
            if (be != null)
                be.UpdateSource();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.TextChanged -= this.OnTextChanged;
        }
    }
}
