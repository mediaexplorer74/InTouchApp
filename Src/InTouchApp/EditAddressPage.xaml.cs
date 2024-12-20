// Decompiled with JetBrains decompiler
// Type: windowsphone_app.EditAddress
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
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Navigation;


namespace windowsphone_app
{
    public sealed partial class EditAddress : Page
    {
        private string addressType = string.Empty;
        private List<Address> addressList = new List<Address>();
      

        public EditAddress()
        {
            this.InitializeComponent();
        
            //EasyTracker.GetTracker().SendView("editAddress");
        }

        protected virtual void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (e.Parameter == null)
                    return;
                NavigationParameters parameter = e.Parameter as NavigationParameters;
                this.addressType = parameter.action;
                this.addressList = JsonConvert.DeserializeObject<List<Address>>(parameter.data);
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                if (this.addressType.StartsWith("home"))
                    comboBoxItem = ((FrameworkElement)this.AddressType_ComboBox).FindName("Home") as ComboBoxItem;
                else if (this.addressType.StartsWith("work"))
                    comboBoxItem = ((FrameworkElement)this.AddressType_ComboBox).FindName("Work") as ComboBoxItem;
                else if (this.addressType.StartsWith("other"))
                    comboBoxItem = ((FrameworkElement)this.AddressType_ComboBox).FindName("Other") as ComboBoxItem;
                ((Selector)this.AddressType_ComboBox).put_SelectedItem((object)comboBoxItem);
                if (string.Equals(this.addressType, "home", StringComparison.OrdinalIgnoreCase))
                    this.addressType = "personal";
                if (this.addressList == null)
                    return;
                foreach (Address address in this.addressList)
                {
                    if (!string.IsNullOrEmpty(address.label))
                    {
                        if (string.Equals(this.addressType, address.label, StringComparison.OrdinalIgnoreCase))
                        {
                            this.assignAddress(address);
                        }
                        else
                        {
                            string str = string.Join(" ", (address.street1 + " " + address.street2 + " " + address.street3).Split(new char[1]
                            {
                ' '
                            }, StringSplitOptions.RemoveEmptyEntries));
                            switch (address.label.ToLower())
                            {
                                case "personal":
                                    ((ContentControl)this.Home).put_Content((object)("home: " + str));
                                    continue;
                                case "work":
                                    ((ContentControl)this.Work).put_Content((object)("work: " + str));
                                    continue;
                                case "other":
                                    ((ContentControl)this.Other).put_Content((object)("other: " + str));
                                    continue;
                                default:
                                    continue;
                            }
                        }
                    }
                }
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

        private void assignAddress(Address _address)
        {
            try
            {
                if (!string.IsNullOrEmpty(_address.street1) || !string.IsNullOrEmpty(_address.street2) || !string.IsNullOrEmpty(_address.street3))
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
                    this.Street_TBox.put_Text(str);
                }
                if (!string.IsNullOrEmpty(_address.city))
                    this.Town_TBox.put_Text(_address.city);
                if (!string.IsNullOrEmpty(_address.state))
                    this.County_TBox.put_Text(_address.state);
                if (!string.IsNullOrEmpty(_address.zip))
                    this.Postcode_TBox.put_Text(_address.zip);
                if (string.IsNullOrEmpty(_address.country))
                    return;
                this.Country_TBox.put_Text(_address.country);
            }
            catch
            {
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("editAddress", "appbar_btn_save_clicked", "user clicked on save address", 0L);
                ComboBoxItem item = ((Selector)this.AddressType_ComboBox).SelectedItem as ComboBoxItem;
                string content = ((ContentControl)item).Content.ToString();
                if (content.Contains(":"))
                {
                    string new_address_type = string.Empty;
                    MessageDialog msgbox = new MessageDialog("You already have an address saved as this address type. Do you want tp replace it with the new address?", "Replace address?");
                    msgbox.Commands.Add((IUICommand)new UICommand("Replace"));
                    msgbox.Commands.Add((IUICommand)new UICommand("Cancel"));
                    IUICommand result = await msgbox.ShowAsync();
                    if (result == null || !(result.Label == "Replace"))
                        return;
                    new_address_type = content.Split(':')[0];
                    if (string.Equals(new_address_type, "home", StringComparison.OrdinalIgnoreCase))
                        new_address_type = "personal";
                    List<Address> final_address = new List<Address>();
                    if (this.addressList != null)
                    {
                        foreach (Address address1 in this.addressList)
                        {
                            if (!string.IsNullOrEmpty(address1.label) && !string.Equals(address1.label, this.addressType, StringComparison.OrdinalIgnoreCase))
                            {
                                if (string.Equals(address1.label, new_address_type, StringComparison.OrdinalIgnoreCase))
                                {
                                    Address address2 = this.getAddress();
                                    address2.label = address1.label;
                                    final_address.Add(address2);
                                }
                                else
                                {
                                    Address address3 = address1;
                                    final_address.Add(address3);
                                }
                            }
                        }
                    }
                    this.saveAddress(final_address);
                }
                else
                {
                    List<Address> final_address = new List<Address>();
                    bool flag = false;
                    if (this.addressList != null)
                    {
                        foreach (Address address4 in this.addressList)
                        {
                            if (string.Equals(address4.label, this.addressType, StringComparison.OrdinalIgnoreCase))
                            {
                                flag = true;
                                Address address5 = this.getAddress();
                                switch (content.ToString())
                                {
                                    case "home":
                                        address5.label = "Personal";
                                        break;
                                    case "work":
                                        address5.label = "Work";
                                        break;
                                    case "other":
                                        address5.label = "Other";
                                        break;
                                }
                                final_address.Add(address5);
                            }
                            else if (!string.IsNullOrEmpty(address4.label))
                            {
                                Address address6 = address4;
                                final_address.Add(address6);
                            }
                        }
                    }
                    if (!flag)
                    {
                        Address address = this.getAddress();
                        switch (content.ToString())
                        {
                            case "home":
                                address.label = "Personal";
                                break;
                            case "work":
                                address.label = "Work";
                                break;
                            case "other":
                                address.label = "Other";
                                break;
                        }
                        final_address.Add(address);
                    }
                    this.saveAddress(final_address);
                }
            }
            catch
            {
            }
        }

        private Address getAddress()
        {
            try
            {
                Address address = new Address();
                if (!string.IsNullOrEmpty(this.Street_TBox.Text))
                {
                    string[] strArray = this.Street_TBox.Text.Split(new char[1]
                    {
            '\n'
                    }, StringSplitOptions.RemoveEmptyEntries);
                    switch (strArray.Length)
                    {
                        case 1:
                            address.street1 = strArray[0];
                            break;
                        case 2:
                            address.street2 = strArray[1];
                            goto case 1;
                        case 3:
                            address.street3 = strArray[2];
                            goto case 2;
                    }
                }
                address.city = this.Town_TBox.Text;
                address.state = this.County_TBox.Text;
                address.zip = this.Postcode_TBox.Text;
                address.country = this.Country_TBox.Text;
                return address;
            }
            catch
            {
                throw;
            }
        }

        private void saveAddress(List<Address> final_address)
        {
            try
            {
                ((Frame)Window.Current.Content).Navigate(typeof(EditContact), (object)new NavigationParameters()
                {
                    action = "Address",
                    data = JsonConvert.SerializeObject((object)final_address, new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })
                });
            }
            catch
            {
                throw;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated method
                EasyTracker.GetTracker().SendEvent("editAddress", "appbar_btn_cancel_clicked", "user clicked on cancel saving address", 0L);
                ((Frame)Window.Current.Content).Navigate(typeof(EditContact), (object)"back_pressed");
            }
            catch
            {
            }
        }//
               
       
    }
}
