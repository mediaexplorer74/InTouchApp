// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.Lookup
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using SQLite;

#nullable disable
namespace InTouchLibrary
{
  public class Lookup
  {
    [PrimaryKey]
    public string MCI_CID { get; set; }

    [Unique]
    public string ID { get; set; }

    public string BASE_VERSION { get; set; }

    public int CONTACT_TYPE { get; set; }

    public int Dirty { get; set; }

    public string HASH { get; set; }

    public string DEVICE_AGGR_ID { get; set; }

    public string LINKED_CONTACT_ID { get; set; }

    public string name_given { get; set; }

    public string name_family { get; set; }

    public string name_prefix { get; set; }

    public string name_middle { get; set; }

    public string name_nickname { get; set; }

    public string name_suffix { get; set; }

    public string photo_uri { get; set; }

    public string job_title { get; set; }

    public string company_name { get; set; }

    public string user_id { get; set; }

    public bool read_only { get; set; }

    public string type { get; set; }

    public string permission { get; set; }

    public string context { get; set; }

    public string status { get; set; }

    public string time_added { get; set; }

    public string time_modififed { get; set; }

    public string time_contacted { get; set; }

    public bool starred { get; set; }

    public string city_work { get; set; }

    public string city_home { get; set; }

    public string country { get; set; }

    public string birthday { get; set; }
  }
}
