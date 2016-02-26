// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.ImageDocumentType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal class ImageDocumentType : AssetDocumentType
  {
    protected override string ImagePath
    {
      get
      {
        return "Resources\\Documents\\Bmp.png";
      }
    }

    public override string Name
    {
      get
      {
        return "Image";
      }
    }

    public override string Description
    {
      get
      {
        return StringTable.ImageDocumentTypeDescription;
      }
    }

    protected override string FileNameBase
    {
      get
      {
        return StringTable.ImageDocumentTypeFileNameBase;
      }
    }

    public override string[] FileExtensions
    {
      get
      {
        return new string[1]
        {
          ".bmp"
        };
      }
    }

    public ImageDocumentType(DesignerContext designerContext)
      : base(designerContext)
    {
    }

    public override Size GetSourcePixelSize(IProjectItem projectItem)
    {
      try
      {
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
        bitmapImage.UriSource = new Uri(projectItem.DocumentReference.Path);
        bitmapImage.EndInit();
        return new Size(bitmapImage.Width, bitmapImage.Height);
      }
      catch (Exception ex)
      {
        return Size.Empty;
      }
    }

    public override bool CanAddToProject(IProject project)
    {
      IProjectContext projectContext = (IProjectContext) ProjectXamlContext.GetProjectContext(project);
      if (projectContext == null || !projectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        return false;
      return base.CanAddToProject(project);
    }

    protected override SceneElement CreateElement(SceneViewModel viewModel, ISceneInsertionPoint insertionPoint, string url)
    {
      try
      {
        ITypeId targetType = (ITypeId) viewModel.ProjectContext.ResolveType(PlatformTypes.Image);
        ImageElement imageElement = (ImageElement) viewModel.CreateSceneNode(targetType);
        imageElement.Uri = url;
        if (viewModel.DesignerContext.ProjectManager.OptionsModel.NameInteractiveElementsByDefault)
          new SceneNodeIDHelper(viewModel, insertionPoint.SceneNode ?? viewModel.ViewRoot).SetValidName((SceneNode) imageElement, url);
        imageElement.SetValueAsWpf(ImageElement.StretchProperty, (object) Stretch.Fill);
        return (SceneElement) imageElement;
      }
      catch (WebException ex)
      {
        this.DesignerContext.MessageDisplayService.ShowError(new ErrorArgs()
        {
          Message = StringTable.ImageDocumentTypeImageInsertFailureMessage,
          Exception = (Exception) ex
        });
        return (SceneElement) null;
      }
    }

    protected override bool DoesNodeReferenceUrl(DocumentNode node, string url)
    {
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      if (documentCompositeNode != null)
      {
        Uri uriValue = DocumentNodeHelper.GetUriValue(documentCompositeNode.Properties[ImageElement.SourceProperty]);
        string uriString = uriValue != (Uri) null ? uriValue.OriginalString : (string) null;
        if (!string.IsNullOrEmpty(uriString))
        {
          Uri uri = node.Context.MakeDesignTimeUri(new Uri(uriString, UriKind.RelativeOrAbsolute));
          if (!uri.IsAbsoluteUri)
            return false;
          string localPath = uri.LocalPath;
          if (StringComparer.OrdinalIgnoreCase.Compare(localPath, url) == 0)
            return true;
          return StringComparer.OrdinalIgnoreCase.Compare(localPath.Replace("file:///", "").Replace("/", "\\"), url) == 0;
        }
      }
      return false;
    }

    protected override double GetNativeWidth(SceneElement element)
    {
      ImageElement imageElement = element as ImageElement;
      double num = 0.0;
      if (imageElement != null && imageElement.IsViewObjectValid)
      {
        BitmapImage imageSafe = this.GetImageSafe(imageElement);
        if (imageSafe != null)
          num = (double) imageSafe.PixelWidth;
      }
      if (num == 0.0)
        num = base.GetNativeWidth(element);
      return num;
    }

    protected override double GetNativeHeight(SceneElement element)
    {
      ImageElement imageElement = element as ImageElement;
      double num = 0.0;
      if (imageElement != null && imageElement.IsViewObjectValid)
      {
        BitmapImage imageSafe = this.GetImageSafe(imageElement);
        if (imageSafe != null)
          num = (double) imageSafe.PixelHeight;
      }
      if (num == 0.0)
        num = base.GetNativeHeight(element);
      return num;
    }

    public override void OnItemInvalidating(IProjectItem projectItem, DocumentReference oldReference)
    {
      DesignerBitmapCache.InvalidateUri(new Uri(oldReference.Path, UriKind.Absolute));
      base.OnItemInvalidating(projectItem, oldReference);
    }

    protected override void RefreshInstance(SceneElement element, SceneDocument sceneDocument, string url)
    {
      ImageElement imageElement = element as ImageElement;
      if (imageElement == null)
        return;
      string uri = imageElement.Uri;
      imageElement.Uri = (string) null;
      imageElement.Uri = uri;
    }

    private BitmapImage GetImageSafe(ImageElement imageElement)
    {
      Uri uri = new Uri(imageElement.Uri, UriKind.RelativeOrAbsolute);
      Uri uriSource = imageElement.ProjectContext.MakeDesignTimeUri(uri, imageElement.DocumentContext.DocumentUrl);
      BitmapImage bitmapImage = (BitmapImage) null;
      try
      {
        bitmapImage = new BitmapImage(uriSource);
      }
      catch (IOException ex)
      {
      }
      catch (UnauthorizedAccessException ex)
      {
      }
      catch (NotSupportedException ex)
      {
      }
      return bitmapImage;
    }

    protected override void LoadImageIcon(string path)
    {
      this.CachedImage = Microsoft.Expression.DesignSurface.FileTable.GetImageSource(path);
    }
  }
}
