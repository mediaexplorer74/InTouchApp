// Decompiled with JetBrains decompiler
// Type: BugSense.Helpers.MD5Helper
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll


namespace BugSense.Helpers
{
  internal sealed class MD5Helper
  {
    private MD5Helper()
    {
    }

    public static uint RotateLeft(uint uiNumber, ushort shift)
    {
      return uiNumber >> 32 - (int) shift | uiNumber << (int) shift;
    }

    public static uint ReverseByte(uint uiNumber)
    {
      return (uint) (((int) uiNumber & (int) byte.MaxValue) << 24 | (int) (uiNumber >> 24) | (int) ((uiNumber & 16711680U) >> 8) | ((int) uiNumber & 65280) << 8);
    }
  }
}
