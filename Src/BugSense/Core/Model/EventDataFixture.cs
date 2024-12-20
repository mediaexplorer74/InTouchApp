// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.EventDataFixture
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Helpers;
using System;
using System.Runtime.Serialization;


namespace BugSense.Core.Model
{
  [DataContract]
  public class EventDataFixture : DataFixture
  {
    private TimeSpan _duration;

    public bool IsPing
    {
      get
      {
        return this.TypeRequest.Equals(Enum.GetName(typeof (DataType), (object) DataType.ping), StringComparison.OrdinalIgnoreCase);
      }
    }

    [DataMember(Name = "eventName")]
    public string EventName { get; set; }

    [DataMember(Name = "eventLevel")]
    public int EventLevel { get; set; }

    [DataMember(Name = "duration")]
    public TimeSpan Duration
    {
      get
      {
        if (this.TypeRequest == JsonRequestType.Gnip)
          this._duration = DateTime.UtcNow.Subtract(SessionManager.Instance.PingSessionStart);
        return this._duration;
      }
      set => this._duration = value;
    }

    public static EventDataFixture GetInstance(string eventTag, AppEnvironment appEnvironment)
    {
      string str = "event";
      EventDataFixture instance = new EventDataFixture();
      if (eventTag.Equals(JsonRequestType.Ping, StringComparison.OrdinalIgnoreCase))
        str = JsonRequestType.Ping;
      else if (eventTag.Equals(JsonRequestType.Gnip, StringComparison.OrdinalIgnoreCase))
        str = JsonRequestType.Gnip;
      instance.AppVersionCode = appEnvironment.AppVersion;
      instance.AppVersion = appEnvironment.AppVersion;
      instance.AppVersionName = appEnvironment.AppName;
      instance.BinaryName = appEnvironment.AppName;
      instance.Locale = appEnvironment.Locale;
      instance.OsVersion = appEnvironment.OsVersion;
      instance.PhoneModel = appEnvironment.PhoneModel;
      instance.SdkPlatform = BugSenseProperties.BugSenseName;
      instance.SdkVersion = BugSenseProperties.BugSenseVersion;
      instance.TypeRequest = str;
      instance.Timestamp = DateTime.UtcNow.DateTimeToUnixTimestamp().ToString();
      instance.Uid = BugSenseProperties.UID;
      instance.UserIdentifier = BugSenseProperties.UserIdentifier;
      instance.EventName = eventTag;
      instance.Carrier = appEnvironment.Carrier;
      instance.Connection = Enum.GetName(typeof (ConnectionType), (object) BugSenseProperties.Connection);
      try
      {
        instance.AppVersionShort = appEnvironment.AppVersion.Substring(0, 3);
      }
      catch
      {
      }
      return instance;
    }

    public static EventDataFixture GetInstance(
      BugSenseEventTag eventTag,
      AppEnvironment appEnvironment)
    {
      return EventDataFixture.GetInstance(eventTag == BugSenseEventTag.Ping ? JsonRequestType.Ping : JsonRequestType.Gnip, appEnvironment);
    }
  }
}
