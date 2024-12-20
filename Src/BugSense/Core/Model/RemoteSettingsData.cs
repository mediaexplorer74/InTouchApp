// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.RemoteSettingsData
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System.Runtime.Serialization;


namespace BugSense.Core.Model
{
  [DataContract]
  public class RemoteSettingsData
  {
    [DataMember(Name = "logLevel")]
    public string LogLevel { get; set; }

    [DataMember(Name = "version")]
    public string Version { get; set; }

    [DataMember(Name = "refreshEveryXseconds")]
    public int RefreshInterval { get; set; }

    [DataMember(Name = "netMonitoring")]
    public bool NetMonitoring { get; set; }

    [DataMember(Name = "hashCode")]
    public string HashCode { get; set; }

    public static RemoteSettingsData Instance => RemoteSettingsData.Nested.instance;

    public RemoteSettingsData()
    {
      this.LogLevel = "verbose";
      this.Version = "1.0";
      this.RefreshInterval = 60;
      this.HashCode = "none";
    }

    private class Nested
    {
      internal static readonly RemoteSettingsData instance = new RemoteSettingsData();
    }
  }
}
