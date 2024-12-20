// Decompiled with JetBrains decompiler
// Type: SQLite.SQLiteConnectionString
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System.IO;
using Windows.Storage;

#nullable disable
namespace SQLite
{
  internal class SQLiteConnectionString
  {
    private static readonly string MetroStyleDataPath = ApplicationData.Current.LocalFolder.Path;

    public string ConnectionString { get; private set; }

    public string DatabasePath { get; private set; }

    public bool StoreDateTimeAsTicks { get; private set; }

    public SQLiteConnectionString(string databasePath, bool storeDateTimeAsTicks)
    {
      this.ConnectionString = databasePath;
      this.StoreDateTimeAsTicks = storeDateTimeAsTicks;
      this.DatabasePath = Path.Combine(SQLiteConnectionString.MetroStyleDataPath, databasePath);
    }
  }
}
