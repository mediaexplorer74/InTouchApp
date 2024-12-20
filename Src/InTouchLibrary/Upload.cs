// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.Upload
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.Phone.PersonalInformation;

#nullable disable
namespace InTouchLibrary
{
  public class Upload
  {
    private Stopwatch upload = new Stopwatch();
    private Stopwatch uploadAPI = new Stopwatch();

    public async Task uploadWPContacts(List<string> modifiedID, List<Avatar> modifiedAvatar)
    {
      List<string> modifiedContactIDList = new List<string>();
      List<string> modifiedContactMCI_CIDList = new List<string>();
      List<string> modifiedContactBaseVersionList = new List<string>();
      List<int> modifiedContactDirtyList = new List<int>();
      List<int> modifiedContactContactTypeList = new List<int>();
      int countID = 0;
      try
      {
        this.upload.Restart();
        this.uploadAPI.Restart();
        InTouchAppDatabase.InTouchAppDB.getDirtyEntries(LocalSettings.localSettings.MCI, out modifiedContactIDList, out modifiedContactMCI_CIDList, out modifiedContactBaseVersionList, out modifiedContactDirtyList, out modifiedContactContactTypeList);
        countID = modifiedContactIDList.Count;
        if (countID > 0)
        {
          int packetID = 0;
          int contactsPerPacket = 100;
          string displayName = string.Empty;
          List<ContactUpload> uploadContactbook = new List<ContactUpload>();
          for (int i = 0; i < countID; ++i)
          {
            string ID = modifiedContactIDList[i];
            string MCI_CID = modifiedContactMCI_CIDList[i];
            int dirtyBit = modifiedContactDirtyList[i];
            int contactType = modifiedContactContactTypeList[i];
            string baseVersion = modifiedContactBaseVersionList[i];
            ContactUpload uploadContact = new ContactUpload();
            uploadContact.contact_id_1 = MCI_CID;
            if (contactType == 2)
            {
              uploadContact.read_only = true;
              uploadContact.account_type = "other";
              uploadContact.account_name = "Other";
              if (dirtyBit == 3)
              {
                uploadContact.action = "delete";
              }
              else
              {
                ContactStore contactStore1 = await ContactManager.RequestStoreAsync();
                Contact WPContact = await contactStore1.GetContactAsync(ID);
                if (WPContact != null)
                {
                  uploadContact.contact_device_id = ID;
                  uploadContact.base_version = baseVersion;
                  uploadContact.origin = "client";
                  uploadContact.action = "import_and_keep";
                  uploadContact.winphone_account = ContactstoreOther.contactStoreOtherObj.GetDataSuppliers(WPContact);
                  Avatar avatar = ContactstoreOther.contactStoreOtherObj.GetManualContactFromWP(WPContact);
                  if (dirtyBit == 2)
                  {
                    string data = await ContactstoreOther.contactStoreOtherObj.GetPhotoFromWP(WPContact);
                    Photo photo = new Photo();
                    photo.data = data;
                    avatar.photo = new List<Photo>();
                    avatar.photo.Add(photo);
                  }
                  avatar.type = "manual";
                  avatar.label = "Manual";
                  uploadContact.avatars = new List<Avatar>();
                  uploadContact.avatars.Add(avatar);
                  uploadContact.name = ContactstoreOther.contactStoreOtherObj.GetNameFiledsFromWP(WPContact);
                  displayName = CommonCode.commonCode.getDisplayName(uploadContact.avatars, uploadContact.name);
                }
              }
            }
            else
            {
              uploadContact.account_type = "intouch";
              uploadContact.account_name = "InTouch";
              if (dirtyBit == 3)
              {
                uploadContact.action = "delete_all";
              }
              else
              {
                Avatar avatar = new Avatar();
                if (modifiedID.Contains(ID))
                {
                  avatar = modifiedAvatar[modifiedID.IndexOf(ID)];
                }
                else
                {
                  if (ContactstoreInTouch.contactStore == null)
                  {
                    Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
                    if (result.Item2)
                      await RestoreContactStore.restoreContactStore.restoreContacts((List<string>) null, true);
                  }
                  StoredContact WPContact = (StoredContact) null;
                  try
                  {
                    try
                    {
                      WPContact = await ContactstoreInTouch.contactStore.FindContactByIdAsync(ID);
                    }
                    catch (Exception ex)
                    {
                      LogFile.Log("Error in reading contact from WP. " + ex.Message, EventType.Error);
                    }
                    if (WPContact != null)
                    {
                      IDictionary<string, object> props = await WPContact.GetPropertiesAsync();
                      avatar = await ContactstoreInTouch.contactStoreInTouch.getManualContactFromWP(WPContact, props);
                    }
                  }
                  catch (Exception ex)
                  {
                    LogFile.Log("Error in reading contact from WP. " + ex.Message, EventType.Error);
                  }
                }
                if (dirtyBit != 2)
                  avatar.photo = (List<Photo>) null;
                uploadContact.contact_device_id = ID;
                uploadContact.base_version = baseVersion;
                uploadContact.read_only = false;
                uploadContact.origin = "client";
                uploadContact.avatars = new List<Avatar>();
                uploadContact.avatars.Add(avatar);
                uploadContact.name = avatar.name;
                displayName = CommonCode.commonCode.getDisplayName(uploadContact.avatars, uploadContact.name);
              }
            }
            uploadContactbook.Add(uploadContact);
            if (i % 3 == 0)
            {
              string empty = string.Empty;
              string text;
              if (string.IsNullOrEmpty(displayName))
                text = "Uploading Contacts : " + (object) (i + 1) + "/" + (object) countID;
              else
                text = "Uploading Contacts : " + (object) (i + 1) + "/" + (object) countID + "\n" + displayName;
              Sync.setSyncStatusBlock(text);
            }
            if (uploadContactbook.Count >= contactsPerPacket)
            {
              if (!LocalSettings.localSettings.isBackground)
                Sync.setContactListView();
              Sync.setSyncStatusBlock("Sending " + (object) uploadContactbook.Count + " contact(s) to server");
              if (await this.uploadingWPContacts(packetID, uploadContactbook, modifiedContactDirtyList))
              {
                ++packetID;
                uploadContactbook.Clear();
                LocalSettings.localSettings.syncSessionLastUpdateTime = DateTime.Now.ToString("d MMM, hh.mm ") + DateTime.Now.ToString("tt").ToLower();
              }
            }
          }
          if (uploadContactbook.Count > 0)
          {
            if (!LocalSettings.localSettings.isBackground)
              Sync.setContactListView();
            Sync.setSyncStatusBlock("Sending " + (object) uploadContactbook.Count + " contact(s) to server");
            if (await this.uploadingWPContacts(packetID, uploadContactbook, modifiedContactDirtyList))
            {
              ++packetID;
              uploadContactbook.Clear();
              LocalSettings.localSettings.syncSessionLastUpdateTime = DateTime.Now.ToString("d MMM, hh.mm ") + DateTime.Now.ToString("tt").ToLower();
            }
          }
          LogFile.Log("Uploaded all WP contacts.", EventType.Information);
          Sync.setSyncStatusBlock("All changes backed up");
          Sync.setImageStatusBlock(string.Empty);
        }
        else
        {
          Sync.setSyncStatusBlock("No changes found on phone");
          Sync.setImageStatusBlock(string.Empty);
        }
        this.upload.Stop();
        this.uploadAPI.Stop();
        LogFile.Log("Time to upload contacts, " + (object) this.upload.ElapsedMilliseconds, EventType.Test);
        LogFile.Log("Time to upload contacts with API, " + (object) this.uploadAPI.ElapsedMilliseconds, EventType.Test);
      }
      catch (Exception ex)
      {
        LogFile.Log("Error while uploading WP contacts. " + ex.Message, EventType.Error);
        throw;
      }
    }

    private async Task<bool> uploadingWPContacts(
      int packetID,
      List<ContactUpload> uploadContactbook,
      List<int> modifiedContactPostUploadDirtyList)
    {
      bool flag;
      try
      {
        LogFile.Log("Uploading WP contacts (packet = " + (object) packetID + ", count = " + (object) uploadContactbook.Count + ").", EventType.Information);
        UploadContactbook uploadContacts = new UploadContactbook();
        uploadContacts.packet_id = packetID;
        uploadContacts.session_id = LocalSettings.localSettings.syncSessionID;
        uploadContacts.contacts = uploadContactbook;
        bool isSuccess = false;
        this.upload.Stop();
        ServerConnectionManager SCMobj = new ServerConnectionManager();
        if (SCMobj.mIsConnectedToNetwork)
        {
          ServerConnectionManager connectionManager1 = SCMobj;
          UploadContactbook uploadContacts1 = uploadContacts;
          ServerConnectionManager connectionManager2 = connectionManager1;
          string token = await LocalSettings.localSettings.getToken();
          isSuccess = await connectionManager2.uploadContacts(uploadContacts1, token);
          this.upload.Start();
          if (isSuccess)
          {
            List<string> toDeleteMCI_CIDList = new List<string>();
            List<string> MCI_CIDList = new List<string>();
            for (int index = 0; index < uploadContactbook.Count; ++index)
            {
              if (modifiedContactPostUploadDirtyList[index] == 3)
                toDeleteMCI_CIDList.Add(uploadContactbook[index].contact_id_1);
              else
                MCI_CIDList.Add(uploadContactbook[index].contact_id_1);
            }
            if (toDeleteMCI_CIDList.Count > 0)
              InTouchAppDatabase.InTouchAppDB.deleteMultipleDirtyEntries(LocalSettings.localSettings.MCI, toDeleteMCI_CIDList);
            if (MCI_CIDList.Count > 0)
              InTouchAppDatabase.InTouchAppDB.unmarkDirtyEntries(LocalSettings.localSettings.MCI, MCI_CIDList);
          }
          flag = isSuccess;
        }
        else
        {
          LogFile.Log("Internet connection is not available.", EventType.Warning);
          throw new NotSupportedException("Unable to sync." + Environment.NewLine + "Please check internet connectivity.");
        }
      }
      catch (Exception ex)
      {
        if (ex is NotSupportedException)
          LogFile.Log("Problem in uploading packet " + (object) packetID + " due to internet connectivity.", EventType.Warning);
        else
          LogFile.Log("Error in updating uploaded WP contacts. " + ex.Message, EventType.Error);
        throw;
      }
      return flag;
    }
  }
}
