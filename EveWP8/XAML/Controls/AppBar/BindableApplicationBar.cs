using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using EveWP8.Extensions;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace EveWP8.XAML.Controls.AppBar
{
    [ContentProperty("Buttons")]
    public class BindableApplicationBar : ItemsControl, IApplicationBar
    {
        public static readonly DependencyProperty IsVisibleProperty;
        public static readonly DependencyProperty IsMenuEnabledProperty;
        public static readonly DependencyProperty BarOpacityProperty;
        public static readonly DependencyProperty BackgroundColorProperty;
        public static readonly DependencyProperty ForegroundColorProperty;
        public static readonly DependencyProperty ModeProperty;

        private readonly ApplicationBar _applicationBar;

        static BindableApplicationBar()
        {
            IsMenuEnabledProperty = DependencyProperty.RegisterAttached("IsMenuEnabled", typeof(bool),
                typeof(BindableApplicationBar), new PropertyMetadata(true, OnEnabledChanged));
            IsVisibleProperty = DependencyProperty.RegisterAttached("IsVisible", typeof(bool),
                typeof(BindableApplicationBar), new PropertyMetadata(true, OnVisibleChanged));

            BarOpacityProperty = DependencyProperty.Register("BarOpacity", typeof(double),
                typeof(BindableApplicationBar), new PropertyMetadata(1.0d, OnBarOpacityChanged));

            BackgroundColorProperty = DependencyProperty.Register("BackgroundColor", typeof(Color),
                typeof(BindableApplicationBar),
                new PropertyMetadata(new Color() { A = 0, B = 0, G = 0, R = 0 }, OnBackgroundColorChanged));

            ForegroundColorProperty = DependencyProperty.Register("ForegroundColor", typeof(Color),
                typeof(BindableApplicationBar),
                new PropertyMetadata(new Color() { A = 0, B = 0, G = 0, R = 0 }, OnForegroundColorChanged));

            ModeProperty = DependencyProperty.Register("Mode", typeof(ApplicationBarMode),
                typeof(BindableApplicationBar),
                new PropertyMetadata(ApplicationBarMode.Default, PropertyChangedCallback));
        }


        //bool _navigated = false;

        //public void frame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        //{
        //	if (!_navigated)
        //	{
        //		_navigated = true;
        //		return;
        //	}

        //	DeviceHelper.GetCurrentApplicationFrame().Navigated -= frame_Navigated;
        //	Uninitialize();
        //}

        //public void SetDataContext(object dataContext)
        //{
        //	DataContext = dataContext;
        //}


        public BindableApplicationBar()
        {
            if (DesignerProperties.IsInDesignTool) return;

            _applicationBar = new ApplicationBar();
            Loaded += ApplicationBarLoaded;
            Unloaded += BindableApplicationBarUnloaded;

            _applicationBar.ForegroundColor = ForegroundColor;
            _applicationBar.BackgroundColor = BackgroundColor;
        }

        public ApplicationBar InternalApplicationBar
        {
            get { return _applicationBar; }
        }

        public double BarOpacity
        {
            get { return (double)GetValue(BarOpacityProperty); }
            set { SetValue(BarOpacityProperty, value); }
        }

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        public bool IsMenuEnabled
        {
            get { return (bool)GetValue(IsMenuEnabledProperty); }
            set { SetValue(IsMenuEnabledProperty, value); }
        }


        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public Color ForegroundColor
        {
            get { return (Color)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        public ApplicationBarMode Mode
        {
            get { return (ApplicationBarMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }


        //public ApplicationBarMode Mode
        //{
        //    get { return _applicationBar.Mode; }
        //    set { _applicationBar.Mode = value; }
        //}

        public double DefaultSize
        {
            get { return _applicationBar.DefaultSize; }
        }

        public double MiniSize
        {
            get { return _applicationBar.MiniSize; }
        }

        public IList Buttons
        {
            get { return Items; }
        }

        public IList MenuItems
        {
            get { return Items; }
        }

        event EventHandler<ApplicationBarStateChangedEventArgs> IApplicationBar.StateChanged
        {
            add { }
            remove { }
        }

        public override void OnApplyTemplate()
        {
            //if (BindableApplicationBar.GetBindableApplicationBar(this) == null)
            //{
            var page = this.FindAncestor(typeof(PhoneApplicationPage)) as PhoneApplicationPage;
            if (page != null)
            {
                page.ApplicationBar = _applicationBar;
            }
            //}
        }

        private void BindableApplicationBarUnloaded(object sender, RoutedEventArgs e)
        {
            Uninitialize();
        }

        private void Uninitialize()
        {
           
        }

        private void Initialize()
        {
           
            Refresh();
        }

        public void Refresh()
        {
            _applicationBar.Opacity = BarOpacity;
            _applicationBar.BackgroundColor = BackgroundColor;
            _applicationBar.ForegroundColor = ForegroundColor;
        }

        private void ApplicationBarLoaded(object sender, RoutedEventArgs e)
        {
            //if (BindableApplicationBar.GetBindableApplicationBar(this) == null)
            //{
            //	var page = this.FindAncestor(typeof (PhoneApplicationPage)) as PhoneApplicationPage;
            //	if (page != null)
            //	{
            //		page.ApplicationBar = _applicationBar;
            //	}
            //}

            Initialize();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (DesignerProperties.IsInDesignTool) return;

            base.OnItemsChanged(e);
            Invalidate();
        }

        public void Invalidate()
        {
            _applicationBar.Buttons.Clear();
            _applicationBar.MenuItems.Clear();

            foreach (
                BindableApplicationBarIconButton button in
                    Items.Where(
                        c =>
                            c is BindableApplicationBarIconButton &&
                            ((BindableApplicationBarIconButton)c).Visibility == Visibility.Visible))
            {
                if (_applicationBar.Buttons.Count == 4) continue;

                _applicationBar.Buttons.Add(button.Button);
            }

            foreach (
                BindableApplicationBarMenuItem button in
                    Items.Where(
                        c =>
                            c is BindableApplicationBarMenuItem &&
                            ((BindableApplicationBarMenuItem)c).Visibility == Visibility.Visible))
            {
                _applicationBar.MenuItems.Add(button.MenuItem);
            }
        }

        private static void OnVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((BindableApplicationBar)d)._applicationBar.IsVisible = (bool)e.NewValue;
            }
        }

        private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((BindableApplicationBar)d)._applicationBar.IsMenuEnabled = (bool)e.NewValue;
            }
        }


        private static void OnBarOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.IsInDesignTool) return;
            ((BindableApplicationBar)d).OnBarOpacityChanged(e);
        }

        private void OnBarOpacityChanged(DependencyPropertyChangedEventArgs e)
        {
            _applicationBar.Opacity = (double)e.NewValue;
        }

        private static void OnBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.IsInDesignTool) return;

            ((BindableApplicationBar)d).OnBackgroundColorChanged(e);
        }

        private void OnBackgroundColorChanged(DependencyPropertyChangedEventArgs e)
        {
            _applicationBar.BackgroundColor = (Color)e.NewValue;
        }


        private static void OnForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.IsInDesignTool) return;
            ((BindableApplicationBar)d).OnForegroundColorChanged(e);
        }

        private void OnForegroundColorChanged(DependencyPropertyChangedEventArgs e)
        {
            _applicationBar.ForegroundColor = (Color)e.NewValue;
        }


        private static void PropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.IsInDesignTool) return;
            ((BindableApplicationBar)obj).OnModeChanged((ApplicationBarMode)e.NewValue);
        }

        private void OnModeChanged(ApplicationBarMode mode)
        {
            _applicationBar.Mode = mode;
        }
    }
}

