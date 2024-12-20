// Decompiled with JetBrains decompiler
// Type: windowsphone_app.OtherItems
// Assembly: windowsphone_app, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A15F7417-63C2-423F-A22E-030DF791B1B9
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\windowsphone_app.exe

using System.Collections.ObjectModel;

namespace windowsphone_app
{
  public class OtherItems
  {
    public string itemName { get; set; }

    public OtherItems(string item) => this.itemName = item;

    public static ObservableCollection<OtherItems> ListOfItems()
    {
      return new ObservableCollection<OtherItems>();
    }
  }
}
