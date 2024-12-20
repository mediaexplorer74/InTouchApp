// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.ContactstoreOther
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using BugSense;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.Storage.Streams;

#nullable disable
namespace InTouchLibrary
{
  public class ContactstoreOther
  {
    private static volatile ContactstoreOther _contactstoreOther;

    public static ContactstoreOther contactStoreOtherObj
    {
      get
      {
        try
        {
          if (ContactstoreOther._contactstoreOther == null)
            ContactstoreOther._contactstoreOther = new ContactstoreOther();
          return ContactstoreOther._contactstoreOther;
        }
        catch
        {
          throw;
        }
      }
    }

    public async Task findChangesInWPOtherContacts()
    {
      try
      {
        List<DBUpdate> UpdateModifiedContactList = new List<DBUpdate>();
        List<DBUpdate> UpdateAddedContactList = new List<DBUpdate>();
        ContactStore contactStore1 = await ContactManager.RequestStoreAsync();
        IReadOnlyList<Contact> contacts = await contactStore1.FindContactsAsync();
        List<string> WPOtherContactMCI_CIDList = InTouchAppDatabase.InTouchAppDB.getWPOtherContactMCI_CIDList(LocalSettings.localSettings.MCI);
        int i = 0;
        int contacts_count = contacts.Count;
        List<string> WPOtherContactIDList = new List<string>();
        if (contacts != null)
        {
          foreach (Contact contact in (IEnumerable<Contact>) contacts)
            WPOtherContactIDList.Add(contact.Id);
        }
        List<string> inMCI_CIDList = new List<string>();
        List<string> inContactHashList = new List<string>();
        List<string> inImageHashList = new List<string>();
        InTouchAppDatabase.InTouchAppDB.readOtherMCI_CID(LocalSettings.localSettings.MCI, WPOtherContactIDList, out inMCI_CIDList, out inContactHashList, out inImageHashList);
        if (contacts != null)
        {
          foreach (Contact WPContact in (IEnumerable<Contact>) contacts)
          {
            if (i % 10 == 0 || i % i == 0)
              Sync.setSyncStatusBlock("Checking modified contacts\n" + (object) (i + 1) + "/" + (object) contacts_count);
            string MCI_CID = inMCI_CIDList[i];
            bool onlyInTouchContact = WPContact.DataSuppliers.Count == 1 && WPContact.DataSuppliers.Contains("InTouchApp");
            if (!string.IsNullOrEmpty(MCI_CID) && !onlyInTouchContact)
              WPOtherContactMCI_CIDList.Remove(MCI_CID);
            DBUpdate DBUpdate = new DBUpdate();
            if (WPContact.DataSuppliers != null && !onlyInTouchContact)
            {
              Avatar avatar = this.GetManualContactFromWP(WPContact);
              string json = JsonConvert.SerializeObject((object) avatar, new JsonSerializerSettings()
              {
                NullValueHandling = NullValueHandling.Ignore
              });
              string WPContactHash = CommonCode.commonCode.getHash(json);
              string photoString = await this.GetPhotoFromWP(WPContact);
              string WPImageHash = CommonCode.commonCode.getHash(photoString);
              DBUpdate.modifiedContactContactHash = WPContactHash;
              DBUpdate.modifiedContactID = WPContact.Id;
              DBUpdate.modifiedContactMCI_CID = MCI_CID;
              if (string.IsNullOrEmpty(MCI_CID))
              {
                MCI_CID = CommonCode.commonCode.createRandomStringID();
                DBUpdate.modifiedContactMCI_CID = MCI_CID;
                DBUpdate.modifiedContactImageHash = WPImageHash;
                DBUpdate.modifiedContactDirty = !string.IsNullOrEmpty(WPImageHash) ? 2 : 1;
                UpdateAddedContactList.Add(DBUpdate);
              }
              else
              {
                string b = inImageHashList[i];
                if (!string.Equals(WPImageHash, b))
                {
                  DBUpdate.modifiedContactDirty = 2;
                  DBUpdate.modifiedContactImageHash = WPImageHash;
                  UpdateModifiedContactList.Add(DBUpdate);
                }
                else if (!string.Equals(WPContactHash, inContactHashList[i]))
                {
                  DBUpdate.modifiedContactDirty = 1;
                  DBUpdate.modifiedContactImageHash = b;
                  UpdateModifiedContactList.Add(DBUpdate);
                }
              }
            }
            ++i;
          }
        }
        if (UpdateAddedContactList.Count > 0)
          InTouchAppDatabase.InTouchAppDB.addMultipleDirtyEntries(LocalSettings.localSettings.MCI, UpdateAddedContactList, 2);
        if (UpdateModifiedContactList.Count > 0)
          InTouchAppDatabase.InTouchAppDB.markModifiedDirtyEntries(LocalSettings.localSettings.MCI, UpdateModifiedContactList);
        if (WPOtherContactMCI_CIDList.Count > 0)
          InTouchAppDatabase.InTouchAppDB.markDeletedDirtyEntries(LocalSettings.localSettings.MCI, WPOtherContactMCI_CIDList);
        LogFile.Log("Total added WP contacts : " + (object) UpdateAddedContactList.Count + ".", EventType.Information);
        LogFile.Log("Total modified WP contacts : " + (object) UpdateModifiedContactList.Count + ".", EventType.Information);
        LogFile.Log("Total deleted WP contacts : " + (object) WPOtherContactMCI_CIDList.Count + ".", EventType.Information);
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in finding changes in WP other contacts. " + ex.Message, EventType.Error);
        BugSenseHandler.Instance.LogException(ex, ex.Message, ex.StackTrace);
        BugSenseHandler.Instance.LogEvent(ex.Message + Environment.NewLine + ex.StackTrace);
        BugSenseHandler.Instance.SendEventAsync("Event: " + ex.Message + Environment.NewLine + ex.StackTrace);
        BugSenseHandler.Instance.SendExceptionAsync(ex, ex.Message, ex.StackTrace);
        throw;
      }
    }

    public Avatar GetManualContactFromWP(Contact WPContact)
    {
      try
      {
        return new Avatar()
        {
          @event = this.GetEventFieldsFromWP(WPContact),
          notes = this.GetNoteFieldSFromWP(WPContact),
          organization = this.GetOrganizationFieldsFromWP(WPContact),
          email = this.GetEmailFieldsFromWP(WPContact),
          address = this.GetAddressFieldsFromWP(WPContact),
          phone = this.GetPhoneNumberFieldsFromWP(WPContact),
          website = this.GetWebsiteFieldsFromWP(WPContact)
        };
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in creating avatar from WP contact. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public Name GetNameFiledsFromWP(Contact WPContact)
    {
      try
      {
        return new Name()
        {
          given = WPContact.FirstName,
          family = WPContact.LastName,
          middle = WPContact.MiddleName,
          suffix = WPContact.HonorificNameSuffix,
          prefix = WPContact.HonorificNamePrefix
        };
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting name fields from WP contact " + WPContact.FirstName + " . " + ex.Message, EventType.Error);
        throw;
      }
    }

    private List<Event> GetEventFieldsFromWP(Contact WPContact)
    {
      try
      {
        List<Event> eventFieldsFromWp = new List<Event>();
        if (WPContact.ImportantDates != null)
        {
          foreach (ContactDate importantDate in (IEnumerable<ContactDate>) WPContact.ImportantDates)
          {
            Event @event = new Event();
            @event.date = importantDate.Year.ToString() + "-" + importantDate.Month.ToString() + "-" + importantDate.Day.ToString();
            if (importantDate.Kind == 1)
              @event.type = EventLabel.anniv.ToString();
            else if (importantDate.Kind == null)
              @event.type = EventLabel.bday.ToString();
            else if (importantDate.Kind == 2)
              @event.type = EventLabel.other.ToString();
            eventFieldsFromWp.Add(@event);
          }
        }
        return eventFieldsFromWp;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting event fields from WP contact " + WPContact.FirstName + " . " + ex.Message, EventType.Error);
        throw;
      }
    }

    private List<Note> GetNoteFieldSFromWP(Contact WPContact)
    {
      try
      {
        List<Note> noteFieldSfromWp = new List<Note>();
        if (!string.IsNullOrEmpty(WPContact.Notes))
          noteFieldSfromWp.Add(new Note()
          {
            text = WPContact.Notes
          });
        return noteFieldSfromWp;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting note fields from WP contact " + WPContact.FirstName + " . " + ex.Message, EventType.Error);
        throw;
      }
    }

    private List<Organization> GetOrganizationFieldsFromWP(Contact WPContact)
    {
      try
      {
        List<Organization> organizationFieldsFromWp = new List<Organization>();
        if (WPContact.JobInfo != null)
        {
          foreach (ContactJobInfo contactJobInfo in (IEnumerable<ContactJobInfo>) WPContact.JobInfo)
            organizationFieldsFromWp.Add(new Organization()
            {
              company = contactJobInfo.CompanyName,
              department = contactJobInfo.Department,
              position = contactJobInfo.Title
            });
        }
        return organizationFieldsFromWp;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting organization fields from WP contact " + WPContact.FirstName + " . " + ex.Message, EventType.Error);
        throw;
      }
    }

    private List<Email> GetEmailFieldsFromWP(Contact WPContact)
    {
      try
      {
        List<Email> emailFieldsFromWp = new List<Email>();
        string empty1 = string.Empty;
        string empty2 = string.Empty;
        string empty3 = string.Empty;
        if (WPContact.Emails != null)
        {
          foreach (ContactEmail email1 in (IEnumerable<ContactEmail>) WPContact.Emails)
          {
            Email email2 = new Email();
            email2.address = email1.Address;
            if (email1.Kind == 2)
              email2.label = "Other";
            else if (email1.Kind == null)
              email2.label = "Personal";
            else if (email1.Kind == 1)
              email2.label = "Work";
            emailFieldsFromWp.Add(email2);
          }
        }
        return emailFieldsFromWp;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting email fields from WP contact " + WPContact.FirstName + " . " + ex.Message, EventType.Error);
        throw;
      }
    }

    private List<Address> GetAddressFieldsFromWP(Contact WPContact)
    {
      try
      {
        List<Address> addressFieldsFromWp = new List<Address>();
        if (WPContact.Addresses != null)
        {
          foreach (ContactAddress address1 in (IEnumerable<ContactAddress>) WPContact.Addresses)
          {
            Address address2 = new Address();
            address2.country = address1.Country;
            address2.city = address1.Locality;
            address2.state = address1.Region;
            address2.street1 = address1.StreetAddress;
            address2.zip = address1.PostalCode;
            if (address1.Kind == 2)
              address2.label = "Other";
            else if (address1.Kind == null)
              address2.label = "Personal";
            else if (address1.Kind == 1)
              address2.label = "Work";
            addressFieldsFromWp.Add(address2);
          }
        }
        return addressFieldsFromWp;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting address fields from WP contact " + WPContact.FirstName + " . " + ex.Message, EventType.Error);
        throw;
      }
    }

    private List<Phone> GetPhoneNumberFieldsFromWP(Contact WPContact)
    {
      try
      {
        List<Phone> numberFieldsFromWp = new List<Phone>();
        if (WPContact.Phones != null)
        {
          foreach (ContactPhone phone1 in (IEnumerable<ContactPhone>) WPContact.Phones)
          {
            Phone phone2 = new Phone();
            phone2.number = phone1.Number;
            if (phone1.Kind == 3)
              phone2.label = "Other";
            else if (phone1.Kind == null)
              phone2.label = "Home";
            else if (phone1.Kind == 1)
              phone2.label = "Mobile";
            else if (phone1.Kind == 2)
              phone2.label = "Work";
            numberFieldsFromWp.Add(phone2);
          }
        }
        return numberFieldsFromWp;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting phone number fields from WP contact " + WPContact.FirstName + " . " + ex.Message, EventType.Error);
        throw;
      }
    }

    private List<Website> GetWebsiteFieldsFromWP(Contact WPContact)
    {
      try
      {
        List<Website> websiteFieldsFromWp = new List<Website>();
        if (WPContact.Websites != null)
        {
          foreach (ContactWebsite website in (IEnumerable<ContactWebsite>) WPContact.Websites)
            websiteFieldsFromWp.Add(new Website()
            {
              url = website.Uri.ToString()
            });
        }
        return websiteFieldsFromWp;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting website fields from WP contact " + WPContact.FirstName + " . " + ex.Message, EventType.Error);
        throw;
      }
    }

    public async Task<string> GetPhotoFromWP(Contact WPContact)
    {
      string photoFromWp;
      try
      {
        string base64ImageRepresentation = string.Empty;
        IRandomAccessStreamReference PictureStream = WPContact.Thumbnail;
        if (PictureStream != null)
        {
          MemoryStream ms = new MemoryStream();
          IRandomAccessStreamWithContentType stream1 = await PictureStream.OpenReadAsync();
          Stream stream = ((IRandomAccessStream) stream1).AsStream();
          stream.CopyTo((Stream) ms);
          byte[] imageArray = ms.ToArray();
          base64ImageRepresentation = Convert.ToBase64String(imageArray);
        }
        photoFromWp = base64ImageRepresentation;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting photo from WP contact " + WPContact.FirstName + " . " + ex.Message, EventType.Error);
        throw;
      }
      return photoFromWp;
    }

    public List<string> GetDataSuppliers(Contact WPContact)
    {
      try
      {
        List<string> dataSuppliers = new List<string>();
        if (WPContact.DataSuppliers != null)
        {
          foreach (string dataSupplier in (IEnumerable<string>) WPContact.DataSuppliers)
            dataSuppliers.Add(dataSupplier);
        }
        return dataSuppliers;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting data supplier. " + ex.Message, EventType.Error);
        throw;
      }
    }
  }
}
