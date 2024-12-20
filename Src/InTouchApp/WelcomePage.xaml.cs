// Decompiled with JetBrains decompiler
// Type: windowsphone_app.Welcome
// Assembly: windowsphone_app, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A15F7417-63C2-423F-A22E-030DF791B1B9
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\windowsphone_app.exe

using InTouchLibrary;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
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
using Windows.UI.Xaml.Markup;

namespace windowsphone_app
{
    public sealed partial class Welcome : Page
    {
        private ServerConnectionManager SCMobj = new ServerConnectionManager();

        public Welcome()
        {
            try
            {
                //EasyTracker.GetTracker().SendView("welcome");
                this.InitializeComponent();
            }
            catch
            {
            }

        }//


        private void Signup_Block_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                //EasyTracker.GetTracker().SendEvent("welcome", "textblock_signup_tapped", "user tapped on signup textblock", 0L);
                ((Frame)Window.Current.Content).Navigate(typeof(SignUp));
            }
            catch
            {
            }
        }

        private void Login_Block_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                //EasyTracker.GetTracker().SendEvent("welcome", "textblock_login_tapped", "user tapped on login textblock", 0L);
                ((Frame)Window.Current.Content).Navigate(typeof(Login));
            }
            catch
            {
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //WindowsRuntimeMarshal.AddEventHandler<EventHandler<BackPressedEventArgs>>(new Func<EventHandler<BackPressedEventArgs>, EventRegistrationToken>(HardwareButtons.add_BackPressed), new Action<EventRegistrationToken>(HardwareButtons.remove_BackPressed), new EventHandler<BackPressedEventArgs>(this.HardwareButtons_BackPressed));
            }
            catch
            {
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //WindowsRuntimeMarshal.RemoveEventHandler<EventHandler<BackPressedEventArgs>>(new Action<EventRegistrationToken>(HardwareButtons.remove_BackPressed), new EventHandler<BackPressedEventArgs>(this.HardwareButtons_BackPressed));
            }
            catch
            {
            }
        }

        private void HardwareButtons_BackPressed(object sender, EventArgs e)
        {
            try
            {
                if (!(Window.Current.Content is Frame))
                    return;

                //TODO
                //e.put_Handled(true);
                Application.Current.Exit();
            }
            catch
            {
            }
        }//
    }
}
