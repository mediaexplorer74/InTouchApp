// Decompiled with JetBrains decompiler
// Type: Splunk.Mi.Utilities.CommonHelper
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core;
using System;


namespace Splunk.Mi.Utilities
{
  internal static class CommonHelper
  {
    public static string NewFileNamePath(FileNameType fileType)
    {
      string str = string.Empty;
      switch (fileType)
      {
        case FileNameType.UnhandledException:
          str = string.Format("{0}\\CCC_{1}_BugSense_Ex_{2}.dat", (object) BugSenseProperties.ExceptionsFolderName, (object) DateTime.UtcNow.ToString("yyyyMMddHHmmss"), (object) Guid.NewGuid());
          break;
        case FileNameType.LoggedException:
          str = string.Format("{0}\\LLC_{1}_BugSense_Ex_{2}.dat", (object) BugSenseProperties.ExceptionsFolderName, (object) DateTime.UtcNow.ToString("yyyyMMddHHmmss"), (object) Guid.NewGuid());
          break;
        case FileNameType.Ping:
          str = string.Format("{0}\\PCC_{1}_BugSense_Ev_{2}.dat", (object) BugSenseProperties.ExceptionsFolderName, (object) DateTime.UtcNow.ToString("yyyyMMddHHmmss"), (object) Guid.NewGuid());
          break;
        case FileNameType.Gnip:
          str = string.Format("{0}\\GCC_{1}_BugSense_Ev_{2}.dat", (object) BugSenseProperties.ExceptionsFolderName, (object) DateTime.UtcNow.ToString("yyyyMMddHHmmss"), (object) Guid.NewGuid());
          break;
        case FileNameType.Event:
          str = string.Format("{0}\\ECC_{1}_BugSense_Ev_{2}.dat", (object) BugSenseProperties.ExceptionsFolderName, (object) DateTime.UtcNow.ToString("yyyyMMddHHmmss"), (object) Guid.NewGuid());
          break;
        case FileNameType.TransactionStart:
          str = string.Format("{0}\\TRSTART_{1}_BugSense_Tr_{2}.dat", (object) BugSenseProperties.ExceptionsFolderName, (object) DateTime.UtcNow.ToString("yyyyMMddHHmmss"), (object) Guid.NewGuid());
          break;
        case FileNameType.TransactionStop:
          str = string.Format("{0}\\TRSTOP_{1}_BugSense_Tr_{2}.dat", (object) BugSenseProperties.ExceptionsFolderName, (object) DateTime.UtcNow.ToString("yyyyMMddHHmmss"), (object) Guid.NewGuid());
          break;
        case FileNameType.NetworkAction:
          str = string.Format("{0}\\NETWORK_{1}_BugSense_Net_{2}.dat", (object) BugSenseProperties.ExceptionsFolderName, (object) DateTime.UtcNow.ToString("yyyyMMddHHmmss"), (object) Guid.NewGuid());
          break;
      }
      return str;
    }
  }
}
