// Decompiled with JetBrains decompiler
// Type: windowsphone_app.EditEvent
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

using GoogleAnalytics;
using InTouchLibrary;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Navigation;

#nullable disable
namespace windowsphone_app
{
    public sealed partial class EditEvent : Page
    {
       

        public EditEvent()
        {
            this.InitializeComponent();
           
           // EasyTracker.GetTracker().SendView("editEvent");
        }

        protected virtual void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (e.Parameter == null)
                    return;
                string str = e.Parameter.ToString();
                if (str.StartsWith(EventLabel.bday.ToString()))
                    this.PageTitle.put_Text("Edit Birthday");
                else if (str.StartsWith(EventLabel.anniv.ToString()))
                {
                    this.PageTitle.put_Text("Edit Anniversary");
                }
                else
                {
                    NavigationParameters parameter = e.Parameter as NavigationParameters;
                    if (parameter.action.StartsWith(EventLabel.bday.ToString()))
                        this.PageTitle.put_Text("Edit Birthday");
                    else if (parameter.action.StartsWith(EventLabel.anniv.ToString()))
                        this.PageTitle.put_Text("Edit Anniversary");
                    if (string.IsNullOrEmpty(parameter.data))
                        return;
                    DateTime result = new DateTime();
                    if (!DateTime.TryParse(parameter.data, out result))
                        return;
                    this.Event_DatePicker.put_Date((DateTimeOffset)result);
                }
            }
            catch
            {
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               // EasyTracker.GetTracker().SendEvent("editEvent", "appbar_btn_save_clicked", "user clicked on save event", 0L);
                string str = this.Event_DatePicker.Date.Day.ToString() + "-" + this.Event_DatePicker.Date.Month.ToString() + "-" + this.Event_DatePicker.Date.Year.ToString();
                NavigationParameters navigationParameters = new NavigationParameters();
                navigationParameters.data = str;
                Frame content = (Frame)Window.Current.Content;
                if (string.Equals(this.PageTitle.Text, "Edit Birthday", StringComparison.OrdinalIgnoreCase))
                {
                    navigationParameters.action = EventLabel.bday.ToString();
                    content.Navigate(typeof(EditContact), (object)navigationParameters);
                }
                else
                {
                    if (!string.Equals(this.PageTitle.Text, "Edit Anniversary", StringComparison.OrdinalIgnoreCase))
                        return;
                    navigationParameters.action = EventLabel.anniv.ToString();
                    content.Navigate(typeof(EditContact), (object)navigationParameters);
                }
            }
            catch
            {
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               //EasyTracker.GetTracker().SendEvent("editEvent", "appbar_btn_cancel_clicked", "user clicked on cancel saving event", 0L);
                ((Frame)Window.Current.Content).Navigate(typeof(EditContact), (object)"back_pressed");
            }
            catch
            {
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            try
            {
                if (!(Window.Current.Content is Frame content) || !content.CanGoBack)
                    return;
                e.put_Handled(true);
                content.Navigate(typeof(EditContact), (object)"back_pressed");
            }
            catch
            {
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowsRuntimeMarshal.AddEventHandler<EventHandler<BackPressedEventArgs>>(new Func<EventHandler<BackPressedEventArgs>, EventRegistrationToken>(HardwareButtons.add_BackPressed), new Action<EventRegistrationToken>(HardwareButtons.remove_BackPressed), new EventHandler<BackPressedEventArgs>(this.HardwareButtons_BackPressed));
            }
            catch
            {
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowsRuntimeMarshal.RemoveEventHandler<EventHandler<BackPressedEventArgs>>(new Action<EventRegistrationToken>(HardwareButtons.remove_BackPressed), new EventHandler<BackPressedEventArgs>(this.HardwareButtons_BackPressed));
            }
            catch
            {
            }
        }//        
    }
}
