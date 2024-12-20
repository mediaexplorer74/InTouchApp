// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.NetworkDataFixture
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace BugSense.Core.Model
{
  [DataContract]
  public class NetworkDataFixture : DataFixture
  {
    private long _endTime;

    [DataMember(Name = "url")]
    public string Url { get; set; }

    [DataMember(Name = "duration")]
    public long Duration { get; set; }

    [DataMember(Name = "statusCode")]
    public int StatusCode { get; set; }

    [DataMember(Name = "contentLength")]
    public double ContentLength { get; set; }

    [DataMember(Name = "requestLength")]
    public double RequestLength { get; set; }

    [DataMember(Name = "failed")]
    public bool Failed { get; set; }

    [DataMember(Name = "headers")]
    public Dictionary<string, string> Headers { get; set; }

    [DataMember(Name = "endTime")]
    public long EndTime
    {
      get
      {
        long num = 0;
        try
        {
          num = Convert.ToInt64(this.Timestamp);
        }
        catch
        {
        }
        return this._endTime = (long) (DateTime.UtcNow.DateTimeToUnixTimestamp() - (double) num);
      }
      set => this._endTime = value;
    }

    public static NetworkDataFixture GetNetworkDataFixture(AppEnvironment appEnvironment)
    {
      string gpsOn = appEnvironment.GpsOn;
      try
      {
        ((StateStatus) Convert.ToInt32(appEnvironment.GpsOn)).ToString();
      }
      catch
      {
      }
      NetworkDataFixture networkDataFixture = new NetworkDataFixture();
      networkDataFixture.AppVersionCode = appEnvironment.AppVersion;
      networkDataFixture.AppVersion = appEnvironment.AppVersion;
      networkDataFixture.AppVersionName = appEnvironment.AppName;
      networkDataFixture.BinaryName = appEnvironment.AppName;
      networkDataFixture.Locale = appEnvironment.Locale;
      networkDataFixture.OsVersion = appEnvironment.OsVersion;
      networkDataFixture.PhoneModel = appEnvironment.PhoneModel;
      networkDataFixture.SdkPlatform = BugSenseProperties.BugSenseName;
      networkDataFixture.SdkVersion = BugSenseProperties.BugSenseVersion;
      networkDataFixture.TypeRequest = JsonRequestType.Network;
      networkDataFixture.Timestamp = DateTime.UtcNow.DateTimeToUnixTimestamp().ToString();
      networkDataFixture.Uid = BugSenseProperties.UID;
      networkDataFixture.UserIdentifier = BugSenseProperties.UserIdentifier;
      networkDataFixture.Headers = new Dictionary<string, string>();
      return networkDataFixture;
    }

    public override string ToString()
    {
      return string.Format("URL: {0} - StatusCode: {1}", (object) this.Url, (object) this.StatusCode);
    }
  }
}
