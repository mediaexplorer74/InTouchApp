// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.RootObjectContactbookAuto
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System.Collections.Generic;

#nullable disable
namespace InTouchLibrary
{
  public class RootObjectContactbookAuto
  {
    public string status { get; set; }

    public string mesg_url { get; set; }

    public PendingRequests pending_requests { get; set; }

    public List<ContactContactbookAuto> contacts { get; set; }

    public int show_notification { get; set; }

    public string stpin { get; set; }

    public string mesg_line_3 { get; set; }

    public string mesg_line_2 { get; set; }

    public string mesg_line_1 { get; set; }

    public int version { get; set; }

    public string action { get; set; }

    public int cbook_api_version_minor { get; set; }

    public int cbook_api_version_major { get; set; }

    public string message { get; set; }
  }
}
