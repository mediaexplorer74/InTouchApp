// Decompiled with JetBrains decompiler
// Type: BugSense.Helpers.Digest
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll


namespace BugSense.Helpers
{
  internal sealed class Digest
  {
    public uint A;
    public uint B;
    public uint C;
    public uint D;

    public Digest()
    {
      this.A = 1732584193U;
      this.B = 4023233417U;
      this.C = 2562383102U;
      this.D = 271733878U;
    }

    public override string ToString()
    {
      return MD5Helper.ReverseByte(this.A).ToString("X8") + MD5Helper.ReverseByte(this.B).ToString("X8") + MD5Helper.ReverseByte(this.C).ToString("X8") + MD5Helper.ReverseByte(this.D).ToString("X8");
    }
  }
}
