// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.ExceptionDataFixture
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


namespace BugSense.Core.Model
{
  [DataContract]
  public class ExceptionDataFixture : DataFixture
  {
    [DataMember(Name = "stacktrace", EmitDefaultValue = false)]
    public string Stacktrace { get; set; }

    [DataMember(Name = "handled", EmitDefaultValue = false)]
    public string Handled { get; set; }

    [DataMember(Name = "klass", EmitDefaultValue = false)]
    public string Klass { get; set; }

    [DataMember(Name = "message", EmitDefaultValue = false)]
    public string Message { get; set; }

    [DataMember(Name = "errorHash", EmitDefaultValue = false)]
    public string ErrorHash { get; set; }

    [DataMember(Name = "where", EmitDefaultValue = false)]
    public string Where { get; set; }

    [DataMember(Name = "rooted")]
    public bool Rooted { get; set; }

    [DataMember(Name = "extra_data", EmitDefaultValue = false)]
    public Dictionary<string, string> LogData { get; set; }

    [DataMember(Name = "gpsStatus", EmitDefaultValue = false)]
    public string GpsStatus { get; set; }

    [DataMember(Name = "memAppTotal")]
    public double MemAppTotal { get; set; }

    [DataMember(Name = "breadcrumbs", EmitDefaultValue = false)]
    public string[] Breadcrumbs { get; set; }

    [DataMember(Name = "appsRunning", EmitDefaultValue = false)]
    public string AppsRunning { get; set; }

    [DataMember(Name = "memSysLow")]
    public string MemSysLow { get; set; }

    [DataMember(Name = "memSysAvailable")]
    public double MemSysAvailable { get; set; }

    [DataMember(Name = "memSysThreshold")]
    public double MemSysThreshold { get; set; }

    [DataMember(Name = "memAppMax")]
    public double MemAppMax { get; set; }

    [DataMember(Name = "memAppAvailable")]
    public double MemAppAvailable { get; set; }

    public static ExceptionDataFixture GetInstance(
      Exception exception,
      AppEnvironment appEnvironment,
      BugSensePerformance bugSensePerformance,
      bool handled,
      LimitedCrashExtraDataList extraData = null)
    {
      string gpsOn = appEnvironment.GpsOn;
      string str = handled ? Enum.GetName(typeof (CrashType), (object) CrashType.HANDLED) : Enum.GetName(typeof (CrashType), (object) CrashType.UNHANDLED);
      try
      {
        gpsOn = ((StateStatus) Convert.ToInt32(appEnvironment.GpsOn)).ToString();
      }
      catch
      {
      }
      ExceptionDataFixture exceptionDataFixture = new ExceptionDataFixture();
      exceptionDataFixture.AppVersionCode = appEnvironment.AppVersion;
      exceptionDataFixture.AppVersion = appEnvironment.AppVersion;
      exceptionDataFixture.AppVersionName = appEnvironment.AppName;
      exceptionDataFixture.BinaryName = appEnvironment.AppName;
      exceptionDataFixture.Locale = appEnvironment.Locale;
      exceptionDataFixture.OsVersion = appEnvironment.OsVersion;
      exceptionDataFixture.PhoneModel = appEnvironment.PhoneModel;
      exceptionDataFixture.Rooted = appEnvironment.Rooted;
      exceptionDataFixture.SdkPlatform = BugSenseProperties.BugSenseName;
      exceptionDataFixture.SdkVersion = BugSenseProperties.BugSenseVersion;
      exceptionDataFixture.TypeRequest = JsonRequestType.Error;
      exceptionDataFixture.Timestamp = DateTime.UtcNow.DateTimeToUnixTimestamp().ToString();
      exceptionDataFixture.Uid = BugSenseProperties.UID;
      exceptionDataFixture.UserIdentifier = BugSenseProperties.UserIdentifier;
      exceptionDataFixture.AppsRunning = "Unknown";
      exceptionDataFixture.Breadcrumbs = ExtraData.BreadCrumbs.ToArrayString();
      exceptionDataFixture.GpsStatus = gpsOn;
      exceptionDataFixture.Handled = str;
      exceptionDataFixture.Klass = exception.GetType().FullName;
      exceptionDataFixture.MemAppAvailable = bugSensePerformance.AppMemAvail;
      exceptionDataFixture.MemAppMax = bugSensePerformance.AppMemMax;
      exceptionDataFixture.MemAppTotal = bugSensePerformance.AppMemTotal;
      exceptionDataFixture.MemSysAvailable = bugSensePerformance.SysMemAvail;
      exceptionDataFixture.MemSysLow = bugSensePerformance.SysMemLow;
      exceptionDataFixture.MemSysThreshold = bugSensePerformance.SysMemThreshold;
      exceptionDataFixture.Message = exception.Message;
      exceptionDataFixture.Stacktrace = StacktraceHelper.GetStackTrace(exception);
      exceptionDataFixture.Carrier = appEnvironment.Carrier;
      exceptionDataFixture.Connection = Enum.GetName(typeof (ConnectionType), (object) BugSenseProperties.Connection);
      ExceptionDataFixture dataFixture = exceptionDataFixture;
      try
      {
        dataFixture.AppVersionShort = appEnvironment.AppVersion.Substring(0, 3);
      }
      catch
      {
      }
      HashSignature hashSignature = new ErrorHashSignature().GetHashSignature(BugSenseProperties.AppName, BugSenseProperties.AppVersion, exception.StackTrace, exception.Message, exception.HResultEx());
      dataFixture.ErrorHash = hashSignature.Signature.ToLower();
      if (extraData != null && extraData.Count > 0)
      {
        dataFixture.LogData = new Dictionary<string, string>(extraData.Count);
        Dictionary<string, string>.KeyCollection keys = dataFixture.LogData.Keys;
        foreach (CrashExtraData crashExtraData in extraData)
          ExceptionDataFixture.AddCrashExtraDataToDictionary(crashExtraData, dataFixture);
      }
      else
      {
        dataFixture.LogData = new Dictionary<string, string>();
        Dictionary<string, string>.KeyCollection keys = dataFixture.LogData.Keys;
      }
      if (ExtraData.CrashExtraData.Count > 0)
      {
        foreach (CrashExtraData crashExtraData in ExtraData.CrashExtraData)
          ExceptionDataFixture.AddCrashExtraDataToDictionary(crashExtraData, dataFixture);
      }
      return dataFixture;
    }

    private static void AddCrashExtraDataToDictionary(
      CrashExtraData crashExtraData,
      ExceptionDataFixture dataFixture)
    {
      if (string.IsNullOrWhiteSpace(dataFixture.LogData.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (x => x.Key.Equals(crashExtraData.Key))).Key))
        dataFixture.LogData.Add(crashExtraData.Key, crashExtraData.Value);
      else
        dataFixture.LogData[crashExtraData.Key] = crashExtraData.Value;
    }
  }
}
