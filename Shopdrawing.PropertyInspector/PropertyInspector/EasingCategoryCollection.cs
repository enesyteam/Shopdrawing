// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.EasingCategoryCollection
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class EasingCategoryCollection : SceneNodeCategory
  {
    private SceneNode[] selectedObjects;

    public bool IsEasingFunctionAvailable
    {
      get
      {
        if (this.selectedObjects.Length <= 0)
          return false;
        SceneNode sceneNode = this.selectedObjects[0];
        return sceneNode.ProjectContext.Platform.Metadata.IsSupported((ITypeResolver) sceneNode.ProjectContext, PlatformTypes.IEasingFunction);
      }
    }

    public bool IsEasingFunctionSelection
    {
      get
      {
        if (this.BasicProperties.Count == 1)
        {
          SceneNode sceneNode = this.selectedObjects[0];
          IType type = sceneNode.ProjectContext.ResolveType(PlatformTypes.IEasingFunction);
          if (type != null && !sceneNode.ProjectContext.PlatformMetadata.IsNullType((ITypeId) type))
            return type.RuntimeType.IsAssignableFrom(this.BasicProperties[0].get_PropertyType());
        }
        return false;
      }
    }

    public bool IsKeySplineSelection
    {
      get
      {
        if (this.BasicProperties.Count == 1)
        {
          SceneNode sceneNode = this.selectedObjects[0];
          IType type = sceneNode.ProjectContext.ResolveType(PlatformTypes.KeySpline);
          if (!sceneNode.ProjectContext.PlatformMetadata.IsNullType((ITypeId) type))
            return type.RuntimeType.IsAssignableFrom(this.BasicProperties[0].get_PropertyType());
        }
        return false;
      }
    }

    public bool IsDiscreteSelection
    {
      get
      {
        if (this.BasicProperties.Count != 0 || this.selectedObjects.Length <= 0)
          return false;
        bool flag = true;
        foreach (SceneNode sceneNode in this.selectedObjects)
        {
          KeyFrameSceneNode keyFrameSceneNode = sceneNode as KeyFrameSceneNode;
          if (keyFrameSceneNode == null || keyFrameSceneNode.InterpolationType != KeyFrameInterpolationType.Discrete)
          {
            flag = false;
            break;
          }
        }
        return flag;
      }
    }

    public override bool IsEmpty
    {
      get
      {
        if (this.selectedObjects != null && this.selectedObjects.Length != 0)
          return !(this.selectedObjects[0] is KeyFrameSceneNode);
        return true;
      }
    }

    public ICommand ConvertToSplineCommand
    {
      get
      {
        return (ICommand) new EasingCategoryCollection.ConvertEasingTypeCommand(this, KeyFrameInterpolationType.Spline);
      }
    }

    public ICommand ConvertToEasingCommand
    {
      get
      {
        return (ICommand) new EasingCategoryCollection.ConvertEasingTypeCommand(this, KeyFrameInterpolationType.Easing);
      }
    }

    public ICommand ConvertToDiscreteCommand
    {
      get
      {
        return (ICommand) new EasingCategoryCollection.ConvertEasingTypeCommand(this, KeyFrameInterpolationType.Discrete);
      }
    }

    public EasingCategoryCollection(string localizedName, IMessageLoggingService messageLogger)
      : base(CategoryLocalizationHelper.CategoryName.Easing, localizedName, messageLogger)
    {
    }

    public override void OnSelectionChanged(SceneNode[] selectedObjects)
    {
      base.OnSelectionChanged(selectedObjects);
      this.selectedObjects = selectedObjects;
      this.UpdateUI();
    }

    private void UpdateUI()
    {
      this.OnPropertyChanged("IsEasingFunctionSelection");
      this.OnPropertyChanged("IsKeySplineSelection");
      this.OnPropertyChanged("IsDiscreteSelection");
      this.OnPropertyChanged("IsEasingFunctionAvailable");
    }

    public override void ApplyFilter(PropertyFilter filter)
    {
      base.ApplyFilter(filter);
      EasingCategoryCollection categoryCollection1 = this;
      int num1 = categoryCollection1.BasicPropertyMatchesFilter | this.get_MatchesFilter() ? 1 : 0;
      categoryCollection1.BasicPropertyMatchesFilter = num1 != 0;
      EasingCategoryCollection categoryCollection2 = this;
      int num2 = categoryCollection2.AdvancedPropertyMatchesFilter | this.get_MatchesFilter() ? 1 : 0;
      categoryCollection2.AdvancedPropertyMatchesFilter = num2 != 0;
    }

    public void ConvertSelection(KeyFrameInterpolationType keyframeInterpolationType)
    {
      SceneNode sceneNode = this.selectedObjects[0];
      if (sceneNode.ViewModel == null)
        return;
      SceneViewModel viewModel = sceneNode.ViewModel;
      using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(StringTable.ConvertKeyFramesUndoUnit, false))
      {
        bool flag = false;
        foreach (object obj in this.selectedObjects)
        {
          KeyFrameSceneNode keyFrame = obj as KeyFrameSceneNode;
          if (keyFrame == null || keyFrame.KeyFrameAnimation == null)
          {
            flag = false;
            break;
          }
          if (keyFrame.InterpolationType != keyframeInterpolationType)
          {
            KeyFrameSceneNode selectionToExtend = keyFrame.KeyFrameAnimation.ReplaceKeyFrame(keyFrame, keyframeInterpolationType, (KeySpline) null);
            if (selectionToExtend == null)
            {
              flag = false;
              break;
            }
            viewModel.KeyFrameSelectionSet.ExtendSelection(selectionToExtend);
            flag = true;
          }
        }
        if (flag)
        {
          editTransaction.Commit();
          viewModel.RefreshSelection();
        }
        else
          editTransaction.Cancel();
      }
    }

    private class ConvertEasingTypeCommand : ICommand
    {
      private KeyFrameInterpolationType interpolationType;
      private EasingCategoryCollection easingCategorySelection;

      public event EventHandler CanExecuteChanged
      {
        add
        {
        }
        remove
        {
        }
      }

      public ConvertEasingTypeCommand(EasingCategoryCollection easingCategorySelection, KeyFrameInterpolationType interpolationType)
      {
        this.easingCategorySelection = easingCategorySelection;
        this.interpolationType = interpolationType;
      }

      public bool CanExecute(object parameter)
      {
        return true;
      }

      public void Execute(object parameter)
      {
        this.easingCategorySelection.ConvertSelection(this.interpolationType);
      }
    }
  }
}
