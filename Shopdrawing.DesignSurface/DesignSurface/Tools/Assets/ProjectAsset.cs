// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.ProjectAsset
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class ProjectAsset : Asset
  {
    private IProjectItem projectItem;
    private string relativeLocation;

    private string RelativeLocation
    {
      get
      {
        if (this.projectItem != null && this.projectItem.DocumentReference != (DocumentReference) null)
          this.relativeLocation = this.projectItem.ProjectRelativeDocumentReference;
        return this.relativeLocation;
      }
    }

    public override string Name
    {
      get
      {
        string str = this.projectItem.ProjectRelativeDocumentReference;
        int num = str.LastIndexOf('\\');
        if (num >= 0)
          str = str.Substring(num + 1);
        return str;
      }
    }

    public override string TypeName
    {
      get
      {
        return this.projectItem.DocumentType.Name;
      }
    }

    public override string Location
    {
      get
      {
        string str = this.projectItem.ProjectRelativeDocumentReference;
        int num = str.LastIndexOf('\\');
        if (num >= 0)
          str = str.Substring(0, num + 1);
        return str;
      }
    }

    public override System.Windows.Size SourcePixelSize
    {
      get
      {
        AssetDocumentType assetDocumentType = this.projectItem.DocumentType as AssetDocumentType;
        if (assetDocumentType != null)
          return assetDocumentType.GetSourcePixelSize(this.projectItem);
        return System.Windows.Size.Empty;
      }
    }

    public ProjectAsset(IProjectItem projectItem)
    {
      this.projectItem = projectItem;
      this.relativeLocation = this.projectItem.ProjectRelativeDocumentReference;
    }

    protected override void ComputeCategories(IList<AssetCategoryPath> categoryPaths, AssetTypeHelper typeHelper)
    {
      if (this.projectItem.DocumentType is AssetDocumentType)
        categoryPaths.Add((AssetCategoryPath) PresetAssetCategoryPath.MediaRoot);
      categoryPaths.Add((AssetCategoryPath) PresetAssetCategoryPath.ProjectRoot);
    }

    protected override AssetInfoModel CreateAssetInfoModel(IProject project)
    {
      return (AssetInfoModel) ProjectAssetInfoModel.Create(this.projectItem);
    }

    protected override SceneNode InternalCreateInstance(ISceneInsertionPoint insertionPoint, Rect rect, OnCreateInstanceAction action)
    {
      AssetDocumentType assetDocumentType = this.projectItem.DocumentType as AssetDocumentType;
      if (assetDocumentType != null)
        assetDocumentType.AddToDocument(this.projectItem, (IView) insertionPoint.SceneNode.ViewModel.DefaultView, insertionPoint, rect);
      else
        this.projectItem.DocumentType.AddToDocument(this.projectItem, (IView) insertionPoint.SceneNode.ViewModel.DefaultView);
      return (SceneNode) null;
    }

    public override SceneNode CreatePrototypeInstance(ISceneInsertionPoint insertionPoint)
    {
      return (SceneNode) null;
    }

    protected override bool InternalCanCreateInstance(ISceneInsertionPoint insertionPoint)
    {
      AssetDocumentType assetDocumentType = this.projectItem.DocumentType as AssetDocumentType;
      IView view = (IView) insertionPoint.SceneNode.ViewModel.DefaultView;
      if (assetDocumentType != null)
        return assetDocumentType.CanInsertTo(this.projectItem, view, insertionPoint);
      return this.projectItem.DocumentType.CanInsertTo(this.projectItem, view);
    }

    public override int CompareTo(Asset other)
    {
      ProjectAsset projectAsset = other as ProjectAsset;
      if (projectAsset == null)
        return base.CompareTo(other);
      return string.Compare(this.RelativeLocation, projectAsset.RelativeLocation, StringComparison.OrdinalIgnoreCase);
    }

    protected override DrawingBrush CreateIconSourceBrush()
    {
      return new DrawingBrush()
      {
        Drawing = (Drawing) new ImageDrawing()
        {
          ImageSource = this.GetIconImageSource(),
          Rect = new Rect(0.0, 0.0, 32.0, 32.0)
        }
      };
    }

    private ImageSource GetIconImageSource()
    {
      Icon icon = (Icon) null;
      try
      {
        icon = Icon.ExtractAssociatedIcon(this.projectItem.DocumentReference.Path);
      }
      catch (ArgumentException ex)
      {
      }
      catch (FileNotFoundException ex)
      {
      }
      if (icon == null)
        return this.projectItem.DocumentType.Image;
      BitmapImage bitmapImage = (BitmapImage) null;
      MemoryStream memoryStream;
      using (icon)
      {
        using (Bitmap bitmap = icon.ToBitmap())
        {
          memoryStream = new MemoryStream();
          bitmap.Save((Stream) memoryStream, ImageFormat.Png);
        }
      }
      if (memoryStream != null)
      {
        bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        memoryStream.Seek(0L, SeekOrigin.Begin);
        bitmapImage.StreamSource = (Stream) memoryStream;
        bitmapImage.EndInit();
      }
      return (ImageSource) bitmapImage;
    }
  }
}
