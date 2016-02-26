using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewVisual : IViewObject
	{
		Size DesiredSize
		{
			get;
		}

		bool IsTransformed
		{
			get;
		}

		bool IsVisible
		{
			get;
		}

		Size RenderSize
		{
			get;
		}

		System.Windows.Visibility Visibility
		{
			get;
		}

		int VisualChildrenCount
		{
			get;
		}

		IViewObject VisualParent
		{
			get;
		}

		bool CheckAccess();

		bool GetIsVisible(IParentChainProvider parentChainProvider);

		Rect GetLayoutSlot();

		IViewVisual GetVisualChild(int index);

		void HitTest(ViewHitTestFilterCallback filterCallback, ViewHitTestResultCallback resultCallback, HitTestParameters hitTestParameters);

		void InvalidateMeasure();

		bool IsAncestorOf(IViewVisual visual);

		bool SimulateGoToState(IViewVisual visualStateManagerHostElement, string stateName, string groupName, bool useTransitions, bool forceExtended, StateTransitionPreviewInfo info, ITypeResolver typeResolver, ViewStoryboardApplyOptions storyboardOptions);

		void StopAllStateTransitions(IViewVisual visualStateManagerHostElement, bool forceExtended, ITypeResolver typeResolver, ViewStoryboardApplyOptions storyboardOptions);

		GeneralTransform TransformToVisual(IViewVisual visual);

		void UpdateLayout();
	}
}