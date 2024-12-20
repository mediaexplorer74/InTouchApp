// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Interfaces.IHashSignature
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Model;


namespace BugSense.Core.Interfaces
{
  public interface IHashSignature
  {
    HashSignature GetHashSignature(
      string appName,
      string appVersion,
      string stacktrace,
      string message,
      string hresult);
  }
}
