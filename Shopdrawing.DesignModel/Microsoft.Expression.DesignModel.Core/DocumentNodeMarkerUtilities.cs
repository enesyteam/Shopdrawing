using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Core
{
	public static class DocumentNodeMarkerUtilities
	{
		public static ReferenceStep GetReferenceStep(DocumentNodeMarker marker)
		{
			Type type;
			if (marker.Parent == null)
			{
				return null;
			}
			if (!marker.IsChild)
			{
				IPropertyId property = marker.Property;
				ReferenceStep referenceStep = property as ReferenceStep ?? marker.DocumentContext.TypeResolver.ResolveProperty(property) as ReferenceStep;
				return referenceStep;
			}
			int childIndex = marker.ChildIndex;
			DocumentNodeMarker parent = marker.Parent;
			type = (parent.Node == null ? PlatformTypeHelper.GetPropertyType(parent.Property) : parent.Node.TargetType);
			return IndexedClrPropertyReferenceStep.GetReferenceStep(marker.DocumentContext.TypeResolver, type, childIndex, false);
		}

		public static Stack<ReferenceStep> PropertyReferencePath(DocumentNodeMarker marker, DocumentNodeMarker ancestor)
		{
			Stack<ReferenceStep> referenceSteps = new Stack<ReferenceStep>();
			for (DocumentNodeMarker i = marker; i != ancestor; i = i.Parent)
			{
				ReferenceStep referenceStep = DocumentNodeMarkerUtilities.GetReferenceStep(i);
				if (referenceStep != null)
				{
					referenceSteps.Push(referenceStep);
				}
				else
				{
					referenceSteps.Clear();
				}
			}
			return referenceSteps;
		}
	}
}