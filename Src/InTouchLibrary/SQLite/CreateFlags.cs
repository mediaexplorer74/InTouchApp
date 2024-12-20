// Decompiled with JetBrains decompiler
// Type: SQLite.CreateFlags
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System;

#nullable disable
namespace SQLite
{
  [Flags]
  public enum CreateFlags
  {
    None = 0,
    ImplicitPK = 1,
    ImplicitIndex = 2,
    AllImplicit = ImplicitIndex | ImplicitPK, // 0x00000003
    AutoIncPK = 4,
  }
}
