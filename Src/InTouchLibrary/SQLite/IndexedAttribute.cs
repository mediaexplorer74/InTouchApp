// Decompiled with JetBrains decompiler
// Type: SQLite.IndexedAttribute
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

using System;

#nullable disable
namespace SQLite
{
  [AttributeUsage(AttributeTargets.Property)]
  public class IndexedAttribute : Attribute
  {
    public string Name { get; set; }

    public int Order { get; set; }

    public virtual bool Unique { get; set; }

    public IndexedAttribute()
    {
    }

    public IndexedAttribute(string name, int order)
    {
      this.Name = name;
      this.Order = order;
    }
  }
}
