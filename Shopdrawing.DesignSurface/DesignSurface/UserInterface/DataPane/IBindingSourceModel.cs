// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.IBindingSourceModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public interface IBindingSourceModel : INotifyPropertyChanged
  {
    string DisplayName { get; }

    string AutomationName { get; }

    bool IsEnabled { get; }

    ISchema Schema { get; }

    SchemaItem SchemaItem { get; }

    string Path { get; }

    string PathDescription { get; }

    SceneNode CreateBindingOrData(SceneViewModel viewModel, SceneNode targetNode, IProperty targetProperty);

    SceneNode CreateBindingOrData(SceneViewModel viewModel, string bindingPath, SceneNode targetNode, IProperty targetProperty);

    void Unhook();
  }
}
