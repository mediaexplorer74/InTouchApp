// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.BugSenseExceptionRequest
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


namespace BugSense.Core.Model
{
  [DataContract]
  public class BugSenseExceptionRequest : IEquatable<BugSenseExceptionRequest>
  {
    private Guid Id { get; set; }

    public Dictionary<string, string> LogData { get; set; }

    [DataMember(Name = "exception", EmitDefaultValue = false)]
    public BugSenseException Exception { get; set; }

    [DataMember(Name = "application_environment", EmitDefaultValue = false)]
    public AppEnvironment AppEnvironment { get; set; }

    [DataMember(Name = "client", EmitDefaultValue = false)]
    public BugSenseClient Client { get; set; }

    [DataMember(Name = "performance", EmitDefaultValue = false)]
    public BugSensePerformance Performance { get; set; }

    [DataMember(Name = "request", EmitDefaultValue = false)]
    public BugSenseInternalRequest Request { get; set; }

    public BugSenseExceptionRequest() => this.Id = Guid.NewGuid();

    public BugSenseExceptionRequest(
      BugSenseException ex,
      AppEnvironment environment,
      BugSensePerformance performance,
      LimitedCrashExtraDataList extraData)
    {
      this.Id = Guid.NewGuid();
      this.Client = new BugSenseClient();
      this.Request = new BugSenseInternalRequest();
      this.Exception = ex;
      this.AppEnvironment = environment;
      this.Performance = performance;
      if (extraData != null && extraData.Count > 0)
      {
        this.LogData = new Dictionary<string, string>(extraData.Count);
        Dictionary<string, string>.KeyCollection keys = this.LogData.Keys;
        foreach (CrashExtraData crashExtraData in extraData)
          this.AddCrashExtraDataToDictionary(crashExtraData);
      }
      else
      {
        this.LogData = new Dictionary<string, string>();
        Dictionary<string, string>.KeyCollection keys = this.LogData.Keys;
      }
      this.AppEnvironment.LogData = this.LogData;
    }

    private void AddCrashExtraDataToDictionary(CrashExtraData crashExtraData)
    {
      if (string.IsNullOrWhiteSpace(this.LogData.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (x => x.Key.Equals(crashExtraData.Key))).Key))
        this.LogData.Add(crashExtraData.Key, crashExtraData.Value);
      else
        this.LogData[crashExtraData.Key] = crashExtraData.Value;
    }

    public bool Equals(BugSenseExceptionRequest other) => this.Id.Equals(other.Id);

    public override int GetHashCode() => this.Id.GetHashCode();
  }
}
