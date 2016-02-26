using Microsoft.Expression.DesignModel.InstanceBuilders;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Core
{
	public class EffectManager<I, E> : IEffectManager
	where I : class
	where E : class
	{
		private static bool StopRegistration;

		private bool effectsDisabled;

		private bool changingEffectState;

		private IList<EffectManager<I, E>.EffectEntry<I, E>> effectRegistrar;

		private ApplyEffectMethod<I, E> applyEffectMethod;

		public bool DisableEffects
		{
			get
			{
				return this.effectsDisabled;
			}
			set
			{
				this.effectsDisabled = value;
				this.ChangeEffectEnableState();
			}
		}

		public bool SuspendAllRegistration
		{
			get
			{
				return EffectManager<I, E>.StopRegistration;
			}
			set
			{
				EffectManager<I, E>.StopRegistration = value;
			}
		}

		public EffectManager(ApplyEffectMethod<I, E> applyEffectMethod)
		{
			this.effectRegistrar = new List<EffectManager<I, E>.EffectEntry<I, E>>();
			this.applyEffectMethod = applyEffectMethod;
		}

		private void ChangeEffectEnableState()
		{
			this.changingEffectState = true;
			for (int i = this.effectRegistrar.Count - 1; i >= 0; i--)
			{
				EffectManager<I, E>.EffectEntry<I, E> item = this.effectRegistrar[i];
				I i1 = WeakReferenceHelper.Unwrap<I>(item.instance);
				if (i1 != null)
				{
					this.applyEffectMethod(i1);
				}
				else
				{
					this.effectRegistrar.RemoveAt(i);
				}
			}
			this.changingEffectState = false;
		}

		public void ClearCache()
		{
			this.effectRegistrar.Clear();
		}

		public void RegisterShadowEffect(object instance, object effectInstance, bool designTimeEnabled)
		{
			if (this.changingEffectState || this.SuspendAllRegistration)
			{
				return;
			}
			I i = (I)(instance as I);
			if (i == null)
			{
				return;
			}
			if ((E)(effectInstance as E) != null)
			{
				EffectManager<I, E>.EffectEntry<I, E> effectEntry = new EffectManager<I, E>.EffectEntry<I, E>()
				{
					instance = new WeakReference((object)i)
				};
				this.effectRegistrar.Add(effectEntry);
			}
		}

		public void UnregisterEffect(ViewNode parent, ViewNode child)
		{
			I instance = (I)(child.Instance as I);
			if ((E)(child.Instance as E) != null)
			{
				instance = (I)(parent.Instance as I);
			}
			if (instance == null)
			{
				return;
			}
			for (int i = this.effectRegistrar.Count - 1; i >= 0; i--)
			{
				EffectManager<I, E>.EffectEntry<I, E> item = this.effectRegistrar[i];
				I i1 = WeakReferenceHelper.Unwrap<I>(item.instance);
				if (i1 == null)
				{
					this.effectRegistrar.RemoveAt(i);
				}
				else if (object.Equals(i1, instance))
				{
					this.effectRegistrar.RemoveAt(i);
				}
			}
		}

		private struct EffectEntry<F, G>
		where F : class
		where G : class
		{
			public WeakReference instance;
		}
	}
}