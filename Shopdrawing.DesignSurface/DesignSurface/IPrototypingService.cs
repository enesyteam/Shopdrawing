// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.IPrototypingService
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface
{
  public interface IPrototypingService
  {
    IPrototypingScreen ActiveScreen { get; }

    event EventHandler<ScreenTypeChangedEventArgs> ScreenTypeChanged;

    IEnumerable<IPrototypingScreen> FindNavigableScreens(IPrototypingScreen sourceScreen);

    IEnumerable<IPrototypingScreen> FindComposedScreens(IPrototypingScreen sourceScreen);

    bool PromoteToCompositionScreen(IProjectItem projectItem);

    void ProcessElementBeforeInsertion(ISceneInsertionPoint insertionPoint, SceneElement element);

    PrototypeScreenType GetScreenType(IType type);

    Point NextOpenGraphPosition();

    bool AddScreen(string displayName, Point position);

    SceneViewModel GetSceneViewModelForScreen(string displayName);

    bool ConnectPrototypingScreens(string fromName, string toName);

    bool ConnectFromStartScreen(string toName);

    bool ScreenExists(string name);

    void CloseScreen(string name);

    void AddElementContextMenuItems(ICommandBar menu, IEnumerable<SceneElement> elements, SceneViewModel viewModel, bool isOnArtboard);
  }
}
