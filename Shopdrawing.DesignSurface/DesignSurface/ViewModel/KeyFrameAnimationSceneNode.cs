// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.KeyFrameAnimationSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class KeyFrameAnimationSceneNode : AnimationSceneNode
  {
    private static ITypeId[] animatableTypesUsingDiscreteObjectAnimations = new ITypeId[10]
    {
      PlatformTypes.Enum,
      PlatformTypes.FontFamily,
      PlatformTypes.FontStretch,
      PlatformTypes.FontStyle,
      PlatformTypes.FontWeight,
      PlatformTypes.Thickness,
      PlatformTypes.Int32,
      PlatformTypes.CornerRadius,
      PlatformTypes.Cursor,
      PlatformTypes.String
    };
    public static readonly KeyFrameAnimationSceneNode.ConcreteKeyFrameAnimationSceneNodeFactory Factory = new KeyFrameAnimationSceneNode.ConcreteKeyFrameAnimationSceneNodeFactory();
    private static Dictionary<ITypeId, ITypeId> animatedTypes = new Dictionary<ITypeId, ITypeId>()
    {
      {
        PlatformTypes.BooleanAnimationUsingKeyFrames,
        PlatformTypes.Boolean
      },
      {
        PlatformTypes.ByteAnimationUsingKeyFrames,
        PlatformTypes.Byte
      },
      {
        PlatformTypes.CharAnimationUsingKeyFrames,
        PlatformTypes.Char
      },
      {
        PlatformTypes.ColorAnimationUsingKeyFrames,
        PlatformTypes.Color
      },
      {
        PlatformTypes.DecimalAnimationUsingKeyFrames,
        PlatformTypes.Decimal
      },
      {
        PlatformTypes.DoubleAnimationUsingKeyFrames,
        PlatformTypes.Double
      },
      {
        PlatformTypes.Int16AnimationUsingKeyFrames,
        PlatformTypes.Int16
      },
      {
        PlatformTypes.Int32AnimationUsingKeyFrames,
        PlatformTypes.Int32
      },
      {
        PlatformTypes.Int64AnimationUsingKeyFrames,
        PlatformTypes.Int64
      },
      {
        PlatformTypes.MatrixAnimationUsingKeyFrames,
        PlatformTypes.Matrix
      },
      {
        PlatformTypes.ObjectAnimationUsingKeyFrames,
        PlatformTypes.Object
      },
      {
        PlatformTypes.Point3DAnimationUsingKeyFrames,
        PlatformTypes.Point3D
      },
      {
        PlatformTypes.PointAnimationUsingKeyFrames,
        PlatformTypes.Point
      },
      {
        PlatformTypes.QuaternionAnimationUsingKeyFrames,
        PlatformTypes.Quaternion
      },
      {
        PlatformTypes.Rotation3DAnimationUsingKeyFrames,
        PlatformTypes.Rotation3D
      },
      {
        PlatformTypes.RectAnimationUsingKeyFrames,
        PlatformTypes.Rect
      },
      {
        PlatformTypes.SingleAnimationUsingKeyFrames,
        PlatformTypes.Single
      },
      {
        PlatformTypes.SizeAnimationUsingKeyFrames,
        PlatformTypes.Size
      },
      {
        PlatformTypes.StringAnimationUsingKeyFrames,
        PlatformTypes.String
      },
      {
        PlatformTypes.ThicknessAnimationUsingKeyFrames,
        PlatformTypes.Thickness
      },
      {
        PlatformTypes.Vector3DAnimationUsingKeyFrames,
        PlatformTypes.Vector3D
      },
      {
        PlatformTypes.VectorAnimationUsingKeyFrames,
        PlatformTypes.Vector
      }
    };
    private static Dictionary<ITypeId, ITypeId> animationsForTypes = new Dictionary<ITypeId, ITypeId>();
    private static Dictionary<ITypeId, ITypeId> discreteKeyFrameTypes;
    private static Dictionary<ITypeId, ITypeId> splineKeyFrameTypes;
    private static Dictionary<ITypeId, ITypeId> easingKeyFrameTypes;

    public IEnumerable<KeyFrameSceneNode> KeyFrames
    {
      get
      {
        return (IEnumerable<KeyFrameSceneNode>) this.KeyFrameCollection;
      }
    }

    public int KeyFrameCount
    {
      get
      {
        return this.KeyFrameCollection.Count;
      }
    }

    public override double NaturalDuration
    {
      get
      {
        double val1 = 0.0;
        if (this.KeyFrameCount > 0)
          val1 = 0.001;
        foreach (KeyFrameSceneNode keyFrameSceneNode in this.KeyFrames)
          val1 = Math.Max(val1, keyFrameSceneNode.Time);
        return val1;
      }
    }

    public override bool IsDiscreteOnly
    {
      get
      {
        foreach (object obj in KeyFrameAnimationSceneNode.splineKeyFrameTypes.Keys)
        {
          if (obj.Equals((object) this.AnimatedType))
            return false;
        }
        foreach (object obj in KeyFrameAnimationSceneNode.easingKeyFrameTypes.Keys)
        {
          if (obj.Equals((object) this.AnimatedType))
            return false;
        }
        return true;
      }
    }

    public bool NoAutoTransitionsProvided
    {
      get
      {
        return this.IsDiscreteOnly;
      }
    }

    public override ITypeId AnimatedType
    {
      get
      {
        ITypeId typeId = (ITypeId) null;
        if (!AnimationSceneNode.PlatformNeutralTypeDictionaryTryGetValue(KeyFrameAnimationSceneNode.animatedTypes, (ITypeId) this.Type, out typeId))
          typeId = PlatformTypes.Object;
        return typeId;
      }
    }

    public bool IsAnimationProxy
    {
      get
      {
        return (bool) this.GetLocalOrDefaultValue(DesignTimeProperties.IsAnimationProxyProperty);
      }
      set
      {
        if (value)
          this.SetLocalValue(DesignTimeProperties.IsAnimationProxyProperty, (object) true);
        else
          this.ClearLocalValue(DesignTimeProperties.IsAnimationProxyProperty);
      }
    }

    public IPropertyId KeyFramesProperty
    {
      get
      {
        return (IPropertyId) this.Type.GetMember(MemberType.LocalProperty, "KeyFrames", MemberAccessTypes.Public);
      }
    }

    private IList<KeyFrameSceneNode> KeyFrameCollection
    {
      get
      {
        return (IList<KeyFrameSceneNode>) new SceneNode.SceneNodeCollection<KeyFrameSceneNode>((SceneNode) this, this.KeyFramesProperty);
      }
    }

    static KeyFrameAnimationSceneNode()
    {
      foreach (ITypeId index in KeyFrameAnimationSceneNode.animatedTypes.Keys)
        KeyFrameAnimationSceneNode.animationsForTypes[KeyFrameAnimationSceneNode.animatedTypes[index]] = index;
      KeyFrameAnimationSceneNode.discreteKeyFrameTypes = new Dictionary<ITypeId, ITypeId>()
      {
        {
          PlatformTypes.Boolean,
          PlatformTypes.DiscreteBooleanKeyFrame
        },
        {
          PlatformTypes.Byte,
          PlatformTypes.DiscreteByteKeyFrame
        },
        {
          PlatformTypes.Char,
          PlatformTypes.DiscreteCharKeyFrame
        },
        {
          PlatformTypes.Color,
          PlatformTypes.DiscreteColorKeyFrame
        },
        {
          PlatformTypes.Decimal,
          PlatformTypes.DiscreteDecimalKeyFrame
        },
        {
          PlatformTypes.Double,
          PlatformTypes.DiscreteDoubleKeyFrame
        },
        {
          PlatformTypes.Int16,
          PlatformTypes.DiscreteInt16KeyFrame
        },
        {
          PlatformTypes.Int32,
          PlatformTypes.DiscreteInt32KeyFrame
        },
        {
          PlatformTypes.Int64,
          PlatformTypes.DiscreteInt64KeyFrame
        },
        {
          PlatformTypes.Matrix,
          PlatformTypes.DiscreteMatrixKeyFrame
        },
        {
          PlatformTypes.Object,
          PlatformTypes.DiscreteObjectKeyFrame
        },
        {
          PlatformTypes.Point3D,
          PlatformTypes.DiscretePoint3DKeyFrame
        },
        {
          PlatformTypes.Point,
          PlatformTypes.DiscretePointKeyFrame
        },
        {
          PlatformTypes.Quaternion,
          PlatformTypes.DiscreteQuaternionKeyFrame
        },
        {
          PlatformTypes.Rotation3D,
          PlatformTypes.DiscreteRotation3DKeyFrame
        },
        {
          PlatformTypes.Rect,
          PlatformTypes.DiscreteRectKeyFrame
        },
        {
          PlatformTypes.Single,
          PlatformTypes.DiscreteSingleKeyFrame
        },
        {
          PlatformTypes.Size,
          PlatformTypes.DiscreteSizeKeyFrame
        },
        {
          PlatformTypes.String,
          PlatformTypes.DiscreteStringKeyFrame
        },
        {
          PlatformTypes.Thickness,
          PlatformTypes.DiscreteThicknessKeyFrame
        },
        {
          PlatformTypes.Vector3D,
          PlatformTypes.DiscreteVector3DKeyFrame
        },
        {
          PlatformTypes.Vector,
          PlatformTypes.DiscreteVectorKeyFrame
        }
      };
      KeyFrameAnimationSceneNode.splineKeyFrameTypes = new Dictionary<ITypeId, ITypeId>()
      {
        {
          PlatformTypes.Byte,
          PlatformTypes.SplineByteKeyFrame
        },
        {
          PlatformTypes.Color,
          PlatformTypes.SplineColorKeyFrame
        },
        {
          PlatformTypes.Decimal,
          PlatformTypes.SplineDecimalKeyFrame
        },
        {
          PlatformTypes.Double,
          PlatformTypes.SplineDoubleKeyFrame
        },
        {
          PlatformTypes.Int16,
          PlatformTypes.SplineInt16KeyFrame
        },
        {
          PlatformTypes.Int32,
          PlatformTypes.SplineInt32KeyFrame
        },
        {
          PlatformTypes.Int64,
          PlatformTypes.SplineInt64KeyFrame
        },
        {
          PlatformTypes.Point3D,
          PlatformTypes.SplinePoint3DKeyFrame
        },
        {
          PlatformTypes.Point,
          PlatformTypes.SplinePointKeyFrame
        },
        {
          PlatformTypes.Quaternion,
          PlatformTypes.SplineQuaternionKeyFrame
        },
        {
          PlatformTypes.Rotation3D,
          PlatformTypes.SplineRotation3DKeyFrame
        },
        {
          PlatformTypes.Rect,
          PlatformTypes.SplineRectKeyFrame
        },
        {
          PlatformTypes.Single,
          PlatformTypes.SplineSingleKeyFrame
        },
        {
          PlatformTypes.Size,
          PlatformTypes.SplineSizeKeyFrame
        },
        {
          PlatformTypes.Thickness,
          PlatformTypes.SplineThicknessKeyFrame
        },
        {
          PlatformTypes.Vector3D,
          PlatformTypes.SplineVector3DKeyFrame
        },
        {
          PlatformTypes.Vector,
          PlatformTypes.SplineVectorKeyFrame
        }
      };
      KeyFrameAnimationSceneNode.easingKeyFrameTypes = new Dictionary<ITypeId, ITypeId>()
      {
        {
          PlatformTypes.Byte,
          PlatformTypes.EasingByteKeyFrame
        },
        {
          PlatformTypes.Color,
          PlatformTypes.EasingColorKeyFrame
        },
        {
          PlatformTypes.Decimal,
          PlatformTypes.EasingDecimalKeyFrame
        },
        {
          PlatformTypes.Double,
          PlatformTypes.EasingDoubleKeyFrame
        },
        {
          PlatformTypes.Int16,
          PlatformTypes.EasingInt16KeyFrame
        },
        {
          PlatformTypes.Int32,
          PlatformTypes.EasingInt32KeyFrame
        },
        {
          PlatformTypes.Int64,
          PlatformTypes.EasingInt64KeyFrame
        },
        {
          PlatformTypes.Point3D,
          PlatformTypes.EasingPoint3DKeyFrame
        },
        {
          PlatformTypes.Point,
          PlatformTypes.EasingPointKeyFrame
        },
        {
          PlatformTypes.Quaternion,
          PlatformTypes.EasingQuaternionKeyFrame
        },
        {
          PlatformTypes.Rotation3D,
          PlatformTypes.EasingRotation3DKeyFrame
        },
        {
          PlatformTypes.Rect,
          PlatformTypes.EasingRectKeyFrame
        },
        {
          PlatformTypes.Single,
          PlatformTypes.EasingSingleKeyFrame
        },
        {
          PlatformTypes.Size,
          PlatformTypes.EasingSizeKeyFrame
        },
        {
          PlatformTypes.Thickness,
          PlatformTypes.EasingThicknessKeyFrame
        },
        {
          PlatformTypes.Vector3D,
          PlatformTypes.EasingVector3DKeyFrame
        },
        {
          PlatformTypes.Vector,
          PlatformTypes.EasingVectorKeyFrame
        }
      };
    }

    protected override void SetTimeRegionCore(TimeRegionChangeDetails details, double scaleFactor, double originalRegionBegin, double originalRegionEnd, double finalRegionBegin, double finalRegionEnd)
    {
      if ((details & TimeRegionChangeDetails.ToZero) != TimeRegionChangeDetails.None)
      {
        IList<KeyFrameSceneNode> keyFrameCollection = this.KeyFrameCollection;
        if (keyFrameCollection.Count > 0)
        {
          for (int index = keyFrameCollection.Count - 1; index > 0; --index)
            this.RemoveKeyFrame(keyFrameCollection[index].Time);
          keyFrameCollection[0].Time = finalRegionBegin;
        }
      }
      if ((details & TimeRegionChangeDetails.Scale) != TimeRegionChangeDetails.None && (details & TimeRegionChangeDetails.FromZero) == TimeRegionChangeDetails.None)
      {
        IList<KeyFrameSceneNode> keyFrameCollection = this.KeyFrameCollection;
        for (int index = 0; index < keyFrameCollection.Count; ++index)
        {
          KeyFrameSceneNode keyFrameSceneNode = keyFrameCollection[index];
          keyFrameSceneNode.Time = (keyFrameSceneNode.Time - originalRegionBegin) * scaleFactor + finalRegionBegin;
        }
      }
      else
      {
        if ((details & TimeRegionChangeDetails.Translate) == TimeRegionChangeDetails.None)
          return;
        double num = finalRegionBegin - originalRegionBegin;
        IList<KeyFrameSceneNode> keyFrameCollection = this.KeyFrameCollection;
        for (int index = 0; index < keyFrameCollection.Count; ++index)
          keyFrameCollection[index].Time += num;
      }
    }

    public bool HasKeyFrameAtTime(double time)
    {
      return this.GetKeyFrameAtTime(time) != null;
    }

    public static ITypeId GetKeyFrameAnimationForType(ITypeId type, IProjectContext projectContext)
    {
      ITypeId type1 = (ITypeId) null;
      bool flag = AnimationSceneNode.PlatformNeutralTypeDictionaryTryGetValue(KeyFrameAnimationSceneNode.animationsForTypes, type, out type1);
      if (flag)
        flag = projectContext.Platform.Metadata.IsSupported((ITypeResolver) projectContext, type1);
      if (!flag)
        type1 = PlatformTypes.ObjectAnimationUsingKeyFrames;
      return type1;
    }

    public void AddKeyFrame(double time, object propertyValue)
    {
      KeyFrameSceneNode keyFrameSceneNode;
      if (!this.IsDiscreteOnly)
      {
        KeyFrameInterpolationType keyFrameInterpolationType = !this.ViewModel.ProjectContext.Platform.Metadata.IsSupported((ITypeResolver) this.ViewModel.ProjectContext, PlatformTypes.IEasingFunction) ? KeyFrameInterpolationType.Spline : KeyFrameInterpolationType.Easing;
        keyFrameSceneNode = (KeyFrameSceneNode) KeyFrameSceneNode.Factory.Instantiate(this.ViewModel, this.GetKeyFrameType(keyFrameInterpolationType));
      }
      else
        keyFrameSceneNode = (KeyFrameSceneNode) KeyFrameSceneNode.Factory.Instantiate(this.ViewModel, this.GetKeyFrameType(KeyFrameInterpolationType.Discrete));
      keyFrameSceneNode.Value = propertyValue;
      keyFrameSceneNode.Time = time;
      this.AddKeyFrame(keyFrameSceneNode);
    }

    public void RemoveKeyFrame(double time)
    {
      int index = this.IndexOfKeyFrameAtTime(time);
      if (index == -1)
        return;
      bool flag = this.ControllingState != null && this.ChangeAtIndexCanAffectTransition(index);
      IList<KeyFrameSceneNode> keyFrameCollection = this.KeyFrameCollection;
      KeyFrameSceneNode keyFrameSceneNode = keyFrameCollection[index];
      Point? nullable = new Point?();
      this.ConvertKeySplineResourcesToLocalValues();
      if (index != 0)
        nullable = new Point?(keyFrameCollection[index - 1].EaseOutControlPoint);
      else if (keyFrameSceneNode.InterpolationType != KeyFrameInterpolationType.Discrete && keyFrameSceneNode.InterpolationType != KeyFrameInterpolationType.Easing)
        nullable = new Point?(this.GetHandoffKeyFrameEaseOutPoint());
      keyFrameCollection.Remove(keyFrameSceneNode);
      if (nullable.HasValue)
      {
        if (index != 0)
          KeyFrameSceneNode.SetEaseOutControlPoint(keyFrameCollection[index - 1], nullable.Value);
        else if (keyFrameCollection.Count > 0)
          this.SetHandoffKeyFrameEaseOutPoint(nullable.Value);
      }
      if (this.NaturalDuration == 0.001)
        ;
      if (!flag)
        return;
      object newValue = (object) null;
      if (keyFrameCollection.Count > 0)
      {
        if (index < keyFrameCollection.Count)
          newValue = keyFrameCollection[index].Value;
        else if (index == keyFrameCollection.Count)
          newValue = keyFrameCollection[index - 1].Value;
      }
      this.ControllingState.UpdateTransitionsForStateValueChange(this.TargetElementAndProperty, newValue);
    }

    public bool SetHandoffKeyFrameEaseOutPoint(Point easeOutPoint)
    {
      IList<KeyFrameSceneNode> keyFrameCollection = this.KeyFrameCollection;
      if (keyFrameCollection == null || keyFrameCollection.Count == 0)
        return false;
      KeyFrameSceneNode keyFrame = keyFrameCollection[0];
      if (keyFrame.InterpolationType != KeyFrameInterpolationType.Spline)
        return this.ReplaceKeyFrame(keyFrame, KeyFrameInterpolationType.Spline, new KeySpline(easeOutPoint, new Point(1.0, 1.0))) != null;
      if (keyFrame.KeySpline == null)
      {
        keyFrame.KeySpline = new KeySpline(easeOutPoint, new Point(1.0, 1.0));
      }
      else
      {
        KeySpline keySpline = new KeySpline(easeOutPoint, keyFrame.KeySpline.ControlPoint2);
        keyFrame.KeySpline = keySpline;
      }
      return true;
    }

    public Point GetHandoffKeyFrameEaseOutPoint()
    {
      IList<KeyFrameSceneNode> keyFrameCollection = this.KeyFrameCollection;
      if (keyFrameCollection == null || keyFrameCollection.Count == 0)
        return new Point();
      if (keyFrameCollection[0].InterpolationType == KeyFrameInterpolationType.Spline && keyFrameCollection[0].KeySpline != null)
        return keyFrameCollection[0].KeySpline.ControlPoint1;
      return new Point();
    }

    public void ClearKeyFrames()
    {
      this.KeyFrameCollection.Clear();
    }

    private bool ConvertKeyFrameKeysplineToLocalValue(KeyFrameSceneNode keyFrame)
    {
      IPropertyId keySplineProperty = keyFrame.KeySplineProperty;
      if (keySplineProperty != null)
      {
        DocumentCompositeNode documentCompositeNode = keyFrame.DocumentNode as DocumentCompositeNode;
        if (documentCompositeNode != null)
        {
          DocumentNode expression = documentCompositeNode.Properties[keySplineProperty];
          if (expression != null && expression.Type.IsResource)
          {
            DocumentNode documentNode = new ExpressionEvaluator(this.ViewModel.DocumentRootResolver).EvaluateExpression(this.ViewModel.ActiveEditingContainerPath, expression);
            if (documentNode != null)
            {
              KeySpline keySpline = (KeySpline) this.ViewModel.CreateInstance(new DocumentNodePath(documentNode, documentNode));
              keyFrame.KeySpline = keySpline;
              return true;
            }
          }
        }
      }
      return false;
    }

    private void ConvertKeySplineResourcesToLocalValues()
    {
      this.ConvertKeySplineResourcesToLocalValues((KeyFrameSceneNode) null);
    }

    private void ConvertKeySplineResourcesToLocalValues(KeyFrameSceneNode unattachedKeyFrame)
    {
      bool flag = false;
      if (unattachedKeyFrame != null)
        flag = this.ConvertKeyFrameKeysplineToLocalValue(unattachedKeyFrame);
      foreach (KeyFrameSceneNode keyFrame in (IEnumerable<KeyFrameSceneNode>) this.KeyFrameCollection)
        flag = flag || this.ConvertKeyFrameKeysplineToLocalValue(keyFrame);
      if (!flag || !this.IsInDocument)
        return;
      this.ViewModel.DefaultView.ShowBubble(StringTable.KeySplineResourceResetWarningMessage, MessageBubbleType.Warning);
    }

    public void MoveKeyFrame(double fromTime, double toTime)
    {
      int index = this.IndexOfKeyFrameAtTime(fromTime);
      if (index == -1)
        return;
      IList<KeyFrameSceneNode> keyFrameCollection = this.KeyFrameCollection;
      KeyFrameSceneNode unattachedKeyFrame = keyFrameCollection[index];
      this.ConvertKeySplineResourcesToLocalValues(unattachedKeyFrame);
      Point? nullable1 = new Point?();
      if (unattachedKeyFrame.CanSetEaseOutControlPoint)
        nullable1 = new Point?(unattachedKeyFrame.EaseOutControlPoint);
      Point? nullable2 = new Point?();
      if (index == keyFrameCollection.Count - 1)
      {
        if (keyFrameCollection.Count > 1)
        {
          KeyFrameSceneNode keyFrameSceneNode = keyFrameCollection[index - 1];
          if (keyFrameSceneNode.Time < toTime && keyFrameSceneNode.CanSetEaseOutControlPoint)
            nullable2 = new Point?(keyFrameSceneNode.EaseOutControlPoint);
        }
        else if (unattachedKeyFrame.InterpolationType != KeyFrameInterpolationType.Discrete && unattachedKeyFrame.InterpolationType != KeyFrameInterpolationType.Easing)
          nullable2 = new Point?(this.GetHandoffKeyFrameEaseOutPoint());
      }
      this.RemoveKeyFrame(fromTime);
      this.RemoveKeyFrame(toTime);
      KeyFrameSceneNode keyFrameSceneNode1 = (KeyFrameSceneNode) unattachedKeyFrame.ViewModel.GetSceneNode(unattachedKeyFrame.DocumentNode.Clone(unattachedKeyFrame.DocumentContext));
      keyFrameSceneNode1.Time = toTime;
      this.AddKeyFrame(keyFrameSceneNode1);
      if (nullable1.HasValue && keyFrameSceneNode1.CanSetEaseOutControlPoint)
        keyFrameSceneNode1.EaseOutControlPoint = nullable1.Value;
      if (!nullable2.HasValue)
        return;
      if (keyFrameCollection.Count > 1)
        keyFrameCollection[index - 1].EaseOutControlPoint = nullable2.Value;
      else
        this.SetHandoffKeyFrameEaseOutPoint(nullable2.Value);
    }

    protected int IndexOfKeyFrameAtTime(double time)
    {
      time = Math.Min(TimelineSceneNode.MaxSeconds, time);
      IList<KeyFrameSceneNode> keyFrameCollection = this.KeyFrameCollection;
      for (int index = 0; index < keyFrameCollection.Count; ++index)
      {
        if (keyFrameCollection[index] != null && keyFrameCollection[index].Time == time)
          return index;
      }
      return -1;
    }

    public KeyFrameSceneNode GetKeyFrameAtTime(double time)
    {
      time = Math.Min(TimelineSceneNode.MaxSeconds, time);
      foreach (KeyFrameSceneNode keyFrameSceneNode in this.KeyFrames)
      {
        if (keyFrameSceneNode.Time == time)
          return keyFrameSceneNode;
      }
      return (KeyFrameSceneNode) null;
    }

    public KeyFrameSceneNode GetKeyFrameAtIndex(int index)
    {
      return this.KeyFrameCollection[index];
    }

    public KeyFrameSceneNode GetNextKeyFrame(KeyFrameSceneNode keyFrame)
    {
      IList<KeyFrameSceneNode> keyFrameCollection = this.KeyFrameCollection;
      int num = keyFrameCollection.IndexOf(keyFrame);
      if (num < keyFrameCollection.Count - 1)
        return keyFrameCollection[num + 1];
      return (KeyFrameSceneNode) null;
    }

    public static bool CanAnimateType(IType type, IProjectContext projectContext)
    {
      bool flag = false;
      foreach (ITypeId index in KeyFrameAnimationSceneNode.animationsForTypes.Keys)
      {
        if (index.Equals((object) type))
        {
          if (projectContext.Platform.Metadata.IsSupported((ITypeResolver) projectContext, KeyFrameAnimationSceneNode.animationsForTypes[index]))
          {
            flag = true;
            break;
          }
          break;
        }
      }
      if (!flag)
        flag = KeyFrameAnimationSceneNode.CanAnimateUsingObjectKeyframes(type);
      return flag;
    }

    private static bool CanAnimateUsingObjectKeyframes(IType type)
    {
      bool flag = false;
      IType nullableType = type.NullableType;
      if (nullableType != null)
        type = nullableType;
      foreach (ITypeId typeId in KeyFrameAnimationSceneNode.animatableTypesUsingDiscreteObjectAnimations)
      {
        if (typeId.IsAssignableFrom((ITypeId) type))
          flag = true;
      }
      if (PlatformTypes.Boolean.Equals((object) type))
        flag = true;
      return flag;
    }

    public void ReverseAllKeyFrames(double? totalDurationOverride)
    {
      if (this.KeyFrameCount <= 0)
        return;
      this.ReverseKeyFrames(0.0, !totalDurationOverride.HasValue ? this.KeyFrameCollection[this.KeyFrameCount - 1].Time : totalDurationOverride.Value);
    }

    public void ReverseKeyFrames(double startTime, double endTime)
    {
      if (this.KeyFrameCount == 0)
        return;
      this.ConvertKeySplineResourcesToLocalValues();
      int capacity = 0;
      IList<KeyFrameSceneNode> keyFrameCollection = this.KeyFrameCollection;
      int index1 = 0;
      while (index1 < keyFrameCollection.Count && keyFrameCollection[index1].Time < startTime)
        ++index1;
      int index2 = index1;
      for (; index1 < keyFrameCollection.Count && keyFrameCollection[index1].Time <= endTime; ++index1)
        ++capacity;
      if (capacity == 0)
        return;
      bool flag = startTime == 0.0 && keyFrameCollection[0].Time != 0.0;
      if (flag)
        ++capacity;
      List<KeyFrameSceneNode> list1 = new List<KeyFrameSceneNode>(capacity);
      List<Point?> list2 = new List<Point?>(capacity);
      if (flag)
      {
        KeyFrameSceneNode keyFrameSceneNode = this.IsDiscreteOnly ? (KeyFrameSceneNode) KeyFrameSceneNode.Factory.Instantiate(this.ViewModel, this.GetKeyFrameType(KeyFrameInterpolationType.Discrete)) : (KeyFrameSceneNode) KeyFrameSceneNode.Factory.Instantiate(this.ViewModel, this.GetKeyFrameType(KeyFrameInterpolationType.Spline));
        keyFrameSceneNode.Time = 0.0;
        SceneElement sceneElement = this.TargetElement as SceneElement;
        IViewObject viewObject = sceneElement.ViewTargetElement;
        if (PlatformTypes.Viewport3D.Equals((object) viewObject.GetIType((ITypeResolver) this.ProjectContext)))
          viewObject = sceneElement.ViewObject;
        PropertyReference propertyReference = DesignTimeProperties.GetAppliedShadowPropertyReference(this.TargetProperty, (ITypeId) this.Type);
        keyFrameSceneNode.Value = viewObject.GetBaseValue(propertyReference);
        list1.Add(keyFrameSceneNode);
        if (keyFrameCollection[0].InterpolationType == KeyFrameInterpolationType.Spline && keyFrameCollection[0].KeySpline != null)
          list2.Add(new Point?(keyFrameCollection[0].KeySpline.ControlPoint1));
        else if (!this.IsDiscreteOnly)
          list2.Add(new Point?(new Point()));
        else
          list2.Add(new Point?());
      }
      int num1 = 0;
      while (list1.Count < capacity)
      {
        if (keyFrameCollection[index2].InterpolationType != KeyFrameInterpolationType.Discrete)
          list2.Add(new Point?(keyFrameCollection[index2].EaseOutControlPoint));
        else
          list2.Add(new Point?());
        list1.Add(keyFrameCollection[index2]);
        keyFrameCollection.RemoveAt(index2);
        ++num1;
      }
      for (int index3 = 0; index3 < list1.Count; ++index3)
      {
        KeyFrameSceneNode keyFrameSceneNode1 = list1[index3];
        Point? nullable1 = list2[index3];
        Point? nullable2 = new Point?();
        if (keyFrameSceneNode1.InterpolationType != KeyFrameInterpolationType.Discrete)
          nullable2 = new Point?(keyFrameSceneNode1.EaseInControlPoint);
        double num2 = endTime - (keyFrameSceneNode1.Time - startTime);
        KeyFrameSceneNode node;
        if (nullable1.HasValue)
        {
          KeyFrameSceneNode keyFrameSceneNode2 = (KeyFrameSceneNode) KeyFrameSceneNode.Factory.Instantiate(this.ViewModel, this.GetKeyFrameType(KeyFrameInterpolationType.Spline));
          keyFrameSceneNode2.ValueNode = keyFrameSceneNode1.ValueNode.Clone(keyFrameSceneNode2.DocumentContext);
          node = keyFrameSceneNode2;
          node.EaseInControlPoint = new Point(1.0 - nullable1.Value.X, 1.0 - nullable1.Value.Y);
        }
        else
        {
          KeyFrameSceneNode keyFrameSceneNode2 = (KeyFrameSceneNode) KeyFrameSceneNode.Factory.Instantiate(this.ViewModel, this.GetKeyFrameType(KeyFrameInterpolationType.Discrete));
          keyFrameSceneNode2.ValueNode = keyFrameSceneNode1.ValueNode.Clone(keyFrameSceneNode2.DocumentContext);
          node = keyFrameSceneNode2;
        }
        node.Time = num2;
        keyFrameCollection.Insert(index2, node);
        if (nullable2.HasValue)
        {
          Point newEaseOut = new Point(1.0 - nullable2.Value.X, 1.0 - nullable2.Value.Y);
          KeyFrameSceneNode.SetEaseOutControlPoint(node, newEaseOut);
        }
      }
      if (!flag)
        return;
      Point? nullable = new Point?(keyFrameCollection[0].EaseOutControlPoint);
      if (nullable.HasValue)
        this.SetHandoffKeyFrameEaseOutPoint(nullable.Value);
      this.RemoveKeyFrame(0.0);
    }

    public override bool ChangeCanAffectTransition(SceneNode node, PropertyReference changedProperty)
    {
      KeyFrameSceneNode keyFrameSceneNode = node as KeyFrameSceneNode;
      if (keyFrameSceneNode != null)
        return this.ChangeAtIndexCanAffectTransition(this.KeyFrameCollection.IndexOf(keyFrameSceneNode));
      return false;
    }

    private ITypeId GetKeyFrameType(KeyFrameInterpolationType keyFrameInterpolationType)
    {
      ITypeId typeId;
      switch (keyFrameInterpolationType)
      {
        case KeyFrameInterpolationType.Discrete:
          AnimationSceneNode.PlatformNeutralTypeDictionaryTryGetValue(KeyFrameAnimationSceneNode.discreteKeyFrameTypes, this.AnimatedType, out typeId);
          break;
        case KeyFrameInterpolationType.Spline:
          AnimationSceneNode.PlatformNeutralTypeDictionaryTryGetValue(KeyFrameAnimationSceneNode.splineKeyFrameTypes, this.AnimatedType, out typeId);
          break;
        case KeyFrameInterpolationType.Easing:
          AnimationSceneNode.PlatformNeutralTypeDictionaryTryGetValue(KeyFrameAnimationSceneNode.easingKeyFrameTypes, this.AnimatedType, out typeId);
          break;
        default:
          typeId = (ITypeId) null;
          break;
      }
      return typeId;
    }

    internal KeyFrameSceneNode ReplaceKeyFrame(KeyFrameSceneNode keyFrame, KeyFrameInterpolationType interpolationType, KeySpline keySpline)
    {
      ITypeId keyFrameType = this.GetKeyFrameType(interpolationType);
      if (keyFrameType == null)
        return (KeyFrameSceneNode) null;
      KeyFrameSceneNode keyFrameSceneNode = (KeyFrameSceneNode) KeyFrameSceneNode.Factory.Instantiate(this.ViewModel, keyFrameType);
      keyFrameSceneNode.Time = keyFrame.Time;
      using (keyFrame.ViewModel.ForceBaseValue())
        keyFrameSceneNode.ValueNode = keyFrame.ValueNode.Clone(keyFrame.ValueNode.Context);
      if (interpolationType == KeyFrameInterpolationType.Spline)
        keyFrameSceneNode.KeySpline = keySpline;
      IList<KeyFrameSceneNode> keyFrameCollection = this.KeyFrameCollection;
      keyFrameCollection[keyFrameCollection.IndexOf(keyFrame)] = keyFrameSceneNode;
      return keyFrameSceneNode;
    }

    public void AddKeyFrame(KeyFrameSceneNode keyFrameSceneNode)
    {
      IList<KeyFrameSceneNode> keyFrameCollection = this.KeyFrameCollection;
      double time = keyFrameSceneNode.Time;
      this.ConvertKeySplineResourcesToLocalValues(keyFrameSceneNode);
      Point? nullable = new Point?();
      int index = 0;
      while (keyFrameCollection.Count > index && keyFrameCollection[index].Time < time)
        ++index;
      KeyFrameSceneNode keyFrameAtTime = this.GetKeyFrameAtTime(time);
      if (keyFrameAtTime != null)
      {
        int num = this.IndexOfKeyFrameAtTime(time);
        if (num != 0)
          nullable = new Point?(keyFrameCollection[num - 1].EaseOutControlPoint);
        else if (keyFrameCollection[0].InterpolationType != KeyFrameInterpolationType.Discrete)
          nullable = new Point?(this.GetHandoffKeyFrameEaseOutPoint());
        keyFrameCollection.Remove(keyFrameAtTime);
      }
      else if (index > 0)
        nullable = new Point?(keyFrameCollection[index - 1].EaseOutControlPoint);
      else if (keyFrameCollection.Count > 0 && keyFrameCollection[0].InterpolationType != KeyFrameInterpolationType.Discrete && keyFrameCollection[0].InterpolationType != KeyFrameInterpolationType.Easing)
        nullable = new Point?(this.GetHandoffKeyFrameEaseOutPoint());
      keyFrameCollection.Insert(index, keyFrameSceneNode);
      if (index != keyFrameCollection.Count - 1 && keyFrameSceneNode.CanSetEaseOutControlPoint)
        keyFrameSceneNode.EaseOutControlPoint = new Point();
      if (nullable.HasValue)
      {
        if (index != 0)
        {
          if (keyFrameCollection[index - 1].CanSetEaseOutControlPoint)
            keyFrameCollection[index - 1].EaseOutControlPoint = nullable.Value;
        }
        else
          this.SetHandoffKeyFrameEaseOutPoint(nullable.Value);
      }
      if (this.ControllingState == null || !this.ChangeAtIndexCanAffectTransition(index) && !this.NoAutoTransitionsProvided)
        return;
      this.ControllingState.UpdateTransitionsForStateValueChange(this.TargetElementAndProperty, keyFrameSceneNode.Value);
    }

    private bool ChangeAtIndexCanAffectTransition(int index)
    {
      return index == 0;
    }

    public class ConcreteKeyFrameAnimationSceneNodeFactory : AnimationSceneNode.ConcreteAnimationSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new KeyFrameAnimationSceneNode();
      }

      public KeyFrameAnimationSceneNode InstantiateWithTarget(SceneViewModel viewModel, SceneNode targetElement, PropertyReference targetProperty, IStoryboardContainer referenceStoryboardContainer, ITypeId type)
      {
        return (KeyFrameAnimationSceneNode) base.InstantiateWithTarget(viewModel, targetElement, targetProperty, referenceStoryboardContainer, type);
      }

      public KeyFrameAnimationSceneNode Instantiate(SceneViewModel viewModel, Type targetType)
      {
        ITypeId targetType1 = (ITypeId) viewModel.ProjectContext.GetType(targetType);
        return (KeyFrameAnimationSceneNode) this.Instantiate(viewModel, targetType1);
      }
    }
  }
}
