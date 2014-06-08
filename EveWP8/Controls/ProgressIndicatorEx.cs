using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using EveWP8.Helpers;
using Microsoft.Phone.Shell;

namespace EveWP8.Controls
{
    public class ProgressIndicatorEx : ContentControl
    {
        private Rectangle backgroundRect;
        private StackPanel stackPanel;
        private ProgressBar progressBar;
        private TextBlock textBlockStatus;

        private ProgressTypes progressType;
        private bool currentSystemTrayState;
        private static string defaultText = "Loading...";
        private bool showLabel;
        private string labelText;


        public ProgressIndicatorEx()
        {
            this.DefaultStyleKey = typeof(ProgressIndicatorEx);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.backgroundRect = this.GetTemplateChild("backgroundRect") as Rectangle;
            this.stackPanel = this.GetTemplateChild("stackPanel") as StackPanel;
            this.progressBar = this.GetTemplateChild("progressBar") as ProgressBar;
            this.textBlockStatus = this.GetTemplateChild("textBlockStatus") as TextBlock;

            this.Text = labelText;

            InitializeProgressType();
        }

        internal Popup ChildWindowPopup { get; private set; }


        public ProgressTypes ProgressType
        {
            get { return this.progressType; }
            set { progressType = value; }
        }

        public bool ShowLabel
        {
            get { return this.showLabel; }
            set { this.showLabel = value; }
        }

        public string Text
        {
            get { return labelText; }
            set
            {
                this.labelText = value;
                if (this.textBlockStatus != null)
                {
                    this.textBlockStatus.Text = value;
                }
            }
        }

        public ProgressBar ProgressBar
        {
            get { return this.progressBar; }
        }

        public new double Opacity
        {
            get { return this.backgroundRect.Opacity; }
            set { this.backgroundRect.Opacity = value; }
        }

        public void Hide()
        {
            // Restore system tray
            SystemTray.IsVisible = currentSystemTrayState;

            if (progressBar != null)
                this.progressBar.IsIndeterminate = false;

            if (ChildWindowPopup != null)
                this.ChildWindowPopup.IsOpen = false;


            var page = PageHelper.CurrentPage;
            if (page.ApplicationBar != null)
            {
                page.ApplicationBar.IsVisible = true;
            }
        }

        public void Show()
        {
            if (this.ChildWindowPopup == null)
            {
                this.ChildWindowPopup = new Popup();

                try
                {
                    this.ChildWindowPopup.Child = this;
                }
                catch (ArgumentException)
                {
                    throw new InvalidOperationException("The control is already shown.");
                }
            }


            if (this.ChildWindowPopup != null && Application.Current.RootVisual != null)
            {
                // Configure accordingly to the type
                InitializeProgressType();

                // Show popup
                this.ChildWindowPopup.IsOpen = true;
            }


            var page = PageHelper.CurrentPage;
            if (page.ApplicationBar != null)
            {
                page.ApplicationBar.IsVisible = false;
            }
        }


        private void HideSystemTray()
        {
            // Capture current state of the system tray
            this.currentSystemTrayState = SystemTray.IsVisible;
            // Hide it
            SystemTray.IsVisible = false;
        }

        private void InitializeProgressType()
        {
            this.HideSystemTray();
            if (this.progressBar == null)
                return;

            this.progressBar.Value = 0;


            switch (this.progressType)
            {
                case ProgressTypes.WaitCursor:
                    this.Opacity = 0.7;
                    this.backgroundRect.Visibility = Visibility.Visible;
                    this.stackPanel.VerticalAlignment = VerticalAlignment.Center;
                    this.progressBar.Foreground = (Brush)Application.Current.Resources["PhoneForegroundBrush"];
                    this.textBlockStatus.Text = Text ?? defaultText;
                    this.textBlockStatus.HorizontalAlignment = HorizontalAlignment.Center;
                    this.textBlockStatus.Visibility = Visibility.Visible;
                    this.textBlockStatus.Margin = new Thickness();
                    this.Height = 800;
                    this.progressBar.IsIndeterminate = true;

                    break;
                case ProgressTypes.DeterminateMiddle:
                    this.Opacity = 0.7;
                    this.backgroundRect.Visibility = Visibility.Visible;
                    this.stackPanel.VerticalAlignment = VerticalAlignment.Center;
                    this.progressBar.Foreground = (Brush)Application.Current.Resources["PhoneAccentBrush"];
                    if (showLabel)
                    {
                        this.textBlockStatus.HorizontalAlignment = HorizontalAlignment.Center;
                        this.textBlockStatus.Visibility = Visibility.Visible;
                        this.textBlockStatus.Margin = new Thickness();
                    }
                    else
                    {
                        this.textBlockStatus.Margin = new Thickness();
                        this.textBlockStatus.Visibility = Visibility.Collapsed;
                    }
                    this.Height = 800;
                    break;
                case ProgressTypes.DeterminateTop:
                    this.Opacity = 0.8;
                    this.backgroundRect.Visibility = Visibility.Visible;
                    this.stackPanel.VerticalAlignment = VerticalAlignment.Top;
                    this.progressBar.Foreground = (Brush)Application.Current.Resources["PhoneAccentBrush"];
                    if (showLabel)
                    {
                        this.textBlockStatus.Visibility = Visibility.Visible;
                        this.textBlockStatus.HorizontalAlignment = HorizontalAlignment.Left;
                        this.textBlockStatus.Margin = new Thickness(18, -5, 0, 0);
                        this.Height = 30;
                    }
                    else
                    {
                        this.textBlockStatus.Visibility = Visibility.Collapsed;
                        this.Height = 4;
                    }

                    break;
                case ProgressTypes.IndeterminateTop:
                    this.Opacity = 0.8;
                    this.backgroundRect.Visibility = Visibility.Visible;
                    this.stackPanel.VerticalAlignment = VerticalAlignment.Top;
                    this.progressBar.Foreground = (Brush)Application.Current.Resources["PhoneAccentBrush"];
                    if (showLabel)
                    {
                        this.textBlockStatus.Visibility = Visibility.Visible;
                        this.textBlockStatus.HorizontalAlignment = HorizontalAlignment.Left;
                        this.textBlockStatus.Margin = new Thickness(18, -5, 0, 0);
                        this.Height = 30;
                    }
                    else
                    {
                        this.textBlockStatus.Visibility = Visibility.Collapsed;
                        this.Height = 4;
                    }
                    this.progressBar.IsIndeterminate = true;
                    break;
                case ProgressTypes.CustomTop:
                    this.stackPanel.VerticalAlignment = VerticalAlignment.Center;
                    this.textBlockStatus.Visibility = Visibility.Visible;
                    if (showLabel)
                    {
                        this.textBlockStatus.Visibility = Visibility.Visible;
                        this.textBlockStatus.HorizontalAlignment = HorizontalAlignment.Left;
                        this.textBlockStatus.Margin = new Thickness(18, -5, 0, 0);
                        this.Height = 30;
                    }
                    else
                    {
                        this.textBlockStatus.Visibility = Visibility.Collapsed;
                        this.Height = 4;
                    }
                    break;
                case ProgressTypes.CustomMiddle:
                    this.stackPanel.VerticalAlignment = VerticalAlignment.Center;
                    if (showLabel)
                    {
                        this.textBlockStatus.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.textBlockStatus.Visibility = Visibility.Collapsed;
                    }
                    this.Height = 800;
                    break;
            }
        }
    }

    public enum ProgressTypes
    {
        WaitCursor,
        IndeterminateTop,
        DeterminateTop,
        DeterminateMiddle,
        CustomMiddle,
        CustomTop
    }
}