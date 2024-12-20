// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.LogFile
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace InTouchLibrary
{
  public static class LogFile
  {
    private static List<string> log_messages = new List<string>(500);
    private static int log_counter = 0;
    private static int max_log_count = 499;

    static LogFile()
    {
      try
      {
        for (int index = 0; index <= LogFile.max_log_count; ++index)
          LogFile.log_messages.Add((string) null);
        LogFile.Log("Start Rolling", EventType.Information);
      }
      catch
      {
      }
    }

    public static void Close()
    {
      try
      {
        LogFile.Log("End Rolling", EventType.Information);
        LogFile.log_messages.Clear();
      }
      catch
      {
      }
    }

    public static void ClearLog() => LogFile.log_messages.Clear();

    private static string GetLogLine(LogEntry entry)
    {
      return string.Format("[{0} | {1}]\t{2}\r", (object) entry.date, (object) entry.type, (object) entry.msg);
    }

    public static void Log(string message, EventType eventType)
    {
      try
      {
        LogEntry entry = new LogEntry(DateTime.Now, eventType, message);
        LogFile.log_messages[LogFile.log_counter] = LogFile.GetLogLine(entry);
        string logMessage = LogFile.log_messages[LogFile.log_counter];
        if (LogFile.log_counter != LogFile.max_log_count)
          ++LogFile.log_counter;
        else
          LogFile.log_counter = 0;
      }
      catch
      {
      }
    }

    public static string GetLogForFeedback()
    {
      try
      {
        StringBuilder stringBuilder = new StringBuilder();
        if (LogFile.log_messages.Count != 0)
        {
          if (!string.IsNullOrEmpty(LogFile.log_messages[LogFile.log_counter]))
          {
            for (int logCounter = LogFile.log_counter; logCounter <= LogFile.max_log_count; ++logCounter)
              stringBuilder.AppendLine(LogFile.log_messages[logCounter]);
          }
          for (int index = 0; index < LogFile.log_counter; ++index)
            stringBuilder.AppendLine(LogFile.log_messages[index]);
        }
        return stringBuilder.ToString();
      }
      catch
      {
        return (string) null;
      }
    }
  }
}
