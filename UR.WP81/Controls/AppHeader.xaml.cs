using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Eve.Core.Extensions;

namespace UR.WP81.Controls
{
    public sealed partial class AppHeader : UserControl
    {
        public static readonly DependencyProperty HeaderProperty = DependencyExtensions.RegisterAttached<string, AppHeader>("Header", string.Empty, Callback);

        private static void Callback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            try
            {
                var thisControl = (AppHeader)obj;

                if (string.IsNullOrEmpty(args.NewValue?.ToString()))
                {
                    thisControl.AppHeaderHeader.Visibility = Visibility.Collapsed;
                    return;
                }

                thisControl.AppHeaderHeader.Visibility = Visibility.Visible;
                thisControl.AppHeaderHeader.Text = args.NewValue.ToString().ToUpperInvariant();
            }
            catch (Exception)
            {
                //todo log
            }

        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }


        //DependencyProperty  DependencyExtensions
        public AppHeader()
        {
            this.InitializeComponent();
        }
    }
}
