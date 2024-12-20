// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.Download
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Phone.PersonalInformation;
using Windows.Storage.Streams;

#nullable disable
namespace InTouchLibrary
{
  public class Download
  {
    private static volatile Download _download;
    private ServerConnectionManager SCMobj = new ServerConnectionManager();
    private Stopwatch _downloadManual = new Stopwatch();
    private Stopwatch _downloadManualWithAPI = new Stopwatch();
    private Stopwatch _downloadAuto = new Stopwatch();
    private Stopwatch _downloadAutoWithAPI = new Stopwatch();
    private Stopwatch _Store = new Stopwatch();
    private Stopwatch _DB = new Stopwatch();
    private Stopwatch _File = new Stopwatch();

    public static Download download
    {
      get
      {
        try
        {
          if (Download._download == null)
            Download._download = new Download();
          return Download._download;
        }
        catch
        {
          throw;
        }
      }
    }

    public async Task downloadManualContacts()
    {
      try
      {
        this._downloadManual.Restart();
        this._downloadManualWithAPI.Restart();
        this._DB.Reset();
        this._File.Reset();
        this._Store.Reset();
        RootObjectContactbookManual contactsManual = new RootObjectContactbookManual();
        string ID = string.Empty;
        this._downloadManual.Stop();
        if (this.SCMobj.mIsConnectedToNetwork)
        {
          Sync.setSyncStatusBlock("Downloading Contacts");
          contactsManual = await this.SCMobj.downloadContactbookManual(await LocalSettings.localSettings.getToken(), LocalSettings.localSettings.versionManual);
          this._downloadManual.Start();
          while (LocalSettings.localSettings.versionManual != contactsManual.version)
          {
            // ISSUE: reference to a compiler-generated field
            if (Download.\u003CdownloadManualContacts\u003Eo__SiteContainer0.\u003C\u003Ep__Site1 == null)
            {
              // ISSUE: reference to a compiler-generated field
              Download.\u003CdownloadManualContacts\u003Eo__SiteContainer0.\u003C\u003Ep__Site1 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (Download)));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, string> target = Download.\u003CdownloadManualContacts\u003Eo__SiteContainer0.\u003C\u003Ep__Site1.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, string>> pSite1 = Download.\u003CdownloadManualContacts\u003Eo__SiteContainer0.\u003C\u003Ep__Site1;
            // ISSUE: reference to a compiler-generated field
            if (Download.\u003CdownloadManualContacts\u003Eo__SiteContainer0.\u003C\u003Ep__Site2 == null)
            {
              // ISSUE: reference to a compiler-generated field
              Download.\u003CdownloadManualContacts\u003Eo__SiteContainer0.\u003C\u003Ep__Site2 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", (IEnumerable<Type>) null, typeof (Download), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.IsStaticType | CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj = Download.\u003CdownloadManualContacts\u003Eo__SiteContainer0.\u003C\u003Ep__Site2.Target((CallSite) Download.\u003CdownloadManualContacts\u003Eo__SiteContainer0.\u003C\u003Ep__Site2, typeof (Convert), contactsManual.contacts);
            string jsonResponse = target((CallSite) pSite1, obj);
            JArray a = JArray.Parse(jsonResponse);
            for (int i = 0; i < a.Count; ++i)
            {
              ContactContactbookManual manualContact = new ContactContactbookManual();
              string contactStr = a[i].ToString();
              try
              {
                manualContact = JsonConvert.DeserializeObject<ContactContactbookManual>(contactStr);
              }
              catch (Exception ex)
              {
                string empty = string.Empty;
                try
                {
                  if (contactStr.Contains("contact_id_1"))
                    empty = Regex.Match(contactStr.Substring(contactStr.IndexOf("contact_id_1") + 15, 10), "\\d+").Value;
                }
                catch
                {
                  LogFile.Log("Error in retrieving contact_id_1.", EventType.Error);
                }
                string message = empty + " : Error in deserializing contact. " + ex.Message;
                LogFile.Log(message, EventType.Error);
                CommonCode.commonCode.reportBug("Sync", message);
                continue;
              }
              string displayName = CommonCode.commonCode.getDisplayName(manualContact.avatars, manualContact.name);
              ID = InTouchAppDatabase.InTouchAppDB.readID(LocalSettings.localSettings.MCI, manualContact.contact_id_1);
              string baseVersion = Convert.ToString(manualContact.version);
              if (!string.IsNullOrEmpty(ID))
              {
                LogFile.Log("Read " + displayName + " from DB.", EventType.Debug);
                if (ContactstoreInTouch.contactStore == null)
                {
                  Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
                  if (result.Item2)
                    await RestoreContactStore.restoreContactStore.restoreContacts((List<string>) null, true);
                }
                StoredContact WPContact = (StoredContact) null;
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
                  if (!string.Equals(manualContact.connection_status, "deleted", StringComparison.OrdinalIgnoreCase))
                  {
                    IDictionary<string, object> props = await WPContact.GetPropertiesAsync();
                    this.setManualContactToWP(manualContact, WPContact, props);
                    await WPContact.SaveAsync();
                    await ContactstoreInTouch.contactStoreInTouch.setRevisionNumber();
                    LogFile.Log("Updated " + displayName + " " + manualContact.contact_id_1 + " to WP.", EventType.Debug);
                    Avatar jsonAvatar = await ContactstoreInTouch.contactStoreInTouch.getManualContactFromWP(WPContact, props);
                    Contacts contact = Contacts.getContact(jsonAvatar.name, jsonAvatar.organization);
                    InTouchAppDatabase.InTouchAppDB.updateBaseVersionAndContactsInfo(LocalSettings.localSettings.MCI, manualContact.contact_id_1, baseVersion, contact);
                    await this.updateImageHandlerTable(manualContact.avatars, manualContact.contact_id_1, WPContact);
                    await LocalSettings.localSettings.saveContact(jsonAvatar, manualContact.contact_id_1);
                  }
                  else
                  {
                    try
                    {
                      if (ContactstoreInTouch.contactStore == null)
                      {
                        Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
                        if (result.Item2)
                          await RestoreContactStore.restoreContactStore.restoreContacts((List<string>) null, true);
                      }
                      await ContactstoreInTouch.contactStore.DeleteContactAsync(ID);
                      await LocalSettings.localSettings.deleteContactFile(manualContact.contact_id_1);
                    }
                    catch (Exception ex)
                    {
                      LogFile.Log("Error in deleting contact " + ID + ". " + ex.Message, EventType.Error);
                    }
                    await ContactstoreInTouch.contactStoreInTouch.setRevisionNumber();
                    InTouchAppDatabase.InTouchAppDB.deleteEntry(LocalSettings.localSettings.MCI, manualContact.contact_id_1);
                    Sync.setContactsManagedBlock();
                    LogFile.Log("Deleted " + displayName + "  from DB and WP as deleted on server.", EventType.Debug);
                  }
                }
                else
                {
                  InTouchAppDatabase.InTouchAppDB.deleteEntry(LocalSettings.localSettings.MCI, manualContact.contact_id_1);
                  LogFile.Log("Deleted " + displayName + " from DB as it doesn't exists in WP.", EventType.Debug);
                  if (!string.Equals(manualContact.connection_status, "deleted", StringComparison.OrdinalIgnoreCase))
                    await this.handleWPNonExistingManualContact(manualContact, baseVersion, displayName);
                  Sync.setContactsManagedBlock();
                }
              }
              else if (!string.Equals(manualContact.connection_status, "deleted", StringComparison.OrdinalIgnoreCase))
              {
                await this.handleWPNonExistingManualContact(manualContact, baseVersion, displayName);
                Sync.setContactsManagedBlock();
              }
              Sync.setSyncStatusBlock("Saving contacts\n" + displayName);
            }
            if (!LocalSettings.localSettings.isBackground && a.Count > 0)
              Sync.setContactListView();
            LocalSettings.localSettings.versionManual = contactsManual.version;
            LocalSettings.localSettings.syncSessionLastUpdateTime = DateTime.Now.ToString("d MMM, hh.mm ") + DateTime.Now.ToString("tt").ToLower();
            this._downloadManual.Stop();
            if (this.SCMobj.mIsConnectedToNetwork)
            {
              Sync.setSyncStatusBlock("Downloading Contacts");
              contactsManual = await this.SCMobj.downloadContactbookManual(await LocalSettings.localSettings.getToken(), LocalSettings.localSettings.versionManual);
              this._downloadManual.Start();
            }
            else
            {
              LogFile.Log("Internet connection is not available.", EventType.Warning);
              throw new NotSupportedException("Unable to sync." + Environment.NewLine + "Please check internet connectivity.");
            }
          }
          LogFile.Log("Synced all manual contacts.", EventType.Information);
          this._downloadManual.Stop();
          this._downloadManualWithAPI.Stop();
          LogFile.Log("Time to download manual contacts, " + (object) this._downloadManual.ElapsedMilliseconds, EventType.Test);
          LogFile.Log("Time to download manual contacts with API, " + (object) this._downloadManualWithAPI.ElapsedMilliseconds, EventType.Test);
          LogFile.Log("DB, " + (object) this._DB.ElapsedMilliseconds, EventType.Test);
          LogFile.Log("Store, " + (object) this._Store.ElapsedMilliseconds, EventType.Test);
          LogFile.Log("File, " + (object) this._File.ElapsedMilliseconds, EventType.Test);
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
          LogFile.Log("Problem in syncing manual contacts due to internet connectivity.", EventType.Warning);
        else
          LogFile.Log("Error while syncing manual contacts. " + ex.Message, EventType.Error);
        throw;
      }
    }

    private async Task handleWPNonExistingManualContact(
      ContactContactbookManual manualContact,
      string baseVersion,
      string displayName)
    {
      try
      {
        if (ContactstoreInTouch.contactStore == null)
        {
          Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
          if (result.Item2)
            await RestoreContactStore.restoreContactStore.restoreContacts((List<string>) null, true);
        }
        this._Store.Start();
        StoredContact newContact = new StoredContact(ContactstoreInTouch.contactStore);
        IDictionary<string, object> props = await newContact.GetPropertiesAsync();
        this.setManualContactToWP(manualContact, newContact, props);
        await newContact.SaveAsync();
        await ContactstoreInTouch.contactStoreInTouch.setRevisionNumber();
        this._Store.Stop();
        LogFile.Log("Added " + displayName + " " + manualContact.contact_id_1 + " to WP.", EventType.Debug);
        this._DB.Start();
        string ID = newContact.Id;
        Lookup lookup = new Lookup();
        lookup.ID = ID;
        lookup.MCI_CID = manualContact.contact_id_1;
        lookup.BASE_VERSION = baseVersion;
        lookup.Dirty = 0;
        lookup.CONTACT_TYPE = 1;
        lookup.HASH = string.Empty;
        ImageHandler imageHandler = new ImageHandler();
        imageHandler.ID = ID;
        imageHandler.MCI_CID = manualContact.contact_id_1;
        Tuple<string, string> result1 = await this.getPhotoURLData(manualContact.avatars, manualContact.contact_id_1, newContact, false);
        imageHandler.CONTACT_PHOTO_URL = result1.Item1;
        imageHandler.DOWNLOAD_STATE = string.IsNullOrEmpty(imageHandler.CONTACT_PHOTO_URL);
        imageHandler.HASH = result1.Item2;
        this._File.Start();
        Avatar jsonAvatar = await ContactstoreInTouch.contactStoreInTouch.getManualContactFromWP(newContact, props);
        await LocalSettings.localSettings.saveContact(jsonAvatar, manualContact.contact_id_1);
        this._File.Stop();
        if (jsonAvatar.name != null)
        {
          lookup.name_family = jsonAvatar.name.family;
          lookup.name_given = jsonAvatar.name.given;
          lookup.name_middle = jsonAvatar.name.middle;
          lookup.name_nickname = jsonAvatar.name.nickname;
          lookup.name_prefix = jsonAvatar.name.prefix;
          lookup.name_suffix = jsonAvatar.name.suffix;
        }
        if (jsonAvatar.organization != null && jsonAvatar.organization.Count != 0)
        {
          lookup.company_name = jsonAvatar.organization[0].company;
          lookup.job_title = jsonAvatar.organization[0].position;
        }
        InTouchAppDatabase.InTouchAppDB.addEntry(LocalSettings.localSettings.MCI, lookup, imageHandler);
        this._DB.Stop();
        LogFile.Log("Added " + displayName + " to DB.", EventType.Debug);
      }
      catch (Exception ex)
      {
        LogFile.Log("Error while setting manual contact " + manualContact.contact_id_1 + " to WP. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public async Task downloadAutoContacts()
    {
      try
      {
        this._downloadAuto.Restart();
        this._downloadAutoWithAPI.Restart();
        RootObjectContactbookAuto contactsAuto = new RootObjectContactbookAuto();
        string ID = string.Empty;
        int i = 0;
        this._downloadAuto.Stop();
        if (this.SCMobj.mIsConnectedToNetwork)
        {
          Sync.setSyncStatusBlock("Downloading Contacts");
          contactsAuto = await this.SCMobj.downloadContactbookAuto(await LocalSettings.localSettings.getToken(), LocalSettings.localSettings.versionAuto);
          this._downloadAuto.Start();
          while (LocalSettings.localSettings.versionAuto != contactsAuto.version)
          {
            if (contactsAuto.contacts != null)
            {
              foreach (ContactContactbookAuto autoContact in contactsAuto.contacts)
              {
                string displayName = CommonCode.commonCode.getDisplayName(autoContact.avatars, (Name) null);
                ++i;
                string MCI_CID = Convert.ToString(autoContact.mci);
                ID = InTouchAppDatabase.InTouchAppDB.readID(LocalSettings.localSettings.MCI, MCI_CID);
                if (!string.IsNullOrEmpty(ID))
                {
                  LogFile.Log("Read " + displayName + " from DB.", EventType.Debug);
                  if (ContactstoreInTouch.contactStore == null)
                  {
                    Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
                    if (result.Item2)
                      await RestoreContactStore.restoreContactStore.restoreContacts((List<string>) null, true);
                  }
                  StoredContact WPContact = (StoredContact) null;
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
                    if (!string.Equals(autoContact.connection_status, "deleted", StringComparison.OrdinalIgnoreCase))
                    {
                      IDictionary<string, object> props = await WPContact.GetPropertiesAsync();
                      this.setAutoContactToWP(autoContact, WPContact, props, MCI_CID);
                      await WPContact.SaveAsync();
                      await ContactstoreInTouch.contactStoreInTouch.setRevisionNumber();
                      LogFile.Log("Updated " + displayName + " " + MCI_CID + " to WP.", EventType.Debug);
                      Avatar jsonAvatar = await ContactstoreInTouch.contactStoreInTouch.getManualContactFromWP(WPContact, props);
                      Contacts contact = Contacts.getContact(jsonAvatar.name, jsonAvatar.organization);
                      InTouchAppDatabase.InTouchAppDB.updateBaseVersionAndContactsInfo(LocalSettings.localSettings.MCI, MCI_CID, (string) null, contact);
                      await this.updateImageHandlerTable(autoContact.avatars, MCI_CID, WPContact);
                      await LocalSettings.localSettings.saveContact(jsonAvatar, MCI_CID);
                    }
                    else
                    {
                      try
                      {
                        if (ContactstoreInTouch.contactStore == null)
                        {
                          Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
                          if (result.Item2)
                            await RestoreContactStore.restoreContactStore.restoreContacts((List<string>) null, true);
                        }
                        await ContactstoreInTouch.contactStore.DeleteContactAsync(ID);
                        await LocalSettings.localSettings.deleteContactFile(MCI_CID);
                      }
                      catch (Exception ex)
                      {
                        LogFile.Log("Error in deleting contact " + ID + ". " + ex.Message, EventType.Error);
                      }
                      await ContactstoreInTouch.contactStoreInTouch.setRevisionNumber();
                      InTouchAppDatabase.InTouchAppDB.deleteEntry(LocalSettings.localSettings.MCI, MCI_CID);
                      Sync.setContactsManagedBlock();
                      LogFile.Log("Deleted " + displayName + MCI_CID + "  from DB and WP as deleted on server.", EventType.Debug);
                    }
                  }
                  else
                  {
                    InTouchAppDatabase.InTouchAppDB.deleteEntry(LocalSettings.localSettings.MCI, MCI_CID);
                    LogFile.Log("Deleted " + displayName + " " + MCI_CID + "  from DB as it doesn't exists in WP.", EventType.Debug);
                    if (!string.Equals(autoContact.connection_status, "deleted", StringComparison.OrdinalIgnoreCase))
                      await this.handleWPNonExistingAutoContact(autoContact, displayName, MCI_CID);
                    Sync.setContactsManagedBlock();
                  }
                }
                else if (!string.Equals(autoContact.connection_status, "deleted", StringComparison.OrdinalIgnoreCase))
                {
                  await this.handleWPNonExistingAutoContact(autoContact, displayName, MCI_CID);
                  Sync.setContactsManagedBlock();
                }
                Sync.setSyncStatusBlock("Saving contacts\n" + displayName);
              }
            }
            if (!LocalSettings.localSettings.isBackground && contactsAuto.contacts.Count > 0)
              Sync.setContactListView();
            LocalSettings.localSettings.versionAuto = contactsAuto.version;
            LocalSettings.localSettings.syncSessionLastUpdateTime = DateTime.Now.ToString("d MMM, hh.mm ") + DateTime.Now.ToString("tt").ToLower();
            this._downloadAuto.Stop();
            if (this.SCMobj.mIsConnectedToNetwork)
            {
              Sync.setSyncStatusBlock("Downloading Contacts");
              contactsAuto = await this.SCMobj.downloadContactbookAuto(await LocalSettings.localSettings.getToken(), LocalSettings.localSettings.versionAuto);
              this._downloadAuto.Start();
            }
            else
            {
              LogFile.Log("Internet connection is not available.", EventType.Warning);
              throw new NotSupportedException("Unable to sync." + Environment.NewLine + "Please check internet connectivity.");
            }
          }
          LogFile.Log("Synced all auto contacts.", EventType.Information);
          this._downloadAuto.Stop();
          this._downloadAutoWithAPI.Stop();
          LogFile.Log("Time to download auto contacts, " + (object) this._downloadAuto.ElapsedMilliseconds, EventType.Test);
          LogFile.Log("Time to download auto contacts with API, " + (object) this._downloadAutoWithAPI.ElapsedMilliseconds, EventType.Test);
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
          LogFile.Log("Problem in syncing auto contacts due to internet connectivity.", EventType.Warning);
        else
          LogFile.Log("Error while syncing auto contacts. " + ex.Message, EventType.Error);
        throw;
      }
    }

    private async Task handleWPNonExistingAutoContact(
      ContactContactbookAuto autoContact,
      string displayName,
      string MCI_CID)
    {
      try
      {
        if (ContactstoreInTouch.contactStore == null)
        {
          Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
          if (result.Item2)
            await RestoreContactStore.restoreContactStore.restoreContacts((List<string>) null, true);
        }
        StoredContact newContact = new StoredContact(ContactstoreInTouch.contactStore);
        IDictionary<string, object> props = await newContact.GetPropertiesAsync();
        this.setAutoContactToWP(autoContact, newContact, props, MCI_CID);
        await newContact.SaveAsync();
        await ContactstoreInTouch.contactStoreInTouch.setRevisionNumber();
        LogFile.Log("Added " + displayName + " " + MCI_CID + " to WP.", EventType.Debug);
        string ID = newContact.Id;
        Lookup lookup = new Lookup();
        lookup.ID = ID;
        lookup.MCI_CID = MCI_CID;
        lookup.BASE_VERSION = "-2";
        lookup.Dirty = 0;
        lookup.CONTACT_TYPE = 0;
        lookup.HASH = string.Empty;
        ImageHandler imageHandler = new ImageHandler();
        imageHandler.ID = ID;
        imageHandler.MCI_CID = MCI_CID;
        Tuple<string, string> result1 = await this.getPhotoURLData(autoContact.avatars, MCI_CID, newContact, false);
        imageHandler.CONTACT_PHOTO_URL = result1.Item1;
        imageHandler.DOWNLOAD_STATE = string.IsNullOrEmpty(imageHandler.CONTACT_PHOTO_URL);
        imageHandler.HASH = result1.Item2;
        Avatar jsonAvatar = await ContactstoreInTouch.contactStoreInTouch.getManualContactFromWP(newContact, props);
        await LocalSettings.localSettings.saveContact(jsonAvatar, MCI_CID);
        if (jsonAvatar.name != null)
        {
          lookup.name_family = jsonAvatar.name.family;
          lookup.name_given = jsonAvatar.name.given;
          lookup.name_middle = jsonAvatar.name.middle;
          lookup.name_nickname = jsonAvatar.name.nickname;
          lookup.name_prefix = jsonAvatar.name.prefix;
          lookup.name_suffix = jsonAvatar.name.suffix;
        }
        if (jsonAvatar.organization != null && jsonAvatar.organization.Count != 0)
        {
          lookup.company_name = jsonAvatar.organization[0].company;
          lookup.job_title = jsonAvatar.organization[0].position;
        }
        InTouchAppDatabase.InTouchAppDB.addEntry(LocalSettings.localSettings.MCI, lookup, imageHandler);
        LogFile.Log("Added " + displayName + " to DB.", EventType.Debug);
      }
      catch (Exception ex)
      {
        LogFile.Log("Error while setting auto contact " + MCI_CID + " to WP. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public async Task setAllPhotoToWP()
    {
      List<string> IDList = new List<string>();
      List<string> urlList = new List<string>();
      List<string> MCI_CIDList = new List<string>();
      int i = 0;
      bool downloadState = false;
      try
      {
        InTouchAppDatabase.InTouchAppDB.getUpdatedUrlList(LocalSettings.localSettings.MCI, out MCI_CIDList, out IDList, out urlList);
        int count = urlList.Count;
        if (count == 0)
          return;
        bool isPhotoChanged = false;
        for (i = 0; i < count; ++i)
        {
          Sync.setImageStatusBlock("Downloading Photos : " + (object) (i + 1) + "/" + (object) count);
          string ID = IDList[i];
          string MCI_CID = MCI_CIDList[i];
          string URL = urlList[i];
          if (!string.IsNullOrEmpty(URL))
          {
            Stream stream = (Stream) null;
            if (this.SCMobj.mIsConnectedToNetwork)
            {
              stream = await this.SCMobj.downloadPhoto(URL, ID, await LocalSettings.localSettings.getToken());
              if (stream != null)
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
                  WPContact = await ContactstoreInTouch.contactStore.FindContactByIdAsync(ID);
                }
                catch (Exception ex)
                {
                  LogFile.Log("Error in reading contact from WP. " + ex.Message, EventType.Error);
                }
                if (WPContact != null)
                {
                  await WPContact.SetDisplayPictureAsync(stream.AsInputStream());
                  await WPContact.SaveAsync();
                  isPhotoChanged = true;
                  await ContactstoreInTouch.contactStoreInTouch.setRevisionNumber();
                  MemoryStream ms = new MemoryStream();
                  stream.CopyTo((Stream) ms);
                  IDictionary<string, object> props = await WPContact.GetPropertiesAsync();
                  Avatar jsonAvatar = await ContactstoreInTouch.contactStoreInTouch.getManualContactFromWP(WPContact, props);
                  await LocalSettings.localSettings.saveContact(jsonAvatar, MCI_CID);
                  LogFile.Log("Saved photo of contact " + MCI_CID + " to WP.", EventType.Debug);
                  string WPImageHash = ContactstoreInTouch.contactStoreInTouch.getWPImageHash(jsonAvatar.photo);
                  InTouchAppDatabase.InTouchAppDB.updateImageHash(LocalSettings.localSettings.MCI, MCI_CID, WPImageHash);
                }
                else
                  LogFile.Log("Can't save photo of contact " + MCI_CID + " to WP as contact does not exists.", EventType.Debug);
                downloadState = true;
                stream.Dispose();
              }
              else
                downloadState = false;
            }
            else
            {
              LogFile.Log("Internet connection is not available. Error in loading photo of contact with URL: \"" + URL + "\".", EventType.Warning);
              throw new NotSupportedException("Unable to sync." + Environment.NewLine + "Please check internet connectivity.");
            }
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
              WPContact = await ContactstoreInTouch.contactStore.FindContactByIdAsync(ID);
            }
            catch (Exception ex)
            {
              LogFile.Log("Error in reading contact from WP. " + ex.Message, EventType.Error);
            }
            await WPContact.SetDisplayPictureAsync((IInputStream) null);
            isPhotoChanged = true;
            await WPContact.SaveAsync();
            await ContactstoreInTouch.contactStoreInTouch.setRevisionNumber();
            IDictionary<string, object> props = await WPContact.GetPropertiesAsync();
            Avatar jsonAvatar = await ContactstoreInTouch.contactStoreInTouch.getManualContactFromWP(WPContact, props);
            await LocalSettings.localSettings.saveContact(jsonAvatar, MCI_CID);
            downloadState = true;
            InTouchAppDatabase.InTouchAppDB.updateImageHash(LocalSettings.localSettings.MCI, MCI_CID, string.Empty);
          }
          InTouchAppDatabase.InTouchAppDB.updateURLAndDownloadState(LocalSettings.localSettings.MCI, MCI_CID, URL, downloadState);
        }
        if (LocalSettings.localSettings.isBackground || !isPhotoChanged)
          return;
        Sync.setContactListView();
      }
      catch (Exception ex)
      {
        if (ex is NotSupportedException)
          LogFile.Log("Problem in saving photo of contact " + MCI_CIDList[i] + " to WP due to internet connectivity.", EventType.Warning);
        else
          LogFile.Log("Error while saving photo of contact " + MCI_CIDList[i] + " to WP. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public async Task setPhotoFieldsToWP(
      List<Avatar> avatar,
      StoredContact WPContact,
      string contact_id_1)
    {
      try
      {
        if (avatar == null)
          return;
        foreach (Avatar _avatar in avatar)
        {
          if (_avatar.photo != null)
          {
            foreach (Photo _photo in _avatar.photo)
            {
              if (!string.IsNullOrEmpty(_photo.data))
              {
                byte[] imageBytes = Convert.FromBase64String(_photo.data);
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                await WPContact.SetDisplayPictureAsync(ms.AsInputStream());
                ms.Dispose();
                LogFile.Log("Saved photo of contact " + WPContact.Id + " to WP.", EventType.Debug);
                break;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        string message = contact_id_1 + " : Error while setting photo fields to WP. " + ex.Message;
        LogFile.Log(message, EventType.Error);
        CommonCode.commonCode.reportBug("Sync", message);
      }
    }

    public void setManualContactToWP(
      ContactContactbookManual manualContact,
      StoredContact WPContact,
      IDictionary<string, object> props)
    {
      try
      {
        this.setNameFieldsToWP(manualContact.avatars, WPContact, props, manualContact.contact_id_1, manualContact.name);
        this.setEventFieldsToWP(manualContact.avatars, WPContact, props, manualContact.contact_id_1);
        this.setNotesFieldsToWP(manualContact.avatars, WPContact, props, manualContact.contact_id_1, manualContact.notes);
        this.setOrganizationFieldsToWP(manualContact.avatars, WPContact, props, manualContact.contact_id_1);
        this.setEmailAddressFieldsToWP(manualContact.avatars, WPContact, props, manualContact.contact_id_1);
        this.setAdressFieldsToWP(manualContact.avatars, WPContact, props, manualContact.contact_id_1);
        this.setPhoneNumberFieldsToWP(manualContact.avatars, WPContact, props, manualContact.contact_id_1);
        this.setWebsiteFieldsToWP(manualContact.avatars, WPContact, props, manualContact.contact_id_1);
      }
      catch (Exception ex)
      {
        LogFile.Log("Error while saving manual contact " + manualContact.contact_id_1 + " to WP. " + ex.Message, EventType.Error);
        throw;
      }
    }

    private void setAutoContactToWP(
      ContactContactbookAuto autoContact,
      StoredContact WPContact,
      IDictionary<string, object> props,
      string MCI_CID)
    {
      try
      {
        Note note = new Note();
        note.text = autoContact.notes;
        List<Note> contact_notes = new List<Note>();
        contact_notes.Add(note);
        this.setNameFieldsToWP(autoContact.avatars, WPContact, props, MCI_CID, (Name) null);
        this.setEventFieldsToWP(autoContact.avatars, WPContact, props, MCI_CID);
        this.setNotesFieldsToWP(autoContact.avatars, WPContact, props, MCI_CID, contact_notes);
        this.setOrganizationFieldsToWP(autoContact.avatars, WPContact, props, MCI_CID);
        this.setEmailAddressFieldsToWP(autoContact.avatars, WPContact, props, MCI_CID);
        this.setAdressFieldsToWP(autoContact.avatars, WPContact, props, MCI_CID);
        this.setPhoneNumberFieldsToWP(autoContact.avatars, WPContact, props, MCI_CID);
        this.setWebsiteFieldsToWP(autoContact.avatars, WPContact, props, MCI_CID);
      }
      catch (Exception ex)
      {
        LogFile.Log("Error while saving auto contact " + MCI_CID + " to WP. " + ex.Message, EventType.Error);
        throw;
      }
    }

    private void setNameFieldsToWP(
      List<Avatar> avatar,
      StoredContact WPContact,
      IDictionary<string, object> props,
      string contact_id_1,
      Name contact_name)
    {
      try
      {
        bool flag = false;
        if (avatar != null)
        {
          foreach (Avatar avatar1 in avatar)
          {
            if (avatar1.name != null)
            {
              if (!string.IsNullOrEmpty(avatar1.name.family))
                WPContact.put_FamilyName(avatar1.name.family);
              else
                WPContact.put_FamilyName(string.Empty);
              if (!string.IsNullOrEmpty(avatar1.name.given))
                WPContact.put_GivenName(avatar1.name.given);
              else
                WPContact.put_GivenName(string.Empty);
              if (!string.IsNullOrEmpty(avatar1.name.prefix))
                WPContact.put_HonorificPrefix(avatar1.name.prefix);
              else
                WPContact.put_HonorificPrefix(string.Empty);
              if (!string.IsNullOrEmpty(avatar1.name.suffix))
                WPContact.put_HonorificSuffix(avatar1.name.suffix);
              else
                WPContact.put_HonorificSuffix(string.Empty);
              props[KnownContactProperties.Nickname] = string.IsNullOrEmpty(avatar1.name.nickname) ? (object) string.Empty : (object) avatar1.name.nickname;
              props[KnownContactProperties.AdditionalName] = string.IsNullOrEmpty(avatar1.name.middle) ? (object) string.Empty : (object) avatar1.name.middle;
              flag = true;
              break;
            }
          }
        }
        if (flag)
          return;
        if (contact_name != null)
        {
          if (!string.IsNullOrEmpty(contact_name.family))
            WPContact.put_FamilyName(contact_name.family);
          else
            WPContact.put_FamilyName(string.Empty);
          if (!string.IsNullOrEmpty(contact_name.given))
            WPContact.put_GivenName(contact_name.given);
          else
            WPContact.put_GivenName(string.Empty);
          if (!string.IsNullOrEmpty(contact_name.prefix))
            WPContact.put_HonorificPrefix(contact_name.prefix);
          else
            WPContact.put_HonorificPrefix(string.Empty);
          if (!string.IsNullOrEmpty(contact_name.suffix))
            WPContact.put_HonorificSuffix(contact_name.suffix);
          else
            WPContact.put_HonorificSuffix(string.Empty);
          props[KnownContactProperties.Nickname] = string.IsNullOrEmpty(contact_name.nickname) ? (object) string.Empty : (object) contact_name.nickname;
          if (!string.IsNullOrEmpty(contact_name.middle))
            props[KnownContactProperties.AdditionalName] = (object) contact_name.middle;
          else
            props[KnownContactProperties.AdditionalName] = (object) string.Empty;
        }
        else
        {
          WPContact.put_FamilyName(string.Empty);
          WPContact.put_GivenName(string.Empty);
          WPContact.put_HonorificPrefix(string.Empty);
          WPContact.put_HonorificSuffix(string.Empty);
          props[KnownContactProperties.Nickname] = (object) string.Empty;
          props[KnownContactProperties.AdditionalName] = (object) string.Empty;
        }
      }
      catch (Exception ex)
      {
        string message = contact_id_1 + " : Error while setting name fields to WP. " + ex.Message;
        LogFile.Log(message, EventType.Error);
        CommonCode.commonCode.reportBug("Sync", message);
      }
    }

    private void setEventFieldsToWP(
      List<Avatar> avatar,
      StoredContact WPContact,
      IDictionary<string, object> props,
      string contact_id_1)
    {
      try
      {
        DateTime result = new DateTime();
        bool flag1 = false;
        bool flag2 = false;
        if (avatar != null)
        {
          foreach (Avatar avatar1 in avatar)
          {
            if (avatar1.@event != null)
            {
              foreach (Event @event in avatar1.@event)
              {
                if (!string.IsNullOrEmpty(@event.type) && !string.IsNullOrEmpty(@event.date) && @event.type != EventLabel.other.ToString() && DateTime.TryParse(@event.date, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                {
                  if (@event.type.Equals(EventLabel.anniv.ToString(), StringComparison.OrdinalIgnoreCase) && !flag1)
                  {
                    props[KnownContactProperties.Anniversary] = (object) new DateTimeOffset(DateTime.Parse(result.Day.ToString() + "-" + result.Month.ToString() + "-" + result.Year.ToString()));
                    flag1 = true;
                  }
                  else if (@event.type.Equals(EventLabel.bday.ToString(), StringComparison.OrdinalIgnoreCase) && !flag2)
                  {
                    props[KnownContactProperties.Birthdate] = (object) new DateTimeOffset(DateTime.Parse(result.Day.ToString() + "-" + result.Month.ToString() + "-" + result.Year.ToString()));
                    flag2 = true;
                  }
                }
              }
              if (flag1)
              {
                if (flag2)
                  break;
              }
            }
          }
        }
        if (!flag1)
          props[KnownContactProperties.Anniversary] = (object) null;
        if (flag2)
          return;
        props[KnownContactProperties.Birthdate] = (object) null;
      }
      catch (Exception ex)
      {
        string message = contact_id_1 + " : Error while setting event fields to WP. " + ex.Message;
        LogFile.Log(message, EventType.Error);
        CommonCode.commonCode.reportBug("Sync", message);
      }
    }

    public void setNotesFieldsToWP(
      List<Avatar> avatar,
      StoredContact WPContact,
      IDictionary<string, object> props,
      string contact_id_1,
      List<Note> contact_notes)
    {
      try
      {
        bool flag = false;
        if (avatar != null)
        {
          foreach (Avatar avatar1 in avatar)
          {
            if (avatar1.notes != null)
            {
              foreach (Note note in avatar1.notes)
              {
                if (!string.IsNullOrEmpty(note.text))
                {
                  props[KnownContactProperties.Notes] = (object) note.text;
                  flag = true;
                  break;
                }
              }
            }
            if (flag)
              break;
          }
        }
        if (!flag && contact_notes != null)
        {
          foreach (Note contactNote in contact_notes)
          {
            if (!string.IsNullOrEmpty(contactNote.text))
            {
              props[KnownContactProperties.Notes] = (object) contactNote.text;
              flag = true;
              break;
            }
          }
        }
        if (flag)
          return;
        props[KnownContactProperties.Notes] = (object) string.Empty;
      }
      catch (Exception ex)
      {
        string message = contact_id_1 + " : Error while setting notes fields to WP. " + ex.Message;
        LogFile.Log(message, EventType.Error);
        CommonCode.commonCode.reportBug("Sync", message);
      }
    }

    public void setOrganizationFieldsToWP(
      List<Avatar> avatar,
      StoredContact WPContact,
      IDictionary<string, object> props,
      string contact_id_1)
    {
      try
      {
        bool flag = false;
        if (avatar != null)
        {
          foreach (Avatar avatar1 in avatar)
          {
            if (avatar1.organization != null)
            {
              foreach (Organization organization in avatar1.organization)
              {
                if (organization != null && !flag)
                {
                  props[KnownContactProperties.JobTitle] = (object) organization.position;
                  props[KnownContactProperties.CompanyName] = (object) organization.company;
                  flag = true;
                  break;
                }
              }
            }
          }
        }
        if (flag)
          return;
        props[KnownContactProperties.JobTitle] = (object) string.Empty;
        props[KnownContactProperties.CompanyName] = (object) string.Empty;
      }
      catch (Exception ex)
      {
        string message = contact_id_1 + " : Error while setting organization fields to WP. " + ex.Message;
        LogFile.Log(message, EventType.Error);
        CommonCode.commonCode.reportBug("Sync", message);
      }
    }

    public void setEmailAddressFieldsToWP(
      List<Avatar> avatar,
      StoredContact WPContact,
      IDictionary<string, object> props,
      string contact_id_1)
    {
      try
      {
        List<int> intList = new List<int>() { 0, 1, 2 };
        List<Email> emailList = new List<Email>();
        if (avatar != null)
        {
          foreach (Avatar avatar1 in avatar)
          {
            if (avatar1.email != null)
            {
              foreach (Email email in avatar1.email)
              {
                int index;
                if (!string.IsNullOrEmpty(email.label))
                {
                  if (email.label.Equals("personal", StringComparison.OrdinalIgnoreCase) || email.label.Equals("home", StringComparison.OrdinalIgnoreCase) && intList.Contains(0))
                    index = 0;
                  else if (email.label.Equals("work", StringComparison.OrdinalIgnoreCase) || email.label.Equals("business", StringComparison.OrdinalIgnoreCase) && intList.Contains(1))
                    index = 1;
                  else if (email.label.Equals("other", StringComparison.OrdinalIgnoreCase) && intList.Contains(2))
                  {
                    index = 2;
                  }
                  else
                  {
                    index = 3;
                    emailList.Add(email);
                  }
                }
                else
                {
                  index = 3;
                  emailList.Add(email);
                }
                if (index == 0 || index == 1 || index == 2)
                {
                  intList.Remove(index);
                  this.decideEmailAddressField(index, WPContact, props, email.address);
                }
              }
            }
          }
        }
        int num = 0;
        while (emailList.Count > 0 && intList.Count > 0)
        {
          int index = intList[0];
          intList.RemoveAt(0);
          this.decideEmailAddressField(index, WPContact, props, emailList[0].address);
          emailList.RemoveAt(0);
          if (intList.Count != 0)
            ++num;
          else
            break;
        }
        for (int index = 0; index < intList.Count; ++index)
          this.deleteEmailAddressField(intList[index], WPContact, props);
      }
      catch (Exception ex)
      {
        string message = contact_id_1 + " : Error while setting address fields to WP. " + ex.Message;
        LogFile.Log(message, EventType.Error);
        CommonCode.commonCode.reportBug("Sync", message);
      }
    }

    private void decideEmailAddressField(
      int index,
      StoredContact WPContact,
      IDictionary<string, object> props,
      string emailAddress)
    {
      try
      {
        switch (index)
        {
          case 0:
            props[KnownContactProperties.Email] = (object) emailAddress;
            break;
          case 1:
            props[KnownContactProperties.WorkEmail] = (object) emailAddress;
            break;
          case 2:
            props[KnownContactProperties.OtherEmail] = (object) emailAddress;
            break;
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error while deciding address. " + ex.Message, EventType.Error);
        throw;
      }
    }

    private void deleteEmailAddressField(
      int index,
      StoredContact WPContact,
      IDictionary<string, object> props)
    {
      try
      {
        switch (index)
        {
          case 0:
            props[KnownContactProperties.Email] = (object) string.Empty;
            break;
          case 1:
            props[KnownContactProperties.WorkEmail] = (object) string.Empty;
            break;
          case 2:
            props[KnownContactProperties.OtherEmail] = (object) string.Empty;
            break;
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error while deleting address. " + ex.Message, EventType.Error);
        throw;
      }
    }

    private void setAdressFieldsToWP(
      List<Avatar> avatar,
      StoredContact WPContact,
      IDictionary<string, object> props,
      string contact_id_1)
    {
      try
      {
        List<int> intList = new List<int>() { 0, 1, 2 };
        List<Address> addressList = new List<Address>();
        if (avatar != null)
        {
          foreach (Avatar avatar1 in avatar)
          {
            if (avatar1.address != null)
            {
              foreach (Address address in avatar1.address)
              {
                int index;
                if (!string.IsNullOrEmpty(address.label))
                {
                  if (address.label.Equals("personal", StringComparison.OrdinalIgnoreCase) || address.label.Equals("home", StringComparison.OrdinalIgnoreCase) && intList.Contains(0))
                    index = 0;
                  else if (address.label.Equals("work", StringComparison.OrdinalIgnoreCase) || address.label.Equals("business", StringComparison.OrdinalIgnoreCase) && intList.Contains(1))
                    index = 1;
                  else if (address.label.Equals("other", StringComparison.OrdinalIgnoreCase) && intList.Contains(1))
                  {
                    index = 2;
                  }
                  else
                  {
                    index = 3;
                    addressList.Add(address);
                  }
                }
                else
                {
                  index = 3;
                  addressList.Add(address);
                }
                if (index == 0 || index == 1 || index == 2)
                {
                  intList.Remove(index);
                  this.decideAddressField(index, WPContact, props, address);
                }
              }
            }
          }
        }
        int num = 0;
        while (addressList.Count > 0 && intList.Count > 0)
        {
          int index = intList[0];
          intList.RemoveAt(0);
          this.decideAddressField(index, WPContact, props, addressList[0]);
          addressList.RemoveAt(0);
          if (intList.Count != 0)
            ++num;
          else
            break;
        }
        for (int index = 0; index < intList.Count; ++index)
          this.deleteAddressField(intList[index], WPContact, props);
      }
      catch (Exception ex)
      {
        string message = contact_id_1 + " : Error while setting address fields to WP. " + ex.Message;
        LogFile.Log(message, EventType.Error);
        CommonCode.commonCode.reportBug("Sync", message);
      }
    }

    private void decideAddressField(
      int index,
      StoredContact WPContact,
      IDictionary<string, object> props,
      Address address)
    {
      try
      {
        ContactAddress contactAddress = new ContactAddress();
        if (!string.IsNullOrEmpty(address.street1) || !string.IsNullOrEmpty(address.street2) || !string.IsNullOrEmpty(address.street3))
        {
          string str = address.street1;
          if (string.IsNullOrEmpty(str))
            str = address.street2;
          else if (!string.IsNullOrEmpty(address.street2))
            str = str + "\n" + address.street2;
          if (string.IsNullOrEmpty(str))
            str = address.street3;
          else if (!string.IsNullOrEmpty(address.street3))
            str = str + "\n" + address.street3;
          contactAddress.put_StreetAddress(str);
        }
        else
          contactAddress.put_StreetAddress(string.Empty);
        if (!string.IsNullOrEmpty(address.city))
          contactAddress.put_Locality(address.city);
        else
          contactAddress.put_Locality(string.Empty);
        if (!string.IsNullOrEmpty(address.state))
          contactAddress.put_Region(address.state);
        else
          contactAddress.put_Region(string.Empty);
        if (!string.IsNullOrEmpty(address.country))
          contactAddress.put_Country(address.country);
        else
          contactAddress.put_Country(string.Empty);
        if (!string.IsNullOrEmpty(address.zip))
          contactAddress.put_PostalCode(address.zip);
        else
          contactAddress.put_PostalCode(string.Empty);
        switch (index)
        {
          case 0:
            props[KnownContactProperties.Address] = (object) contactAddress;
            break;
          case 1:
            props[KnownContactProperties.WorkAddress] = (object) contactAddress;
            break;
          case 2:
            props[KnownContactProperties.OtherAddress] = (object) contactAddress;
            break;
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error while deciding address. " + ex.Message, EventType.Error);
        throw;
      }
    }

    private void deleteAddressField(
      int index,
      StoredContact WPContact,
      IDictionary<string, object> props)
    {
      try
      {
        ContactAddress contactAddress = new ContactAddress();
        contactAddress.put_StreetAddress(string.Empty);
        contactAddress.put_Locality(string.Empty);
        contactAddress.put_Region(string.Empty);
        contactAddress.put_Country(string.Empty);
        contactAddress.put_PostalCode(string.Empty);
        switch (index)
        {
          case 0:
            props[KnownContactProperties.Address] = (object) contactAddress;
            break;
          case 1:
            props[KnownContactProperties.WorkAddress] = (object) contactAddress;
            break;
          case 2:
            props[KnownContactProperties.OtherAddress] = (object) contactAddress;
            break;
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error while deleting address. " + ex.Message, EventType.Error);
        throw;
      }
    }

    private void setWebsiteFieldsToWP(
      List<Avatar> avatar,
      StoredContact WPContact,
      IDictionary<string, object> props,
      string contact_id_1)
    {
      try
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.ToString();
        bool flag = false;
        if (avatar != null)
        {
          foreach (Avatar avatar1 in avatar)
          {
            if (avatar1.website != null)
            {
              foreach (Website website in avatar1.website)
              {
                if (!string.IsNullOrEmpty(website.url))
                {
                  stringBuilder.Append(website.url);
                  stringBuilder.Append(';');
                  flag = true;
                }
              }
            }
          }
        }
        if (!flag)
          props[KnownContactProperties.Url] = (object) string.Empty;
        else
          props[KnownContactProperties.Url] = (object) stringBuilder.ToString();
      }
      catch (Exception ex)
      {
        string message = contact_id_1 + " : Error while setting website fields to WP. " + ex.Message;
        LogFile.Log(message, EventType.Error);
        CommonCode.commonCode.reportBug("Sync", message);
      }
    }

    private void setPhoneNumberFieldsToWP(
      List<Avatar> avatar,
      StoredContact WPContact,
      IDictionary<string, object> props,
      string contact_id_1)
    {
      try
      {
        List<string> stringList = new List<string>()
        {
          "mobile",
          "mobile 2",
          "home",
          "home 2",
          "work",
          "work 2",
          "company",
          "home fax",
          "work fax"
        };
        List<int> intList = new List<int>()
        {
          0,
          1,
          2,
          3,
          4,
          5,
          6,
          7,
          8
        };
        List<InTouchLibrary.Phone> phoneList = new List<InTouchLibrary.Phone>();
        int index1 = 0;
        string empty = string.Empty;
        if (avatar != null)
        {
          foreach (Avatar avatar1 in avatar)
          {
            if (avatar1.phone != null)
            {
              foreach (InTouchLibrary.Phone phone in avatar1.phone)
              {
                if (phone != null)
                {
                  bool flag = false;
                  string str1 = string.Empty;
                  string str2 = string.Empty;
                  string str3 = string.Empty;
                  if (!string.IsNullOrEmpty(phone.type))
                    str2 = phone.type.ToLower();
                  if (!string.IsNullOrEmpty(phone.label))
                    str3 = phone.label.ToLower();
                  if (!string.IsNullOrEmpty(str2) && stringList.Contains(str2))
                  {
                    index1 = stringList.IndexOf(str2);
                    flag = true;
                  }
                  if (!flag && !string.IsNullOrEmpty(str3) && stringList.Contains(str3))
                  {
                    index1 = stringList.IndexOf(str3);
                    flag = true;
                  }
                  if (!flag)
                    index1 = 9;
                  if (index1 != 9)
                  {
                    intList.Remove(index1);
                    stringList[index1] = string.Empty;
                    if (!string.IsNullOrEmpty(phone.country_code))
                      str1 = !phone.country_iso.StartsWith("+") ? "+" + phone.country_code : phone.country_code;
                    string number = str1 + phone.area_code + phone.number;
                    this.decidePhoneField(index1, WPContact, props, number);
                  }
                  else
                    phoneList.Add(phone);
                }
              }
            }
          }
        }
        int num = 0;
        while (phoneList.Count > 0 && intList.Count > 0)
        {
          int index2 = intList[0];
          intList.RemoveAt(0);
          string number = phoneList[0].country_code + phoneList[0].area_code + phoneList[0].number;
          this.decidePhoneField(index2, WPContact, props, number);
          phoneList.RemoveAt(0);
          if (intList.Count != 0)
            ++num;
          else
            break;
        }
        for (int index3 = 0; index3 < intList.Count; ++index3)
          this.deletePhoneField(intList[index3], WPContact, props);
      }
      catch (Exception ex)
      {
        string message = contact_id_1 + " : Error while setting phone number fields to WP. " + ex.Message;
        LogFile.Log(message, EventType.Error);
        CommonCode.commonCode.reportBug("Sync", message);
      }
    }

    private void decidePhoneField(
      int index,
      StoredContact WPContact,
      IDictionary<string, object> props,
      string number)
    {
      try
      {
        switch (index)
        {
          case 0:
            props[KnownContactProperties.MobileTelephone] = (object) number;
            break;
          case 1:
            props[KnownContactProperties.AlternateMobileTelephone] = (object) number;
            break;
          case 2:
            props[KnownContactProperties.Telephone] = (object) number;
            break;
          case 3:
            props[KnownContactProperties.AlternateTelephone] = (object) number;
            break;
          case 4:
            props[KnownContactProperties.WorkTelephone] = (object) number;
            break;
          case 5:
            props[KnownContactProperties.AlternateWorkTelephone] = (object) number;
            break;
          case 6:
            props[KnownContactProperties.CompanyTelephone] = (object) number;
            break;
          case 7:
            props[KnownContactProperties.HomeFax] = (object) number;
            break;
          case 8:
            props[KnownContactProperties.WorkFax] = (object) number;
            break;
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error while deciding phone. " + ex.Message, EventType.Error);
        throw;
      }
    }

    private void deletePhoneField(
      int index,
      StoredContact WPContact,
      IDictionary<string, object> props)
    {
      try
      {
        switch (index)
        {
          case 0:
            props[KnownContactProperties.MobileTelephone] = (object) string.Empty;
            break;
          case 1:
            props[KnownContactProperties.AlternateMobileTelephone] = (object) string.Empty;
            break;
          case 2:
            props[KnownContactProperties.Telephone] = (object) string.Empty;
            break;
          case 3:
            props[KnownContactProperties.AlternateTelephone] = (object) string.Empty;
            break;
          case 4:
            props[KnownContactProperties.WorkTelephone] = (object) string.Empty;
            break;
          case 5:
            props[KnownContactProperties.AlternateWorkTelephone] = (object) string.Empty;
            break;
          case 6:
            props[KnownContactProperties.CompanyTelephone] = (object) string.Empty;
            break;
          case 7:
            props[KnownContactProperties.HomeFax] = (object) string.Empty;
            break;
          case 8:
            props[KnownContactProperties.WorkFax] = (object) string.Empty;
            break;
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error while deleting phone. " + ex.Message, EventType.Error);
        throw;
      }
    }

    private async Task<Tuple<string, string>> getPhotoURLData(
      List<Avatar> avatar,
      string contact_id_1,
      StoredContact WPContact,
      bool shallUpdateDBImageHash)
    {
      Tuple<string, string> photoUrlData;
      try
      {
        string WPImageHash = string.Empty;
        string contactPhotoUrl = string.Empty;
        bool gotValue = false;
        if (avatar != null)
        {
          foreach (Avatar _avatar in avatar)
          {
            if (_avatar.photo != null)
            {
              foreach (Photo _photo in _avatar.photo)
              {
                if (!string.IsNullOrEmpty(_photo.data))
                {
                  byte[] imageBytes = Convert.FromBase64String(_photo.data);
                  MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                  await WPContact.SetDisplayPictureAsync(ms.AsInputStream());
                  await WPContact.SaveAsync();
                  ms.Dispose();
                  LogFile.Log("Saved photo of contact " + WPContact.Id + " to WP.", EventType.Debug);
                  WPImageHash = CommonCode.commonCode.getHash(_photo.data);
                  if (shallUpdateDBImageHash)
                    InTouchAppDatabase.InTouchAppDB.updateImageHash(LocalSettings.localSettings.MCI, contact_id_1, WPImageHash);
                  gotValue = true;
                  break;
                }
                if (!string.IsNullOrEmpty(_photo.url) && _photo.url.Length > 6)
                {
                  contactPhotoUrl = _photo.url;
                  gotValue = true;
                  break;
                }
              }
            }
            if (gotValue)
              break;
          }
        }
        photoUrlData = Tuple.Create<string, string>(contactPhotoUrl, WPImageHash);
      }
      catch (Exception ex)
      {
        LogFile.Log("Error while retrieving PHOTO_URL of " + contact_id_1 + ". " + ex.Message, EventType.Error);
        throw;
      }
      return photoUrlData;
    }

    private async Task updateImageHandlerTable(
      List<Avatar> avatar,
      string contact_id_1,
      StoredContact WPContact)
    {
      try
      {
        string ImageHandlerTableURL = InTouchAppDatabase.InTouchAppDB.readURL(LocalSettings.localSettings.MCI, contact_id_1);
        Tuple<string, string> result = await this.getPhotoURLData(avatar, contact_id_1, WPContact, true);
        string manualContactURL = result.Item1;
        if (string.Equals(ImageHandlerTableURL, manualContactURL))
          return;
        InTouchAppDatabase.InTouchAppDB.updateURLAndDownloadState(LocalSettings.localSettings.MCI, contact_id_1, manualContactURL, string.IsNullOrEmpty(manualContactURL));
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in updading IMT. " + ex.Message, EventType.Error);
        throw;
      }
    }
  }
}
