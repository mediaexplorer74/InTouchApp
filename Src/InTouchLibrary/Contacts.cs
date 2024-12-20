// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.Contacts
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System.Collections.Generic;

#nullable disable
namespace InTouchLibrary
{
  public class Contacts
  {
    public string modifiedContactNameFamily;
    public string modifiedContactNameGiven;
    public string modifiedContactNameMiddle;
    public string modifiedContactNameNickname;
    public string modifiedContactNamePrefix;
    public string modifiedContactNameSuffix;
    public string modifiedContactCompanyName;
    public string modifiedContactJobTitle;

    public static Contacts getContact(Name name, List<Organization> organization)
    {
      try
      {
        Contacts contact = new Contacts();
        if (name != null)
        {
          contact.modifiedContactNameFamily = name.family;
          contact.modifiedContactNameGiven = name.given;
          contact.modifiedContactNameMiddle = name.middle;
          contact.modifiedContactNameNickname = name.nickname;
          contact.modifiedContactNamePrefix = name.prefix;
          contact.modifiedContactNameSuffix = name.suffix;
        }
        if (organization != null)
        {
          foreach (Organization organization1 in organization)
          {
            contact.modifiedContactCompanyName = organization1.company;
            contact.modifiedContactJobTitle = organization1.position;
          }
        }
        return contact;
      }
      catch
      {
        throw;
      }
    }
  }
}
