#region

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using EveWP8.Extensions;
using Microsoft.Phone.Shell;

#endregion

namespace EveWP8.XAML.Controls.AppBar
{
    public class BindableApplicationBarMenuItem : FrameworkElement, IApplicationBarMenuItem
    {
        public event EventHandler Click;

        public new static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.RegisterAttached("Visibility", typeof(Visibility),
                typeof(BindableApplicationBarMenuItem), new PropertyMetadata(OnVisibilityChanged));

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var button = ((BindableApplicationBarMenuItem)d);
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
            typeof(BindableApplicationBarMenuItem),
            new PropertyMetadata(CommandChanged));


        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter",
                typeof(object),
                typeof(BindableApplicationBarMenuItem),
                new PropertyMetadata(CommandParameterChanged));


        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached("Text",
            typeof(string),
            typeof(BindableApplicationBarMenuItem),
            new PropertyMetadata(OnTextChanged));


        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled",
            typeof(bool),
            typeof(BindableApplicationBarMenuItem),
            new PropertyMetadata(true, OnEnabledChanged));


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

        private static void CommandChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.IsInDesignTool) return;

            var menuItem = (BindableApplicationBarMenuItem)source;

            var oldCommand = e.OldValue as ICommand;

            if (oldCommand != null)
            {
                oldCommand.CanExecuteChanged -= menuItem.CommandCanExecuteChanged;
            }

            var command = e.NewValue as ICommand;
            if (command == null)
            {
                return;
            }
            menuItem.IsEnabled = command.CanExecute(menuItem.CommandParameter);
            command.CanExecuteChanged += menuItem.CommandCanExecuteChanged;
        }

        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            if (this.Command != null)
            {
                this.IsEnabled = this.Command.CanExecute(this.CommandParameter);
            }
        }

        private static void CommandParameterChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var menuItem = (BindableApplicationBarMenuItem)source;
            if (menuItem.Command == null)
            {
                return;
            }
            menuItem.IsEnabled = menuItem.Command.CanExecute(e.NewValue);
        }

        private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((BindableApplicationBarMenuItem)d).MenuItem.IsEnabled = (bool)e.NewValue;
            }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((BindableApplicationBarMenuItem)d).MenuItem.Text = e.NewValue.ToString();
            }
        }

        public ApplicationBarMenuItem MenuItem { get; set; }

        public BindableApplicationBarMenuItem()
        {
            MenuItem = new ApplicationBarMenuItem { Text = "-" };
            MenuItem.Click += ApplicationBarMenuItemClick;
        }

        private void ApplicationBarMenuItemClick(object sender, EventArgs e)
        {
            if (Command != null)
            {
                Command.Execute(CommandParameter);
            }
            if (Click != null)
                Click(this, e);
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
    }
}