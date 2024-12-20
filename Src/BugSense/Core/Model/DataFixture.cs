// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.DataFixture
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;
using System.Runtime.Serialization;


namespace BugSense.Core.Model
{
  [DataContract]
  public abstract class DataFixture
  {
    private string _remoteIp = "{%#@@#%}";
    private string _msFromstart;

    [DataMember(Name = "sdkVersion", EmitDefaultValue = false)]
    public string SdkVersion { get; set; }

    [DataMember(Name = "sdkPlatform", EmitDefaultValue = false)]
    public string SdkPlatform { get; set; }

    [DataMember(Name = "type", EmitDefaultValue = false)]
    public string TypeRequest { get; set; }

    [DataMember(Name = "device", EmitDefaultValue = false)]
    public string PhoneModel { get; set; }

    [DataMember(Name = "osVersion")]
    public string OsVersion { get; set; }

    [DataMember(Name = "locale", EmitDefaultValue = false)]
    public string Locale { get; set; }

    [DataMember(Name = "uuid", EmitDefaultValue = false)]
    public string Uid { get; set; }

    [DataMember(Name = "userIdentifier", EmitDefaultValue = false)]
    public string UserIdentifier { get; set; }

    [DataMember(Name = "timestamp")]
    public string Timestamp { get; set; }

    [DataMember(Name = "carrier", EmitDefaultValue = false)]
    public string Carrier { get; set; }

    [DataMember(Name = "remote_ip")]
    public string RemoteIp
    {
      get => this._remoteIp;
      set => this._remoteIp = value;
    }

    [DataMember(Name = "connection")]
    public string Connection { get; set; }

    [DataMember(Name = "appVersionCode", EmitDefaultValue = false)]
    public string AppVersionCode { get; set; }

    [DataMember(Name = "appVersionName", EmitDefaultValue = false)]
    public string AppVersionName { get; set; }

    [DataMember(Name = "packageName", EmitDefaultValue = false)]
    public string PackageName { get; set; }

    [DataMember(Name = "appVersion", EmitDefaultValue = false)]
    public string AppVersion { get; set; }

    [DataMember(Name = "appVersionShort", EmitDefaultValue = false)]
    public string AppVersionShort { get; set; }

    [DataMember(Name = "binaryName", EmitDefaultValue = false)]
    public string BinaryName { get; set; }

    [DataMember(Name = "msFromStart", EmitDefaultValue = false)]
    public string MsFromStart
    {
      get
      {
        return this._msFromstart = DateTime.UtcNow.Subtract(SessionManager.Instance.SessionStart).Milliseconds.ToString();
      }
      set => this._msFromstart = value;
    }
  }
}
