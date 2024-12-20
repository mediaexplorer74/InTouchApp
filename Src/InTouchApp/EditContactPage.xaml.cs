// Decompiled with JetBrains decompiler
// Type: windowsphone_app.EditContact
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;


namespace windowsphone_app
{
    public sealed partial class EditContact : Page
    {
        private string avatar_str = string.Empty;
        private Avatar avatar = new Avatar();
        private string ID = string.Empty;
        private CoreApplicationView view;
        public ObservableCollection<OtherItems> _itemOthers;
     
        public EditContact()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            this._itemOthers = OtherItems.ListOfItems();
            ((FrameworkElement)this).put_DataContext((object)this);
           
            //EasyTracker.GetTracker().SendView("editContact");
        }

        public ObservableCollection<OtherItems> listOfItems
        {
            get => this._itemOthers;
            set => this._itemOthers = value;
        }

        protected virtual void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                this.view = CoreApplication.GetCurrentView();
                if (e.Parameter != null)
                {
                    if (!string.IsNullOrEmpty(e.Parameter.ToString()) && !string.Equals("back_pressed", e.Parameter.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        NavigationParameters parameter = e.Parameter as NavigationParameters;
                        if (parameter.action.StartsWith(EventLabel.bday.ToString()))
                        {
                            if (!string.IsNullOrEmpty(parameter.data))
                            {
                                ((UIElement)this.BirthdayGrid).put_Visibility((Visibility)0);
                                this.Birthdate_TBlock.put_Text(parameter.data);
                                bool flag = false;
                                if (this.avatar.@event != null)
                                {
                                    foreach (Event @event in this.avatar.@event)
                                    {
                                        if (string.Equals(@event.type, parameter.action))
                                        {
                                            flag = true;
                                            @event.date = parameter.data;
                                            break;
                                        }
                                    }
                                }
                                if (!flag)
                                {
                                    Event @event = new Event();
                                    @event.date = parameter.data;
                                    @event.type = parameter.action;
                                    if (this.avatar.@event == null)
                                        this.avatar.@event = new List<Event>();
                                    this.avatar.@event.Add(@event);
                                }
                            }
                        }
                        else if (parameter.action.StartsWith(EventLabel.anniv.ToString()))
                        {
                            if (!string.IsNullOrEmpty(parameter.data))
                            {
                                ((UIElement)this.AnniversaryGrid).put_Visibility((Visibility)0);
                                this.Anniversary_TBlock.put_Text(parameter.data);
                                bool flag = false;
                                if (this.avatar.@event != null)
                                {
                                    foreach (Event @event in this.avatar.@event)
                                    {
                                        if (string.Equals(@event.type, parameter.action))
                                        {
                                            flag = true;
                                            @event.date = parameter.data;
                                            break;
                                        }
                                    }
                                }
                                if (!flag)
                                {
                                    Event @event = new Event();
                                    @event.date = parameter.data;
                                    @event.type = parameter.action;
                                    if (this.avatar.@event == null)
                                        this.avatar.@event = new List<Event>();
                                    this.avatar.@event.Add(@event);
                                }
                            }
                        }
                        else if (parameter.action.StartsWith("Name"))
                        {
                            if (!string.IsNullOrEmpty(parameter.data))
                            {
                                Name name = new Name();
                                this.avatar.name = JsonConvert.DeserializeObject<Name>(parameter.data);
                                string displayName = CommonCode.commonCode.getDisplayName(new List<Avatar>()
                {
                  this.avatar
                }, (Name)null);
                                if (!string.IsNullOrEmpty(displayName))
                                    this.ContactName_TBox.put_Text(displayName);
                                else
                                    this.ContactName_TBox.put_Text(string.Empty);
                            }
                        }
                        else if (parameter.action.StartsWith("Address"))
                        {
                            if (!string.IsNullOrEmpty(parameter.data))
                            {
                                List<Address> addressList = new List<Address>();
                                this.avatar.address = JsonConvert.DeserializeObject<List<Address>>(parameter.data);
                                ((UIElement)this.HomeAddressGrid).put_Visibility((Visibility)1);
                                ((UIElement)this.WorkAddressGrid).put_Visibility((Visibility)1);
                                ((UIElement)this.OtherAddressGrid).put_Visibility((Visibility)1);
                                this.displayAddress();
                            }
                        }
                        else
                        {
                            if (parameter.action.StartsWith("contactInfo"))
                            {
                                if (parameter.action.Contains("#"))
                                    this.ID = parameter.action.Split('#')[1];
                                if (!string.IsNullOrEmpty(parameter.data))
                                    this.avatar_str = parameter.data;
                            }
                            if (!string.IsNullOrEmpty(parameter.data))
                                this.avatar = JsonConvert.DeserializeObject<Avatar>(parameter.data);
                            if (this.avatar != null)
                            {
                                string displayName = CommonCode.commonCode.getDisplayName(new List<Avatar>()
                {
                  this.avatar
                }, (Name)null);
                                if (!string.IsNullOrEmpty(displayName))
                                    this.ContactName_TBox.put_Text(displayName);
                                else
                                    this.ContactName_TBox.put_Text(string.Empty);
                                this.displayPhoto();
                                this.displayPhone();
                                this.displayEmail();
                                this.displayWebsite();
                                this.displayAddress();
                                this.displayEvent();
                                this.displayNotes();
                                this.displayOrganization();
                            }
                        }
                    }
                }
                else
                {
                    ((UIElement)this.MobilePhoneGrid).put_Visibility((Visibility)0);
                    ((UIElement)this.PersonalEmailGrid).put_Visibility((Visibility)0);
                }
                if (!string.IsNullOrEmpty(this.ID))
                    this.PageTitle.put_Text("Edit Contact");
                else
                    this.PageTitle.put_Text("New Contact");
            }
            catch
            {
            }
        }

        private async void displayPhoto()
        {
            try
            {
                this.ContactImage.put_ImageSource((ImageSource)await ContactstoreInTouch.contactStoreInTouch.getPhotoFromAvatar(this.avatar.photo));
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
                if (this.avatar.phone.Count == 7)
                    ((UIElement)this.PhoneOtherGrid).put_Visibility((Visibility)1);
                else
                    ((UIElement)this.PhoneOtherGrid).put_Visibility((Visibility)0);
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
                foreach (Email email in this.avatar.email)
                {
                    if (!string.IsNullOrEmpty(email.label) && !string.IsNullOrEmpty(email.address))
                        this.decideEmailField(email.label, email.address);
                }
                if (this.avatar.email.Count == 3)
                    ((UIElement)this.EmailOtherGrid).put_Visibility((Visibility)1);
                else
                    ((UIElement)this.EmailOtherGrid).put_Visibility((Visibility)0);
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
                string str = string.Empty;
                foreach (Website website in this.avatar.website)
                    str = str + website.url + " ";
                if (string.IsNullOrEmpty(str))
                    return;
                ((UIElement)this.WebsiteGrid).put_Visibility((Visibility)0);
                this.Website_TBox.put_Text(str);
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

        private void displayEvent()
        {
            try
            {
                if (this.avatar.@event == null)
                    return;
                foreach (Event @event in this.avatar.@event)
                {
                    if (string.Equals(@event.type, EventLabel.bday.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        DateTime result = new DateTime();
                        string str = string.Empty;
                        if (DateTime.TryParse(@event.date, out result))
                            str = result.Day.ToString() + "-" + result.Month.ToString() + "-" + result.Year.ToString();
                        if (!string.IsNullOrEmpty(str))
                        {
                            ((UIElement)this.BirthdayGrid).put_Visibility((Visibility)0);
                            this.Birthdate_TBlock.put_Text(str);
                        }
                    }
                    else if (string.Equals(@event.type, EventLabel.anniv.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        DateTime result = new DateTime();
                        string str = string.Empty;
                        if (DateTime.TryParse(@event.date, out result))
                            str = result.Day.ToString() + "-" + result.Month.ToString() + "-" + result.Year.ToString();
                        if (!string.IsNullOrEmpty(str))
                        {
                            ((UIElement)this.AnniversaryGrid).put_Visibility((Visibility)0);
                            this.AnniversaryDate_TBlock.put_Text(str);
                        }
                    }
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
                        this.Notes_TBox.put_Text(note.text);
                        break;
                    }
                }
            }
            catch
            {
            }
        }

        private void displayOrganization()
        {
            try
            {
                if (this.avatar.organization == null)
                    return;
                foreach (Organization organization in this.avatar.organization)
                {
                    if (!string.IsNullOrEmpty(organization.company))
                    {
                        ((UIElement)this.CompanyGrid).put_Visibility((Visibility)0);
                        this.Company_TBox.put_Text(organization.company);
                    }
                    if (!string.IsNullOrEmpty(organization.position))
                    {
                        ((UIElement)this.PositionGrid).put_Visibility((Visibility)0);
                        this.Position_TBox.put_Text(organization.position);
                    }
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
                        this.MobilePhone_TBox.put_Text(number);
                        break;
                    case "mobile 2":
                        ((UIElement)this.MobilePhone2Grid).put_Visibility((Visibility)0);
                        this.Mobile2Phone_TBox.put_Text(number);
                        break;
                    case "home":
                        ((UIElement)this.HomePhoneGrid).put_Visibility((Visibility)0);
                        this.HomePhone_TBox.put_Text(number);
                        break;
                    case "home 2":
                        ((UIElement)this.HomePhone2Grid).put_Visibility((Visibility)0);
                        this.Home2Phone_TBox.put_Text(number);
                        break;
                    case "work":
                        ((UIElement)this.WorkPhoneGrid).put_Visibility((Visibility)0);
                        this.WorkPhone_TBox.put_Text(number);
                        break;
                    case "work 2":
                        ((UIElement)this.WorkPhone2Grid).put_Visibility((Visibility)0);
                        this.Work2Phone_TBox.put_Text(number);
                        break;
                    case "company":
                        ((UIElement)this.CompanyPhoneGrid).put_Visibility((Visibility)0);
                        this.CompanyPhone_TBox.put_Text(number);
                        break;
                    case "home fax":
                        ((UIElement)this.HomeFaxGrid).put_Visibility((Visibility)0);
                        this.HomeFax_TBox.put_Text(number);
                        break;
                    case "work fax":
                        ((UIElement)this.WorkFaxGrid).put_Visibility((Visibility)0);
                        this.WorkFax_TBox.put_Text(number);
                        break;
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
                        this.PersonalEmail_TBox.put_Text(url);
                        break;
                    case "work":
                        ((UIElement)this.WorkEmailGrid).put_Visibility((Visibility)0);
                        this.WorkEmail_TBox.put_Text(url);
                        break;
                    case "other":
                        ((UIElement)this.OtherEmailGrid).put_Visibility((Visibility)0);
                        this.OtherEmail_TBox.put_Text(url);
                        break;
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
        }

        private async void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            try
            {
                if (!(Window.Current.Content is Frame _))
                    return;
                e.put_Handled(true);
                await this.Cancel_Contact();
            }
            catch
            {
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("editContact", "appbar_btn_save_clicked", "user clicked on save contact", 0L);
                this.Save_Contact();
            }
            catch
            {
            }
        }

        private void Save_Contact()
        {
            try
            {
                NavigationParameters navigationParameters = new NavigationParameters();
                navigationParameters.action = "avatar#" + this.ID;
                navigationParameters.data = JsonConvert.SerializeObject((object)this.avatar, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                Frame content = (Frame)Window.Current.Content;
                this.put_NavigationCacheMode((NavigationCacheMode)0);
                content.Navigate(typeof(ContactInformation), (object)navigationParameters);
            }
            catch
            {
            }
        }

        private async Task Cancel_Contact()
        {
            try
            {
                Frame frame = Window.Current.Content as Frame;
                if (string.IsNullOrEmpty(this.avatar_str))
                {
                    this.put_NavigationCacheMode((NavigationCacheMode)0);
                    frame.Navigate(typeof(Settings), (object)"back_pressed");
                }
                else
                {
                    string new_avatar_str = JsonConvert.SerializeObject((object)this.avatar, new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    if (string.Equals(this.avatar_str, new_avatar_str))
                    {
                        this.put_NavigationCacheMode((NavigationCacheMode)0);
                        frame.Navigate(typeof(ContactInformation), (object)("back_pressed#" + this.ID));
                    }
                    else
                    {
                        MessageDialog msgbox = new MessageDialog("You can save your changes now or you can discard them.", "Save  changes?");
                        msgbox.Commands.Add((IUICommand)new UICommand("Save"));
                        msgbox.Commands.Add((IUICommand)new UICommand("Cancel"));
                        IUICommand result = await msgbox.ShowAsync();
                        if (result != null && result.Label == "Save")
                        {
                            // ISSUE: reference to a compiler-generated method
                            // ISSUE: reference to a compiler-generated method
                            EasyTracker.GetTracker().SendEvent("editContact", "messagedialog_btn_save_tapped", "user clicked on save contact", 0L);
                            this.Save_Contact();
                        }
                        else
                        {
                            // ISSUE: reference to a compiler-generated method
                            // ISSUE: reference to a compiler-generated method
                            EasyTracker.GetTracker().SendEvent("editContact", "messagedialog_btn_cancel_tapped", "user clicked on cancel saving contact", 0L);
                            this.put_NavigationCacheMode((NavigationCacheMode)0);
                            frame.Navigate(typeof(ContactInformation), (object)("back_pressed#" + this.ID));
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void ContactName_Img_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Frame content = (Frame)Window.Current.Content;
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("editContact", "name_img_tapped", "user tapped on to edit name", 0L);
                if (this.avatar.name != null)
                {
                    string str = JsonConvert.SerializeObject((object)this.avatar.name, new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    content.Navigate(typeof(EditName), (object)str);
                }
                else
                    content.Navigate(typeof(EditName));
            }
            catch
            {
            }
        }

        private void ContactName_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Frame content = (Frame)Window.Current.Content;
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("editContact", "name_tapped", "user tapped on to edit name", 0L);
                if (this.avatar.name != null)
                {
                    string str = JsonConvert.SerializeObject((object)this.avatar.name, new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    content.Navigate(typeof(EditName), (object)str);
                }
                else
                    content.Navigate(typeof(EditName));
            }
            catch
            {
            }
        }

        private void PhoneOtherGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("editContact", "phoneOther_tapped", "user get available phone labels", 0L);
                if (this._itemOthers == null)
                    this._itemOthers = new ObservableCollection<OtherItems>();
                else
                    this._itemOthers.Clear();
                string empty = string.Empty;
                if (((UIElement)this.MobilePhoneGrid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("Mobile".ToLower()));
                if (((UIElement)this.MobilePhone2Grid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("mobile 2"));
                if (((UIElement)this.HomePhoneGrid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("home"));
                if (((UIElement)this.HomePhone2Grid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("home 2"));
                if (((UIElement)this.WorkPhoneGrid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("work"));
                if (((UIElement)this.WorkPhone2Grid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("work 2"));
                if (((UIElement)this.CompanyPhoneGrid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("company"));
                if (((UIElement)this.HomeFaxGrid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("home fax"));
                if (((UIElement)this.WorkFaxGrid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("work fax"));
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            }
            catch
            {
            }
        }

        private void EmailOtherGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("editContact", "emailOther_tapped", "user get available email labels", 0L);
                if (this._itemOthers == null)
                    this._itemOthers = new ObservableCollection<OtherItems>();
                else
                    this._itemOthers.Clear();
                if (((UIElement)this.PersonalEmailGrid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("personal"));
                if (((UIElement)this.WorkEmailGrid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("work"));
                if (((UIElement)this.OtherEmailGrid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("other"));
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            }
            catch
            {
            }
        }

        private void HomeAddressGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("editContact", "homeAddress_tapped", "user can edit home address", 0L);
                ((Frame)Window.Current.Content).Navigate(typeof(EditAddress), (object)new NavigationParameters()
                {
                    action = "home",
                    data = JsonConvert.SerializeObject((object)this.avatar.address, new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })
                });
            }
            catch
            {
            }
        }

        private void WorkAddressGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("editContact", "workAddress_tapped", "user can edit work address", 0L);
                ((Frame)Window.Current.Content).Navigate(typeof(EditAddress), (object)new NavigationParameters()
                {
                    action = "work",
                    data = JsonConvert.SerializeObject((object)this.avatar.address, new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })
                });
            }
            catch
            {
            }
        }

        private void OtherAddressGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("editContact", "otherAddress_tapped", "user can edit other address", 0L);
                ((Frame)Window.Current.Content).Navigate(typeof(EditAddress), (object)new NavigationParameters()
                {
                    action = "other",
                    data = JsonConvert.SerializeObject((object)this.avatar.address, new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })
                });
            }
            catch
            {
            }
        }

        private void BirthdayGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("editContact", "bday_tapped", "user can edit birthday", 0L);
                ((Frame)Window.Current.Content).Navigate(typeof(EditEvent), (object)new NavigationParameters()
                {
                    action = EventLabel.bday.ToString(),
                    data = this.Birthdate_TBlock.Text
                });
            }
            catch
            {
            }
        }

        private void AnniversaryGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("editContact", "anniv_tapped", "user can edit aniversary", 0L);
                ((Frame)Window.Current.Content).Navigate(typeof(EditEvent), (object)new NavigationParameters()
                {
                    action = EventLabel.anniv.ToString(),
                    data = this.AnniversaryDate_TBlock.Text
                });
            }
            catch
            {
            }
        }

        private async void Cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("editContact", "appbar_btn_cancel_clicked", "user clicked on cancel saving contact", 0L);
                await this.Cancel_Contact();
            }
            catch
            {
            }
        }

        private void OtherGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("editContact", "other_tapped", "user get available other fields", 0L);
                if (this._itemOthers == null)
                    this._itemOthers = new ObservableCollection<OtherItems>();
                else
                    this._itemOthers.Clear();
                if (((UIElement)this.HomeAddressGrid).Visibility != null || ((UIElement)this.WorkAddressGrid).Visibility != null || ((UIElement)this.OtherAddressGrid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("address"));
                if (((UIElement)this.WebsiteGrid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("website"));
                if (((UIElement)this.BirthdayGrid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("birthday"));
                if (((UIElement)this.AnniversaryGrid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("anniversary"));
                if (((UIElement)this.NotesGrid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("notes"));
                if (((UIElement)this.CompanyGrid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("company"));
                if (((UIElement)this.PositionGrid).Visibility != null)
                    this._itemOthers.Add(new OtherItems("job title"));
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            }
            catch
            {
            }
        }

        private void OtherListPickerFlyout_ItemsPicked(
          ListPickerFlyout sender,
          ItemsPickedEventArgs args)
        {
            try
            {
                OtherItems selectedItem = (OtherItems)sender.SelectedItem;
                if (this._itemOthers != null)
                {
                    if (this._itemOthers.Count == 1)
                        ((UIElement)this.OtherGrid).put_Visibility((Visibility)1);
                    else
                        ((UIElement)this.OtherGrid).put_Visibility((Visibility)0);
                }
                if (selectedItem == null)
                    return;
                switch (selectedItem.itemName)
                {
                    case "address":
                        string empty = string.Empty;
                        string str = ((UIElement)this.HomeAddressGrid).Visibility == null ? (((UIElement)this.WorkAddressGrid).Visibility == null ? "other" : "work") : "home";
                        ((Frame)Window.Current.Content).Navigate(typeof(EditAddress), (object)new NavigationParameters()
                        {
                            action = str,
                            data = JsonConvert.SerializeObject((object)this.avatar.address, new JsonSerializerSettings()
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            })
                        });
                        break;
                    case "website":
                        ((UIElement)this.WebsiteGrid).put_Visibility((Visibility)0);
                        break;
                    case "birthday":
                        ((Frame)Window.Current.Content).Navigate(typeof(EditEvent), (object)EventLabel.bday.ToString());
                        break;
                    case "anniversary":
                        ((Frame)Window.Current.Content).Navigate(typeof(EditEvent), (object)EventLabel.anniv.ToString());
                        break;
                    case "notes":
                        ((UIElement)this.NotesGrid).put_Visibility((Visibility)0);
                        break;
                    case "company":
                        ((UIElement)this.CompanyGrid).put_Visibility((Visibility)0);
                        break;
                    case "job title":
                        ((UIElement)this.PositionGrid).put_Visibility((Visibility)0);
                        break;
                }
            }
            catch
            {
            }
        }

        private void EmailListPickerFlyout_ItemsPicked(
          ListPickerFlyout sender,
          ItemsPickedEventArgs args)
        {
            try
            {
                OtherItems selectedItem = (OtherItems)sender.SelectedItem;
                if (this._itemOthers != null)
                {
                    if (this._itemOthers.Count == 1)
                        ((UIElement)this.EmailOtherGrid).put_Visibility((Visibility)1);
                    else
                        ((UIElement)this.EmailOtherGrid).put_Visibility((Visibility)0);
                }
                if (selectedItem == null)
                    return;
                switch (selectedItem.itemName)
                {
                    case "personal":
                        ((UIElement)this.PersonalEmailGrid).put_Visibility((Visibility)0);
                        break;
                    case "work":
                        ((UIElement)this.WorkEmailGrid).put_Visibility((Visibility)0);
                        break;
                    case "other":
                        ((UIElement)this.OtherEmailGrid).put_Visibility((Visibility)0);
                        break;
                }
            }
            catch
            {
            }
        }

        private void PhoneListPickerFlyout_ItemsPicked(
          ListPickerFlyout sender,
          ItemsPickedEventArgs args)
        {
            try
            {
                OtherItems selectedItem = (OtherItems)sender.SelectedItem;
                if (this._itemOthers != null)
                {
                    if (this._itemOthers.Count == 1)
                        ((UIElement)this.PhoneOtherGrid).put_Visibility((Visibility)1);
                    else
                        ((UIElement)this.PhoneOtherGrid).put_Visibility((Visibility)0);
                }
                if (selectedItem == null)
                    return;
                switch (selectedItem.itemName)
                {
                    case "mobile":
                        ((UIElement)this.MobilePhoneGrid).put_Visibility((Visibility)0);
                        break;
                    case "mobile 2":
                        ((UIElement)this.MobilePhone2Grid).put_Visibility((Visibility)0);
                        break;
                    case "home":
                        ((UIElement)this.HomePhoneGrid).put_Visibility((Visibility)0);
                        break;
                    case "home 2":
                        ((UIElement)this.HomePhone2Grid).put_Visibility((Visibility)0);
                        break;
                    case "work":
                        ((UIElement)this.WorkPhoneGrid).put_Visibility((Visibility)0);
                        break;
                    case "work 2":
                        ((UIElement)this.WorkPhone2Grid).put_Visibility((Visibility)0);
                        break;
                    case "company":
                        ((UIElement)this.CompanyPhoneGrid).put_Visibility((Visibility)0);
                        break;
                    case "home fax":
                        ((UIElement)this.HomeFaxGrid).put_Visibility((Visibility)0);
                        break;
                    case "work fax":
                        ((UIElement)this.WorkFaxGrid).put_Visibility((Visibility)0);
                        break;
                }
            }
            catch
            {
            }
        }

        private void ContactName_TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.avatar.name = this.getNameFromTextBox(this.ContactName_TBox.Text);
            }
            catch
            {
            }
        }

        public Name getNameFromTextBox(string nameEdited)
        {
            if (string.IsNullOrEmpty(nameEdited))
            {
                nameEdited = "";
                return (Name)null;
            }
            string[] strArray = nameEdited.Split(new char[1]
            {
        ' '
            }, StringSplitOptions.RemoveEmptyEntries);
            int length = strArray.Length;
            if (strArray == null)
                return (Name)null;
            if (length <= 0)
                return (Name)null;
            List<string> stringList1 = new List<string>()
      {
        "ms",
        "miss",
        "mrs",
        "mr",
        "master",
        "dr",
        "prof",
        "ofc",
        "ms.",
        "miss.",
        "mrs.",
        "mr.",
        "master.",
        "dr.",
        "prof.",
        "ofc."
      };
            List<string> stringList2 = new List<string>()
      {
        "jr",
        "jr.",
        "sr",
        "sr.",
        "Ph.D",
        "Ph.D.",
        "md"
      };
            Name nameFromTextBox = new Name();
            int index1 = 0;
            if (stringList1.Contains(strArray[0].ToLower()))
                nameFromTextBox.prefix = strArray[0];
            if (stringList2.Contains(strArray[length - 1].ToLower()))
                nameFromTextBox.suffix = strArray[length - 1];
            int num1;
            if (nameFromTextBox.prefix != null && nameFromTextBox.suffix != null)
            {
                index1 = 1;
                num1 = length - 1;
            }
            else if (nameFromTextBox.prefix != null)
            {
                index1 = 1;
                num1 = length;
            }
            else if (nameFromTextBox.suffix != null)
            {
                index1 = 0;
                num1 = length - 1;
            }
            else
                num1 = length;
            int num2 = num1 - index1;
            if (num2 == 1)
                nameFromTextBox.given = string.IsNullOrEmpty(strArray[index1]) ? (string)null : strArray[index1];
            else if (num2 == 2)
            {
                nameFromTextBox.given = string.IsNullOrEmpty(strArray[index1]) ? (string)null : strArray[index1];
                nameFromTextBox.family = string.IsNullOrEmpty(strArray[index1 + 1]) ? (string)null : strArray[index1 + 1];
            }
            else if (num2 >= 3)
            {
                int index2 = index1;
                nameFromTextBox.given = string.IsNullOrEmpty(strArray[index2]) ? (string)null : strArray[index2];
                nameFromTextBox.middle = string.IsNullOrEmpty(strArray[index2 + 1]) ? (string)null : strArray[index2 + 1];
                nameFromTextBox.family = string.IsNullOrEmpty(strArray[index2 + 2]) ? (string)null : strArray[index2 + 2];
                for (int index3 = index1 + 3; index3 < num1; ++index3)
                {
                    if (!string.IsNullOrEmpty(nameFromTextBox.family) && !string.IsNullOrEmpty(strArray[index3]))
                        nameFromTextBox.family = nameFromTextBox.family + " " + strArray[index3];
                    else if (string.IsNullOrEmpty(nameFromTextBox.family))
                        nameFromTextBox.family = string.IsNullOrEmpty(strArray[index3]) ? (string)null : strArray[index3];
                }
            }
            return nameFromTextBox;
        }

        private void Position_TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bool flag = false;
                if (this.avatar.organization != null)
                {
                    using (List<Organization>.Enumerator enumerator = this.avatar.organization.GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                        {
                            Organization current = enumerator.Current;
                            flag = true;
                            current.position = this.Position_TBox.Text;
                        }
                    }
                }
                if (flag)
                    return;
                Organization organization = new Organization();
                organization.position = this.Position_TBox.Text;
                if (this.avatar.organization == null)
                    this.avatar.organization = new List<Organization>();
                this.avatar.organization.Add(organization);
            }
            catch
            {
            }
        }

        private void Company_TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bool flag = false;
                if (this.avatar.organization != null)
                {
                    using (List<Organization>.Enumerator enumerator = this.avatar.organization.GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                        {
                            Organization current = enumerator.Current;
                            flag = true;
                            current.company = this.Company_TBox.Text;
                        }
                    }
                }
                if (flag)
                    return;
                Organization organization = new Organization();
                organization.company = this.Company_TBox.Text;
                if (this.avatar.organization == null)
                    this.avatar.organization = new List<Organization>();
                this.avatar.organization.Add(organization);
            }
            catch
            {
            }
        }

        private void Notes_TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                List<Note> noteList = new List<Note>();
                Note note = new Note();
                note.text = this.Notes_TBox.Text;
                if (this.avatar.notes == null)
                    this.avatar.notes = new List<Note>();
                noteList.Add(note);
                this.avatar.notes = noteList;
            }
            catch
            {
            }
        }

        private void Website_TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                List<Website> websiteList = new List<Website>();
                string[] strArray = this.Website_TBox.Text.Split(new char[2]
                {
          ';',
          ' '
                }, StringSplitOptions.RemoveEmptyEntries);
                if (strArray != null)
                {
                    foreach (string str in strArray)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            Website website = new Website();
                            website.url = str;
                            if (this.avatar.website == null)
                                this.avatar.website = new List<Website>();
                            websiteList.Add(website);
                        }
                    }
                }
                this.avatar.website = websiteList;
            }
            catch
            {
            }
        }

        private void OtherEmail_TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bool flag = false;
                if (this.avatar.email != null)
                {
                    foreach (Email email in this.avatar.email)
                    {
                        if (string.Equals(email.label, "Other", StringComparison.OrdinalIgnoreCase))
                        {
                            flag = true;
                            email.address = this.OtherEmail_TBox.Text;
                            break;
                        }
                    }
                }
                if (flag)
                    return;
                Email email1 = new Email();
                email1.address = this.OtherEmail_TBox.Text;
                email1.label = "Other";
                if (this.avatar.email == null)
                    this.avatar.email = new List<Email>();
                this.avatar.email.Add(email1);
            }
            catch
            {
            }
        }

        private void WorkEmail_TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bool flag = false;
                if (this.avatar.email != null)
                {
                    foreach (Email email in this.avatar.email)
                    {
                        if (string.Equals(email.label, "Work", StringComparison.OrdinalIgnoreCase))
                        {
                            flag = true;
                            email.address = this.WorkEmail_TBox.Text;
                            break;
                        }
                    }
                }
                if (flag)
                    return;
                Email email1 = new Email();
                email1.address = this.WorkEmail_TBox.Text;
                email1.label = "Work";
                if (this.avatar.email == null)
                    this.avatar.email = new List<Email>();
                this.avatar.email.Add(email1);
            }
            catch
            {
            }
        }

        private void PersonalEmail_TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bool flag = false;
                if (this.avatar.email != null)
                {
                    foreach (Email email in this.avatar.email)
                    {
                        if (string.Equals(email.label, "Personal", StringComparison.OrdinalIgnoreCase))
                        {
                            flag = true;
                            email.address = this.PersonalEmail_TBox.Text;
                            break;
                        }
                    }
                }
                if (flag)
                    return;
                Email email1 = new Email();
                email1.address = this.PersonalEmail_TBox.Text;
                email1.label = "Personal";
                if (this.avatar.email == null)
                    this.avatar.email = new List<Email>();
                this.avatar.email.Add(email1);
            }
            catch
            {
            }
        }

        private void WorkFax_TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bool flag = false;
                if (this.avatar.phone != null)
                {
                    foreach (InTouchLibrary.Phone phone in this.avatar.phone)
                    {
                        if (string.Equals(phone.label, "Work Fax", StringComparison.OrdinalIgnoreCase))
                        {
                            flag = true;
                            phone.number = this.WorkFax_TBox.Text;
                            break;
                        }
                    }
                }
                if (flag)
                    return;
                InTouchLibrary.Phone phone1 = new InTouchLibrary.Phone();
                phone1.number = this.WorkFax_TBox.Text;
                phone1.label = "Work Fax";
                if (this.avatar.phone == null)
                    this.avatar.phone = new List<InTouchLibrary.Phone>();
                this.avatar.phone.Add(phone1);
            }
            catch
            {
            }
        }

        private void HomeFax_TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bool flag = false;
                if (this.avatar.phone != null)
                {
                    foreach (InTouchLibrary.Phone phone in this.avatar.phone)
                    {
                        if (string.Equals(phone.label, "Home Fax", StringComparison.OrdinalIgnoreCase))
                        {
                            flag = true;
                            phone.number = this.HomeFax_TBox.Text;
                            break;
                        }
                    }
                }
                if (flag)
                    return;
                InTouchLibrary.Phone phone1 = new InTouchLibrary.Phone();
                phone1.number = this.HomeFax_TBox.Text;
                phone1.label = "Home Fax";
                if (this.avatar.phone == null)
                    this.avatar.phone = new List<InTouchLibrary.Phone>();
                this.avatar.phone.Add(phone1);
            }
            catch
            {
            }
        }

        private void CompanyPhone_TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bool flag = false;
                if (this.avatar.phone != null)
                {
                    foreach (InTouchLibrary.Phone phone in this.avatar.phone)
                    {
                        if (string.Equals(phone.label, "Company", StringComparison.OrdinalIgnoreCase))
                        {
                            flag = true;
                            phone.number = this.CompanyPhone_TBox.Text;
                            break;
                        }
                    }
                }
                if (flag)
                    return;
                InTouchLibrary.Phone phone1 = new InTouchLibrary.Phone();
                phone1.number = this.CompanyPhone_TBox.Text;
                phone1.label = "Company";
                if (this.avatar.phone == null)
                    this.avatar.phone = new List<InTouchLibrary.Phone>();
                this.avatar.phone.Add(phone1);
            }
            catch
            {
            }
        }

        private void Work2Phone_TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bool flag = false;
                if (this.avatar.phone != null)
                {
                    foreach (InTouchLibrary.Phone phone in this.avatar.phone)
                    {
                        if (string.Equals(phone.label, "Work 2", StringComparison.OrdinalIgnoreCase))
                        {
                            flag = true;
                            phone.number = this.Work2Phone_TBox.Text;
                            break;
                        }
                    }
                }
                if (flag)
                    return;
                InTouchLibrary.Phone phone1 = new InTouchLibrary.Phone();
                phone1.number = this.Work2Phone_TBox.Text;
                phone1.label = "Work 2";
                if (this.avatar.phone == null)
                    this.avatar.phone = new List<InTouchLibrary.Phone>();
                this.avatar.phone.Add(phone1);
            }
            catch
            {
            }
        }

        private void Home2Phone_TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bool flag = false;
                if (this.avatar.phone != null)
                {
                    foreach (InTouchLibrary.Phone phone in this.avatar.phone)
                    {
                        if (string.Equals(phone.label, "Home 2", StringComparison.OrdinalIgnoreCase))
                        {
                            flag = true;
                            phone.number = this.Home2Phone_TBox.Text;
                            break;
                        }
                    }
                }
                if (flag)
                    return;
                InTouchLibrary.Phone phone1 = new InTouchLibrary.Phone();
                phone1.number = this.Home2Phone_TBox.Text;
                phone1.label = "Home 2";
                if (this.avatar.phone == null)
                    this.avatar.phone = new List<InTouchLibrary.Phone>();
                this.avatar.phone.Add(phone1);
            }
            catch
            {
            }
        }

        private void HomePhone_TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bool flag = false;
                if (this.avatar.phone != null)
                {
                    foreach (InTouchLibrary.Phone phone in this.avatar.phone)
                    {
                        if (string.Equals(phone.label, "Home", StringComparison.OrdinalIgnoreCase))
                        {
                            flag = true;
                            phone.number = this.HomePhone_TBox.Text;
                            break;
                        }
                    }
                }
                if (flag)
                    return;
                InTouchLibrary.Phone phone1 = new InTouchLibrary.Phone();
                phone1.number = this.HomePhone_TBox.Text;
                phone1.label = "Home";
                if (this.avatar.phone == null)
                    this.avatar.phone = new List<InTouchLibrary.Phone>();
                this.avatar.phone.Add(phone1);
            }
            catch
            {
            }
        }

        private void Mobile2Phone_TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bool flag = false;
                if (this.avatar.phone != null)
                {
                    foreach (InTouchLibrary.Phone phone in this.avatar.phone)
                    {
                        if (string.Equals(phone.label, "Mobile 2", StringComparison.OrdinalIgnoreCase))
                        {
                            flag = true;
                            phone.number = this.Mobile2Phone_TBox.Text;
                            break;
                        }
                    }
                }
                if (flag)
                    return;
                InTouchLibrary.Phone phone1 = new InTouchLibrary.Phone();
                phone1.number = this.Mobile2Phone_TBox.Text;
                phone1.label = "Mobile 2";
                if (this.avatar.phone == null)
                    this.avatar.phone = new List<InTouchLibrary.Phone>();
                this.avatar.phone.Add(phone1);
            }
            catch
            {
            }
        }

        private void MobilePhone_TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bool flag = false;
                if (this.avatar.phone != null)
                {
                    foreach (InTouchLibrary.Phone phone in this.avatar.phone)
                    {
                        if (string.Equals(phone.label, "Mobile", StringComparison.OrdinalIgnoreCase))
                        {
                            flag = true;
                            phone.number = this.MobilePhone_TBox.Text;
                            break;
                        }
                    }
                }
                if (flag)
                    return;
                InTouchLibrary.Phone phone1 = new InTouchLibrary.Phone();
                phone1.number = this.MobilePhone_TBox.Text;
                phone1.label = "Mobile";
                if (this.avatar.phone == null)
                    this.avatar.phone = new List<InTouchLibrary.Phone>();
                this.avatar.phone.Add(phone1);
            }
            catch
            {
            }
        }

        private void WorkPhone_TBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bool flag = false;
                if (this.avatar.phone != null)
                {
                    foreach (InTouchLibrary.Phone phone in this.avatar.phone)
                    {
                        if (string.Equals(phone.label, "Work", StringComparison.OrdinalIgnoreCase))
                        {
                            flag = true;
                            phone.number = this.WorkPhone_TBox.Text;
                            break;
                        }
                    }
                }
                if (flag)
                    return;
                InTouchLibrary.Phone phone1 = new InTouchLibrary.Phone();
                phone1.number = this.WorkPhone_TBox.Text;
                phone1.label = "Work";
                if (this.avatar.phone == null)
                    this.avatar.phone = new List<InTouchLibrary.Phone>();
                this.avatar.phone.Add(phone1);
            }
            catch
            {
            }
        }

        private void ContactDisplayImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("editContact", "img_displayImage_tapped", "user tapped on display image to change it", 0L);
                FileOpenPicker fileOpenPicker = new FileOpenPicker();
                fileOpenPicker.put_SuggestedStartLocation((PickerLocationId)6);
                fileOpenPicker.put_ViewMode((PickerViewMode)1);
                fileOpenPicker.FileTypeFilter.Clear();
                fileOpenPicker.FileTypeFilter.Add(".bmp");
                fileOpenPicker.FileTypeFilter.Add(".png");
                fileOpenPicker.FileTypeFilter.Add(".jpeg");
                fileOpenPicker.FileTypeFilter.Add(".jpg");
                fileOpenPicker.PickSingleFileAndContinue();
                CoreApplicationView view = this.view;
                // ISSUE: method pointer
                WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<CoreApplicationView, IActivatedEventArgs>>(new Func<TypedEventHandler<CoreApplicationView, IActivatedEventArgs>, EventRegistrationToken>(view.add_Activated), new Action<EventRegistrationToken>(view.remove_Activated), new TypedEventHandler<CoreApplicationView, IActivatedEventArgs>((object)this, __methodptr(viewActivated)));
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in contact display image tap. " + ex.Message, EventType.Error);
            }
        }

        private async void viewActivated(CoreApplicationView sender, IActivatedEventArgs args1)
        {
            try
            {
                if (!(args1 is FileOpenPickerContinuationEventArgs args) || args.Files.Count == 0)
                    return;
                // ISSUE: method pointer
                WindowsRuntimeMarshal.RemoveEventHandler<TypedEventHandler<CoreApplicationView, IActivatedEventArgs>>(new Action<EventRegistrationToken>(this.view.remove_Activated), new TypedEventHandler<CoreApplicationView, IActivatedEventArgs>((object)this, __methodptr(viewActivated)));
                StorageFile storageFile = args.Files[0];
                IRandomAccessStream stream = await storageFile.OpenAsync((FileAccessMode)0);
                MemoryStream ms = new MemoryStream();
                Stream stream1 = stream.AsStream();
                stream1.CopyTo((Stream)ms);
                byte[] imageArray = ms.ToArray();
                List<Photo> photoList = new List<Photo>();
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                photoList.Add(new Photo()
                {
                    data = base64ImageRepresentation
                });
                this.avatar.photo = photoList;
                this.ContactImage.put_ImageSource((ImageSource)await ContactstoreInTouch.contactStoreInTouch.getPhotoFromAvatar(this.avatar.photo));
            }
            catch (Exception ex)
            {
                LogFile.Log("Problem in changing display image. " + ex.Message, EventType.Error);
            }
        }

       
    }
}
