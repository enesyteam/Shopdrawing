using System;
using System.Windows.Media;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public delegate HitTestFilterBehavior ViewHitTestFilterCallback(IViewObject potentialHitTestTarget, IParentChainProvider parentChainProvider);
}