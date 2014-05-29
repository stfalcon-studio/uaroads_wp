#region

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using EveWP8.Extensions;
using Microsoft.Phone.Shell;

#endregion

namespace EveWP8.XAML.Controls.AppBar
{
    public class BindableApplicationBarIconButton : FrameworkElement, IApplicationBarIconButton
    {
        public event EventHandler Click;

        public new static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.RegisterAttached("Visibility", typeof(Visibility),
                typeof(BindableApplicationBarIconButton), new PropertyMetadata(OnVisibilityChanged));

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var button = ((BindableApplicationBarIconButton)d);
                var bar =
                    button.GetVisualAncestors().FirstOrDefault(c => c is BindableApplicationBar) as
                        BindableApplicationBar;
                if (bar != null)
                {
                    bar.Invalidate();
                }
            }
        }

        public new Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command",
            typeof(ICommand),
            typeof(BindableApplicationBarIconButton),
            new PropertyMetadata(CommandChanged));


        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter",
                typeof(object),
                typeof(BindableApplicationBarIconButton),
                new PropertyMetadata(CommandParameterChanged));


        public static readonly DependencyProperty CommandParameterValueProperty =
            DependencyProperty.RegisterAttached("CommandParameterValue",
                typeof(object),
                typeof(BindableApplicationBarMenuItem),
                null);

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled",
            typeof(bool),
            typeof(BindableApplicationBarIconButton),
            new PropertyMetadata(true, OnEnabledChanged));


        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached("Text",
            typeof(string),
            typeof(BindableApplicationBarIconButton),
            new PropertyMetadata(OnTextChanged));

        public static readonly DependencyProperty IconUriProperty = DependencyProperty.RegisterAttached("IconUri",
            typeof(Uri),
            typeof(BindableApplicationBarIconButton),
            new PropertyMetadata(OnIconUriChanged));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public object CommandParameterValue
        {
            get { return GetValue(CommandParameterValueProperty); }
            set { SetValue(CommandParameterValueProperty, value); }
        }

        private static void CommandChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var barIconButton = (BindableApplicationBarIconButton)source;
            var command = e.NewValue as ICommand;
            if (command == null)
            {
                return;
            }
            barIconButton.CommandChanged();

            //barIconButton.IsEnabled = command.CanExecute(barIconButton.CommandParameter);
        }


        private static void CommandParameterChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var barIconButton = (BindableApplicationBarIconButton)source;
            if (barIconButton.Command == null)
            {
                return;
            }
            barIconButton.IsEnabled = barIconButton.Command.CanExecute(e.NewValue);
        }

        private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((BindableApplicationBarIconButton)d).Button.IsEnabled = (bool)e.NewValue;
            }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((BindableApplicationBarIconButton)d).Button.Text = e.NewValue.ToString();
            }
        }

        private static void OnIconUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var newValue = (Uri)e.NewValue;
                ((BindableApplicationBarIconButton)d).Button.IconUri = new Uri(newValue.OriginalString,
                    UriKind.Relative);
            }
        }


        public ApplicationBarIconButton Button { get; set; }

        public BindableApplicationBarIconButton()
        {
            Button = new ApplicationBarIconButton
            {
                Text = "-",
                IconUri = new Uri("/holder.png", UriKind.Relative)
            };
            Button.Click += ApplicationBarIconButtonClick;

            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (Command != null)
            {
                Command.CanExecuteChanged -= CommandOnCanExecuteChanged;
            }
        }

        private void ApplicationBarIconButtonClick(object sender, EventArgs e)
        {
            if (Command != null && CommandParameter != null)
            {
                Command.Execute(CommandParameter);
            }
            else if (Command != null)
            {
                Command.Execute(CommandParameterValue);
            }

            if (Click != null)
            {
                Click(this, e);
            }
        }

        protected void CommandChanged()
        {
            if (Command == null) return;

            IsEnabled = Command.CanExecute(CommandParameter);

            Command.CanExecuteChanged += CommandOnCanExecuteChanged;
        }

        private void CommandOnCanExecuteChanged(object sender, EventArgs eventArgs)
        {
            if (Command == null)
            {
                return;
            }

            IsEnabled = Command.CanExecute(CommandParameter);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Uri IconUri
        {
            get { return (Uri)GetValue(IconUriProperty); }
            set { SetValue(IconUriProperty, value); }
        }
    }
}