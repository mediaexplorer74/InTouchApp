// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.ContactContactbookManual
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System.Collections.Generic;

#nullable disable
namespace InTouchLibrary
{
  public class ContactContactbookManual
  {
    public string contact_id_1 { get; set; }

    public string contact_device_id { get; set; }

    public string contact_device_aggr_id { get; set; }

    public string connection_status { get; set; }

    public int version { get; set; }

    public Name name { get; set; }

    public string gender { get; set; }

    public List<Avatar> avatars { get; set; }

    public List<Note> notes { get; set; }
  }
}
