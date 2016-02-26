// Decompiled with JetBrains decompiler
// Type: MS.Internal.Features.PolicyDrivenToolFeatureConnector`1
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Policies;

namespace MS.Internal.Features
{
  internal abstract class PolicyDrivenToolFeatureConnector<FeatureProviderType> : PolicyDrivenFeatureConnector<FeatureProviderType> where FeatureProviderType : FeatureProvider
  {
    private Tool _currentTool;

    protected Tool CurrentTool
    {
      get
      {
        return this._currentTool;
      }
    }

    protected PolicyDrivenToolFeatureConnector(FeatureManager manager)
      : base(manager)
    {
      this.Context.Items.Subscribe<Tool>((SubscribeContextCallback<Tool>) (newTool => this.UpdateCurrentTool(newTool)));
    }

    protected virtual void UpdateCurrentTool(Tool newTool)
    {
      if (this._currentTool == newTool)
        return;
      this._currentTool = newTool;
      this.UpdateFeatureProviders();
    }
  }
}
