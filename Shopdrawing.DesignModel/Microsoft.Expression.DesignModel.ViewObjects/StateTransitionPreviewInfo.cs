using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public class StateTransitionPreviewInfo
	{
		public MethodInfo RemoveTransitionCompletedMethodInfo
		{
			get;
			set;
		}

		public Microsoft.Expression.DesignModel.ViewObjects.TransitionCompletedCallback TransitionCompletedCallback
		{
			get;
			set;
		}

		public Delegate TransitionCompletedDelegate
		{
			get;
			set;
		}

		public object TransitioningGroup
		{
			get;
			set;
		}

		public StateTransitionPreviewInfo(Microsoft.Expression.DesignModel.ViewObjects.TransitionCompletedCallback completedCallback)
		{
			this.TransitionCompletedCallback = completedCallback;
		}

		public void AttachTransitionCompletedEvent(Type handlerType)
		{
			MethodInfo method = this.GetType().GetMethod("OnTransitionCompleted");
			this.TransitionCompletedDelegate = Delegate.CreateDelegate(handlerType, this, method);
		}

		public void OnTransitionCompleted(object sender, EventArgs e)
		{
			MethodInfo removeTransitionCompletedMethodInfo = this.RemoveTransitionCompletedMethodInfo;
			object transitioningGroup = this.TransitioningGroup;
			object[] transitionCompletedDelegate = new object[] { this.TransitionCompletedDelegate };
			removeTransitionCompletedMethodInfo.Invoke(transitioningGroup, transitionCompletedDelegate);
			if (this.TransitionCompletedCallback != null)
			{
				this.TransitionCompletedCallback();
			}
		}
	}
}