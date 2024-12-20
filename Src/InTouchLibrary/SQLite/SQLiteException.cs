// Decompiled with JetBrains decompiler
// Type: SQLite.SQLiteException
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System;

#nullable disable
namespace SQLite
{
  public class SQLiteException : Exception
  {
    public SQLite3.Result Result { get; private set; }

    protected SQLiteException(SQLite3.Result r, string message)
      : base(message)
    {
      this.Result = r;
    }

    public static SQLiteException New(SQLite3.Result r, string message)
    {
      return new SQLiteException(r, message);
    }
  }
}
