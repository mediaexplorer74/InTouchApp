// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.ContactstoreInTouch
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using GoogleAnalytics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Phone.PersonalInformation;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

#nullable disable
namespace InTouchLibrary
{
  public class ContactstoreInTouch
  {
    private static volatile ContactstoreInTouch _contactStoreInTouch;
    public static ContactStore contactStore;

    public static ContactstoreInTouch contactStoreInTouch
    {
      get
      {
        try
        {
          if (ContactstoreInTouch._contactStoreInTouch == null)
            ContactstoreInTouch._contactStoreInTouch = new ContactstoreInTouch();
          return ContactstoreInTouch._contactStoreInTouch;
        }
        catch
        {
          throw;
        }
      }
    }

    public void setContactStore() => ContactstoreInTouch.contactStore = (ContactStore) null;

    public async Task<Tuple<bool, bool>> GetContactStore()
    {
      Tuple<bool, bool> contactStore;
      try
      {
        bool storeDoesntExists = false;
        bool restored = false;
        for (int i = 0; i < 3; ++i)
        {
          try
          {
            ContactstoreInTouch.contactStore = await ContactStore.CreateOrOpenAsync((ContactStoreSystemAccessMode) 1, (ContactStoreApplicationAccessMode) 1);
            if (ContactstoreInTouch.contactStore != null)
            {
              ContactQueryResult QueryResult = ContactstoreInTouch.contactStore.CreateContactQuery();
              IReadOnlyList<StoredContact> contactList = (IReadOnlyList<StoredContact>) null;
              if (QueryResult != null)
              {
                contactList = await QueryResult.GetContactsAsync();
                if (contactList != null)
                {
                  if (contactList.Count == 0 && InTouchAppDatabase.InTouchAppDB.countInTouchEntriesFromLT(LocalSettings.localSettings.MCI) != 0)
                  {
                    restored = true;
                    break;
                  }
                }
                else
                  storeDoesntExists = true;
              }
              else
                storeDoesntExists = true;
              if (storeDoesntExists)
              {
                if (InTouchAppDatabase.InTouchAppDB.countInTouchEntriesFromLT(LocalSettings.localSettings.MCI) != 0)
                {
                  restored = true;
                  break;
                }
                break;
              }
              break;
            }
          }
          catch (Exception ex)
          {
            if (i == 3)
              throw;
          }
        }
        contactStore = Tuple.Create<bool, bool>(storeDoesntExists, restored);
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in retrieving contactstore. " + ex.Message, EventType.Error);
        throw;
      }
      return contactStore;
    }

    public async Task getStore()
    {
      ContactstoreInTouch.contactStore = await ContactStore.CreateOrOpenAsync((ContactStoreSystemAccessMode) 1, (ContactStoreApplicationAccessMode) 1);
    }

    public async Task setRevisionNumber()
    {
      try
      {
        if (ContactstoreInTouch.contactStore == null)
        {
          Tuple<bool, bool> result = await this.GetContactStore();
          if (result.Item2)
            await RestoreContactStore.restoreContactStore.restoreContacts((List<string>) null, true);
        }
        LocalSettings.localSettings.revisionNumber = Convert.ToInt32(ContactstoreInTouch.contactStore.RevisionNumber);
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in contactStore. " + ex.Message, EventType.Error);
      }
    }

    public async Task alterLookupTable()
    {
      try
      {
        bool columnExists = InTouchAppDatabase.InTouchAppDB.checkforColumnAlterTable(LocalSettings.localSettings.MCI);
        if (columnExists && !LocalSettings.localSettings.updateDBWithValues)
          return;
        LocalSettings.localSettings.updateDBWithValues = true;
        if (ContactstoreInTouch.contactStore == null)
        {
          Tuple<bool, bool> result = await this.GetContactStore();
          if (result.Item2)
            await RestoreContactStore.restoreContactStore.restoreContacts((List<string>) null, true);
        }
        ContactQueryResult QueryResult = ContactstoreInTouch.contactStore.CreateContactQuery();
        IReadOnlyList<StoredContact> _contactList = (IReadOnlyList<StoredContact>) null;
        if (QueryResult == null)
          return;
        _contactList = await QueryResult.GetContactsAsync();
        if (_contactList == null)
          return;
        List<Contacts> contactList = new List<Contacts>();
        List<string> IDList = new List<string>();
        foreach (StoredContact storedContact in (IEnumerable<StoredContact>) _contactList)
        {
          Contacts contact = new Contacts();
          IDictionary<string, object> props = await storedContact.GetPropertiesAsync();
          Name name = new Name();
          string CompanyName = string.Empty;
          string JobTitle = string.Empty;
          this.getContactInfoFromWP(storedContact, props, out name, out CompanyName, out JobTitle);
          contact.modifiedContactNameFamily = name.family;
          contact.modifiedContactNameGiven = name.given;
          contact.modifiedContactNameMiddle = name.middle;
          contact.modifiedContactNameNickname = name.nickname;
          contact.modifiedContactNamePrefix = name.prefix;
          contact.modifiedContactNameSuffix = name.suffix;
          contact.modifiedContactCompanyName = CompanyName;
          contact.modifiedContactJobTitle = JobTitle;
          contactList.Add(contact);
          IDList.Add(storedContact.Id);
        }
        InTouchAppDatabase.InTouchAppDB.updateDBForContacts(LocalSettings.localSettings.MCI, contactList, IDList);
        LocalSettings.localSettings.updateDBWithValues = false;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in finding changes in WP InTouch contacts. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public async Task<Tuple<List<string>, List<Avatar>>> findChangesInWPInTouchContacts()
    {
      Tuple<List<string>, List<Avatar>> wpInTouchContacts;
      try
      {
        await this.alterLookupTable();
        if (ContactstoreInTouch.contactStore == null)
        {
          Tuple<bool, bool> result = await this.GetContactStore();
          if (result.Item2)
            await RestoreContactStore.restoreContactStore.restoreContacts((List<string>) null, true);
        }
        List<DBUpdate> UpdateModifiedContactList = new List<DBUpdate>();
        List<DBUpdate> UpdateAddedContactList = new List<DBUpdate>();
        List<string> modifiedID = new List<string>();
        List<Avatar> modifiedAvatar = new List<Avatar>();
        int ContactStoreRevisionNumber = Convert.ToInt32(ContactstoreInTouch.contactStore.RevisionNumber);
        bool ShallRestore = false;
        List<string> markDeletedDirtyMCI_CIDList = new List<string>();
        if (LocalSettings.localSettings.revisionNumber != ContactStoreRevisionNumber)
        {
          IReadOnlyList<ContactChangeRecord> result = await ContactstoreInTouch.contactStore.GetChangesAsync((ulong) LocalSettings.localSettings.revisionNumber);
          if (result != null)
          {
            foreach (ContactChangeRecord record in (IEnumerable<ContactChangeRecord>) result)
            {
              DBUpdate DBUpdate = new DBUpdate();
              string Id = record.Id;
              ContactChangeType ChangeType = record.ChangeType;
              string DBImageHash = string.Empty;
              bool isImageAlreadyModified = false;
              string MCI_CID = InTouchAppDatabase.InTouchAppDB.readInTouchMCI_CID(LocalSettings.localSettings.MCI, Id, out DBImageHash, out isImageAlreadyModified);
              DBUpdate.modifiedContactID = Id;
              DBUpdate.modifiedContactContactHash = string.Empty;
              DBUpdate.modifiedContactImageHash = string.Empty;
              if (ChangeType == null || ChangeType == 1)
              {
                bool isCreated = false;
                if (ChangeType == null && string.IsNullOrEmpty(MCI_CID))
                {
                  MCI_CID = CommonCode.commonCode.createRandomStringID();
                  DBUpdate.modifiedContactMCI_CID = MCI_CID;
                  isCreated = true;
                }
                else
                  DBUpdate.modifiedContactMCI_CID = MCI_CID;
                StoredContact WPContact = (StoredContact) null;
                try
                {
                  WPContact = await ContactstoreInTouch.contactStore.FindContactByIdAsync(Id);
                }
                catch (Exception ex)
                {
                  LogFile.Log("Error in reading contact from WP. " + ex.Message, EventType.Error);
                }
                if (WPContact != null)
                {
                  IDictionary<string, object> props = await WPContact.GetPropertiesAsync();
                  Avatar jsonAvatar = await ContactstoreInTouch.contactStoreInTouch.getManualContactFromWP(WPContact, props);
                  await LocalSettings.localSettings.saveContact(jsonAvatar, MCI_CID);
                  string WPImageHash = this.getWPImageHash(jsonAvatar.photo);
                  if (!string.Equals(WPImageHash, DBImageHash) || isImageAlreadyModified)
                  {
                    DBUpdate.modifiedContactDirty = 2;
                    DBUpdate.modifiedContactImageHash = WPImageHash;
                  }
                  else
                  {
                    DBUpdate.modifiedContactDirty = 1;
                    DBUpdate.modifiedContactImageHash = DBImageHash;
                  }
                  modifiedID.Add(Id);
                  modifiedAvatar.Add(jsonAvatar);
                  DBUpdate.contact = Contacts.getContact(jsonAvatar.name, jsonAvatar.organization);
                  if (isCreated)
                    UpdateAddedContactList.Add(DBUpdate);
                  else
                    UpdateModifiedContactList.Add(DBUpdate);
                }
              }
              else if (ChangeType == 2)
                markDeletedDirtyMCI_CIDList.Add(MCI_CID);
            }
          }
          if (UpdateAddedContactList.Count > 0)
            InTouchAppDatabase.InTouchAppDB.addMultipleDirtyEntries(LocalSettings.localSettings.MCI, UpdateAddedContactList, 1);
          if (UpdateModifiedContactList.Count > 0)
            InTouchAppDatabase.InTouchAppDB.markModifiedDirtyEntries(LocalSettings.localSettings.MCI, UpdateModifiedContactList);
          int deletedCount = markDeletedDirtyMCI_CIDList.Count;
          if (deletedCount > 0)
          {
            int WPStoreCount = InTouchAppDatabase.InTouchAppDB.countInTouchEntriesFromLT(LocalSettings.localSettings.MCI);
            if ((double) markDeletedDirtyMCI_CIDList.Count / (double) WPStoreCount * 100.0 >= 10.0)
            {
              string message = "InTouchApp detected " + (object) deletedCount + " deleted contacts." + Environment.NewLine + "Whether you want to delete them or restore contacts from server?";
              if (!LocalSettings.localSettings.isBackground)
              {
                MessageDialog msgbox = new MessageDialog(message, "Deleted Contacts Found");
                msgbox.Commands.Add((IUICommand) new UICommand("Delete"));
                msgbox.Commands.Add((IUICommand) new UICommand("Restore"));
                msgbox.put_DefaultCommandIndex(0U);
                msgbox.put_CancelCommandIndex(1U);
                IUICommand msgboxResult = await msgbox.ShowAsync();
                if (ServerConnectionManager.mIsDeveloper)
                {
                  // ISSUE: object of a compiler-generated type is created
                  EasyTracker.Current.Config = new EasyTrackerConfig()
                  {
                    TrackingId = "UA-20523668-11"
                  };
                }
                else
                {
                  // ISSUE: object of a compiler-generated type is created
                  EasyTracker.Current.Config = new EasyTrackerConfig()
                  {
                    TrackingId = "UA-20523668-10"
                  };
                }
                if (msgboxResult.Label.Equals("Restore"))
                {
                  // ISSUE: reference to a compiler-generated method
                  // ISSUE: reference to a compiler-generated method
                  EasyTracker.GetTracker().SendEvent("sync", "messagedialog_btn_restore_deletedcontacts_tapped", "user tapped on messagedialog restore button, to restore deleted contacts", 0L);
                  ShallRestore = true;
                }
                else
                {
                  // ISSUE: reference to a compiler-generated method
                  // ISSUE: reference to a compiler-generated method
                  EasyTracker.GetTracker().SendEvent("sync", "messagedialog_btn_delete_deletedcontacts_tapped", "user tapped on messagedialog delete button, to delete deleted contacts", 0L);
                }
              }
              else
              {
                XmlDocument templateContent = ToastNotificationManager.GetTemplateContent((ToastTemplateType) 5);
                XmlNodeList elementsByTagName = templateContent.GetElementsByTagName("text");
                ((IReadOnlyList<IXmlNode>) elementsByTagName)[0].AppendChild((IXmlNode) templateContent.CreateTextNode("Detected " + (object) deletedCount + " deleted contacts."));
                ((IReadOnlyList<IXmlNode>) elementsByTagName)[1].AppendChild((IXmlNode) templateContent.CreateTextNode("Tap to sync"));
                ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(templateContent));
                Application.Current.Exit();
              }
            }
          }
          if (!ShallRestore)
          {
            InTouchAppDatabase.InTouchAppDB.markDeletedDirtyEntries(LocalSettings.localSettings.MCI, markDeletedDirtyMCI_CIDList);
            await LocalSettings.localSettings.deleteContactFiles(markDeletedDirtyMCI_CIDList);
          }
          else
            await RestoreContactStore.restoreContactStore.restoreContacts(markDeletedDirtyMCI_CIDList, true);
        }
        await this.setRevisionNumber();
        LogFile.Log("Total added WP contacts : " + (object) UpdateAddedContactList.Count + ".", EventType.Information);
        LogFile.Log("Total modified WP contacts : " + (object) UpdateModifiedContactList.Count + ".", EventType.Information);
        if (!ShallRestore)
          LogFile.Log("Total deleted WP contacts : " + (object) markDeletedDirtyMCI_CIDList.Count + ".", EventType.Information);
        else
          LogFile.Log("Total restored WP contacts : " + (object) markDeletedDirtyMCI_CIDList.Count + ".", EventType.Information);
        wpInTouchContacts = Tuple.Create<List<string>, List<Avatar>>(modifiedID, modifiedAvatar);
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in finding changes in WP InTouch contacts. " + ex.Message, EventType.Error);
        throw;
      }
      return wpInTouchContacts;
    }

    public string getWPImageHash(List<Photo> photo)
    {
      try
      {
        string wpImageHash = string.Empty;
        if (photo != null && photo.Count != 0)
        {
          string data = photo[0].data;
          if (!string.IsNullOrEmpty(data))
            wpImageHash = CommonCode.commonCode.getHash(Convert.ToBase64String(Convert.FromBase64String(data)));
        }
        return wpImageHash;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting InTouch contact image hash. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public async Task<Avatar> getManualContactFromWP(
      StoredContact WPContact,
      IDictionary<string, object> props)
    {
      Avatar manualContactFromWp;
      try
      {
        string givenName = WPContact.GivenName;
        Avatar avatar = new Avatar();
        avatar.@event = new List<Event>();
        avatar.email = new List<Email>();
        avatar.address = new List<Address>();
        avatar.phone = new List<InTouchLibrary.Phone>();
        avatar.name = new Name();
        string JobTitle = string.Empty;
        string CompanyName = string.Empty;
        if (props != null)
        {
          foreach (KeyValuePair<string, object> prop in (IEnumerable<KeyValuePair<string, object>>) props)
          {
            object obj = prop.Value;
            if (obj != null)
            {
              switch (prop.Key)
              {
                case "GivenName":
                  avatar.name.given = obj.ToString();
                  continue;
                case "FamilyName":
                  avatar.name.family = obj.ToString();
                  continue;
                case "HonorificPrefix":
                  avatar.name.prefix = obj.ToString();
                  continue;
                case "HonorificSuffix":
                  avatar.name.suffix = obj.ToString();
                  continue;
                case "AdditionalName":
                  avatar.name.middle = obj.ToString();
                  continue;
                case "Nickname":
                  avatar.name.nickname = obj.ToString();
                  continue;
                case "Notes":
                  avatar.notes = this.getNoteFieldSFromWP(givenName, obj.ToString());
                  continue;
                case "JobTitle":
                  JobTitle = obj.ToString();
                  continue;
                case "CompanyName":
                  CompanyName = obj.ToString();
                  continue;
                case "Anniversary":
                  avatar.@event.Add(this.getEventFieldsFromWP(givenName, EventLabel.anniv.ToString(), obj.ToString()));
                  continue;
                case "Birthdate":
                  avatar.@event.Add(this.getEventFieldsFromWP(givenName, EventLabel.bday.ToString(), obj.ToString()));
                  continue;
                case "Email":
                  avatar.email.Add(this.getEmailFieldsFromWP(givenName, "Personal", obj.ToString()));
                  continue;
                case "WorkEmail":
                  avatar.email.Add(this.getEmailFieldsFromWP(givenName, "Work", obj.ToString()));
                  continue;
                case "OtherEmail":
                  avatar.email.Add(this.getEmailFieldsFromWP(givenName, "Other", obj.ToString()));
                  continue;
                case "Url":
                  avatar.website = this.getWebsiteFieldsFromWP(givenName, obj.ToString());
                  continue;
                case "Address":
                  avatar.address.Add(this.getAddressFieldsFromWP(givenName, "Personal", (ContactAddress) obj));
                  continue;
                case "WorkAddress":
                  avatar.address.Add(this.getAddressFieldsFromWP(givenName, "Work", (ContactAddress) obj));
                  continue;
                case "OtherAddress":
                  avatar.address.Add(this.getAddressFieldsFromWP(givenName, "Other", (ContactAddress) obj));
                  continue;
                case "MobileTelephone":
                  avatar.phone.Add(this.getPhoneNumberFieldsFromWP(givenName, "Mobile", obj.ToString()));
                  continue;
                case "AlternateMobileTelephone":
                  avatar.phone.Add(this.getPhoneNumberFieldsFromWP(givenName, "Mobile 2", obj.ToString()));
                  continue;
                case "Telephone":
                  avatar.phone.Add(this.getPhoneNumberFieldsFromWP(givenName, "Home", obj.ToString()));
                  continue;
                case "WorkTelephone":
                  avatar.phone.Add(this.getPhoneNumberFieldsFromWP(givenName, "Work", obj.ToString()));
                  continue;
                case "AlternateWorkTelephone":
                  avatar.phone.Add(this.getPhoneNumberFieldsFromWP(givenName, "Work 2", obj.ToString()));
                  continue;
                case "HomeFax":
                  avatar.phone.Add(this.getPhoneNumberFieldsFromWP(givenName, "Home Fax", obj.ToString()));
                  continue;
                case "WorkFax":
                  avatar.phone.Add(this.getPhoneNumberFieldsFromWP(givenName, "Work Fax", obj.ToString()));
                  continue;
                case "CompanyPhone":
                  avatar.phone.Add(this.getPhoneNumberFieldsFromWP(givenName, "Company", obj.ToString()));
                  continue;
                case "Telephone2":
                  avatar.phone.Add(this.getPhoneNumberFieldsFromWP(givenName, "Home 2", obj.ToString()));
                  continue;
                default:
                  continue;
              }
            }
          }
        }
        if (!string.IsNullOrEmpty(JobTitle) || !string.IsNullOrEmpty(CompanyName))
        {
          avatar.organization = new List<Organization>();
          avatar.organization.Add(new Organization()
          {
            company = CompanyName,
            position = JobTitle
          });
        }
        avatar.photo = await this.getPhotoFromWP(WPContact);
        avatar.type = "manual";
        avatar.label = "Manual";
        manualContactFromWp = avatar;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in creating avatar from WP contact. " + ex.Message, EventType.Error);
        throw;
      }
      return manualContactFromWp;
    }

    public void getContactInfoFromWP(
      StoredContact WPContact,
      IDictionary<string, object> props,
      out Name name,
      out string companyName,
      out string jobTitle)
    {
      name = new Name();
      jobTitle = string.Empty;
      companyName = string.Empty;
      try
      {
        string givenName = WPContact.GivenName;
        if (props == null)
          return;
        foreach (KeyValuePair<string, object> prop in (IEnumerable<KeyValuePair<string, object>>) props)
        {
          object obj = prop.Value;
          if (obj != null)
          {
            switch (prop.Key)
            {
              case "GivenName":
                name.given = obj.ToString();
                continue;
              case "FamilyName":
                name.family = obj.ToString();
                continue;
              case "HonorificPrefix":
                name.prefix = obj.ToString();
                continue;
              case "HonorificSuffix":
                name.suffix = obj.ToString();
                continue;
              case "AdditionalName":
                name.middle = obj.ToString();
                continue;
              case "Nickname":
                name.nickname = obj.ToString();
                continue;
              case "JobTitle":
                jobTitle = obj.ToString();
                continue;
              case "CompanyName":
                companyName = obj.ToString();
                continue;
              default:
                continue;
            }
          }
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting contact info from WP contact. " + ex.Message, EventType.Error);
        throw;
      }
    }

    private Event getEventFieldsFromWP(string givenName, string label, string value)
    {
      try
      {
        Event eventFieldsFromWp = new Event();
        if (!string.IsNullOrEmpty(value))
        {
          eventFieldsFromWp.type = label;
          DateTime result;
          if (DateTime.TryParse(value, out result))
            eventFieldsFromWp.date = result.Date.ToString("yyyy-MM-dd");
        }
        return eventFieldsFromWp;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting event fields from WP contact " + givenName + " . " + ex.Message, EventType.Error);
        throw;
      }
    }

    private List<Note> getNoteFieldSFromWP(string givenName, string value)
    {
      try
      {
        return new List<Note>()
        {
          new Note() { text = value }
        };
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting note fields from WP contact " + givenName + " . " + ex.Message, EventType.Error);
        throw;
      }
    }

    private Email getEmailFieldsFromWP(string givenName, string label, string value)
    {
      try
      {
        return new Email()
        {
          address = value,
          label = label
        };
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting email fields from WP contact " + givenName + " . " + ex.Message, EventType.Error);
        throw;
      }
    }

    private Address getAddressFieldsFromWP(string givenName, string label, ContactAddress value)
    {
      try
      {
        Address addressFieldsFromWp = new Address();
        addressFieldsFromWp.city = value.Locality;
        addressFieldsFromWp.state = value.Region;
        if (!string.IsNullOrEmpty(value.StreetAddress))
        {
          string[] strArray = value.StreetAddress.Split(new char[1]
          {
            '\n'
          }, StringSplitOptions.RemoveEmptyEntries);
          switch (strArray.Length)
          {
            case 1:
              addressFieldsFromWp.street1 = strArray[0];
              break;
            case 2:
              addressFieldsFromWp.street2 = strArray[1];
              goto case 1;
            case 3:
              addressFieldsFromWp.street3 = strArray[2];
              goto case 2;
          }
        }
        addressFieldsFromWp.zip = value.PostalCode;
        addressFieldsFromWp.country = value.Country;
        addressFieldsFromWp.label = label;
        return addressFieldsFromWp;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting address fields from WP contact " + givenName + " . " + ex.Message, EventType.Error);
        throw;
      }
    }

    private InTouchLibrary.Phone getPhoneNumberFieldsFromWP(
      string givenName,
      string label,
      string value)
    {
      try
      {
        return new InTouchLibrary.Phone() { number = value, label = label };
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting phone number fields from WP contact " + givenName + " . " + ex.Message, EventType.Error);
        throw;
      }
    }

    public async Task<List<Photo>> getPhotoFromWP(StoredContact WPContact)
    {
      List<Photo> photoFromWp;
      try
      {
        List<Photo> photoList = new List<Photo>();
        string base64ImageRepresentation = string.Empty;
        IRandomAccessStream PictureStream = await WPContact.GetDisplayPictureAsync();
        if (PictureStream != null)
        {
          MemoryStream destination = new MemoryStream();
          PictureStream.AsStream().CopyTo((Stream) destination);
          base64ImageRepresentation = Convert.ToBase64String(destination.ToArray());
          photoList.Add(new Photo()
          {
            data = base64ImageRepresentation
          });
          ((IDisposable) PictureStream).Dispose();
        }
        photoFromWp = photoList;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting photo from WP contact " + WPContact.GivenName + " . " + ex.Message, EventType.Error);
        throw;
      }
      return photoFromWp;
    }

    public async Task<BitmapImage> getPhotoFromAvatar(List<Photo> photo)
    {
      BitmapImage photoFromAvatar;
      try
      {
        BitmapImage bitmapImage = new BitmapImage();
        bool isPhotoAssigned = false;
        if (photo != null)
        {
          foreach (Photo _photo in photo)
          {
            if (!string.IsNullOrEmpty(_photo.data))
            {
              isPhotoAssigned = true;
              byte[] imageBytes = Convert.FromBase64String(_photo.data);
              MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
              await ((BitmapSource) bitmapImage).SetSourceAsync(ms.AsRandomAccessStream());
              ms.Dispose();
            }
          }
        }
        if (!isPhotoAssigned)
          bitmapImage = new BitmapImage(new Uri("ms-appx:///Assets/in_img_default_profile_48dp.png"));
        photoFromAvatar = bitmapImage;
      }
      catch
      {
        throw;
      }
      return photoFromAvatar;
    }

    private List<Website> getWebsiteFieldsFromWP(string givenName, string value)
    {
      try
      {
        List<Website> websiteFieldsFromWp = new List<Website>();
        char[] separator = new char[2]{ ';', ' ' };
        string[] strArray = value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        if (strArray != null)
        {
          foreach (string str in strArray)
          {
            if (!string.IsNullOrEmpty(str))
              websiteFieldsFromWp.Add(new Website()
              {
                url = str
              });
          }
        }
        return websiteFieldsFromWp;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting website fields from WP contact " + givenName + " . " + ex.Message, EventType.Error);
        throw;
      }
    }
  }
}
