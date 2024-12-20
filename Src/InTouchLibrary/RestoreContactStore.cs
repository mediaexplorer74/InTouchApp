// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.RestoreContactStore
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Phone.PersonalInformation;

#nullable disable
namespace InTouchLibrary
{
  public class RestoreContactStore
  {
    private static volatile RestoreContactStore _restoreContactStore;
    private object lockThis = new object();

    public static RestoreContactStore restoreContactStore
    {
      get
      {
        try
        {
          if (RestoreContactStore._restoreContactStore == null)
            RestoreContactStore._restoreContactStore = new RestoreContactStore();
          return RestoreContactStore._restoreContactStore;
        }
        catch
        {
          throw;
        }
      }
    }

    public async Task restoreContacts(List<string> MCI_CIDList, bool entireRestore)
    {
      try
      {
        LocalSettings.localSettings.syncSessionState = 7;
        if (ContactstoreInTouch.contactStore == null)
          await ContactstoreInTouch.contactStoreInTouch.getStore();
        Stopwatch restore = new Stopwatch();
        restore.Start();
        LocalSettings.localSettings.syncSessionLastUpdateTime = DateTime.Now.ToString("d MMM, hh.mm ") + DateTime.Now.ToString("tt").ToLower();
        Sync.setSyncStatusBlock("Restoring contacts");
        List<string> IDList = new List<string>();
        if (MCI_CIDList == null)
          MCI_CIDList = InTouchAppDatabase.InTouchAppDB.getWPInTouchContactMCI_CIDList(LocalSettings.localSettings.MCI, out IDList, entireRestore);
        int count = MCI_CIDList.Count;
        if (entireRestore)
        {
          for (int i = 0; i < count; ++i)
          {
            Sync.setSyncStatusBlock("Restoring contacts : " + (object) (i + 1) + "/" + (object) count);
            await this.restoreContact(MCI_CIDList[i]);
          }
        }
        else
        {
          for (int i = 0; i < count; ++i)
          {
            Sync.setSyncStatusBlock("Restoring contacts : " + (object) (i + 1) + "/" + (object) count);
            StoredContact WPContact = (StoredContact) null;
            try
            {
              WPContact = await ContactstoreInTouch.contactStore.FindContactByIdAsync(IDList[i]);
            }
            catch
            {
            }
            if (WPContact == null)
              await this.restoreContact(MCI_CIDList[i]);
          }
        }
        LocalSettings.localSettings.syncSessionLastUpdateTime = DateTime.Now.ToString("d MMM, hh.mm ") + DateTime.Now.ToString("tt").ToLower();
        restore.Stop();
        LogFile.Log("Time to restore contacts, " + (object) restore.ElapsedMilliseconds, EventType.Test);
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in restoring contacts. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public async Task restoreContact(string MCI_CID)
    {
      try
      {
        string json = await LocalSettings.localSettings.loadContact(MCI_CID);
        if (string.IsNullOrEmpty(json))
          return;
        Avatar jsonAvatar = JsonConvert.DeserializeObject<Avatar>(json);
        ContactContactbookManual manualContact = new ContactContactbookManual()
        {
          avatars = new List<Avatar>()
        };
        manualContact.avatars.Add(jsonAvatar);
        manualContact.contact_id_1 = MCI_CID;
        StoredContact WPContact = new StoredContact(ContactstoreInTouch.contactStore);
        IDictionary<string, object> props = await WPContact.GetPropertiesAsync();
        Download.download.setManualContactToWP(manualContact, WPContact, props);
        await Download.download.setPhotoFieldsToWP(manualContact.avatars, WPContact, manualContact.contact_id_1);
        await WPContact.SaveAsync();
        await ContactstoreInTouch.contactStoreInTouch.setRevisionNumber();
        InTouchAppDatabase.InTouchAppDB.updateID(LocalSettings.localSettings.MCI, MCI_CID, WPContact.Id);
      }
      catch (Exception ex)
      {
        string message = MCI_CID + " : Error in restoring contact. " + ex.Message;
        LogFile.Log(message, EventType.Error);
        CommonCode.commonCode.reportBug("Sync", message);
      }
    }
  }
}
