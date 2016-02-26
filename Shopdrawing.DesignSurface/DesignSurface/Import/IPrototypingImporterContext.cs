// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Import.IPrototypingImporterContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design.Model;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Import
{
  public interface IPrototypingImporterContext : IModelItemImporterContext, IImporterContext
  {
    Point NextOpenGraphPosition();

    bool CreatePrototypingScreen(string name, Point position);

    ModelItem GetRootForPrototypingScreen(string name);

    bool ConnectPrototypingScreens(string fromName, string toName);

    bool ConnectFromStartScreen(string toName);

    bool ScreenExists(string name);

    void CloseScreen(string name);
  }
}
