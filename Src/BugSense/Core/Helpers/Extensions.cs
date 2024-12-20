// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Helpers.Extensions
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using Newtonsoft.Json;
using Splunk.Mi.Utilities;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;


namespace BugSense.Core.Helpers
{
  public static class Extensions
  {
    private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static string SerializeToJson<T>(this T obj)
    {
      try
      {
        return JsonConvert.SerializeObject((object) obj);
      }
      catch (Exception ex)
      {
        ConsoleManager.LogToConsole(string.Format("Serialization FAILED: {0}", (object) ex));
      }
      return (string) null;
    }

    public static T DeserializeJson<T>(this string jsonData) where T : class
    {
      try
      {
        return JsonConvert.DeserializeObject<T>(jsonData);
      }
      catch (Exception ex)
      {
        ConsoleManager.LogToConsole(string.Format("Deserialization FAILED: {0}", (object) ex));
        return default (T);
      }
    }

    public static double DateTimeToUnixTimestamp(this DateTime dateTime)
    {
      return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
    }

    public static DateTime UnixTimeStampToDateTime(this double unixTimeStamp)
    {
      return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(unixTimeStamp).ToLocalTime();
    }

    public static long GetCurrentUnixTimestampMillis()
    {
      return (long) (DateTime.UtcNow - Extensions.UnixEpoch).TotalMilliseconds;
    }

    public static DateTime DateTimeFromUnixTimestampMillis(this long millis)
    {
      return Extensions.UnixEpoch.AddMilliseconds((double) millis);
    }

    public static long GetCurrentUnixTimestampSeconds()
    {
      return (long) (DateTime.UtcNow - Extensions.UnixEpoch).TotalSeconds;
    }

    public static DateTime DateTimeFromUnixTimestampSeconds(this long seconds)
    {
      return Extensions.UnixEpoch.AddSeconds((double) seconds);
    }

    public static async Task<byte[]> ToBytesZipAsync(this string value)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(value);
      byte[] array;
      using (MemoryStream msi = new MemoryStream(bytes))
      {
        using (MemoryStream mso = new MemoryStream())
        {
          using (GZipStream gs = new GZipStream((Stream) mso, CompressionMode.Compress))
            await msi.CopyToAsync((Stream) gs);
          array = mso.ToArray();
        }
      }
      return array;
    }

    public static async Task<string> ToStringUnzipAsync(this byte[] value)
    {
      string stringUnzipAsync;
      using (MemoryStream msi = new MemoryStream(value))
      {
        using (MemoryStream mso = new MemoryStream())
        {
          using (GZipStream gs = new GZipStream((Stream) msi, CompressionMode.Decompress))
            await gs.CopyToAsync((Stream) mso);
          byte[] bytes = mso.ToArray();
          stringUnzipAsync = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
      }
      return stringUnzipAsync;
    }
  }
}
