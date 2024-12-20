// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.Avatar
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System.Collections.Generic;

#nullable disable
namespace InTouchLibrary
{
  public class Avatar
  {
    public string label { get; set; }

    public string type { get; set; }

    public Name name { get; set; }

    public string gender { get; set; }

    public List<Address> address { get; set; }

    public List<Email> email { get; set; }

    public List<Organization> organization { get; set; }

    public List<Event> @event { get; set; }

    public List<Phone> phone { get; set; }

    public List<Social> social { get; set; }

    public List<Website> website { get; set; }

    public List<Photo> photo { get; set; }

    public List<Note> notes { get; set; }

    public string headline { get; set; }

    public object bio { get; set; }

    public string label_char { get; set; }
  }
}
