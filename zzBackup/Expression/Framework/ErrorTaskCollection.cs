// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ErrorTaskCollection
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.Framework
{
  public sealed class ErrorTaskCollection : ObservableCollection<IErrorTask>, IErrorTaskCollection, ICollection<IErrorTask>, IEnumerable<IErrorTask>, IEnumerable
  {
    private DateTime timestamp;

    public DateTime Timestamp
    {
      get
      {
        return this.timestamp;
      }
      set
      {
        this.timestamp = value;
        this.OnTimestampChanged();
      }
    }

    public event EventHandler TimestampChanged;

    public ErrorTaskCollection()
    {
      this.Timestamp = DateTime.Now;
    }

    protected override void ClearItems()
    {
      this.Timestamp = DateTime.Now;
      base.ClearItems();
    }

    private void OnTimestampChanged()
    {
      if (this.TimestampChanged == null)
        return;
      this.TimestampChanged((object) this, EventArgs.Empty);
    }
  }
}
