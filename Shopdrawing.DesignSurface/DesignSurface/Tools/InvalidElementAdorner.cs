// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.InvalidElementAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.SceneCommands.ModalCommands;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class InvalidElementAdorner : ErrorAdorner
  {
    private static readonly Brush BorderBrush = ErrorAdorner.CreateErrorAdornerBrush(Colors.Red);
    private static readonly DrawingImage ContentDrawing = FileTable.GetDrawingImage("Resources\\Adorners\\InvalidElementAdorner.xaml");
    private List<SceneElement> targets;
    private List<Exception> exceptions;

    internal IList<SceneElement> TargetElements
    {
      get
      {
        return (IList<SceneElement>) this.targets;
      }
    }

    public override object ToolTip
    {
      get
      {
        SceneViewModel viewModel = this.Element.ViewModel;
        SceneView defaultView = viewModel.DefaultView;
        Exception exception = defaultView.GetException(this.Element.DocumentNodePath);
        System.Windows.Controls.ToolTip containingToolTip = (System.Windows.Controls.ToolTip) null;
        if (exception != null)
        {
          containingToolTip = new System.Windows.Controls.ToolTip();
          containingToolTip.StaysOpen = false;
          DocumentNode exceptionTarget = defaultView.GetExceptionTarget(this.Element.DocumentNodePath);
          string exceptionMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.InvalidElementDescription, (object) this.Element.DisplayName, (object) exceptionTarget.TargetType, (object) exception.Message);
          CommandListToolTipContent listToolTipContent = new CommandListToolTipContent(viewModel, containingToolTip, exceptionMessage);
          listToolTipContent.AddAction((ModalCommandBase) new UndoCommand(viewModel));
          listToolTipContent.AddAction((ModalCommandBase) new ViewExceptionCommand(exception, viewModel));
          listToolTipContent.AddAction((ModalCommandBase) new ShowXamlCommand(viewModel, exceptionTarget));
          listToolTipContent.AddAction((ModalCommandBase) new DeleteElementCommand(this.Element, viewModel));
          containingToolTip.Content = (object) listToolTipContent;
        }
        return (object) containingToolTip;
      }
    }

    static InvalidElementAdorner()
    {
      InvalidElementAdorner.ContentDrawing.Freeze();
    }

    public InvalidElementAdorner(AdornerSet adornerSet)
      : base(adornerSet)
    {
      this.targets = new List<SceneElement>();
      this.exceptions = new List<Exception>();
      SceneView defaultView = this.Element.ViewModel.DefaultView;
      foreach (DocumentNodePath invalidElement in (IEnumerable<DocumentNodePath>) defaultView.GenerateVisiblePaths(this.Element.DocumentNode))
      {
        Exception exception = defaultView.GetException(invalidElement);
        if (exception != null)
        {
          SceneElement sceneElement = invalidElement.Node.SceneNode as SceneElement;
          if (sceneElement != null && defaultView.IsInArtboard(this.Element))
          {
            this.targets.Add(sceneElement);
            this.exceptions.Add(exception);
          }
        }
      }
    }

    public override void Draw(DrawingContext context, Matrix matrix)
    {
      for (int index = 0; index < this.targets.Count; ++index)
      {
        SceneElement adornedElement = this.targets[index];
        Matrix transformToArtboard = adornedElement.TransformToArtboard;
        this.DrawAdorner(context, transformToArtboard, adornedElement, InvalidElementAdorner.BorderBrush, InvalidElementAdorner.ContentDrawing, this.exceptions[index].Message);
      }
    }
  }
}
