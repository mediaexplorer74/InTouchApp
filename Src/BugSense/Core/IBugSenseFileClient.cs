// Decompiled with JetBrains decompiler
// Type: BugSense.Core.IBugSenseFileClient
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BugSense.Core
{
  internal interface IBugSenseFileClient
  {
    void CreateDirectoriesIfNotExist();

    BugSenseLogResult Save(string filePath, string jsonRequest);

    string Read(string filePath);

    Task<BugSenseLogResult> SaveAsync(string filePath, string data);

    Task<string> ReadAsync(string filePath);

    Task<List<string>> ReadLoggedExceptions();

    BugSenseLogResult Delete(string filePath);

    Task<BugSenseLogResult> DeleteAsync(string filePath);
  }
}
