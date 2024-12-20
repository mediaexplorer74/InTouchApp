// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.LocalSettings
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;
using Windows.Storage.Streams;

#nullable disable
namespace InTouchLibrary
{
  public class LocalSettings
  {
    private static volatile LocalSettings _localSettings;
    public ApplicationDataContainer currentLocalSettings = ApplicationData.Current.LocalSettings;

    public static LocalSettings localSettings
    {
      get
      {
        try
        {
          if (LocalSettings._localSettings == null)
            LocalSettings._localSettings = new LocalSettings();
          return LocalSettings._localSettings;
        }
        catch
        {
          throw;
        }
      }
    }

    public int versionManual
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("Version_Manual") ? (int) ((IDictionary<string, object>) this.currentLocalSettings.Values)["Version_Manual"] : -2;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["Version_Manual"] = (object) value;
      }
    }

    public int versionAuto
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("Version_InTouch") ? (int) ((IDictionary<string, object>) this.currentLocalSettings.Values)["Version_InTouch"] : -1;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["Version_InTouch"] = (object) value;
      }
    }

    public int revisionNumber
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("RevisionNumber") ? (int) ((IDictionary<string, object>) this.currentLocalSettings.Values)["RevisionNumber"] : 0;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["RevisionNumber"] = (object) value;
      }
    }

    public string lastSyncTime
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("Last_Sync_Time") ? (string) ((IDictionary<string, object>) this.currentLocalSettings.Values)["Last_Sync_Time"] ?? string.Empty : string.Empty;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["Last_Sync_Time"] = (object) value;
      }
    }

    public string lastCheckedTime
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("last_checked_time") ? (string) ((IDictionary<string, object>) this.currentLocalSettings.Values)["last_checked_time"] ?? string.Empty : string.Empty;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["last_checked_time"] = (object) value;
      }
    }

    public byte[] token
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("Token") ? (byte[]) ((IDictionary<string, object>) this.currentLocalSettings.Values)["Token"] ?? (byte[]) null : (byte[]) null;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["Token"] = (object) value;
      }
    }

    public string MCI
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey(nameof (MCI)) ? (string) ((IDictionary<string, object>) this.currentLocalSettings.Values)[nameof (MCI)] ?? string.Empty : string.Empty;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)[nameof (MCI)] = (object) value;
      }
    }

    public string iid
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey(nameof (iid)) ? (string) ((IDictionary<string, object>) this.currentLocalSettings.Values)[nameof (iid)] ?? string.Empty : string.Empty;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)[nameof (iid)] = (object) value;
      }
    }

    public string serverName
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("ServerName") ? (string) ((IDictionary<string, object>) this.currentLocalSettings.Values)["ServerName"] ?? string.Empty : string.Empty;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["ServerName"] = (object) value;
      }
    }

    public bool downloadPhotoOnWifi
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("DownloadPhotoOnWifi") && (bool) ((IDictionary<string, object>) this.currentLocalSettings.Values)["DownloadPhotoOnWifi"];
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["DownloadPhotoOnWifi"] = (object) value;
      }
    }

    public bool isBackground
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("is_background") && (bool) ((IDictionary<string, object>) this.currentLocalSettings.Values)["is_background"];
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["is_background"] = (object) value;
      }
    }

    public string channelUri
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("channel_uri") ? (string) ((IDictionary<string, object>) this.currentLocalSettings.Values)["channel_uri"] ?? string.Empty : string.Empty;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["channel_uri"] = (object) value;
      }
    }

    public string appVersion
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("AppVersion") ? (string) ((IDictionary<string, object>) this.currentLocalSettings.Values)["AppVersion"] ?? string.Empty : string.Empty;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["AppVersion"] = (object) value;
      }
    }

    public string syncSessionID
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("sync_session_id") ? (string) ((IDictionary<string, object>) this.currentLocalSettings.Values)["sync_session_id"] ?? string.Empty : string.Empty;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["sync_session_id"] = (object) value;
      }
    }

    public string syncSessionLastUpdateTime
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("sync_session_last_update_time") ? (string) ((IDictionary<string, object>) this.currentLocalSettings.Values)["sync_session_last_update_time"] ?? string.Empty : string.Empty;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["sync_session_last_update_time"] = (object) value;
      }
    }

    public string syncSessionOwner
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("sync_session_owner") ? (string) ((IDictionary<string, object>) this.currentLocalSettings.Values)["sync_session_owner"] ?? string.Empty : string.Empty;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["sync_session_owner"] = (object) value;
      }
    }

    public int syncSessionState
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("sync_session_state") ? (int) ((IDictionary<string, object>) this.currentLocalSettings.Values)["sync_session_state"] : 0;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["sync_session_state"] = (object) value;
      }
    }

    public bool syncOtherContacts
    {
      get
      {
        return !((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("SyncOtherContacts") || (bool) ((IDictionary<string, object>) this.currentLocalSettings.Values)["SyncOtherContacts"];
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["SyncOtherContacts"] = (object) value;
      }
    }

    public string syncStatusBlock
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("sync_status_block") ? (string) ((IDictionary<string, object>) this.currentLocalSettings.Values)["sync_status_block"] ?? string.Empty : string.Empty;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["sync_status_block"] = (object) value;
      }
    }

    public string editContactType
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("edit_contact_type") ? (string) ((IDictionary<string, object>) this.currentLocalSettings.Values)["edit_contact_type"] ?? string.Empty : string.Empty;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["edit_contact_type"] = (object) value;
      }
    }

    public string accountType
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("account_type") ? (string) ((IDictionary<string, object>) this.currentLocalSettings.Values)["account_type"] ?? string.Empty : string.Empty;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["account_type"] = (object) value;
      }
    }

    public string contactsManagedBlock
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("contacts_managed_block") ? (string) ((IDictionary<string, object>) this.currentLocalSettings.Values)["contacts_managed_block"] ?? string.Empty : string.Empty;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["contacts_managed_block"] = (object) value;
      }
    }

    public string imageStatusBlock
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("image_status_block") ? (string) ((IDictionary<string, object>) this.currentLocalSettings.Values)["image_status_block"] ?? string.Empty : string.Empty;
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["image_status_block"] = (object) value;
      }
    }

    public bool downloadPhotoOnWifiTSEnable
    {
      get
      {
        return !((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("download_photo_on_wifi_ts_enable") || (bool) ((IDictionary<string, object>) this.currentLocalSettings.Values)["download_photo_on_wifi_ts_enable"];
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["download_photo_on_wifi_ts_enable"] = (object) value;
      }
    }

    public bool backupPhoneContactsBlockTapEnable
    {
      get
      {
        return !((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("backup_phone_contacts_tap_enable") || (bool) ((IDictionary<string, object>) this.currentLocalSettings.Values)["backup_phone_contacts_tap_enable"];
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["backup_phone_contacts_tap_enable"] = (object) value;
      }
    }

    public bool dontCloseAppVisible
    {
      get
      {
        return !((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("dont_close_app_visible") || (bool) ((IDictionary<string, object>) this.currentLocalSettings.Values)["dont_close_app_visible"];
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["dont_close_app_visible"] = (object) value;
      }
    }

    public bool updateDBWithValues
    {
      get
      {
        return ((IDictionary<string, object>) this.currentLocalSettings.Values).ContainsKey("update_DB_with_values") && (bool) ((IDictionary<string, object>) this.currentLocalSettings.Values)["update_DB_with_values"];
      }
      set
      {
        ((IDictionary<string, object>) this.currentLocalSettings.Values)["update_DB_with_values"] = (object) value;
      }
    }

    public async Task setToken(string _token)
    {
      try
      {
        if (!string.IsNullOrEmpty(_token))
        {
          string strDescriptor = "LOCAL=user";
          DataProtectionProvider Provider = new DataProtectionProvider(strDescriptor);
          BinaryStringEncoding encoding = (BinaryStringEncoding) 0;
          IBuffer buffMsg = CryptographicBuffer.ConvertStringToBinary(_token, encoding);
          IBuffer buffProtected = await Provider.ProtectAsync(buffMsg);
          byte[] protectedDataArray = buffProtected.ToArray();
          this.token = protectedDataArray;
        }
        else
          this.token = (byte[]) null;
      }
      catch (Exception ex)
      {
        LogFile.Log("Problem in setting token. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public async Task<string> getToken()
    {
      try
      {
        if (this.token == null)
          return string.Empty;
        BinaryStringEncoding encoding = (BinaryStringEncoding) 0;
        DataProtectionProvider Provider = new DataProtectionProvider();
        IBuffer buffUnprotected = await Provider.UnprotectAsync(this.token.AsBuffer());
        string strClearText = CryptographicBuffer.ConvertBinaryToString(encoding, buffUnprotected);
        return strClearText;
      }
      catch (Exception ex)
      {
        LogFile.Log("Problem in reading token. " + ex.Message, EventType.Error);
        return string.Empty;
      }
    }

    public async Task saveContact(Avatar jsonAvatar, string MCI_CID)
    {
      try
      {
        string json = JsonConvert.SerializeObject((object) jsonAvatar, new JsonSerializerSettings()
        {
          NullValueHandling = NullValueHandling.Ignore
        });
        StorageFolder backupFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Backup", (CreationCollisionOption) 3);
        StorageFile backupFile = (StorageFile) null;
        while (backupFile == null)
        {
          try
          {
            backupFile = await backupFolder.CreateFileAsync(MCI_CID + ".jcf", (CreationCollisionOption) 3);
          }
          catch
          {
          }
        }
        await FileIO.WriteTextAsync((IStorageFile) backupFile, json);
      }
      catch (Exception ex)
      {
        string message = MCI_CID + " : Error in saving contact to file. " + ex.Message;
        LogFile.Log(message, EventType.Error);
        CommonCode.commonCode.reportBug("Sync", message);
      }
    }

    public async Task<string> loadContact(string MCI_CID)
    {
      try
      {
        StorageFolder backupFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Backup", (CreationCollisionOption) 3);
        StorageFile backupFile = (StorageFile) null;
        string value = string.Empty;
        while (backupFile == null)
        {
          try
          {
            backupFile = await backupFolder.CreateFileAsync(MCI_CID + ".jcf", (CreationCollisionOption) 3);
          }
          catch
          {
          }
        }
        value = await FileIO.ReadTextAsync((IStorageFile) backupFile);
        return value;
      }
      catch (Exception ex)
      {
        string message = MCI_CID + " : Error in reading contact from file. " + ex.Message;
        LogFile.Log(message, EventType.Error);
        CommonCode.commonCode.reportBug("Sync", message);
        return (string) null;
      }
    }

    public async Task deleteContactFile(string MCI_CID)
    {
      try
      {
        StorageFolder backupFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Backup", (CreationCollisionOption) 3);
        StorageFile backupFile = (StorageFile) null;
        while (backupFile == null)
        {
          try
          {
            backupFile = await backupFolder.CreateFileAsync(MCI_CID + ".jcf", (CreationCollisionOption) 3);
          }
          catch
          {
          }
        }
        await backupFile.DeleteAsync((StorageDeleteOption) 1);
      }
      catch (Exception ex)
      {
        string message = MCI_CID + " : Error in deleting contact file. " + ex.Message;
        LogFile.Log(message, EventType.Error);
        CommonCode.commonCode.reportBug("Sync", message);
      }
    }

    public async Task deleteContactFiles(List<string> MCI_CIDList)
    {
      try
      {
        StorageFolder backupFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Backup", (CreationCollisionOption) 3);
        for (int i = 0; i < MCI_CIDList.Count; ++i)
        {
          string MCI_CID = MCI_CIDList[i];
          StorageFile backupFile = (StorageFile) null;
          while (backupFile == null)
          {
            try
            {
              backupFile = await backupFolder.CreateFileAsync(MCI_CID + ".jcf", (CreationCollisionOption) 3);
            }
            catch
            {
            }
          }
          try
          {
            await backupFile.DeleteAsync((StorageDeleteOption) 1);
          }
          catch (Exception ex)
          {
            string message = MCI_CID + " : Error in deleting contact file. " + ex.Message;
            LogFile.Log(message, EventType.Error);
            CommonCode.commonCode.reportBug("Sync", message);
          }
        }
      }
      catch
      {
      }
    }

    public async Task deleteAllContactFiles()
    {
      try
      {
        StorageFolder backupFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Backup", (CreationCollisionOption) 3);
        await backupFolder.DeleteAsync((StorageDeleteOption) 1);
      }
      catch (Exception ex)
      {
        string message = "Error in deleting all contact files. " + ex.Message;
        LogFile.Log(message + ex.Message, EventType.Error);
        CommonCode.commonCode.reportBug("Sync", message);
      }
    }
  }
}
