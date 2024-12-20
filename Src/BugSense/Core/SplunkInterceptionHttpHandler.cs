// Decompiled with JetBrains decompiler
// Type: BugSense.Core.SplunkInterceptionHttpHandler
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Helpers;
using BugSense.Core.Model;
using BugSense.Device.Specific;
using Splunk.Mi.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;


namespace BugSense.Core
{
  public class SplunkInterceptionHttpHandler : DelegatingHandler
  {
    private readonly IDeviceUtil _deviceUtil = (IDeviceUtil) new DeviceUtil();
    private readonly IBugSenseFileClient _fileClient = (IBugSenseFileClient) new FileRepository();

    public event EventHandler<NetworkDataFixture> NetworkDataLogged = (param0, param1) => { };

    protected virtual void OnNetworkDataLogged(NetworkDataFixture e)
    {
      EventHandler<NetworkDataFixture> networkDataLogged = this.NetworkDataLogged;
      if (networkDataLogged == null)
        return;
      networkDataLogged((object) this, e);
    }

    public SplunkInterceptionHttpHandler(HttpMessageHandler messageHandler)
      : base(messageHandler)
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      NetworkDataFixture networkDataFixture = NetworkDataFixture.GetNetworkDataFixture(this._deviceUtil.GetAppEnvironment());
      networkDataFixture.Url = request.RequestUri.ToString();
      byte[] contentBytes = await request.Content.ReadAsByteArrayAsync();
      networkDataFixture.ContentLength = (double) contentBytes.Length;
      foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) request.Headers)
        networkDataFixture.Headers.Add(header.Key, header.Value.FirstOrDefault<string>());
      ConsoleManager.LogToConsole(string.Format("Intercepting call!"));
      ConsoleManager.LogToConsole(string.Format("URL: {0}", (object) request.RequestUri));
      ConsoleManager.LogToConsole(string.Format("HTTP Method: {0}", (object) request.Method));
      byte[] sendBytes = await request.Content.ReadAsByteArrayAsync();
      ConsoleManager.LogToConsole(string.Format("Bytes to send: {0}", (object) sendBytes.Length));
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      HttpResponseMessage responseMessage = await base.SendAsync(request, cancellationToken);
      stopwatch.Stop();
      networkDataFixture.StatusCode = (int) responseMessage.StatusCode;
      networkDataFixture.Failed = !responseMessage.IsSuccessStatusCode;
      networkDataFixture.Duration = stopwatch.ElapsedMilliseconds;
      ConsoleManager.LogToConsole(string.Format("Response Code: {0}", (object) responseMessage.StatusCode));
      byte[] receivedBytes = await responseMessage.Content.ReadAsByteArrayAsync();
      ConsoleManager.LogToConsole(string.Format("Latency time in millis: {0}", (object) stopwatch.ElapsedMilliseconds));
      ConsoleManager.LogToConsole(string.Format("Bytes received: {0}", (object) receivedBytes.Length));
      BugSenseLogResult logResult = await this.SaveNetworkActionAsync(networkDataFixture.SerializeToJson<NetworkDataFixture>());
      if (logResult.ResultState == BugSenseResultState.OK && RemoteSettingsData.Instance.NetMonitoring)
        this.OnNetworkDataLogged(networkDataFixture);
      return responseMessage;
    }

    private Task<BugSenseLogResult> SaveNetworkActionAsync(string networkData)
    {
      return Task.Run<BugSenseLogResult>((Func<Task<BugSenseLogResult>>) (async () =>
      {
        BugSenseLogResult logResult = new BugSenseLogResult()
        {
          ResultState = BugSenseResultState.Undefined
        };
        if (RemoteSettingsData.Instance.NetMonitoring)
        {
          string fileName = CommonHelper.NewFileNamePath(FileNameType.NetworkAction);
          logResult = await this._fileClient.SaveAsync(fileName, networkData);
        }
        return logResult;
      }));
    }
  }
}
