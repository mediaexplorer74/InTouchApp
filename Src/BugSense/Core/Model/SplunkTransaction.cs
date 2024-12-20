// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.SplunkTransaction
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System.Diagnostics;


namespace BugSense.Core.Model
{
  internal class SplunkTransaction
  {
    private Stopwatch TimerStopWatch { get; set; }

    public double Elapsed
    {
      get
      {
        this.TimerStopWatch.Stop();
        return (double) this.TimerStopWatch.ElapsedMilliseconds;
      }
    }

    public string TransactionId { get; set; }

    public TrStart TransactionStart { get; set; }

    public TrStop TransactionStop { get; set; }

    public SplunkTransaction()
    {
      this.TimerStopWatch = new Stopwatch();
      this.TimerStopWatch.Start();
    }
  }
}
