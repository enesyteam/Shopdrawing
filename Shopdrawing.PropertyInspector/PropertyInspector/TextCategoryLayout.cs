// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TextCategoryLayout
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.Text;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.Text;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Expression.Project;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class TextCategoryLayout : Grid, INotifyPropertyChanged, IComponentConnector
  {
    public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register("Category", typeof (CategoryBase), typeof (TextCategoryLayout), (PropertyMetadata) new FrameworkPropertyMetadata(null, new PropertyChangedCallback(TextCategoryLayout.CategoryPropertyChanged)));
    public static readonly RoutedCommand OnComboBoxCommitCommand = new RoutedCommand("OnComboBoxCommit", typeof (TextCategoryLayout));
    public static readonly DependencyProperty ComboBoxEntriesProperty = DependencyProperty.RegisterAttached("ComboBoxEntries", typeof (IEnumerable), typeof (TextCategoryLayout), (PropertyMetadata) new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
    public static readonly DependencyProperty TabMatchesFilterProperty = DependencyProperty.RegisterAttached("TabMatchesFilter", typeof (bool), typeof (TextCategoryLayout), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, FrameworkPropertyMetadataOptions.Inherits));
    public static readonly DependencyProperty CurrentFontProperty = DependencyProperty.Register("CurrentFont", typeof (SourcedFontFamilyItem), typeof (TextCategoryLayout), (PropertyMetadata) new FrameworkPropertyMetadata(null, new PropertyChangedCallback(TextCategoryLayout.CurrentFontChanged)));
    private static readonly double[] fontSizeList = new double[16]
    {
      6.0,
      7.0,
      8.0,
      9.0,
      10.0,
      11.0,
      12.0,
      14.0,
      16.0,
      18.0,
      20.0,
      22.0,
      24.0,
      36.0,
      48.0,
      72.0
    };
    private static readonly double[] lineHeightList = new double[17]
    {
      double.NaN,
      6.0,
      7.0,
      8.0,
      9.0,
      10.0,
      11.0,
      12.0,
      14.0,
      16.0,
      18.0,
      20.0,
      22.0,
      24.0,
      36.0,
      48.0,
      72.0
    };
    private static readonly double[] lineHeightListNoAuto = new double[16]
    {
      6.0,
      7.0,
      8.0,
      9.0,
      10.0,
      11.0,
      12.0,
      14.0,
      16.0,
      18.0,
      20.0,
      22.0,
      24.0,
      36.0,
      48.0,
      72.0
    };
    private static readonly double[] textIndentList = new double[17]
    {
      0.0,
      1.0,
      2.0,
      3.0,
      4.0,
      5.0,
      6.0,
      7.0,
      8.0,
      9.0,
      10.0,
      11.0,
      12.0,
      13.0,
      14.0,
      15.0,
      100.0
    };
    private static readonly double[] indentList = new double[17]
    {
      0.0,
      1.0,
      2.0,
      3.0,
      4.0,
      5.0,
      6.0,
      7.0,
      8.0,
      9.0,
      10.0,
      11.0,
      12.0,
      13.0,
      14.0,
      15.0,
      100.0
    };
    private static readonly double[] markerOffsetList = new double[6]
    {
      1.0,
      2.0,
      3.0,
      4.0,
      5.0,
      10.0
    };
    private static readonly TextMarkerStyle[] markerStyleList = new TextMarkerStyle[10]
    {
      TextMarkerStyle.None,
      TextMarkerStyle.Box,
      TextMarkerStyle.Circle,
      TextMarkerStyle.Square,
      TextMarkerStyle.Disc,
      TextMarkerStyle.Decimal,
      TextMarkerStyle.LowerLatin,
      TextMarkerStyle.UpperLatin,
      TextMarkerStyle.LowerRoman,
      TextMarkerStyle.UpperRoman
    };
    private List<List<PropertyContainer>> propertyContainersByTab = new List<List<PropertyContainer>>();
    private bool isParagraphTabAvailable = true;
    private bool isLineIndentTabAvailable = true;
    private bool isListTabAvailable = true;
    private bool isFontEmbeddingAvailable = true;
    private bool isFontSubsetTaskAvailable = true;
    private SceneNodeProperty textDecorationsProperty;
    private SceneNodeProperty textIndentProperty;
    private SceneNodeCategory currentCategory;
    private bool isProcessingEmbedding;
    private SceneNodeObjectSet sceneNodeObjectSet;
    internal TextCategoryLayout Root;
    internal FocusDenyingTabControl TextTabControl;
    internal TabItem FontTab;
    internal PropertyContainer FontFamilyPropertyContainer;
    internal PropertyContainer FontSizePropertyContainer;
    internal PropertyContainer BoldPropertyContainer;
    internal PropertyContainer ItalicPropertyContainer;
    internal PropertyContainer UnderlinePropertyContainer;
    internal Button HyperlinkButton;
    internal CheckBox EmbedCheckbox;
    internal TabItem ParagraphTab;
    internal Icon LineHeightIcon;
    internal PropertyContainer LineHeightPropertyContainer;
    internal Icon PSBIcon;
    internal PropertyContainer ParagraphSpacingBeforePropertyContainer;
    internal Icon PSAIcon;
    internal PropertyContainer ParagraphSpacingAfterPropertyContainer;
    internal Icon AlignmentIcon;
    internal PropertyContainer AlignPropertyContainer;
    internal TabItem LineIndentTab;
    internal Icon LeftIndentIcon;
    internal PropertyContainer LeftIndentPropertyContainer;
    internal Icon RightIndentIcon;
    internal PropertyContainer RightIndentPropertyContainer;
    internal Icon FirstLineIndentIcon;
    internal PropertyContainer FirstLineIndentPropertyContainer;
    internal TabItem ListTab;
    internal ComboBox BulletMarkerCombo;
    internal ComboBox BulletOffsetCombo;
    private bool _contentLoaded;

    public CategoryBase Category
    {
      get
      {
        return (CategoryBase) this.GetValue(TextCategoryLayout.CategoryProperty);
      }
      set
      {
        this.SetValue(TextCategoryLayout.CategoryProperty, value);
      }
    }

    public SourcedFontFamilyItem CurrentFont
    {
      get
      {
        return (SourcedFontFamilyItem) this.GetValue(TextCategoryLayout.CurrentFontProperty);
      }
      set
      {
        this.SetValue(TextCategoryLayout.CurrentFontProperty, value);
      }
    }

    public bool IsFontEmbeddingSetupEnabled
    {
      get
      {
        Command embeddingCommand = this.SetupFontEmbeddingCommand;
        if (embeddingCommand != null)
          return embeddingCommand.IsEnabled;
        return false;
      }
    }

    public bool IsFontEmbeddingSetupAvailable
    {
      get
      {
        Command embeddingCommand = this.SetupFontEmbeddingCommand;
        if (embeddingCommand != null)
          return embeddingCommand.IsAvailable;
        return false;
      }
    }

    private Command SetupFontEmbeddingCommand
    {
      get
      {
        Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SetupFontEmbeddingCommand embeddingCommand = (Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SetupFontEmbeddingCommand) null;
        SceneNodeCategory sceneNodeCategory = this.DataContext as SceneNodeCategory;
        if (sceneNodeCategory != null)
        {
          SceneViewModel viewModel = (SceneViewModel) null;
          using (IEnumerator<PropertyEntry> enumerator = ((CategoryEntry) sceneNodeCategory).get_Properties().GetEnumerator())
          {
            while (((IEnumerator) enumerator).MoveNext())
            {
              SceneNodeProperty sceneNodeProperty = enumerator.Current as SceneNodeProperty;
              if (sceneNodeProperty != null)
              {
                viewModel = sceneNodeProperty.SceneNodeObjectSet.ViewModel;
                break;
              }
            }
          }
          if (viewModel != null)
            embeddingCommand = new Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SetupFontEmbeddingCommand(viewModel);
        }
        return (Command) embeddingCommand;
      }
    }

    public TextCategory TextCategory
    {
      get
      {
        return this.currentCategory as TextCategory;
      }
    }

    public bool IsParagraphTabAvailable
    {
      get
      {
        return this.isParagraphTabAvailable;
      }
      set
      {
        if (this.isParagraphTabAvailable == value)
          return;
        this.isParagraphTabAvailable = value;
        this.OnPropertyChanged("IsParagraphTabAvailable");
      }
    }

    public bool IsLineIndentTabAvailable
    {
      get
      {
        return this.isLineIndentTabAvailable;
      }
      set
      {
        if (this.isLineIndentTabAvailable == value)
          return;
        this.isLineIndentTabAvailable = value;
        this.OnPropertyChanged("IsLineIndentTabAvailable");
      }
    }

    public bool IsListTabAvailable
    {
      get
      {
        return this.isListTabAvailable;
      }
      set
      {
        if (this.isListTabAvailable == value)
          return;
        this.isListTabAvailable = value;
        this.OnPropertyChanged("IsListTabAvailable");
      }
    }

    private SceneNodeObjectSet ObjectSet
    {
      get
      {
        if (this.Category != null)
        {
          IEnumerator<PropertyEntry> enumerator = ((CategoryEntry) this.Category).get_Properties().GetEnumerator();
          if (((IEnumerator) enumerator).MoveNext())
            return ((SceneNodeProperty) enumerator.Current).SceneNodeObjectSet;
        }
        return (SceneNodeObjectSet) null;
      }
    }

    private IEnumerable<SceneNode> ObjectSetObjects
    {
      get
      {
        if (this.ObjectSet != null)
        {
          foreach (SceneNode sceneNode in this.ObjectSet.Objects)
          {
            TextRangeElement textRange = sceneNode as TextRangeElement;
            if (textRange != null)
              yield return (SceneNode) textRange.TextEditProxy.TextSource;
            else
              yield return sceneNode;
          }
        }
      }
    }

    public bool IsFontEmbedded
    {
      get
      {
        if (this.sceneNodeObjectSet == null || this.sceneNodeObjectSet.ViewModel == null)
          return false;
        ProjectFontFamilyItem projectFontFamilyItem = this.CurrentKnownFont as ProjectFontFamilyItem;
        if (projectFontFamilyItem == null)
          return false;
        IProjectItem projectItem = this.sceneNodeObjectSet.DesignerContext.ActiveProject.FindItem(projectFontFamilyItem.ProjectFont.FontDocumentReference);
        if (projectItem != null)
          return projectItem.Properties["BuildAction"] == "BlendEmbeddedFont";
        return false;
      }
      set
      {
        if (this.sceneNodeObjectSet.ViewModel == null || this.isProcessingEmbedding)
          return;
        this.isProcessingEmbedding = true;
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, (Delegate) new DispatcherOperationCallback(this.DoFontEmbedding), (object) (bool) (value ? 1 : 0));
      }
    }

    public bool IsFontSubsetTaskAvailable
    {
      get
      {
        return this.isFontSubsetTaskAvailable;
      }
      set
      {
        if (this.isFontSubsetTaskAvailable == value)
          return;
        this.isFontSubsetTaskAvailable = value;
        this.OnPropertyChanged("IsFontSubsetTaskAvailable");
      }
    }

    public bool IsFontEmbeddingAvailable
    {
      get
      {
        return this.isFontEmbeddingAvailable;
      }
      set
      {
        if (this.isFontEmbeddingAvailable == value)
          return;
        this.isFontEmbeddingAvailable = value;
        this.OnPropertyChanged("IsFontEmbeddingAvailable");
      }
    }

    public bool IsFontEmbeddingEnabled
    {
      get
      {
        bool flag1 = false;
        if (this.sceneNodeObjectSet != null && this.sceneNodeObjectSet.ViewModel != null && (this.ObjectSet != null && ((CategoryEntry) this.Category).get_Item("FontFamily") != null) && this.ObjectSet.GetValue(RichTextBoxRangeElement.FontFamilyProperty) != MixedProperty.Mixed)
        {
          SourcedFontFamilyItem currentKnownFont = this.CurrentKnownFont;
          bool flag2 = currentKnownFont is UnknownSourceFontFamilyItem;
          SystemFontFamilyItem systemFontFamilyItem = currentKnownFont as SystemFontFamilyItem;
          bool flag3 = systemFontFamilyItem != null && systemFontFamilyItem.DisplayAsNativeSilverlightFont;
          bool flag4 = this.ObjectSet.RepresentativeSceneNode is StyleNode;
          bool flag5 = currentKnownFont != null && currentKnownFont.FamilyName == "Portable User Interface";
          flag1 = !flag2 && !flag3 && !flag4 && !flag5;
        }
        return flag1;
      }
    }

    private SourcedFontFamilyItem CurrentKnownFont
    {
      get
      {
        SourcedFontFamilyItem sourcedFontFamilyItem1 = this.CurrentFont;
        UnknownSourceFontFamilyItem sourceFontFamilyItem = sourcedFontFamilyItem1 as UnknownSourceFontFamilyItem;
        if (sourceFontFamilyItem != null)
        {
          foreach (SourcedFontFamilyItem sourcedFontFamilyItem2 in FontFamilyValueEditor.GetFontFamilies(this.sceneNodeObjectSet))
          {
            if (sourceFontFamilyItem.Equals((object) sourcedFontFamilyItem2))
            {
              sourcedFontFamilyItem1 = sourcedFontFamilyItem2;
              break;
            }
          }
        }
        return sourcedFontFamilyItem1;
      }
    }

    public System.Windows.Input.ICommand BulletsCommand
    {
      get
      {
        return (System.Windows.Input.ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnApplyBullets));
      }
    }

    public bool IsBulletsCommandEnabled
    {
      get
      {
        RichTextBox richTextBox = (RichTextBox) null;
        if (this.sceneNodeObjectSet != null && this.sceneNodeObjectSet.ProjectContext != null && (JoltHelper.TypeSupported((ITypeResolver) this.sceneNodeObjectSet.ProjectContext, PlatformTypes.List) && this.sceneNodeObjectSet.ViewModel.TextSelectionSet.TextEditProxy != null))
          richTextBox = this.sceneNodeObjectSet.ViewModel.TextSelectionSet.TextEditProxy.EditingElement.PlatformSpecificObject as RichTextBox;
        return richTextBox != null;
      }
    }

    public System.Windows.Input.ICommand CreateHyperlinkCommand
    {
      get
      {
        return (System.Windows.Input.ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnCreateHyperlink));
      }
    }

    public bool IsCreateHyperlinkVisible
    {
      get
      {
        if (this.sceneNodeObjectSet == null || this.sceneNodeObjectSet.ViewModel == null || (this.sceneNodeObjectSet.ViewModel.RootNode == null || this.sceneNodeObjectSet.ViewModel.TextSelectionSet.TextEditProxy == null) || !(this.sceneNodeObjectSet.ViewModel.TextSelectionSet.TextEditProxy.EditingElement is IViewFlowDocumentEditor))
          return false;
        bool flag1 = PlatformTypes.NavigationWindow.IsAssignableFrom((ITypeId) this.sceneNodeObjectSet.ViewModel.RootNode.Type) || PlatformTypes.Page.IsAssignableFrom((ITypeId) this.sceneNodeObjectSet.ViewModel.RootNode.Type);
        bool flag2 = !this.sceneNodeObjectSet.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) && PlatformTypes.RichTextBox.IsAssignableFrom((ITypeId) this.sceneNodeObjectSet.ViewModel.TextSelectionSet.TextEditProxy.TextSource.Type);
        if (!flag1)
          return flag2;
        return true;
      }
    }

    public bool IsCreateHyperlinkEnabled
    {
      get
      {
        if (!this.IsCreateHyperlinkVisible)
          return false;
        RichTextBox richTextBox = (RichTextBox) this.sceneNodeObjectSet.ViewModel.TextSelectionSet.TextEditProxy.EditingElement.PlatformSpecificObject;
        bool flag1 = richTextBox.Selection.Start.Paragraph != null && richTextBox.Selection.Start.Paragraph == richTextBox.Selection.End.Paragraph;
        bool flag2 = richTextBox.Selection.Start.Paragraph == richTextBox.Document.Blocks.LastBlock;
        if (!flag1)
          return flag2;
        return true;
      }
    }

    public SceneNodeProperty TextDecorationsProperty
    {
      get
      {
        SceneNodeProperty sceneNodeProperty = (SceneNodeProperty) null;
        if (this.currentCategory != null)
          sceneNodeProperty = (SceneNodeProperty) ((CategoryEntry) this.currentCategory).get_Item("TextDecorations");
        if (sceneNodeProperty == null)
        {
          TextCategory textCategory = this.TextCategory;
          if (textCategory != null && textCategory.SupportsTextProperties)
            sceneNodeProperty = this.textDecorationsProperty;
        }
        return sceneNodeProperty;
      }
    }

    public SceneNodeProperty TextIndentProperty
    {
      get
      {
        SceneNodeProperty sceneNodeProperty = (SceneNodeProperty) null;
        if (this.currentCategory != null)
          sceneNodeProperty = (SceneNodeProperty) ((CategoryEntry) this.currentCategory).get_Item("TextIndent");
        if (sceneNodeProperty == null)
        {
          TextCategory textCategory = this.TextCategory;
          if (textCategory != null && textCategory.SupportsParagraphProperties)
            sceneNodeProperty = this.textIndentProperty;
        }
        return sceneNodeProperty;
      }
    }

    public SceneNodeProperty TextAlignmentProperty
    {
      get
      {
        SceneNodeProperty sceneNodeProperty = (SceneNodeProperty) null;
        if (this.currentCategory != null)
          sceneNodeProperty = (SceneNodeProperty) ((CategoryEntry) this.currentCategory).get_Item("TextAlignment") ?? (SceneNodeProperty) ((CategoryEntry) this.currentCategory).get_Item("Block.TextAlignment");
        return sceneNodeProperty;
      }
    }

    public SceneNodeProperty LineHeightProperty
    {
      get
      {
        SceneNodeProperty sceneNodeProperty = (SceneNodeProperty) null;
        if (this.currentCategory != null)
          sceneNodeProperty = (SceneNodeProperty) ((CategoryEntry) this.currentCategory).get_Item("LineHeight") ?? (SceneNodeProperty) ((CategoryEntry) this.currentCategory).get_Item("Block.LineHeight");
        return sceneNodeProperty;
      }
    }

    public UnitsOptionsModel UnitsOptionsModel
    {
      get
      {
        if (this.sceneNodeObjectSet == null)
          return (UnitsOptionsModel) null;
        return this.sceneNodeObjectSet.DesignerContext.UnitsOptionsModel;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public TextCategoryLayout()
    {
      Binding binding = new Binding();
      this.SetBinding(TextCategoryLayout.CategoryProperty, (BindingBase) binding);
      this.InitializeComponent();
      this.SetBinding(TextCategoryLayout.CurrentFontProperty, (BindingBase) new Binding()
      {
        Source = this.FontFamilyPropertyContainer,
        Path = new PropertyPath("(0).(1)", new object[2]
        {
          (object) FontFamilyEditor.FontFamilyEditorProperty,
          (object) FontFamilyEditor.CurrentFontItemProperty
        })
      });
      this.TextTabControl.DataContext = this;
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.TextCategoryLayout_DataContextChanged);
      this.Loaded += new RoutedEventHandler(this.TextCategoryLayout_Loaded);
      this.Unloaded += new RoutedEventHandler(this.TextCategoryLayout_Unloaded);
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.TextAdvancedCategoryLayout_DataContextChanged);
    }

    private static void CategoryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TextCategoryLayout textCategoryLayout = d as TextCategoryLayout;
      if (textCategoryLayout == null)
        return;
      CategoryEntry categoryEntry1 = e.OldValue as CategoryEntry;
      CategoryEntry categoryEntry2 = e.NewValue as CategoryEntry;
      if (categoryEntry1 != null)
        categoryEntry1.remove_FilterApplied(new EventHandler<PropertyFilterAppliedEventArgs>(textCategoryLayout.OnFilterApplied));
      if (categoryEntry2 == null)
        return;
      categoryEntry2.add_FilterApplied(new EventHandler<PropertyFilterAppliedEventArgs>(textCategoryLayout.OnFilterApplied));
    }

    private static void CurrentFontChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TextCategoryLayout textCategoryLayout = d as TextCategoryLayout;
      if (textCategoryLayout == null)
        return;
      textCategoryLayout.OnPropertyChanged("IsFontEmbedded");
      textCategoryLayout.OnPropertyChanged("IsFontEmbeddingEnabled");
    }

    private void OnFilterApplied(object source, PropertyFilterAppliedEventArgs args)
    {
      bool flag1 = false;
      bool flag2 = false;
      int num = -1;
      int selectedIndex = this.TextTabControl.SelectedIndex;
      for (int index1 = 0; index1 < this.propertyContainersByTab.Count; ++index1)
      {
        bool flag3 = false;
        List<PropertyContainer> list = this.propertyContainersByTab[index1];
        for (int index2 = 0; index2 < this.propertyContainersByTab[index1].Count; ++index2)
        {
          PropertyContainer propertyContainer = list[index2];
          if (propertyContainer.get_PropertyEntry() != null)
          {
            propertyContainer.get_PropertyEntry().ApplyFilter(args.get_Filter());
            if (propertyContainer.get_PropertyEntry().get_MatchesFilter())
            {
              if (selectedIndex == index1)
                flag2 = true;
              if (num == -1)
                num = index1;
              flag3 = true;
              flag1 = true;
            }
          }
        }
        ((DependencyObject) this.TextTabControl.Items[index1]).SetValue(TextCategoryLayout.TabMatchesFilterProperty, (object) (bool) (flag3 ? 1 : 0));
      }
      this.Category.BasicPropertyMatchesFilter = flag1;
      if (flag2 || num == -1)
        return;
      this.TextTabControl.SelectedIndex = num;
    }

    public static object GetComboBoxEntries(DependencyObject target)
    {
      return target.GetValue(TextCategoryLayout.ComboBoxEntriesProperty);
    }

    public static void SetComboBoxEntries(DependencyObject target, object value)
    {
      target.SetValue(TextCategoryLayout.ComboBoxEntriesProperty, value);
    }

    public static bool GetTabMatchesFilter(DependencyObject target)
    {
      return (bool) target.GetValue(TextCategoryLayout.TabMatchesFilterProperty);
    }

    public static void SetTabMatchesFilter(DependencyObject target, bool value)
    {
      target.SetValue(TextCategoryLayout.TabMatchesFilterProperty, (object) (bool) (value ? 1 : 0));
    }

    private void TextAdvancedCategoryLayout_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.OnPropertyChanged("IsFontEmbeddingSetupEnabled");
      this.OnPropertyChanged("IsFontEmbeddingSetupAvailable");
    }

    private void TextCategoryLayout_Loaded(object sender, RoutedEventArgs e)
    {
      this.TryInitialize();
    }

    private void TextCategoryLayout_Unloaded(object sender, RoutedEventArgs e)
    {
      this.Dispose();
    }

    private void OnFontEmbeddingSetupClick(object sender, RoutedEventArgs e)
    {
      Command embeddingCommand = this.SetupFontEmbeddingCommand;
      if (embeddingCommand == null)
        return;
      embeddingCommand.Execute();
    }

    private void TextCategoryLayout_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (this.currentCategory != null)
        this.currentCategory.BasicProperties.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.BasicProperties_CollectionChanged);
      this.currentCategory = e.NewValue as SceneNodeCategory;
      this.OnPropertyChanged("TextCategory");
      if (this.currentCategory == null || this.TryInitialize())
        return;
      this.currentCategory.BasicProperties.CollectionChanged += new NotifyCollectionChangedEventHandler(this.BasicProperties_CollectionChanged);
    }

    private void BasicProperties_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (!this.TryInitialize())
        return;
      this.currentCategory.BasicProperties.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.BasicProperties_CollectionChanged);
    }

    private bool TryInitialize()
    {
      if (this.sceneNodeObjectSet == null && this.currentCategory != null && this.currentCategory.BasicProperties.Count > 0)
      {
        SceneNodeProperty sceneNodeProperty = (SceneNodeProperty) this.currentCategory.BasicProperties[0];
        if (sceneNodeProperty.SceneNodeObjectSet.ProjectContext != null)
        {
          ReferenceStep singleStep1 = (ReferenceStep) sceneNodeProperty.SceneNodeObjectSet.ProjectContext.ResolveProperty(RichTextBoxRangeElement.TextDecorationsProperty);
          ReferenceStep singleStep2 = (ReferenceStep) sceneNodeProperty.SceneNodeObjectSet.ProjectContext.ResolveProperty(RichTextBoxRangeElement.TextIndentPropertyId);
          if (singleStep1 != null)
          {
            this.textDecorationsProperty = sceneNodeProperty.SceneNodeObjectSet.CreateSceneNodeProperty(new PropertyReference(singleStep1), (AttributeCollection) null);
            ((UnderlineStickyButtonConverter) this.Resources[(object) "UnderlineStickyButtonConverter"]).Underline = this.textDecorationsProperty.Converter.ConvertFromString("Underline");
          }
          if (singleStep2 != null)
            this.textIndentProperty = sceneNodeProperty.SceneNodeObjectSet.CreateSceneNodeProperty(new PropertyReference(singleStep2), (AttributeCollection) null);
          this.Initialize(sceneNodeProperty.SceneNodeObjectSet);
          return true;
        }
      }
      return false;
    }

    private object Initialize(SceneNodeObjectSet sceneNodeObjectSet)
    {
      this.sceneNodeObjectSet = sceneNodeObjectSet;
      this.InitializeDesignerContext(sceneNodeObjectSet.DesignerContext);
      this.CommandBindings.Add(new CommandBinding((System.Windows.Input.ICommand) TextCategoryLayout.OnComboBoxCommitCommand, new ExecutedRoutedEventHandler(this.OnComboBoxCommit)));
      this.propertyContainersByTab.Clear();
      int index = 0;
      foreach (TabItem tabItem in (IEnumerable) this.TextTabControl.Items)
      {
        this.propertyContainersByTab.Add(new List<PropertyContainer>());
        foreach (Visual visual in ElementUtilities.GetVisualTree((Visual) tabItem.Content))
        {
          PropertyContainer propertyContainer = visual as PropertyContainer;
          if (propertyContainer != null)
            this.propertyContainersByTab[index].Add(propertyContainer);
        }
        ++index;
      }
      CollectionViewSource.GetDefaultView((object) TextCategoryLayout.markerOffsetList).CurrentChanged += new EventHandler(this.MarkerOffsetChanged);
      CollectionViewSource.GetDefaultView((object) TextCategoryLayout.markerStyleList).CurrentChanged += new EventHandler(this.MarkerStyleChanged);
      IProjectContext projectContext = sceneNodeObjectSet.ProjectContext;
      if (projectContext != null)
      {
        this.IsParagraphTabAvailable = true;
        this.IsLineIndentTabAvailable = true;
        this.IsListTabAvailable = JoltHelper.TypeSupported((ITypeResolver) projectContext, PlatformTypes.List);
        this.IsFontSubsetTaskAvailable = FontEmbedder.IsSubsetFontTargetInstalled((ITypeResolver) projectContext);
        this.IsFontEmbeddingAvailable = true;
      }
      return null;
    }

    private object DoFontEmbedding(object parameter)
    {
      try
      {
        bool flag = (bool) parameter;
        using (SceneEditTransaction editTransaction = this.sceneNodeObjectSet.ViewModel.CreateEditTransaction(StringTable.EmbedFontUndoUnit))
        {
          SourcedFontFamilyItem currentKnownFont = this.CurrentKnownFont;
          ProjectFontFamilyItem projectFontFamilyItem = currentKnownFont as ProjectFontFamilyItem;
          if (projectFontFamilyItem != null)
          {
            if (flag)
            {
              this.sceneNodeObjectSet.ViewModel.FontEmbedder.EmbedProjectFont(projectFontFamilyItem.ProjectFont);
              ProjectFont projectFont = projectFontFamilyItem.ProjectFont;
            }
            else
              this.sceneNodeObjectSet.ViewModel.FontEmbedder.UnembedProjectFont((IProjectFont) projectFontFamilyItem.ProjectFont);
          }
          SystemFontFamilyItem systemFontFamilyItem = currentKnownFont as SystemFontFamilyItem;
          if (systemFontFamilyItem != null)
          {
            if (flag)
            {
              SceneNodeProperty sceneNodeProperty = (SceneNodeProperty) this.FontFamilyPropertyContainer.get_PropertyEntry();
              if (sceneNodeProperty.ValueSource == DependencyPropertyValueSource.get_DefaultValue() || sceneNodeProperty.ValueSource == DependencyPropertyValueSource.get_InheritedValue())
              {
                using (this.sceneNodeObjectSet.ViewModel.ForceDefaultSetValue())
                  sceneNodeProperty.SetValue(sceneNodeProperty.GetValue());
              }
              this.sceneNodeObjectSet.ViewModel.DefaultView.TryExitTextEditMode();
              this.sceneNodeObjectSet.ViewModel.FontEmbedder.EmbedSystemFont(systemFontFamilyItem.SystemFontFamily);
            }
            else
            {
              editTransaction.Cancel();
              return null;
            }
          }
          editTransaction.Commit();
        }
        this.sceneNodeObjectSet.DesignerContext.ProjectManager.CurrentSolution.RefreshProject(this.sceneNodeObjectSet.DesignerContext.ActiveProject);
      }
      finally
      {
        this.OnPropertyChanged("IsFontEmbedded");
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (Delegate) (() => this.isProcessingEmbedding = false));
      }
      return null;
    }

    private void InitializeDesignerContext(DesignerContext designerContext)
    {
      designerContext.UnitsOptionsModel.PropertyChanged += new PropertyChangedEventHandler(this.UnitsOptionsModel_PropertyChanged);
      this.InitializeUnitComboBoxes(designerContext.UnitsOptionsModel.UnitType);
      this.BulletMarkerCombo.ItemsSource = (IEnumerable) TextCategoryLayout.markerStyleList;
      this.BulletOffsetCombo.ItemsSource = (IEnumerable) TextCategoryLayout.markerOffsetList;
      designerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateSceneUpdatePhase);
    }

    private void UnitsOptionsModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      UnitsOptionsModel unitsOptionsModel = sender as UnitsOptionsModel;
      if (unitsOptionsModel == null)
        return;
      this.InitializeUnitComboBoxes(unitsOptionsModel.UnitType);
    }

    private UnitTypedSize[] WrapUnitsList(double[] list, UnitType units)
    {
      UnitTypedSize[] unitTypedSizeArray = new UnitTypedSize[list.Length];
      for (int index = 0; index < list.Length; ++index)
        unitTypedSizeArray[index] = UnitTypedSize.CreateFromUnits(list[index], units);
      return unitTypedSizeArray;
    }

    private void InitializeUnitComboBoxes(UnitType unitType)
    {
      UnitTypedSize[] unitTypedSizeArray1 = this.WrapUnitsList(TextCategoryLayout.fontSizeList, unitType);
      ((DependencyObject) this.FontSizePropertyContainer).SetValue(TextCategoryLayout.ComboBoxEntriesProperty, (object) unitTypedSizeArray1);
      double[] list = TextCategoryLayout.lineHeightListNoAuto;
      if (this.sceneNodeObjectSet.ProjectContext != null && this.sceneNodeObjectSet.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsAutoLineHeight))
        list = TextCategoryLayout.lineHeightList;
      UnitTypedSize[] unitTypedSizeArray2 = this.WrapUnitsList(list, unitType);
      ((DependencyObject) this.LineHeightPropertyContainer).SetValue(TextCategoryLayout.ComboBoxEntriesProperty, (object) unitTypedSizeArray2);
      UnitTypedSize[] unitTypedSizeArray3 = this.WrapUnitsList(TextCategoryLayout.textIndentList, unitType);
      ((DependencyObject) this.FirstLineIndentPropertyContainer).SetValue(TextCategoryLayout.ComboBoxEntriesProperty, (object) unitTypedSizeArray3);
      UnitTypedSize[] unitTypedSizeArray4 = this.WrapUnitsList(TextCategoryLayout.indentList, unitType);
      ((DependencyObject) this.ParagraphSpacingBeforePropertyContainer).SetValue(TextCategoryLayout.ComboBoxEntriesProperty, (object) unitTypedSizeArray4);
      ((DependencyObject) this.ParagraphSpacingAfterPropertyContainer).SetValue(TextCategoryLayout.ComboBoxEntriesProperty, (object) unitTypedSizeArray4);
      ((DependencyObject) this.LeftIndentPropertyContainer).SetValue(TextCategoryLayout.ComboBoxEntriesProperty, (object) unitTypedSizeArray4);
      ((DependencyObject) this.RightIndentPropertyContainer).SetValue(TextCategoryLayout.ComboBoxEntriesProperty, (object) unitTypedSizeArray4);
    }

    public void Dispose()
    {
      if (this.textDecorationsProperty != null)
      {
        this.textDecorationsProperty.OnRemoveFromCategory();
        this.textDecorationsProperty = (SceneNodeProperty) null;
      }
      if (this.textIndentProperty != null)
      {
        this.textIndentProperty.OnRemoveFromCategory();
        this.textIndentProperty = (SceneNodeProperty) null;
      }
      if (this.sceneNodeObjectSet != null)
      {
        if (this.sceneNodeObjectSet.DesignerContext.UnitsOptionsModel != null)
          this.sceneNodeObjectSet.DesignerContext.UnitsOptionsModel.PropertyChanged -= new PropertyChangedEventHandler(this.UnitsOptionsModel_PropertyChanged);
        if (this.sceneNodeObjectSet.DesignerContext.SelectionManager != null)
          this.sceneNodeObjectSet.DesignerContext.SelectionManager.LateActiveSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.SelectionManager_LateSceneUpdatePhase);
        this.sceneNodeObjectSet = (SceneNodeObjectSet) null;
      }
      CollectionViewSource.GetDefaultView((object) TextCategoryLayout.markerOffsetList).CurrentChanged -= new EventHandler(this.MarkerOffsetChanged);
      CollectionViewSource.GetDefaultView((object) TextCategoryLayout.markerStyleList).CurrentChanged -= new EventHandler(this.MarkerStyleChanged);
    }

    private void SelectionManager_LateSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if ((args.DirtyViewState & (SceneViewModel.ViewStateBits.ElementSelection | SceneViewModel.ViewStateBits.TextSelection)) != SceneViewModel.ViewStateBits.None)
      {
        this.OnPropertyChanged("IsBulletsCommandEnabled");
        this.OnPropertyChanged("IsCreateHyperlinkVisible");
        this.OnPropertyChanged("IsCreateHyperlinkEnabled");
        this.OnPropertyChanged("TextDecorationsProperty");
        this.OnPropertyChanged("TextIndentProperty");
        this.OnPropertyChanged("TextAlignmentProperty");
        this.OnPropertyChanged("LineHeightProperty");
      }
      this.OnPropertyChanged("IsFontEmbedded");
      this.OnPropertyChanged("IsFontEmbeddingEnabled");
    }

    private void MarkerOffsetChanged(object sender, EventArgs e)
    {
      this.OnApplyBullets();
    }

    private void MarkerStyleChanged(object sender, EventArgs e)
    {
      this.OnApplyBullets();
    }

    private void OnApplyBullets()
    {
      if (this.sceneNodeObjectSet.ViewModel == null)
        return;
      RichTextBox editingElement = (RichTextBox) null;
      if (this.sceneNodeObjectSet.ViewModel.TextSelectionSet.TextEditProxy != null)
        editingElement = this.sceneNodeObjectSet.ViewModel.TextSelectionSet.TextEditProxy.EditingElement.PlatformSpecificObject as RichTextBox;
      if (editingElement != null)
      {
        TextMarkerStyle textMarkerStyle = (TextMarkerStyle) CollectionViewSource.GetDefaultView((object) TextCategoryLayout.markerStyleList).CurrentItem;
        List listParent = this.FindListParent(editingElement.Selection.Start.Parent);
        if (textMarkerStyle == TextMarkerStyle.None && listParent != null)
          this.RemoveList(listParent, editingElement);
        else if (textMarkerStyle != TextMarkerStyle.None && listParent == null)
        {
          EditingCommands.ToggleBullets.Execute(null, (IInputElement) editingElement);
          listParent = this.FindListParent(editingElement.Selection.Start.Parent);
        }
        if (textMarkerStyle != TextMarkerStyle.None && listParent != null)
        {
          this.ApplyMarkerOffset(listParent);
          this.ApplyMarkerStyle(listParent);
        }
      }
      this.ReturnFocus();
    }

    private void RemoveList(List list, RichTextBox editingElement)
    {
      switch (list.MarkerStyle)
      {
        case TextMarkerStyle.Disc:
        case TextMarkerStyle.Circle:
        case TextMarkerStyle.Square:
        case TextMarkerStyle.Box:
          EditingCommands.ToggleBullets.Execute(null, (IInputElement) editingElement);
          break;
        case TextMarkerStyle.LowerRoman:
        case TextMarkerStyle.UpperRoman:
        case TextMarkerStyle.LowerLatin:
        case TextMarkerStyle.UpperLatin:
        case TextMarkerStyle.Decimal:
          EditingCommands.ToggleNumbering.Execute(null, (IInputElement) editingElement);
          break;
      }
    }

    private List FindListParent(DependencyObject selectionParent)
    {
      TextElement textElement = selectionParent as TextElement;
      while (textElement != null && !(textElement is List))
        textElement = textElement.Parent as TextElement;
      return (List) textElement;
    }

    private void ApplyMarkerOffset(List list)
    {
      ICollectionView defaultView = CollectionViewSource.GetDefaultView((object) TextCategoryLayout.markerOffsetList);
      if (defaultView.CurrentItem == null)
        return;
      double num = (double) defaultView.CurrentItem;
      list.MarkerOffset = num;
    }

    private void ApplyMarkerStyle(List list)
    {
      ICollectionView defaultView = CollectionViewSource.GetDefaultView((object) TextCategoryLayout.markerStyleList);
      if (defaultView.CurrentItem == null)
        return;
      TextMarkerStyle textMarkerStyle = (TextMarkerStyle) defaultView.CurrentItem;
      list.MarkerStyle = textMarkerStyle;
    }

    private void OnCreateHyperlink()
    {
      RichTextBox richTextBox = (RichTextBox) this.sceneNodeObjectSet.ViewModel.TextSelectionSet.TextEditProxy.EditingElement.PlatformSpecificObject;
      Hyperlink hyperlink = this.FindHyperlink(richTextBox.Selection);
      string hyperlinkText = richTextBox.Selection.Start.Paragraph != richTextBox.Document.Blocks.LastBlock || richTextBox.Selection.End.Paragraph != null ? richTextBox.Selection.Text : new TextRange(richTextBox.Selection.Start, richTextBox.Selection.Start.Paragraph.ContentEnd).Text;
      Uri navigateUri = (Uri) null;
      if (hyperlink != null)
      {
        navigateUri = hyperlink.NavigateUri;
        Run childRun = this.FindChildRun(hyperlink);
        if (childRun != null)
          hyperlinkText = childRun.Text;
      }
      if (!InsertHyperlinkDialog.CreateHyperlink(ref navigateUri, ref hyperlinkText))
        return;
      if (hyperlink == null)
        hyperlink = new Hyperlink(richTextBox.Selection.Start, richTextBox.Selection.End);
      if (string.IsNullOrEmpty(navigateUri.ToString()))
        hyperlink.ClearValue(Hyperlink.NavigateUriProperty);
      else
        hyperlink.NavigateUri = navigateUri;
      hyperlink.Inlines.Clear();
      hyperlink.Inlines.Add((Inline) new Run(hyperlinkText));
    }

    private Hyperlink FindHyperlink(TextSelection textSelection)
    {
      return ((this.FindParentHyperlink(textSelection.Start) ?? this.FindParentHyperlink(textSelection.End)) ?? this.FindSiblingHyperlink(textSelection.Start, LogicalDirection.Forward, textSelection)) ?? this.FindSiblingHyperlink(textSelection.End, LogicalDirection.Backward, textSelection);
    }

    private Hyperlink FindParentHyperlink(TextPointer position)
    {
      for (FrameworkContentElement frameworkContentElement = position.Parent as FrameworkContentElement; frameworkContentElement != null; frameworkContentElement = frameworkContentElement.Parent as FrameworkContentElement)
      {
        Hyperlink hyperlink = frameworkContentElement as Hyperlink;
        if (hyperlink != null)
          return hyperlink;
      }
      return (Hyperlink) null;
    }

    private Hyperlink FindSiblingHyperlink(TextPointer position, LogicalDirection direction, TextSelection textSelection)
    {
      Inline inline = position.Parent as Inline;
      Paragraph paragraph = position.Parent as Paragraph;
      if (paragraph != null)
        inline = paragraph.Inlines.FirstInline;
      Hyperlink hyperlink = inline as Hyperlink;
      while (inline != null && hyperlink == null)
      {
        if (direction == LogicalDirection.Forward)
        {
          inline = inline.NextInline;
          hyperlink = inline as Hyperlink;
        }
        else
        {
          inline = inline.PreviousInline;
          hyperlink = inline as Hyperlink;
        }
      }
      if (hyperlink != null && (textSelection.End.CompareTo(hyperlink.ContentStart) < 0 || textSelection.Start.CompareTo(hyperlink.ContentEnd) > 0))
        hyperlink = (Hyperlink) null;
      return hyperlink;
    }

    private Run FindChildRun(Hyperlink hyperlink)
    {
      foreach (Inline inline in (TextElementCollection<Inline>) hyperlink.Inlines)
      {
        Run run = inline as Run;
        if (run != null)
          return run;
      }
      return (Run) null;
    }

    private void OnComboBoxCommit(object sender, ExecutedRoutedEventArgs e)
    {
      this.ReturnFocus();
    }

    private void ReturnFocus()
    {
      if (this.sceneNodeObjectSet.DesignerContext.ActiveView == null)
        return;
      this.sceneNodeObjectSet.DesignerContext.ActiveView.ReturnFocus();
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent(this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/categoryeditors/text/textcategorylayout.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.Root = (TextCategoryLayout) target;
          break;
        case 2:
          this.TextTabControl = (FocusDenyingTabControl) target;
          break;
        case 3:
          this.FontTab = (TabItem) target;
          break;
        case 4:
          this.FontFamilyPropertyContainer = (PropertyContainer) target;
          break;
        case 5:
          this.FontSizePropertyContainer = (PropertyContainer) target;
          break;
        case 6:
          this.BoldPropertyContainer = (PropertyContainer) target;
          break;
        case 7:
          this.ItalicPropertyContainer = (PropertyContainer) target;
          break;
        case 8:
          this.UnderlinePropertyContainer = (PropertyContainer) target;
          break;
        case 9:
          this.HyperlinkButton = (Button) target;
          break;
        case 10:
          this.EmbedCheckbox = (CheckBox) target;
          break;
        case 11:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.OnFontEmbeddingSetupClick);
          break;
        case 12:
          this.ParagraphTab = (TabItem) target;
          break;
        case 13:
          this.LineHeightIcon = (Icon) target;
          break;
        case 14:
          this.LineHeightPropertyContainer = (PropertyContainer) target;
          break;
        case 15:
          this.PSBIcon = (Icon) target;
          break;
        case 16:
          this.ParagraphSpacingBeforePropertyContainer = (PropertyContainer) target;
          break;
        case 17:
          this.PSAIcon = (Icon) target;
          break;
        case 18:
          this.ParagraphSpacingAfterPropertyContainer = (PropertyContainer) target;
          break;
        case 19:
          this.AlignmentIcon = (Icon) target;
          break;
        case 20:
          this.AlignPropertyContainer = (PropertyContainer) target;
          break;
        case 21:
          this.LineIndentTab = (TabItem) target;
          break;
        case 22:
          this.LeftIndentIcon = (Icon) target;
          break;
        case 23:
          this.LeftIndentPropertyContainer = (PropertyContainer) target;
          break;
        case 24:
          this.RightIndentIcon = (Icon) target;
          break;
        case 25:
          this.RightIndentPropertyContainer = (PropertyContainer) target;
          break;
        case 26:
          this.FirstLineIndentIcon = (Icon) target;
          break;
        case 27:
          this.FirstLineIndentPropertyContainer = (PropertyContainer) target;
          break;
        case 28:
          this.ListTab = (TabItem) target;
          break;
        case 29:
          this.BulletMarkerCombo = (ComboBox) target;
          break;
        case 30:
          this.BulletOffsetCombo = (ComboBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
