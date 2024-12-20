// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.BugSenseException
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using BugSense.Core.Helpers;
using System;
using System.Runtime.Serialization;
using System.Text;


namespace BugSense.Core.Model
{
  [DataContract]
  public class BugSenseException : IEquatable<BugSenseException>
  {
    private string _msFromstart;

    private Guid Id { get; set; }

    internal Exception OriginalException { get; set; }

    [DataMember(Name = "message", EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Name = "backtrace", EmitDefaultValue = false)]
    public string StackTrace { get; set; }

    [DataMember(Name = "occured_at", EmitDefaultValue = false)]
    public DateTime DateOccured { get; set; }

    [DataMember(Name = "klass", EmitDefaultValue = false)]
    public string ClassName { get; set; }

    [DataMember(Name = "where", EmitDefaultValue = false)]
    public string Where { get; set; }

    [DataMember(Name = "tags", EmitDefaultValue = false)]
    public string Tag { get; set; }

    [DataMember(Name = "breadcrumbs", EmitDefaultValue = false)]
    public string Breadcrumbs { get; set; }

    [DataMember(Name = "handled")]
    public int Handled { get; set; }

    [DataMember(Name = "error_hash", EmitDefaultValue = false)]
    public string ErrorHash { get; set; }

    [DataMember(Name = "timestamp", EmitDefaultValue = false)]
    public string TimeStamp { get; set; }

    [DataMember(Name = "ms_from_start", EmitDefaultValue = false)]
    public string MsFromStart
    {
      get
      {
        return this._msFromstart = DateTime.UtcNow.Subtract(SessionManager.Instance.SessionStart).Milliseconds.ToString();
      }
      set => this._msFromstart = value;
    }

    public BugSenseException() => this.Id = Guid.NewGuid();

    public bool Equals(BugSenseException other) => this.Id.Equals(other.Id);

    public override int GetHashCode() => this.Id.GetHashCode();

    public static BugSenseException GetInstance(Exception ex, bool handled)
    {
      BugSenseException instance = new BugSenseException()
      {
        ClassName = ex.GetType().FullName,
        DateOccured = DateTime.UtcNow,
        Name = ex.Message,
        OriginalException = ex,
        Handled = Convert.ToInt32(handled),
        Breadcrumbs = ExtraData.BreadCrumbs.ToString(),
        StackTrace = StacktraceHelper.GetStackTrace(ex).Trim(),
        TimeStamp = DateTime.UtcNow.DateTimeToUnixTimestamp().ToString()
      };
      HashSignature hashSignature = new ErrorHashSignature().GetHashSignature(BugSenseProperties.AppName, BugSenseProperties.AppVersion, ex.StackTrace, ex.Message, ex.HResultEx());
      instance.ErrorHash = hashSignature.Signature.ToLower();
      instance.Where = hashSignature.Where;
      if (hashSignature.Tags.Count > 1)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string tag in hashSignature.Tags)
          stringBuilder.Append(tag + ",");
        instance.Tag = stringBuilder.ToString();
      }
      else if (hashSignature.Tags.Count == 1)
        instance.Tag = hashSignature.Tags[0];
      return instance;
    }
  }
}
