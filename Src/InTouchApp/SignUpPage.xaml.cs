// Decompiled with JetBrains decompiler
// Type: windowsphone_app.SignUp
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Phone.UI.Input;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;

namespace windowsphone_app
{
    public sealed partial class SignUp : Page
    {
        private string Country_Code = string.Empty;
        public List<Entity> _CountryEntity;
        private ServerConnectionManager SCMobj = new ServerConnectionManager();
       

        public SignUp()
        {
            try
            {
                //EasyTracker.GetTracker().SendView("signup");
                this.InitializeComponent();
                this._CountryEntity = Entity.ListOfCountry();
                ((FrameworkElement)this).DataContext = (object)this;
            }
            catch
            {
            }
        }

        public List<Entity> ListofCountry
        {
            get => this._CountryEntity;
            set => this._CountryEntity = value;
        }

        private void HardwareButtons_BackPressed(object sender, EventArgs e)
        {
            try
            {
                if (!(Window.Current.Content is Frame content) || !content.CanGoBack)
                    return;
                e.put_Handled(true);
                content.Navigate(typeof(Welcome));
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

        private async void SignUp_Btn_Click(object sender, RoutedEventArgs e)
        {
            bool shall_Register = true;
            try
            {
                if (!((Control)this.SignUp_Btn).IsEnabled)
                    return;
                
                //EasyTracker.GetTracker().SendEvent("signup", "btn_signup_tapped", "user tapped on signup button", 0L);
                ((Control)this.SignUp_Btn).put_IsEnabled(false);
                RegistrationEssentials registrationEssentials = new RegistrationEssentials();
                if (string.IsNullOrEmpty(this.UserName_TB.Text))
                {
                    ((UIElement)this.UserName_msg).put_Visibility((Visibility)0);
                    this.UserName_msg.put_Text("This field is required.");
                    shall_Register = false;
                }
                else
                    registrationEssentials.name = this.UserName_TB.Text;
                if (string.IsNullOrEmpty(this.Email_TB.Text))
                {
                    ((UIElement)this.Email_msg).put_Visibility((Visibility)0);
                    this.Email_msg.put_Text("This field is required.");
                    shall_Register = false;
                }
                else if (this.Email_TB.Text.Contains("@") && this.Email_TB.Text.Contains("."))
                {
                    registrationEssentials.email = this.Email_TB.Text;
                }
                else
                {
                    ((UIElement)this.Email_msg).put_Visibility((Visibility)0);
                    this.Email_msg.put_Text("Enter a valid email address.");
                    shall_Register = false;
                }
                if (string.IsNullOrEmpty(this.Phone_TB.Text))
                {
                    ((UIElement)this.Phone_msg).put_Visibility((Visibility)0);
                    this.Phone_msg.put_Text("Phone number is required.");
                    shall_Register = false;
                }
                else if (this.Phone_TB.Text.Contains<char>(' '))
                {
                    int length = this.Phone_TB.Text.IndexOf(' ');
                    registrationEssentials.phone_area_code = this.Phone_TB.Text.Substring(0, length);
                    registrationEssentials.phone_number = this.Phone_TB.Text.Substring(length + 1);
                }
                else
                    registrationEssentials.phone_number = this.Phone_TB.Text;
                if (string.IsNullOrEmpty(this.Country_code_TB.Text))
                {
                    ((UIElement)this.Phone_msg).put_Visibility((Visibility)0);
                    if (string.IsNullOrEmpty(this.Phone_msg.Text))
                        this.Phone_msg.Text = "Please select country ";
                    else
                        this.Phone_msg.Text = "Please select country & number";
                    shall_Register = false;
                }
                else
                    registrationEssentials.phone_country_code = this.Country_Code;
                if (string.IsNullOrEmpty(this.Password_PB.Password))
                {
                    ((UIElement)this.Password_msg).put_Visibility((Visibility)0);
                    this.Password_msg.put_Text("This field is required.");
                    shall_Register = false;
                }
                else if (this.Password_PB.Password.Length < 6)
                {
                    ((UIElement)this.Password_msg).put_Visibility((Visibility)0);
                    this.Password_msg.put_Text("Please choose password of at least 6 characters.");
                    shall_Register = false;
                }
                else
                    registrationEssentials.password = this.Password_PB.Password;
                if (string.IsNullOrEmpty(this.InTouchID_TB.Text))
                {
                    ((UIElement)this.InTouchID_msg).put_Visibility((Visibility)0);
                    this.InTouchID_msg.put_Text("Please select your Username.");
                    shall_Register = false;
                }
                else if (this.InTouchID_TB.Text.Length < 8)
                {
                    ((UIElement)this.InTouchID_msg).put_Visibility((Visibility)0);
                    this.InTouchID_msg.put_Text("Username too small (minimum 8 characters).");
                    shall_Register = false;
                }
                else if (this.InTouchID_TB.Text.Length > 19)
                {
                    ((UIElement)this.InTouchID_msg).put_Visibility((Visibility)0);
                    this.InTouchID_msg.put_Text("Maximum allowed length is 19.");
                    shall_Register = false;
                }
                else
                    registrationEssentials.iid = this.InTouchID_TB.Text;
                if (shall_Register)
                {
                    ((Control)this.UserName_TB).put_IsEnabled(false);
                    ((Control)this.Email_TB).put_IsEnabled(false);
                    ((Control)this.Phone_TB).put_IsEnabled(false);
                    ((Control)this.Password_PB).put_IsEnabled(false);
                    ((Control)this.InTouchID_TB).put_IsEnabled(false);
                    ((Control)this.CountryCombo).put_IsEnabled(false);
                    registrationEssentials.dummy = false;
                    ServerConnectionManager SCMObj = new ServerConnectionManager();
                    RegistrationReply Registration_ReplyObj = await SCMObj.registerUser(registrationEssentials);
                    if (string.Equals(Registration_ReplyObj.status, "success", StringComparison.OrdinalIgnoreCase))
                    {
                        string message = "Congratulations!" + Environment.NewLine + "Welcome to the world of worry-free contact management." + Environment.NewLine + "Stay connected, stay in touch!";
                        MessageDialog msgbox = new MessageDialog(message, "Sign Up Successful");
                        msgbox.Commands.Add((IUICommand)new UICommand("Continue"));
                        msgbox.put_DefaultCommandIndex(0U);
                        msgbox.put_CancelCommandIndex(0U);
                        IUICommand iuiCommand = await msgbox.ShowAsync();
                        // ISSUE: reference to a compiler-generated method
                        // ISSUE: reference to a compiler-generated method
                        EasyTracker.GetTracker().SendEvent("registration", "api_call_successful", "user has successfully registered", 0L);
                        await this.Login_Process(this.InTouchID_TB.Text, this.Password_PB.Password);
                    }
                    else
                    {
                        this.InTouchID_TB.put_Text(Registration_ReplyObj.iid);
                        bool flag = false;
                        if (!string.IsNullOrEmpty(Registration_ReplyObj.error_name))
                        {
                            ((UIElement)this.UserName_msg).put_Visibility((Visibility)0);
                            this.UserName_msg.put_Text(Registration_ReplyObj.error_name);
                            flag = true;
                        }
                        if (!string.IsNullOrEmpty(Registration_ReplyObj.error_email))
                        {
                            ((UIElement)this.Email_msg).put_Visibility((Visibility)0);
                            this.Email_msg.put_Text(Registration_ReplyObj.error_email);
                            flag = true;
                        }
                        if (!string.IsNullOrEmpty(Registration_ReplyObj.error_phone))
                        {
                            ((UIElement)this.Phone_msg).put_Visibility((Visibility)0);
                            this.Phone_msg.put_Text(Registration_ReplyObj.error_phone);
                            flag = true;
                        }
                        if (!string.IsNullOrEmpty(Registration_ReplyObj.error_iid))
                        {
                            ((UIElement)this.InTouchID_msg).Visibility = Visibility.Visible;
                            this.InTouchID_msg.Text = Registration_ReplyObj.error_iid;
                            flag = true;
                        }
                        if (!string.IsNullOrEmpty(Registration_ReplyObj.error_password))
                        {
                            ((UIElement)this.Password_msg).Visibility = Visibility.Visible;
                            this.Password_msg.Text = Registration_ReplyObj.error_password;
                            flag = true;
                        }
                        if (!flag && !string.IsNullOrEmpty(Registration_ReplyObj.message))
                        {
                            ((UIElement)this.SignUp_msg).Visibility = Visibility.Visible;
                            this.SignUp_msg.Text = Registration_ReplyObj.message;
                        }
                        
                        //EasyTracker.GetTracker().SendEvent("registration", "api_call_error", "user failed to register", 0L);
                    }
                    ((Control)this.UserName_TB).IsEnabled = true;
                    ((Control)this.Email_TB).IsEnabled = true;
                    ((Control)this.Phone_TB).IsEnabled = true;
                    ((Control)this.Password_PB).IsEnabled = true;
                    ((Control)this.InTouchID_TB).IsEnabled = true;
                    ((Control)this.CountryCombo).IsEnabled = true;
                }
              ((Control)this.SignUp_Btn).IsEnabled = true;
            }
            catch (Exception ex)
            {
                try
                {
                    ((Control)this.SignUp_Btn).IsEnabled = true;
                    ((Control)this.UserName_TB).IsEnabled = true;
                    ((Control)this.Email_TB).IsEnabled = true;
                    ((Control)this.Phone_TB).IsEnabled = true;
                    ((Control)this.Password_PB).IsEnabled = true;
                    ((Control)this.InTouchID_TB).IsEnabled = true;
                    ((Control)this.CountryCombo).IsEnabled = true;
                    LogFile.Log("Problem in sign up. " + ex.Message, EventType.Warning);
                    this.SignUp_msg.Text = "Problem in sign up";
                }
                catch
                {
                }
            }
        }

        public async Task Login_Process(string LoginID, string Password)
        {
            try
            {
                Frame currentFrame = (Frame)Window.Current.Content;
                currentFrame.Navigate(typeof(LoggingIn));
                Tuple<bool, RootObjectToken, string, string, string> result = await this.SCMobj.loginToInTouch(LoginID, Password);
                if (result.Item1)
                {
                    Login.login.Post_Successfull_Login(result.Item2);
                }
                else
                {
                    string str = result.Item3;
                    Frame content = (Frame)Window.Current.Content;
                    currentFrame.Navigate(typeof(Login), (object)str);
                }
            }
            catch (Exception ex)
            {
                string empty = string.Empty;
                ((Frame)Window.Current.Content).Navigate(typeof(Login),
                    (object)(!(ex is NotSupportedException) ? "Login Failed." : ex.Message));
            }
        }

        private void UserName_TB_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ((UIElement)this.UserName_msg).Visibility = Visibility.Collapsed;
            }
            catch
            {
            }
        }

        private void Email_TB_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ((UIElement)this.Email_msg).Visibility = Visibility.Collapsed;
            }
            catch
            {
            }
        }

        private void Phone_TB_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ((UIElement)this.Phone_msg).Visibility = Visibility.Collapsed;
            }
            catch
            {
            }
        }

        private void InTouchID_TB_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ((UIElement)this.InTouchID_msg).Visibility = Visibility.Collapsed;
            }
            catch
            {
            }
        }

        private void Password_PB_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ((UIElement)this.Password_msg).Visibility = Visibility.Collapsed;
            }
            catch
            {
            }
        }

        private void CountryCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //EasyTracker.GetTracker().SendEvent("signup", "combo_country_selected", "user selected country", 0L);
                ((Control)this.Country_code_TB).IsEnabled = true;
                this.Country_Code = string.Empty;
                if (((Selector)this.CountryCombo).SelectedIndex == -1)
                {
                    this.Country_code_TB.Text = string.Empty;
                }
                else
                {
                    Entity selectedItem = (Entity)((Selector)this.CountryCombo).SelectedItem;
                    this.Country_code_TB.Text = "+" + selectedItem.Country_Code;
                    this.Country_Code = selectedItem.Country_Code;
                }
              ((Control)this.Country_code_TB).IsEnabled = false;
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in selecting country combo" + ex.Message, EventType.Warning);
            }
        }

        private async void TermsOfService_Hyperlink_Click(
          Hyperlink sender,
          HyperlinkClickEventArgs args)
        {
            try
            {
                //EasyTracker.GetTracker().SendEvent("signup", "link_terms_of_service_tapped", "user tapped on terms of service link", 0L);
                int num = await Launcher.LaunchUriAsync(new Uri(this.SCMobj.getServerName() + "/termsofservice/", UriKind.Absolute)) ? 1 : 0;
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in opening terms of service link. " + ex.Message, EventType.Warning);
            }
        }

        public static bool isValidEmail(string inputEmail)
        {
            return new Regex("^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$").IsMatch(inputEmail);
        }
       
    }
}

