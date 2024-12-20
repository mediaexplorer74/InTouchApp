// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.ProfileToken
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System.Collections.Generic;

#nullable disable
namespace InTouchLibrary
{
  public class ProfileToken
  {
    public string name_first { get; set; }

    public string name_last { get; set; }

    public string mci { get; set; }

    public string gender { get; set; }

    public string mci_str { get; set; }

    public List<Avatar> avatars { get; set; }

    public string name_display { get; set; }

    public string image_url { get; set; }

    public string iid_display_num { get; set; }
  }
}
