// Decompiled with JetBrains decompiler
// Type: InTouchLibrary.SyncSessionState
// Assembly: InTouchLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EC5C0AD-EDBE-443B-8339-00330D273297
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\InTouchLibrary.dll

#nullable disable
namespace InTouchLibrary
{
  public enum SyncSessionState
  {
    Invalid = -1, // 0xFFFFFFFF
    SyncCompleted = 0,
    DownloadingPhotos = 1,
    DownloadingAutoContacts = 2,
    DownloadingManualContacts = 3,
    UploadingWPContacts = 4,
    FindingModifiedIntouchContacts = 5,
    FindingModifiedOtherContacts = 6,
    RestoringContacts = 7,
  }
}
