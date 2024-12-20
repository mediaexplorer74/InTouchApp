// App

using System;

using System.IO;
using System.Linq;

using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;

using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;


using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
//using windowsphone_app.InTouchApp_XamlTypeInfo;

using BugSense;
using BugSense.Model;
//using GoogleAnalytics;
using InTouchLibrary;
using Microsoft.ApplicationInsights;


namespace windowsphone_app
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {

        public static TelemetryClient TelemetryClient;
        private TransitionCollection transitions;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            App.TelemetryClient = new TelemetryClient();
            this.InitializeComponent();

            if (ServerConnectionManager.mIsDeveloper)
                BugSenseHandler.Instance.InitAndStartSession(
                    (IExceptionManager)new ExceptionManager(Application.Current), "a40bd5c3");
            else
                BugSenseHandler.Instance.InitAndStartSession(
                    (IExceptionManager)new ExceptionManager(Application.Current), "80f48917");
            BugSenseHandler.Instance.UserIdentifier = LocalSettings.localSettings.iid;
            App app = this;
            
            
            //WindowsRuntimeMarshal.AddEventHandler<SuspendingEventHandler>(
            //new Func<SuspendingEventHandler, EventRegistrationToken>((
            //(Application)app).add_Suspending),
            //new Action<EventRegistrationToken>(((Application)app).remove_Suspending),
            //new SuspendingEventHandler(this.OnSuspending));
            this.Suspending += OnSuspending;
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
        }


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            /*Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;

            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
            */

            try
            {
                if (!(Window.Current.Content is Frame rootFrame))
                {
                    rootFrame = new Frame();
                    rootFrame.CacheSize = 5;
                    ApplicationExecutionState previousExecutionState = e.PreviousExecutionState;
                    Window.Current.Content = rootFrame;
                }

                if (((ContentControl)rootFrame).Content == null)
                {
                    if (((ContentControl)rootFrame).ContentTransitions != null)
                    {
                        this.transitions = new TransitionCollection();

                        foreach ( Transition contentTransition in rootFrame.ContentTransitions )
                            ((ICollection<Transition>)this.transitions).Add(contentTransition);
                    }

                  ((ContentControl)rootFrame).ContentTransitions = (TransitionCollection)null;

                    //RnD
                    //WindowsRuntimeMarshal.AddEventHandler<NavigatedEventHandler>(
                    //    new Func<NavigatedEventHandler, EventRegistrationToken>(rootFrame.add_Navigated), 
                    //    new Action<EventRegistrationToken>(rootFrame.remove_Navigated), new NavigatedEventHandler(this.RootFrame_FirstNavigated));
                    
                    //RnD
                    //StatusBar statusBar = StatusBar.GetForCurrentView();
                    //statusBar.ForegroundColor = new Color?(new Color()
                    //{
                    //    A = byte.MaxValue,
                    //    R = (byte)117,
                    //    G = (byte)117,
                    //    B = (byte)117
                    //});
                    DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
                    if (ServerConnectionManager.mIsDeveloper)
                    {
                        // ISSUE: object of a compiler-generated type is created
                        //EasyTracker.Current.Config = new EasyTrackerConfig()
                        //{
                        //    TrackingId = "UA-20523668-11"
                        //};
                    }
                    else
                    {
                        // ISSUE: object of a compiler-generated type is created
                        //EasyTracker.Current.Config = new EasyTrackerConfig()
                        //{
                        //    TrackingId = "UA-20523668-10"
                        //};
                    }
                    LocalSettings.localSettings.isBackground = false;
                    LocalSettings.localSettings.syncOtherContacts = false;

                    string appVersion = string.Format("Ver: {0}.{1}.{2}.{3}",
                        (object)Package.Current.Id.Version.Major, 
                        (object)Package.Current.Id.Version.Minor, 
                        (object)Package.Current.Id.Version.Build, 
                        (object)Package.Current.Id.Version.Revision);

                    if (!string.Equals(LocalSettings.localSettings.appVersion, appVersion))
                    {
                        LocalSettings.localSettings.appVersion = appVersion;
                        BackgroundExecutionManager.RemoveAccess();
                    }
                    BackgroundAccessStatus status = 
                        await BackgroundExecutionManager.RequestAccessAsync();
                    if (status != BackgroundAccessStatus.Denied)
                    {
                        // ?
                    }

                    if (LocalSettings.localSettings.token == null)
                    {
                        if (!rootFrame.Navigate(typeof(Welcome), (object)e.Arguments))
                            throw new Exception("Failed to create login page");
                    }
                    else if (!rootFrame.Navigate(typeof(Settings), (object)e.Arguments))
                        throw new Exception("Failed to create Settings page");
                }
                Window.Current.Activate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ex] App : " + ex.Message);
            }

        }//


        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            Frame frame1 = sender as Frame;
            Frame frame2 = frame1;
            TransitionCollection transitionCollection1 = this.transitions;
            if (transitionCollection1 == null)
            {
                TransitionCollection transitionCollection2 = new TransitionCollection();

                transitionCollection2.Add((Transition)new NavigationThemeTransition());

                transitionCollection1 = transitionCollection2;
            }
            frame2.ContentTransitions = transitionCollection1;

            //RnD
            //WindowsRuntimeMarshal.RemoveEventHandler<NavigatedEventHandler>(new Action<EventRegistrationToken>(frame1.remove_Navigated), new NavigatedEventHandler(this.RootFrame_FirstNavigated));
        }



        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
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
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
