// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Helpers.EntropyUUID
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;
using System.Threading;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;


namespace BugSense.Core.Helpers
{
  internal class EntropyUUID
  {
    public static string Get()
    {
      FileRepository fileRepository = new FileRepository();
      string jsonRequest = fileRepository.Read(BugSenseProperties.GeneralFolderName + "\\bugsense.udid");
      if (string.IsNullOrEmpty(jsonRequest))
      {
        jsonRequest = EntropyUUID.GetNew();
        fileRepository.Save(BugSenseProperties.GeneralFolderName + "\\bugsense.udid", jsonRequest);
      }
      return jsonRequest;
    }

    private static string GetNew()
    {
      string str1 = DateTime.Now.Millisecond.ToString();
      string str2 = new object().GetHashCode().ToString();
      DateTime now = DateTime.Now;
      new ManualResetEvent(false).WaitOne(256);
      string str3 = (DateTime.Now.Ticks - now.Ticks).ToString();
      string str4 = (new Random().Next() % 65536).ToString();
      string str5 = (DateTime.Now.Ticks % 10L).ToString();
      return EntropyUUID.GetSHA1Hash(str1 + str2 + str3 + str4 + str5);
    }

    private static string GetSHA1Hash(string input)
    {
      IBuffer binary = CryptographicBuffer.ConvertStringToBinary(input, (BinaryStringEncoding) 0);
      HashAlgorithmProvider algorithmProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
      string algorithmName = algorithmProvider.AlgorithmName;
      return CryptographicBuffer.EncodeToHexString(algorithmProvider.HashData(binary)).ToLowerInvariant();
    }
  }
}
