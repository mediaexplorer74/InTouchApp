// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.LimitedBreadCrumbList
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace BugSense.Core.Model
{
  public class LimitedBreadCrumbList : 
    IList<string>,
    ICollection<string>,
    IEnumerable<string>,
    IEnumerable
  {
    private const int _maxCount = 16;

    public static int MaxCount => 16;

    private List<string> BreadCrumbList { get; set; }

    public LimitedBreadCrumbList() => this.BreadCrumbList = new List<string>();

    public void Add(string item)
    {
      if (this.BreadCrumbList.Count >= 16 && this.BreadCrumbList.Count > 0)
        this.BreadCrumbList.RemoveAt(0);
      this.BreadCrumbList.Add(item);
    }

    public void Remove(string item)
    {
      if (!this.BreadCrumbList.Contains(item))
        return;
      this.BreadCrumbList.Remove(item);
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      int num = 1;
      foreach (string breadCrumb in this.BreadCrumbList)
      {
        string str = breadCrumb.Replace("\\|", "_");
        if (str.ToCharArray()[0] == '_')
          str = str.Replace("_", "-");
        stringBuilder.Append(str);
        if (num < this.BreadCrumbList.Count)
          stringBuilder.Append("|");
        ++num;
      }
      return stringBuilder.ToString();
    }

    public string[] ToArrayString()
    {
      List<string> stringList = new List<string>();
      foreach (string breadCrumb in this.BreadCrumbList)
        stringList.Add(breadCrumb);
      return stringList.ToArray();
    }

    public int IndexOf(string item)
    {
      return this.BreadCrumbList.Contains(item) ? this.BreadCrumbList.IndexOf(item) : -1;
    }

    public void Insert(int index, string item) => this.BreadCrumbList.Insert(index, item);

    public void RemoveAt(int index)
    {
      if (index > this.BreadCrumbList.Count)
        return;
      this.BreadCrumbList.RemoveAt(index);
    }

    public string this[int index]
    {
      get => this.BreadCrumbList[index];
      set
      {
        if (index > this.BreadCrumbList.Count)
          return;
        this.BreadCrumbList[index] = value;
      }
    }

    public void Clear() => this.BreadCrumbList.Clear();

    public bool Contains(string item) => this.BreadCrumbList.Contains(item);

    public void CopyTo(string[] array, int arrayIndex)
    {
      this.BreadCrumbList.CopyTo(array, arrayIndex);
    }

    bool ICollection<string>.Remove(string item) => this.BreadCrumbList.Remove(item);

    public int Count => this.BreadCrumbList.Count;

    public bool IsReadOnly => false;

    public IEnumerator<string> GetEnumerator()
    {
      return (IEnumerator<string>) this.BreadCrumbList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.BreadCrumbList.GetEnumerator();
  }
}
