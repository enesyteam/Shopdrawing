// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PathTargetDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Controls;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  internal sealed class PathTargetDialog : Dialog
  {
    private PathTargetDialog.PathTargetDialogModel model;

    private PathTargetDialog(SceneElement root, SceneElement path, string title, string infoText, PathTargetDialog.PathTargetValidator canTargetElement, ITypeId constraintType)
    {
      FrameworkElement element = FileTable.GetElement("Resources\\MotionPathTargetDialog.xaml");
      this.DialogContent = (UIElement) element;
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.Title = title;
      this.model = new PathTargetDialog.PathTargetDialogModel(root, path, canTargetElement, infoText, constraintType);
      element.DataContext = (object) this.model;
    }

    public static BaseFrameworkElement ChooseMotionPathTarget(SceneElement root, SceneElement path)
    {
      PathTargetDialog pathTargetDialog = new PathTargetDialog(root, path, StringTable.MotionPathTargetDialogTitle, StringTable.MotionPathTargetDialogInfoText, new PathTargetDialog.PathTargetValidator(PathTargetDialog.ValidateMotionPathTarget), (ITypeId) null);
      bool? nullable = pathTargetDialog.ShowDialog();
      if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? true : false)) != 0 && pathTargetDialog.model.SelectedElement != null)
        return (BaseFrameworkElement) pathTargetDialog.model.SelectedElement.Element;
      return (BaseFrameworkElement) null;
    }

    public static Base2DElement ChooseClippingPathTarget(SceneElement root, SceneElement path)
    {
      PathTargetDialog pathTargetDialog = new PathTargetDialog(root, path, StringTable.ClippingPathTargetDialogTitle, StringTable.ClippingPathTargetDialogInfoText, new PathTargetDialog.PathTargetValidator(PathTargetDialog.ValidateClippingPathTarget), (ITypeId) null);
      bool? nullable = pathTargetDialog.ShowDialog();
      if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? true : false)) != 0 && pathTargetDialog.model.SelectedElement != null)
        return (Base2DElement) pathTargetDialog.model.SelectedElement.Element;
      return (Base2DElement) null;
    }

    public static SceneElement ChooseElement(SceneElement root, SceneElement path)
    {
      PathTargetDialog pathTargetDialog = new PathTargetDialog(root, path, StringTable.SelectElementDialogTitle, StringTable.SelectElementDialogInfoText, new PathTargetDialog.PathTargetValidator(PathTargetDialog.AllowAllTargets), (ITypeId) null);
      bool? nullable = pathTargetDialog.ShowDialog();
      if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? true : false)) != 0 && pathTargetDialog.model.SelectedElement != null)
        return pathTargetDialog.model.SelectedElement.Element;
      return (SceneElement) null;
    }

    public static SceneElement ChooseElementForBehavior(SceneElement root, ITypeId constraintType)
    {
      PathTargetDialog pathTargetDialog = new PathTargetDialog(root, (SceneElement) null, StringTable.SelectElementDialogTitle, StringTable.SelectElementDialogInfoText, new PathTargetDialog.PathTargetValidator(PathTargetDialog.ValidateBehaviorTargets), constraintType);
      bool? nullable = pathTargetDialog.ShowDialog();
      if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? true : false)) != 0 && pathTargetDialog.model.SelectedElement != null)
        return pathTargetDialog.model.SelectedElement.Element;
      return (SceneElement) null;
    }

    private static bool ValidateBehaviorTargets(bool isRoot, SceneElement target, SceneElement path, ITypeId constraintType)
    {
      if (isRoot || !target.CanNameElement)
        return false;
      if (constraintType != null)
        return constraintType.IsAssignableFrom((ITypeId) target.Type);
      return true;
    }

    private static bool AllowAllTargets(bool isRoot, SceneElement target, SceneElement path, ITypeId constraintType)
    {
      return true;
    }

    private static bool ValidateMotionPathTarget(bool isRoot, SceneElement target, SceneElement path, ITypeId constraintType)
    {
      return !target.IsLocked && !isRoot;
    }

    private static bool ValidateClippingPathTarget(bool isRoot, SceneElement target, SceneElement path, ITypeId constraintType)
    {
      return !target.IsLocked && !isRoot && (path != target && PlatformTypes.UIElement.IsAssignableFrom((ITypeId) target.Type)) && !path.IsAncestorOf((SceneNode) target);
    }

    private delegate bool PathTargetValidator(bool isRoot, SceneElement target, SceneElement path, ITypeId constraintType);

    private class PathTargetDialogModel : INotifyPropertyChanged
    {
      private SelectionContext<ElementNode> selectionContext;
      private ElementNode root;
      private SceneElement path;
      private PathTargetDialog.PathTargetValidator canTargetElement;
      private string infoText;
      private ITypeId constraintType;

      public ElementNode Root
      {
        get
        {
          return this.root;
        }
      }

      public string InfoText
      {
        get
        {
          return this.infoText;
        }
      }

      public ElementNode SelectedElement
      {
        get
        {
          return this.selectionContext.PrimarySelection;
        }
        set
        {
          this.selectionContext.SetSelection(value);
        }
      }

      public event PropertyChangedEventHandler PropertyChanged;

      public PathTargetDialogModel(SceneElement root, SceneElement path, PathTargetDialog.PathTargetValidator canTargetElement, string infoText, ITypeId constraintType)
      {
        this.canTargetElement = canTargetElement;
        this.path = path;
        this.constraintType = constraintType;
        this.selectionContext = (SelectionContext<ElementNode>) new SingleSelectionContext<ElementNode>();
        this.selectionContext.PropertyChanged += new PropertyChangedEventHandler(this.selectionContext_PropertyChanged);
        this.root = this.BuildElementNodeTree(root, (ElementNode) null);
        this.infoText = infoText;
      }

      private ElementNode BuildElementNodeTree(SceneElement element, ElementNode parent)
      {
        ElementNode parent1 = new ElementNode(element, this.selectionContext);
        parent1.IsExpanded = true;
        parent1.Parent = parent;
        parent1.IsSelectable = this.canTargetElement(parent == null, element, this.path, this.constraintType);
        this.ProcessContent(element, parent1);
        return parent1;
      }

      private void ProcessContent(SceneElement node, ElementNode parent)
      {
        foreach (SceneNode sceneNode in node.GetAllContent())
        {
          BaseFrameworkElement frameworkElement = sceneNode as BaseFrameworkElement;
          if (frameworkElement != null)
          {
            parent.Children.Add(this.BuildElementNodeTree((SceneElement) frameworkElement, parent));
          }
          else
          {
            SceneElement node1 = sceneNode as SceneElement;
            if (node1 != null)
              this.ProcessContent(node1, parent);
          }
        }
      }

      private void OnPropertyChanged(string propertyName)
      {
        if (this.PropertyChanged == null)
          return;
        this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
      }

      private void selectionContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
        if (!(e.PropertyName == "PrimarySelection"))
          return;
        this.OnPropertyChanged("SelectedElement");
      }
    }
  }
}
