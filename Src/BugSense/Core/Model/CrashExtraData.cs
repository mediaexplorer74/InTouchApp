// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.CrashExtraData
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;


namespace BugSense.Core.Model
{
  public class CrashExtraData : IEquatable<CrashExtraData>
  {
    private const int _maxLength = 128;
    private string _value;

    private Guid Id { get; set; }

    public static int MaxLength => 128;

    public string Key { get; set; }

    public string Value
    {
      get => this._value;
      set
      {
        if (string.IsNullOrWhiteSpace(value))
          return;
        string str = value.Trim();
        if (value.Length <= CrashExtraData.MaxLength)
        {
          this._value = str;
        }
        else
        {
          int length = Math.Min(CrashExtraData.MaxLength, str.Length);
          this._value = str.Substring(0, length);
        }
      }
    }

    public CrashExtraData() => this.Id = Guid.NewGuid();

    public CrashExtraData(string key, string value)
      : this()
    {
      this.Key = key;
      this.Value = value;
    }

    public override string ToString()
    {
      return string.Format("[CrashExtraData: Key={0}, Value={1}]", (object) this.Key, (object) this.Value);
    }

    public override int GetHashCode() => this.Id.GetHashCode();

    public override bool Equals(object obj) => this.Id.Equals(((CrashExtraData) obj).Id);

    public bool Equals(CrashExtraData other) => this.Id.Equals(other.Id);
  }
}
