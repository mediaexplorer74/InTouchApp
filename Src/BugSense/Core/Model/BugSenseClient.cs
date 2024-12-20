// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.BugSenseClient
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;
using System.Runtime.Serialization;


namespace BugSense.Core.Model
{
  [DataContract]
  public class BugSenseClient : IEquatable<BugSenseClient>
  {
    private Guid Id { get; set; }

    [DataMember(Name = "version", EmitDefaultValue = false)]
    public string Version { get; set; }

    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Name = "flavor", EmitDefaultValue = false)]
    public string Flavor { get; set; }

    public BugSenseClient()
    {
      this.Id = Guid.NewGuid();
      this.Version = "bugsense-version-" + BugSenseProperties.BugSenseVersion;
      this.Name = BugSenseProperties.BugSenseName;
      this.Flavor = BugSenseProperties.Flavor;
    }

    public bool Equals(BugSenseClient other) => this.Id.Equals(other.Id);

    public override int GetHashCode() => this.Id.GetHashCode();
  }
}
