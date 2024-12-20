// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.Sync
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Phone.PersonalInformation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

#nullable disable
namespace InTouchLibrary
{
  public class Sync
  {
    private static volatile Sync _sync;
    public static TextBlock syncStatusBlock;
    public static TextBlock contactsManagedBlock;
    public static TextBlock imagesStatusBlock;
    public static TextBlock lastSyncTimeBlock;
    public static ToggleSwitch downloadPhotoOnWiFiTS;
    public static ListView contactListView;
    public static ObservableCollection<ContactSample> contactSamples = new ObservableCollection<ContactSample>();

    public static Sync sync
    {
      get
      {
        try
        {
          if (Sync._sync == null)
            Sync._sync = new Sync();
          return Sync._sync;
        }
        catch
        {
          throw;
        }
      }
    }

    public static bool isSyncRunning
    {
      get
      {
        try
        {
          if (string.IsNullOrEmpty(LocalSettings.localSettings.syncSessionLastUpdateTime))
            return false;
          DateTime t2 = DateTime.ParseExact(LocalSettings.localSettings.syncSessionLastUpdateTime, "d MMM, hh.mm tt", (IFormatProvider) CultureInfo.InvariantCulture);
          if (LocalSettings.localSettings.syncSessionState != 7 && LocalSettings.localSettings.syncSessionState != 0 || !(LocalSettings.localSettings.syncSessionOwner == SyncStateOwner.App.ToString()))
            t2 = t2.AddMinutes(5.0);
          if ((LocalSettings.localSettings.syncSessionState <= 0 || DateTime.Compare(DateTime.Now, t2) < 0) && LocalSettings.localSettings.syncSessionState > 0)
            return true;
          if (LocalSettings.localSettings.syncSessionState != 7 && LocalSettings.localSettings.syncSessionState != 0)
          {
            LocalSettings.localSettings.syncSessionState = -1;
            Sync.setImageStatusBlock(string.Empty);
            Sync.setSyncStatusBlock(string.Empty);
          }
          return false;
        }
        catch (Exception ex)
        {
          LogFile.Log("Problem in finding whether current sync is running. " + ex.Message, EventType.Warning);
          return false;
        }
      }
    }

    public static bool shallDownload
    {
      get
      {
        try
        {
          return string.IsNullOrEmpty(LocalSettings.localSettings.lastSyncTime) || DateTime.Compare(DateTime.Now, DateTime.ParseExact(LocalSettings.localSettings.lastSyncTime, "d MMM, hh.mm tt", (IFormatProvider) CultureInfo.InvariantCulture).AddHours(4.0)) > 0;
        }
        catch (Exception ex)
        {
          LogFile.Log("Problem in shall download. " + ex.Message, EventType.Warning);
          return false;
        }
      }
    }

    public async Task startSync(bool shallDownload)
    {
      try
      {
        if (LocalSettings.localSettings.syncSessionState == 7)
          await RestoreContactStore.restoreContactStore.restoreContacts((List<string>) null, false);
        else if (ContactstoreInTouch.contactStore == null)
        {
          Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
          if (result.Item2)
            await RestoreContactStore.restoreContactStore.restoreContacts((List<string>) null, true);
        }
        await this.uploadContacts();
        if (shallDownload)
          await this.downloadContacts();
        else
          LocalSettings.localSettings.syncSessionState = 0;
        await CommonCode.commonCode.bugReporter();
      }
      catch (Exception ex)
      {
        string empty = string.Empty;
        string text;
        if (ex is NotSupportedException)
        {
          if (Sync.syncStatusBlock != null)
            Sync.syncStatusBlock.put_FontSize(18.0);
          text = ex.Message;
        }
        else
          text = "Unable to sync";
        Sync.setImageStatusBlock(string.Empty);
        Sync.setSyncStatusBlock(text);
        throw;
      }
    }

    private async Task deleteContactWithoutNumber()
    {
      try
      {
        List<string> IDList = new List<string>();
        ContactQueryResult QueryResult = ContactstoreInTouch.contactStore.CreateContactQuery();
        IReadOnlyList<StoredContact> _contactList = (IReadOnlyList<StoredContact>) null;
        if (QueryResult != null)
        {
          _contactList = await QueryResult.GetContactsAsync();
          if (_contactList != null)
          {
            foreach (StoredContact storedContact in (IEnumerable<StoredContact>) _contactList)
            {
              IDictionary<string, object> props = await storedContact.GetPropertiesAsync();
              bool phone_exists = false;
              if (props != null)
              {
                foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) props)
                {
                  if (!phone_exists)
                  {
                    if (keyValuePair.Value != null)
                    {
                      switch (keyValuePair.Key)
                      {
                        case "MobileTelephone":
                          phone_exists = true;
                          continue;
                        case "AlternateMobileTelephone":
                          phone_exists = true;
                          continue;
                        case "Telephone":
                          phone_exists = true;
                          continue;
                        case "WorkTelephone":
                          phone_exists = true;
                          continue;
                        case "AlternateWorkTelephone":
                          phone_exists = true;
                          continue;
                        case "HomeFax":
                          phone_exists = true;
                          continue;
                        case "WorkFax":
                          phone_exists = true;
                          continue;
                        case "CompanyPhone":
                          phone_exists = true;
                          continue;
                        case "Telephone2":
                          phone_exists = true;
                          continue;
                        default:
                          continue;
                      }
                    }
                  }
                  else
                    break;
                }
              }
              if (!phone_exists)
                IDList.Add(storedContact.Id);
            }
          }
        }
        await InTouchAppDatabase.InTouchAppDB.readAndMarkDeletedDirtyEntries(LocalSettings.localSettings.MCI, IDList);
      }
      catch
      {
      }
    }

    public async Task uploadContacts()
    {
      try
      {
        string sessionID = CommonCode.commonCode.createRandomStringSession();
        LocalSettings.localSettings.syncSessionID = sessionID;
        Sync.setContactsManagedBlock();
        Sync.setImageStatusBlock(string.Empty);
        if (LocalSettings.localSettings.syncOtherContacts)
        {
          Stopwatch other = new Stopwatch();
          other.Start();
          Sync.setSyncStatusBlock("Reading phone contacts");
          LocalSettings.localSettings.syncSessionState = 6;
          await ContactstoreOther.contactStoreOtherObj.findChangesInWPOtherContacts();
          LocalSettings.localSettings.syncOtherContacts = false;
          LocalSettings.localSettings.syncSessionLastUpdateTime = DateTime.Now.ToString("d MMM, hh.mm ") + DateTime.Now.ToString("tt").ToLower();
          other.Stop();
          LogFile.Log("Time to get updated other contacts, " + (object) other.ElapsedMilliseconds, EventType.Test);
        }
        else
          Sync.setSyncStatusBlock("Checking modified contacts");
        Stopwatch intouch = new Stopwatch();
        intouch.Start();
        if (ContactstoreInTouch.contactStore == null)
        {
          Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
          if (result.Item2)
            await RestoreContactStore.restoreContactStore.restoreContacts((List<string>) null, true);
        }
        LocalSettings.localSettings.syncSessionState = 5;
        Sync.setSyncStatusBlock("Checking modified contacts");
        Tuple<List<string>, List<Avatar>> result1 = await ContactstoreInTouch.contactStoreInTouch.findChangesInWPInTouchContacts();
        intouch.Stop();
        LogFile.Log("Time to get updated InTouch contacts, " + (object) intouch.ElapsedMilliseconds, EventType.Test);
        LocalSettings.localSettings.lastCheckedTime = DateTime.Now.ToString("d MMM, hh.mm ") + DateTime.Now.ToString("tt").ToLower();
        LocalSettings.localSettings.syncSessionLastUpdateTime = DateTime.Now.ToString("d MMM, hh.mm ") + DateTime.Now.ToString("tt").ToLower();
        Sync.setContactsManagedBlock();
        LocalSettings.localSettings.syncSessionState = 4;
        Upload upload = new Upload();
        await upload.uploadWPContacts(result1.Item1, result1.Item2);
        LocalSettings.localSettings.lastCheckedTime = DateTime.Now.ToString("d MMM, hh.mm ") + DateTime.Now.ToString("tt").ToLower();
      }
      catch
      {
        throw;
      }
    }

    public async Task downloadContacts()
    {
      try
      {
        Sync.setContactsManagedBlock();
        LogFile.Log("Downloading started.", EventType.Information);
        LocalSettings.localSettings.syncSessionState = 3;
        LogFile.Log("Downloading Manual contacts.", EventType.Information);
        await Download.download.downloadManualContacts();
        LocalSettings.localSettings.syncSessionState = 2;
        LogFile.Log("Downloading Auto contacts.", EventType.Information);
        await Download.download.downloadAutoContacts();
        Sync.setSyncStatusBlock("All contacts are up to date");
        ServerConnectionManager SCMobj = new ServerConnectionManager();
        if (LocalSettings.localSettings.downloadPhotoOnWifi)
        {
          if (LocalSettings.localSettings.downloadPhotoOnWifi)
          {
            if (!SCMobj.mIsWiFiAvailable)
              goto label_9;
          }
          else
            goto label_9;
        }
        try
        {
          LocalSettings.localSettings.syncSessionState = 1;
          Sync.setDownloadPhotoOnWiFiTS(false);
          await Download.download.setAllPhotoToWP();
          LocalSettings.localSettings.syncSessionLastUpdateTime = DateTime.Now.ToString("d MMM, hh.mm ") + DateTime.Now.ToString("tt").ToLower();
          Sync.setDownloadPhotoOnWiFiTS(true);
        }
        catch
        {
          Sync.setDownloadPhotoOnWiFiTS(true);
          throw;
        }
label_9:
        Sync.setImageStatusBlock("Sync Completed");
        await ContactstoreInTouch.contactStoreInTouch.setRevisionNumber();
        LocalSettings.localSettings.syncSessionState = 0;
        LogFile.Log("Sync completed.", EventType.Information);
        Sync.setLastSyncTimeBlock();
        LocalSettings.localSettings.syncSessionLastUpdateTime = DateTime.Now.ToString("d MMM, hh.mm ") + DateTime.Now.ToString("tt").ToLower();
      }
      catch
      {
        throw;
      }
    }

    public static void setContactListView()
    {
      try
      {
        CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
        // ISSUE: reference to a compiler-generated field
        if (Sync.CS\u0024\u003C\u003E9__CachedAnonymousMethodDelegate24 == null)
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: method pointer
          Sync.CS\u0024\u003C\u003E9__CachedAnonymousMethodDelegate24 = new DispatchedHandler((object) null, __methodptr(\u003CsetContactListView\u003Eb__20));
        }
        // ISSUE: reference to a compiler-generated field
        DispatchedHandler methodDelegate24 = Sync.CS\u0024\u003C\u003E9__CachedAnonymousMethodDelegate24;
        dispatcher.RunAsync((CoreDispatcherPriority) 1, methodDelegate24);
      }
      catch
      {
      }
    }

    public static void setContactsManagedBlock()
    {
      try
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        Sync.\u003C\u003Ec__DisplayClass29 cDisplayClass29 = new Sync.\u003C\u003Ec__DisplayClass29();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass29.text = "Contacts Managed : " + (object) InTouchAppDatabase.InTouchAppDB.countInTouchEntriesFromLT(LocalSettings.localSettings.MCI);
        // ISSUE: reference to a compiler-generated field
        LocalSettings.localSettings.contactsManagedBlock = cDisplayClass29.text;
        // ISSUE: method pointer
        CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync((CoreDispatcherPriority) 1, new DispatchedHandler((object) cDisplayClass29, __methodptr(\u003CsetContactsManagedBlock\u003Eb__28)));
      }
      catch
      {
      }
    }

    public static void setSyncStatusBlock(string text)
    {
      DispatchedHandler dispatchedHandler1 = (DispatchedHandler) null;
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      Sync.\u003C\u003Ec__DisplayClass2d cDisplayClass2d = new Sync.\u003C\u003Ec__DisplayClass2d();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass2d.text = text;
      try
      {
        // ISSUE: reference to a compiler-generated field
        LocalSettings.localSettings.syncStatusBlock = cDisplayClass2d.text;
        CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
        if (dispatchedHandler1 == null)
        {
          // ISSUE: method pointer
          dispatchedHandler1 = new DispatchedHandler((object) cDisplayClass2d, __methodptr(\u003CsetSyncStatusBlock\u003Eb__2b));
        }
        DispatchedHandler dispatchedHandler2 = dispatchedHandler1;
        dispatcher.RunAsync((CoreDispatcherPriority) 1, dispatchedHandler2);
      }
      catch
      {
      }
    }

    public static void setImageStatusBlock(string text)
    {
      DispatchedHandler dispatchedHandler1 = (DispatchedHandler) null;
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      Sync.\u003C\u003Ec__DisplayClass31 cDisplayClass31 = new Sync.\u003C\u003Ec__DisplayClass31();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass31.text = text;
      try
      {
        // ISSUE: reference to a compiler-generated field
        LocalSettings.localSettings.imageStatusBlock = cDisplayClass31.text;
        CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
        if (dispatchedHandler1 == null)
        {
          // ISSUE: method pointer
          dispatchedHandler1 = new DispatchedHandler((object) cDisplayClass31, __methodptr(\u003CsetImageStatusBlock\u003Eb__2f));
        }
        DispatchedHandler dispatchedHandler2 = dispatchedHandler1;
        dispatcher.RunAsync((CoreDispatcherPriority) 1, dispatchedHandler2);
      }
      catch
      {
      }
    }

    private static void setLastSyncTimeBlock()
    {
      try
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        Sync.\u003C\u003Ec__DisplayClass34 cDisplayClass34 = new Sync.\u003C\u003Ec__DisplayClass34();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass34.time = DateTime.Now.ToString("d MMM, hh.mm ") + DateTime.Now.ToString("tt").ToLower();
        // ISSUE: reference to a compiler-generated field
        LocalSettings.localSettings.lastSyncTime = cDisplayClass34.time;
        // ISSUE: method pointer
        CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync((CoreDispatcherPriority) 1, new DispatchedHandler((object) cDisplayClass34, __methodptr(\u003CsetLastSyncTimeBlock\u003Eb__33)));
      }
      catch
      {
      }
    }

    private static void setDownloadPhotoOnWiFiTS(bool shallEnable)
    {
      DispatchedHandler dispatchedHandler1 = (DispatchedHandler) null;
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      Sync.\u003C\u003Ec__DisplayClass38 cDisplayClass38 = new Sync.\u003C\u003Ec__DisplayClass38();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass38.shallEnable = shallEnable;
      try
      {
        // ISSUE: reference to a compiler-generated field
        LocalSettings.localSettings.downloadPhotoOnWifiTSEnable = cDisplayClass38.shallEnable;
        CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
        if (dispatchedHandler1 == null)
        {
          // ISSUE: method pointer
          dispatchedHandler1 = new DispatchedHandler((object) cDisplayClass38, __methodptr(\u003CsetDownloadPhotoOnWiFiTS\u003Eb__36));
        }
        DispatchedHandler dispatchedHandler2 = dispatchedHandler1;
        dispatcher.RunAsync((CoreDispatcherPriority) 1, dispatchedHandler2);
      }
      catch
      {
      }
    }
  }
}
