// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.ImageHandler
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using SQLite;

#nullable disable
namespace InTouchLibrary
{
  public class ImageHandler
  {
    [PrimaryKey]
    public string MCI_CID { get; set; }

    [Unique]
    public string ID { get; set; }

    public string CONTACT_PHOTO_URL { get; set; }

    public bool DOWNLOAD_STATE { get; set; }

    public string HASH { get; set; }
  }
}
