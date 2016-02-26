using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignModel.Core
{
	public class ReferenceStepQuery
	{
		private ReferenceStepQuery.Step[] steps;

		public ReferenceStepQuery(params ReferenceStepQuery.Step[] steps)
		{
			this.steps = steps;
		}

		public ReferenceStepQuery.MatchType Test(PropertyReference propertyReference)
		{
			return this.Test(propertyReference, 0);
		}

		public ReferenceStepQuery.MatchType Test(PropertyReference propertyReference, int startIndex)
		{
			int i;
			ReferenceStepQuery.Step.MatchType matchType;
			int num = 0;
			for (i = startIndex; num < (int)this.steps.Length && i < propertyReference.Count; i++)
			{
				ReferenceStepQuery.Step step = this.steps[num];
				ReferenceStepQuery.Step.MatchType matchType1 = step.Matches(propertyReference[i]);
				if (matchType1 == ReferenceStepQuery.Step.MatchType.NoMatch)
				{
					return ReferenceStepQuery.MatchType.NoMatch;
				}
				if (matchType1 == ReferenceStepQuery.Step.MatchType.ContinueMatch)
				{
					if (num == (int)this.steps.Length - 1)
					{
						return ReferenceStepQuery.MatchType.Exact;
					}
					do
					{
						i++;
						if (i >= propertyReference.Count)
						{
							break;
						}
						matchType = this.steps[num + 1].Matches(propertyReference[i]);
						matchType1 = matchType;
					}
					while (matchType != ReferenceStepQuery.Step.MatchType.YesMatch);
				}
				num++;
			}
			if (num == (int)this.steps.Length && i == propertyReference.Count)
			{
				return ReferenceStepQuery.MatchType.Exact;
			}
			if (num == (int)this.steps.Length)
			{
				return ReferenceStepQuery.MatchType.WildCardContains;
			}
			return ReferenceStepQuery.MatchType.PropertyReferenceContains;
		}

		public ReferenceStepQuery.MatchType Test(DocumentNodeMarker marker, DocumentNodeMarker root)
		{
			ReferenceStepQuery.Step.MatchType matchType;
			int length = (int)this.steps.Length - 1;
			DocumentNodeMarker parent = marker;
			while (length >= 0 && parent != root)
			{
				ReferenceStepQuery.Step.MatchType matchType1 = this.steps[length].Matches(parent);
				if (matchType1 == ReferenceStepQuery.Step.MatchType.NoMatch)
				{
					return ReferenceStepQuery.MatchType.NoMatch;
				}
				if (matchType1 == ReferenceStepQuery.Step.MatchType.ContinueMatch)
				{
					if (length == 0)
					{
						return ReferenceStepQuery.MatchType.Exact;
					}
					do
					{
						parent = parent.Parent;
						if (parent == root)
						{
							break;
						}
						matchType = this.steps[length - 1].Matches(parent);
						matchType1 = matchType;
					}
					while (matchType != ReferenceStepQuery.Step.MatchType.YesMatch);
				}
				length--;
			}
			if (length == 0 && parent == root)
			{
				return ReferenceStepQuery.MatchType.Exact;
			}
			if (length < 0)
			{
				return ReferenceStepQuery.MatchType.NoMatch;
			}
			return ReferenceStepQuery.MatchType.NoMatch;
		}

		public enum MatchType
		{
			NoMatch,
			PropertyReferenceContains,
			WildCardContains,
			Exact
		}

		public struct Step
		{
			private ReferenceStepQuery.WildcardType wildcard;

			private ReferenceStep referenceStep;

			private string nameContains;

			public Step(ReferenceStepQuery.WildcardType wildcard)
			{
				this.wildcard = wildcard;
				this.referenceStep = null;
				this.nameContains = string.Empty;
			}

			public Step(ReferenceStep referenceStep)
			{
				this.wildcard = ReferenceStepQuery.WildcardType.None;
				this.referenceStep = referenceStep;
				this.nameContains = string.Empty;
			}

			public Step(string nameContains)
			{
				this.wildcard = ReferenceStepQuery.WildcardType.None;
				this.referenceStep = null;
				this.nameContains = nameContains;
			}

			public ReferenceStepQuery.Step.MatchType Matches(ReferenceStep otherStep)
			{
				if (this.wildcard != ReferenceStepQuery.WildcardType.None)
				{
					if (this.wildcard == ReferenceStepQuery.WildcardType.Descendant)
					{
						return ReferenceStepQuery.Step.MatchType.ContinueMatch;
					}
					if (this.wildcard != ReferenceStepQuery.WildcardType.Indexer)
					{
						return ReferenceStepQuery.Step.MatchType.NoMatch;
					}
					if (otherStep.GetType() != typeof(IndexedClrPropertyReferenceStep))
					{
						return ReferenceStepQuery.Step.MatchType.NoMatch;
					}
					return ReferenceStepQuery.Step.MatchType.YesMatch;
				}
				if (this.referenceStep == null)
				{
					if (!otherStep.Name.Contains(this.nameContains))
					{
						return ReferenceStepQuery.Step.MatchType.NoMatch;
					}
					return ReferenceStepQuery.Step.MatchType.YesMatch;
				}
				if (this.referenceStep.SortValue == otherStep.SortValue && this.referenceStep.GetType() == otherStep.GetType())
				{
					return ReferenceStepQuery.Step.MatchType.YesMatch;
				}
				return ReferenceStepQuery.Step.MatchType.NoMatch;
			}

			public ReferenceStepQuery.Step.MatchType Matches(DocumentNodeMarker marker)
			{
				if (this.wildcard != ReferenceStepQuery.WildcardType.None)
				{
					if (this.wildcard == ReferenceStepQuery.WildcardType.Descendant)
					{
						return ReferenceStepQuery.Step.MatchType.ContinueMatch;
					}
					if (this.wildcard != ReferenceStepQuery.WildcardType.Indexer)
					{
						return ReferenceStepQuery.Step.MatchType.NoMatch;
					}
					if (!marker.IsChild)
					{
						return ReferenceStepQuery.Step.MatchType.NoMatch;
					}
					return ReferenceStepQuery.Step.MatchType.YesMatch;
				}
				if (this.referenceStep != null)
				{
					if (this.referenceStep.Equals(marker.Property))
					{
						return ReferenceStepQuery.Step.MatchType.YesMatch;
					}
					return ReferenceStepQuery.Step.MatchType.NoMatch;
				}
				if (marker.IsChild)
				{
					return ReferenceStepQuery.Step.MatchType.NoMatch;
				}
				if (!marker.Property.Name.Contains(this.nameContains))
				{
					return ReferenceStepQuery.Step.MatchType.NoMatch;
				}
				return ReferenceStepQuery.Step.MatchType.YesMatch;
			}

			public enum MatchType
			{
				NoMatch,
				ContinueMatch,
				YesMatch
			}
		}

		public enum WildcardType
		{
			Indexer,
			Descendant,
			None
		}
	}
}