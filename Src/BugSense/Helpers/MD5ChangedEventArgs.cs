// Decompiled with JetBrains decompiler
// Type: BugSense.Helpers.MD5ChangedEventArgs
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;


namespace BugSense.Helpers
{
  internal class MD5ChangedEventArgs : EventArgs
  {
    public readonly byte[] NewData;
    public readonly string FingerPrint;

    public MD5ChangedEventArgs(byte[] data, string HashedValue)
    {
      byte[] numArray = new byte[data.Length];
      for (int index = 0; index < data.Length; ++index)
        numArray[index] = data[index];
      this.FingerPrint = HashedValue;
    }

    public MD5ChangedEventArgs(string data, string HashedValue)
    {
      byte[] numArray = new byte[data.Length];
      for (int index = 0; index < data.Length; ++index)
        numArray[index] = (byte) data[index];
      this.FingerPrint = HashedValue;
    }
  }
}
