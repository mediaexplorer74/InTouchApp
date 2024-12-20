// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.BugSensePerformance
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System.Runtime.Serialization;


namespace BugSense.Core.Model
{
  [DataContract]
  public class BugSensePerformance
  {
    [DataMember(Name = "appMemAvail")]
    public double AppMemAvail { get; set; }

    [DataMember(Name = "appMemMax")]
    public double AppMemMax { get; set; }

    [DataMember(Name = "appMemTotal")]
    public double AppMemTotal { get; set; }

    [DataMember(Name = "sysMemAvail")]
    public double SysMemAvail { get; set; }

    [DataMember(Name = "sysMemLow")]
    public string SysMemLow { get; set; }

    [DataMember(Name = "sysMemThreshold")]
    public double SysMemThreshold { get; set; }

    public BugSensePerformance()
    {
      this.AppMemAvail = 0.0;
      this.AppMemMax = 0.0;
      this.AppMemTotal = 0.0;
      this.SysMemAvail = 0.0;
      this.SysMemLow = "False";
      this.SysMemThreshold = 0.0;
    }
  }
}
