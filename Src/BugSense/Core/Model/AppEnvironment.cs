// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.AppEnvironment
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace BugSense.Core.Model
{
  [DataContract]
  public class AppEnvironment : IEquatable<AppEnvironment>
  {
    private Guid Id { get; set; }

    [DataMember(Name = "uid", EmitDefaultValue = false)]
    public string Uid { get; set; }

    [DataMember(Name = "build_uuid", EmitDefaultValue = false)]
    public string BuildUid { get; set; }

    [DataMember(Name = "image_base_address", EmitDefaultValue = false)]
    public string ImageBaseAddress { get; set; }

    [DataMember(Name = "architecture", EmitDefaultValue = false)]
    public string Architecture { get; set; }

    [DataMember(Name = "phone", EmitDefaultValue = false)]
    public string PhoneModel { get; set; }

    [DataMember(Name = "manufacturer", EmitDefaultValue = false)]
    public string PhoneManufacturer { get; set; }

    [DataMember(Name = "internal_version", EmitDefaultValue = false)]
    public string InternalVersion { get; set; }

    [DataMember(Name = "appver", EmitDefaultValue = false)]
    public string AppVersion { get; set; }

    [DataMember(Name = "brand", EmitDefaultValue = false)]
    public string Brand { get; set; }

    [DataMember(Name = "appname", EmitDefaultValue = false)]
    public string AppName { get; set; }

    [DataMember(Name = "osver", EmitDefaultValue = false)]
    public string OsVersion { get; set; }

    [DataMember(Name = "wifi_on")]
    public int WifiOn { get; set; }

    [DataMember(Name = "gps_on")]
    public string GpsOn { get; set; }

    [DataMember(Name = "cellular_data")]
    public string CellularData { get; set; }

    [DataMember(Name = "carrier", EmitDefaultValue = false)]
    public string Carrier { get; set; }

    [DataMember(Name = "screen:width")]
    public double ScreenWidth { get; set; }

    [DataMember(Name = "screen:height")]
    public double ScreenHeight { get; set; }

    [DataMember(Name = "screen:orientation", EmitDefaultValue = false)]
    public string ScreenOrientation { get; set; }

    [DataMember(Name = "screen_dpi(x:y)", EmitDefaultValue = false)]
    public string ScreenDpi { get; set; }

    [DataMember(Name = "rooted")]
    public bool Rooted { get; set; }

    [DataMember(Name = "locale", EmitDefaultValue = false)]
    public string Locale { get; set; }

    [DataMember(Name = "geo_region", EmitDefaultValue = false)]
    public string GeoRegion { get; set; }

    [DataMember(Name = "cpu_model", EmitDefaultValue = false)]
    public string CpuModel { get; set; }

    [DataMember(Name = "cpu_bitness")]
    public int CpuBitness { get; set; }

    [DataMember(Name = "log_data")]
    public Dictionary<string, string> LogData { get; set; }

    public AppEnvironment()
    {
      this.Id = Guid.NewGuid();
      this.AppName = BugSenseProperties.AppName;
      this.AppVersion = BugSenseProperties.AppVersion;
      this.Brand = BugSenseProperties.PhoneBrand;
      this.OsVersion = BugSenseProperties.OSVersion;
      this.PhoneModel = BugSenseProperties.PhoneModel;
      this.Locale = BugSenseProperties.Locale;
      this.PhoneManufacturer = BugSenseProperties.PhoneBrand;
      this.ScreenHeight = BugSenseProperties.DeviceScreenProperties.Height;
      this.ScreenWidth = BugSenseProperties.DeviceScreenProperties.Width;
      this.ScreenDpi = string.Format("{0}:{1}", (object) BugSenseProperties.DeviceScreenProperties.Xdpi, (object) BugSenseProperties.DeviceScreenProperties.Ydpi);
      this.Uid = BugSenseProperties.UID;
      this.Architecture = BugSenseProperties.Architecture;
      this.Carrier = BugSenseProperties.Carrier;
      this.Rooted = BugSenseProperties.Rooted;
      this.CpuModel = "unknown";
      this.GeoRegion = "unknown";
      this.CpuBitness = 64;
      this.CellularData = BugSenseProperties.MobileNetOn.ToString();
      this.ScreenOrientation = BugSenseProperties.Orientation;
      this.WifiOn = BugSenseProperties.WIFIOn;
      this.GpsOn = BugSenseProperties.GPSOn.ToString();
    }

    public bool Equals(AppEnvironment other) => this.Id.Equals(other.Id);

    public override int GetHashCode() => this.Id.GetHashCode();
  }
}
