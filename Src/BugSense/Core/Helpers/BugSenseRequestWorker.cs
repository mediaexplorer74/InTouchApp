// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Helpers.BugSenseRequestWorker
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Interfaces;
using BugSense.Core.Model;
using BugSense.Device.Specific;
using Splunk.Mi.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BugSense.Core.Helpers
{
  internal class BugSenseRequestWorker : IRequestWorker
  {
    private const string NotInitializedDescription = "Splunk plugin is not initialized properly";
    private const string NotHandledWhileDebugging = "HandleWhileDebugging is false, ResultState is OK but no action really taken";

    public event EventHandler<BugSenseLoggedRequestEventArgs> LoggedRequestHandled = (param0, param1) => { };

    public event EventHandler<BugSenseResponseResultEventArgs> PingEventCompleted = (param0, param1) => { };

    public event EventHandler<NetworkDataFixture> NetworkDataLogged = (param0, param1) => { };

    private IBugSenseFileClient FileRepository { get; set; }

    private IBugSenseServiceClient ServiceRepository { get; set; }

    private IRequestJsonSerializer RequestJsonSerializer { get; set; }

    private IDeviceUtil DeviceInfo { get; set; }

    private IContentResolver ContentResolver { get; set; }

    private SplunkTransactionManager TransactionManager { get; set; }

    public bool IsInitialized { get; set; }

    public BugSenseRequestWorker(
      IBugSenseFileClient fileRepo,
      IBugSenseServiceClient serviceRepo,
      IRequestJsonSerializer requestJsonSerializer,
      IDeviceUtil deviceInfo,
      IContentResolver contentResolver)
    {
      this.FileRepository = fileRepo;
      this.ServiceRepository = serviceRepo;
      this.RequestJsonSerializer = requestJsonSerializer;
      this.DeviceInfo = deviceInfo;
      this.ContentResolver = contentResolver;
      this.TransactionManager = new SplunkTransactionManager(fileRepo, deviceInfo, requestJsonSerializer);
    }

    private async Task SendEventAndProcessPreviousLoggedCrashes()
    {
      BugSenseResponseResult pingResponseResult = await this.SendEventAsync(BugSenseEventTag.Ping).ConfigureAwait(false);
      this.OnPingEventCompleted((object) this, new BugSenseResponseResultEventArgs()
      {
        ResponseResult = pingResponseResult
      });
      await this.ProcessPreviousLoggedCrashesAsync().ConfigureAwait(false);
    }

    public void Init()
    {
      this.FileRepository.CreateDirectoriesIfNotExist();
      this.DeviceInfo.AppendBugSenseInfo();
      this.DeviceInfo.GetDeviceConnectionInfo();
      this.DeviceInfo.GetScreenInfo();
      this.IsInitialized = true;
      SessionManager.Instance.SessionStart = DateTime.UtcNow;
      this.SendEventAndProcessPreviousLoggedCrashes();
    }

    public string GetErrorHash(string jsonRequest)
    {
      return this.RequestJsonSerializer.GetErrorHash(jsonRequest);
    }

    public async Task Flush()
    {
      await this.ProcessPreviousLoggedCrashesAsync().ConfigureAwait(false);
    }

    public Task<TransactionStartResult> TransactionStart(string transactionId)
    {
      return this.TransactionManager.TransactionStart(transactionId);
    }

    public Task<TransactionStopResult> TransactionStop(string transactionId)
    {
      return this.TransactionManager.TransactionStop(transactionId);
    }

    public Task<TransactionStopResult> TransactionCancel(string transactionId, string reason)
    {
      return this.TransactionManager.TransactionCancel(transactionId, reason);
    }

    public void StopAllTransactions(string errorHash) => this.TransactionManager.StopAll(errorHash);

    public async Task<long> GetLastCrashId()
    {
      long lastCrashErrorid = 0;
      if (this.IsInitialized)
      {
        string filePath = BugSenseProperties.GeneralFolderName + "\\" + BugSenseProperties.CrashOnLastRunFileName;
        string contents = await this.FileRepository.ReadAsync(filePath).ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(contents))
        {
          CrashOnLastRun lastCrash = contents.DeserializeJson<CrashOnLastRun>();
          if (lastCrash != null)
          {
            lastCrashErrorid = lastCrash.ErrorID;
            lastCrash.ErrorID = 0L;
            BugSenseLogResult bugSenseLogResult = await this.FileRepository.SaveAsync(filePath, lastCrash.SerializeToJson<CrashOnLastRun>()).ConfigureAwait(false);
          }
        }
      }
      return lastCrashErrorid;
    }

    public async Task<int> GetTotalCrashesNum()
    {
      if (this.IsInitialized)
      {
        string filePath = BugSenseProperties.GeneralFolderName + "\\" + BugSenseProperties.CrashOnLastRunFileName;
        string contents = await this.FileRepository.ReadAsync(filePath).ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(contents))
        {
          CrashOnLastRun crashOnLastRun = contents.DeserializeJson<CrashOnLastRun>();
          if (crashOnLastRun != null)
            return crashOnLastRun.TotalCrashes;
        }
      }
      return 0;
    }

    public async Task<bool> ClearTotalCrashesNum()
    {
      bool result = false;
      if (this.IsInitialized)
      {
        string filePath = BugSenseProperties.GeneralFolderName + "\\" + BugSenseProperties.CrashOnLastRunFileName;
        string contents = await this.FileRepository.ReadAsync(filePath).ConfigureAwait(false);
        BugSenseLogResult logResult = new BugSenseLogResult();
        if (!string.IsNullOrWhiteSpace(contents))
        {
          CrashOnLastRun lastCrash = contents.DeserializeJson<CrashOnLastRun>();
          if (lastCrash != null)
          {
            lastCrash.TotalCrashes = 0;
            logResult = await this.FileRepository.SaveAsync(filePath, lastCrash.SerializeToJson<CrashOnLastRun>());
          }
        }
        result = logResult.ResultState == BugSenseResultState.OK;
      }
      return result;
    }

    public async Task<BugSenseResponseResult> SendEventAsync(BugSenseEventTag tag)
    {
      BugSenseResponseResult senseResponseResult = new BugSenseResponseResult();
      senseResponseResult.ResultState = BugSenseProperties.HandleWhileDebugging ? BugSenseResultState.Undefined : BugSenseResultState.OK;
      senseResponseResult.RequestType = BugSenseRequestType.Event;
      senseResponseResult.Description = BugSenseProperties.HandleWhileDebugging ? (string) null : "HandleWhileDebugging is false, ResultState is OK but no action really taken";
      senseResponseResult.HandledWhileDebugging = BugSenseProperties.HandleWhileDebugging;
      BugSenseResponseResult result = senseResponseResult;
      if (BugSenseProperties.HandleWhileDebugging)
      {
        SerializeResult jsonSerializeResult = this.RequestJsonSerializer.SerializeEventToJson(tag, this.DeviceInfo.GetAppEnvironment());
        string bugSenseEventData = jsonSerializeResult.EncodedJson;
        string fileName = CommonHelper.NewFileNamePath(tag == BugSenseEventTag.Ping ? FileNameType.Ping : FileNameType.Gnip);
        BugSenseLogResult logResult = await this.FileRepository.SaveAsync(fileName, bugSenseEventData);
        if (logResult.ResultState == BugSenseResultState.OK)
        {
          result = await this.ServiceRepository.ExecuteBugSenseRequestAsync(BugSenseProperties.AnalyticsURL, bugSenseEventData, false, this.ContentResolver.EventContentType());
          result.RequestType = BugSenseRequestType.Event;
          result.ClientRequest = jsonSerializeResult.DecodedJson;
          if (result.ResultState == BugSenseResultState.OK)
          {
            BugSenseLogResult bugSenseLogResult = await this.FileRepository.DeleteAsync(fileName);
          }
        }
      }
      return result;
    }

    public BugSenseLogResult LogEvent(BugSenseEventTag tag)
    {
      BugSenseLogResult bugSenseLogResult1 = new BugSenseLogResult();
      bugSenseLogResult1.ResultState = BugSenseProperties.HandleWhileDebugging ? BugSenseResultState.Undefined : BugSenseResultState.OK;
      bugSenseLogResult1.RequestType = BugSenseRequestType.Event;
      bugSenseLogResult1.Description = BugSenseProperties.HandleWhileDebugging ? (string) null : "HandleWhileDebugging is false, ResultState is OK but no action really taken";
      bugSenseLogResult1.HandledWhileDebugging = BugSenseProperties.HandleWhileDebugging;
      BugSenseLogResult bugSenseLogResult2 = bugSenseLogResult1;
      if (BugSenseProperties.HandleWhileDebugging)
      {
        SerializeResult json = this.RequestJsonSerializer.SerializeEventToJson(tag, this.DeviceInfo.GetAppEnvironment());
        string encodedJson = json.EncodedJson;
        bugSenseLogResult2 = this.FileRepository.Save(CommonHelper.NewFileNamePath(tag == BugSenseEventTag.Ping ? FileNameType.Ping : FileNameType.Gnip), encodedJson);
        bugSenseLogResult2.ClientRequest = json.DecodedJson;
        bugSenseLogResult2.RequestType = BugSenseRequestType.Event;
      }
      else
        bugSenseLogResult2.SetHandleWhileDebuggingExceptionError();
      return bugSenseLogResult2;
    }

    public async Task<BugSenseLogResult> LogEventAsync(BugSenseEventTag tag)
    {
      BugSenseLogResult bugSenseLogResult = new BugSenseLogResult();
      bugSenseLogResult.ResultState = BugSenseProperties.HandleWhileDebugging ? BugSenseResultState.Undefined : BugSenseResultState.OK;
      bugSenseLogResult.RequestType = BugSenseRequestType.Event;
      bugSenseLogResult.Description = BugSenseProperties.HandleWhileDebugging ? (string) null : "HandleWhileDebugging is false, ResultState is OK but no action really taken";
      bugSenseLogResult.HandledWhileDebugging = BugSenseProperties.HandleWhileDebugging;
      BugSenseLogResult result = bugSenseLogResult;
      if (BugSenseProperties.HandleWhileDebugging)
      {
        SerializeResult jsonSerializeResult = this.RequestJsonSerializer.SerializeEventToJson(tag, this.DeviceInfo.GetAppEnvironment());
        string bugSenseEventData = jsonSerializeResult.EncodedJson;
        string fileName = CommonHelper.NewFileNamePath(tag == BugSenseEventTag.Ping ? FileNameType.Ping : FileNameType.Gnip);
        result = await this.FileRepository.SaveAsync(fileName, bugSenseEventData);
        result.ClientRequest = jsonSerializeResult.DecodedJson;
        result.RequestType = BugSenseRequestType.Event;
      }
      else
        result.SetHandleWhileDebuggingExceptionError();
      return result;
    }

    public async Task<BugSenseResponseResult> SendEventAsync(string tag)
    {
      BugSenseResponseResult senseResponseResult = new BugSenseResponseResult();
      senseResponseResult.ResultState = BugSenseProperties.HandleWhileDebugging ? BugSenseResultState.Undefined : BugSenseResultState.OK;
      senseResponseResult.RequestType = BugSenseRequestType.Event;
      senseResponseResult.Description = BugSenseProperties.HandleWhileDebugging ? (string) null : "HandleWhileDebugging is false, ResultState is OK but no action really taken";
      senseResponseResult.HandledWhileDebugging = BugSenseProperties.HandleWhileDebugging;
      BugSenseResponseResult result = senseResponseResult;
      if (BugSenseProperties.HandleWhileDebugging)
      {
        if (!string.IsNullOrWhiteSpace(tag))
        {
          SerializeResult jsonSerializeResult = this.RequestJsonSerializer.SerializeEventToJson(tag, this.DeviceInfo.GetAppEnvironment());
          string bugSenseEventData = jsonSerializeResult.EncodedJson;
          string fileName = CommonHelper.NewFileNamePath(FileNameType.Event);
          BugSenseLogResult logResult = await this.FileRepository.SaveAsync(fileName, tag);
          result.ClientRequest = jsonSerializeResult.DecodedJson;
          if (logResult.ResultState == BugSenseResultState.OK)
          {
            result = await this.ServiceRepository.ExecuteBugSenseRequestAsync(BugSenseProperties.AnalyticsURL, bugSenseEventData, false, this.ContentResolver.EventContentType());
            result.ClientRequest = jsonSerializeResult.DecodedJson;
            result.RequestType = BugSenseRequestType.Event;
            if (result.ResultState == BugSenseResultState.OK)
            {
              BugSenseLogResult bugSenseLogResult = await this.FileRepository.DeleteAsync(fileName);
            }
          }
          else
          {
            result.ExceptionError = logResult.ExceptionError;
            result.Description = logResult.Description;
            result.HandledWhileDebugging = logResult.HandledWhileDebugging;
          }
        }
        else
        {
          result.ResultState = BugSenseResultState.Error;
          result.Description = "Event tag is null or empty.";
        }
      }
      else
        result.SetHandleWhileDebuggingExceptionError();
      return result;
    }

    public async Task<BugSenseLogResult> LogEventAsync(string tag)
    {
      BugSenseLogResult bugSenseLogResult = new BugSenseLogResult();
      bugSenseLogResult.ResultState = BugSenseProperties.HandleWhileDebugging ? BugSenseResultState.Undefined : BugSenseResultState.OK;
      bugSenseLogResult.RequestType = BugSenseRequestType.Event;
      bugSenseLogResult.Description = BugSenseProperties.HandleWhileDebugging ? (string) null : "HandleWhileDebugging is false, ResultState is OK but no action really taken";
      bugSenseLogResult.HandledWhileDebugging = BugSenseProperties.HandleWhileDebugging;
      BugSenseLogResult result = bugSenseLogResult;
      if (BugSenseProperties.HandleWhileDebugging)
      {
        if (!string.IsNullOrWhiteSpace(tag))
        {
          SerializeResult jsonSerializeResult = this.RequestJsonSerializer.SerializeEventToJson(tag, this.DeviceInfo.GetAppEnvironment());
          string bugSenseEventData = jsonSerializeResult.EncodedJson;
          result = await this.FileRepository.SaveAsync(CommonHelper.NewFileNamePath(FileNameType.Event), bugSenseEventData);
          result.ClientRequest = jsonSerializeResult.DecodedJson;
          result.RequestType = BugSenseRequestType.Event;
        }
        else
        {
          result.ResultState = BugSenseResultState.Error;
          result.Description = "Event tag cannot be null or empty.";
        }
      }
      else
        result.SetHandleWhileDebuggingExceptionError();
      return result;
    }

    public BugSenseLogResult LogEvent(string tag)
    {
      BugSenseLogResult bugSenseLogResult1 = new BugSenseLogResult();
      bugSenseLogResult1.ResultState = BugSenseProperties.HandleWhileDebugging ? BugSenseResultState.Undefined : BugSenseResultState.OK;
      bugSenseLogResult1.RequestType = BugSenseRequestType.Event;
      bugSenseLogResult1.Description = BugSenseProperties.HandleWhileDebugging ? (string) null : "HandleWhileDebugging is false, ResultState is OK but no action really taken";
      bugSenseLogResult1.HandledWhileDebugging = BugSenseProperties.HandleWhileDebugging;
      BugSenseLogResult bugSenseLogResult2 = bugSenseLogResult1;
      if (BugSenseProperties.HandleWhileDebugging)
      {
        if (!string.IsNullOrWhiteSpace(tag))
        {
          SerializeResult json = this.RequestJsonSerializer.SerializeEventToJson(tag, this.DeviceInfo.GetAppEnvironment());
          string encodedJson = json.EncodedJson;
          bugSenseLogResult2 = this.FileRepository.Save(CommonHelper.NewFileNamePath(FileNameType.Event), encodedJson);
          bugSenseLogResult2.ClientRequest = json.DecodedJson;
          bugSenseLogResult2.RequestType = BugSenseRequestType.Event;
        }
        else
        {
          bugSenseLogResult2.ResultState = BugSenseResultState.Error;
          bugSenseLogResult2.Description = "Event tag cannot be null or empty.";
        }
      }
      else
        bugSenseLogResult2.SetHandleWhileDebuggingExceptionError();
      return bugSenseLogResult2;
    }

    public async Task<BugSenseResponseResult> SendExceptionAsync(
      Exception exception,
      LimitedCrashExtraDataList extraData = null,
      string filePath = null)
    {
      BugSenseResponseResult senseResponseResult = new BugSenseResponseResult();
      senseResponseResult.ResultState = BugSenseProperties.HandleWhileDebugging ? BugSenseResultState.Undefined : BugSenseResultState.OK;
      senseResponseResult.RequestType = BugSenseRequestType.Error;
      senseResponseResult.Description = BugSenseProperties.HandleWhileDebugging ? (string) null : "HandleWhileDebugging is false, ResultState is OK but no action really taken";
      senseResponseResult.HandledWhileDebugging = BugSenseProperties.HandleWhileDebugging;
      BugSenseResponseResult result = senseResponseResult;
      if (BugSenseProperties.HandleWhileDebugging)
      {
        AppEnvironment appEnvironment = this.DeviceInfo.GetAppEnvironment();
        BugSensePerformance bugSensePerformance = this.DeviceInfo.GetBugSensePerformance();
        SerializeResult jsonSerializeResult = this.RequestJsonSerializer.SerializeCrashToJson(exception, appEnvironment, bugSensePerformance, true, extraData);
        if (!string.IsNullOrWhiteSpace(jsonSerializeResult.EncodedJson))
        {
          string serializedData = jsonSerializeResult.EncodedJson;
          if (string.IsNullOrWhiteSpace(filePath))
            filePath = CommonHelper.NewFileNamePath(FileNameType.LoggedException);
          BugSenseLogResult logResult = await this.FileRepository.SaveAsync(filePath, serializedData);
          result.ClientRequest = jsonSerializeResult.DecodedJson;
          if (logResult.ResultState == BugSenseResultState.OK)
          {
            result = await this.ServiceRepository.ExecuteBugSenseRequestAsync(BugSenseProperties.Url, serializedData, true, this.ContentResolver.ErrorContentType());
            result.ClientRequest = jsonSerializeResult.DecodedJson;
            if (result.ResultState == BugSenseResultState.OK)
            {
              BugSenseLogResult bugSenseLogResult = await this.FileRepository.DeleteAsync(filePath);
            }
          }
          else
          {
            result.ExceptionError = logResult.ExceptionError;
            result.Description = logResult.Description;
            result.HandledWhileDebugging = logResult.HandledWhileDebugging;
          }
        }
        else
        {
          result.ResultState = BugSenseResultState.Error;
          result.Description = "JSON encoding failed";
        }
      }
      else
        result.SetHandleWhileDebuggingExceptionError();
      return result;
    }

    public async Task<BugSenseLogResult> LogExceptionAsync(
      Exception exception,
      LimitedCrashExtraDataList extraData = null,
      string filePath = null)
    {
      BugSenseLogResult bugSenseLogResult = new BugSenseLogResult();
      bugSenseLogResult.ResultState = BugSenseProperties.HandleWhileDebugging ? BugSenseResultState.Undefined : BugSenseResultState.OK;
      bugSenseLogResult.RequestType = BugSenseRequestType.Error;
      bugSenseLogResult.Description = BugSenseProperties.HandleWhileDebugging ? (string) null : "HandleWhileDebugging is false, ResultState is OK but no action really taken";
      bugSenseLogResult.HandledWhileDebugging = BugSenseProperties.HandleWhileDebugging;
      BugSenseLogResult result = bugSenseLogResult;
      if (BugSenseProperties.HandleWhileDebugging)
      {
        AppEnvironment appEnvironment = this.DeviceInfo.GetAppEnvironment();
        BugSensePerformance bugSensePerformance = this.DeviceInfo.GetBugSensePerformance();
        SerializeResult jsonSerializeResult = this.RequestJsonSerializer.SerializeCrashToJson(exception, appEnvironment, bugSensePerformance, true, extraData);
        if (!string.IsNullOrWhiteSpace(jsonSerializeResult.EncodedJson))
        {
          string serializedData = jsonSerializeResult.EncodedJson;
          if (string.IsNullOrWhiteSpace(filePath))
            filePath = CommonHelper.NewFileNamePath(FileNameType.LoggedException);
          result = await this.FileRepository.SaveAsync(filePath, serializedData);
          result.ClientRequest = jsonSerializeResult.DecodedJson;
        }
        else
        {
          result.ResultState = BugSenseResultState.Error;
          result.Description = "JSON encoding failed";
        }
      }
      else
        result.SetHandleWhileDebuggingExceptionError();
      return result;
    }

    public BugSenseLogResult LogException(
      Exception exception,
      LimitedCrashExtraDataList extraData = null,
      string filePath = null)
    {
      BugSenseLogResult bugSenseLogResult1 = new BugSenseLogResult();
      bugSenseLogResult1.ResultState = BugSenseProperties.HandleWhileDebugging ? BugSenseResultState.Undefined : BugSenseResultState.OK;
      bugSenseLogResult1.RequestType = BugSenseRequestType.Error;
      bugSenseLogResult1.Description = BugSenseProperties.HandleWhileDebugging ? (string) null : "HandleWhileDebugging is false, ResultState is OK but no action really taken";
      bugSenseLogResult1.HandledWhileDebugging = BugSenseProperties.HandleWhileDebugging;
      BugSenseLogResult bugSenseLogResult2 = bugSenseLogResult1;
      if (BugSenseProperties.HandleWhileDebugging)
      {
        AppEnvironment appEnvironment = this.DeviceInfo.GetAppEnvironment();
        BugSensePerformance sensePerformance = this.DeviceInfo.GetBugSensePerformance();
        SerializeResult json = this.RequestJsonSerializer.SerializeCrashToJson(exception, appEnvironment, sensePerformance, true, extraData);
        if (!string.IsNullOrWhiteSpace(json.EncodedJson))
        {
          string encodedJson = json.EncodedJson;
          if (string.IsNullOrWhiteSpace(filePath))
            filePath = CommonHelper.NewFileNamePath(FileNameType.LoggedException);
          bugSenseLogResult2 = this.FileRepository.Save(filePath, encodedJson);
          bugSenseLogResult2.ClientRequest = json.DecodedJson;
        }
        else
        {
          bugSenseLogResult2.ResultState = BugSenseResultState.Error;
          bugSenseLogResult2.Description = "JSON encoding failed";
        }
      }
      else
        bugSenseLogResult2.SetHandleWhileDebuggingExceptionError();
      return bugSenseLogResult2;
    }

    public BugSenseLogResult HandleException(
      Exception exception,
      LimitedCrashExtraDataList extraData = null,
      string filePath = null)
    {
      BugSenseLogResult bugSenseLogResult1 = new BugSenseLogResult();
      bugSenseLogResult1.ResultState = BugSenseProperties.HandleWhileDebugging ? BugSenseResultState.Undefined : BugSenseResultState.OK;
      bugSenseLogResult1.RequestType = BugSenseRequestType.Error;
      bugSenseLogResult1.Description = BugSenseProperties.HandleWhileDebugging ? (string) null : "HandleWhileDebugging is false, ResultState is OK but no action really taken";
      bugSenseLogResult1.HandledWhileDebugging = BugSenseProperties.HandleWhileDebugging;
      BugSenseLogResult bugSenseLogResult2 = bugSenseLogResult1;
      if (BugSenseProperties.HandleWhileDebugging)
      {
        AppEnvironment appEnvironment = this.DeviceInfo.GetAppEnvironment();
        BugSensePerformance sensePerformance = this.DeviceInfo.GetBugSensePerformance();
        SerializeResult json = this.RequestJsonSerializer.SerializeCrashToJson(exception, appEnvironment, sensePerformance, false, extraData);
        if (!string.IsNullOrWhiteSpace(json.EncodedJson))
        {
          string encodedJson = json.EncodedJson;
          bugSenseLogResult2.ClientRequest = json.DecodedJson;
          if (string.IsNullOrWhiteSpace(filePath))
            filePath = CommonHelper.NewFileNamePath(FileNameType.UnhandledException);
          BugSenseLogResult bugSenseLogResult3 = this.FileRepository.Save(filePath, encodedJson);
          bugSenseLogResult2.ResultState = bugSenseLogResult3.ResultState;
          if (bugSenseLogResult3.ResultState != BugSenseResultState.OK)
          {
            bugSenseLogResult2.ExceptionError = bugSenseLogResult3.ExceptionError;
            bugSenseLogResult2.Description = bugSenseLogResult3.Description;
            bugSenseLogResult2.HandledWhileDebugging = bugSenseLogResult3.HandledWhileDebugging;
          }
          if (BugSenseProperties.LastAction != null)
            BugSenseProperties.LastAction();
        }
        else
        {
          bugSenseLogResult2.ResultState = BugSenseResultState.Error;
          bugSenseLogResult2.Description = "JSON encoding failed";
        }
      }
      else
        bugSenseLogResult2.SetHandleWhileDebuggingExceptionError();
      return bugSenseLogResult2;
    }

    public async Task<BugSenseLogResult> HandleExceptionAsync(
      Exception exception,
      LimitedCrashExtraDataList extraData = null,
      string filePath = null)
    {
      BugSenseLogResult bugSenseLogResult = new BugSenseLogResult();
      bugSenseLogResult.ResultState = BugSenseProperties.HandleWhileDebugging ? BugSenseResultState.Undefined : BugSenseResultState.OK;
      bugSenseLogResult.RequestType = BugSenseRequestType.Error;
      bugSenseLogResult.Description = BugSenseProperties.HandleWhileDebugging ? (string) null : "HandleWhileDebugging is false, ResultState is OK but no action really taken";
      bugSenseLogResult.HandledWhileDebugging = BugSenseProperties.HandleWhileDebugging;
      BugSenseLogResult result = bugSenseLogResult;
      if (BugSenseProperties.HandleWhileDebugging)
      {
        AppEnvironment appEnvironment = this.DeviceInfo.GetAppEnvironment();
        BugSensePerformance bugSensePerformance = this.DeviceInfo.GetBugSensePerformance();
        SerializeResult jsonSerializeResult = this.RequestJsonSerializer.SerializeCrashToJson(exception, appEnvironment, bugSensePerformance, false, extraData);
        if (!string.IsNullOrWhiteSpace(jsonSerializeResult.EncodedJson))
        {
          string serializedData = jsonSerializeResult.EncodedJson;
          result.ClientRequest = jsonSerializeResult.DecodedJson;
          if (string.IsNullOrWhiteSpace(filePath))
            filePath = CommonHelper.NewFileNamePath(FileNameType.UnhandledException);
          BugSenseLogResult logResult = await this.FileRepository.SaveAsync(filePath, serializedData);
          result.ClientRequest = jsonSerializeResult.DecodedJson;
          result.ResultState = logResult.ResultState;
          if (logResult.ResultState != BugSenseResultState.OK)
          {
            result.ExceptionError = logResult.ExceptionError;
            result.Description = logResult.Description;
            result.HandledWhileDebugging = logResult.HandledWhileDebugging;
          }
          if (BugSenseProperties.LastAction != null)
            BugSenseProperties.LastAction();
        }
        else
        {
          result.ResultState = BugSenseResultState.Error;
          result.Description = "JSON encoding failed";
        }
      }
      else
        result.SetHandleWhileDebuggingExceptionError();
      return result;
    }

    public async Task ProcessPreviousLoggedCrashesAsync()
    {
      List<string> filesToSend = await this.FileRepository.ReadLoggedExceptions().ConfigureAwait(false);
      if (filesToSend == null || filesToSend.Count <= 0)
        return;
      foreach (string fileName in filesToSend)
      {
        string filePath = fileName;
        string content = await this.FileRepository.ReadAsync(filePath);
        bool isError = filePath.Contains("LLC") || filePath.Contains("CCC");
        BugSenseResponseResult result = await this.ServiceRepository.ExecuteBugSenseRequestAsync(isError ? BugSenseProperties.Url : BugSenseProperties.AnalyticsURL, content, isError, isError ? this.ContentResolver.ErrorContentType() : this.ContentResolver.EventContentType());
        if (result.ResultState == BugSenseResultState.OK)
        {
          BugSenseLogResult bugSenseLogResult = await this.FileRepository.DeleteAsync(fileName);
        }
        result.ClientRequest = this.RequestJsonSerializer.DecodeEncodedCrashJson(content);
        this.OnLoggedRequestHandled((object) this, new BugSenseLoggedRequestEventArgs()
        {
          BugSenseLoggedResponseResult = result
        });
      }
    }

    public BugSenseLogResult StartSession()
    {
      BugSenseLogResult bugSenseLogResult1 = new BugSenseLogResult();
      bugSenseLogResult1.ResultState = BugSenseResultState.Undefined;
      bugSenseLogResult1.Description = this.IsInitialized ? (string) null : "Splunk plugin is not initialized properly";
      BugSenseLogResult bugSenseLogResult2 = bugSenseLogResult1;
      if (this.IsInitialized && DateTime.Now.Subtract(SessionManager.Instance.PingSessionStart).Minutes > 1)
        bugSenseLogResult2 = this.LogEvent(BugSenseEventTag.Ping);
      return bugSenseLogResult2;
    }

    public async Task<BugSenseLogResult> StartSessionAsync()
    {
      BugSenseLogResult bugSenseLogResult = new BugSenseLogResult();
      bugSenseLogResult.ResultState = BugSenseResultState.Undefined;
      bugSenseLogResult.Description = this.IsInitialized ? (string) null : "Splunk plugin is not initialized properly";
      BugSenseLogResult result = bugSenseLogResult;
      if (this.IsInitialized)
      {
        DateTime pingDateTimeStarted = SessionManager.Instance.PingSessionStart;
        DateTime dateTimeNow = DateTime.Now;
        TimeSpan timeSpanSinceLastSession = dateTimeNow.Subtract(pingDateTimeStarted);
        if (timeSpanSinceLastSession.Minutes > 1)
          result = await this.LogEventAsync(BugSenseEventTag.Ping);
      }
      return result;
    }

    public BugSenseLogResult CloseSession()
    {
      BugSenseLogResult bugSenseLogResult1 = new BugSenseLogResult();
      bugSenseLogResult1.ResultState = BugSenseResultState.Undefined;
      bugSenseLogResult1.Description = this.IsInitialized ? (string) null : "Splunk plugin is not initialized properly";
      BugSenseLogResult bugSenseLogResult2 = bugSenseLogResult1;
      if (this.IsInitialized)
        bugSenseLogResult2 = this.LogEvent(BugSenseEventTag.Gnip);
      return bugSenseLogResult2;
    }

    public async Task<BugSenseLogResult> CloseSessionAsync()
    {
      BugSenseLogResult bugSenseLogResult = new BugSenseLogResult();
      bugSenseLogResult.ResultState = BugSenseResultState.Undefined;
      bugSenseLogResult.Description = this.IsInitialized ? (string) null : "Splunk plugin is not initialized properly";
      BugSenseLogResult result = bugSenseLogResult;
      if (this.IsInitialized)
        result = await this.LogEventAsync(BugSenseEventTag.Gnip);
      return result;
    }

    protected virtual void OnLoggedRequestHandled(
      object sender,
      BugSenseLoggedRequestEventArgs args)
    {
      this.LoggedRequestHandled(sender, args);
    }

    protected virtual void OnPingEventCompleted(object sender, BugSenseResponseResultEventArgs args)
    {
      this.PingEventCompleted(sender, args);
    }
  }
}
