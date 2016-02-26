// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.MaterialEditor.MaterialEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.MaterialEditor
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  internal sealed class MaterialEditor : StackPanel, INotifyPropertyChanged, IComponentConnector
  {
    private ObservableCollection<MaterialBaseEditor> materialList = new ObservableCollection<MaterialBaseEditor>();
    private SceneNodeProperty editingProperty;
    private bool isSettingType;
    private ICollectionView materialListView;
    private ICommand addMaterialCommand;
    private ICommand removeMaterialCommand;
    private ICommand bringForwardCommand;
    private ICommand sendBackCommand;
    internal MaterialEditor MaterialEditorControl;
    internal Button AddMaterialButton;
    internal Button DeleteMaterialButton;
    internal Button MoveMaterialDownButton;
    internal Button MoveMaterialUpButton;
    internal SingleSelectionListBox MaterialSelectorListBox;
    private bool _contentLoaded;

    public ICollectionView MaterialList
    {
      get
      {
        return this.materialListView;
      }
    }

    public ICommand AddMaterialCommand
    {
      get
      {
        return this.addMaterialCommand;
      }
    }

    public ICommand RemoveMaterialCommand
    {
      get
      {
        return this.removeMaterialCommand;
      }
    }

    public bool IsRemoveMaterialEnabled
    {
      get
      {
        return this.materialListView.CurrentItem is MaterialBaseEditor;
      }
    }

    public ICommand BringForwardCommand
    {
      get
      {
        return this.bringForwardCommand;
      }
    }

    public bool IsBringForwardEnabled
    {
      get
      {
        return this.materialListView.CurrentPosition < this.materialList.Count - 1;
      }
    }

    public ICommand SendBackCommand
    {
      get
      {
        return this.sendBackCommand;
      }
    }

    public bool IsSendBackEnabled
    {
      get
      {
        return this.materialListView.CurrentPosition > 0;
      }
    }

    public bool IsSelectionDiffuse
    {
      get
      {
        return typeof (DiffuseMaterial).Equals(this.SelectedMaterialType);
      }
      set
      {
        this.SwitchMaterialType(typeof (DiffuseMaterial));
      }
    }

    public bool IsSelectionEmissive
    {
      get
      {
        return typeof (EmissiveMaterial).Equals(this.SelectedMaterialType);
      }
      set
      {
        this.SwitchMaterialType(typeof (EmissiveMaterial));
      }
    }

    public bool IsSelectionSpecular
    {
      get
      {
        return typeof (SpecularMaterial).Equals(this.SelectedMaterialType);
      }
      set
      {
        this.SwitchMaterialType(typeof (SpecularMaterial));
      }
    }

    private Type SelectedMaterialType
    {
      get
      {
        if (this.materialListView.CurrentPosition >= 0 && this.materialListView.CurrentPosition < this.materialList.Count)
          return this.materialList[this.materialListView.CurrentPosition].Type;
        return (Type) null;
      }
    }

    private SceneViewModel ViewModel
    {
      get
      {
        return this.editingProperty.SceneNodeObjectSet.ViewModel;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public MaterialEditor()
    {
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.OnMaterialEditorDataContextChanged);
      this.Loaded += new RoutedEventHandler(this.OnMaterialEditorLoaded);
      this.Unloaded += new RoutedEventHandler(this.OnMaterialEditorUnloaded);
      this.addMaterialCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.AddMaterial));
      this.removeMaterialCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.RemoveMaterial));
      this.bringForwardCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.BringForwardMaterial));
      this.sendBackCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.SendBackMaterial));
      this.materialListView = CollectionViewSource.GetDefaultView((object) this.materialList);
      this.materialListView.CurrentChanged += new EventHandler(this.OnMaterialListViewCurrentChanged);
      this.InitializeComponent();
    }

    private void AddMaterial()
    {
      MaterialBaseEditor materialEditor = this.materialListView.CurrentItem as MaterialBaseEditor;
      if (materialEditor != null)
      {
        DocumentNode documentNode = this.ViewModel.CreateSceneNode(materialEditor.Type).DocumentNode;
        SceneNodeProperty propertyForParent = this.GetSceneNodePropertyForParent(materialEditor);
        using (this.ViewModel.ForceBaseValue())
        {
          if (propertyForParent == null)
          {
            bool isMixed;
            DocumentNode valueAsDocumentNode = this.editingProperty.GetLocalValueAsDocumentNode(true, out isMixed);
            if (!isMixed)
            {
              this.editingProperty.SetValue((object) this.ViewModel.CreateSceneNode(typeof (MaterialGroup)).DocumentNode);
              SceneNodeProperty sceneNodeProperty = this.editingProperty.SceneNodeObjectSet.CreateSceneNodeProperty(this.editingProperty.Reference.Append(MaterialGroupNode.ChildrenProperty), (AttributeCollection) null);
              sceneNodeProperty.SetValue((object) this.ViewModel.CreateSceneNode(typeof (MaterialCollection)).DocumentNode);
              sceneNodeProperty.AddValue((object) valueAsDocumentNode);
              sceneNodeProperty.AddValue((object) documentNode);
              sceneNodeProperty.OnRemoveFromCategory();
            }
          }
          else
          {
            int index = ((IndexedClrPropertyReferenceStep) materialEditor.MaterialProperty.Reference.LastStep).Index;
            propertyForParent.InsertValue(index + 1, (object) documentNode);
            propertyForParent.OnRemoveFromCategory();
          }
        }
        this.Rebuild();
        this.materialListView.MoveCurrentToNext();
      }
      else
      {
        using (this.ViewModel.ForceBaseValue())
          this.editingProperty.SetValue((object) this.ViewModel.CreateSceneNode(typeof (DiffuseMaterial)).DocumentNode);
        this.Rebuild();
      }
    }

    private void RemoveMaterial()
    {
      MaterialBaseEditor materialEditor = this.materialListView.CurrentItem as MaterialBaseEditor;
      SceneNodeProperty propertyForParent = this.GetSceneNodePropertyForParent(materialEditor);
      using (this.ViewModel.ForceBaseValue())
      {
        if (propertyForParent == null)
        {
          this.editingProperty.ClearValue();
        }
        else
        {
          int index = ((IndexedClrPropertyReferenceStep) materialEditor.MaterialProperty.Reference.LastStep).Index;
          propertyForParent.RemoveValueAt(index);
          propertyForParent.OnRemoveFromCategory();
        }
      }
      this.Rebuild();
    }

    private void BringForwardMaterial()
    {
      int currentPosition = this.materialListView.CurrentPosition;
      this.SwapMaterials(this.materialList[currentPosition], this.materialList[currentPosition + 1]);
      this.materialListView.MoveCurrentToPosition(currentPosition + 1);
    }

    private void SendBackMaterial()
    {
      int currentPosition = this.materialListView.CurrentPosition;
      this.SwapMaterials(this.materialList[currentPosition], this.materialList[currentPosition - 1]);
      this.materialListView.MoveCurrentToPosition(currentPosition - 1);
    }

    private void SwapMaterials(MaterialBaseEditor firstMaterial, MaterialBaseEditor secondMaterial)
    {
      SceneNodeProperty propertyForParent1 = this.GetSceneNodePropertyForParent(firstMaterial);
      SceneNodeProperty propertyForParent2 = this.GetSceneNodePropertyForParent(secondMaterial);
      int index1 = ((IndexedClrPropertyReferenceStep) firstMaterial.MaterialProperty.Reference.LastStep).Index;
      int index2 = ((IndexedClrPropertyReferenceStep) secondMaterial.MaterialProperty.Reference.LastStep).Index;
      bool isMixed;
      DocumentNode valueAsDocumentNode1 = firstMaterial.MaterialProperty.GetLocalValueAsDocumentNode(true, out isMixed);
      DocumentNode valueAsDocumentNode2 = secondMaterial.MaterialProperty.GetLocalValueAsDocumentNode(true, out isMixed);
      if (index1 > index2)
      {
        propertyForParent1.RemoveValueAt(index1);
        propertyForParent2.RemoveValueAt(index2);
        propertyForParent2.InsertValue(index2, (object) valueAsDocumentNode1);
        propertyForParent1.InsertValue(index1, (object) valueAsDocumentNode2);
      }
      else
      {
        propertyForParent2.RemoveValueAt(index2);
        propertyForParent1.RemoveValueAt(index1);
        propertyForParent1.InsertValue(index1, (object) valueAsDocumentNode2);
        propertyForParent2.InsertValue(index2, (object) valueAsDocumentNode1);
      }
      this.Rebuild();
    }

    private SceneNodeProperty GetSceneNodePropertyForParent(MaterialBaseEditor materialEditor)
    {
      PropertyReference reference = materialEditor.MaterialProperty.Reference;
      int endIndex = reference.Count - 2;
      if (endIndex >= this.editingProperty.Reference.Count)
        return materialEditor.MaterialProperty.SceneNodeObjectSet.CreateSceneNodeProperty(reference.Subreference(0, endIndex), (AttributeCollection) null);
      return (SceneNodeProperty) null;
    }

    private void OnMaterialEditorDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.UpdateMaterialEditorFromDataContext();
    }

    private void OnMaterialEditorUnloaded(object sender, RoutedEventArgs e)
    {
      this.ResetEditingProperty();
      this.Rebuild();
    }

    private void OnMaterialEditorLoaded(object sender, RoutedEventArgs e)
    {
      this.UpdateMaterialEditorFromDataContext();
    }

    private void UpdateMaterialEditorFromDataContext()
    {
      this.ResetEditingProperty();
      Microsoft.Windows.Design.PropertyEditing.PropertyValue propertyValue = this.DataContext as Microsoft.Windows.Design.PropertyEditing.PropertyValue;
      if (propertyValue != null)
        this.editingProperty = (SceneNodeProperty) propertyValue.ParentProperty;
      if (this.editingProperty != null)
      {
        this.editingProperty.PropertyChanged += new PropertyChangedEventHandler(this.OnEditingPropertyPropertyChanged);
        this.editingProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnMaterialChanged);
      }
      this.Rebuild();
    }

    private void ResetEditingProperty()
    {
      if (this.editingProperty == null)
        return;
      this.editingProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnMaterialChanged);
      this.editingProperty.PropertyChanged -= new PropertyChangedEventHandler(this.OnEditingPropertyPropertyChanged);
      this.editingProperty = (SceneNodeProperty) null;
    }

    private void OnMaterialChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      if (!typeof (Material).IsAssignableFrom(PlatformTypeHelper.GetPropertyType((IProperty) e.PropertyReference.LastStep)))
        return;
      this.Rebuild();
    }

    private void OnEditingPropertyPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Associated"))
        return;
      this.Rebuild();
    }

    private void Rebuild()
    {
      foreach (MaterialBaseEditor materialBaseEditor in (Collection<MaterialBaseEditor>) this.materialList)
        materialBaseEditor.Unhook();
      int currentPosition = this.materialListView.CurrentPosition;
      this.materialList.Clear();
      if (this.editingProperty == null || !this.editingProperty.Associated)
        return;
      bool isMixed;
      DocumentNode valueAsDocumentNode = this.editingProperty.GetLocalValueAsDocumentNode(true, out isMixed);
      if (!isMixed && valueAsDocumentNode != null)
      {
        MaterialNode root = this.ViewModel.GetSceneNode(valueAsDocumentNode) as MaterialNode;
        if (root != null)
        {
          foreach (KeyValuePair<MaterialNode, PropertyReference> keyValuePair in new MaterialEditor.MaterialNodeTreeAdapter(root, this.editingProperty.Reference).Materials)
          {
            if (typeof (SpecularMaterial).IsAssignableFrom(keyValuePair.Key.TargetType))
              this.materialList.Add((MaterialBaseEditor) new SpecularMaterialEditor(this.editingProperty.SceneNodeObjectSet, keyValuePair.Value));
            else if (typeof (DiffuseMaterial).IsAssignableFrom(keyValuePair.Key.TargetType))
              this.materialList.Add((MaterialBaseEditor) new DiffuseMaterialEditor(this.editingProperty.SceneNodeObjectSet, keyValuePair.Value));
            else if (typeof (EmissiveMaterial).IsAssignableFrom(keyValuePair.Key.TargetType))
              this.materialList.Add((MaterialBaseEditor) new EmissiveMaterialEditor(this.editingProperty.SceneNodeObjectSet, keyValuePair.Value));
          }
        }
      }
      if (this.materialList.Count > 0)
      {
        if (currentPosition >= this.materialList.Count)
          this.materialListView.MoveCurrentToLast();
        else if (currentPosition == -1)
          this.materialListView.MoveCurrentToFirst();
        else
          this.materialListView.MoveCurrentToPosition(currentPosition);
      }
      this.OnPropertyChanged("IsRemoveMaterialEnabled");
      this.OnPropertyChanged("IsSendBackEnabled");
      this.OnPropertyChanged("IsBringForwardEnabled");
    }

    private void OnMaterialListViewCurrentChanged(object sender, EventArgs e)
    {
      this.OnPropertyChanged("IsRemoveMaterialEnabled");
      this.OnPropertyChanged("IsSendBackEnabled");
      this.OnPropertyChanged("IsBringForwardEnabled");
      this.OnMaterialTypeChange();
    }

    private void OnMaterialTypeChange()
    {
      this.OnPropertyChanged("IsSelectionDiffuse");
      this.OnPropertyChanged("IsSelectionEmissive");
      this.OnPropertyChanged("IsSelectionSpecular");
    }

    private void SwitchMaterialType(Type type)
    {
      MaterialBaseEditor materialBaseEditor = this.materialListView.CurrentItem as MaterialBaseEditor;
      if (materialBaseEditor == null || !(materialBaseEditor.Type != type) || this.isSettingType)
        return;
      this.isSettingType = true;
      materialBaseEditor.MaterialProperty.SetValue((object) this.ViewModel.CreateSceneNode(type).DocumentNode);
      this.Rebuild();
      this.OnMaterialTypeChange();
      this.isSettingType = false;
    }

    private void OnPropertyChanged(string name)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(name));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/materialeditor/materialeditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.MaterialEditorControl = (MaterialEditor) target;
          break;
        case 2:
          this.AddMaterialButton = (Button) target;
          break;
        case 3:
          this.DeleteMaterialButton = (Button) target;
          break;
        case 4:
          this.MoveMaterialDownButton = (Button) target;
          break;
        case 5:
          this.MoveMaterialUpButton = (Button) target;
          break;
        case 6:
          this.MaterialSelectorListBox = (SingleSelectionListBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    private class MaterialNodeTreeAdapter
    {
      private MaterialNode root;
      private PropertyReference propertyReference;

      public IEnumerable<KeyValuePair<MaterialNode, PropertyReference>> Materials
      {
        get
        {
          MaterialGroupNode groupNode = this.root as MaterialGroupNode;
          if (groupNode != null)
          {
            for (int i = 0; i < groupNode.Children.Count; ++i)
            {
              MaterialNode childNode = groupNode.Children[i];
              if (childNode != null)
              {
                ReferenceStep[] childStep = new ReferenceStep[2]
                {
                  groupNode.GetPropertyForChild((SceneNode) childNode) as ReferenceStep,
                  (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep(this.root.ProjectContext.Platform.Metadata.DefaultTypeResolver, typeof (MaterialCollection), i)
                };
                PropertyReference childReference = PropertyReference.CreateNewPropertyReferenceFromStepsWithoutCopy(childStep);
                MaterialEditor.MaterialNodeTreeAdapter childTree = new MaterialEditor.MaterialNodeTreeAdapter(childNode, this.propertyReference.Append(childReference));
                foreach (KeyValuePair<MaterialNode, PropertyReference> keyValuePair in childTree.Materials)
                  yield return keyValuePair;
              }
            }
          }
          else
            yield return new KeyValuePair<MaterialNode, PropertyReference>(this.root, this.propertyReference);
        }
      }

      public MaterialNodeTreeAdapter(MaterialNode root, PropertyReference propertyReference)
      {
        this.root = root;
        this.propertyReference = propertyReference;
      }
    }
  }
}
