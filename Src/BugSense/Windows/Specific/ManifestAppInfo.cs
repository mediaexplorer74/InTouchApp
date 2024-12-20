// Decompiled with JetBrains decompiler
// Type: BugSense.Windows.Specific.ManifestAppInfo
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;


namespace BugSense.Windows.Specific
{
  internal class ManifestAppInfo
  {
    private static Dictionary<string, string> _properties;

    private static Dictionary<string, string> Properties
    {
      get
      {
        if (ManifestAppInfo._properties == null)
        {
          ManifestAppInfo._properties = new Dictionary<string, string>();
          using (XmlReader reader = XDocument.Load("WMAppManifest.xml").CreateReader(ReaderOptions.None))
          {
            reader.ReadToDescendant("App");
            if (!reader.IsStartElement())
              throw new FormatException("App tag not found in WMAppManifest.xml ");
            reader.MoveToFirstAttribute();
            while (reader.MoveToNextAttribute())
              ManifestAppInfo._properties.Add(reader.Name, reader.Value);
          }
        }
        return ManifestAppInfo._properties;
      }
    }

    public string Version => ManifestAppInfo.Properties[nameof (Version)];

    public string ProductId => ManifestAppInfo.Properties["ProductID"];

    public string Title => ManifestAppInfo.Properties[nameof (Title)];

    public string TitleUc
    {
      get => string.IsNullOrEmpty(this.Title) ? (string) null : this.Title.ToUpperInvariant();
    }

    public string Genre => ManifestAppInfo.Properties[nameof (Genre)];

    public string Description => ManifestAppInfo.Properties[nameof (Description)];

    public string Publisher => ManifestAppInfo.Properties[nameof (Publisher)];
  }
}
