// Decompiled with JetBrains decompiler
// Type: BugSense.Core.BugSenseSyncContextHandler
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Interfaces;
using BugSense.Core.Model;
using Splunk.Mi.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace BugSense.Core
{
  public abstract class BugSenseSyncContextHandler : BugSenseHandlerBase
  {
    private static EventHandler<SyncContextExceptionEventArgs> SynchronizationContextExceptionCaught = (EventHandler<SyncContextExceptionEventArgs>) ((param0, param1) => { });

    internal BugSenseSyncContextHandler(IRequestWorker worker)
      : base(worker)
    {
      BugSenseSyncContextHandler.SynchronizationContextExceptionCaught += new EventHandler<SyncContextExceptionEventArgs>(this.SyncContextExceptionHandler);
      TaskScheduler.UnobservedTaskException += new EventHandler<UnobservedTaskExceptionEventArgs>(this.UnobservedTaskExceptionsHandler);
    }

    protected override void InitAndStartSession(string apiKey)
    {
      base.InitAndStartSession(apiKey);
      BugSenseSyncContextHandler.AsyncSynchronizationContext.Register();
    }

    public void RegisterAsyncHandlerContext()
    {
      BugSenseSyncContextHandler.AsyncSynchronizationContext.Register();
    }

    public void RegisterUnobservedTaskExceptions()
    {
      TaskScheduler.UnobservedTaskException += new EventHandler<UnobservedTaskExceptionEventArgs>(this.UnobservedTaskExceptionsHandler);
    }

    public void UnregisterUnobservedTaskExceptions()
    {
      TaskScheduler.UnobservedTaskException -= new EventHandler<UnobservedTaskExceptionEventArgs>(this.UnobservedTaskExceptionsHandler);
    }

    private async void SyncContextExceptionHandler(object sender, SyncContextExceptionEventArgs e)
    {
      BugSenseLogResult logResult = await this.LogUnobservedUnawaitedExceptionAsync(e.SyncContextException).ConfigureAwait(false);
      this.OnUnhandledSyncExceptionHandled(e.SyncContextException, logResult);
    }

    private async void UnobservedTaskExceptionsHandler(
      object sender,
      UnobservedTaskExceptionEventArgs e)
    {
      BugSenseLogResult logResult = await this.LogUnobservedUnawaitedExceptionAsync((Exception) e.Exception).ConfigureAwait(false);
      this.OnUnhandledSyncExceptionHandled((Exception) e.Exception, logResult);
    }

    private async Task<BugSenseLogResult> LogUnobservedUnawaitedExceptionAsync(Exception exception)
    {
      return await this.BugSenseWorker.HandleExceptionAsync(exception, ExtraData.CrashExtraData);
    }

    private void OnUnhandledSyncExceptionHandled(Exception exception, BugSenseLogResult logResult)
    {
      this.OnUnhandledExceptionHandled((object) this, new BugSenseUnhandledHandlerEventArgs()
      {
        ClientJsonRequest = logResult.ClientRequest,
        ExceptionObject = exception,
        HandledSuccessfully = logResult.ResultState == BugSenseResultState.OK
      });
    }

    internal class AsyncSynchronizationContext : SynchronizationContext
    {
      private readonly SynchronizationContext _syncContext;

      //public static SynchronizationContext syncContext;

      public static BugSenseSyncContextHandler.AsyncSynchronizationContext Register()
      {
        SynchronizationContext current = SynchronizationContext.Current;
        BugSenseSyncContextHandler.AsyncSynchronizationContext syncContext = null;
        switch (current)
        {
          case null:
            //TODO
            return syncContext;
          default:
            syncContext = new BugSenseSyncContextHandler.AsyncSynchronizationContext(current);
            try
            {
              SynchronizationContext.SetSynchronizationContext(
                  (SynchronizationContext) syncContext);
              goto case null;
            }
            catch (Exception ex)
            {
              ConsoleManager.LogToConsole(string.Format(
                  "SetSynchronizationContext Exception: {0}", (object) ex));
              goto case null;
            }
        }
      }

      public AsyncSynchronizationContext(SynchronizationContext syncContext)
      {
        this._syncContext = syncContext;
      }

      public override SynchronizationContext CreateCopy()
      {
        return (SynchronizationContext) 
                    new BugSenseSyncContextHandler.AsyncSynchronizationContext(
                        this._syncContext.CreateCopy());
      }

      public override void OperationCompleted() => this._syncContext.OperationCompleted();

      public override void OperationStarted() => this._syncContext.OperationStarted();

      public override void Post(SendOrPostCallback d, object state)
      {
        this._syncContext.Post(
            BugSenseSyncContextHandler.AsyncSynchronizationContext.WrapCallback(d), state);
      }

      public override void Send(SendOrPostCallback d, object state)
      {
        this._syncContext.Send(d, state);
      }

      private static SendOrPostCallback WrapCallback(SendOrPostCallback sendOrPostCallback)
      {
        return (SendOrPostCallback) (state =>
        {
          Exception exception = (Exception) null;
          try
          {
            sendOrPostCallback(state);
          }
          catch (Exception ex)
          {
            exception = ex;
          }
          if (exception == null)
            return;

          BugSenseSyncContextHandler.SynchronizationContextExceptionCaught(
              (object) null, new SyncContextExceptionEventArgs()
          {
            SyncContextException = exception
          });
        });
      }
    }
  }
}
