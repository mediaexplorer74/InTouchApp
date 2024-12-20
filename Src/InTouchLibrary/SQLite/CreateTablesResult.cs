// Decompiled with JetBrains decompiler
// Type: SQLite.CreateTablesResult
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace SQLite
{
  public class CreateTablesResult
  {
    public Dictionary<Type, int> Results { get; private set; }

    internal CreateTablesResult() => this.Results = new Dictionary<Type, int>();
  }
}
