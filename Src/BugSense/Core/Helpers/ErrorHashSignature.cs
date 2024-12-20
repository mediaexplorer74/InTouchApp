// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Helpers.ErrorHashSignature
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Interfaces;
using BugSense.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;


namespace BugSense.Core.Helpers
{
  public class ErrorHashSignature : IHashSignature
  {
    public HashSignature GetHashSignature(
      string appName,
      string appVersion,
      string stacktrace,
      string message,
      string hresult)
    {
      HashSignature hashSignature = new HashSignature();
      if (!string.IsNullOrWhiteSpace(stacktrace))
      {
        string[] source = stacktrace.Split(new string[1]
        {
          "\r\n"
        }, StringSplitOptions.RemoveEmptyEntries);
        if (((IEnumerable<string>) source).Any<string>())
        {
          try
          {
            string str1 = ((IEnumerable<string>) source).FirstOrDefault<string>((Func<string, bool>) (p => p.Contains(appName)));
            string offendingLine = string.IsNullOrWhiteSpace(str1) ? source[0].Trim() : str1.Trim();
            int num1 = offendingLine.IndexOf("in", StringComparison.OrdinalIgnoreCase);
            if (num1 > -1)
            {
              string str2 = offendingLine.Substring(3, num1 - 3);
              hashSignature.Where = str2.Trim();
            }
            else
            {
              int num2 = offendingLine.LastIndexOf('.');
              hashSignature.Where = offendingLine.Substring(num2 - 2).Trim();
            }
            foreach (string str3 in ((IEnumerable<string>) source).Where<string>((Func<string, bool>) (p => p.Trim().StartsWith("at"))))
            {
              string str4 = str3.Trim();
              int num3 = str4.IndexOf('.') - 1;
              if (num3 > -1)
              {
                string str5 = str4.Substring(3, num3 - 2);
                if (!hashSignature.Tags.Contains(str5))
                  hashSignature.Tags.Add(str5);
              }
            }
            hashSignature.Signature = this.ComputeSignature(appVersion, offendingLine, hresult);
          }
          catch
          {
          }
        }
      }
      return hashSignature;
    }

    private string ComputeSignature(string appVersion, string offendingLine, string hresult)
    {
      return ErrorHashSignature.GetMd5Hash((appVersion.Trim() + offendingLine.Trim() + hresult.Trim()).Trim());
    }

    private static string GetMd5Hash(string value)
    {
      return CryptographicBuffer.EncodeToHexString(HashAlgorithmProvider.OpenAlgorithm("MD5").HashData(CryptographicBuffer.ConvertStringToBinary(value, (BinaryStringEncoding) 0)));
    }
  }
}
