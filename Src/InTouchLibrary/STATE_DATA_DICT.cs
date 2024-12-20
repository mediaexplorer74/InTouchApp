// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.STATE_DATA_DICT
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

#nullable disable
namespace InTouchLibrary
{
  internal class STATE_DATA_DICT
  {
    public string SyncSessionOwner { get; set; }

    public string SyncSessionID { get; set; }

    public string SyncSessionState { get; set; }

    public string StoreRevisionNumber { get; set; }

    public string SavedRevisionNumber { get; set; }

    public string SyncSessionLastUpdateTime { get; set; }

    public string StoreModifiedContactsCount { get; set; }

    public string DBDirtyIntouchContactsCount { get; set; }
  }
}
