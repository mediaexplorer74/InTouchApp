// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Interfaces.IRequestJsonSerializer
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Model;
using System;


namespace BugSense.Core.Interfaces
{
  public interface IRequestJsonSerializer
  {
    SerializeResult SerializeEventToJson(BugSenseEventTag eventTag, AppEnvironment appEnvironment);

    SerializeResult SerializeEventToJson(string eventTag, AppEnvironment appEnvironment);

    SerializeResult SerializeCrashToJson(
      Exception exception,
      AppEnvironment appEnvironment,
      BugSensePerformance bugSensePerformance,
      bool handled,
      LimitedCrashExtraDataList extraData);

    string DecodeEncodedCrashJson(string encodedJson);

    string GetErrorHash(string jsonRequest);

    SerializeResult SerializeTransaction<T>(T transaction);
  }
}
