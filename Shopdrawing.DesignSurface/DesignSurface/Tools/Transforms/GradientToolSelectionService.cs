// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.GradientToolSelectionService
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using System;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class GradientToolSelectionService : INotifyPropertyChanged, IDisposable
  {
    private int index;
    private DocumentNode adornedBrush;

    public int Index
    {
      get
      {
        return this.index;
      }
      set
      {
        if (this.index == value)
          return;
        this.index = value;
        this.OnPropertyChanged("Index");
      }
    }

    public DocumentNode AdornedBrush
    {
      get
      {
        return this.adornedBrush;
      }
      set
      {
        if (this.adornedBrush == value)
          return;
        this.adornedBrush = value;
        this.OnPropertyChanged("AdornedBrush");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void Dispose()
    {
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
