// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.SourceControl.SourceControlProjectModel
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.Expression.Framework.SourceControl
{
  public class SourceControlProjectModel : SourceControlProcessModel
  {
    private string[] registeredServers;

    public string ServerName { get; private set; }

    public string ServerPath { get; private set; }

    public string LocalPath { get; private set; }

    public SourceControlProjectModel(string serverName, string serverPath, string localPath)
      : base("", localPath)
    {
      this.ServerName = serverName;
      this.ServerPath = serverPath;
      this.LocalPath = localPath;
    }

    public string[] GetRegisteredServers()
    {
      return this.registeredServers;
    }

    public void SetRegisteredServers(string[] serverList)
    {
      this.registeredServers = serverList;
    }
  }
}
