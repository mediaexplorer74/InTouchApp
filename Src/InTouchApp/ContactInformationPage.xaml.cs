// Decompiled with JetBrains decompiler
// Type: windowsphone_app.ContactInformation
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Calls;
using Windows.ApplicationModel.Chat;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Phone.PersonalInformation;
using Windows.Phone.UI.Input;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

#nullable disable
namespace windowsphone_app
{
    public sealed partial class ContactInformation : Page
    {
        private StoredContact WPContact;
        private Avatar avatar = new Avatar();
        private string ID = string.Empty;

        public ContactInformation()
        {
            this.InitializeComponent();

            //EasyTracker.GetTracker().SendView("ContactInformation");
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            try
            {
                if (!(Window.Current.Content is Frame content) || !content.CanGoBack)
                    return;
                e.put_Handled(true);
                if (LocalSettings.localSettings.editContactType == EditContactType.ContactEdit.ToString())
                {
                    this.put_NavigationCacheMode((NavigationCacheMode)0);
                    content.Navigate(typeof(Settings), (object)"edited");
                }
                else
                {
                    this.put_NavigationCacheMode((NavigationCacheMode)0);
                    content.Navigate(typeof(Settings), (object)"back_pressed");
                }
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

        public ContactInformation()
        {
            // ISSUE: reference to a compiler-generated method
            // ISSUE: reference to a compiler-generated method
            EasyTracker.GetTracker().SendView("contactInformation");
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
                // ISSUE: method pointer
                WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<DataTransferManager, 
                    DataRequestedEventArgs>>(new Func<TypedEventHandler<DataTransferManager, 
                    DataRequestedEventArgs>, EventRegistrationToken>(dataTransferManager.add_DataRequested), new Action<EventRegistrationToken>(dataTransferManager.remove_DataRequested), new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>((object)this, __methodptr(shareTextHandler)));
                if (e.Parameter != null)
                {
                    if (string.IsNullOrEmpty(e.Parameter.ToString()))
                        return;
                    if (e.Parameter.ToString().StartsWith("back_pressed"))
                    {
                        if (this.WPContact != null)
                            return;
                        if (e.Parameter.ToString().Contains("#"))
                            this.ID = e.Parameter.ToString().Split('#')[1];
                        if (ContactstoreInTouch.contactStore == null)
                        {
                            Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
                            if (result.Item2)
                                await RestoreContactStore.restoreContactStore.restoreContacts((List<string>)null, true);
                        }
                        try
                        {
                            this.WPContact = await ContactstoreInTouch.contactStore.FindContactByIdAsync(this.ID);
                        }
                        catch (Exception ex)
                        {
                            LogFile.Log("Error in reading contact to display from WP. " + ex.Message, EventType.Error);
                        }
                        if (this.WPContact == null)
                            return;
                        IDictionary<string, object> props = await this.WPContact.GetPropertiesAsync();
                        this.avatar = await ContactstoreInTouch.contactStoreInTouch.getManualContactFromWP(this.WPContact, props);
                        await this.displayContactInformation();
                    }
                    else
                    {
                        NavigationParameters navigationParameter = e.Parameter as NavigationParameters;
                        if (navigationParameter.action.StartsWith("avatar"))
                        {
                            if (navigationParameter.action.Contains("#"))
                            {
                                this.ID = navigationParameter.action.Split('#')[1];
                                if (string.IsNullOrEmpty(this.ID))
                                    this.WPContact = new StoredContact(ContactstoreInTouch.contactStore);
                            }
                            if (this.WPContact == null)
                            {
                                if (ContactstoreInTouch.contactStore == null)
                                {
                                    Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
                                    if (result.Item2)
                                        await RestoreContactStore.restoreContactStore.restoreContacts((List<string>)null, true);
                                }
                                try
                                {
                                    this.WPContact = await ContactstoreInTouch.contactStore.FindContactByIdAsync(this.ID);
                                }
                                catch (Exception ex)
                                {
                                    LogFile.Log("Error in reading contact to display from WP. " + ex.Message, EventType.Error);
                                }
                            }
                            if (this.WPContact == null)
                                return;
                            this.clearAllGrids();
                            this.avatar = JsonConvert.DeserializeObject<Avatar>(navigationParameter.data);
                            await this.displayContactInformation();
                            IDictionary<string, object> props = await this.WPContact.GetPropertiesAsync();
                            ContactContactbookManual manualContact = new ContactContactbookManual()
                            {
                                avatars = new List<Avatar>()
                            };
                            manualContact.avatars.Add(this.avatar);
                            Download.download.setManualContactToWP(manualContact, this.WPContact, props);
                            await Download.download.setPhotoFieldsToWP(manualContact.avatars, this.WPContact, manualContact.contact_id_1);
                            await this.WPContact.SaveAsync();
                            this.ID = this.WPContact.Id;
                            LocalSettings.localSettings.editContactType = EditContactType.ContactEdit.ToString();
                            bool isAdded = false;
                            string DBImageHash = string.Empty;
                            DBUpdate DBUpdate = new DBUpdate();
                            bool ignore = false;
                            string MCI_CID = InTouchAppDatabase.InTouchAppDB.readInTouchMCI_CID(LocalSettings.localSettings.MCI, this.ID, out DBImageHash, out ignore);
                            DBUpdate.modifiedContactID = this.ID;
                            DBUpdate.modifiedContactContactHash = string.Empty;
                            DBUpdate.modifiedContactImageHash = string.Empty;
                            if (string.IsNullOrEmpty(MCI_CID))
                            {
                                isAdded = true;
                                MCI_CID = CommonCode.commonCode.createRandomStringID();
                            }
                            DBUpdate.modifiedContactMCI_CID = MCI_CID;
                            await LocalSettings.localSettings.saveContact(this.avatar, MCI_CID);
                            string WPImageHash = ContactstoreInTouch.contactStoreInTouch.getWPImageHash(this.avatar.photo);
                            if (!string.Equals(WPImageHash, DBImageHash))
                            {
                                DBUpdate.modifiedContactDirty = 2;
                                DBUpdate.modifiedContactImageHash = WPImageHash;
                            }
                            else
                            {
                                DBUpdate.modifiedContactDirty = 1;
                                DBUpdate.modifiedContactImageHash = DBImageHash;
                            }
                            DBUpdate.contact = Contacts.getContact(this.avatar.name, this.avatar.organization);
                            List<DBUpdate> DBUpdateList = new List<DBUpdate>();
                            DBUpdateList.Add(DBUpdate);
                            if (isAdded)
                                InTouchAppDatabase.InTouchAppDB.addMultipleDirtyEntries(LocalSettings.localSettings.MCI, DBUpdateList, 1);
                            else
                                InTouchAppDatabase.InTouchAppDB.markModifiedDirtyEntries(LocalSettings.localSettings.MCI, DBUpdateList);
                        }
                        else
                        {
                            if (!navigationParameter.action.StartsWith("listViewItem"))
                                return;
                            LocalSettings.localSettings.editContactType = string.Empty;
                            this.ID = navigationParameter.data;
                            if (ContactstoreInTouch.contactStore == null)
                            {
                                Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
                                if (result.Item2)
                                    await RestoreContactStore.restoreContactStore.restoreContacts((List<string>)null, true);
                            }
                            try
                            {
                                this.WPContact = await ContactstoreInTouch.contactStore.FindContactByIdAsync(this.ID);
                            }
                            catch (Exception ex)
                            {
                                LogFile.Log("Error in reading contact to display from WP. " + ex.Message, EventType.Error);
                            }
                            if (this.WPContact != null)
                            {
                                IDictionary<string, object> props = await this.WPContact.GetPropertiesAsync();
                                this.avatar = await ContactstoreInTouch.contactStoreInTouch.getManualContactFromWP(this.WPContact, props);
                                await this.displayContactInformation();
                            }
                            else
                            {
                                Frame content = (Frame)Window.Current.Content;
                                this.put_NavigationCacheMode((NavigationCacheMode)0);
                                content.Navigate(typeof(Settings), (object)"back_pressed");
                            }
                        }
                    }
                }
                else
                {
                    Frame content = (Frame)Window.Current.Content;
                    this.put_NavigationCacheMode((NavigationCacheMode)0);
                    content.Navigate(typeof(Settings), (object)"back_pressed");
                }
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in displaying contact. " + ex.Message, EventType.Warning);
            }
        }

        private async Task displayContactInformation()
        {
            try
            {
                this.ContactImage.put_ImageSource((ImageSource)await ContactstoreInTouch.contactStoreInTouch.getPhotoFromAvatar(this.avatar.photo));
                ContactContactbookManual manualContact = new ContactContactbookManual()
                {
                    avatars = new List<Avatar>()
                };
                manualContact.avatars.Add(this.avatar);
                this.ContactName.put_Text(CommonCode.commonCode.getDisplayName(manualContact.avatars, (Name)null));
                bool isOrganizationAssigned = false;
                if (this.avatar.organization != null)
                {
                    foreach (Organization organization in this.avatar.organization)
                    {
                        isOrganizationAssigned = true;
                        this.ContactOrganizationInfo.put_Text(ContactSample.getOrganizationInfo(organization.company, organization.position));
                    }
                }
                if (!isOrganizationAssigned)
                    this.ContactOrganizationInfo.put_Text(string.Empty);
                this.displayPhone();
                this.displayEmail();
                this.displayWebsite();
                this.displayAddress();
                this.displayEvent();
                this.displayNotes();
                if (!ServerConnectionManager.mIsDeveloper)
                    return;
                this.displayDebugData();
            }
            catch
            {
            }
        }

        private void clearAllGrids()
        {
            try
            {
                ((UIElement)this.MobilePhoneGrid).put_Visibility((Visibility)1);
                ((UIElement)this.MobilePhoneGrid2).put_Visibility((Visibility)1);
                ((UIElement)this.HomePhoneGrid).put_Visibility((Visibility)1);
                ((UIElement)this.HomePhoneGrid2).put_Visibility((Visibility)1);
                ((UIElement)this.HomeFaxPhoneGrid).put_Visibility((Visibility)1);
                ((UIElement)this.WorkFaxPhoneGrid).put_Visibility((Visibility)1);
                ((UIElement)this.CompanyPhoneGrid).put_Visibility((Visibility)1);
                ((UIElement)this.WorkPhoneGrid).put_Visibility((Visibility)1);
                ((UIElement)this.WorkPhoneGrid2).put_Visibility((Visibility)1);
                ((UIElement)this.PersonalEmailGrid).put_Visibility((Visibility)1);
                ((UIElement)this.WorkEmailGrid).put_Visibility((Visibility)1);
                ((UIElement)this.OtherEmailGrid).put_Visibility((Visibility)1);
                ((UIElement)this.WebsiteGrid).put_Visibility((Visibility)1);
                ((UIElement)this.WebsiteGrid1).put_Visibility((Visibility)1);
                ((UIElement)this.WebsiteGrid2).put_Visibility((Visibility)1);
                ((UIElement)this.WebsiteGrid3).put_Visibility((Visibility)1);
                ((UIElement)this.WebsiteGrid4).put_Visibility((Visibility)1);
                ((UIElement)this.WebsiteGrid5).put_Visibility((Visibility)1);
                ((UIElement)this.WebsiteGrid6).put_Visibility((Visibility)1);
                ((UIElement)this.HomeAddressGrid).put_Visibility((Visibility)1);
                ((UIElement)this.WorkAddressGrid).put_Visibility((Visibility)1);
                ((UIElement)this.OtherAddressGrid).put_Visibility((Visibility)1);
                ((UIElement)this.BirthdayGrid).put_Visibility((Visibility)1);
                ((UIElement)this.AnniversaryGrid).put_Visibility((Visibility)1);
                ((UIElement)this.NotesGrid).put_Visibility((Visibility)1);
                ((UIElement)this.DebugGrid).put_Visibility((Visibility)1);
            }
            catch
            {
            }
        }

        private void displayPhone()
        {
            try
            {
                if (this.avatar.phone == null)
                    return;
                foreach (InTouchLibrary.Phone phone in this.avatar.phone)
                {
                    if (!string.IsNullOrEmpty(phone.label) && !string.IsNullOrEmpty(phone.number))
                        this.decidePhoneField(phone.label, phone.number);
                }
            }
            catch
            {
            }
        }

        private void decidePhoneField(string label, string number)
        {
            try
            {
                switch (label.ToLower())
                {
                    case "mobile":
                        ((UIElement)this.MobilePhoneGrid).put_Visibility((Visibility)0);
                        ((UIElement)this.MobilePhone_TBlock).put_Visibility((Visibility)0);
                        this.MobilePhone_TBlock.put_Text(number);
                        ((FrameworkElement)this.MobilePhone_TBlock).put_Tag((object)number);
                        ((FrameworkElement)this.MobilePhone_TBlock_Header).put_Tag((object)number);
                        ((FrameworkElement)this.MobileMessage_Img).put_Tag((object)number);
                        ((FrameworkElement)this.Mobile_Img).put_Tag((object)number);
                        break;
                    case "mobile 2":
                        ((UIElement)this.MobilePhoneGrid2).put_Visibility((Visibility)0);
                        this.MobilePhone_TBlock2.put_Text(number);
                        ((FrameworkElement)this.MobilePhone_TBlock2).put_Tag((object)number);
                        ((FrameworkElement)this.MobilePhone_TBlock_Header2).put_Tag((object)number);
                        ((FrameworkElement)this.MobileMessage_Img2).put_Tag((object)number);
                        ((FrameworkElement)this.Mobile_Img2).put_Tag((object)number);
                        break;
                    case "home":
                        ((UIElement)this.HomePhoneGrid).put_Visibility((Visibility)0);
                        this.HomePhone_TBlock.put_Text(number);
                        ((FrameworkElement)this.HomePhoneGrid).put_Tag((object)number);
                        break;
                    case "home 2":
                        ((UIElement)this.HomePhoneGrid2).put_Visibility((Visibility)0);
                        this.HomePhone_TBlock2.put_Text(number);
                        ((FrameworkElement)this.HomePhoneGrid2).put_Tag((object)number);
                        break;
                    case "work":
                        ((UIElement)this.WorkPhoneGrid).put_Visibility((Visibility)0);
                        this.WorkPhone_TBlock.put_Text(number);
                        ((FrameworkElement)this.WorkPhone_TBlock).put_Tag((object)number);
                        ((FrameworkElement)this.WorkPhone_TBlock_Header).put_Tag((object)number);
                        ((FrameworkElement)this.WorkMessage_Img).put_Tag((object)number);
                        ((FrameworkElement)this.Work_Img).put_Tag((object)number);
                        break;
                    case "work 2":
                        ((UIElement)this.WorkPhoneGrid2).put_Visibility((Visibility)0);
                        this.WorkPhone_TBlock2.put_Text(number);
                        ((FrameworkElement)this.WorkPhone_TBlock2).put_Tag((object)number);
                        ((FrameworkElement)this.WorkPhone_TBlock_Header2).put_Tag((object)number);
                        ((FrameworkElement)this.WorkMessage_Img2).put_Tag((object)number);
                        ((FrameworkElement)this.Work_Img2).put_Tag((object)number);
                        break;
                    case "company":
                        ((UIElement)this.CompanyPhoneGrid).put_Visibility((Visibility)0);
                        this.CompanyPhone_TBlock.put_Text(number);
                        ((FrameworkElement)this.CompanyPhone_TBlock).put_Tag((object)number);
                        ((FrameworkElement)this.CompanyPhone_TBlock_Header).put_Tag((object)number);
                        ((FrameworkElement)this.CompanyMessage_Img).put_Tag((object)number);
                        ((FrameworkElement)this.Company_Img).put_Tag((object)number);
                        break;
                    case "home fax":
                        ((UIElement)this.HomeFaxPhoneGrid).put_Visibility((Visibility)0);
                        this.HomeFaxPhone_TBlock.put_Text(number);
                        ((FrameworkElement)this.HomeFaxPhoneGrid).put_Tag((object)number);
                        break;
                    case "work fax":
                        ((UIElement)this.WorkFaxPhoneGrid).put_Visibility((Visibility)0);
                        this.WorkFax_TBlock.put_Text(number);
                        ((FrameworkElement)this.WorkFaxPhoneGrid).put_Tag((object)number);
                        break;
                }
            }
            catch
            {
            }
        }

        private void displayEmail()
        {
            try
            {
                if (this.avatar.email == null)
                    return;
                foreach (InTouchLibrary.Email email in this.avatar.email)
                {
                    if (!string.IsNullOrEmpty(email.label) && !string.IsNullOrEmpty(email.address))
                        this.decideEmailField(email.label, email.address);
                }
            }
            catch
            {
            }
        }

        private void decideEmailField(string label, string url)
        {
            try
            {
                switch (label.ToLower())
                {
                    case "personal":
                        ((UIElement)this.PersonalEmailGrid).put_Visibility((Visibility)0);
                        this.PersonalEmail.put_Text(url);
                        ((FrameworkElement)this.PersonalEmailGrid).put_Tag((object)url);
                        break;
                    case "work":
                        ((UIElement)this.WorkEmailGrid).put_Visibility((Visibility)0);
                        this.WorkEmail.put_Text(url);
                        ((FrameworkElement)this.WorkEmailGrid).put_Tag((object)url);
                        break;
                    case "other":
                        ((UIElement)this.OtherEmailGrid).put_Visibility((Visibility)0);
                        this.OtherEmail.put_Text(url);
                        ((FrameworkElement)this.OtherEmailGrid).put_Tag((object)url);
                        break;
                }
            }
            catch
            {
            }
        }

        private void displayWebsite()
        {
            try
            {
                if (this.avatar.website == null)
                    return;
                int count = 0;
                foreach (InTouchLibrary.Website website in this.avatar.website)
                {
                    this.decideWebsiteField(count, website.url);
                    ++count;
                }
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in displaying website. " + ex.Message, EventType.Warning);
            }
        }

        private void decideWebsiteField(int count, string url)
        {
            try
            {
                switch (count)
                {
                    case 0:
                        ((UIElement)this.WebsiteGrid).put_Visibility((Visibility)0);
                        ((FrameworkElement)this.WebsiteGrid).put_Tag((object)url);
                        this.Website.put_Text(url);
                        break;
                    case 1:
                        ((UIElement)this.WebsiteGrid1).put_Visibility((Visibility)0);
                        ((FrameworkElement)this.WebsiteGrid1).put_Tag((object)url);
                        this.Website1.put_Text(url);
                        break;
                    case 2:
                        ((UIElement)this.WebsiteGrid2).put_Visibility((Visibility)0);
                        ((FrameworkElement)this.WebsiteGrid2).put_Tag((object)url);
                        this.Website2.put_Text(url);
                        break;
                    case 3:
                        ((UIElement)this.WebsiteGrid3).put_Visibility((Visibility)0);
                        ((FrameworkElement)this.WebsiteGrid3).put_Tag((object)url);
                        this.Website3.put_Text(url);
                        break;
                    case 4:
                        ((UIElement)this.WebsiteGrid4).put_Visibility((Visibility)0);
                        ((FrameworkElement)this.WebsiteGrid4).put_Tag((object)url);
                        this.Website4.put_Text(url);
                        break;
                    case 5:
                        ((UIElement)this.WebsiteGrid5).put_Visibility((Visibility)0);
                        ((FrameworkElement)this.WebsiteGrid5).put_Tag((object)url);
                        this.Website5.put_Text(url);
                        break;
                    case 6:
                        ((UIElement)this.WebsiteGrid6).put_Visibility((Visibility)0);
                        ((FrameworkElement)this.WebsiteGrid6).put_Tag((object)url);
                        this.Website6.put_Text(url);
                        break;
                }
            }
            catch
            {
            }
        }

        private void displayAddress()
        {
            try
            {
                if (this.avatar.address == null)
                    return;
                foreach (Address _address in this.avatar.address)
                {
                    if (!string.IsNullOrEmpty(_address.label))
                        this.decideAddressField(_address.label, _address);
                }
            }
            catch
            {
            }
        }

        private void decideAddressField(string label, Address _address)
        {
            try
            {
                string str = _address.street1;
                if (string.IsNullOrEmpty(str))
                    str = _address.street2;
                else if (!string.IsNullOrEmpty(_address.street2))
                    str = str + Environment.NewLine + _address.street2;
                if (string.IsNullOrEmpty(str))
                    str = _address.street3;
                else if (!string.IsNullOrEmpty(_address.street3))
                    str = str + Environment.NewLine + _address.street3;
                if (string.IsNullOrEmpty(str))
                    str = _address.zip;
                else if (!string.IsNullOrEmpty(_address.zip))
                    str = str + Environment.NewLine + _address.zip;
                if (string.IsNullOrEmpty(str))
                    str = _address.city;
                else if (!string.IsNullOrEmpty(_address.city))
                    str = !string.IsNullOrEmpty(_address.zip) ? str + " " + _address.city : str + Environment.NewLine + _address.city;
                if (string.IsNullOrEmpty(str))
                    str = _address.state;
                else if (!string.IsNullOrEmpty(_address.state))
                    str = !string.IsNullOrEmpty(_address.city) ? str + " " + _address.state : str + Environment.NewLine + _address.state;
                if (string.IsNullOrEmpty(str))
                    str = _address.country;
                else if (!string.IsNullOrEmpty(_address.country))
                    str = str + Environment.NewLine + _address.country;
                if (string.IsNullOrEmpty(str))
                    return;
                switch (label.ToLower())
                {
                    case "personal":
                        ((UIElement)this.HomeAddressGrid).put_Visibility((Visibility)0);
                        this.HomeAdressInfo_TBlock.put_Text(str);
                        break;
                    case "work":
                        ((UIElement)this.WorkAddressGrid).put_Visibility((Visibility)0);
                        this.WorkAdressInfo_TBlock.put_Text(str);
                        break;
                    case "other":
                        ((UIElement)this.OtherAddressGrid).put_Visibility((Visibility)0);
                        this.OtherAdressInfo_TBlock.put_Text(str);
                        break;
                }
            }
            catch
            {
            }
        }

        private void displayNotes()
        {
            try
            {
                if (this.avatar.notes == null)
                    return;
                foreach (Note note in this.avatar.notes)
                {
                    if (!string.IsNullOrEmpty(note.text))
                    {
                        ((UIElement)this.NotesGrid).put_Visibility((Visibility)0);
                        this.Notes_TBlock.put_Text(note.text);
                        break;
                    }
                }
            }
            catch
            {
            }
        }

        private void displayDebugData()
        {
            try
            {
                ((UIElement)this.DebugGrid).put_Visibility((Visibility)0);
                string MCI_CID = string.Empty;
                int Dirty = 0;
                this.device_id_TBlock.put_Text(this.ID);
                InTouchAppDatabase.InTouchAppDB.getDebugData(LocalSettings.localSettings.MCI, this.ID, out MCI_CID, out Dirty);
                this.contact_id_1_TBlock.put_Text(MCI_CID);
                this.Dirty_TBlock.put_Text(((DirtyBit)Dirty).ToString());
            }
            catch
            {
            }
        }

        private void displayEvent()
        {
            try
            {
                if (this.avatar.@event == null)
                    return;
                foreach (Event @event in this.avatar.@event)
                {
                    if (!string.IsNullOrEmpty(@event.type))
                    {
                        DateTime result = new DateTime();
                        string empty = string.Empty;
                        string str;
                        if (DateTime.TryParse(@event.date, out result))
                            str = result.Day.ToString() + " " + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(result.Month) + " " + result.Year.ToString();
                        else
                            str = @event.date;
                        if (string.Equals(@event.type, EventLabel.anniv.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            ((UIElement)this.AnniversaryGrid).put_Visibility((Visibility)0);
                            this.Anniversary_TBlock.put_Text(str);
                        }
                        else if (string.Equals(@event.type, EventLabel.bday.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            ((UIElement)this.BirthdayGrid).put_Visibility((Visibility)0);
                            this.Birthday_TBlock.put_Text(str);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void Phone_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                TextBlock textBlock = new TextBlock();
                Image image = new Image();
                Border border = new Border();
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("contactInformation", "phone_tapped", "user tapped on phone", 0L);
                if (object.ReferenceEquals((object)textBlock.GetType(), (object)sender.GetType()))
                    PhoneCallManager.ShowPhoneCallUI(((FrameworkElement)sender).Tag.ToString(), this.ContactName.Text);
                else if (object.ReferenceEquals((object)image.GetType(), (object)sender.GetType()))
                    PhoneCallManager.ShowPhoneCallUI(((FrameworkElement)sender).Tag.ToString(), this.ContactName.Text);
                else
                    PhoneCallManager.ShowPhoneCallUI(((FrameworkElement)sender).Tag.ToString(), this.ContactName.Text);
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in calling. " + ex.Message, EventType.Warning);
            }
        }

        private async void Message_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("contactInformation", "message_tapped", "user tapped on message", 0L);
                Image MessageImg = (Image)sender;
                ChatMessage msg = new ChatMessage();
                msg.Recipients.Add(((FrameworkElement)MessageImg).Tag.ToString());
                await ChatMessageManager.ShowComposeSmsMessageAsync(msg);
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in sending message. " + ex.Message, EventType.Warning);
            }
        }

        private async void Email_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                EmailMessage mail = new EmailMessage();
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("contactInformation", "email_tapped", "user tapped on email", 0L);
                Border EmailBorder = (Border)sender;
                mail.To.Add(new EmailRecipient(((FrameworkElement)EmailBorder).Tag.ToString(), this.ContactName.Text));
                await EmailManager.ShowComposeNewEmailAsync(mail);
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in sending email. " + ex.Message, EventType.Warning);
            }
        }

        private async void Website_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Border Website = (Border)sender;
                string website = string.Empty;
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("contactInformation", "website_tapped", "user tapped on website", 0L);
                website = ((FrameworkElement)Website).Tag.ToString();
                int num = await Launcher.LaunchUriAsync(new Uri(website, UriKind.Absolute)) ? 1 : 0;
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in opening website. " + ex.Message, EventType.Warning);
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("contactInformation", "appbar_btn_edit_clicked", "user clicked on edit appbar button", 0L);
                if (this.avatar == null)
                    return;
                NavigationParameters navigationParameters = new NavigationParameters();
                navigationParameters.action = "contactInfo#" + this.ContactName.Text;
                navigationParameters.data = JsonConvert.SerializeObject((object)this.avatar, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                Frame content = (Frame)Window.Current.Content;
                this.put_NavigationCacheMode((NavigationCacheMode)1);
                content.Navigate(typeof(EditContact), (object)navigationParameters);
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in edit button click. " + ex.Message, EventType.Warning);
            }
        }

        private void ShareBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("contactInformation", "appbar_btn_share_clicked", "user clicked on share appbar button", 0L);
                DataTransferManager.ShowShareUI();
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in share button click. " + ex.Message, EventType.Warning);
            }
        }

        private void shareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            try
            {
                DataRequest request = e.Request;
                request.Data.Properties.put_Title(this.ContactName.Text);
                string str1 = string.Empty;
                if (this.avatar.phone != null)
                {
                    foreach (InTouchLibrary.Phone phone in this.avatar.phone)
                        str1 = str1 + Environment.NewLine + phone.number + " (" + phone.label + ")";
                }
                if (this.avatar.email != null)
                {
                    foreach (InTouchLibrary.Email email in this.avatar.email)
                        str1 = str1 + Environment.NewLine + email.address + " (" + email.label + ")";
                }
                string str2 = str1 + Environment.NewLine + "-via InTouchApp.com";
                request.Data.SetText(str2);
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in sharing contact info. " + ex.Message, EventType.Warning);
            }
        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("contactInformation", "appbar_btn_delete_clicked", "user clicked on delete appbar button", 0L);
                string message = this.ContactName.Text + " will be deleted from your InTouch account.";
                MessageDialog msgbox = new MessageDialog(message, "Delete Contact?");
                msgbox.Commands.Add((IUICommand)new UICommand("Delete"));
                msgbox.Commands.Add((IUICommand)new UICommand("Cancel"));
                msgbox.put_DefaultCommandIndex(1U);
                msgbox.put_CancelCommandIndex(1U);
                IUICommand msgbox_result = await msgbox.ShowAsync();
                if (!msgbox_result.Label.Equals("Delete"))
                    return;
                try
                {
                    await ContactstoreInTouch.contactStore.DeleteContactAsync(this.ID);
                }
                catch (Exception ex)
                {
                    LogFile.Log("Error in deleting contact " + this.ID + ". " + ex.Message, EventType.Error);
                }
                await InTouchAppDatabase.InTouchAppDB.readAndMarkDeletedDirtyEntries(LocalSettings.localSettings.MCI, new List<string>()
        {
          this.ID
        });
                Frame currentFrame = (Frame)Window.Current.Content;
                this.put_NavigationCacheMode((NavigationCacheMode)0);
                currentFrame.Navigate(typeof(Settings), (object)"edited");
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in delete button click. " + ex.Message, EventType.Warning);
            }
        }//
              
    }
}
