// Decompiled with JetBrains decompiler
// Type: BugSense.Core.BugSenseHandlerBase
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Interfaces;
using BugSense.Core.Model;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace BugSense.Core
{
  public abstract class BugSenseHandlerBase
  {
    private const string NotInitializedDescription = "Splunk plugin is not initialized properly";
    private const string ExceptionIsNullMessage = "Exception object parameter is null";
    private string _userIdentifier;
    private bool _handleWhileDebugging = true;
    private bool _useProxy = BugSenseProperties.ProxyEnabled;
    private IRequestWorker _bugSenseWorker;

    public event EventHandler<BugSenseUnhandledHandlerEventArgs> UnhandledExceptionHandled = (param0, param1) => { };

    public event EventHandler<BugSenseLoggedRequestEventArgs> LoggedRequestHandled = (param0, param1) => { };

    public static bool IsInitialized { get; internal set; }

    public string UserIdentifier
    {
      get => this._userIdentifier;
      set
      {
        if (!(value != this._userIdentifier) || string.IsNullOrWhiteSpace(value))
          return;
        this._userIdentifier = BugSenseProperties.UserIdentifier = value;
      }
    }

    public bool HandleWhileDebugging
    {
      get => this._handleWhileDebugging;
      set
      {
        if (value == this._handleWhileDebugging)
          return;
        this._handleWhileDebugging = BugSenseProperties.HandleWhileDebugging = value;
      }
    }

    public bool UseProxy
    {
      get => this._useProxy;
      set
      {
        if (value == this._useProxy)
          return;
        this._useProxy = BugSenseProperties.ProxyEnabled = value;
      }
    }

    public LimitedCrashExtraDataList CrashExtraData => ExtraData.CrashExtraData;

    public LimitedBreadCrumbList Breadcrumbs => ExtraData.BreadCrumbs;

    internal IRequestWorker BugSenseWorker
    {
      get
      {
        return this._bugSenseWorker != null ? this._bugSenseWorker : throw new NullReferenceException("Please call BugSenseHandlerBase.InitAndStartSession(string apiKey) before using the BugSenseWorker in your inherit class");
      }
      private set => this._bugSenseWorker = value;
    }

    internal BugSenseHandlerBase(IRequestWorker worker)
    {
      this.BugSenseWorker = worker;
      this.BugSenseWorker.PingEventCompleted += (EventHandler<BugSenseResponseResultEventArgs>) ((sender, args) => SessionManager.Instance.PingSessionStart = DateTime.UtcNow);
      this.BugSenseWorker.LoggedRequestHandled += (EventHandler<BugSenseLoggedRequestEventArgs>) ((sender, args) => this.OnLoggedRequestHandled((object) this, args));
    }

    protected virtual void InitAndStartSession(string apiKey)
    {
      BugSenseProperties.APIKey = apiKey != null && apiKey.Length >= 8 && apiKey.Length <= 14 ? apiKey : throw new ArgumentException("Your BugSense API Key is invalid!", nameof (apiKey));
      this.BugSenseWorker.Init();
    }

    protected virtual void OnUnhandledExceptionHandled(
      object sender,
      BugSenseUnhandledHandlerEventArgs args)
    {
      this.BugSenseWorker.StopAllTransactions(this.BugSenseWorker.GetErrorHash(args.ClientJsonRequest));
      this.UnhandledExceptionHandled(sender, args);
    }

    protected virtual void OnLoggedRequestHandled(
      object sender,
      BugSenseLoggedRequestEventArgs args)
    {
      this.LoggedRequestHandled(sender, args);
    }

    public async Task Flush() => await this.BugSenseWorker.Flush().ConfigureAwait(false);

    public async Task<long> GetLastCrashId()
    {
      return await this.BugSenseWorker.GetLastCrashId().ConfigureAwait(false);
    }

    public async Task<int> GetTotalCrashesNum()
    {
      return await this.BugSenseWorker.GetTotalCrashesNum().ConfigureAwait(false);
    }

    public async Task<bool> ClearTotalCrashesNum()
    {
      return await this.BugSenseWorker.ClearTotalCrashesNum().ConfigureAwait(false);
    }

    public void AddCrashExtraData(BugSense.Core.Model.CrashExtraData customData)
    {
      if (customData == null)
        throw new ArgumentNullException(nameof (customData), "The CrashExtraData parameter cannot be null.");
      BugSense.Core.Model.CrashExtraData crashExtraData = ExtraData.CrashExtraData.FirstOrDefault<BugSense.Core.Model.CrashExtraData>((Func<BugSense.Core.Model.CrashExtraData, bool>) (p => p.Key.Equals(customData.Key)));
      if (crashExtraData != null)
        crashExtraData.Value = customData.Value;
      else
        ExtraData.CrashExtraData.Add(customData);
    }

    public bool RemoveCrashExtraData(string keyName)
    {
      BugSense.Core.Model.CrashExtraData crashExtraData = ExtraData.CrashExtraData.SingleOrDefault<BugSense.Core.Model.CrashExtraData>((Func<BugSense.Core.Model.CrashExtraData, bool>) (p => p.Key.Equals(keyName)));
      bool flag = crashExtraData != null;
      if (flag)
        ExtraData.CrashExtraData.Remove(crashExtraData);
      return flag;
    }

    public void ClearCrashExtraData() => ExtraData.CrashExtraData.Clear();

    public void LeaveBreadCrumb(string crumb) => ExtraData.BreadCrumbs.Add(crumb);

    public void ClearBreadCrumbs() => ExtraData.BreadCrumbs.Clear();

    public void LastActionBeforeTerminate(Action lastAction)
    {
      BugSenseProperties.LastAction = lastAction;
    }

    public async Task<BugSenseResponseResult> SendEventAsync(string tag)
    {
      BugSenseResponseResult senseResponseResult = new BugSenseResponseResult();
      senseResponseResult.ResultState = BugSenseResultState.Undefined;
      senseResponseResult.Description = BugSenseHandlerBase.IsInitialized ? (string) null : "Splunk plugin is not initialized properly";
      BugSenseResponseResult result = senseResponseResult;
      if (BugSenseHandlerBase.IsInitialized)
        result = await this.BugSenseWorker.SendEventAsync(tag).ConfigureAwait(false);
      return result;
    }

    public async Task<BugSenseLogResult> LogEventAsync(string tag)
    {
      BugSenseLogResult bugSenseLogResult = new BugSenseLogResult();
      bugSenseLogResult.ResultState = BugSenseResultState.Undefined;
      bugSenseLogResult.Description = BugSenseHandlerBase.IsInitialized ? (string) null : "Splunk plugin is not initialized properly";
      BugSenseLogResult result = bugSenseLogResult;
      if (BugSenseHandlerBase.IsInitialized)
        result = await this.BugSenseWorker.LogEventAsync(tag);
      return result;
    }

    public BugSenseLogResult LogEvent(string tag)
    {
      BugSenseLogResult bugSenseLogResult1 = new BugSenseLogResult();
      bugSenseLogResult1.ResultState = BugSenseResultState.Undefined;
      bugSenseLogResult1.Description = BugSenseHandlerBase.IsInitialized ? (string) null : "Splunk plugin is not initialized properly";
      BugSenseLogResult bugSenseLogResult2 = bugSenseLogResult1;
      if (BugSenseHandlerBase.IsInitialized)
        bugSenseLogResult2 = this.BugSenseWorker.LogEvent(tag);
      return bugSenseLogResult2;
    }

    public BugSenseLogResult CloseSession() => this.BugSenseWorker.CloseSession();

    public async Task<BugSenseLogResult> CloseSessionAsync()
    {
      return await this.BugSenseWorker.CloseSessionAsync();
    }

    public BugSenseLogResult StartSession() => this.BugSenseWorker.StartSession();

    public async Task<BugSenseLogResult> StartSessionAsync()
    {
      return await this.BugSenseWorker.StartSessionAsync();
    }

    public async Task<BugSenseResponseResult> SendExceptionAsync(
      Exception exception,
      string key,
      string value)
    {
      BugSenseResponseResult senseResponseResult = new BugSenseResponseResult();
      senseResponseResult.ResultState = BugSenseResultState.Undefined;
      senseResponseResult.Description = BugSenseHandlerBase.IsInitialized ? (string) null : "Splunk plugin is not initialized properly";
      BugSenseResponseResult result = senseResponseResult;
      if (BugSenseHandlerBase.IsInitialized)
      {
        if (exception == null)
          throw new ArgumentNullException(nameof (exception), "Exception object parameter is null");
        LimitedCrashExtraDataList dataList = (LimitedCrashExtraDataList) null;
        if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
          dataList = new LimitedCrashExtraDataList()
          {
            new BugSense.Core.Model.CrashExtraData() { Key = key, Value = value }
          };
        result = await this.BugSenseWorker.SendExceptionAsync(exception, dataList);
      }
      return result;
    }

    public async Task<BugSenseResponseResult> SendExceptionAsync(
      Exception exception,
      LimitedCrashExtraDataList extraData = null)
    {
      BugSenseResponseResult senseResponseResult = new BugSenseResponseResult();
      senseResponseResult.ResultState = BugSenseResultState.Undefined;
      senseResponseResult.Description = BugSenseHandlerBase.IsInitialized ? (string) null : "Splunk plugin is not initialized properly";
      BugSenseResponseResult result = senseResponseResult;
      if (BugSenseHandlerBase.IsInitialized)
      {
        if (exception == null)
          throw new ArgumentNullException(nameof (exception), "Exception object parameter is null");
        result = await this.BugSenseWorker.SendExceptionAsync(exception, extraData);
      }
      return result;
    }

    public async Task<BugSenseLogResult> LogExceptionAsync(
      Exception exception,
      string key,
      string value)
    {
      BugSenseLogResult bugSenseLogResult = new BugSenseLogResult();
      bugSenseLogResult.ResultState = BugSenseResultState.Undefined;
      bugSenseLogResult.Description = BugSenseHandlerBase.IsInitialized ? (string) null : "Splunk plugin is not initialized properly";
      BugSenseLogResult result = bugSenseLogResult;
      if (BugSenseHandlerBase.IsInitialized)
      {
        if (exception == null)
          throw new ArgumentNullException(nameof (exception), "Exception object parameter is null");
        LimitedCrashExtraDataList dataList = (LimitedCrashExtraDataList) null;
        if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
          dataList = new LimitedCrashExtraDataList()
          {
            new BugSense.Core.Model.CrashExtraData() { Key = key, Value = value }
          };
        result = await this.BugSenseWorker.LogExceptionAsync(exception, dataList);
      }
      return result;
    }

    public async Task<BugSenseLogResult> LogExceptionAsync(
      Exception exception,
      LimitedCrashExtraDataList extraData = null)
    {
      BugSenseLogResult bugSenseLogResult = new BugSenseLogResult();
      bugSenseLogResult.ResultState = BugSenseResultState.Undefined;
      bugSenseLogResult.Description = BugSenseHandlerBase.IsInitialized ? (string) null : "Splunk plugin is not initialized properly";
      BugSenseLogResult result = bugSenseLogResult;
      if (BugSenseHandlerBase.IsInitialized)
      {
        if (exception == null)
          throw new ArgumentNullException(nameof (exception), "Exception object parameter is null");
        result = await this.BugSenseWorker.LogExceptionAsync(exception, extraData);
      }
      return result;
    }

    public BugSenseLogResult LogException(Exception exception, string key, string value)
    {
      BugSenseLogResult bugSenseLogResult1 = new BugSenseLogResult();
      bugSenseLogResult1.ResultState = BugSenseResultState.Undefined;
      bugSenseLogResult1.Description = BugSenseHandlerBase.IsInitialized ? (string) null : "Splunk plugin is not initialized properly";
      BugSenseLogResult bugSenseLogResult2 = bugSenseLogResult1;
      if (BugSenseHandlerBase.IsInitialized)
      {
        if (exception == null)
          throw new ArgumentNullException(nameof (exception), "Exception object parameter is null");
        LimitedCrashExtraDataList extraData = (LimitedCrashExtraDataList) null;
        if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
          extraData = new LimitedCrashExtraDataList()
          {
            new BugSense.Core.Model.CrashExtraData() { Key = key, Value = value }
          };
        bugSenseLogResult2 = this.BugSenseWorker.LogException(exception, extraData);
      }
      return bugSenseLogResult2;
    }

    public BugSenseLogResult LogException(Exception exception, LimitedCrashExtraDataList extraData = null)
    {
      BugSenseLogResult bugSenseLogResult1 = new BugSenseLogResult();
      bugSenseLogResult1.ResultState = BugSenseResultState.Undefined;
      bugSenseLogResult1.Description = BugSenseHandlerBase.IsInitialized ? (string) null : "Splunk plugin is not initialized properly";
      BugSenseLogResult bugSenseLogResult2 = bugSenseLogResult1;
      if (BugSenseHandlerBase.IsInitialized)
      {
        if (exception == null)
          throw new ArgumentNullException(nameof (exception), "Exception object parameter is null");
        bugSenseLogResult2 = this.BugSenseWorker.LogException(exception, extraData);
      }
      return bugSenseLogResult2;
    }
  }
}
