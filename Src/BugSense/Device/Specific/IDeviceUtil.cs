// Decompiled with JetBrains decompiler
// Type: BugSense.Device.Specific.IDeviceUtil
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Model;


namespace BugSense.Device.Specific
{
  internal interface IDeviceUtil
  {
    void AppendBugSenseInfo();

    void GetDeviceConnectionInfo();

    void GetScreenInfo();

    AppEnvironment GetAppEnvironment();

    BugSensePerformance GetBugSensePerformance();

    bool IsLowMemDevice { get; }
  }
}
