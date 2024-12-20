// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.PublicRequestJsonSerializer
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Helpers;
using BugSense.Core.Interfaces;
using Splunk.Mi.Utilities;
using System;


namespace BugSense.Core.Model
{
  internal class PublicRequestJsonSerializer : IRequestJsonSerializer
  {
    public SerializeResult SerializeEventToJson(
      BugSenseEventTag eventTag,
      AppEnvironment appEnvironment)
    {
      SerializeResult json = new SerializeResult();
      string bugSenseEvent = BugSenseEventFactory.CreateBugSenseEvent(eventTag);
      json.DecodedJson = bugSenseEvent;
      json.EncodedJson = bugSenseEvent;
      return json;
    }

    public SerializeResult SerializeEventToJson(string eventTag, AppEnvironment appEnvironment)
    {
      SerializeResult json = new SerializeResult();
      string bugSenseEvent = BugSenseEventFactory.CreateBugSenseEvent(eventTag);
      json.DecodedJson = bugSenseEvent;
      json.EncodedJson = bugSenseEvent;
      return json;
    }

    public SerializeResult SerializeCrashToJson(
      Exception exception,
      AppEnvironment appEnvironment,
      BugSensePerformance bugSensePerformance,
      bool handled,
      LimitedCrashExtraDataList extraData)
    {
      SerializeResult json1 = new SerializeResult();
      string json2 = new BugSenseExceptionRequest(BugSenseException.GetInstance(exception, handled), appEnvironment, bugSensePerformance, extraData).SerializeToJson<BugSenseExceptionRequest>();
      json1.DecodedJson = json2;
      try
      {
        string str = "data=" + Uri.EscapeDataString(json2);
        json1.EncodedJson = str;
      }
      catch (FormatException ex)
      {
        ConsoleManager.LogToConsole(string.Format("Uri FormatException: {0}", (object) ex));
      }
      catch (ArgumentNullException ex)
      {
        ConsoleManager.LogToConsole(string.Format("Uri ArgumentNullException: {0}", (object) ex));
      }
      return json1;
    }

    public string DecodeEncodedCrashJson(string encodedJson)
    {
      int num = encodedJson.IndexOf("=", StringComparison.Ordinal);
      return num > -1 ? Uri.UnescapeDataString(encodedJson.Substring(num + 1)) : encodedJson;
    }

    public string GetErrorHash(string jsonRequest)
    {
      string errorHash = string.Empty;
      try
      {
        errorHash = jsonRequest.DeserializeJson<BugSenseExceptionRequest>().Exception.ErrorHash;
      }
      catch (Exception ex)
      {
        ConsoleManager.LogToConsole(string.Format("Get ErrorHash Exception: {0}", (object) ex));
      }
      return errorHash;
    }

    public SerializeResult SerializeTransaction<T>(T transaction)
    {
      SerializeResult serializeResult = new SerializeResult()
      {
        DecodedJson = transaction.SerializeToJson<T>()
      };
      serializeResult.EncodedJson = string.Format("data={0}", (object) Uri.EscapeDataString(serializeResult.DecodedJson));
      return serializeResult;
    }
  }
}
