using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public class CommonProperties
	{
		private bool initialized;

		private PropertyReference renderTransform;

		private PropertyReference scaleX;

		private PropertyReference scaleY;

		private PropertyReference scaleCenterX;

		private PropertyReference scaleCenterY;

		private PropertyReference skewX;

		private PropertyReference skewY;

		private PropertyReference skewCenterX;

		private PropertyReference skewCenterY;

		private PropertyReference rotationAngle;

		private PropertyReference rotationCenterX;

		private PropertyReference rotationCenterY;

		private PropertyReference translationX;

		private PropertyReference translationY;

		private PropertyReference renderTransformScaleX;

		private PropertyReference renderTransformScaleY;

		private PropertyReference renderTransformSkewX;

		private PropertyReference renderTransformSkewY;

		private PropertyReference renderTransformRotationAngle;

		private PropertyReference renderTransformTranslationX;

		private PropertyReference renderTransformTranslationY;

		private PropertyReference rotateTransform3DReference;

		private PropertyReference scaleTransform3DReference;

		private PropertyReference translateTransform3DReference;

		private PropertyReference rotateTransform3DRotationReference;

		private PropertyReference brushRelativeTransformReference;

		private PropertyReference brushScaleXReference;

		private PropertyReference brushScaleYReference;

		private PropertyReference brushScaleCenterXReference;

		private PropertyReference brushScaleCenterYReference;

		private PropertyReference brushSkewXReference;

		private PropertyReference brushSkewYReference;

		private PropertyReference brushSkewCenterXReference;

		private PropertyReference brushSkewCenterYReference;

		private PropertyReference brushRotationCenterXReference;

		private PropertyReference brushRotationCenterYReference;

		private PropertyReference brushTranslationXReference;

		private PropertyReference brushTranslationYReference;

		private PropertyReference brushRotationAngleReference;

		public PropertyReference BrushRelativeTransformReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.brushRelativeTransformReference;
			}
		}

		public PropertyReference BrushRotationAngleReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.brushRotationAngleReference;
			}
		}

		public PropertyReference BrushRotationCenterXReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.brushRotationCenterXReference;
			}
		}

		public PropertyReference BrushRotationCenterYReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.brushRotationCenterYReference;
			}
		}

		public PropertyReference BrushScaleCenterXReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.brushScaleCenterXReference;
			}
		}

		public PropertyReference BrushScaleCenterYReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.brushScaleCenterYReference;
			}
		}

		public PropertyReference BrushScaleXReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.brushScaleXReference;
			}
		}

		public PropertyReference BrushScaleYReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.brushScaleYReference;
			}
		}

		public PropertyReference BrushSkewCenterXReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.brushSkewCenterXReference;
			}
		}

		public PropertyReference BrushSkewCenterYReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.brushSkewCenterYReference;
			}
		}

		public PropertyReference BrushSkewXReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.brushSkewXReference;
			}
		}

		public PropertyReference BrushSkewYReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.brushSkewYReference;
			}
		}

		public PropertyReference BrushTranslationXReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.brushTranslationXReference;
			}
		}

		public PropertyReference BrushTranslationYReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.brushTranslationYReference;
			}
		}

		public IPlatformTypes PlatformMetadata
		{
			get;
			private set;
		}

		public PropertyReference RenderTransform
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.renderTransform;
			}
		}

		public PropertyReference RenderTransformRotationAngle
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.renderTransformRotationAngle;
			}
		}

		public PropertyReference RenderTransformScaleX
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.renderTransformScaleX;
			}
		}

		public PropertyReference RenderTransformScaleY
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.renderTransformScaleY;
			}
		}

		public PropertyReference RenderTransformSkewX
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.renderTransformSkewX;
			}
		}

		public PropertyReference RenderTransformSkewY
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.renderTransformSkewY;
			}
		}

		public PropertyReference RenderTransformTranslationX
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.renderTransformTranslationX;
			}
		}

		public PropertyReference RenderTransformTranslationY
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.renderTransformTranslationY;
			}
		}

		public PropertyReference RotateTransform3DReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.rotateTransform3DReference;
			}
		}

		public PropertyReference RotateTransform3DRotationReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.rotateTransform3DRotationReference;
			}
		}

		public PropertyReference RotationAngle
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.rotationAngle;
			}
		}

		public PropertyReference RotationCenterX
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.rotationCenterX;
			}
		}

		public PropertyReference RotationCenterY
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.rotationCenterY;
			}
		}

		public PropertyReference ScaleCenterX
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.scaleCenterX;
			}
		}

		public PropertyReference ScaleCenterY
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.scaleCenterY;
			}
		}

		public PropertyReference ScaleTransform3DReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.scaleTransform3DReference;
			}
		}

		public PropertyReference ScaleX
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.scaleX;
			}
		}

		public PropertyReference ScaleY
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.scaleY;
			}
		}

		public PropertyReference SkewCenterX
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.skewCenterX;
			}
		}

		public PropertyReference SkewCenterY
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.skewCenterY;
			}
		}

		public PropertyReference SkewX
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.skewX;
			}
		}

		public PropertyReference SkewY
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.skewY;
			}
		}

		public PropertyReference TranslateTransform3DReference
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.translateTransform3DReference;
			}
		}

		public PropertyReference TranslationX
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.translationX;
			}
		}

		public PropertyReference TranslationY
		{
			get
			{
				this.EnsurePropertyReferences();
				return this.translationY;
			}
		}

		public CommonProperties(IPlatformTypes platformMetadata)
		{
			this.PlatformMetadata = platformMetadata;
		}

		private void EnsurePropertyReferences()
		{
			if (!this.initialized)
			{
				this.initialized = true;
				if (!this.PlatformMetadata.IsCapabilitySet(PlatformCapability.SupportsCompositeTransform))
				{
					PropertyReference propertyReference = new PropertyReference((ReferenceStep)this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.TransformGroup.GetMember(MemberType.Property, "Children", MemberAccessTypes.Public)));
					PropertyReference propertyReference1 = propertyReference.Append(IndexedClrPropertyReferenceStep.GetReferenceStep(this.PlatformMetadata, PlatformTypes.TransformCollection, CanonicalTransformOrder.ScaleIndex));
					PropertyReference propertyReference2 = propertyReference.Append(IndexedClrPropertyReferenceStep.GetReferenceStep(this.PlatformMetadata, PlatformTypes.TransformCollection, CanonicalTransformOrder.SkewIndex));
					PropertyReference propertyReference3 = propertyReference.Append(IndexedClrPropertyReferenceStep.GetReferenceStep(this.PlatformMetadata, PlatformTypes.TransformCollection, CanonicalTransformOrder.RotateIndex));
					PropertyReference propertyReference4 = propertyReference.Append(IndexedClrPropertyReferenceStep.GetReferenceStep(this.PlatformMetadata, PlatformTypes.TransformCollection, CanonicalTransformOrder.TranslateIndex));
					this.scaleX = propertyReference1.Append((ReferenceStep)this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.ScaleTransform.GetMember(MemberType.Property, "ScaleX", MemberAccessTypes.Public)));
					this.scaleY = propertyReference1.Append((ReferenceStep)this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.ScaleTransform.GetMember(MemberType.Property, "ScaleY", MemberAccessTypes.Public)));
					this.scaleCenterX = propertyReference1.Append((ReferenceStep)this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.ScaleTransform.GetMember(MemberType.Property, "CenterX", MemberAccessTypes.Public)));
					this.scaleCenterY = propertyReference1.Append((ReferenceStep)this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.ScaleTransform.GetMember(MemberType.Property, "CenterY", MemberAccessTypes.Public)));
					this.skewX = propertyReference2.Append((ReferenceStep)this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.SkewTransform.GetMember(MemberType.Property, "AngleX", MemberAccessTypes.Public)));
					this.skewY = propertyReference2.Append((ReferenceStep)this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.SkewTransform.GetMember(MemberType.Property, "AngleY", MemberAccessTypes.Public)));
					this.skewCenterX = propertyReference2.Append((ReferenceStep)this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.SkewTransform.GetMember(MemberType.Property, "CenterX", MemberAccessTypes.Public)));
					this.skewCenterY = propertyReference2.Append((ReferenceStep)this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.SkewTransform.GetMember(MemberType.Property, "CenterY", MemberAccessTypes.Public)));
					this.rotationAngle = propertyReference3.Append((ReferenceStep)this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.RotateTransform.GetMember(MemberType.Property, "Angle", MemberAccessTypes.Public)));
					this.rotationCenterX = propertyReference3.Append((ReferenceStep)this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.RotateTransform.GetMember(MemberType.Property, "CenterX", MemberAccessTypes.Public)));
					this.rotationCenterY = propertyReference3.Append((ReferenceStep)this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.RotateTransform.GetMember(MemberType.Property, "CenterY", MemberAccessTypes.Public)));
					this.translationX = propertyReference4.Append((ReferenceStep)this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.TranslateTransform.GetMember(MemberType.Property, "X", MemberAccessTypes.Public)));
					this.translationY = propertyReference4.Append((ReferenceStep)this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.TranslateTransform.GetMember(MemberType.Property, "Y", MemberAccessTypes.Public)));
				}
				else
				{
					PropertyReference propertyReference5 = new PropertyReference((ReferenceStep)this.PlatformMetadata.ResolveProperty(KnownProperties.CenterXProperty));
					PropertyReference propertyReference6 = new PropertyReference((ReferenceStep)this.PlatformMetadata.ResolveProperty(KnownProperties.CenterYProperty));
					this.scaleX = new PropertyReference((ReferenceStep)this.PlatformMetadata.ResolveProperty(KnownProperties.ScaleXProperty));
					this.scaleY = new PropertyReference((ReferenceStep)this.PlatformMetadata.ResolveProperty(KnownProperties.ScaleYProperty));
					this.scaleCenterX = propertyReference5;
					this.scaleCenterY = propertyReference6;
					this.skewX = new PropertyReference((ReferenceStep)this.PlatformMetadata.ResolveProperty(KnownProperties.SkewXProperty));
					this.skewY = new PropertyReference((ReferenceStep)this.PlatformMetadata.ResolveProperty(KnownProperties.SkewYProperty));
					this.skewCenterX = propertyReference5;
					this.skewCenterY = propertyReference6;
					this.rotationAngle = new PropertyReference((ReferenceStep)this.PlatformMetadata.ResolveProperty(KnownProperties.RotationProperty));
					this.rotationCenterX = propertyReference5;
					this.rotationCenterY = propertyReference6;
					this.translationX = new PropertyReference((ReferenceStep)this.PlatformMetadata.ResolveProperty(KnownProperties.TranslateXProperty));
					this.translationY = new PropertyReference((ReferenceStep)this.PlatformMetadata.ResolveProperty(KnownProperties.TranslateYProperty));
				}
				this.renderTransform = new PropertyReference(this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.FrameworkElement.GetMember(MemberType.Property, "RenderTransform", MemberAccessTypes.Public)) as ReferenceStep);
				this.renderTransformScaleX = this.RenderTransform.Append(this.ScaleX);
				this.renderTransformScaleY = this.RenderTransform.Append(this.ScaleY);
				this.renderTransformSkewX = this.RenderTransform.Append(this.SkewX);
				this.renderTransformSkewY = this.RenderTransform.Append(this.SkewY);
				this.renderTransformRotationAngle = this.RenderTransform.Append(this.rotationAngle);
				this.renderTransformTranslationX = this.RenderTransform.Append(this.translationX);
				this.renderTransformTranslationY = this.RenderTransform.Append(this.translationY);
				if (!this.PlatformMetadata.IsNullType(this.PlatformMetadata.ResolveType(PlatformTypes.Transform3DGroup)))
				{
					PropertyReference propertyReference7 = new PropertyReference((ReferenceStep)this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.Transform3DGroup.GetMember(MemberType.Property, "Children", MemberAccessTypes.Public)));
					this.scaleTransform3DReference = propertyReference7.Append(IndexedClrPropertyReferenceStep.GetReferenceStep(this.PlatformMetadata, PlatformTypes.Transform3DCollection, 1));
					this.rotateTransform3DReference = propertyReference7.Append(IndexedClrPropertyReferenceStep.GetReferenceStep(this.PlatformMetadata, PlatformTypes.Transform3DCollection, 2));
					this.translateTransform3DReference = propertyReference7.Append(IndexedClrPropertyReferenceStep.GetReferenceStep(this.PlatformMetadata, PlatformTypes.Transform3DCollection, 4));
					this.rotateTransform3DRotationReference = this.rotateTransform3DReference.Append((ReferenceStep)this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.RotateTransform3D.GetMember(MemberType.Property, "Rotation", MemberAccessTypes.Public)));
				}
				this.brushRelativeTransformReference = new PropertyReference((ReferenceStep)this.PlatformMetadata.ResolveProperty((IPropertyId)PlatformTypes.Brush.GetMember(MemberType.Property, "RelativeTransform", MemberAccessTypes.Public)));
				this.brushScaleXReference = this.BrushRelativeTransformReference.Append(this.ScaleX);
				this.brushScaleYReference = this.BrushRelativeTransformReference.Append(this.ScaleY);
				this.brushScaleCenterXReference = this.BrushRelativeTransformReference.Append(this.ScaleCenterX);
				this.brushScaleCenterYReference = this.BrushRelativeTransformReference.Append(this.ScaleCenterY);
				this.brushSkewXReference = this.BrushRelativeTransformReference.Append(this.SkewX);
				this.brushSkewYReference = this.BrushRelativeTransformReference.Append(this.SkewY);
				this.brushSkewCenterXReference = this.BrushRelativeTransformReference.Append(this.SkewCenterX);
				this.brushSkewCenterYReference = this.BrushRelativeTransformReference.Append(this.SkewCenterY);
				this.brushRotationAngleReference = this.BrushRelativeTransformReference.Append(this.RotationAngle);
				this.brushRotationCenterXReference = this.BrushRelativeTransformReference.Append(this.RotationCenterX);
				this.brushRotationCenterYReference = this.BrushRelativeTransformReference.Append(this.RotationCenterY);
				this.brushTranslationXReference = this.BrushRelativeTransformReference.Append(this.TranslationX);
				this.brushTranslationYReference = this.BrushRelativeTransformReference.Append(this.TranslationY);
			}
		}

		public static string GetName(object target, IPlatformMetadata platformMetadata)
		{
			DependencyPropertyReferenceStep dependencyPropertyReferenceStep = DesignTimeProperties.ResolveDesignTimeReferenceStep(DesignTimeProperties.XNameProperty, platformMetadata);
			return (string)dependencyPropertyReferenceStep.GetValue(target);
		}

		public static void SetName(object target, string name, IPlatformMetadata platformMetadata)
		{
			DependencyPropertyReferenceStep dependencyPropertyReferenceStep = DesignTimeProperties.ResolveDesignTimeReferenceStep(DesignTimeProperties.XNameProperty, platformMetadata);
			dependencyPropertyReferenceStep.SetValue(target, name);
		}
	}
}