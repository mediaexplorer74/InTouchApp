// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.StacktraceHelper
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;
using System.Text;


namespace BugSense.Core.Model
{
  public static class StacktraceHelper
  {
    public static string GetStackTrace(Exception ex)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(!string.IsNullOrWhiteSpace(ex.ToString()) ? ex.ToString() : "not available");
      for (Exception innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
      {
        string fullName = innerException.GetType().FullName;
        stringBuilder.AppendLine(string.Format("--- Inner exception of type {0} start ---", (object) fullName));
        stringBuilder.AppendLine(string.Format("--- Message: {0} ---", (object) innerException.Message.Replace(Environment.NewLine, " ")));
        stringBuilder.AppendLine(!string.IsNullOrWhiteSpace(innerException.ToString()) ? innerException.ToString() : "not available");
        stringBuilder.AppendLine("--- End of inner exception stack trace ---");
      }
      return stringBuilder.ToString();
    }
  }
}
