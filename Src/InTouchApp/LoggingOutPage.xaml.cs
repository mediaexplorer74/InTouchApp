// Decompiled with JetBrains decompiler
// Type: windowsphone_app.LoggingOut
// Assembly: windowsphone_app, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A15F7417-63C2-423F-A22E-030DF791B1B9
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\windowsphone_app.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//using GoogleAnalytics;
using InTouchLibrary;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;

namespace windowsphone_app
{
    public sealed partial class LoggingOut : Page
    {
        private ServerConnectionManager SCMobj = new ServerConnectionManager();
       

        public LoggingOut()
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendView("loggingOut");
                this.InitializeComponent();
            }
            catch
            {
            }
        }

        private void FeedbackBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //EasyTracker.GetTracker().SendEvent("logging_out", "btn_feedback_tapped", "user tapped on feedback button", 0L);
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            }
            catch
            {
            }
        }

        private async void SendFeedback_Flyout_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //EasyTracker.GetTracker().SendEvent("logging_out", "flyout_btn_sendfeedback_tapped", "user tapped on send feedback flyout button to share feedback", 0L);
                ((FlyoutBase)this.FeedbackFlyout).Hide();
                await Login.login.Feedback();
            }
            catch
            {
            }
        }

        private async void HelpBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //EasyTracker.GetTracker().SendEvent("logging_out", "appbar_btn_help_clicked", "user clicked on help appbar button", 0L);
                int num = await Launcher.LaunchUriAsync(new Uri(this.SCMobj.getServerName() + "/faq/winphone/", UriKind.Absolute)) ? 1 : 0;
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in opening help link. " + ex.Message, EventType.Warning);
            }
        }//
       
    }
}
