// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.NameInteractiveElementsCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class NameInteractiveElementsCommand : SceneCommandBase
  {
    public override bool IsEnabled
    {
      get
      {
        if (this.IsAvailable && this.SceneViewModel.Document.IsEditable)
          return this.SceneViewModel.RootNode != null;
        return false;
      }
    }

    public NameInteractiveElementsCommand(SceneViewModel viewModel)
      : base(viewModel)
    {
    }

    public override void Execute()
    {
      Dictionary<IType, bool> dictionary = new Dictionary<IType, bool>();
      foreach (ITypeId typeId in DefaultTypeInstantiator.InteractiveElementTypes)
      {
        IType key = this.SceneViewModel.ProjectContext.ResolveType(typeId);
        if (!this.SceneViewModel.ProjectContext.PlatformMetadata.IsNullType((ITypeId) key))
          dictionary.Add(key, true);
      }
      List<SceneNode> list = new List<SceneNode>();
      foreach (DocumentNode node in this.SceneViewModel.RootNode.DocumentNode.DescendantNodes)
      {
        for (IType key = node.Type; key != null; key = key.BaseType)
        {
          if (dictionary.ContainsKey(key))
          {
            SceneNode sceneNode = this.SceneViewModel.GetSceneNode(node);
            if (string.IsNullOrEmpty(sceneNode.Name))
            {
              list.Add(sceneNode);
              break;
            }
            break;
          }
        }
      }
      if (list.Count > 0)
      {
        list.Sort((Comparison<SceneNode>) ((a, b) =>
        {
          INodeSourceContext sourceContext1 = a.DocumentNode.SourceContext;
          INodeSourceContext sourceContext2 = b.DocumentNode.SourceContext;
          if (sourceContext1 == null || sourceContext2 == null)
            return a.DocumentNode.Marker.CompareTo((object) b.DocumentNode.Marker);
          return sourceContext1.TextRange.Offset.CompareTo(sourceContext2.TextRange.Offset);
        }));
        using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(StringTable.UndoNameInteractiveElements))
        {
          foreach (SceneNode sceneNode in list)
            sceneNode.EnsureNamed();
          editTransaction.Commit();
        }
      }
      if (this.DesignerContext.MessageDisplayService == null)
        return;
      string message;
      if (list.Count > 0)
        message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.InteractiveElementsNamedSome, new object[1]
        {
          (object) list.Count
        });
      else
        message = StringTable.InteractiveElementsNamedNone;
      int num = (int) this.DesignerContext.MessageDisplayService.ShowMessage(message);
    }
  }
}
