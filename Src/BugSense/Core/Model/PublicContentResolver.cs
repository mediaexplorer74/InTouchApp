// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.PublicContentResolver
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Interfaces;


namespace BugSense.Core.Model
{
  internal class PublicContentResolver : IContentResolver
  {
    public string EventContentType() => "text/plain";

    public string ErrorContentType() => "application/x-www-form-urlencoded";
  }
}
