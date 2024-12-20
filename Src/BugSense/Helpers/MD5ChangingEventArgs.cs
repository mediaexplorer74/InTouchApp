// Decompiled with JetBrains decompiler
// Type: BugSense.Helpers.MD5ChangingEventArgs
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;


namespace BugSense.Helpers
{
  internal class MD5ChangingEventArgs : EventArgs
  {
    public readonly byte[] NewData;

    public MD5ChangingEventArgs(byte[] data)
    {
      byte[] numArray = new byte[data.Length];
      for (int index = 0; index < data.Length; ++index)
        numArray[index] = data[index];
    }

    public MD5ChangingEventArgs(string data)
    {
      byte[] numArray = new byte[data.Length];
      for (int index = 0; index < data.Length; ++index)
        numArray[index] = (byte) data[index];
    }
  }
}
