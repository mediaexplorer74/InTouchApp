// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.BugSenseMessageException
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;


namespace BugSense.Core.Model
{
  public class BugSenseMessageException : Exception
  {
    private string BugSenseStackTrace { get; set; }

    private string BugSenseMessage { get; set; }

    public BugSenseMessageException(string message, string stacktrace)
    {
      if (string.IsNullOrWhiteSpace(message))
        throw new ArgumentNullException(nameof (message), "Parameter cannot be null.");
      this.BugSenseStackTrace = !string.IsNullOrWhiteSpace(stacktrace) ? stacktrace : throw new ArgumentNullException(nameof (stacktrace), "Parameter cannot be null.");
      this.BugSenseMessage = message;
    }

    public override string Message => this.BugSenseMessage;

    public override string StackTrace => this.BugSenseStackTrace;
  }
}
