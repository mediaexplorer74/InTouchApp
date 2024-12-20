// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.BugSenseUnhandledHandlerEventArgs
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;


namespace BugSense.Core.Model
{
  public class BugSenseUnhandledHandlerEventArgs : EventArgs
  {
    public string ClientJsonRequest { get; set; }

    public Exception ExceptionObject { get; internal set; }

    public bool HandledSuccessfully { get; internal set; }
  }
}
