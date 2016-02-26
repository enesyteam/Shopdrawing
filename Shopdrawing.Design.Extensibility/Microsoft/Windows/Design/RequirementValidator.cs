// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.RequirementValidator
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

using Microsoft.Windows.Design.Features;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Windows.Design
{
  public sealed class RequirementValidator
  {
    private static object _syncLock = new object();
    private FeatureManager _featureManager;
    private Type _type;
    private IEnumerable<RequirementAttribute> _requirements;
    private EventHandler _requirementsChanged;
    private IEnumerable<RequirementSubscription> _subscriptions;
    private static Dictionary<Type, IEnumerable<RequirementAttribute>> _requirementCache;

    public bool MeetsRequirements
    {
      get
      {
        int length = 0;
        int num = 0;
        foreach (RequirementAttribute requirementAttribute in this.Requirements)
        {
          if (!requirementAttribute.MeetsRequirement(this._featureManager.Context))
          {
            if (requirementAttribute.AllRequired)
              return false;
            ++num;
          }
          if (!requirementAttribute.AllRequired)
            ++length;
        }
        if (num > 0)
        {
          Type[] typeArray = new Type[length];
          bool[] flagArray = new bool[length];
          int index1 = 0;
          foreach (RequirementAttribute requirementAttribute in this.Requirements)
          {
            if (!requirementAttribute.AllRequired)
            {
              Type type = requirementAttribute.GetType();
              int index2 = -1;
              for (int index3 = 0; index3 < index1; ++index3)
              {
                if (typeArray[index3] == type)
                {
                  index2 = index3;
                  break;
                }
              }
              if (index2 == -1)
              {
                typeArray[index1] = type;
                index2 = index1;
                ++index1;
              }
              if (!flagArray[index2])
                flagArray[index2] = requirementAttribute.MeetsRequirement(this._featureManager.Context);
            }
          }
          for (int index2 = 0; index2 < index1; ++index2)
          {
            if (!flagArray[index2])
              return false;
          }
        }
        return true;
      }
    }

    public IEnumerable<RequirementAttribute> PendingRequirements
    {
      get
      {
        foreach (RequirementAttribute requirementAttribute in this.Requirements)
        {
          if (!requirementAttribute.MeetsRequirement(this._featureManager.Context))
            yield return requirementAttribute;
        }
      }
    }

    public IEnumerable<RequirementAttribute> Requirements
    {
      get
      {
        if (this._requirements == null)
        {
          lock (RequirementValidator._syncLock)
          {
            if (RequirementValidator._requirementCache == null)
              RequirementValidator._requirementCache = new Dictionary<Type, IEnumerable<RequirementAttribute>>();
            if (!RequirementValidator._requirementCache.TryGetValue(this._type, out this._requirements))
            {
              object[] local_0 = this._type.GetCustomAttributes(typeof (RequirementAttribute), true);
              RequirementAttribute[] local_1 = new RequirementAttribute[local_0.Length];
              Array.Copy((Array) local_0, (Array) local_1, local_0.Length);
              this._requirements = (IEnumerable<RequirementAttribute>) local_1;
              RequirementValidator._requirementCache[this._type] = this._requirements;
            }
          }
        }
        return this._requirements;
      }
    }

    public Type Type
    {
      get
      {
        return this._type;
      }
    }

    public event EventHandler RequirementsChanged
    {
      add
      {
        if (value == null)
          return;
        bool flag = this._requirementsChanged == null;
        this._requirementsChanged += value;
        if (!flag)
          return;
        this.SubscribeRequirements();
      }
      remove
      {
        bool flag = this._requirementsChanged != null;
        this._requirementsChanged -= value;
        if (this._requirementsChanged != null || !flag)
          return;
        this.UnsubscribeRequirements();
      }
    }

    public RequirementValidator(FeatureManager featureManager, Type type)
    {
      if (featureManager == null)
        throw new ArgumentNullException("featureManager");
      if (type == null)
        throw new ArgumentNullException("type");
      this._featureManager = featureManager;
      this._type = type;
    }

    private void OnRequirementChanged(object sender, EventArgs e)
    {
      if (this._requirementsChanged == null)
        return;
      this._requirementsChanged((object) this, EventArgs.Empty);
    }

    private void SubscribeRequirements()
    {
      List<RequirementSubscription> list = new List<RequirementSubscription>();
      foreach (RequirementAttribute requirementAttribute in this.Requirements)
      {
        RequirementSubscription subscription = requirementAttribute.CreateSubscription(this._featureManager.Context);
        subscription.RequirementChanged += new EventHandler(this.OnRequirementChanged);
        list.Add(subscription);
      }
      this._subscriptions = (IEnumerable<RequirementSubscription>) list;
    }

    [Conditional("DEBUG")]
    internal static void Trace(string format, params object[] args)
    {
    }

    private void UnsubscribeRequirements()
    {
      if (this._subscriptions == null)
        return;
      foreach (RequirementSubscription requirementSubscription in this._subscriptions)
        requirementSubscription.RequirementChanged -= new EventHandler(this.OnRequirementChanged);
      this._subscriptions = (IEnumerable<RequirementSubscription>) null;
    }
  }
}
