// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.ContactUpload
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System.Collections.Generic;

#nullable disable
namespace InTouchLibrary
{
  public class ContactUpload
  {
    public string origin { get; set; }

    public string contact_id_1 { get; set; }

    public string contact_device_id { get; set; }

    public string contact_device_aggr_id { get; set; }

    public string base_version { get; set; }

    public Name name { get; set; }

    public string gender { get; set; }

    public List<Avatar> avatars { get; set; }

    public List<Note> notes { get; set; }

    public string action { get; set; }

    public bool read_only { get; set; }

    public string account_type { get; set; }

    public string account_name { get; set; }

    public List<string> winphone_account { get; set; }
  }
}
