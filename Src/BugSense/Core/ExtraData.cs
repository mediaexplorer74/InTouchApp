// Decompiled with JetBrains decompiler
// Type: BugSense.Core.ExtraData
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Model;
using System.Collections.Generic;


namespace BugSense.Core
{
  internal static class ExtraData
  {
    public static LimitedCrashExtraDataList CrashExtraData { get; set; }

    public static List<BugSense.Core.Model.CrashExtraMap> CrashExtraMap { get; set; }

    public static LimitedBreadCrumbList BreadCrumbs { get; set; }

    static ExtraData()
    {
      ExtraData.CrashExtraData = new LimitedCrashExtraDataList();
      ExtraData.CrashExtraMap = new List<BugSense.Core.Model.CrashExtraMap>();
      ExtraData.BreadCrumbs = new LimitedBreadCrumbList();
    }
  }
}
