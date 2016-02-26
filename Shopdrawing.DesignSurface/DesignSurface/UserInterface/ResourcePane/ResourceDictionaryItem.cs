// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ResourceDictionaryItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public class ResourceDictionaryItem : ResourceItem
  {
    private DocumentCompositeNode dictionary;
    private DocumentNodeMarker marker;

    public override Type EffectiveType
    {
      get
      {
        return typeof (ResourceDictionary);
      }
    }

    public override object ToolTip
    {
      get
      {
        string str = this.DesignTimeSource;
        if (string.IsNullOrEmpty(str))
          str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ElementTimelineItemBracketedName, new object[1]
          {
            (object) this.DocumentNode.TargetType
          });
        return (object) str;
      }
    }

    public string Source
    {
      get
      {
        Uri sourceUri = this.SourceUri;
        if (!(sourceUri != (Uri) null))
          return (string) null;
        return sourceUri.OriginalString;
      }
    }

    public string DisplayName
    {
      get
      {
        string source = this.Source;
        if (source != null)
          return Path.GetFileName(source);
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ElementTimelineItemBracketedName, new object[1]
        {
          (object) this.DocumentNode.TargetType
        });
      }
    }

    public string DesignTimeSource
    {
      get
      {
        Uri designTimeSourceUri = this.DesignTimeSourceUri;
        if (!(designTimeSourceUri != (Uri) null))
          return (string) null;
        return designTimeSourceUri.OriginalString;
      }
    }

    public Uri SourceUri
    {
      get
      {
        return this.dictionary.GetUriValue(ResourceDictionaryNode.SourceProperty);
      }
    }

    public Uri DesignTimeSourceUri
    {
      get
      {
        Uri sourceUri = this.SourceUri;
        if (!(sourceUri != (Uri) null))
          return (Uri) null;
        return this.dictionary.Context.MakeDesignTimeUri(sourceUri);
      }
    }

    public override DocumentNode DocumentNode
    {
      get
      {
        return (DocumentNode) this.dictionary;
      }
    }

    public override DocumentNodeMarker Marker
    {
      get
      {
        return this.marker;
      }
    }

    public ICommand EditCommand
    {
      get
      {
        return (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() =>
        {
          try
          {
            if (string.IsNullOrEmpty(this.DesignTimeSource))
              return;
            IDocumentRoot documentRoot = this.Container.ProjectContext.GetDocumentRoot(this.DesignTimeSource);
            if (documentRoot == null)
              return;
            ISceneViewHost sceneViewHost = (ISceneViewHost) this.Container.ProjectContext.GetService(typeof (ISceneViewHost));
            if (sceneViewHost == null)
              return;
            sceneViewHost.OpenView(documentRoot, true);
          }
          catch (FileNotFoundException ex)
          {
            this.Container.ViewModel.DesignerContext.MessageDisplayService.ShowError(ex.Message);
          }
          catch (NotSupportedException ex)
          {
            this.Container.ViewModel.DesignerContext.MessageDisplayService.ShowError(ex.Message);
          }
        }))
        {
          IsEnabled = !string.IsNullOrEmpty(this.DesignTimeSource)
        };
      }
    }

    public ResourceDictionaryItem(ResourceManager resourceManager, ResourceContainer resourceContainer, DocumentCompositeNode dictionary)
      : base(resourceManager, resourceContainer)
    {
      this.dictionary = dictionary;
      this.marker = dictionary.Marker;
    }

    public override void InteractiveDelete()
    {
      ResourceDictionaryNode resourceDictionaryNode = (ResourceDictionaryNode) this.Container.ViewModel.GetSceneNode((DocumentNode) this.dictionary);
      if (this.DesignerContext.MessageDisplayService.ShowMessage(new MessageBoxArgs()
      {
        Message = StringTable.RemovingLinkToExternalDictionaryWarningMessage,
        Button = MessageBoxButton.YesNo,
        Image = MessageBoxImage.Exclamation
      }) != MessageBoxResult.Yes)
        return;
      using (SceneEditTransaction editTransaction = this.Container.ViewModel.CreateEditTransaction(StringTable.UndoUnitDeleteResource))
      {
        this.Container.ResourceDictionaryNode.MergedDictionaries.Remove(resourceDictionaryNode);
        editTransaction.Commit();
      }
    }
  }
}
