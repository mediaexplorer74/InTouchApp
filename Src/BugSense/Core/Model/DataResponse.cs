// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.DataResponse
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System.Runtime.Serialization;


namespace BugSense.Core.Model
{
  [DataContract]
  public class DataResponse
  {
    [DataMember(Name = "url")]
    public string Url { get; set; }

    [DataMember(Name = "contentText")]
    public string ContentText { get; set; }

    [DataMember(Name = "eid")]
    public long Eid { get; set; }

    [DataMember(Name = "tickerText")]
    public string TickerText { get; set; }

    [DataMember(Name = "contentTitle")]
    public string ContentTitle { get; set; }
  }
}
