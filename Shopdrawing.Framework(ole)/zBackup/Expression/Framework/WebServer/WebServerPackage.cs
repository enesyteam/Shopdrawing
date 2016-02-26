// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.WebServer.WebServerPackage
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;

namespace Microsoft.Expression.Framework.WebServer
{
  public class WebServerPackage : IPackage
  {
    private IServices services;

    public void Load(IServices services)
    {
      this.services = services;
      this.services.AddService(typeof (IWebServerService), (object) new WebServerService(this.services));
    }

    public void Unload()
    {
      ((IWebServerService) this.services.GetService(typeof (IWebServerService))).StopAllSessions();
      this.services.RemoveService(typeof (IWebServerService));
    }
  }
}
