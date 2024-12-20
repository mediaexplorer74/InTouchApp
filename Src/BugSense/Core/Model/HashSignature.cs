// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.HashSignature
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System.Collections.Generic;


namespace BugSense.Core.Model
{
  public class HashSignature
  {
    public string Signature { get; set; }

    public string Where { get; set; }

    public List<string> Tags { get; set; }

    public HashSignature()
    {
      this.Signature = "Unknown";
      this.Tags = new List<string>();
      this.Where = "Unknown";
    }
  }
}
