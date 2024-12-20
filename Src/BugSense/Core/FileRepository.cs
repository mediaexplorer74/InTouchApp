// Decompiled with JetBrains decompiler
// Type: BugSense.Core.FileRepository
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;


namespace BugSense.Core
{
  internal class FileRepository : IBugSenseFileClient
  {
    private SemaphoreSlim SemaphoreMutex { get; set; }

    public FileRepository() => this.SemaphoreMutex = new SemaphoreSlim(1);

    public async void CreateDirectoriesIfNotExist()
    {
      BugSenseLogResult ifNotExistsAsync = await this.CreateDirectoriesIfNotExistsAsync();
    }

    private async Task<BugSenseLogResult> CreateDirectoriesIfNotExistsAsync()
    {
      this.SemaphoreMutex.WaitAsync().ConfigureAwait(false);
      BugSenseLogResult logResult = new BugSenseLogResult();
      try
      {
        StorageFolder folderAsync1 = await ApplicationData.Current.LocalFolder.CreateFolderAsync(BugSenseProperties.ExceptionsFolderName, (CreationCollisionOption) 3);
        StorageFolder folderAsync2 = await ApplicationData.Current.LocalFolder.CreateFolderAsync(BugSenseProperties.GeneralFolderName, (CreationCollisionOption) 3);
        logResult.ResultState = BugSenseResultState.OK;
      }
      catch (Exception ex)
      {
        logResult.ResultState = BugSenseResultState.Error;
        logResult.ExceptionError = ex;
      }
      this.SemaphoreMutex.Release();
      return logResult;
    }

    public BugSenseLogResult Save(string filePath, string jsonRequest)
    {
      Task<BugSenseLogResult> task = Task.Run<BugSenseLogResult>((Func<Task<BugSenseLogResult>>) (async () => await this.SaveStaticHelperAsync(filePath, jsonRequest).ConfigureAwait(false)));
      task.Wait();
      return task.Result;
    }

    private async Task<BugSenseLogResult> SaveStaticHelperAsync(string filePath, string jsonRequest)
    {
      BugSenseLogResult logResult = new BugSenseLogResult();
      if (!filePath.Contains(BugSenseProperties.ExceptionsFolderName))
      {
        if (!filePath.Contains(BugSenseProperties.GeneralFolderName))
          filePath = BugSenseProperties.ExceptionsFolderName + "\\" + filePath;
      }
      try
      {
        this.SemaphoreMutex.WaitAsync().ConfigureAwait(false);
        StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filePath, (CreationCollisionOption) 1);
        if (file != null)
        {
          using (IRandomAccessStream randomAccessStreams = await file.OpenAsync((FileAccessMode) 1))
          {
            using (IOutputStream outputStream = randomAccessStreams.GetOutputStreamAt(0UL))
            {
              using (DataWriter dataWriter = new DataWriter(outputStream))
              {
                int num1 = (int) dataWriter.WriteString(jsonRequest);
                int num2 = (int) await (IAsyncOperation<uint>) dataWriter.StoreAsync();
                dataWriter.DetachStream();
              }
              int num = await outputStream.FlushAsync() ? 1 : 0;
              logResult.ResultState = BugSenseResultState.OK;
            }
          }
        }
      }
      catch (Exception ex)
      {
        logResult.ResultState = BugSenseResultState.Error;
        logResult.ExceptionError = ex;
      }
      this.SemaphoreMutex.Release();
      return logResult;
    }

    public string Read(string filePath)
    {
      Task<string> task = Task.Run<string>((Func<Task<string>>) (async () => await this.ReadStaticHelperAsync(filePath).ConfigureAwait(false)));
      task.Wait();
      return task.Result;
    }

    private async Task<string> ReadStaticHelperAsync(string filePath)
    {
      StorageFile file = (StorageFile) null;
      if (!filePath.Contains(BugSenseProperties.ExceptionsFolderName))
      {
        if (!filePath.Contains(BugSenseProperties.GeneralFolderName))
          filePath = BugSenseProperties.ExceptionsFolderName + "\\" + filePath;
      }
      try
      {
        file = await ApplicationData.Current.LocalFolder.GetFileAsync(filePath);
      }
      catch
      {
      }
      string data = string.Empty;
      if (file == null)
        return data;
      using (IRandomAccessStream randomAccessStream = await file.OpenAsync((FileAccessMode) 0))
      {
        using (IInputStream inputStream = randomAccessStream.GetInputStreamAt(0UL))
        {
          using (DataReader dataReader = new DataReader(inputStream))
          {
            int num = (int) await (IAsyncOperation<uint>) dataReader.LoadAsync((uint) randomAccessStream.Size);
            data = dataReader.ReadString((uint) randomAccessStream.Size);
            dataReader.DetachStream();
            return data;
          }
        }
      }
    }

    public async Task<BugSenseLogResult> SaveAsync(string filePath, string data)
    {
      return await this.SaveStaticHelperAsync(filePath, data).ConfigureAwait(false);
    }

    public async Task<string> ReadAsync(string filePath)
    {
      string result = await this.ReadStaticHelperAsync(filePath).ConfigureAwait(false);
      return result;
    }

    public async Task<List<string>> ReadLoggedExceptions()
    {
      StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(BugSenseProperties.ExceptionsFolderName);
      IReadOnlyList<StorageFile> filesList = await folder.GetFilesAsync();
      List<string> filePathList = new List<string>();
      foreach (StorageFile storageFile in (IEnumerable<StorageFile>) filesList)
        filePathList.Add(storageFile.Name);
      return filePathList;
    }

    public BugSenseLogResult Delete(string filePath)
    {
      Task<BugSenseLogResult> task = Task.Run<BugSenseLogResult>((Func<Task<BugSenseLogResult>>) (async () => await this.DeleteAsync(filePath).ConfigureAwait(false)));
      task.Wait();
      return task.Result;
    }

    public async Task<BugSenseLogResult> DeleteAsync(string filePath)
    {
      StorageFile file = (StorageFile) null;
      BugSenseLogResult logResult = new BugSenseLogResult();
      if (!filePath.Contains(BugSenseProperties.ExceptionsFolderName))
      {
        if (!filePath.Contains(BugSenseProperties.GeneralFolderName))
          filePath = BugSenseProperties.ExceptionsFolderName + "\\" + filePath;
      }
      try
      {
        this.SemaphoreMutex.WaitAsync().ConfigureAwait(false);
        file = await ApplicationData.Current.LocalFolder.GetFileAsync(filePath);
      }
      catch (Exception ex)
      {
        logResult.ResultState = BugSenseResultState.Error;
        logResult.ExceptionError = ex;
      }
      if (file != null)
      {
        await file.DeleteAsync();
        logResult.ResultState = BugSenseResultState.OK;
      }
      this.SemaphoreMutex.Release();
      return logResult;
    }
  }
}
