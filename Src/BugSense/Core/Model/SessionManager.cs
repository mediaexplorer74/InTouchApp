// Decompiled with JetBrains decompiler
// Type: BugSense.Core.Model.SessionManager
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;


namespace BugSense.Core.Model
{
  internal class SessionManager
  {
    public DateTime PingSessionStart { get; set; }

    public DateTime SessionStart { get; set; }

    public static SessionManager Instance => SessionManager.Nested.instance;

    private class Nested
    {
      internal static readonly SessionManager instance = new SessionManager();
    }
  }
}
