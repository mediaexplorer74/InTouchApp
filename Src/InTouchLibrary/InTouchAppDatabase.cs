// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.InTouchAppDatabase
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

#nullable disable
namespace InTouchLibrary
{
  public class InTouchAppDatabase
  {
    private string applicationFolderPath = ApplicationData.Current.LocalFolder.Path;
    private static volatile InTouchAppDatabase _InTouchAppDB;

    public static InTouchAppDatabase InTouchAppDB
    {
      get
      {
        try
        {
          if (InTouchAppDatabase._InTouchAppDB == null)
            InTouchAppDatabase._InTouchAppDB = new InTouchAppDatabase();
          return InTouchAppDatabase._InTouchAppDB;
        }
        catch
        {
          throw;
        }
      }
    }

    public async Task createDBAsync(string DBName)
    {
      try
      {
        bool dbExist = true;
        try
        {
          StorageFile fileAsync = await ApplicationData.Current.LocalFolder.GetFileAsync(DBName);
          LogFile.Log("DB already exists.", EventType.Information);
        }
        catch (Exception ex)
        {
          dbExist = false;
        }
        if (dbExist)
          return;
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          sqLiteConnection.CreateTable<Lookup>();
          LogFile.Log("Created LT.", EventType.Information);
          sqLiteConnection.CreateTable<ImageHandler>();
          LogFile.Log("Created IMT.", EventType.Information);
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in creating DB async. " + ex.Message + Environment.NewLine + ex.GetType().ToString(), EventType.Error);
        throw;
      }
    }

    public async Task deleteDB(string DBName)
    {
      try
      {
        StorageFile sf = await ApplicationData.Current.LocalFolder.GetFileAsync(DBName);
        await sf.DeleteAsync();
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in deleting DB. " + ex.Message, EventType.Error);
      }
    }

    public bool checkforColumnAlterTable(string DBName)
    {
      try
      {
        string str = "name_given";
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          if (string.Equals(sqLiteConnection.GetMapping<Lookup>().FindColumn(str).Name, str))
            return true;
          try
          {
            sqLiteConnection.CreateTable<Lookup>();
            LogFile.Log("Created LT.", EventType.Information);
            sqLiteConnection.CreateTable<ImageHandler>();
            LogFile.Log("Created IMT.", EventType.Information);
            LocalSettings.localSettings.updateDBWithValues = true;
          }
          catch (Exception ex)
          {
            LogFile.Log("Error in altering DB async. " + ex.Message + Environment.NewLine + ex.GetType().ToString(), EventType.Error);
            throw;
          }
          return false;
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in checking column existence in DB. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void addEntry(string DBName, Lookup newLookup, ImageHandler newImageHandler)
    {
      try
      {
        using (SQLiteConnection Conn = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
          Conn.RunInTransaction((Action) (() =>
          {
            Conn.Insert((object) newLookup);
            Conn.Insert((object) newImageHandler);
          }));
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in adding entry to DB. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void deleteEntry(string DBName, string MCI_CID)
    {
      try
      {
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          sqLiteConnection.Delete<Lookup>((object) MCI_CID);
          sqLiteConnection.Delete<ImageHandler>((object) MCI_CID);
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in deleting entry from DB. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public string readID(string DBName, string MCI_CID)
    {
      try
      {
        string str = string.Empty;
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          Lookup lookup = sqLiteConnection.Query<Lookup>("select ID from Lookup where MCI_CID = ?", (object) MCI_CID).FirstOrDefault<Lookup>();
          if (lookup != null)
            str = lookup.ID;
        }
        return str;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in reading ID from LT. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public string readInTouchMCI_CID(
      string DBName,
      string ID,
      out string imageHash,
      out bool isImageAlreadyModified)
    {
      try
      {
        string str = string.Empty;
        imageHash = string.Empty;
        isImageAlreadyModified = false;
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          Lookup lookup = sqLiteConnection.Query<Lookup>("select MCI_CID, Dirty from Lookup where ID = ?", (object) ID).FirstOrDefault<Lookup>();
          if (lookup != null)
          {
            str = lookup.MCI_CID;
            if (lookup.Dirty == 2)
              isImageAlreadyModified = true;
            ImageHandler imageHandler = sqLiteConnection.Query<ImageHandler>("select HASH from ImageHandler where MCI_CID = ?", (object) str).FirstOrDefault<ImageHandler>();
            if (imageHandler != null)
              imageHash = imageHandler.HASH;
          }
        }
        return str;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in reading InTouch MCI_CID and image HASH. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void readOtherMCI_CID(
      string DBName,
      List<string> WPOtherContactIDList,
      out List<string> MCI_CIDList,
      out List<string> ContactHashList,
      out List<string> inImageHashList)
    {
      try
      {
        MCI_CIDList = new List<string>();
        ContactHashList = new List<string>();
        inImageHashList = new List<string>();
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          for (int index = 0; index < WPOtherContactIDList.Count; ++index)
          {
            Lookup lookup = sqLiteConnection.Query<Lookup>("select MCI_CID, HASH from Lookup where ID = ?", (object) WPOtherContactIDList[index]).FirstOrDefault<Lookup>();
            string empty = string.Empty;
            if (lookup != null)
            {
              string mciCid = lookup.MCI_CID;
              MCI_CIDList.Add(mciCid);
              ContactHashList.Add(lookup.HASH);
              ImageHandler imageHandler = sqLiteConnection.Query<ImageHandler>("select HASH from ImageHandler where MCI_CID = ?", (object) mciCid).FirstOrDefault<ImageHandler>();
              if (imageHandler != null)
                inImageHashList.Add(imageHandler.HASH);
              else
                inImageHashList.Add(string.Empty);
            }
            else
            {
              MCI_CIDList.Add(string.Empty);
              ContactHashList.Add(string.Empty);
              inImageHashList.Add(string.Empty);
            }
          }
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in reading other MCI_CID from LT. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public string readMCI_CID(string DBName, string ID)
    {
      try
      {
        string str = string.Empty;
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          Lookup lookup = sqLiteConnection.Query<Lookup>("select MCI_CID from Lookup where ID = ?", (object) ID).FirstOrDefault<Lookup>();
          if (lookup != null)
            str = lookup.MCI_CID;
        }
        return str;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in reading MCI_CID. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void updateBaseVersionAndContactsInfo(
      string DBName,
      string MCI_CID,
      string baseVersion,
      Contacts contact)
    {
      try
      {
        using (SQLiteConnection Conn = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          Lookup existingL = Conn.Find<Lookup>((object) MCI_CID);
          if (existingL == null)
            return;
          if (!string.IsNullOrEmpty(baseVersion))
            existingL.BASE_VERSION = baseVersion;
          existingL.name_family = contact.modifiedContactNameFamily;
          existingL.name_given = contact.modifiedContactNameGiven;
          existingL.name_middle = contact.modifiedContactNameMiddle;
          existingL.name_nickname = contact.modifiedContactNameNickname;
          existingL.name_prefix = contact.modifiedContactNamePrefix;
          existingL.name_suffix = contact.modifiedContactNameSuffix;
          existingL.company_name = contact.modifiedContactCompanyName;
          existingL.job_title = contact.modifiedContactJobTitle;
          Conn.RunInTransaction((Action) (() => Conn.Update((object) existingL)));
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in updating baseversion and contacts info in LT. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void getUpdatedUrlList(
      string DBName,
      out List<string> MCI_CIDList,
      out List<string> IDList,
      out List<string> urlList)
    {
      IDList = new List<string>();
      urlList = new List<string>();
      MCI_CIDList = new List<string>();
      try
      {
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          List<ImageHandler> imageHandlerList = sqLiteConnection.Query<ImageHandler>("select ID, CONTACT_PHOTO_URL, MCI_CID from ImageHandler where DOWNLOAD_STATE = ?", (object) false);
          if (imageHandlerList != null)
          {
            foreach (ImageHandler imageHandler in imageHandlerList)
            {
              IDList.Add(imageHandler.ID);
              urlList.Add(imageHandler.CONTACT_PHOTO_URL);
              MCI_CIDList.Add(imageHandler.MCI_CID);
            }
          }
        }
        LogFile.Log("Total images to be downloaded : " + (object) urlList.Count + ".", EventType.Information);
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting updated URL list from IHT. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public string readURL(string DBName, string MCI_CID)
    {
      try
      {
        string str = string.Empty;
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          ImageHandler imageHandler = sqLiteConnection.Query<ImageHandler>("select CONTACT_PHOTO_URL from ImageHandler where MCI_CID = ?", (object) MCI_CID).FirstOrDefault<ImageHandler>();
          if (imageHandler != null)
            str = imageHandler.CONTACT_PHOTO_URL;
        }
        return str;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in reading URL from IHT. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void updateURLAndDownloadState(
      string DBName,
      string MCI_CID,
      string contactPhotoUrl,
      bool downloadState)
    {
      try
      {
        using (SQLiteConnection Conn = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          ImageHandler existingI = Conn.Find<ImageHandler>((object) MCI_CID);
          if (existingI == null)
            return;
          existingI.CONTACT_PHOTO_URL = contactPhotoUrl;
          existingI.DOWNLOAD_STATE = downloadState;
          Conn.RunInTransaction((Action) (() => Conn.Update((object) existingI)));
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in updating URL and Download state in IHT. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public int countEntriesFromLT(string DBName)
    {
      try
      {
        int num = 0;
        using (SQLiteConnection conn = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
          num = new TableQuery<Lookup>(conn).Count();
        return num;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in counting entries from LT. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public int countInTouchEntriesFromLT(string DBName)
    {
      try
      {
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
          return sqLiteConnection.Query<Lookup>("select MCI_CID from Lookup where CONTACT_TYPE != ?", (object) 2).Count;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in counting InTouch entries from LT. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void addMultipleDirtyEntries(
      string DBName,
      List<DBUpdate> DBUpdateAddedContactList,
      int contactType)
    {
      try
      {
        List<Lookup> lookupList = new List<Lookup>();
        List<ImageHandler> imageHandlerList = new List<ImageHandler>();
        for (int index = 0; index < DBUpdateAddedContactList.Count; ++index)
        {
          Lookup lookup = new Lookup();
          ImageHandler imageHandler = new ImageHandler();
          string modifiedContactId = DBUpdateAddedContactList[index].modifiedContactID;
          string modifiedContactMciCid = DBUpdateAddedContactList[index].modifiedContactMCI_CID;
          string contactImageHash = DBUpdateAddedContactList[index].modifiedContactImageHash;
          lookup.MCI_CID = modifiedContactMciCid;
          lookup.ID = modifiedContactId;
          lookup.BASE_VERSION = "-1";
          lookup.HASH = DBUpdateAddedContactList[index].modifiedContactContactHash;
          lookup.CONTACT_TYPE = contactType;
          Contacts contacts = new Contacts();
          Contacts contact = DBUpdateAddedContactList[index].contact;
          if (contact != null)
          {
            lookup.name_family = contact.modifiedContactNameFamily;
            lookup.name_given = contact.modifiedContactNameGiven;
            lookup.name_middle = contact.modifiedContactNameMiddle;
            lookup.name_nickname = contact.modifiedContactNameNickname;
            lookup.name_prefix = contact.modifiedContactNamePrefix;
            lookup.name_suffix = contact.modifiedContactNameSuffix;
            lookup.company_name = contact.modifiedContactCompanyName;
            lookup.job_title = contact.modifiedContactJobTitle;
          }
          lookup.Dirty = !string.IsNullOrEmpty(contactImageHash) ? 2 : 1;
          lookupList.Add(lookup);
          imageHandler.MCI_CID = modifiedContactMciCid;
          imageHandler.ID = modifiedContactId;
          imageHandler.CONTACT_PHOTO_URL = string.Empty;
          imageHandler.HASH = contactImageHash;
          imageHandler.DOWNLOAD_STATE = true;
          imageHandlerList.Add(imageHandler);
        }
        using (SQLiteConnection Conn = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
          Conn.RunInTransaction((Action) (() =>
          {
            Conn.InsertAll((IEnumerable) lookupList);
            Conn.InsertAll((IEnumerable) imageHandlerList);
          }));
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in adding entries to DB. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void markModifiedDirtyEntries(string DBName, List<DBUpdate> DBUpdateModifiedContactList)
    {
      try
      {
        List<Lookup> lookupList = new List<Lookup>();
        List<ImageHandler> imageHandlerList = new List<ImageHandler>();
        using (SQLiteConnection Conn = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          for (int index = 0; index < DBUpdateModifiedContactList.Count; ++index)
          {
            Lookup lookup = Conn.Find<Lookup>((object) DBUpdateModifiedContactList[index].modifiedContactMCI_CID);
            if (lookup != null && lookup.CONTACT_TYPE != 0)
            {
              lookup.Dirty = DBUpdateModifiedContactList[index].modifiedContactDirty;
              lookup.HASH = DBUpdateModifiedContactList[index].modifiedContactContactHash;
              Contacts contacts = new Contacts();
              Contacts contact = DBUpdateModifiedContactList[index].contact;
              if (contact != null)
              {
                lookup.name_family = contact.modifiedContactNameFamily;
                lookup.name_given = contact.modifiedContactNameGiven;
                lookup.name_middle = contact.modifiedContactNameMiddle;
                lookup.name_nickname = contact.modifiedContactNameNickname;
                lookup.name_prefix = contact.modifiedContactNamePrefix;
                lookup.name_suffix = contact.modifiedContactNameSuffix;
                lookup.company_name = contact.modifiedContactCompanyName;
                lookup.job_title = contact.modifiedContactJobTitle;
              }
              lookupList.Add(lookup);
              if (DBUpdateModifiedContactList[index].modifiedContactDirty == 2)
              {
                ImageHandler imageHandler = Conn.Find<ImageHandler>((object) DBUpdateModifiedContactList[index].modifiedContactMCI_CID);
                if (imageHandler != null)
                {
                  imageHandler.CONTACT_PHOTO_URL = string.Empty;
                  imageHandler.HASH = DBUpdateModifiedContactList[index].modifiedContactImageHash;
                  imageHandlerList.Add(imageHandler);
                }
              }
            }
          }
          if (lookupList.Count > 0)
            Conn.RunInTransaction((Action) (() => Conn.UpdateAll((IEnumerable) lookupList)));
          if (imageHandlerList.Count <= 0)
            return;
          Conn.RunInTransaction((Action) (() => Conn.UpdateAll((IEnumerable) imageHandlerList)));
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in marking and updating modified entries as dirty in DB. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void printTable(string DBName)
    {
      try
      {
        List<Lookup> lookupList1 = new List<Lookup>();
        List<string> stringList = new List<string>();
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          List<Lookup> lookupList2 = sqLiteConnection.Query<Lookup>("select * from Lookup where CONTACT_TYPE != ?", (object) 2);
          if (lookupList2 == null)
            return;
          foreach (Lookup lookup in lookupList2)
            stringList.Add(lookup.name_given + " " + lookup.name_middle + " " + lookup.name_family + " " + lookup.MCI_CID);
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in reading LT. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void getDebugData(string DBName, string ID, out string MCI_CID, out int Dirty)
    {
      try
      {
        MCI_CID = string.Empty;
        Dirty = 0;
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          Lookup lookup = sqLiteConnection.Query<Lookup>("select MCI_CID,Dirty from Lookup where ID = ?", (object) ID).FirstOrDefault<Lookup>();
          if (lookup == null)
            return;
          MCI_CID = lookup.MCI_CID;
          Dirty = lookup.Dirty;
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in getting debug data from LT. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void markDeletedDirtyEntries(string DBName, List<string> MCI_CIDList)
    {
      try
      {
        List<Lookup> lookupList = new List<Lookup>();
        using (SQLiteConnection Conn = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          for (int index = 0; index < MCI_CIDList.Count; ++index)
          {
            string mciCid = MCI_CIDList[index];
            Lookup lookup = Conn.Find<Lookup>((object) mciCid);
            if (lookup != null)
            {
              if (lookup.CONTACT_TYPE != 0)
              {
                lookup.Dirty = 3;
                lookupList.Add(lookup);
              }
              else
              {
                Conn.Delete<Lookup>((object) mciCid);
                Conn.Delete<ImageHandler>((object) mciCid);
              }
            }
          }
          if (lookupList.Count <= 0)
            return;
          Conn.RunInTransaction((Action) (() => Conn.UpdateAll((IEnumerable) lookupList)));
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in marking deleted entries as dirty in LT. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public async Task readAndMarkDeletedDirtyEntries(string DBName, List<string> IDList)
    {
      try
      {
        List<Lookup> lookupList = new List<Lookup>();
        using (SQLiteConnection Conn = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          for (int i = 0; i < IDList.Count; ++i)
          {
            string MCI_CID = string.Empty;
            Lookup existingL = Conn.Query<Lookup>("select * from Lookup where ID = ?", (object) IDList[i]).FirstOrDefault<Lookup>();
            if (existingL != null)
            {
              MCI_CID = existingL.MCI_CID;
              if (existingL.CONTACT_TYPE != 0)
              {
                existingL.Dirty = 3;
                lookupList.Add(existingL);
              }
              else
              {
                Conn.Delete<Lookup>((object) MCI_CID);
                Conn.Delete<ImageHandler>((object) MCI_CID);
              }
              await LocalSettings.localSettings.deleteContactFile(MCI_CID);
            }
          }
          if (lookupList.Count <= 0)
            return;
          Conn.RunInTransaction((Action) (() => Conn.UpdateAll((IEnumerable) lookupList)));
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in reading and marking deleted entries as dirty in LT. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void updateImageHash(string DBName, string MCI_CID, string hash)
    {
      try
      {
        using (SQLiteConnection Conn = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          ImageHandler existingI = Conn.Find<ImageHandler>((object) MCI_CID);
          if (existingI == null)
            return;
          existingI.HASH = hash;
          Conn.RunInTransaction((Action) (() => Conn.Update((object) existingI)));
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in updating Imahe Hash from IMT. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void updateID(string DBName, string MCI_CID, string ID)
    {
      try
      {
        using (SQLiteConnection Conn = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          Lookup existingL = Conn.Find<Lookup>((object) MCI_CID);
          if (existingL == null)
            return;
          existingL.ID = ID;
          Conn.RunInTransaction((Action) (() => Conn.Update((object) existingL)));
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in updating ID after restore. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void unmarkDirtyEntries(string DBName, List<string> MCI_CIDList)
    {
      try
      {
        List<Lookup> lookupList = new List<Lookup>();
        using (SQLiteConnection Conn = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          for (int index = 0; index < MCI_CIDList.Count; ++index)
          {
            Lookup lookup = Conn.Find<Lookup>((object) MCI_CIDList[index]);
            if (lookup != null)
            {
              lookup.Dirty = 0;
              lookupList.Add(lookup);
            }
          }
          if (lookupList.Count <= 0)
            return;
          Conn.RunInTransaction((Action) (() => Conn.UpdateAll((IEnumerable) lookupList)));
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in unmarking and updating dirty entries from DB. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void deleteMultipleDirtyEntries(string DBName, List<string> toDeleteMCI_CIDList)
    {
      try
      {
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          for (int index = 0; index < toDeleteMCI_CIDList.Count; ++index)
          {
            sqLiteConnection.Delete<Lookup>((object) toDeleteMCI_CIDList[index]);
            sqLiteConnection.Delete<ImageHandler>((object) toDeleteMCI_CIDList[index]);
          }
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in deleting dirty entries from DB. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void deleteAllEntries(string DBName)
    {
      try
      {
        using (SQLiteConnection Conn = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
          Conn.RunInTransaction((Action) (() =>
          {
            Conn.DeleteAll<Lookup>();
            Conn.DeleteAll<ImageHandler>();
          }));
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in deleting all entries from DB. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void getDirtyEntries(
      string DBName,
      out List<string> modifiedContact_IDList,
      out List<string> modifiedContact_MCI_CIDList,
      out List<string> modifiedContactBaseVersionList,
      out List<int> modifiedContactDirtyList,
      out List<int> modifiedContactContactTypeList)
    {
      modifiedContact_IDList = new List<string>();
      modifiedContact_MCI_CIDList = new List<string>();
      modifiedContactBaseVersionList = new List<string>();
      modifiedContactDirtyList = new List<int>();
      modifiedContactContactTypeList = new List<int>();
      try
      {
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          List<Lookup> lookupList = sqLiteConnection.Query<Lookup>("select ID, MCI_CID, BASE_VERSION, Dirty, CONTACT_TYPE from Lookup where Dirty != ?", (object) 0);
          if (lookupList == null)
            return;
          foreach (Lookup lookup in lookupList)
          {
            modifiedContact_IDList.Add(lookup.ID);
            modifiedContact_MCI_CIDList.Add(lookup.MCI_CID);
            modifiedContactBaseVersionList.Add(lookup.BASE_VERSION);
            modifiedContactDirtyList.Add(lookup.Dirty);
            modifiedContactContactTypeList.Add(lookup.CONTACT_TYPE);
          }
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in retrieving dirty entries from DB. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public int getDirtyIntouchEntriesCount(string DBName)
    {
      try
      {
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
          return sqLiteConnection.Query<Lookup>("select MCI_CID from Lookup where Dirty != ?", (object) 0).Count;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in gettinng count of dirty entries from DB. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public List<string> getWPOtherContactMCI_CIDList(string DBName)
    {
      try
      {
        List<string> contactMciCidList = new List<string>();
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          List<Lookup> lookupList = sqLiteConnection.Query<Lookup>("select MCI_CID from Lookup where CONTACT_TYPE = ?", (object) 2);
          if (lookupList != null)
          {
            foreach (Lookup lookup in lookupList)
              contactMciCidList.Add(lookup.MCI_CID);
          }
        }
        return contactMciCidList;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in retrieving WPOtherContactMCI_CIDList. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public List<string> getWPInTouchContactMCI_CIDList(
      string DBName,
      out List<string> IDList,
      bool Entire_Restore)
    {
      try
      {
        List<string> contactMciCidList = new List<string>();
        IDList = new List<string>();
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          List<Lookup> lookupList = sqLiteConnection.Query<Lookup>("select MCI_CID, ID from Lookup where CONTACT_TYPE != ?", (object) 2);
          if (lookupList != null)
          {
            foreach (Lookup lookup in lookupList)
            {
              contactMciCidList.Add(lookup.MCI_CID);
              if (!Entire_Restore)
                IDList.Add(lookup.ID);
            }
          }
        }
        return contactMciCidList;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in retrieving WPInTouchContactMCI_CIDList. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public List<ContactSample> readAllDBData(string DBName)
    {
      try
      {
        List<Lookup> lookupList1 = new List<Lookup>();
        List<ContactSample> contactSampleList = new List<ContactSample>();
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          List<Lookup> lookupList2 = sqLiteConnection.Query<Lookup>("select ID, name_given, name_family, name_prefix, name_middle, name_nickname, name_suffix, job_title, company_name from Lookup where CONTACT_TYPE != ? AND Dirty != ?", (object) 2, (object) 3);
          if (lookupList2 != null)
          {
            foreach (Lookup lookup in lookupList2)
            {
              ContactSample contactSample = new ContactSample();
              string str1 = lookup.name_given + " " + lookup.name_middle + " " + lookup.name_family + " " + lookup.name_suffix;
              string str2 = lookup.name_prefix + " " + str1;
              contactSample.mContactName = string.Join(" ", str1.Split(new char[1]
              {
                ' '
              }, StringSplitOptions.RemoveEmptyEntries));
              contactSample.mContactDisplayName = string.Join(" ", str2.Split(new char[1]
              {
                ' '
              }, StringSplitOptions.RemoveEmptyEntries));
              contactSample.mContactOrganizationInfo = ContactSample.getOrganizationInfo(lookup.company_name, lookup.job_title);
              contactSample.contactID = lookup.ID;
              contactSample.mContactDisplayImage = new BitmapImage(new Uri("ms-appx:///Assets/in_img_default_profile_48dp.png"));
              contactSampleList.Add(contactSample);
            }
          }
        }
        return contactSampleList;
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in reading all DB data. " + ex.Message, EventType.Error);
        throw;
      }
    }

    public void updateDBForContacts(string DBName, List<Contacts> contactList, List<string> IDList)
    {
      try
      {
        List<Lookup> lookupList = new List<Lookup>();
        using (SQLiteConnection Conn = new SQLiteConnection(Path.Combine(this.applicationFolderPath, DBName)))
        {
          for (int index = 0; index < IDList.Count; ++index)
          {
            Lookup lookup = Conn.Query<Lookup>("select * from Lookup where ID = ?", (object) IDList[index]).FirstOrDefault<Lookup>();
            if (lookup != null)
            {
              Contacts contacts = new Contacts();
              Contacts contact = contactList[index];
              lookup.name_family = contact.modifiedContactNameFamily;
              lookup.name_given = contact.modifiedContactNameGiven;
              lookup.name_middle = contact.modifiedContactNameMiddle;
              lookup.name_nickname = contact.modifiedContactNameNickname;
              lookup.name_prefix = contact.modifiedContactNamePrefix;
              lookup.name_suffix = contact.modifiedContactNameSuffix;
              lookup.company_name = contact.modifiedContactCompanyName;
              lookup.job_title = contact.modifiedContactJobTitle;
              lookupList.Add(lookup);
            }
          }
          if (lookupList.Count <= 0)
            return;
          Conn.RunInTransaction((Action) (() => Conn.UpdateAll((IEnumerable) lookupList)));
        }
      }
      catch (Exception ex)
      {
        LogFile.Log("Error in updating DB fo0r contacts. " + ex.Message, EventType.Error);
        throw;
      }
    }
  }
}
