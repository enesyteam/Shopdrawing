// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.WebServer.WebServerSettings
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.Expression.Framework.WebServer
{
  public class WebServerSettings : IWebServerSettings
  {
    public const int AutoSelectPort = 0;

    public bool ShowTrayIcon { get; set; }

    public string LocalPath { get; set; }

    public string VirtualPath { get; set; }

    public string PhpServerExe { get; set; }

    public bool Silent { get; set; }

    public int Port { get; set; }

    public bool UseNtlmAuthentication { get; set; }

    public bool ShowDirectoryListing { get; set; }

    public WebServerSettings(string localPath)
    {
      this.LocalPath = localPath;
      this.ShowTrayIcon = true;
      this.Port = 0;
      this.Silent = false;
      this.UseNtlmAuthentication = false;
      this.ShowDirectoryListing = true;
      this.VirtualPath = "/";
    }
  }
}
