// Decompiled with JetBrains decompiler
// Type: BugSense.Core.BugSenseProperties
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Helpers;
using BugSense.Core.Model;
using System;


namespace BugSense.Core
{
  internal class BugSenseProperties
  {
    private static string _apiKey;
    private static bool _proxyEnabled = false;
    private static string _debugTestUrl;
    private static string _uid;

    public static string APIKey
    {
      get => BugSenseProperties._apiKey;
      set
      {
        if (!(value != BugSenseProperties._apiKey) || string.IsNullOrWhiteSpace(value))
          return;
        BugSenseProperties._apiKey = value;
        BugSenseProperties.MakeAnalyticsUrl();
      }
    }

    public static bool ProxyEnabled
    {
      get => BugSenseProperties._proxyEnabled;
      set
      {
        if (value == BugSenseProperties._proxyEnabled || !string.IsNullOrWhiteSpace(BugSenseProperties._debugTestUrl))
          return;
        BugSenseProperties._proxyEnabled = value;
        BugSenseProperties.MakeAnalyticsUrl();
      }
    }

    private static bool HasDebugUrl => BugSenseProperties._debugTestUrl != null;

    public static string DebugTestUrl
    {
      get => BugSenseProperties._debugTestUrl;
      set
      {
        if (!(value != BugSenseProperties.Url))
          return;
        string str;
        BugSenseProperties._debugTestUrl = str = value;
        BugSenseProperties.AnalyticsURL = str;
        BugSenseProperties.Url = str;
      }
    }

    public static string UID
    {
      get
      {
        if (string.IsNullOrWhiteSpace(BugSenseProperties._uid))
          BugSenseProperties._uid = EntropyUUID.Get();
        return BugSenseProperties._uid;
      }
    }

    public static Action LastAction { get; set; }

    public static string Carrier { get; set; }

    public static string AppName { get; set; }

    public static bool Rooted { get; set; }

    public static string UserIdentifier { get; set; }

    public static bool HandleWhileDebugging { get; set; }

    public static string Url { get; private set; }

    public static string AnalyticsURL { get; private set; }

    public static int MaxExceptions { get; set; }

    public static string ExceptionsFolderName { get; set; }

    public static string GeneralFolderName { get; set; }

    public static string CrashOnLastRunFileName { get; set; }

    public static string BugSenseVersion { get; set; }

    public static string BugSenseName { get; set; }

    public static string UserAgent { get; set; }

    public static long TotalCrashes { get; set; }

    public static string AppVersion { get; set; }

    public static string Tag { get; private set; }

    public static string OSVersion { get; set; }

    public static string PhoneModel { get; set; }

    public static string PhoneBrand { get; set; }

    public static int WIFIOn { get; set; }

    public static int MobileNetOn { get; set; }

    public static int GPSOn { get; set; }

    public static ScreenProperties DeviceScreenProperties { get; set; }

    public static string Locale { get; set; }

    public static string AppsRunning { get; set; }

    public static string Rotation { get; set; }

    public static string Orientation { get; set; }

    public static string Architecture { get; set; }

    public static string Flavor { get; set; }

    public static string BugSenseFolderPath { get; set; }

    public static ConnectionType Connection { get; set; }

    static BugSenseProperties()
    {
      BugSenseProperties.HandleWhileDebugging = true;
      BugSenseProperties.AppName = string.Empty;
      BugSenseProperties.APIKey = string.Empty;
      BugSenseProperties.MaxExceptions = 5;
      BugSenseProperties.ExceptionsFolderName = "BugSense_Exceptions";
      BugSenseProperties.GeneralFolderName = "BugSense_General";
      BugSenseProperties.CrashOnLastRunFileName = "crashonlastrun.json";
      BugSenseProperties.Url = "https://www.bugsense.com/api/errors";
      BugSenseProperties.AnalyticsURL = "http://ticks2.bugsense.com/api/ticks/";
      BugSenseProperties.BugSenseVersion = "3.6";
      BugSenseProperties.AppVersion = "unknown";
      BugSenseProperties.Tag = "Splunk.MI";
      BugSenseProperties.OSVersion = "unknown";
      BugSenseProperties.PhoneModel = "unknown";
      BugSenseProperties.PhoneBrand = "unknown";
      BugSenseProperties.Orientation = "unknown";
      BugSenseProperties.UserAgent = "unknown";
      BugSenseProperties.WIFIOn = 2;
      BugSenseProperties.MobileNetOn = 2;
      BugSenseProperties.GPSOn = 2;
      BugSenseProperties.DeviceScreenProperties = new ScreenProperties();
    }

    public static string NewFileNamePath(FileNameType fileType)
    {
      string str = string.Empty;
      switch (fileType)
      {
        case FileNameType.UnhandledException:
          str = string.Format("{0}\\CCC_{1}_BugSense_Ex_{2}.dat", (object) BugSenseProperties.ExceptionsFolderName, (object) DateTime.UtcNow.ToString("yyyyMMddHHmmss"), (object) Guid.NewGuid());
          break;
        case FileNameType.LoggedException:
          str = string.Format("{0}\\LLC_{1}_BugSense_Ex_{2}.dat", (object) BugSenseProperties.ExceptionsFolderName, (object) DateTime.UtcNow.ToString("yyyyMMddHHmmss"), (object) Guid.NewGuid());
          break;
        case FileNameType.Ping:
          str = string.Format("{0}\\PCC_{1}_BugSense_Ev_{2}.dat", (object) BugSenseProperties.ExceptionsFolderName, (object) DateTime.UtcNow.ToString("yyyyMMddHHmmss"), (object) Guid.NewGuid());
          break;
        case FileNameType.Event:
          str = string.Format("{0}\\ECC_{1}_BugSense_Ev_{2}.dat", (object) BugSenseProperties.ExceptionsFolderName, (object) DateTime.UtcNow.ToString("yyyyMMddHHmmss"), (object) Guid.NewGuid());
          break;
      }
      return str;
    }

    private static void MakeAnalyticsUrl()
    {
      if (string.IsNullOrWhiteSpace(BugSenseProperties._apiKey) || BugSenseProperties.AnalyticsURL.Contains(BugSenseProperties._apiKey))
        return;
      if (!BugSenseProperties._proxyEnabled)
      {
        BugSenseProperties.AnalyticsURL = BugSenseProperties.AnalyticsURL + BugSenseProperties._apiKey + "/" + BugSenseProperties.UID;
      }
      else
      {
        BugSenseProperties.Url = "http://alt.bugsense.com/api/errors";
        BugSenseProperties.AnalyticsURL = "http://alt.bugsense.com/api/ticks/" + BugSenseProperties._apiKey + "/" + BugSenseProperties.UID;
      }
    }

    public static string CreateUrl(int errorsCount, int eventsCount)
    {
      string str = string.Format("https://{0}.bugsense.com/{1}/{2}/{3}/{4}", (object) BugSenseProperties.APIKey, (object) BugSenseProperties.AppVersion, (object) BugSenseProperties.APIKey, (object) errorsCount, (object) eventsCount);
      return BugSenseProperties.HasDebugUrl ? BugSenseProperties.DebugTestUrl : str;
    }
  }
}
