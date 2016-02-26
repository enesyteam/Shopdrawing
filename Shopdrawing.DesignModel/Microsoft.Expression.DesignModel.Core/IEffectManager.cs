using Microsoft.Expression.DesignModel.InstanceBuilders;
using System;

namespace Microsoft.Expression.DesignModel.Core
{
	public interface IEffectManager
	{
		bool DisableEffects
		{
			get;
			set;
		}

		bool SuspendAllRegistration
		{
			get;
			set;
		}

		void ClearCache();

		void RegisterShadowEffect(object instance, object effectInstance, bool designTimeEnabled);

		void UnregisterEffect(ViewNode parent, ViewNode child);
	}
}