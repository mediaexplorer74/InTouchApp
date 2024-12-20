// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.ContactContactbookAuto
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System.Collections.Generic;

#nullable disable
namespace InTouchLibrary
{
  public class ContactContactbookAuto
  {
    public string account_type { get; set; }

    public List<Avatar> avatars { get; set; }

    public string connection_status { get; set; }

    public long mci { get; set; }

    public string mci_display { get; set; }

    public string name_display { get; set; }

    public string notes { get; set; }

    public string iid { get; set; }

    public List<object> tags { get; set; }

    public string gender { get; set; }
  }
}
