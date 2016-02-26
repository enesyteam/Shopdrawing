// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Documents.IView
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.ComponentModel;

namespace Microsoft.Expression.Framework.Documents
{
  public interface IView : INotifyPropertyChanged, IDisposable
  {
    string Caption { get; }

    bool IsDirty { get; }

    string TabToolTip { get; }

    object ActiveEditor { get; }

    void ReturnFocus();

    void Initialize();

    void Deactivating();

    void Activated();

    void Deactivated();
  }
}
