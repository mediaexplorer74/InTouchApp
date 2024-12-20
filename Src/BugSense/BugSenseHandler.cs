// BugSense.BugSenseHandler

using BugSense.Core;
using BugSense.Core.Helpers;
using BugSense.Core.Interfaces;
using BugSense.Core.Model;
using BugSense.Device.Specific;
using BugSense.Model;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;


namespace BugSense
{
  public class BugSenseHandler : BugSenseSyncContextHandler
  {
    private Application ApplicationContext { get; set; }

    public static BugSenseHandler Instance => BugSenseHandler.Nested.instance;

    protected BugSenseHandler()
      : base((IRequestWorker) new BugSenseRequestWorker((IBugSenseFileClient) new FileRepository(), (IBugSenseServiceClient) new ServiceRepository(), (IRequestJsonSerializer) new PublicRequestJsonSerializer(), (IDeviceUtil) new DeviceUtil(), (IContentResolver) new PublicContentResolver()))
    {
    }

    protected BugSenseHandler(IRequestWorker requestWorker)
      : base(requestWorker)
    {
    }

    public void InitAndStartSession(IExceptionManager exceptionManager, string apiKey)
    {
      if (BugSenseHandlerBase.IsInitialized)
        return;
      if (exceptionManager == null)
        throw new ArgumentNullException(nameof (exceptionManager), "IExceptionManager is Null!");
      this.InitAndStartSession(apiKey);
      if (exceptionManager is ExceptionManager exceptionManager1)
      {
        this.ApplicationContext = exceptionManager1.ApplicationContext != null
                    ? exceptionManager1.ApplicationContext
                    : throw new ArgumentNullException(nameof (exceptionManager), 
                    "Application context cannot be null!");
        Application applicationContext = this.ApplicationContext;
        
          //TODO
          //WindowsRuntimeMarshal.AddEventHandler<UnhandledExceptionEventHandler>(
          //  new Func<UnhandledExceptionEventHandler, EventRegistrationToken>(
          //      applicationContext.add_UnhandledException), 
          //  new Action<EventRegistrationToken>(applicationContext.remove_UnhandledException), 
          //  new UnhandledExceptionEventHandler(this.UnhandledExceptionsHandler));
      }
      else
        exceptionManager.UnhandledException += new UnhandledExceptionEventHandler(
            this.UnhandledExceptionsHandler);

      BugSenseProperties.UserAgent = "WP";
      BugSenseHandlerBase.IsInitialized = true;
    }

    private void UnhandledExceptionsHandler(object sender, UnhandledExceptionEventArgs e)
    {
      if (e != null)
      {
        BugSenseLogResult bugSenseLogResult = this.BugSenseWorker.HandleException(e.Exception, ExtraData.CrashExtraData);
        this.OnUnhandledExceptionHandled((object) this, new BugSenseUnhandledHandlerEventArgs()
        {
          ClientJsonRequest = bugSenseLogResult.ClientRequest,
          ExceptionObject = e.Exception,
          HandledSuccessfully = bugSenseLogResult.ResultState.Equals((object) BugSenseResultState.OK)
        });
      }
      else
        this.OnUnhandledExceptionHandled((object) this, new BugSenseUnhandledHandlerEventArgs()
        {
          ClientJsonRequest = "Mock UnhandledException",
          ExceptionObject = new Exception("Mock UnhandledException"),
          HandledSuccessfully = true
        });
    }

    private class Nested
    {
      internal static readonly BugSenseHandler instance = new BugSenseHandler();
    }
  }
}
