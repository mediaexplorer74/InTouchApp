// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.LimitedCrashExtraDataList
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace BugSense.Core.Model
{
  public class LimitedCrashExtraDataList : 
    IList<CrashExtraData>,
    ICollection<CrashExtraData>,
    IEnumerable<CrashExtraData>,
    IEnumerable
  {
    private const int _maxCount = 32;
    private readonly List<CrashExtraData> _crashExtraDataList = new List<CrashExtraData>();

    public static int MaxCount => 32;

    public void Add(CrashExtraData item)
    {
      if (this._crashExtraDataList.Count >= 32 && this._crashExtraDataList.Count > 0)
        this._crashExtraDataList.RemoveAt(0);
      this._crashExtraDataList.Add(item);
    }

    public void Remove(CrashExtraData item)
    {
      if (!this._crashExtraDataList.Contains(item))
        return;
      this._crashExtraDataList.Remove(item);
    }

    public void Add(string key, string value)
    {
      if (this._crashExtraDataList.Count >= 32)
        this._crashExtraDataList.RemoveAt(0);
      this._crashExtraDataList.Add(new CrashExtraData(key, value));
    }

    public void Remove(string key)
    {
      CrashExtraData crashExtraData = this._crashExtraDataList.FirstOrDefault<CrashExtraData>((Func<CrashExtraData, bool>) (p => p.Key.Equals(key)));
      if (crashExtraData == null)
        return;
      this._crashExtraDataList.Remove(crashExtraData);
    }

    public int IndexOf(CrashExtraData item)
    {
      return this._crashExtraDataList.Contains(item) ? this._crashExtraDataList.IndexOf(item) : -1;
    }

    public void Insert(int index, CrashExtraData item)
    {
      this._crashExtraDataList.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
      if (index > this._crashExtraDataList.Count)
        return;
      this._crashExtraDataList.RemoveAt(index);
    }

    public CrashExtraData this[int index]
    {
      get => this._crashExtraDataList[index];
      set
      {
        if (index > this._crashExtraDataList.Count)
          return;
        this._crashExtraDataList[index] = value;
      }
    }

    public void Clear() => this._crashExtraDataList.Clear();

    public bool Contains(CrashExtraData item) => this._crashExtraDataList.Contains(item);

    public void CopyTo(CrashExtraData[] array, int arrayIndex)
    {
      this._crashExtraDataList.CopyTo(array, arrayIndex);
    }

    bool ICollection<CrashExtraData>.Remove(CrashExtraData item)
    {
      return this._crashExtraDataList.Remove(item);
    }

    public int Count => this._crashExtraDataList.Count;

    public bool IsReadOnly => false;

    public IEnumerator<CrashExtraData> GetEnumerator()
    {
      return (IEnumerator<CrashExtraData>) this._crashExtraDataList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this._crashExtraDataList.GetEnumerator();
    }
  }
}
