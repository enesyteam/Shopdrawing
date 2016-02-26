// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransitionEffectEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.SkinEditing;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Controls;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class TransitionEffectEditor : UserControl, IComponentConnector
  {
    private TransitionEffectPickerPopup popup;
    internal Grid TransitionEffectPickerIconBorder;
    internal Icon EffectOffIcon;
    internal Icon EffectOnIcon;
    private bool _contentLoaded;

    public TransitionEffectEditor()
    {
      this.InitializeComponent();
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      ITransitionEffectSite transitionEffectSite = this.DataContext as ITransitionEffectSite;
      e.Handled = true;
      transitionEffectSite.EnsureTransitionSceneNode();
      this.CreatePopup(transitionEffectSite.TransitionSceneNode);
    }

    private void CreatePopup(VisualStateTransitionSceneNode transitionNode)
    {
      VisualStateManagerSceneNode.EnsureExtendedAssemblyReferences((ITypeResolver) transitionNode.ProjectContext, transitionNode.DesignerContext.AssemblyService, transitionNode.DesignerContext.ViewUpdateManager);
      IType type = transitionNode.ProjectContext.ResolveType(ProjectNeutralTypes.TransitionEffect);
      if (transitionNode.ProjectContext.PlatformMetadata.IsNullType((ITypeId) type))
        return;
      transitionNode.ViewModel.TransitionSelectionSet.SetSelection(transitionNode);
      SceneNodeProperty transitionEffectProperty = new VisualStateTransitionEditor(transitionNode).VisualTransitionObjectSet.CreateSceneNodeProperty(new PropertyReference((ReferenceStep) transitionNode.ProjectContext.ResolveProperty(VisualStateManagerSceneNode.TransitionEffectProperty)), TypeUtilities.GetAttributes(type.RuntimeType));
      this.popup = new TransitionEffectPickerPopup(transitionEffectProperty, transitionNode.ViewModel.DefaultView);
      transitionNode.ViewModel.Closing += new EventHandler(this.ViewModel_Closing);
      this.popup.SynchronousClosed += (EventHandler) ((sender, e) =>
      {
        if (transitionNode.IsAttached && transitionNode.ViewModel != null)
          transitionNode.ViewModel.TransitionSelectionSet.Clear();
        if (transitionEffectProperty != null)
        {
          transitionEffectProperty.OnRemoveFromCategory();
          transitionEffectProperty = (SceneNodeProperty) null;
        }
        transitionNode.ViewModel.Closing -= new EventHandler(this.ViewModel_Closing);
        this.popup = (TransitionEffectPickerPopup) null;
      });
      this.popup.Placement = PlacementMode.Bottom;
      this.popup.PlacementTarget = (UIElement) this;
      this.popup.IsOpen = true;
    }

    private void ViewModel_Closing(object sender, EventArgs e)
    {
      if (this.popup == null)
        return;
      this.popup.IsOpen = false;
      this.popup = (TransitionEffectPickerPopup) null;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/skinediting/transitioneffecteditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.TransitionEffectPickerIconBorder = (Grid) target;
          break;
        case 2:
          this.EffectOffIcon = (Icon) target;
          break;
        case 3:
          this.EffectOnIcon = (Icon) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
