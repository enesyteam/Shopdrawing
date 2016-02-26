// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.RequirementSubscription
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using System;

namespace Microsoft.Windows.Design
{
  public abstract class RequirementSubscription
  {
    private EventHandler _requirementChanged;
    private RequirementAttribute _requirement;

    public RequirementAttribute Requirement
    {
      get
      {
        return this._requirement;
      }
    }

    public event EventHandler RequirementChanged
    {
      add
      {
        if (value == null)
          return;
        bool flag = this._requirementChanged == null;
        this._requirementChanged += value;
        if (!flag)
          return;
        this.Subscribe();
      }
      remove
      {
        bool flag = this._requirementChanged != null;
        this._requirementChanged -= value;
        if (this._requirementChanged != null || !flag)
          return;
        this.Unsubscribe();
      }
    }

    protected RequirementSubscription(RequirementAttribute requirement)
    {
      if (requirement == null)
        throw new ArgumentNullException("requirement");
      this._requirement = requirement;
    }

    protected void OnRequirementChanged()
    {
      if (this._requirementChanged == null)
        return;
      this._requirementChanged((object) this, EventArgs.Empty);
    }

    protected abstract void Subscribe();

    protected abstract void Unsubscribe();
  }
}
