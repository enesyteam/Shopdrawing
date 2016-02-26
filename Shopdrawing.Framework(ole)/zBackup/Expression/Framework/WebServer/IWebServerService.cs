// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.WebServer.IWebServerService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.Expression.Framework.WebServer
{
  public interface IWebServerService
  {
    int StartServer(IWebServerSettings settings);

    string GetSessionAddress(int handle);

    string GetSessionAddress(int handle, string localPath);

    bool IsServerReachable(int handle, int timeout);

    void StopBrowsingSession(int handle);

    void StopAllSessions();
  }
}
