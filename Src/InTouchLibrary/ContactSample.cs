// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.ContactSample
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Phone.PersonalInformation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

#nullable disable
namespace InTouchLibrary
{
  public class ContactSample
  {
    public string contactID { get; set; }

    public string mContactOrganizationInfo { get; set; }

    public string mContactDisplayName { get; set; }

    public string mContactName { get; set; }

    public BitmapImage mContactDisplayImage { get; set; }

    public static string getOrganizationInfo(string companyName, string jobTitle)
    {
      try
      {
        string organizationInfo = string.Empty + companyName;
        if (!string.IsNullOrEmpty(jobTitle))
          organizationInfo = string.IsNullOrEmpty(organizationInfo) ? jobTitle : organizationInfo + "/" + jobTitle;
        return organizationInfo;
      }
      catch
      {
        throw;
      }
    }

    public static async Task<BitmapImage> getWPDisplayImageFromID(string ID)
    {
      BitmapImage displayImageFromId;
      try
      {
        if (ContactstoreInTouch.contactStore == null)
        {
          Tuple<bool, bool> result = await ContactstoreInTouch.contactStoreInTouch.GetContactStore();
          if (result.Item2)
            await RestoreContactStore.restoreContactStore.restoreContacts((List<string>) null, true);
        }
        StoredContact contact = (StoredContact) null;
        try
        {
          contact = await ContactstoreInTouch.contactStore.FindContactByIdAsync(ID);
        }
        catch (Exception ex)
        {
          LogFile.Log("Error in reading contact from WP. " + ex.Message, EventType.Error);
        }
        BitmapImage displayImage = new BitmapImage();
        if (contact != null)
        {
          IRandomAccessStream PictureStream = await contact.GetDisplayPictureAsync();
          if (PictureStream != null)
          {
            ((BitmapSource) displayImage).SetSource(PictureStream);
            ((IDisposable) PictureStream).Dispose();
          }
        }
        displayImageFromId = displayImage;
      }
      catch (Exception ex)
      {
        LogFile.Log("Problem in getting display image." + ex.Message, EventType.Warning);
        throw;
      }
      return displayImageFromId;
    }
  }
}
