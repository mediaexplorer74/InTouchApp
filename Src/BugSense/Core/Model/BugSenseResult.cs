// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.BugSenseResult
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;


namespace BugSense.Core.Model
{
  public abstract class BugSenseResult
  {
    public BugSenseRequestType RequestType { get; internal set; }

    public string Description { get; internal set; }

    public BugSenseResultState ResultState { get; internal set; }

    public Exception ExceptionError { get; internal set; }

    public string ClientRequest { get; internal set; }

    public bool HandledWhileDebugging { get; internal set; }

    internal BugSenseResult()
    {
      this.ResultState = BugSenseResultState.Undefined;
      this.HandledWhileDebugging = true;
    }

    internal void SetHandleWhileDebuggingExceptionError()
    {
      this.ExceptionError = (Exception) new BugSenseMessageException("HandleWhileDebugging is False", "You should see that the ResultState is OK, this is a normal result due to HandleWhileDebugging is equal to false.");
      this.HandledWhileDebugging = false;
    }
  }
}
