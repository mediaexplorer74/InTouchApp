// Decompiled with JetBrains decompiler
// Type: windowsphone_app.EditName
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
using Newtonsoft.Json;
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

namespace windowsphone_app
{
    public sealed partial class EditName : Page
    {
       
        public EditName()
        {
            this.InitializeComponent();
           
            //EasyTracker.GetTracker().SendView("editName");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (e.Parameter == null || string.IsNullOrEmpty(e.Parameter.ToString()))
                    return;
                Name name1 = new Name();
                Name name2 = JsonConvert.DeserializeObject<Name>(e.Parameter.ToString());
                if (!string.IsNullOrEmpty(name2.given))
                    this.FirstName_TBox.put_Text(name2.given);
                if (!string.IsNullOrEmpty(name2.family))
                    this.SurName_TBox.put_Text(name2.family);
                if (!string.IsNullOrEmpty(name2.middle))
                    this.MiddleName_TBox.put_Text(name2.middle);
                if (!string.IsNullOrEmpty(name2.nickname))
                    this.NickName_TBox.put_Text(name2.nickname);
                if (!string.IsNullOrEmpty(name2.prefix))
                    this.Title_TBox.put_Text(name2.prefix);
                if (string.IsNullOrEmpty(name2.suffix))
                    return;
                this.Suffix_TBox.put_Text(name2.suffix);
            }
            catch
            {
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //EasyTracker.GetTracker().SendEvent("editName", "appbar_btn_save_clicked", "user clicked on save name", 0L);
                ((Frame)Window.Current.Content).Navigate(typeof(EditContact), (object)new NavigationParameters()
                {
                    action = "Name",
                    data = JsonConvert.SerializeObject((object)new Name()
                    {
                        given = this.FirstName_TBox.Text,
                        family = this.SurName_TBox.Text,
                        middle = this.MiddleName_TBox.Text,
                        nickname = this.NickName_TBox.Text,
                        prefix = this.Title_TBox.Text,
                        suffix = this.Suffix_TBox.Text
                    }, new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })
                });
            }
            catch
            {
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //EasyTracker.GetTracker().SendEvent("editName", "appbar_btn_cancel_clicked", "user clicked on cancel saving name", 0L);
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
