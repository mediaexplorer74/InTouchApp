// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.BugSenseResponseResult
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll


namespace BugSense.Core.Model
{
  public class BugSenseResponseResult : BugSenseResult
  {
    public long ErrorId { get; internal set; }

    public string ServerResponse { get; internal set; }

    public string Url { get; internal set; }

    public string ContentText { get; internal set; }

    public string TickerText { get; internal set; }

    public string ContentTitle { get; internal set; }

    public bool IsResolved
    {
      get
      {
        return !string.IsNullOrWhiteSpace(this.Url) && !string.IsNullOrWhiteSpace(this.ContentText) && !string.IsNullOrWhiteSpace(this.ContentTitle);
      }
    }
  }
}
