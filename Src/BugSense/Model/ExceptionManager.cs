// Decompiled with JetBrains decompiler
// Type: BugSense.Model.ExceptionManager
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;


namespace BugSense.Model
{
  public class ExceptionManager : IExceptionManager
  {
    public Application ApplicationContext { get; set; }

    public ExceptionManager(Application app) => this.ApplicationContext = app;

    public event UnhandledExceptionEventHandler UnhandledException
    {
      add
      {
        Application applicationContext = this.ApplicationContext;

                //TODO
        //WindowsRuntimeMarshal.AddEventHandler<UnhandledExceptionEventHandler>(new Func<UnhandledExceptionEventHandler, EventRegistrationToken>(applicationContext.add_UnhandledException), new Action<EventRegistrationToken>(applicationContext.remove_UnhandledException), value);
      }
      remove
      {
                //TODO
        //WindowsRuntimeMarshal.RemoveEventHandler<UnhandledExceptionEventHandler>(new Action<EventRegistrationToken>(this.ApplicationContext.remove_UnhandledException), value);
      }
    }
  }
}
