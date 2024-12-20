// Decompiled with JetBrains decompiler
// Type: SQLite.SQLiteConnectionWithLock
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System;
using System.Threading;

#nullable disable
namespace SQLite
{
  internal class SQLiteConnectionWithLock : SQLiteConnection
  {
    private readonly object _lockPoint = new object();

    public SQLiteConnectionWithLock(
      SQLiteConnectionString connectionString,
      SQLiteOpenFlags openFlags)
      : base(connectionString.DatabasePath, openFlags, connectionString.StoreDateTimeAsTicks)
    {
    }

    public IDisposable Lock()
    {
      return (IDisposable) new SQLiteConnectionWithLock.LockWrapper(this._lockPoint);
    }

    private class LockWrapper : IDisposable
    {
      private object _lockPoint;

      public LockWrapper(object lockPoint)
      {
        this._lockPoint = lockPoint;
        Monitor.Enter(this._lockPoint);
      }

      public void Dispose() => Monitor.Exit(this._lockPoint);
    }
  }
}
