// Decompiled with JetBrains decompiler
// Type: BugSense.Device.Specific.DeviceUtil
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core;
using BugSense.Core.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Graphics.Display;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System;
using Windows.System.UserProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace BugSense.Device.Specific
{
  internal class DeviceUtil : IDeviceUtil
  {
    public void AppendBugSenseInfo()
    {
      BugSenseProperties.AppName = Package.Current.Id.Name;
      PackageVersion version = Package.Current.Id.Version;
      BugSenseProperties.AppVersion = string.Format("{0}.{1}.{2}.{3}", (object) version.Major, (object) version.Minor, (object) version.Build, (object) version.Revision);
      EasClientDeviceInformation deviceInformation = new EasClientDeviceInformation();
      BugSenseProperties.OSVersion = deviceInformation.OperatingSystem;
      BugSenseProperties.PhoneBrand = deviceInformation.SystemManufacturer;
      BugSenseProperties.PhoneModel = deviceInformation.FriendlyName;
      BugSenseProperties.BugSenseName = "bugsense-wp8";
      BugSenseProperties.Locale = CultureInfo.CurrentCulture.EnglishName;
      BugSenseProperties.Carrier = "unknown";
      BugSenseProperties.MobileNetOn = 2;
      try
      {
        BugSenseProperties.DeviceScreenProperties.Xdpi = DisplayProperties.LogicalDpi;
        BugSenseProperties.DeviceScreenProperties.Ydpi = DisplayProperties.LogicalDpi;
      }
      catch
      {
      }
    }

    public void GetDeviceConnectionInfo()
    {
    }

    public void GetScreenInfo()
    {
      try
      {
        if (Window.Current.Content is Frame content)
        {
          BugSenseProperties.DeviceScreenProperties.Width = ((FrameworkElement) content).ActualHeight;
          BugSenseProperties.DeviceScreenProperties.Height = ((FrameworkElement) content).ActualWidth;
        }
      }
      catch
      {
      }
      try
      {
        BugSenseProperties.Orientation = DisplayProperties.CurrentOrientation.ToString();
      }
      catch
      {
      }
    }

    public AppEnvironment GetAppEnvironment()
    {
      AppEnvironment environment = new AppEnvironment()
      {
        AppName = BugSenseProperties.AppName,
        AppVersion = BugSenseProperties.AppVersion,
        OsVersion = BugSenseProperties.OSVersion,
        CpuModel = "unknown",
        CpuBitness = 64,
        PhoneManufacturer = BugSenseProperties.PhoneBrand,
        PhoneModel = BugSenseProperties.PhoneModel,
        ScreenDpi = "unavailable",
        Uid = BugSenseProperties.UID,
        GeoRegion = GlobalizationPreferences.HomeGeographicRegion,
        Locale = BugSenseProperties.Locale,
        CellularData = BugSenseProperties.MobileNetOn.ToString(),
        Carrier = BugSenseProperties.Carrier,
        Rooted = false
      };
      Task.Run<string>((Func<Task<string>>) (async () => environment.CpuModel = await DeviceUtil.GetCpu().ConfigureAwait(false))).Wait();
      this.GetDeviceConnectionInfo();
      this.GetScreenInfo();
      return environment;
    }

    public BugSensePerformance GetBugSensePerformance()
    {
      double megabytes1 = DeviceUtil.ConvertBytesToMegabytes((long) MemoryManager.AppMemoryUsageLimit);
      double megabytes2 = DeviceUtil.ConvertBytesToMegabytes((long) MemoryManager.AppMemoryUsage);
      double num = megabytes1 - megabytes2;
      return new BugSensePerformance()
      {
        AppMemMax = megabytes1,
        AppMemTotal = megabytes2,
        AppMemAvail = num,
        SysMemAvail = 0.0,
        SysMemLow = this.IsLowMemDevice.ToString(),
        SysMemThreshold = num
      };
    }

    private static double ConvertBytesToMegabytes(long bytes) => (double) bytes / 1024.0 / 1024.0;

    public bool IsLowMemDevice => false;

    private static async Task<string> GetCpu()
    {
      string result = "unknown";
      try
      {
        DeviceInformationCollection ifaces = await DeviceInformation.FindAllAsync("System.Devices.InterfaceClassGuid:=\"{97FADB10-4E33-40AE-359C-8BEF029DBDD0}\"", (IEnumerable<string>) null);
        if (ifaces != null)
          result = ((IReadOnlyList<DeviceInformation>) ifaces)[0].Name;
      }
      catch
      {
      }
      return result;
    }
  }
}
