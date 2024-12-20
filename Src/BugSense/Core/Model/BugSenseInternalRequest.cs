// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.BugSenseInternalRequest
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;
using System.Runtime.Serialization;


namespace BugSense.Core.Model
{
  [DataContract]
  public class BugSenseInternalRequest : IEquatable<BugSenseInternalRequest>
  {
    private Guid Id { get; set; }

    [DataMember(Name = "comment", EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember(Name = "user_id", EmitDefaultValue = false)]
    public string UserIdentifier { get; set; }

    public BugSenseInternalRequest()
    {
      this.Id = Guid.NewGuid();
      this.UserIdentifier = string.IsNullOrWhiteSpace(BugSenseProperties.UserIdentifier) ? string.Empty : BugSenseProperties.UserIdentifier;
      this.Comment = string.Empty;
    }

    public bool Equals(BugSenseInternalRequest other) => this.Id.Equals(other.Id);

    public override int GetHashCode() => this.Id.GetHashCode();
  }
}
