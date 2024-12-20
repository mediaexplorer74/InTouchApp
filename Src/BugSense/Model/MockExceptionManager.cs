// Decompiled with JetBrains decompiler
// Type: BugSense.Model.MockExceptionManager
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using Windows.UI.Xaml;


namespace BugSense.Model
{
  public class MockExceptionManager : IExceptionManager
  {
    public event UnhandledExceptionEventHandler UnhandledException = (param0, param1) => { };

    public void MockUnhandledException(object sender)
    {
      this.UnhandledException(sender, (UnhandledExceptionEventArgs) null);
    }
  }
}
