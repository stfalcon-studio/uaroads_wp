using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using Eve.Core.UI;
using UR.Core.WP81.API;
using UR.Core.WP81.Services;
using UR.WP81.Views;
using Microsoft.ApplicationInsights;
using UR.Core.WP81.Common;
using UR.Core.WP81.Models;

namespace UR.WP81
{
    public sealed partial class App
    {
#if WINDOWS_PHONE_APP
        private TransitionCollection _transitions;
        //private ContinuationManager _continuationManager;
#endif

        private WinRTContainer _container;

        public App()
        {
            //this.RequestedTheme = ApplicationTheme.Light;
            WindowsAppInitializer.InitializeAsync();

            InitializeComponent();


            Current.UnhandledException += CurrentOnUnhandledException;
        }

        private void CurrentOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {

        }

        private SplashScreen _argsSplashScreen;

        //private Frame CreateRootFrame()
        //{
        //    var rootFrame = Window.Current.Content as Frame;

        //    // Do not repeat app initialization when the Window already has content,
        //    // just ensure that the window is active
        //    if (rootFrame == null)
        //    {
        //        // Create a Frame to act as the navigation context and navigate to the first page
        //        rootFrame = new Frame();

        //        // Set the default language
        //        rootFrame.Language = ApplicationLanguages.Languages[0];

        //        // Place the frame in the current Window
        //        Window.Current.Content = rootFrame;
        //    }

        //    return rootFrame;
        //}

        //private async Task RestoreStatusAsync(ApplicationExecutionState previousExecutionState)
        //{
        //    // Do not repeat app initialization when the Window already has content,
        //    // just ensure that the window is active
        //    if (previousExecutionState == ApplicationExecutionState.Terminated)
        //    {
        //        // Restore the saved session state only when appropriate
        //        try
        //        {
        //            await SuspensionManager.RestoreAsync();
        //        }
        //        catch (SuspensionManagerException)
        //        {
        //            // Something went wrong restoring state.
        //            // Assume there is no state and continue
        //        }
        //    }
        //}

        //#if WINDOWS_PHONE_APP
        //        /// <summary>
        //        /// Handle OnActivated event to deal with File Open/Save continuation activation kinds
        //        /// </summary>
        //        /// <param name="e">Application activated event arguments, it can be casted to proper sub-type based on ActivationKind</param>
        //        protected async override void OnActivated(IActivatedEventArgs e)
        //        {
        //            base.OnActivated(e);

        //            _continuationManager = new ContinuationManager();

        //            Frame rootFrame = CreateRootFrame();
        //            await RestoreStatusAsync(e.PreviousExecutionState);

        //            if (rootFrame.Content == null)
        //            {
        //                rootFrame.Navigate(typeof(LoginPage));
        //            }

        //            var continuationEventArgs = e as IContinuationActivatedEventArgs;
        //            if (continuationEventArgs != null)
        //            {
        //                // Call ContinuationManager to handle continuation activation
        //                _continuationManager.Continue(continuationEventArgs, rootFrame);
        //            }

        //            Window.Current.Activate();
        //        }
        //#endif

        protected async override void Configure()
        {
            //show splash screen


            //if (_argsSplashScreen != null)
            //{
            //    var rootFrame = Window.Current.Content as Frame;
            //    if (rootFrame == null)
            //    {
            //        rootFrame = new Frame();
            //        //rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
            //    }

            //    var loadState = false; //(args.PreviousExecutionState == ApplicationExecutionState.Terminated);
            //    var extendedSplash = new SplashScreenPage(_argsSplashScreen, loadState);
            //    rootFrame.Content = extendedSplash;
            //    Window.Current.Content = rootFrame;
            //    //Window.Current.Activate();
            //}


#if DEBUG
            ApiClient.SetLogger(true);
            LogManager.GetLog = GetLog;
#endif


            MessageBinder.SpecialValues.Add("$clickeditem", c => ((ItemClickEventArgs)c.EventArgs).ClickedItem);
            //MessageBinder.SpecialValues.Add(//"$clickeditem", c => ((ItemClickEventArgs)c.EventArgs).ClickedItem);

            EveWindow.Register();

            _container = new WinRTContainer();

            _container.RegisterWinRTServices();

            _container.PerRequest<IDbTracksProvider, AppDatabase>();
            _container.PerRequest<ITrackDataProvider, TrackDataDataProvider>();
            _container.PerRequest<ITracksProvider, TracksProvider>();
            _container.RegisterSingleton(typeof(ILockScreenManager), string.Empty, typeof(LockScreenManager));

            //_container.Singleton<TrackWriter>();

            //_container.RegisterSingleton(typeof(AppGlobalCommandHandler), "AppGlobalCommandHandler", typeof(AppGlobalCommandHandler));


            //register all VM's
            var allViewmodels =
                AssemblySource.Instance.First().DefinedTypes.Where(type => type.Name.EndsWith("ViewModel")).ToList();

            foreach (var vm in allViewmodels)
            {
                if (vm.IsAbstract) continue;
                if (vm.Name.StartsWith("_")) continue;

                //register all VM's
                var type = vm.AsType();

                Debug.WriteLine($"add {type.Name}");
                _container.RegisterPerRequest(type, null, type);
            }

            //_container.PerRequest<SplashScreenPageViewModel>();
            //_container.PerRequest<MainPageViewModel>();

            //_container.PerRequest<TrackListPageViewModel>();
            //_container.PerRequest<LoginPageViewModel>();
            //_container.PerRequest<SettingsPageViewModel>();

            var db = _container.GetInstance<IDbTracksProvider>();

            await db.InitiliaseAsync();

            //_container.PerRequest<IApplicationDataService, ApplicationDataService>();
            //_container.PerRequest<ISessionService, SessionService>();
            //_container.PerRequest<IStorageService, StorageService>();
            ////_container.PerRequest<IMicrosoftService, MicrosoftService>();
            //_container.PerRequest<IGoogleService, GoogleService>();
            //_container.PerRequest<ITwitterService, TwitterService>();
            //_container.PerRequest<IVkontakteService, VkontakteService>();
            //_container.PerRequest<IFacebookService, FacebookService>();
            //_container.PerRequest<INetworkInformationService, NetworkInformationService>();
            ////_container.PerRequest<ILogManager, LogManager>();
            //_container.PerRequest<IEmailComposeService, EmailComposeService>();
            ////_container.PerRequest<INavigationService, NavigationService>();

            //await Task.Delay(2000);

            //SettingsService.UserToken = "124123";

            //var str = SettingsService.UserToken;

            //SettingsService.UserToken = "";
        }

        private ILog GetLog(Type type)
        {
            return new DebugLog(type);
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            _container.RegisterNavigationService(rootFrame);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            //SettingsService.UserToken = null;
            //SettingsService.User = null;

            //var token = SettingsService.UserToken;
            //var user = SettingsService.User;

            //if (args.PreviousExecutionState == ApplicationExecutionState.NotRunning)
            //{
            //    if (token != null)
            //    {
            //        //remove token from store
            //        if (!token.RememberMe)
            //        {
            //            token = null;
            //            user = null;
            //            SettingsService.UserToken = null;
            //            SettingsService.User = null;
            //        }
            //    }
            //}

            //StateService.Instance.UserToken = token;
            //StateService.Instance.User = user;

            //if (args.PreviousExecutionState != ApplicationExecutionState.Running)
            //{
            //    _argsSplashScreen = args.SplashScreen;

            //    //var rootFrame = Window.Current.Content as Frame;


            //    //if (rootFrame == null)
            //    //{
            //    //    rootFrame = new Frame();
            //    //    //rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
            //    //}

            //    //var loadState = (args.PreviousExecutionState == ApplicationExecutionState.Terminated);
            //    //var extendedSplash = new SplashScreenPage(args.SplashScreen, loadState);
            //    //rootFrame.Content = extendedSplash;
            //    //Window.Current.Content = rootFrame;
            //    ////Window.Current.Activate();
            //}
            //else
            //{
            //    //return;
            //}



            //BugSenseHandler.Instance.InitAndStartSession(new ExceptionManager(Current), AppConstants.AppTrackingBugSenceKey);

            //BugSenseHandler.Instance.HandleWhileDebugging = false;


            //BugSenseHandler.Instance.LastActionBeforeTerminate(async () =>
            //{
            //    await MessageDialogExt
            //        .CreateFacade("Произошла ошибка. Мы получим отчет об ошибке и исправим проблему в ближайшее время!")
            //        .WithCommand("ok")
            //        .ShowAsync();
            //    App.Current.Exit();
            //});
            try
            {
                var t = new EasClientDeviceInformation();
                StateService.Instance.IsEmu = t.SystemSku == "Microsoft Virtual";
            }
            catch (Exception)
            {
            }

            // Other specific operations
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
                return;


            //InputPane.GetForCurrentView().Showing += OnShowing;
            //InputPane.GetForCurrentView().Hiding += OnHiding;

            StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);


            //DisplayRootView<SplashScreenPage>();

            DisplayRootView<SplashScreenPage>();
        }



        /*

        protected override void OnSuspending(object sender, SuspendingEventArgs e)
        {
            base.OnSuspending(sender, e);
        }*/

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();


        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            if (rootFrame != null)
            {
                rootFrame.ContentTransitions = _transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
                rootFrame.Navigated -= RootFrame_FirstNavigated;
            }
        }
#endif
    }
}