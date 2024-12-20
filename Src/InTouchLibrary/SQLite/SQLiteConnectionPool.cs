﻿// Decompiled with JetBrains decompiler
// Type: SQLite.SQLiteConnectionPool
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System.Collections.Generic;

#nullable disable
namespace SQLite
{
  internal class SQLiteConnectionPool
  {
    private readonly Dictionary<string, SQLiteConnectionPool.Entry> _entries = new Dictionary<string, SQLiteConnectionPool.Entry>();
    private readonly object _entriesLock = new object();
    private static readonly SQLiteConnectionPool _shared = new SQLiteConnectionPool();

    public static SQLiteConnectionPool Shared => SQLiteConnectionPool._shared;

    public SQLiteConnectionWithLock GetConnection(
      SQLiteConnectionString connectionString,
      SQLiteOpenFlags openFlags)
    {
      lock (this._entriesLock)
      {
        string connectionString1 = connectionString.ConnectionString;
        SQLiteConnectionPool.Entry entry;
        if (!this._entries.TryGetValue(connectionString1, out entry))
        {
          entry = new SQLiteConnectionPool.Entry(connectionString, openFlags);
          this._entries[connectionString1] = entry;
        }
        return entry.Connection;
      }
    }

    public void Reset()
    {
      lock (this._entriesLock)
      {
        foreach (SQLiteConnectionPool.Entry entry in this._entries.Values)
          entry.OnApplicationSuspended();
        this._entries.Clear();
      }
    }

    public void ApplicationSuspended() => this.Reset();

    private class Entry
    {
      public SQLiteConnectionString ConnectionString { get; private set; }

      public SQLiteConnectionWithLock Connection { get; private set; }

      public Entry(SQLiteConnectionString connectionString, SQLiteOpenFlags openFlags)
      {
        this.ConnectionString = connectionString;
        this.Connection = new SQLiteConnectionWithLock(connectionString, openFlags);
      }

      public void OnApplicationSuspended()
      {
        this.Connection.Dispose();
        this.Connection = (SQLiteConnectionWithLock) null;
      }
    }
  }
}