// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.LogEntry
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System;

#nullable disable
namespace InTouchLibrary
{
  public struct LogEntry
  {
    public DateTime date;
    public EventType type;
    public string msg;

    public LogEntry(DateTime _date, EventType _type, string _msg)
    {
      this.date = _date;
      this.type = _type;
      this.msg = _msg;
    }
  }
}
