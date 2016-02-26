// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Model.ModelEditingScope
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal.Properties;
using System;

namespace Microsoft.Windows.Design.Model
{
  public abstract class ModelEditingScope : IDisposable
  {
    private string _description;
    private bool _completed;
    private bool _reverted;

    public string Description
    {
      get
      {
        if (this._description != null)
          return this._description;
        return string.Empty;
      }
      set
      {
        this._description = value;
      }
    }

    ~ModelEditingScope()
    {
      this.Dispose(false);
    }

    public void Complete()
    {
      if (this._reverted)
        throw new InvalidOperationException(Resources.Error_EditingScopeReverted);
      if (this._completed)
        throw new InvalidOperationException(Resources.Error_EdtingScopeCompleted);
      if (this.CanComplete())
      {
        bool flag = false;
        this._completed = true;
        try
        {
          this.OnComplete();
          flag = true;
        }
        finally
        {
          if (flag)
            GC.SuppressFinalize((object) this);
          else
            this._completed = false;
        }
      }
      else
        this.Revert();
    }

    public void Revert()
    {
      if (this._completed)
        throw new InvalidOperationException(Resources.Error_EdtingScopeCompleted);
      if (this._reverted)
        return;
      bool flag = false;
      this._reverted = true;
      try
      {
        this.OnRevert(false);
        flag = true;
      }
      finally
      {
        if (flag)
          GC.SuppressFinalize((object) this);
        else
          this._reverted = false;
      }
    }

    public virtual void Update()
    {
    }

    public void Dispose()
    {
      this.Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this._completed || this._reverted)
        return;
      if (disposing)
        this.Revert();
      else
        this.OnRevert(true);
    }

    protected abstract void OnComplete();

    protected abstract bool CanComplete();

    protected abstract void OnRevert(bool finalizing);
  }
}
