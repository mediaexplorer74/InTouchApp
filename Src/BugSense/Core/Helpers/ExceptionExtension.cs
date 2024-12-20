// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Helpers.ExceptionExtension
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;


namespace BugSense.Core.Helpers
{
  public static class ExceptionExtension
  {
    public static string HResultEx(this Exception exception) => exception.HResult.ToString();
  }
}
