// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.UploadContactbook
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System.Collections.Generic;

#nullable disable
namespace InTouchLibrary
{
  public class UploadContactbook
  {
    public string session_id { get; set; }

    public int packet_id { get; set; }

    public List<ContactUpload> contacts { get; set; }
  }
}
