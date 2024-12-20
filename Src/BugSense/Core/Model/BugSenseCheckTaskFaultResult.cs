// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.BugSenseCheckTaskFaultResult
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;
using System.Threading.Tasks;


namespace BugSense.Core.Model
{
  public sealed class BugSenseCheckTaskFaultResult
  {
    public string Description { get; set; }

    public Exception TaskException { get; set; }

    public string Id { get; set; }

    public Task TaskToCheck { get; set; }

    public LimitedCrashExtraDataList ExtraData { get; set; }

    internal int SecondsToWait { get; set; }

    internal int Retries { get; set; }

    internal BugSenseCheckTaskFaultResult()
    {
    }
  }
}
