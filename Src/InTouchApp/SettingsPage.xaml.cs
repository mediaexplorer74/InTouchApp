// Decompiled with JetBrains decompiler
// Type: windowsphone_app.Settings
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
//using GoogleAnalytics;
using InTouchLibrary;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Networking.PushNotifications;
using Windows.Phone.PersonalInformation;
using Windows.Phone.UI.Input;
using Windows.System;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using windowsphone_app.Common;

namespace windowsphone_app
{
    public sealed partial class Settings : Page
    {
        private const string Sample_Task_Entry_Point = "BackgroundClass.BackgroundClass";
        private readonly NavigationHelper _navigationHelper;
        private readonly ObservableDictionary _defaultViewModel;
        private static volatile Settings _settings;
        private BackgroundTaskRegistration task;
        private bool eventAdded;
        public PushNotificationChannel channel;
        private ServerConnectionManager SCMobj;
        private static TextBlock Sync_Status_Block;
        private static TextBlock Last_Sync_Time_Block;
        private static TextBlock Contacts_Managed_Block;
        private static TextBlock Image_Status_Block;
        private static ProgressRing Sync_ProgressRing;
        private static ToggleSwitch DownloadPhotoOnWiFi_TS;
        private static TextBlock DontCloseApp_Block;
        private static TextBlock Backup_Phone_Contacts_Block;
        private TextBlock ContactID_Block;
        private TextBlock AccountType_Block;
        private TextBlock AccountType_Header_Block;
        private TextBlock Upgrade_Block;
        private TextBlock SyncSession_ID_Block;
        private TextBlock SyncSession_Owner_Block;
        private TextBlock SyncSession_State_Block;
        private TextBlock SyncSession_LastUpdateTime_Block;
        private TextBlock ModifiedContactsCount_Block;
        private TextBlock StoreRevisionNumber_Block;
        private TextBlock DBDirtyIntouchContactsCount_Block;
        private TextBlock AppVersion_Block;
        private TextBlock Support_Block;
        private TextBox Feedback_Message_Text;
        private CheckBox ShallAttachLog_CheckBox;
        private TextBox SearchTextBox;
        private TextBlock NoResult;
        private TextBlock textBlock1;
        private TextBlock contact_display_name;
        private TextBlock contact_organization_name;
        private TextBlock RestoringContacts;
        private ListView ContactListView;
        private Flyout FeedbackFlyout;
        private TextBlock AppMemoryUsage;
        private TextBlock AppMemoryUsageLimit;
        private TextBlock AppMemoryUsageLevel;
        private CommandBar contactListCommandBar;
        private CommandBar syncCommandBar;
        private CommandBar settingsCommandBar;
        private CommandBar aboutCommandBar;
        private static AppBarButton _SyncBtn;
        private static AppBarButton _LogoutBtn;
        private AppBarButton SyncBtn;
        private AppBarButton ShareBtn;
        private AppBarButton ShareBtn1;
        private AppBarButton ShareBtn2;
        private AppBarButton ShareBtn3;
        private AppBarButton SearchBtn;
        private AppBarButton MultipleSelectBtn;
        private AppBarButton AddBtn;
        private AppBarButton MultipleDeleteBtn;
        private AppBarButton LogoutBtn;
        private string prevText;
        private int offset;
        private bool incall;
        private bool endOfList;
        private bool layoutUpdateFlag;
        private string displayName;
        private string shareText;
        private static CoreDispatcher dispatcher;
        private static TimeSpan period = TimeSpan.FromSeconds(5.0);
        private ThreadPoolTimer PeriodicTimer;
       

        public NavigationHelper navigationHelper => this._navigationHelper;

        public ObservableDictionary defaultViewModel => this._defaultViewModel;

        public static Settings settings
        {
            get
            {
                try
                {
                    if (Settings._settings == null)
                        Settings._settings = new Settings();
                    return Settings._settings;
                }
                catch
                {
                    throw;
                }
            }
        }

        public static BackgroundTaskRegistration registerBackgroundTask(
          string taskEntryPoint,
          string name,
          IBackgroundTrigger trigger)
        {
            try
            {
                foreach (KeyValuePair<Guid, IBackgroundTaskRegistration> allTask in (IEnumerable<KeyValuePair<Guid, IBackgroundTaskRegistration>>)BackgroundTaskRegistration.AllTasks)
                {
                    if (allTask.Value.Name == name)
                        return (BackgroundTaskRegistration)allTask.Value;
                }
                BackgroundTaskBuilder backgroundTaskBuilder = new BackgroundTaskBuilder();
                backgroundTaskBuilder.put_Name(name);
                backgroundTaskBuilder.put_TaskEntryPoint(taskEntryPoint);
                backgroundTaskBuilder.SetTrigger(trigger);
                return backgroundTaskBuilder.Register();
            }
            catch
            {
                throw;
            }
        }

        public Settings()
        {
            CommandBar commandBar1 = new CommandBar();
            ((AppBar)commandBar1).put_IsOpen(true);
            this.contactListCommandBar = commandBar1;
            CommandBar commandBar2 = new CommandBar();
            ((AppBar)commandBar2).put_IsOpen(true);
            this.syncCommandBar = commandBar2;
            CommandBar commandBar3 = new CommandBar();
            ((AppBar)commandBar3).put_IsOpen(true);
            this.settingsCommandBar = commandBar3;
            CommandBar commandBar4 = new CommandBar();
            ((AppBar)commandBar4).put_IsOpen(true);
            this.aboutCommandBar = commandBar4;
            AppBarButton appBarButton1 = new AppBarButton();
            appBarButton1.put_Icon((IconElement)new SymbolIcon((Symbol)57623));
            appBarButton1.put_Label("Sync");
            this.SyncBtn = appBarButton1;
            AppBarButton appBarButton2 = new AppBarButton();
            appBarButton2.put_Icon((IconElement)new SymbolIcon((Symbol)57802));
            appBarButton2.put_Label("Share app");
            this.ShareBtn = appBarButton2;
            AppBarButton appBarButton3 = new AppBarButton();
            appBarButton3.put_Icon((IconElement)new SymbolIcon((Symbol)57802));
            appBarButton3.put_Label("Share app");
            this.ShareBtn1 = appBarButton3;
            AppBarButton appBarButton4 = new AppBarButton();
            appBarButton4.put_Icon((IconElement)new SymbolIcon((Symbol)57802));
            appBarButton4.put_Label("Share app");
            this.ShareBtn2 = appBarButton4;
            AppBarButton appBarButton5 = new AppBarButton();
            appBarButton5.put_Icon((IconElement)new SymbolIcon((Symbol)57802));
            appBarButton5.put_Label("Share app");
            this.ShareBtn3 = appBarButton5;
            AppBarButton appBarButton6 = new AppBarButton();
            appBarButton6.put_Icon((IconElement)new SymbolIcon((Symbol)57626));
            appBarButton6.put_Label("Find Contact");
            this.SearchBtn = appBarButton6;
            AppBarButton appBarButton7 = new AppBarButton();
            appBarButton7.put_Icon((IconElement)new SymbolIcon((Symbol)57721));
            appBarButton7.put_Label("Select");
            this.MultipleSelectBtn = appBarButton7;
            AppBarButton appBarButton8 = new AppBarButton();
            appBarButton8.put_Icon((IconElement)new SymbolIcon((Symbol)57609));
            appBarButton8.put_Label("Add Contact");
            this.AddBtn = appBarButton8;
            AppBarButton appBarButton9 = new AppBarButton();
            appBarButton9.put_Icon((IconElement)new SymbolIcon((Symbol)57607));
            appBarButton9.put_Label("Delete");
            ((UIElement)appBarButton9).put_Visibility((Visibility)1);
            this.MultipleDeleteBtn = appBarButton9;
            AppBarButton appBarButton10 = new AppBarButton();
            appBarButton10.put_Label("Logout");
            this.LogoutBtn = appBarButton10;
            this.layoutUpdateFlag = true;
            this.displayName = string.Empty;
            this.shareText = string.Empty;
            // ISSUE: reference to a compiler-generated field
            if (Settings.CS\u0024\u003C\u003E9__CachedAnonymousMethodDelegate10 == null)
      {
                // ISSUE: reference to a compiler-generated field
                // ISSUE: method pointer
                Settings.CS\u0024\u003C\u003E9__CachedAnonymousMethodDelegate10 = new TimerElapsedHandler((object)null, __methodptr(\u003C\u002Ector\u003Eb__e));
            }
            // ISSUE: reference to a compiler-generated field
            this.PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(Settings.CS\u0024\u003C\u003E9__CachedAnonymousMethodDelegate10, Settings.period);
            // ISSUE: explicit constructor call
            base.\u002Ector();
            try
            {
                this.InitializeComponent();
                this.prepareAppBars();
                this.put_NavigationCacheMode((NavigationCacheMode)1);
                this._navigationHelper = new NavigationHelper((Page)this);
                this._navigationHelper.LoadState += new LoadStateEventHandler(this.NavigationHelper_LoadState);
                this._navigationHelper.SaveState += new SaveStateEventHandler(this.NavigationHelper_SaveState);
                ((FrameworkElement)this).put_DataContext((object)this);
                this.openChannelAndRegisterTask();
            }
            catch
            {
            }
        }

        private void prepareAppBars()
        {
            try
            {
                AppBarButton searchBtn = this.SearchBtn;
                WindowsRuntimeMarshal.AddEventHandler<RoutedEventHandler>(new Func<RoutedEventHandler, EventRegistrationToken>(((ButtonBase)searchBtn).add_Click), new Action<EventRegistrationToken>(((ButtonBase)searchBtn).remove_Click), new RoutedEventHandler(this.SearchBtn_Click));
                ((ICollection<ICommandBarElement>)this.contactListCommandBar.PrimaryCommands).Add((ICommandBarElement)this.SearchBtn);
                AppBarButton multipleSelectBtn = this.MultipleSelectBtn;
                WindowsRuntimeMarshal.AddEventHandler<RoutedEventHandler>(new Func<RoutedEventHandler, EventRegistrationToken>(((ButtonBase)multipleSelectBtn).add_Click), new Action<EventRegistrationToken>(((ButtonBase)multipleSelectBtn).remove_Click), new RoutedEventHandler(this.MultipleSelectBtn_Click));
                ((ICollection<ICommandBarElement>)this.contactListCommandBar.PrimaryCommands).Add((ICommandBarElement)this.MultipleSelectBtn);
                AppBarButton addBtn = this.AddBtn;
                WindowsRuntimeMarshal.AddEventHandler<RoutedEventHandler>(new Func<RoutedEventHandler, EventRegistrationToken>(((ButtonBase)addBtn).add_Click), new Action<EventRegistrationToken>(((ButtonBase)addBtn).remove_Click), new RoutedEventHandler(this.AddBtn_Click));
                ((ICollection<ICommandBarElement>)this.contactListCommandBar.PrimaryCommands).Add((ICommandBarElement)this.AddBtn);
                AppBarButton multipleDeleteBtn = this.MultipleDeleteBtn;
                WindowsRuntimeMarshal.AddEventHandler<RoutedEventHandler>(new Func<RoutedEventHandler, EventRegistrationToken>(((ButtonBase)multipleDeleteBtn).add_Click), new Action<EventRegistrationToken>(((ButtonBase)multipleDeleteBtn).remove_Click), new RoutedEventHandler(this.MultipleDeleteBtn_Click));
                ((ICollection<ICommandBarElement>)this.contactListCommandBar.PrimaryCommands).Add((ICommandBarElement)this.MultipleDeleteBtn);
                AppBarButton shareBtn = this.ShareBtn;
                WindowsRuntimeMarshal.AddEventHandler<RoutedEventHandler>(new Func<RoutedEventHandler, EventRegistrationToken>(((ButtonBase)shareBtn).add_Click), new Action<EventRegistrationToken>(((ButtonBase)shareBtn).remove_Click), new RoutedEventHandler(this.ShareBtn_Click));
                ((ICollection<ICommandBarElement>)this.contactListCommandBar.SecondaryCommands).Add((ICommandBarElement)this.ShareBtn);
                AppBarButton syncBtn = this.SyncBtn;
                WindowsRuntimeMarshal.AddEventHandler<RoutedEventHandler>(new Func<RoutedEventHandler, EventRegistrationToken>(((ButtonBase)syncBtn).add_Click), new Action<EventRegistrationToken>(((ButtonBase)syncBtn).remove_Click), new RoutedEventHandler(this.SyncBtn_Click));
                ((ICollection<ICommandBarElement>)this.syncCommandBar.PrimaryCommands).Add((ICommandBarElement)this.SyncBtn);
                Settings._SyncBtn = this.SyncBtn;
                AppBarButton shareBtn1 = this.ShareBtn1;
                WindowsRuntimeMarshal.AddEventHandler<RoutedEventHandler>(new Func<RoutedEventHandler, EventRegistrationToken>(((ButtonBase)shareBtn1).add_Click), new Action<EventRegistrationToken>(((ButtonBase)shareBtn1).remove_Click), new RoutedEventHandler(this.ShareBtn_Click));
                ((ICollection<ICommandBarElement>)this.syncCommandBar.SecondaryCommands).Add((ICommandBarElement)this.ShareBtn1);
                AppBarButton logoutBtn = this.LogoutBtn;
                WindowsRuntimeMarshal.AddEventHandler<RoutedEventHandler>(new Func<RoutedEventHandler, EventRegistrationToken>(((ButtonBase)logoutBtn).add_Click), new Action<EventRegistrationToken>(((ButtonBase)logoutBtn).remove_Click), new RoutedEventHandler(this.LogoutBtn_Click));
                ((ICollection<ICommandBarElement>)this.settingsCommandBar.SecondaryCommands).Add((ICommandBarElement)this.LogoutBtn);
                Settings._LogoutBtn = this.LogoutBtn;
                AppBarButton shareBtn2 = this.ShareBtn2;
                WindowsRuntimeMarshal.AddEventHandler<RoutedEventHandler>(new Func<RoutedEventHandler, EventRegistrationToken>(((ButtonBase)shareBtn2).add_Click), new Action<EventRegistrationToken>(((ButtonBase)shareBtn2).remove_Click), new RoutedEventHandler(this.ShareBtn_Click));
                ((ICollection<ICommandBarElement>)this.settingsCommandBar.SecondaryCommands).Add((ICommandBarElement)this.ShareBtn2);
                AppBarButton shareBtn3 = this.ShareBtn3;
                WindowsRuntimeMarshal.AddEventHandler<RoutedEventHandler>(new Func<RoutedEventHandler, EventRegistrationToken>(((ButtonBase)shareBtn3).add_Click), new Action<EventRegistrationToken>(((ButtonBase)shareBtn3).remove_Click), new RoutedEventHandler(this.ShareBtn_Click));
                ((ICollection<ICommandBarElement>)this.aboutCommandBar.SecondaryCommands).Add((ICommandBarElement)this.ShareBtn3);
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in preparing app bars. " + ex.Message, EventType.Error);
            }
        }

        public async Task openChannelAndRegisterTask()
        {
            try
            {
                this.channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                if (this.channel != null)
                {
                    if (!string.Equals(this.channel.Uri, LocalSettings.localSettings.channelUri) || DateTime.Compare(this.channel.ExpirationTime.DateTime, DateTime.Now) <= 0)
                        await this.SCMobj.pushMessaging(await LocalSettings.localSettings.getToken(), this.channel.Uri);
                    this.updateListener(true);
                    PushNotificationTrigger trigger = new PushNotificationTrigger();
                    Settings.registerBackgroundTask("BackgroundClass.BackgroundClass", "PushNotification", (IBackgroundTrigger)trigger);
                }
                else
                    LocalSettings.localSettings.channelUri = string.Empty;
            }
            catch (Exception ex)
            {
                LogFile.Log("Error in creating channel. " + ex.Message, EventType.Error);
            }
        }

        private async void OnPushNotificationReceived(
          PushNotificationChannel sender,
          PushNotificationReceivedEventArgs e)
        {
            try
            {
                string notificationContent = string.Empty;
                LogFile.Log("NotificationType is " + (object)e.NotificationType, EventType.Test);
                switch ((int)e.NotificationType)
                {
                    case 0:
                        notificationContent = e.ToastNotification.Content.InnerText;
                        break;
                    case 1:
                        notificationContent = e.TileNotification.Content.InnerText;
                        break;
                    case 2:
                        notificationContent = e.BadgeNotification.Content.InnerText;
                        break;
                    case 3:
                        notificationContent = e.RawNotification.Content;
                        break;
                }
                LogFile.Log("Notification content is " + notificationContent, EventType.Test);
                e.put_Cancel(true);
                PushMessage pushMessage = new PushMessage();
                await pushMessage.processPushMessage(notificationContent);
            }
            catch
            {
            }
        }

        private bool updateListener(bool add)
        {
            try
            {
                if (this.channel == null)
                    return false;
                if (add && !this.eventAdded)
                {
                    PushNotificationChannel channel = this.channel;
                    // ISSUE: method pointer
                    WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<PushNotificationChannel, PushNotificationReceivedEventArgs>>(new Func<TypedEventHandler<PushNotificationChannel, PushNotificationReceivedEventArgs>, EventRegistrationToken>(channel.add_PushNotificationReceived), new Action<EventRegistrationToken>(channel.remove_PushNotificationReceived), new TypedEventHandler<PushNotificationChannel, PushNotificationReceivedEventArgs>((object)this, __methodptr(OnPushNotificationReceived)));
                    this.eventAdded = true;
                }
                else if (!add && this.eventAdded)
                {
                    // ISSUE: method pointer
                    WindowsRuntimeMarshal.RemoveEventHandler<TypedEventHandler<PushNotificationChannel, PushNotificationReceivedEventArgs>>(new Action<EventRegistrationToken>(this.channel.remove_PushNotificationReceived), new TypedEventHandler<PushNotificationChannel, PushNotificationReceivedEventArgs>((object)this, __methodptr(OnPushNotificationReceived)));
                    this.eventAdded = false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected virtual void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                DataTransferManager forCurrentView = DataTransferManager.GetForCurrentView();
                // ISSUE: method pointer
                WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<DataTransferManager, DataRequestedEventArgs>>(new Func<TypedEventHandler<DataTransferManager, DataRequestedEventArgs>, EventRegistrationToken>(forCurrentView.add_DataRequested), new Action<EventRegistrationToken>(forCurrentView.remove_DataRequested), new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>((object)this, __methodptr(shareTextHandler)));
                this.updateListener(true);
                if (e.Parameter == null || string.IsNullOrEmpty(e.Parameter.ToString()))
                    return;
                if (string.Equals("back_pressed", e.Parameter.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    this._navigationHelper.OnNavigatedTo(e);
                }
                else
                {
                    if (!string.Equals("edited", e.Parameter.ToString(), StringComparison.OrdinalIgnoreCase))
                        return;
                    if (!Sync.isSyncRunning)
                        this.startSettingsSync(false);
                    else
                        Sync.setContactsManagedBlock();
                }
            }
            catch
            {
            }
        }

        protected virtual void OnNavigatedFrom(NavigationEventArgs e)
        {
            try
            {
                this.updateListener(false);
                if (e.Parameter == null || string.IsNullOrEmpty(e.Parameter.ToString()))
                    return;
                this._navigationHelper.OnNavigatedFrom(e);
            }
            catch
            {
            }
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            try
            {
                if (e.PageState != null)
                {
                    this.defaultViewModel["Followers"] = (object)(List<ContactSample>)e.PageState["Followers"];
                    ((ListViewBase)this.ContactListView).ScrollIntoView((object)((ItemsControl)this.ContactListView).ContainerFromIndex((int)e.PageState["FirstVisibleItemIndex"]));
                }
                else
                    this.defaultViewModel["Followers"] = (object)new List<ContactSample>();
            }
            catch
            {
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            try
            {
                int firstVisibleIndex = ((ItemsStackPanel)((ItemsControl)this.ContactListView).ItemsPanelRoot).FirstVisibleIndex;
                e.PageState["FirstVisibleItemIndex"] = (object)firstVisibleIndex;
                e.PageState["Followers"] = this.defaultViewModel["Followers"];
            }
            catch
            {
            }
        }

        private void Sync_Staus_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.Sync_Status_Block = (TextBlock)sender;
                if (LocalSettings.localSettings.syncSessionState <= 0 || string.Equals(LocalSettings.localSettings.syncSessionOwner, SyncStateOwner.App.ToString()))
                    LocalSettings.localSettings.syncStatusBlock = string.Empty;
                Sync.syncStatusBlock = Settings.Sync_Status_Block;
            }
            catch
            {
            }
        }

        private void Last_Sync_Time_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.Last_Sync_Time_Block = (TextBlock)sender;
                if (!string.IsNullOrEmpty(LocalSettings.localSettings.lastSyncTime))
                    Settings.Last_Sync_Time_Block.put_Text("Last Sync: " + LocalSettings.localSettings.lastSyncTime);
                Sync.lastSyncTimeBlock = Settings.Last_Sync_Time_Block;
            }
            catch
            {
            }
        }

        private void Contacts_Managed_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.Contacts_Managed_Block = (TextBlock)sender;
                string str = "Contacts Managed : " + (object)InTouchAppDatabase.InTouchAppDB.countInTouchEntriesFromLT(LocalSettings.localSettings.MCI);
                LocalSettings.localSettings.contactsManagedBlock = str;
                Settings.Contacts_Managed_Block.put_Text(str);
                Sync.contactsManagedBlock = Settings.Contacts_Managed_Block;
            }
            catch
            {
            }
        }

        private void Image_Staus_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.Image_Status_Block = (TextBlock)sender;
                if (LocalSettings.localSettings.syncSessionState <= 0 || string.Equals(LocalSettings.localSettings.syncSessionOwner, SyncStateOwner.App.ToString()))
                    LocalSettings.localSettings.imageStatusBlock = string.Empty;
                Sync.imagesStatusBlock = Settings.Image_Status_Block;
            }
            catch
            {
            }
        }

        private void ContactID_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ContactID_Block = (TextBlock)sender;
                this.ContactID_Block.put_Text("*" + LocalSettings.localSettings.iid);
            }
            catch
            {
            }
        }

        private void AccountType_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.AccountType_Block = (TextBlock)sender;
            }
            catch
            {
            }
        }

        private void Upgrade_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Upgrade_Block = (TextBlock)sender;
            }
            catch
            {
            }
        }

        private void AccountType_Header_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.AccountType_Header_Block = (TextBlock)sender;
                this.getAccountType();
            }
            catch
            {
            }
        }

        private void ModifiedContactsCount_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ModifiedContactsCount_Block = (TextBlock)sender;
            }
            catch
            {
            }
        }

        private void DBDirtyIntouchContactsCount_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.DBDirtyIntouchContactsCount_Block = (TextBlock)sender;
            }
            catch
            {
            }
        }

        private void SyncSession_Owner_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.SyncSession_Owner_Block = (TextBlock)sender;
            }
            catch
            {
            }
        }

        private void SyncSession_ID_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.SyncSession_ID_Block = (TextBlock)sender;
            }
            catch
            {
            }
        }

        private void SyncSession_State_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.SyncSession_State_Block = (TextBlock)sender;
            }
            catch
            {
            }
        }

        private void SyncSession_LastUpdateTime_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.SyncSession_LastUpdateTime_Block = (TextBlock)sender;
            }
            catch
            {
            }
        }

        private void StoreRevisionNumber_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.StoreRevisionNumber_Block = (TextBlock)sender;
            }
            catch
            {
            }
        }

        private async void AppVersion_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.AppVersion_Block = (TextBlock)sender;
                this.setAppVersion();
                if (LocalSettings.localSettings.syncOtherContacts)
                {
                    string message = "Phone contacts will be backed up into your InTouch account.";
                    MessageDialog msgbox = new MessageDialog(message, "Backup Phone Contacts?");
                    msgbox.Commands.Add((IUICommand)new UICommand("Backup"));
                    msgbox.Commands.Add((IUICommand)new UICommand("Cancel"));
                    msgbox.put_DefaultCommandIndex(0U);
                    msgbox.put_CancelCommandIndex(0U);
                    IUICommand msgbox_result = await msgbox.ShowAsync();
                    if (msgbox_result.Label.Equals("Cancel"))
                    {
                        // ISSUE: reference to a compiler-generated method
                        // ISSUE: reference to a compiler-generated method
                        EasyTracker.GetTracker().SendEvent("sync", "messagedialog_accept_backup_phone_contact_tapped", "user tapped on messagedialog backup button, to accept backup phone contacts", 0L);
                        LocalSettings.localSettings.syncOtherContacts = false;
                        string message_cancel = "You can always backup contacts later from Settings.";
                        MessageDialog msgbox_cancel = new MessageDialog(message_cancel, "Backup Cancelled");
                        msgbox_cancel.Commands.Add((IUICommand)new UICommand("OK"));
                        msgbox_cancel.put_DefaultCommandIndex(0U);
                        msgbox_cancel.put_CancelCommandIndex(0U);
                        IUICommand iuiCommand = await msgbox_cancel.ShowAsync();
                    }
                    else
                    {
                        // ISSUE: reference to a compiler-generated method
                        // ISSUE: reference to a compiler-generated method
                        EasyTracker.GetTracker().SendEvent("sync", "messagedialog_cancel_backup_phone_contact_tapped", "user tapped on messagedialog cancel button, to cancel backup phone contacts", 0L);
                        string message_backup = "From now on, please add or edit contacts from your InTouch account in the People Hub." + Environment.NewLine + "InTouchApp will backup and sync these changes for you.";
                        MessageDialog msgbox_backup = new MessageDialog(message_backup, "Important");
                        msgbox_backup.Commands.Add((IUICommand)new UICommand("OK"));
                        msgbox_backup.put_DefaultCommandIndex(0U);
                        msgbox_backup.put_CancelCommandIndex(0U);
                        IUICommand iuiCommand = await msgbox_backup.ShowAsync();
                    }
                    if (Sync.isSyncRunning)
                        return;
                    if (this.MainHub.SectionsInView != null && this.MainHub.SectionsInView.Count > 0 && !string.Equals(((FrameworkElement)this.MainHub.SectionsInView[0]).Name, "Sync_Section"))
                        this.MainHub.ScrollToSection(this.Sync_Section);
                    await this.startSettingsSync(true);
                    await this.computeContactListView();
                }
                else if (LocalSettings.localSettings.syncSessionState == 7)
                {
                    ((UIElement)this.ContactListView).put_Visibility((Visibility)1);
                    ((UIElement)this.RestoringContacts).put_Visibility((Visibility)0);
                    if (Sync.isSyncRunning)
                        return;
                    if (this.MainHub.SectionsInView != null && this.MainHub.SectionsInView.Count > 0 && !string.Equals(((FrameworkElement)this.MainHub.SectionsInView[0]).Name, "Sync_Section"))
                        this.MainHub.ScrollToSection(this.Sync_Section);
                    await this.startSettingsSync(false);
                    await this.computeContactListView();
                }
                else
                {
                    if (ContactstoreInTouch.contactStore != null)
                        return;
                    Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
                    if (result.Item2)
                    {
                        LocalSettings.localSettings.syncSessionState = 7;
                        ((UIElement)this.ContactListView).put_Visibility((Visibility)1);
                        ((UIElement)this.RestoringContacts).put_Visibility((Visibility)0);
                        if (Sync.isSyncRunning)
                            return;
                        if (this.MainHub.SectionsInView != null && this.MainHub.SectionsInView.Count > 0 && !string.Equals(((FrameworkElement)this.MainHub.SectionsInView[0]).Name, "Sync_Section"))
                            this.MainHub.ScrollToSection(this.Sync_Section);
                        await this.startSettingsSync(false);
                        await this.computeContactListView();
                    }
                    else
                    {
                        await this.computeContactListView();
                        string format = "d MMM, hh.mm tt";
                        if (Sync.isSyncRunning)
                            return;
                        if (Sync.shallDownload)
                        {
                            await this.startSettingsSync(true);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(LocalSettings.localSettings.lastCheckedTime))
                                return;
                            DateTime LastCheckedTime = DateTime.ParseExact(LocalSettings.localSettings.lastCheckedTime, format, (IFormatProvider)CultureInfo.InvariantCulture).AddMinutes(30.0);
                            if (DateTime.Compare(DateTime.Now, LastCheckedTime) <= 0)
                                return;
                            await this.startSettingsSync(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile.Log("Error in loading app version block. " + ex.Message, EventType.Error);
                string str = "Unable to sync" + Environment.NewLine + "Please share feedback";
                Settings.Sync_Status_Block.put_Text(str);
                LocalSettings.localSettings.syncStatusBlock = str;
            }
        }

        private void DontCloseApp_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.DontCloseApp_Block = (TextBlock)sender;
            }
            catch
            {
            }
        }

        private void Sync_ProgressRing_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.Sync_ProgressRing = (ProgressRing)sender;
            }
            catch
            {
            }
        }

        private void DownloadPhotoOnWiFi_TS_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.DownloadPhotoOnWiFi_TS = (ToggleSwitch)sender;
                Sync.downloadPhotoOnWiFiTS = Settings.DownloadPhotoOnWiFi_TS;
                if (LocalSettings.localSettings.downloadPhotoOnWifi)
                    Settings.DownloadPhotoOnWiFi_TS.put_IsOn(true);
                else
                    Settings.DownloadPhotoOnWiFi_TS.put_IsOn(false);
            }
            catch
            {
            }
        }

        private void Backup_Phone_Contacts_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.Backup_Phone_Contacts_Block = (TextBlock)sender;
            }
            catch
            {
            }
        }

        private void Support_Block_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Support_Block = (TextBlock)sender;
            }
            catch
            {
            }
        }

        private void Feedback_Message_Text_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Feedback_Message_Text = (TextBox)sender;
            }
            catch
            {
            }
        }

        private void ShallAttachLog_CheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ShallAttachLog_CheckBox = (CheckBox)sender;
            }
            catch
            {
            }
        }

        private void Debug_Section_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ServerConnectionManager.mIsDeveloper)
                    return;
                ((UIElement)this.Debug_Section).put_Visibility((Visibility)0);
            }
            catch
            {
            }
        }

        private async void ContactList_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.ContactListView != null)
                    return;
                this.ContactListView = (ListView)sender;
                ((ItemsControl)this.ContactListView).put_ItemsSource((object)Sync.contactSamples);
                Sync.contactListView = this.ContactListView;
            }
            catch
            {
            }
        }

        private async Task computeContactListView()
        {
            try
            {
                Sync.contactSamples = new ObservableCollection<ContactSample>((IEnumerable<ContactSample>)InTouchAppDatabase.InTouchAppDB.readAllDBData(LocalSettings.localSettings.MCI).ToList<ContactSample>());
                if (Sync.contactSamples.Count > 0)
                    Sync.contactSamples = new ObservableCollection<ContactSample>(Sync.contactSamples.Where<ContactSample>((Func<ContactSample, bool>)(i => i.mContactName != string.Empty)).OrderBy<ContactSample, string>((Func<ContactSample, string>)(i => i.mContactName)).Concat<ContactSample>(Sync.contactSamples.Where<ContactSample>((Func<ContactSample, bool>)(i => i.mContactName == string.Empty))));
                ((ItemsControl)this.ContactListView).put_ItemsSource((object)Sync.contactSamples);
                ScrollViewer viewer = Settings.getScrollViewer((DependencyObject)this.ContactListView);
                WindowsRuntimeMarshal.AddEventHandler<EventHandler<ScrollViewerViewChangedEventArgs>>(new Func<EventHandler<ScrollViewerViewChangedEventArgs>, EventRegistrationToken>(viewer.add_ViewChanged), new Action<EventRegistrationToken>(viewer.remove_ViewChanged), new EventHandler<ScrollViewerViewChangedEventArgs>(this.MainPage_ViewChanged));
                ((UIElement)this.ContactListView).put_Visibility((Visibility)0);
                ((UIElement)this.RestoringContacts).put_Visibility((Visibility)1);
                await this.fetchContacts(0);
            }
            catch
            {
            }
        }

        private void Search_TB_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.SearchTextBox = (TextBox)sender;
            }
            catch
            {
            }
        }

        private void ContactList_LayoutUpdated(object sender, object e)
        {
            try
            {
                if (this.layoutUpdateFlag)
                    this.searchVisualTree((DependencyObject)this.ContactListView);
                this.layoutUpdateFlag = false;
            }
            catch
            {
            }
        }

        private void contact_display_name_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.contact_display_name = (TextBlock)sender;
            }
            catch
            {
            }
        }

        private void contact_organization_name_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.contact_organization_name = (TextBlock)sender;
            }
            catch
            {
            }
        }

        private void NoResult_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.NoResult = (TextBlock)sender;
            }
            catch
            {
            }
        }

        private void SyncBtn_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void LogoutBtn_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public static ScrollViewer getScrollViewer(DependencyObject depObj)
        {
            try
            {
                if (depObj is ScrollViewer)
                    return depObj as ScrollViewer;
                for (int index = 0; index < VisualTreeHelper.GetChildrenCount(depObj); ++index)
                {
                    ScrollViewer scrollViewer = Settings.getScrollViewer(VisualTreeHelper.GetChild(depObj, index));
                    if (scrollViewer != null)
                        return scrollViewer;
                }
                return (ScrollViewer)null;
            }
            catch
            {
                throw;
            }
        }

        private async void MainPage_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            try
            {
                ScrollViewer view = (ScrollViewer)sender;
                double progress = view.VerticalOffset / view.ScrollableHeight;
                if (!(progress > 0.7 & !this.incall) || this.endOfList)
                    return;
                this.incall = true;
                await this.fetchContacts(++this.offset);
            }
            catch
            {
            }
        }

        private async Task fetchContacts(int offset)
        {
            try
            {
                int start = offset * 20;
                if (Sync.contactSamples != null)
                {
                    for (int i = start; i < start + 20; ++i)
                    {
                        if (i < Sync.contactSamples.Count)
                        {
                            BitmapImage displayImage = await ContactSample.getWPDisplayImageFromID(Sync.contactSamples[i].contactID);
                            if (displayImage != null && ((BitmapSource)displayImage).PixelHeight != 0 && ((BitmapSource)displayImage).PixelWidth != 0)
                            {
                                ContactSample contactSample = Sync.contactSamples[i];
                                contactSample.mContactDisplayImage = displayImage;
                                Sync.contactSamples[i] = contactSample;
                            }
                        }
                        else
                        {
                            this.endOfList = true;
                            break;
                        }
                    }
                }
                this.incall = false;
            }
            catch
            {
            }
        }

        private void contactFilterString_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.setContactListViewAsPerSearchTextBoxVisibility();
                if (((ICollection<object>)((ItemsControl)this.ContactListView).Items).Count == 0)
                    ((UIElement)this.NoResult).put_Visibility((Visibility)0);
                else
                    ((UIElement)this.NoResult).put_Visibility((Visibility)1);
                this.layoutUpdateFlag = true;
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in contact filter string. " + ex.Message, EventType.Warning);
            }
        }

        private async void MultipleDeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ((Control)this.MultipleDeleteBtn).put_IsEnabled(false);
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("contactList", "appbar_btn_deletemultiple_clicked", "user clicked on delete multiple appbar button", 0L);
                List<ContactSample> contactSampleList = new List<ContactSample>();
                List<string> IDList = new List<string>();
                if (((ListViewBase)this.ContactListView).SelectedItems != null)
                {
                    foreach (object item in (IEnumerable<object>)((ListViewBase)this.ContactListView).SelectedItems)
                    {
                        ContactSample sample = item as ContactSample;
                        string ID = sample.contactID;
                        contactSampleList.Add(sample);
                        IDList.Add(ID);
                        try
                        {
                            await ContactstoreInTouch.contactStore.DeleteContactAsync(ID);
                        }
                        catch (Exception ex)
                        {
                            LogFile.Log("Error in deleting contact " + ID + ". " + ex.Message, EventType.Error);
                        }
                    }
                }
                if (contactSampleList != null)
                {
                    foreach (ContactSample contactSample in contactSampleList)
                        Sync.contactSamples.Remove(contactSample);
                }
                this.setContactListViewAsPerSearchTextBoxVisibility();
                ((Control)this.MultipleDeleteBtn).put_IsEnabled(true);
                ((ListViewBase)this.ContactListView).put_SelectionMode((ListViewSelectionMode)1);
                ((ListViewBase)this.ContactListView).put_IsItemClickEnabled(true);
                ((UIElement)this.MultipleSelectBtn).put_Visibility((Visibility)0);
                ((UIElement)this.MultipleDeleteBtn).put_Visibility((Visibility)1);
                ((UIElement)this.SearchBtn).put_Visibility((Visibility)0);
                ((UIElement)this.AddBtn).put_Visibility((Visibility)0);
                if (contactSampleList.Count == 0)
                    return;
                if (!Sync.isSyncRunning)
                {
                    this.startSettingsSync(false);
                }
                else
                {
                    await InTouchAppDatabase.InTouchAppDB.readAndMarkDeletedDirtyEntries(LocalSettings.localSettings.MCI, IDList);
                    Sync.setContactsManagedBlock();
                }
            }
            catch (Exception ex)
            {
                LogFile.Log("Error in multiple delete button click. " + ex.Message, EventType.Error);
            }
        }

        private void DownloadPhotoOnWiFi_TS_Toggled(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("settings", "toggleswitch_download_photo_on_wifi_toggled", "user toggled, download photo on wifi toggle switch", 0L);
                if (Settings.DownloadPhotoOnWiFi_TS.IsOn)
                    LocalSettings.localSettings.downloadPhotoOnWifi = true;
                else
                    LocalSettings.localSettings.downloadPhotoOnWifi = false;
            }
            catch (Exception ex)
            {
                LogFile.Log("Error in toggling download photo on Wifi switch. " + ex.Message, EventType.Error);
            }
        }

        private async void Backup_Phone_Contacts_Block_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("settings", "textblock_backup_phone_contacts_tapped", "user tapped on backup phone contacts textblock", 0L);
                if (!LocalSettings.localSettings.backupPhoneContactsBlockTapEnable || Sync.isSyncRunning)
                    return;
                LocalSettings.localSettings.syncOtherContacts = true;
                if (this.MainHub.SectionsInView != null && this.MainHub.SectionsInView.Count > 0 && !string.Equals(((FrameworkElement)this.MainHub.SectionsInView[0]).Name, "Sync_Section"))
                    this.MainHub.ScrollToSection(this.Sync_Section);
                await this.startSettingsSync(true);
            }
            catch (Exception ex)
            {
                LogFile.Log("Error in backing up phone contacts. " + ex.Message, EventType.Error);
            }
        }

        private async void Support_Block_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("settings", "textblock_feedback_tapped", "user tapped on feedback textblock", 0L);
                if (this.SCMobj.mIsConnectedToNetwork)
                {
                    FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
                }
                else
                {
                    string message = "Unable to share feedback." + Environment.NewLine + "Please check internet connectivity.";
                    MessageDialog msgbox = new MessageDialog(message, "Feedback Support");
                    msgbox.Commands.Add((IUICommand)new UICommand("OK"));
                    msgbox.put_DefaultCommandIndex(0U);
                    msgbox.put_CancelCommandIndex(0U);
                    IUICommand iuiCommand = await msgbox.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                LogFile.Log("Error in feedback in settings. " + ex.Message, EventType.Error);
            }
        }

        private async void NeedHelp_Block_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("settings", "textblock_help_tapped", "user tapped on help textblock", 0L);
                int num = await Launcher.LaunchUriAsync(new Uri(this.SCMobj.getServerName() + "/faq/winphone/", UriKind.Absolute)) ? 1 : 0;
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in opening help link. " + ex.Message, EventType.Warning);
            }
        }

        private async void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("settings", "appbar_btn_logout_clicked", "user clicked on logout button", 0L);
                string message = "Logout is not recommeneded." + Environment.NewLine + "Any unsynced changes will be lost and InTouch managed contacts will be removed from device." + Environment.NewLine + "Re-login will bring your InTouch contacts back to this device.";
                MessageDialog msgbox = new MessageDialog(message, "Logout Warning");
                // ISSUE: method pointer
                msgbox.Commands.Add((IUICommand)new UICommand("Logout", new UICommandInvokedHandler((object)this, __methodptr(logoutInvokedHandler))));
                // ISSUE: method pointer
                msgbox.Commands.Add((IUICommand)new UICommand("Cancel", new UICommandInvokedHandler((object)this, __methodptr(logoutInvokedHandler))));
                msgbox.put_DefaultCommandIndex(1U);
                msgbox.put_CancelCommandIndex(1U);
                IUICommand iuiCommand = await msgbox.ShowAsync();
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in logout btn click. " + ex.Message, EventType.Warning);
                throw;
            }
        }

        private async void logoutInvokedHandler(IUICommand command)
        {
            try
            {
                if (string.Equals(command.Label, "Logout", StringComparison.OrdinalIgnoreCase))
                {
                    // ISSUE: reference to a compiler-generated method
                    // ISSUE: reference to a compiler-generated method
                    EasyTracker.GetTracker().SendEvent("settings", "messagedialog_btn_accept_logout_tapped", "user tapped on messagedialog logout button, to accept logout", 0L);
                    string text = string.Empty;
                    Settings.Sync_Status_Block.put_Text(text);
                    LocalSettings.localSettings.syncStatusBlock = text;
                    Settings.Image_Status_Block.put_Text(text);
                    LocalSettings.localSettings.imageStatusBlock = text;
                    Frame currentFrame = (Frame)Window.Current.Content;
                    currentFrame.Navigate(typeof(LoggingOut));
                    try
                    {
                        if (ContactstoreInTouch.contactStore == null)
                        {
                            Tuple<bool, bool> contactStore = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
                        }
                        await ContactstoreInTouch.contactStore.DeleteAsync();
                        ContactstoreInTouch.contactStoreInTouch.setContactStore();
                    }
                    catch (Exception ex)
                    {
                        LogFile.Log("Error in deleting contact store. " + ex.Message, EventType.Error);
                        ContactstoreInTouch.contactStoreInTouch.setContactStore();
                    }
                    LogFile.Log("Deleted contact store on logout.", EventType.Information);
                    await InTouchAppDatabase.InTouchAppDB.deleteDB(LocalSettings.localSettings.MCI);
                    LogFile.Log("Deleted DB on logout.", EventType.Information);
                    this.SCMobj.logoutFromInTouch(await LocalSettings.localSettings.getToken());
                    Task SetToken = LocalSettings.localSettings.setToken("");
                    Task DeleteContactFiles = LocalSettings.localSettings.deleteAllContactFiles();
                    LocalSettings.localSettings.iid = string.Empty;
                    LocalSettings.localSettings.versionAuto = -1;
                    LocalSettings.localSettings.versionManual = -2;
                    LocalSettings.localSettings.revisionNumber = 0;
                    LocalSettings.localSettings.lastSyncTime = string.Empty;
                    LocalSettings.localSettings.MCI = string.Empty;
                    LocalSettings.localSettings.serverName = string.Empty;
                    Sync.contactSamples.Clear();
                    if (this.channel != null)
                        this.channel.Close();
                    LocalSettings.localSettings.channelUri = string.Empty;
                    await SetToken;
                    await DeleteContactFiles;
                    Frame currentFrame1 = (Frame)Window.Current.Content;
                    currentFrame1.Navigate(typeof(Login));
                }
                else
                {
                    // ISSUE: reference to a compiler-generated method
                    // ISSUE: reference to a compiler-generated method
                    EasyTracker.GetTracker().SendEvent("settings", "messagedialog_btn_cancel_logout_tapped", "user tapped on messagedialog cancel button, to cancel logout", 0L);
                }
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in logout invoke handler. " + ex.Message, EventType.Warning);
                string str = "Unable to Logout." + Environment.NewLine + "Please try again.";
                Frame content = (Frame)Window.Current.Content;
                if (LocalSettings.localSettings.token == null)
                    content.Navigate(typeof(Login));
                else
                    content.Navigate(typeof(Settings));
            }
        }

        private async void SendFeedback_Flyout_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("settings", "flyout_btn_sendfeedback_tapped", "user tapped on send feedback flyout button to share feedback", 0L);
                if (!((UIElement)this.Support_Block).IsTapEnabled)
                    return;
                ((UIElement)this.Support_Block).put_IsTapEnabled(false);
                string Feedback_Reply_message = string.Empty;
                string Feedback_Message = string.Empty;
                if (!string.IsNullOrEmpty(this.Feedback_Message_Text.Text))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    string[] strArray = this.Feedback_Message_Text.Text.Split('\n');
                    if (strArray != null)
                    {
                        foreach (string str in strArray)
                            stringBuilder.AppendLine(str);
                        bool flag;
                        int num = flag ? 1 : 0;
                    }
                    Feedback_Message = stringBuilder.ToString();
                }
                bool? isChecked = ((ToggleButton)this.ShallAttachLog_CheckBox).IsChecked;
                if ((!isChecked.GetValueOrDefault() ? 0 : (isChecked.HasValue ? 1 : 0)) != 0)
                    Feedback_Message = string.IsNullOrEmpty(Feedback_Message) ? LogFile.GetLogForFeedback() : Feedback_Message + Environment.NewLine + "\n" + LogFile.GetLogForFeedback();
                bool isSuccess = false;
                string message = string.Empty;
                if (!string.IsNullOrEmpty(Feedback_Message))
                {
                    Tuple<bool, string> result = await this.SCMobj.feedback(await LocalSettings.localSettings.getToken(), Feedback_Message);
                    isSuccess = result.Item1;
                    Feedback_Reply_message = result.Item2;
                    if (isSuccess)
                        this.Feedback_Message_Text.put_Text(string.Empty);
                    message = Feedback_Reply_message;
                }
                else
                    message = "Feedback can't be empty. " + Environment.NewLine + "Please share your feedback.";
                MessageDialog msgbox = new MessageDialog(message, "Feedback Support");
                msgbox.Commands.Add((IUICommand)new UICommand("OK"));
                msgbox.put_DefaultCommandIndex(0U);
                msgbox.put_CancelCommandIndex(0U);
                IUICommand iuiCommand = await msgbox.ShowAsync();
                ((UIElement)this.Support_Block).put_IsTapEnabled(true);
            }
            catch (Exception ex)
            {
                LogFile.Log("Error in sending email to InTouch Support. " + ex.Message, EventType.Error);
                ((UIElement)this.Support_Block).put_IsTapEnabled(true);
            }
        }

        private void onKeyDownHandler(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                if (e.Key != 13)
                    return;
                this.Feedback_Message_Text.put_Text(this.Feedback_Message_Text.Text + Environment.NewLine);
            }
            catch
            {
            }
        }

        private async void MainHub_SectionsInViewChanged(
          object sender,
          SectionsInViewChangedEventArgs e)
        {
            try
            {
                if (this.MainHub.SectionsInView == null || this.MainHub.SectionsInView.Count <= 0)
                    return;
                switch (((FrameworkElement)this.MainHub.SectionsInView[0]).Name)
                {
                    case "ContacList_Section":
                        if (LocalSettings.localSettings.syncSessionState == 7)
                        {
                            this.put_BottomAppBar((AppBar)this.contactListCommandBar);
                            ((UIElement)this.BottomAppBar).put_Visibility((Visibility)1);
                        }
                        else
                        {
                            this.put_BottomAppBar((AppBar)this.contactListCommandBar);
                            ((UIElement)this.BottomAppBar).put_Visibility((Visibility)0);
                            this.BottomAppBar.put_ClosedDisplayMode((AppBarClosedDisplayMode)0);
                            if (this.ContactListView != null)
                            {
                                if (((ListViewBase)this.ContactListView).SelectionMode == 2)
                                {
                                    ((UIElement)this.SearchBtn).put_Visibility((Visibility)1);
                                    ((UIElement)this.AddBtn).put_Visibility((Visibility)1);
                                    ((UIElement)this.MultipleSelectBtn).put_Visibility((Visibility)1);
                                    ((UIElement)this.MultipleDeleteBtn).put_Visibility((Visibility)0);
                                }
                                else
                                {
                                    ((UIElement)this.SearchBtn).put_Visibility((Visibility)0);
                                    ((UIElement)this.AddBtn).put_Visibility((Visibility)0);
                                    ((UIElement)this.MultipleSelectBtn).put_Visibility((Visibility)0);
                                    ((UIElement)this.MultipleDeleteBtn).put_Visibility((Visibility)1);
                                }
                            }
                        }
                        // ISSUE: reference to a compiler-generated method
                        // ISSUE: reference to a compiler-generated method
                        EasyTracker.GetTracker().SendView("contactList");
                        break;
                    case "Sync_Section":
                        this.put_BottomAppBar((AppBar)this.syncCommandBar);
                        ((UIElement)this.BottomAppBar).put_Visibility((Visibility)0);
                        this.BottomAppBar.put_ClosedDisplayMode((AppBarClosedDisplayMode)0);
                        // ISSUE: reference to a compiler-generated method
                        // ISSUE: reference to a compiler-generated method
                        EasyTracker.GetTracker().SendView("sync");
                        break;
                    case "Settings_Section":
                        this.put_BottomAppBar((AppBar)this.settingsCommandBar);
                        ((UIElement)this.BottomAppBar).put_Visibility((Visibility)0);
                        this.BottomAppBar.put_ClosedDisplayMode((AppBarClosedDisplayMode)1);
                        if (this.AccountType_Block != null)
                            this.getAccountType();
                        // ISSUE: reference to a compiler-generated method
                        // ISSUE: reference to a compiler-generated method
                        EasyTracker.GetTracker().SendView("settings");
                        break;
                    case "About_Section":
                        this.put_BottomAppBar((AppBar)this.aboutCommandBar);
                        ((UIElement)this.BottomAppBar).put_Visibility((Visibility)0);
                        this.BottomAppBar.put_ClosedDisplayMode((AppBarClosedDisplayMode)1);
                        // ISSUE: reference to a compiler-generated method
                        // ISSUE: reference to a compiler-generated method
                        EasyTracker.GetTracker().SendView("about");
                        break;
                    case "Debug_Section":
                        if (!ServerConnectionManager.mIsDeveloper)
                            break;
                        ((UIElement)this.BottomAppBar).put_Visibility((Visibility)1);
                        try
                        {
                            if (ContactstoreInTouch.contactStore == null)
                            {
                                Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
                                if (result.Item2)
                                    await RestoreContactStore.restoreContactStore.restoreContacts((List<string>)null, true);
                            }
                            this.SyncSession_Owner_Block.put_Text("Owner: " + LocalSettings.localSettings.syncSessionOwner);
                            this.SyncSession_ID_Block.put_Text("Sync ID: " + LocalSettings.localSettings.syncSessionID);
                            SyncSessionState SyncState = (SyncSessionState)LocalSettings.localSettings.syncSessionState;
                            this.SyncSession_State_Block.put_Text("Sync State: " + SyncState.ToString());
                            this.SyncSession_LastUpdateTime_Block.put_Text("Sync Last Update: " + LocalSettings.localSettings.syncSessionLastUpdateTime);
                            IReadOnlyList<ContactChangeRecord> result1 = await ContactstoreInTouch.contactStore.GetChangesAsync((ulong)LocalSettings.localSettings.revisionNumber);
                            this.ModifiedContactsCount_Block.put_Text("Modified Store Contacts: " + (object)result1.Count);
                            this.StoreRevisionNumber_Block.put_Text("Saved Revision No.: " + (object)LocalSettings.localSettings.revisionNumber + "\nStore Revision no.: " + (object)Convert.ToInt32(ContactstoreInTouch.contactStore.RevisionNumber));
                            this.DBDirtyIntouchContactsCount_Block.put_Text("Modified DB Contacts: " + InTouchAppDatabase.InTouchAppDB.getDirtyIntouchEntriesCount(LocalSettings.localSettings.MCI).ToString());
                            // ISSUE: reference to a compiler-generated method
                            // ISSUE: reference to a compiler-generated method
                            EasyTracker.GetTracker().SendView("debug");
                            break;
                        }
                        catch (Exception ex)
                        {
                            LogFile.Log("Problem in getting modified contacts count. " + ex.Message, EventType.Warning);
                            break;
                        }
                }
            }
            catch
            {
            }
        }

        private async void AccountType_Header_Block_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("settings", "textblock_account_type_tapped", "user tapped on account type textblock", 0L);
                if (!((UIElement)this.AccountType_Header_Block).IsTapEnabled)
                    return;
                ((UIElement)this.AccountType_Header_Block).put_IsTapEnabled(false);
                await this.getAccountType();
                ((UIElement)this.AccountType_Header_Block).put_IsTapEnabled(true);
            }
            catch (Exception ex)
            {
                ((UIElement)this.AccountType_Header_Block).put_IsTapEnabled(true);
                LogFile.Log("Problem in getting account type. " + ex.Message, EventType.Warning);
            }
        }

        private async void Upgrade_Block_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("settings", "textblock_upgrade_tapped", "user tapped on upgrade textblock", 0L);
                int num = await Launcher.LaunchUriAsync(new Uri(this.SCMobj.getServerName() + "/user/upgrade/?iid=" + LocalSettings.localSettings.iid + "&utm_medium=intouchapp&utm_source=winphone&utm_term=settings", UriKind.Absolute)) ? 1 : 0;
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in opening upgrade link. " + ex.Message, EventType.Warning);
            }
        }

        private async void SyncBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("sync", "appbar_btn_sync_clicked", "user clicked on sync appbar button", 0L);
                await this.startSettingsSync(true);
            }
            catch
            {
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("contactList", "appbar_btn_add_clicked", "user clicked on add appbar button", 0L);
                ((Frame)Window.Current.Content).Navigate(typeof(EditContact));
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in add button click. " + ex.Message, EventType.Warning);
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("contactList", "appbar_btn_search_clicked", "user clicked on search appbar button", 0L);
                ((UIElement)this.SearchTextBox).put_Visibility((Visibility)0);
                ((Control)this.SearchTextBox).Focus((FocusState)2);
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in search button click. " + ex.Message, EventType.Warning);
            }
        }

        private void ShareBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("settings", "appbar_btn_share_clicked", "user clicked on share appbar button", 0L);
                DataTransferManager.ShowShareUI();
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in share button click. " + ex.Message, EventType.Warning);
            }
        }

        private void ContactList_Holding(object sender, HoldingRoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("contactList", "contactList_holding", "user hold on listview", 0L);
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            }
            catch (Exception ex)
            {
                LogFile.Log("Error in contactListView holding. " + ex.Message, EventType.Error);
            }
        }

        private void ContactList_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("contactList", "listviewitem_clicked", "user clicked on listviewitem", 0L);
                if (!(e.ClickedItem is ContactSample clickedItem))
                    return;
                ((Selector)this.ContactListView).put_SelectedItem((object)null);
                ((Frame)Window.Current.Content).Navigate(typeof(ContactInformation), (object)new NavigationParameters()
                {
                    action = "listViewItem",
                    data = clickedItem.contactID
                });
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in selecting contact. " + ex.Message, EventType.Warning);
            }
        }

        private void MultipleSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("contactList", "multipleSelection_btn_search_clicked", "user clicked on multiple selection appbar button", 0L);
                ((ListViewBase)this.ContactListView).put_IsItemClickEnabled(false);
                ((ListViewBase)this.ContactListView).put_SelectionMode((ListViewSelectionMode)2);
                ((UIElement)this.MultipleDeleteBtn).put_Visibility((Visibility)0);
                ((UIElement)this.MultipleSelectBtn).put_Visibility((Visibility)1);
                ((UIElement)this.SearchBtn).put_Visibility((Visibility)1);
                ((UIElement)this.AddBtn).put_Visibility((Visibility)1);
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in multiple selection button click. " + ex.Message, EventType.Warning);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Frame.CanGoForward)
                    this.Frame.ForwardStack.RemoveAt(this.Frame.ForwardStack.Count - 1);
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

        private async void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            try
            {
                if (!(Window.Current.Content is Frame _))
                    return;
                e.put_Handled(true);
                if (((ListViewBase)this.ContactListView).SelectionMode == 2)
                {
                    ((ListViewBase)this.ContactListView).SelectedItems.Clear();
                    ((ListViewBase)this.ContactListView).put_SelectionMode((ListViewSelectionMode)1);
                    ((ListViewBase)this.ContactListView).put_IsItemClickEnabled(true);
                    ((UIElement)this.MultipleDeleteBtn).put_Visibility((Visibility)1);
                    ((UIElement)this.MultipleSelectBtn).put_Visibility((Visibility)0);
                    ((UIElement)this.SearchBtn).put_Visibility((Visibility)0);
                    ((UIElement)this.AddBtn).put_Visibility((Visibility)0);
                }
                else
                {
                    if (this.SearchTextBox == null)
                        return;
                    if (((UIElement)this.SearchTextBox).Visibility == null)
                    {
                        this.SearchTextBox.put_Text(string.Empty);
                        this.layoutUpdateFlag = false;
                        ((UIElement)this.NoResult).put_Visibility((Visibility)1);
                        ((ItemsControl)this.ContactListView).put_ItemsSource((object)Sync.contactSamples);
                        ((UIElement)this.SearchTextBox).put_Visibility((Visibility)1);
                    }
                    else
                    {
                        ((UIElement)this.NoResult).put_Visibility((Visibility)1);
                        string empty1 = string.Empty;
                        string empty2 = string.Empty;
                        if (Sync.isSyncRunning)
                        {
                            MessageDialog msgbox = new MessageDialog("Sync is running." + Environment.NewLine + "Please do no close the app", "Exit Warning");
                            msgbox.Commands.Add((IUICommand)new UICommand("OK"));
                            IUICommand iuiCommand = await msgbox.ShowAsync();
                        }
                        else
                            Application.Current.Exit();
                    }
                }
            }
            catch
            {
            }
        }

        private void searchVisualTree(DependencyObject targetElement)
        {
            try
            {
                if (targetElement == null)
                    return;
                int childrenCount = VisualTreeHelper.GetChildrenCount(targetElement);
                for (int index = 0; index < childrenCount; ++index)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(targetElement, index);
                    if (child is TextBlock)
                    {
                        this.textBlock1 = (TextBlock)child;
                        this.prevText = this.textBlock1.Text;
                        if ((((FrameworkElement)this.textBlock1).Name == "contact_organization_name" || ((FrameworkElement)this.textBlock1).Name == "contact_display_name") && this.textBlock1.Text.ToUpper().Contains(this.SearchTextBox.Text.ToUpper()))
                            this.highlightText();
                    }
                    else
                        this.searchVisualTree(child);
                }
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in searching visual tree. " + ex.Message, EventType.Warning);
            }
        }

        private void highlightText()
        {
            try
            {
                if (this.textBlock1 == null)
                    return;
                string upper = this.textBlock1.Text.ToUpper();
                ((ICollection<Inline>)this.textBlock1.Inlines).Clear();
                int num = upper.IndexOf(this.SearchTextBox.Text.ToUpper());
                int length = this.SearchTextBox.Text.Length;
                if (num < 0)
                    return;
                Run run1 = new Run();
                run1.put_Text(this.prevText.Substring(num, length));
                Run run2 = run1;
                ((TextElement)run2).put_Foreground((Brush)new SolidColorBrush(Colors.Orange));
                InlineCollection inlines1 = this.textBlock1.Inlines;
                Run run3 = new Run();
                run3.put_Text(this.prevText.Substring(0, num));
                Run run4 = run3;
                ((ICollection<Inline>)inlines1).Add((Inline)run4);
                ((ICollection<Inline>)this.textBlock1.Inlines).Add((Inline)run2);
                InlineCollection inlines2 = this.textBlock1.Inlines;
                Run run5 = new Run();
                run5.put_Text(this.prevText.Substring(num + length));
                Run run6 = run5;
                ((ICollection<Inline>)inlines2).Add((Inline)run6);
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in highlighting selected text. " + ex.Message, EventType.Warning);
            }
        }

        public async Task startSettingsSync(bool shallDownload)
        {
            try
            {
                if (!((Control)this.SyncBtn).IsEnabled)
                    return;
                ((Control)this.SyncBtn).put_IsEnabled(false);
                ((Control)this.LogoutBtn).put_IsEnabled(false);
                Settings.setBackupPhoneContactsBlock(false);
                Settings.Sync_ProgressRing.put_IsActive(true);
                Settings.setDontCloseAppBlock((Visibility)0);
                Settings.Sync_Status_Block.put_FontSize(20.26);
                if (string.IsNullOrEmpty(LocalSettings.localSettings.channelUri))
                    await this.openChannelAndRegisterTask();
                LocalSettings.localSettings.syncSessionOwner = SyncStateOwner.App.ToString();
                await Sync.sync.startSync(shallDownload);
                Settings.Sync_ProgressRing.put_IsActive(false);
                ((Control)this.SyncBtn).put_IsEnabled(true);
                ((Control)this.LogoutBtn).put_IsEnabled(true);
                Settings.setBackupPhoneContactsBlock(true);
                Settings.setDontCloseAppBlock((Visibility)1);
                await this.fetchContacts(0);
            }
            catch (Exception ex)
            {
                if (LocalSettings.localSettings.syncSessionState != 7)
                    LocalSettings.localSettings.syncSessionState = -1;
                string empty1 = string.Empty;
                Settings.Image_Status_Block.put_Text(empty1);
                LocalSettings.localSettings.imageStatusBlock = empty1;
                Settings.Sync_ProgressRing.put_IsActive(false);
                ((Control)this.SyncBtn).put_IsEnabled(true);
                ((Control)this.LogoutBtn).put_IsEnabled(true);
                Settings.setBackupPhoneContactsBlock(true);
                Settings.setDontCloseAppBlock((Visibility)1);
                string empty2 = string.Empty;
                if (ex is NotSupportedException)
                {
                    Settings.Sync_Status_Block.put_FontSize(18.0);
                    string message = ex.Message;
                    Settings.Sync_Status_Block.put_Text(message);
                    LocalSettings.localSettings.syncStatusBlock = message;
                    LogFile.Log("Unable to sync. " + ex.Message, EventType.Error);
                }
                else
                {
                    string str = "Unable to sync";
                    Settings.Sync_Status_Block.put_Text(str);
                    LocalSettings.localSettings.syncStatusBlock = str;
                    LogFile.Log("Unable to sync. " + ex.Message, EventType.Error);
                }
            }
        }

        private async Task getAccountType()
        {
            try
            {
                bool isSuccess = false;
                if (this.SCMobj.mIsConnectedToNetwork)
                {
                    Tuple<bool, string, string> result = await this.SCMobj.getUserAccountStatus(await LocalSettings.localSettings.getToken());
                    isSuccess = result.Item1;
                    string AccountType_Name = result.Item2;
                    if (isSuccess)
                    {
                        string str = result.Item3;
                        if (!string.IsNullOrEmpty(AccountType_Name))
                        {
                            LocalSettings.localSettings.accountType = AccountType_Name;
                            this.AccountType_Block.put_Text(AccountType_Name);
                        }
                        if (string.IsNullOrEmpty(str))
                            return;
                        if (str.Equals("IND_003"))
                            ((UIElement)this.Upgrade_Block).put_Visibility((Visibility)1);
                        else
                            ((UIElement)this.Upgrade_Block).put_Visibility((Visibility)0);
                    }
                    else
                    {
                        this.AccountType_Block.put_Text(LocalSettings.localSettings.accountType);
                        if (this.AccountType_Block.Text.Equals("IND_003") || string.IsNullOrEmpty(this.AccountType_Block.Text))
                            ((UIElement)this.Upgrade_Block).put_Visibility((Visibility)1);
                        else
                            ((UIElement)this.Upgrade_Block).put_Visibility((Visibility)0);
                        if (string.IsNullOrEmpty(AccountType_Name))
                            return;
                        MessageDialog msgbox = new MessageDialog(AccountType_Name, "Warning");
                        msgbox.Commands.Add((IUICommand)new UICommand("OK"));
                        msgbox.put_DefaultCommandIndex(0U);
                        msgbox.put_CancelCommandIndex(0U);
                        IUICommand iuiCommand = await msgbox.ShowAsync();
                    }
                }
                else
                {
                    this.AccountType_Block.put_Text(LocalSettings.localSettings.accountType);
                    if (this.AccountType_Block.Text.Equals("IND_003") || string.IsNullOrEmpty(this.AccountType_Block.Text))
                        ((UIElement)this.Upgrade_Block).put_Visibility((Visibility)1);
                    else
                        ((UIElement)this.Upgrade_Block).put_Visibility((Visibility)0);
                    string message = "Please check internet connectivity.";
                    MessageDialog msgbox = new MessageDialog(message, "Warning");
                    msgbox.Commands.Add((IUICommand)new UICommand("OK"));
                    msgbox.put_DefaultCommandIndex(0U);
                    msgbox.put_CancelCommandIndex(0U);
                    IUICommand iuiCommand = await msgbox.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in getting account type. " + ex.Message, EventType.Warning);
            }
        }

        private void setAppVersion()
        {
            try
            {
                string appVersion = LocalSettings.localSettings.appVersion;
                if (ServerConnectionManager.mIsDebug)
                    this.AppVersion_Block.put_Text(appVersion + Environment.NewLine + this.SCMobj.getServerNameAPI());
                else
                    this.AppVersion_Block.put_Text(appVersion);
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in getting app version for Settings. " + ex.Message, EventType.Warning);
            }
        }

        private static void setBackupPhoneContactsBlock(bool isTapEnabled)
        {
            try
            {
                LocalSettings.localSettings.backupPhoneContactsBlockTapEnable = isTapEnabled;
                if (Settings.Backup_Phone_Contacts_Block == null)
                    return;
                ((UIElement)Settings.Backup_Phone_Contacts_Block).put_IsTapEnabled(isTapEnabled);
            }
            catch
            {
            }
        }

        private static void setDontCloseAppBlock(Visibility _visibility)
        {
            try
            {
                LocalSettings.localSettings.dontCloseAppVisible = _visibility == null;
                if (Settings.DontCloseApp_Block == null)
                    return;
                ((UIElement)Settings.DontCloseApp_Block).put_Visibility(_visibility);
            }
            catch
            {
            }
        }

        private void MemoryUsage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void AppMemoryUsage_Block_Loaded(object sender, RoutedEventArgs e)
        {
            this.AppMemoryUsage = (TextBlock)sender;
        }

        private void AppMemoryUsageLevel_Block_Loaded(object sender, RoutedEventArgs e)
        {
            this.AppMemoryUsageLevel = (TextBlock)sender;
        }

        private void AppMemoryUsageLimit_Block_Loaded(object sender, RoutedEventArgs e)
        {
            this.AppMemoryUsageLimit = (TextBlock)sender;
        }

        private async void Edit_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("contactList", "menuflyoutitem_edit_clicked", "user clicked on edit MenuFlyoutItem", 0L);
                if (!((e.OriginalSource as FrameworkElement).DataContext is ContactSample sample))
                    return;
                string ID = sample.contactID;
                if (ContactstoreInTouch.contactStore == null)
                {
                    Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
                    if (result.Item2)
                        await RestoreContactStore.restoreContactStore.restoreContacts((List<string>)null, true);
                }
                StoredContact WPContact = (StoredContact)null;
                try
                {
                    WPContact = await ContactstoreInTouch.contactStore.FindContactByIdAsync(ID);
                }
                catch (Exception ex)
                {
                    LogFile.Log("Error in reading contact to display from WP. " + ex.Message, EventType.Error);
                }
                if (WPContact == null)
                    return;
                IDictionary<string, object> props = await WPContact.GetPropertiesAsync();
                Avatar avatar = await ContactstoreInTouch.contactStoreInTouch.getManualContactFromWP(WPContact, props);
                NavigationParameters navigationParameter = new NavigationParameters();
                navigationParameter.action = "contactInfo#" + ID;
                navigationParameter.data = JsonConvert.SerializeObject((object)avatar, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                Frame currentFrame = (Frame)Window.Current.Content;
                currentFrame.Navigate(typeof(EditContact), (object)navigationParameter);
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in edit MenuFlyoutItem click. " + ex.Message, EventType.Warning);
            }
        }

        private async void Delete_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("contactList", "menuflyoutitem_delete_clicked", "user clicked on delete MenuFlyoutItem", 0L);
                if (!((e.OriginalSource as FrameworkElement).DataContext is ContactSample sample))
                    return;
                string message = sample.mContactDisplayName + " will be deleted from your InTouch account.";
                MessageDialog msgbox = new MessageDialog(message, "Delete Contact?");
                msgbox.Commands.Add((IUICommand)new UICommand("Delete"));
                msgbox.Commands.Add((IUICommand)new UICommand("Cancel"));
                msgbox.put_DefaultCommandIndex(1U);
                msgbox.put_CancelCommandIndex(1U);
                IUICommand msgbox_result = await msgbox.ShowAsync();
                if (!msgbox_result.Label.Equals("Delete"))
                    return;
                string ID = sample.contactID;
                try
                {
                    await ContactstoreInTouch.contactStore.DeleteContactAsync(ID);
                    Sync.contactSamples.Remove(sample);
                    this.setContactListViewAsPerSearchTextBoxVisibility();
                }
                catch (Exception ex)
                {
                    LogFile.Log("Error in deleting contact " + ID + ". " + ex.Message, EventType.Error);
                }
                if (!Sync.isSyncRunning)
                {
                    this.startSettingsSync(false);
                }
                else
                {
                    await InTouchAppDatabase.InTouchAppDB.readAndMarkDeletedDirtyEntries(LocalSettings.localSettings.MCI, new List<string>()
          {
            ID
          });
                    Sync.setContactsManagedBlock();
                }
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in delete MenuFlyoutItem click. " + ex.Message, EventType.Warning);
            }
        }

        private void setContactListViewAsPerSearchTextBoxVisibility()
        {
            try
            {
                if (((UIElement)this.SearchTextBox).Visibility != null)
                    return;
                ((ItemsControl)this.ContactListView).put_ItemsSource((object)Sync.contactSamples.Where<ContactSample>((Func<ContactSample, bool>)(w => w.mContactDisplayName.ToUpper().Contains(this.SearchTextBox.Text.ToUpper()) || w.mContactOrganizationInfo.ToUpper().Contains(this.SearchTextBox.Text.ToUpper()))));
            }
            catch
            {
            }
        }

        private async void Share_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("contactList", "menuflyoutitem_share_clicked", "user clicked on share MenuFlyoutItem", 0L);
                if (!((e.OriginalSource as FrameworkElement).DataContext is ContactSample sample))
                    return;
                this.displayName = sample.mContactDisplayName;
                string email_text = string.Empty;
                string phone_text = string.Empty;
                StoredContact WPContact = (StoredContact)null;
                try
                {
                    WPContact = await ContactstoreInTouch.contactStore.FindContactByIdAsync(sample.contactID);
                }
                catch (Exception ex)
                {
                    LogFile.Log("Error in reading contact to display from WP. " + ex.Message, EventType.Error);
                }
                if (WPContact == null)
                    return;
                IDictionary<string, object> props = await WPContact.GetPropertiesAsync();
                if (props != null)
                {
                    foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>)props)
                    {
                        object obj = keyValuePair.Value;
                        if (obj != null)
                        {
                            switch (keyValuePair.Key)
                            {
                                case "Email":
                                    email_text = email_text + Environment.NewLine + obj.ToString() + " (Personal)";
                                    continue;
                                case "WorkEmail":
                                    email_text = email_text + Environment.NewLine + obj.ToString() + " (Work)";
                                    continue;
                                case "OtherEmail":
                                    email_text = email_text + Environment.NewLine + obj.ToString() + " (Other)";
                                    continue;
                                case "MobileTelephone":
                                    phone_text = phone_text + Environment.NewLine + obj.ToString() + " (Mobile)";
                                    continue;
                                case "AlternateMobileTelephone":
                                    phone_text = phone_text + Environment.NewLine + obj.ToString() + " (Mobile 2)";
                                    continue;
                                case "Telephone":
                                    phone_text = phone_text + Environment.NewLine + obj.ToString() + " (Home)";
                                    continue;
                                case "WorkTelephone":
                                    phone_text = phone_text + Environment.NewLine + obj.ToString() + " (Work)";
                                    continue;
                                case "AlternateWorkTelephone":
                                    phone_text = phone_text + Environment.NewLine + obj.ToString() + " (Work 2)";
                                    continue;
                                case "HomeFax":
                                    phone_text = phone_text + Environment.NewLine + obj.ToString() + " (Home Fax)";
                                    continue;
                                case "WorkFax":
                                    phone_text = phone_text + Environment.NewLine + obj.ToString() + " (Work Fax)";
                                    continue;
                                case "CompanyPhone":
                                    phone_text = phone_text + Environment.NewLine + obj.ToString() + " (Company)";
                                    continue;
                                case "Telephone2":
                                    phone_text = phone_text + Environment.NewLine + obj.ToString() + " (Home 2)";
                                    continue;
                                default:
                                    continue;
                            }
                        }
                    }
                }
                if (string.IsNullOrEmpty(phone_text))
                    this.shareText = phone_text + email_text + Environment.NewLine + "-via InTouchApp.com";
                else
                    this.shareText = phone_text + Environment.NewLine + email_text + Environment.NewLine + "-via InTouchApp.com";
                DataTransferManager.ShowShareUI();
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in share MenuFlyoutItem click. " + ex.Message, EventType.Warning);
            }
        }

        private void shareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            try
            {
                DataRequest request = e.Request;
                if (!string.IsNullOrEmpty(this.shareText))
                {
                    request.Data.Properties.put_Title(this.displayName);
                    request.Data.SetText(this.shareText);
                    this.shareText = string.Empty;
                }
                else
                {
                    request.Data.Properties.put_Title("Have you tried InTouchApp?");
                    string str = "It keeps your contacts safe & up-to-date. I am loving it & you will too! Download for FREE at http://intouchapp.com/" + LocalSettings.localSettings.iid + "/?r=wp01";
                    request.Data.SetText(str);
                }
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in sharing contact info. " + ex.Message, EventType.Warning);
            }
        }

        private void RestoringContacts_Loaded(object sender, RoutedEventArgs e)
        {
            this.RestoringContacts = (TextBlock)sender;
        }//
             
    }
}
