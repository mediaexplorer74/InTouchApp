// Decompiled with JetBrains decompiler
// Type: windowsphone_app.Login
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

using BugSense;
using GoogleAnalytics;
using InTouchLibrary;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Navigation;

namespace windowsphone_app
{
    public sealed partial class Login : Page
    {
        private static volatile Login _login;
        private ServerConnectionManager SCMobj = new ServerConnectionManager();
       

        public static Login login
        {
            get
            {
                try
                {
                    if (Login._login == null)
                        Login._login = new Login();
                    return Login._login;
                }
                catch
                {
                    throw;
                }
            }
        }

        private string action_url { get; set; }

        public Login()
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendView(nameof(login));
                this.InitializeComponent();
                this.put_NavigationCacheMode((NavigationCacheMode)1);
                this.SetAppVersion();
                if (!ServerConnectionManager.mIsDebug)
                    return;
                ((UIElement)this.Choose_Server).put_Visibility((Visibility)0);
            }
            catch
            {
            }
        }

        private void SetAppVersion()
        {
            try
            {
                string appVersion = LocalSettings.localSettings.appVersion;
                if (ServerConnectionManager.mIsDebug)
                    this.AppVersion.put_Text(appVersion + Environment.NewLine + this.SCMobj.getServerNameAPI());
                else
                    this.AppVersion.put_Text(appVersion);
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in getting app version for Login. " + ex.Message, EventType.Warning);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (e.Parameter == null)
                    return;
                this.Login_Failed_msg.put_Text(e.Parameter.ToString());
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
                WindowsRuntimeMarshal.AddEventHandler<EventHandler<BackPressedEventArgs>>(new Func<EventHandler<BackPressedEventArgs>, EventRegistrationToken>(HardwareButtons.add_BackPressed), new Action<EventRegistrationToken>(HardwareButtons.remove_BackPressed), new EventHandler<BackPressedEventArgs>(this.HardwareButtons_BackPressed));
                BugSenseHandler.Instance.RegisterAsyncHandlerContext();
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
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("login", "btn_login_tapped", "user tapped on login button", 0L);
                this.Login_Failed_msg.put_Text(string.Empty);
                if (this.LoginID.Text.Equals("testqwe", StringComparison.OrdinalIgnoreCase) && this.Password.Password.Equals("testqwe"))
                {
                    ((UIElement)this.Choose_Server).put_Visibility((Visibility)0);
                    this.Choose_Server_Click(sender, e);
                    this.Login_Failed_msg.put_Text("You succeed in secret login");
                }
                else if (this.LoginID.Text != "Username or Email" && !string.IsNullOrEmpty(this.LoginID.Text) && this.Password.Password != "Password" && !string.IsNullOrEmpty(this.Password.Password) && this.SCMobj.mIsConnectedToNetwork)
                {
                    Frame currentFrame = (Frame)Window.Current.Content;
                    currentFrame.Navigate(typeof(LoggingIn));
                    Tuple<bool, RootObjectToken, string, string, string> result = await this.SCMobj.loginToInTouch(this.LoginID.Text, this.Password.Password);
                    if (result.Item1)
                    {
                        this.Post_Successfull_Login(result.Item2);
                    }
                    else
                    {
                        this.Clear_All_Fields();
                        string ResponseMessage = result.Item3;
                        this.Login_Failed_msg.put_Text(ResponseMessage);
                        string action = result.Item5;
                        if (!string.IsNullOrEmpty(action) && action.Equals("Upgrade", StringComparison.OrdinalIgnoreCase))
                        {
                            this.action_url = result.Item4;
                            MessageDialog msgbox = new MessageDialog(ResponseMessage, "Upgrade Account");
                            msgbox.Commands.Add((IUICommand)new UICommand("OK"));
                            msgbox.put_DefaultCommandIndex(0U);
                            msgbox.put_CancelCommandIndex(0U);
                            IUICommand iuiCommand = await msgbox.ShowAsync();
                            // ISSUE: reference to a compiler-generated method
                            // ISSUE: reference to a compiler-generated method
                            EasyTracker.GetTracker().SendEvent("login", "messagedialog_btn_upgrade_account_tapped", "user tapped on messagedialog ok button, to upgrade account", 0L);
                            try
                            {
                                int num = await Launcher.LaunchUriAsync(new Uri(this.action_url, UriKind.Absolute)) ? 1 : 0;
                            }
                            catch (Exception ex)
                            {
                                LogFile.Log("Problem in opening upgrade link. " + ex.Message, EventType.Warning);
                            }
                        }
                        Frame currentFrame1 = (Frame)Window.Current.Content;
                        currentFrame1.Navigate(typeof(Login));
                    }
                }
                else
                {
                    this.LoginID_msg.put_Text(this.LoginID.Text.Equals("Username or Email") || string.IsNullOrEmpty(this.LoginID.Text) ? "This field is required" : string.Empty);
                    this.Password_msg.put_Text(this.Password.Password.Equals("Password") || string.IsNullOrEmpty(this.Password.Password) ? "This field is required" : string.Empty);
                    if (this.SCMobj.mIsConnectedToNetwork)
                        return;
                    this.Login_Failed_msg.put_Text("Please check internet connectivity");
                    LogFile.Log("Internet connection is not available.", EventType.Warning);
                }
            }
            catch (Exception ex)
            {
                LogFile.Log("Error in loggin in. " + ex.Message, EventType.Error);
                this.Clear_All_Fields();
                if (string.IsNullOrEmpty(this.Login_Failed_msg.Text))
                    this.Login_Failed_msg.put_Text("Login Failed.");
                ((Frame)Window.Current.Content).Navigate(typeof(Login));
            }
        }

        internal async void Post_Successfull_Login(RootObjectToken rObj)
        {
            try
            {
                LocalSettings.localSettings.MCI = string.Format("InTouchDB" + rObj.mci);
                if (rObj.new_device_connected)
                    await InTouchAppDatabase.InTouchAppDB.deleteDB(LocalSettings.localSettings.MCI);
                try
                {
                    await InTouchAppDatabase.InTouchAppDB.createDBAsync(LocalSettings.localSettings.MCI);
                }
                catch
                {
                    this.Login_Failed_msg.put_Text("Unable to create DB");
                    throw;
                }
                Task SetToken = LocalSettings.localSettings.setToken(rObj.token);
                LocalSettings.localSettings.iid = rObj.iid;
                BugSenseHandler.Instance.UserIdentifier = LocalSettings.localSettings.iid;
                await SetToken;
                if (ContactstoreInTouch.contactStore == null)
                {
                    Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
                    if (result.Item1)
                    {
                        try
                        {
                            await ContactstoreInTouch.contactStore.DeleteAsync();
                            ContactstoreInTouch.contactStoreInTouch.setContactStore();
                        }
                        catch (Exception ex)
                        {
                            LogFile.Log("Error in deleting contact store. " + ex.Message, EventType.Error);
                            ContactstoreInTouch.contactStoreInTouch.setContactStore();
                        }
                    }
                }
                LocalSettings.localSettings.syncOtherContacts = true;
                Frame currentFrame = (Frame)Window.Current.Content;
                currentFrame.Navigate(typeof(Settings));
                LogFile.Log("Login successful.", EventType.Information);
            }
            catch (Exception ex)
            {
                LogFile.Log("Error in post successful login. " + ex.Message, EventType.Error);
                throw;
            }
        }

        private void LoginID_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.LoginID.Text.Equals("Username or Email"))
                    this.LoginID.put_Text(string.Empty);
                this.LoginID_msg.put_Text(string.Empty);
            }
            catch
            {
            }
        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Password.put_Password(string.Empty);
                this.Password_msg.put_Text(string.Empty);
            }
            catch
            {
            }
        }

        public void Clear_All_Fields()
        {
            try
            {
                this.Login_Failed_msg.put_Text(string.Empty);
                this.LoginID_msg.put_Text(string.Empty);
                this.Password_msg.put_Text(string.Empty);
                this.Password.put_Password(string.Empty);
            }
            catch
            {
            }
        }

        private void FeedbackBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("login", "btn_feedback_tapped", "user tapped on feedback button", 0L);
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
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("login", "flyout_btn_sendfeedback_tapped", "user tapped on send feedback flyout button to share feedback", 0L);
                ((FlyoutBase)this.FeedbackFlyout).Hide();
                await this.Feedback();
            }
            catch
            {
            }
        }

        public async Task Feedback()
        {
            try
            {
                EmailMessage mail = new EmailMessage();
                mail.put_Subject("Feedback for InTouchApp WinPhone");
                if (!string.IsNullOrEmpty(this.InTouchID_Text.Text))
                    mail.put_Body("InTouchID: " + this.InTouchID_Text.Text + Environment.NewLine + "App Version: " + this.AppVersion.Text + Environment.NewLine + "Windows OS Version: 8.1" + Environment.NewLine + "Please type your feedback or describe your error here." + Environment.NewLine);
                else
                    mail.put_Body("App Version: " + this.AppVersion.Text + Environment.NewLine + "Windows OS Version: 8.1" + Environment.NewLine + "Please type your feedback or describe your error here." + Environment.NewLine);
                mail.To.Add(new EmailRecipient("support@intouchid.net"));
                bool? isChecked = ((ToggleButton)this.ShallAttachLog_CheckBox).IsChecked;
                if ((!isChecked.GetValueOrDefault() ? 0 : (isChecked.HasValue ? 1 : 0)) != 0)
                {
                    StorageFolder folder = ApplicationData.Current.LocalFolder;
                    StorageFile sampleFile = (StorageFile)null;
                    while (sampleFile == null)
                    {
                        try
                        {
                            sampleFile = await folder.CreateFileAsync("InTouchApp_Log.txt", (CreationCollisionOption)1);
                        }
                        catch
                        {
                        }
                    }
                    try
                    {
                        await FileIO.WriteTextAsync((IStorageFile)sampleFile, LogFile.GetLogForFeedback());
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;
                    }
                    RandomAccessStreamReference stream = RandomAccessStreamReference.CreateFromFile((IStorageFile)sampleFile);
                    EmailAttachment attachment = new EmailAttachment(sampleFile.Name, (IRandomAccessStreamReference)stream);
                    mail.Attachments.Add(attachment);
                }
                await EmailManager.ShowComposeNewEmailAsync(mail);
                this.InTouchID_Text.put_Text(string.Empty);
            }
            catch (Exception ex)
            {
                LogFile.Log("Error in sending email to InTouch Support. " + ex.Message, EventType.Error);
            }
        }

        private async void HelpBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("login", "appbar_btn_help_clicked", "user clicked on help appbar button", 0L);
                int num = await Launcher.LaunchUriAsync(new Uri(this.SCMobj.getServerName() + "/faq/winphone/", UriKind.Absolute)) ? 1 : 0;
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in opening help link. " + ex.Message, EventType.Warning);
            }
        }

        private async void ForgotPassword_Hyperlink_Click_1(
          Hyperlink sender,
          HyperlinkClickEventArgs args)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("login", "link_forgot_password_tapped", "user tapped on forgot password link", 0L);
                int num = await Launcher.LaunchUriAsync(new Uri(this.SCMobj.getServerName() + "/user/password/reset/", UriKind.Absolute)) ? 1 : 0;
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in opening forgot password link. " + ex.Message, EventType.Warning);
            }
        }

        private void Choose_Server_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Local_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LocalSettings.localSettings.serverName = "http://192.168.0.105:8000";
                this.SetAppVersion();
            }
            catch
            {
            }
        }

        private void Test_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LocalSettings.localSettings.serverName = "https://test.intouchapp.com";
                this.SetAppVersion();
            }
            catch
            {
            }
        }

        private void Production_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LocalSettings.localSettings.serverName = "https://api13.intouchapp.com";
                this.SetAppVersion();
            }
            catch
            {
            }
        }

        private void Dev_Choice_Menu_Flyout_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Dev_Choice_BtnClick(object sender, RoutedEventArgs e)
        {
        }

        private void Dev_Server_Flyout_Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void TermsOfService_Hyperlink_Click(
          Hyperlink sender,
          HyperlinkClickEventArgs args)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("login", "link_terms_of_service_tapped", "user tapped on terms of service link", 0L);
                int num = await Launcher.LaunchUriAsync(new Uri(this.SCMobj.getServerName() + "/termsofservice/", UriKind.Absolute)) ? 1 : 0;
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in opening terms of service link. " + ex.Message, EventType.Warning);
            }
        }//       
    }
}
