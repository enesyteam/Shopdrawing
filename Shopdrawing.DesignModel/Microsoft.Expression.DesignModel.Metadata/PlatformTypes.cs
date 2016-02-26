using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public abstract class PlatformTypes : IPlatformTypes, IPlatformMetadata, IMetadataResolver, IDisposable
	{
		private const string MicrosoftPublicKeyToken = "31bf3856ad364e35"; ///xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

		private const string TestPublicKeyToken = "1a2c496f4b3bbc64";

		public readonly static ITypeId Array;

		public readonly static ITypeId ArrayList;

		public readonly static ITypeId Boolean;

		public readonly static ITypeId Byte;

		public readonly static ITypeId Char;

		public readonly static ITypeId DateTime;

		public readonly static ITypeId Decimal;

		public readonly static ITypeId Delegate;

		public readonly static ITypeId Double;

		public readonly static ITypeId Enum;

		public readonly static ITypeId ICollection;

		public readonly static ITypeId ICollectionT;

		public readonly static ITypeId IConvertible;

		public readonly static ITypeId IDictionary;

		public readonly static ITypeId IEnumerable;

		public readonly static ITypeId IEnumerableT;

		public readonly static ITypeId IEnumerator;

		public readonly static ITypeId IEnumeratorT;

		public readonly static ITypeId IList;

		public readonly static ITypeId IListT;

		public readonly static ITypeId Int16;

		public readonly static ITypeId UInt16;

		public readonly static ITypeId Int32;

		public readonly static ITypeId UInt32;

		public readonly static ITypeId Int64;

		public readonly static ITypeId UInt64;

		public readonly static ITypeId Object;

		public readonly static ITypeId ObservableCollection;

		public readonly static ITypeId ReadOnlyCollection;

		public readonly static ITypeId SByte;

		public readonly static ITypeId Single;

		public readonly static ITypeId String;

		public readonly static ITypeId Type;

		public readonly static ITypeId Uri;

		public readonly static ITypeId UriKind;

		public readonly static ITypeId Void;

		public readonly static ITypeId StringBuilder;

		public readonly static ITypeId DefaultValueAttribute;

		public readonly static ITypeId DefaultBindingPropertyAttribute;

		public readonly static ITypeId DefaultEventAttribute;

		public readonly static ITypeId AlternateContentPropertyAttribute;

		public readonly static ITypeId FieldInfo;

		public readonly static ITypeId ConstructorInfo;

		public readonly static ITypeId MemberInfo;

		public readonly static ITypeId MethodInfo;

		public readonly static ITypeId PropertyInfo;

		public readonly static ITypeId IXmlSerializable;

		public readonly static ITypeId XData;

		public readonly static ITypeId ArrayExtension;

		public readonly static ITypeId INameScope;

		public readonly static ITypeId Null;

		public readonly static ITypeId TypeExtension;

		public readonly static ITypeId ContentPropertyAttribute;

		public readonly static ITypeId Application;

		public readonly static ITypeId AdornerDecorator;

		public readonly static ITypeId Binding;

		public readonly static ITypeId BindingBase;

		public readonly static ITypeId BitmapEffect;

		public readonly static ITypeId BitmapEffectGroup;

		public readonly static ITypeId BitmapCacheOption;

		public readonly static ITypeId Block;

		public readonly static ITypeId BlockCollection;

		public readonly static ITypeId BlockUIContainer;

		public readonly static ITypeId BlurBitmapEffect;

		public readonly static ITypeId DropShadowBitmapEffect;

		public readonly static ITypeId OuterGlowBitmapEffect;

		public readonly static ITypeId BitmapEffectInput;

		public readonly static ITypeId CollectionViewSource;

		public readonly static ITypeId ContentPresenter;

		public readonly static ITypeId ContextMenu;

		public readonly static ITypeId Control;

		public readonly static ITypeId ControllableStoryboardAction;

		public readonly static ITypeId ControlTemplate;

		public readonly static ITypeId TemplatePartAttribute;

		public readonly static ITypeId Cursor;

		public readonly static ITypeId DataTemplate;

		public readonly static ITypeId DependencyObject;

		public readonly static ITypeId DependencyProperty;

		public readonly static ITypeId DynamicResource;

		public readonly static ITypeId Duration;

		public readonly static ITypeId EventArgs;

		public readonly static ITypeId Expression;

		public readonly static ITypeId FontFamily;

		public readonly static ITypeId FontStretch;

		public readonly static ITypeId FontStyle;

		public readonly static ITypeId FontWeight;

		public readonly static ITypeId Frame;

		public readonly static ITypeId FrameworkContentElement;

		public readonly static ITypeId FrameworkElement;

		public readonly static ITypeId FrameworkTemplate;

		public readonly static ITypeId Glyphs;

		public readonly static ITypeId GridLength;

		public readonly static ITypeId ICommand;

		public readonly static ITypeId CommandConverter;

		public readonly static ITypeId IFrameworkInputElement;

		public readonly static ITypeId ImageSource;

		public readonly static ITypeId InputMethod;

		public readonly static ITypeId INotifyCollectionChanged;

		public readonly static ITypeId INotifyPropertyChanged;

		public readonly static ITypeId NotifyCollectionChangedEventHandler;

		public readonly static ITypeId KeySpline;

		public readonly static ITypeId KeyTime;

		public readonly static ITypeId Orientation;

		public readonly static ITypeId PropertyChangedEventHandler;

		public readonly static ITypeId PropertyChangedEventArgs;

		public readonly static ITypeId PropertyMetadata;

		public readonly static ITypeId PropertyPath;

		public readonly static ITypeId RepeatBehavior;

		public readonly static ITypeId RelativeSource;

		public readonly static ITypeId RelativeSourceMode;

		public readonly static ITypeId ResourceDictionary;

		public readonly static ITypeId RoutedEvent;

		public readonly static ITypeId Setter;

		public readonly static ITypeId StaticResource;

		public readonly static ITypeId Style;

		public readonly static ITypeId TemplateBinding;

		public readonly static ITypeId AccessText;

		public readonly static ITypeId AmbientLight;

		public readonly static ITypeId AnimationTimeline;

		public readonly static ITypeId BeginStoryboard;

		public readonly static ITypeId BindingMode;

		public readonly static ITypeId BitmapImage;

		public readonly static ITypeId Border;

		public readonly static ITypeId BrowsableAttribute;

		public readonly static ITypeId Brush;

		public readonly static ITypeId BulletDecorator;

		public readonly static ITypeId Button;

		public readonly static ITypeId ButtonBase;

		public readonly static ITypeId Camera;

		public readonly static ITypeId Canvas;

		public readonly static ITypeId CategoryAttribute;

		public readonly static ITypeId CheckBox;

		public readonly static ITypeId Color;

		public readonly static ITypeId ColumnDefinition;

		public readonly static ITypeId ColumnDefinitionCollection;

		public readonly static ITypeId ComboBox;

		public readonly static ITypeId ComboBoxItem;

		public readonly static ITypeId Condition;

		public readonly static ITypeId ConstructorArgumentAttribute;

		public readonly static ITypeId ContentControl;

		public readonly static ITypeId CornerRadius;

		public readonly static ITypeId DataTrigger;

		public readonly static ITypeId Decorator;

		public readonly static ITypeId DeepZoomImageTileSource;

		public readonly static ITypeId DescriptionAttribute;

		public readonly static ITypeId DesignerSerializationVisibilityAttribute;

		public readonly static ITypeId DictionaryEntry;

		public readonly static ITypeId DiffuseMaterial;

		public readonly static ITypeId DirectionalLight;

		public readonly static ITypeId DocumentViewer;

		public readonly static ITypeId DoubleAnimationUsingPath;

		public readonly static ITypeId DoubleCollection;

		public readonly static ITypeId Drawing;

		public readonly static ITypeId DrawingBrush;

		public readonly static ITypeId DrawingGroup;

		public readonly static ITypeId DrawingImage;

		public readonly static ITypeId EditorBrowsableAttribute;

		public readonly static ITypeId Ellipse;

		public readonly static ITypeId Effect;

		public readonly static ITypeId EmissiveMaterial;

		public readonly static ITypeId BlurEffect;

		public readonly static ITypeId DropShadowEffect;

		public readonly static ITypeId EventTrigger;

		public readonly static ITypeId FlowDocument;

		public readonly static ITypeId FlowDocumentScrollViewer;

		public readonly static ITypeId Freezable;

		public readonly static ITypeId GeometryModel3D;

		public readonly static ITypeId GradientBrush;

		public readonly static ITypeId GradientStop;

		public readonly static ITypeId GradientStopCollection;

		public readonly static ITypeId Grid;

		public readonly static ITypeId GridView;

		public readonly static ITypeId GridViewColumn;

		public readonly static ITypeId GridViewColumnHeader;

		public readonly static ITypeId GridViewHeaderRowPresenter;

		public readonly static ITypeId GroupBox;

		public readonly static ITypeId GroupStyle;

		public readonly static ITypeId HorizontalAlignment;

		public readonly static ITypeId WebBrowserBrush;

		public readonly static ITypeId Hyperlink;

		public readonly static ITypeId IKeyFrame;

		public readonly static ITypeId IKeyFrameAnimation;

		public readonly static ITypeId IValueConverter;

		public readonly static ITypeId Image;

		public readonly static ITypeId ImageBrush;

		public readonly static ITypeId InkCanvas;

		public readonly static ITypeId InkPresenter;

		public readonly static ITypeId Inline;

		public readonly static ITypeId InlineCollection;

		public readonly static ITypeId InlineUIContainer;

		public readonly static ITypeId ItemCollection;

		public readonly static ITypeId ItemsControl;

		public readonly static ITypeId ItemsPanelTemplate;

		public readonly static ITypeId ItemsPresenter;

		public readonly static ITypeId Light;

		public readonly static ITypeId LinearGradientBrush;

		public readonly static ITypeId List;

		public readonly static ITypeId ListBox;

		public readonly static ITypeId ListBoxItem;

		public readonly static ITypeId ListView;

		public readonly static ITypeId Material;

		public readonly static ITypeId MaterialGroup;

		public readonly static ITypeId Matrix;

		public readonly static ITypeId Matrix3D;

		public readonly static ITypeId MatrixAnimationUsingPath;

		public readonly static ITypeId MatrixCamera;

		public readonly static ITypeId MediaElement;

		public readonly static ITypeId MediaTimeline;

		public readonly static ITypeId Menu;

		public readonly static ITypeId MenuItem;

		public readonly static ITypeId Model3D;

		public readonly static ITypeId Model3DCollection;

		public readonly static ITypeId Model3DGroup;

		public readonly static ITypeId ModelVisual3D;

		public readonly static ITypeId ModelUIElement3D;

		public readonly static ITypeId MultiScaleImage;

		public readonly static ITypeId MultiScaleTileSource;

		public readonly static ITypeId ContainerUIElement3D;

		public readonly static ITypeId Viewport2DVisual3D;

		public readonly static ITypeId MultiTrigger;

		public readonly static ITypeId NavigationWindow;

		public readonly static ITypeId ObjectDataProvider;

		public readonly static ITypeId OrthographicCamera;

		public readonly static ITypeId Page;

		public readonly static ITypeId Panel;

		public readonly static ITypeId Paragraph;

		public readonly static ITypeId Path;

		public readonly static ITypeId Geometry;

		public readonly static ITypeId PathGeometry;

		public readonly static ITypeId PathFigure;

		public readonly static ITypeId PathSegment;

		public readonly static ITypeId PathSegmentCollection;

		public readonly static ITypeId PointCollection;

		public readonly static ITypeId ArcSegment;

		public readonly static ITypeId LineSegment;

		public readonly static ITypeId QuadraticBezierSegment;

		public readonly static ITypeId Quaternion;

		public readonly static ITypeId BezierSegment;

		public readonly static ITypeId PolyLineSegment;

		public readonly static ITypeId Polygon;

		public readonly static ITypeId PolyQuadraticBezierSegment;

		public readonly static ITypeId PolyBezierSegment;

		public readonly static ITypeId PathFigureCollection;

		public readonly static ITypeId PasswordBox;

		public readonly static ITypeId PauseStoryboard;

		public readonly static ITypeId PerspectiveCamera;

		public readonly static ITypeId ProjectionCamera;

		public readonly static ITypeId Point;

		public readonly static ITypeId Point3D;

		public readonly static ITypeId Point3DCollection;

		public readonly static ITypeId Point4D;

		public readonly static ITypeId PointAnimationUsingPath;

		public readonly static ITypeId PointLight;

		public readonly static ITypeId PointLightBase;

		public readonly static ITypeId Popup;

		public readonly static ITypeId ProgressBar;

		public readonly static ITypeId RadioButton;

		public readonly static ITypeId RadialGradientBrush;

		public readonly static ITypeId Rect;

		public readonly static ITypeId Rectangle;

		public readonly static ITypeId RectangleGeometry;

		public readonly static ITypeId RemoveStoryboard;

		public readonly static ITypeId RepeatButton;

		public readonly static ITypeId ResumeStoryboard;

		public readonly static ITypeId ResizeGrip;

		public readonly static ITypeId RichTextBox;

		public readonly static ITypeId Rect3D;

		public readonly static ITypeId RotateTransform;

		public readonly static ITypeId RotateTransform3D;

		public readonly static ITypeId Rotation3D;

		public readonly static ITypeId RowDefinition;

		public readonly static ITypeId RowDefinitionCollection;

		public readonly static ITypeId Run;

		public readonly static ITypeId ScaleTransform;

		public readonly static ITypeId ScaleTransform3D;

		public readonly static ITypeId ScrollBar;

		public readonly static ITypeId ScrollContentPresenter;

		public readonly static ITypeId ScrollViewer;

		public readonly static ITypeId Section;

		public readonly static ITypeId Selector;

		public readonly static ITypeId Separator;

		public readonly static ITypeId SetterBaseCollection;

		public readonly static ITypeId Shape;

		public readonly static ITypeId Size;

		public readonly static ITypeId Size3D;

		public readonly static ITypeId SkewTransform;

		public readonly static ITypeId SkipStoryboardToFill;

		public readonly static ITypeId Slider;

		public readonly static ITypeId SolidColorBrush;

		public readonly static ITypeId Span;

		public readonly static ITypeId SpecularMaterial;

		public readonly static ITypeId SpellCheck;

		public readonly static ITypeId SpotLight;

		public readonly static ITypeId StackPanel;

		public readonly static ITypeId StaticExtension;

		public readonly static ITypeId StopStoryboard;

		public readonly static ITypeId Storyboard;

		public readonly static ITypeId StreamGeometry;

		public readonly static ITypeId TextAlignment;

		public readonly static ITypeId TextBlock;

		public readonly static ITypeId TextBox;

		public readonly static ITypeId TextOptions;

		public readonly static ITypeId TextBoxBase;

		public readonly static ITypeId TextDecorationCollection;

		public readonly static ITypeId TextElement;

		public readonly static ITypeId TickBar;

		public readonly static ITypeId Thickness;

		public readonly static ITypeId Thumb;

		public readonly static ITypeId Timeline;

		public readonly static ITypeId TimelineCollection;

		public readonly static ITypeId TimeSpan;

		public readonly static ITypeId TileBrush;

		public readonly static ITypeId ToggleButton;

		public readonly static ITypeId ToolBar;

		public readonly static ITypeId ToolBarPanel;

		public readonly static ITypeId ToolBarTray;

		public readonly static ITypeId ToolTipService;

		public readonly static ITypeId ToolTip;

		public readonly static ITypeId Track;

		public readonly static ITypeId Transform;

		public readonly static ITypeId Transform3D;

		public readonly static ITypeId Transform3DCollection;

		public readonly static ITypeId Transform3DGroup;

		public readonly static ITypeId TransformCollection;

		public readonly static ITypeId TransformGroup;

		public readonly static ITypeId TranslateTransform;

		public readonly static ITypeId TranslateTransform3D;

		public readonly static ITypeId Trigger;

		public readonly static ITypeId TriggerAction;

		public readonly static ITypeId TriggerBase;

		public readonly static ITypeId TypeConverter;

		public readonly static ITypeId TypeConverterAttribute;

		public readonly static ITypeId TriggerCollection;

		public readonly static ITypeId UIElement;

		public readonly static ITypeId UIElementCollection;

		public readonly static ITypeId UniformGrid;

		public readonly static ITypeId UpdateSourceTrigger;

		public readonly static ITypeId UserControl;

		public readonly static ITypeId Vector;

		public readonly static ITypeId Vector3D;

		public readonly static ITypeId VerticalAlignment;

		public readonly static ITypeId VideoBrush;

		public readonly static ITypeId Viewport3D;

		public readonly static ITypeId Vector3DCollection;

		public readonly static ITypeId VectorCollection;

		public readonly static ITypeId VideoDrawing;

		public readonly static ITypeId VirtualizingStackPanel;

		public readonly static ITypeId Visual;

		public readonly static ITypeId Visual3D;

		public readonly static ITypeId VisualBrush;

		public readonly static ITypeId Window;

		public readonly static ITypeId WebBrowser;

		public readonly static ITypeId XmlDataProvider;

		public readonly static ITypeId BooleanAnimationUsingKeyFrames;

		public readonly static ITypeId ByteAnimation;

		public readonly static ITypeId ByteAnimationUsingKeyFrames;

		public readonly static ITypeId CharAnimationUsingKeyFrames;

		public readonly static ITypeId ColorAnimation;

		public readonly static ITypeId ColorAnimationUsingKeyFrames;

		public readonly static ITypeId DecimalAnimation;

		public readonly static ITypeId DecimalAnimationUsingKeyFrames;

		public readonly static ITypeId DoubleAnimation;

		public readonly static ITypeId DoubleAnimationUsingKeyFrames;

		public readonly static ITypeId Int16Animation;

		public readonly static ITypeId Int16AnimationUsingKeyFrames;

		public readonly static ITypeId Int32Animation;

		public readonly static ITypeId Int32AnimationUsingKeyFrames;

		public readonly static ITypeId Int64Animation;

		public readonly static ITypeId Int64AnimationUsingKeyFrames;

		public readonly static ITypeId MatrixAnimationUsingKeyFrames;

		public readonly static ITypeId ObjectAnimationUsingKeyFrames;

		public readonly static ITypeId Point3DAnimation;

		public readonly static ITypeId Point3DAnimationUsingKeyFrames;

		public readonly static ITypeId PointAnimation;

		public readonly static ITypeId PointAnimationUsingKeyFrames;

		public readonly static ITypeId QuaternionAnimation;

		public readonly static ITypeId QuaternionAnimationUsingKeyFrames;

		public readonly static ITypeId Rotation3DAnimation;

		public readonly static ITypeId Rotation3DAnimationUsingKeyFrames;

		public readonly static ITypeId RectAnimation;

		public readonly static ITypeId RectAnimationUsingKeyFrames;

		public readonly static ITypeId SingleAnimation;

		public readonly static ITypeId SingleAnimationUsingKeyFrames;

		public readonly static ITypeId SizeAnimation;

		public readonly static ITypeId SizeAnimationUsingKeyFrames;

		public readonly static ITypeId StringAnimationUsingKeyFrames;

		public readonly static ITypeId ThicknessAnimation;

		public readonly static ITypeId ThicknessAnimationUsingKeyFrames;

		public readonly static ITypeId Vector3DAnimation;

		public readonly static ITypeId Vector3DAnimationUsingKeyFrames;

		public readonly static ITypeId VectorAnimation;

		public readonly static ITypeId VectorAnimationUsingKeyFrames;

		public readonly static ITypeId BooleanKeyFrame;

		public readonly static ITypeId ByteKeyFrame;

		public readonly static ITypeId CharKeyFrame;

		public readonly static ITypeId ColorKeyFrame;

		public readonly static ITypeId DecimalKeyFrame;

		public readonly static ITypeId DoubleKeyFrame;

		public readonly static ITypeId Int16KeyFrame;

		public readonly static ITypeId Int32KeyFrame;

		public readonly static ITypeId Int64KeyFrame;

		public readonly static ITypeId MatrixKeyFrame;

		public readonly static ITypeId ObjectKeyFrame;

		public readonly static ITypeId Point3DKeyFrame;

		public readonly static ITypeId PointKeyFrame;

		public readonly static ITypeId QuaternionKeyFrame;

		public readonly static ITypeId Rotation3DKeyFrame;

		public readonly static ITypeId RectKeyFrame;

		public readonly static ITypeId SingleKeyFrame;

		public readonly static ITypeId SizeKeyFrame;

		public readonly static ITypeId StringKeyFrame;

		public readonly static ITypeId ThicknessKeyFrame;

		public readonly static ITypeId Vector3DKeyFrame;

		public readonly static ITypeId VectorKeyFrame;

		public readonly static ITypeId DiscreteBooleanKeyFrame;

		public readonly static ITypeId DiscreteByteKeyFrame;

		public readonly static ITypeId DiscreteCharKeyFrame;

		public readonly static ITypeId DiscreteColorKeyFrame;

		public readonly static ITypeId DiscreteDecimalKeyFrame;

		public readonly static ITypeId DiscreteDoubleKeyFrame;

		public readonly static ITypeId DiscreteInt16KeyFrame;

		public readonly static ITypeId DiscreteInt32KeyFrame;

		public readonly static ITypeId DiscreteInt64KeyFrame;

		public readonly static ITypeId DiscreteMatrixKeyFrame;

		public readonly static ITypeId DiscreteObjectKeyFrame;

		public readonly static ITypeId DiscretePoint3DKeyFrame;

		public readonly static ITypeId DiscretePointKeyFrame;

		public readonly static ITypeId DiscreteQuaternionKeyFrame;

		public readonly static ITypeId DiscreteRotation3DKeyFrame;

		public readonly static ITypeId DiscreteRectKeyFrame;

		public readonly static ITypeId DiscreteSingleKeyFrame;

		public readonly static ITypeId DiscreteSizeKeyFrame;

		public readonly static ITypeId DiscreteStringKeyFrame;

		public readonly static ITypeId DiscreteThicknessKeyFrame;

		public readonly static ITypeId DiscreteVector3DKeyFrame;

		public readonly static ITypeId DiscreteVectorKeyFrame;

		public readonly static ITypeId SplineByteKeyFrame;

		public readonly static ITypeId SplineColorKeyFrame;

		public readonly static ITypeId SplineDecimalKeyFrame;

		public readonly static ITypeId SplineDoubleKeyFrame;

		public readonly static ITypeId SplineInt16KeyFrame;

		public readonly static ITypeId SplineInt32KeyFrame;

		public readonly static ITypeId SplineInt64KeyFrame;

		public readonly static ITypeId SplinePoint3DKeyFrame;

		public readonly static ITypeId SplinePointKeyFrame;

		public readonly static ITypeId SplineQuaternionKeyFrame;

		public readonly static ITypeId SplineRotation3DKeyFrame;

		public readonly static ITypeId SplineRectKeyFrame;

		public readonly static ITypeId SplineSingleKeyFrame;

		public readonly static ITypeId SplineSizeKeyFrame;

		public readonly static ITypeId SplineThicknessKeyFrame;

		public readonly static ITypeId SplineVector3DKeyFrame;

		public readonly static ITypeId SplineVectorKeyFrame;

		public readonly static ITypeId IEasingFunction;

		public readonly static ITypeId EasingFunctionBase;

		public readonly static ITypeId EasingMode;

		public readonly static ITypeId BackEase;

		public readonly static ITypeId BounceEase;

		public readonly static ITypeId CircleEase;

		public readonly static ITypeId CubicEase;

		public readonly static ITypeId ElasticEase;

		public readonly static ITypeId ExponentialEase;

		public readonly static ITypeId PowerEase;

		public readonly static ITypeId QuadraticEase;

		public readonly static ITypeId QuarticEase;

		public readonly static ITypeId QuinticEase;

		public readonly static ITypeId SineEase;

		public readonly static ITypeId EasingByteKeyFrame;

		public readonly static ITypeId EasingColorKeyFrame;

		public readonly static ITypeId EasingDecimalKeyFrame;

		public readonly static ITypeId EasingDoubleKeyFrame;

		public readonly static ITypeId EasingInt16KeyFrame;

		public readonly static ITypeId EasingInt32KeyFrame;

		public readonly static ITypeId EasingInt64KeyFrame;

		public readonly static ITypeId EasingPoint3DKeyFrame;

		public readonly static ITypeId EasingPointKeyFrame;

		public readonly static ITypeId EasingQuaternionKeyFrame;

		public readonly static ITypeId EasingRotation3DKeyFrame;

		public readonly static ITypeId EasingRectKeyFrame;

		public readonly static ITypeId EasingSingleKeyFrame;

		public readonly static ITypeId EasingSizeKeyFrame;

		public readonly static ITypeId EasingThicknessKeyFrame;

		public readonly static ITypeId EasingVector3DKeyFrame;

		public readonly static ITypeId EasingVectorKeyFrame;

		public readonly static ITypeId Projection;

		public readonly static ITypeId PlaneProjection;

		public readonly static ITypeId HyperlinkButton;

		public readonly static ITypeId INavigate;

		public readonly static ITypeId CompositeTransform;

		public readonly static ITypeId AnnotationManager;

		public readonly static ITypeId Annotation;

		public readonly static ITypeId StandInPopup;

		public readonly static ITypeId DesignDataExtension;

		public readonly static ITypeId DesignInstanceExtension;

		internal readonly static IType NullTypeInstance;

		private IPlatformRuntimeContext platformRuntimeContext;

		private IPlatformReferenceContext platformReferenceContext;

		private Dictionary<AssemblyName, IAssembly> platformAssemblyTable = new Dictionary<AssemblyName, IAssembly>(new PlatformTypes.AssemblyNameComparer());

		private Microsoft.Expression.DesignModel.Metadata.DefaultTypeResolver defaultTypeResolver;

		private ITypeMetadataFactory typeMetadataFactory;

		private Microsoft.Expression.DesignModel.Metadata.CommonProperties commonProperties;

		private Dictionary<string, IAssembly> assemblies = new Dictionary<string, IAssembly>();

		private Dictionary<System.Type, IType> types = new Dictionary<System.Type, IType>();

		private Dictionary<System.Type, PlatformTypes.ExternalType> externalTypes = new Dictionary<System.Type, PlatformTypes.ExternalType>();

		private Dictionary<string, IType> typeNameToType = new Dictionary<string, IType>();

		private object platformCachesSyncLock = new object();

		private Dictionary<string, object> platformCaches;

		private bool isDisposed;

		private List<List<IAssemblyId>> assemblyGroupMapping;

		private static ICollection<Assembly> designToolAssemblies;

		private static Dictionary<FrameworkName, PlatformTypes.BlendSdkDescriptor> SdkDescriptors;

		internal uint PlatformTypeUniqueId;

		private BitArray[] isAssignableFromCache = new BitArray[1000];

		private IType[] typeCache = new IType[700];

		private Dictionary<PlatformNeutralPropertyId, IMemberId> memberCache = new Dictionary<PlatformNeutralPropertyId, IMemberId>();

		private object[] platformCapabilities = new object[System.Enum.GetValues(typeof(PlatformCapability)).Length];

		private static IList<PlatformCapabilitySettings> platformCapabilitySettings;

		public Microsoft.Expression.DesignModel.Metadata.CommonProperties CommonProperties
		{
			get
			{
				return this.commonProperties;
			}
		}

		public abstract ReadOnlyCollection<IAssembly> DefaultAssemblies
		{
			get;
		}

		public abstract ReadOnlyCollection<IAssembly> DefaultAssemblyReferences
		{
			get;
		}

		public ITypeResolver DefaultTypeResolver
		{
			get
			{
				if (this.defaultTypeResolver == null)
				{
					this.defaultTypeResolver = new Microsoft.Expression.DesignModel.Metadata.DefaultTypeResolver(this);
				}
				return this.defaultTypeResolver;
			}
		}

		public abstract Microsoft.Expression.DesignModel.Metadata.DesignTimeProperties DesignTimeProperties
		{
			get;
		}

		public static ICollection<Assembly> DesignToolAssemblies
		{
			get
			{
				return PlatformTypes.designToolAssemblies;
			}
		}

		public abstract string IdentityPrefix
		{
			get;
		}

		public abstract ICollection<string> ImageFileExtensions
		{
			get;
		}

		public string InteractionsAssemblyFullName
		{
			get;
			private set;
		}

		public string InteractivityAssemblyFullName
		{
			get;
			private set;
		}

		public abstract IKnownProperties KnownProperties
		{
			get;
		}

		public abstract IKnownTypes KnownTypes
		{
			get;
		}

		public IPlatformReferenceContext ReferenceContext
		{
			get
			{
				return this.platformReferenceContext;
			}
		}

		public IPlatformRuntimeContext RuntimeContext
		{
			get
			{
				return this.platformRuntimeContext;
			}
		}

		public abstract FrameworkName RuntimeFramework
		{
			get;
		}

		public FrameworkName TargetFramework
		{
			get
			{
				return this.platformReferenceContext.TargetFramework;
			}
		}

		protected bool TypeForwardingEnabled
		{
			get;
			set;
		}

		public ITypeMetadataFactory TypeMetadataFactory
		{
			get
			{
				return this.typeMetadataFactory;
			}
		}

		public abstract UndefinedClrPropertyImplementation UndefinedClrPropertyImplementationInstance
		{
			get;
		}

		public abstract XmlnsDefinitionMap XmlnsMap
		{
			get;
		}

		static PlatformTypes()
		{
			PlatformTypes.Array = new PlatformNeutralTypeId("System.Array");
			PlatformTypes.ArrayList = new PlatformNeutralTypeId("System.Collections.ArrayList");
			PlatformTypes.Boolean = new PlatformNeutralTypeId("System.Boolean");
			PlatformTypes.Byte = new PlatformNeutralTypeId("System.Byte");
			PlatformTypes.Char = new PlatformNeutralTypeId("System.Char");
			PlatformTypes.DateTime = new PlatformNeutralTypeId("System.DateTime");
			PlatformTypes.Decimal = new PlatformNeutralTypeId("System.Decimal");
			PlatformTypes.Delegate = new PlatformNeutralTypeId("System.Delegate");
			PlatformTypes.Double = new PlatformNeutralTypeId("System.Double");
			PlatformTypes.Enum = new PlatformNeutralTypeId("System.Enum");
			PlatformTypes.ICollection = new PlatformNeutralTypeId("System.Collections.ICollection");
			PlatformTypes.ICollectionT = new PlatformNeutralTypeId("System.Collections.Generic.ICollection`1");
			PlatformTypes.IConvertible = new PlatformNeutralTypeId("System.IConvertible");
			PlatformTypes.IDictionary = new PlatformNeutralTypeId("System.Collections.IDictionary");
			PlatformTypes.IEnumerable = new PlatformNeutralTypeId("System.Collections.IEnumerable");
			PlatformTypes.IEnumerableT = new PlatformNeutralTypeId("System.Collections.Generic.IEnumerable`1");
			PlatformTypes.IEnumerator = new PlatformNeutralTypeId("System.Collections.IEnumerator");
			PlatformTypes.IEnumeratorT = new PlatformNeutralTypeId("System.Collections.Generic.IEnumerator`1");
			PlatformTypes.IList = new PlatformNeutralTypeId("System.Collections.IList");
			PlatformTypes.IListT = new PlatformNeutralTypeId("System.Collections.Generic.IList`1");
			PlatformTypes.Int16 = new PlatformNeutralTypeId("System.Int16");
			PlatformTypes.UInt16 = new PlatformNeutralTypeId("System.UInt16");
			PlatformTypes.Int32 = new PlatformNeutralTypeId("System.Int32");
			PlatformTypes.UInt32 = new PlatformNeutralTypeId("System.UInt32");
			PlatformTypes.Int64 = new PlatformNeutralTypeId("System.Int64");
			PlatformTypes.UInt64 = new PlatformNeutralTypeId("System.UInt64");
			PlatformTypes.Object = new PlatformNeutralTypeId("System.Object");
			PlatformTypes.ObservableCollection = new PlatformNeutralTypeId("System.Collections.ObjectModel.ObservableCollection`1");
			PlatformTypes.ReadOnlyCollection = new PlatformNeutralTypeId("System.Collections.ObjectModel.ReadOnlyCollection`1");
			PlatformTypes.SByte = new PlatformNeutralTypeId("System.SByte");
			PlatformTypes.Single = new PlatformNeutralTypeId("System.Single");
			PlatformTypes.String = new PlatformNeutralTypeId("System.String");
			PlatformTypes.Type = new PlatformNeutralTypeId("System.Type");
			PlatformTypes.Uri = new PlatformNeutralTypeId("System.Uri");
			PlatformTypes.UriKind = new PlatformNeutralTypeId("System.UriKind");
			PlatformTypes.Void = new PlatformNeutralTypeId("System.Void");
			PlatformTypes.StringBuilder = new PlatformNeutralTypeId("System.Text.StringBuilder");
			PlatformTypes.DefaultValueAttribute = new PlatformNeutralTypeId("System.ComponentModel.DefaultValueAttribute");
			PlatformTypes.DefaultBindingPropertyAttribute = new PlatformNeutralTypeId("System.ComponentModel.DefaultBindingPropertyAttribute");
			PlatformTypes.DefaultEventAttribute = new PlatformNeutralTypeId("System.ComponentModel.DefaultEventAttribute");
			PlatformTypes.AlternateContentPropertyAttribute = new PlatformNeutralTypeId("System.ComponentModel.AlternateContentPropertyAttribute");
			PlatformTypes.FieldInfo = new PlatformNeutralTypeId("System.Reflection.FieldInfo");
			PlatformTypes.ConstructorInfo = new PlatformNeutralTypeId("System.Reflection.ConstructorInfo");
			PlatformTypes.MemberInfo = new PlatformNeutralTypeId("System.Reflection.MemberInfo");
			PlatformTypes.MethodInfo = new PlatformNeutralTypeId("System.Reflection.MethodInfo");
			PlatformTypes.PropertyInfo = new PlatformNeutralTypeId("System.Reflection.PropertyInfo");
			PlatformTypes.IXmlSerializable = new PlatformNeutralTypeId("System.Xml.Serialization.IXmlSerializable");
			PlatformTypes.XData = new PlatformNeutralTypeId("XData");
			PlatformTypes.ArrayExtension = new PlatformNeutralTypeId("System.Windows.Markup.ArrayExtension");
			PlatformTypes.INameScope = new PlatformNeutralTypeId("System.Windows.Markup.INameScope");
			PlatformTypes.Null = new PlatformNeutralTypeId("System.Windows.Markup.NullExtension");
			PlatformTypes.TypeExtension = new PlatformNeutralTypeId("System.Windows.Markup.TypeExtension");
			PlatformTypes.ContentPropertyAttribute = new PlatformNeutralTypeId("System.Windows.Markup.ContentPropertyAttribute");
			PlatformTypes.Application = new PlatformNeutralTypeId("System.Windows.Application");
			PlatformTypes.AdornerDecorator = new PlatformNeutralTypeId("System.Windows.Documents.AdornerDecorator");
			PlatformTypes.Binding = new PlatformNeutralTypeId("System.Windows.Data.Binding");
			PlatformTypes.BindingBase = new PlatformNeutralTypeId("System.Windows.Data.BindingBase");
			PlatformTypes.BitmapEffect = new PlatformNeutralTypeId("System.Windows.Media.Effects.BitmapEffect");
			PlatformTypes.BitmapEffectGroup = new PlatformNeutralTypeId("System.Windows.Media.Effects.BitmapEffectGroup");
			PlatformTypes.BitmapCacheOption = new PlatformNeutralTypeId("System.Windows.Media.Imaging.BitmapCacheOption");
			PlatformTypes.Block = new PlatformNeutralTypeId("System.Windows.Documents.Block");
			PlatformTypes.BlockCollection = new PlatformNeutralTypeId("System.Windows.Documents.BlockCollection");
			PlatformTypes.BlockUIContainer = new PlatformNeutralTypeId("System.Windows.Documents.BlockUIContainer");
			PlatformTypes.BlurBitmapEffect = new PlatformNeutralTypeId("System.Windows.Media.Effects.BlurBitmapEffect");
			PlatformTypes.DropShadowBitmapEffect = new PlatformNeutralTypeId("System.Windows.Media.Effects.DropShadowBitmapEffect");
			PlatformTypes.OuterGlowBitmapEffect = new PlatformNeutralTypeId("System.Windows.Media.Effects.OuterGlowBitmapEffect");
			PlatformTypes.BitmapEffectInput = new PlatformNeutralTypeId("System.Windows.Media.Effects.BitmapEffectInput");
			PlatformTypes.CollectionViewSource = new PlatformNeutralTypeId("System.Windows.Data.CollectionViewSource");
			PlatformTypes.ContentPresenter = new PlatformNeutralTypeId("System.Windows.Controls.ContentPresenter");
			PlatformTypes.ContextMenu = new PlatformNeutralTypeId("System.Windows.Controls.ContextMenu");
			PlatformTypes.Control = new PlatformNeutralTypeId("System.Windows.Controls.Control");
			PlatformTypes.ControllableStoryboardAction = new PlatformNeutralTypeId("System.Windows.Media.Animation.ControllableStoryboardAction");
			PlatformTypes.ControlTemplate = new PlatformNeutralTypeId("System.Windows.Controls.ControlTemplate");
			PlatformTypes.TemplatePartAttribute = new PlatformNeutralTypeId("System.Windows.TemplatePartAttribute");
			PlatformTypes.Cursor = new PlatformNeutralTypeId("System.Windows.Input.Cursor");
			PlatformTypes.DataTemplate = new PlatformNeutralTypeId("System.Windows.DataTemplate");
			PlatformTypes.DependencyObject = new PlatformNeutralTypeId("System.Windows.DependencyObject");
			PlatformTypes.DependencyProperty = new PlatformNeutralTypeId("System.Windows.DependencyProperty");
			PlatformTypes.DynamicResource = new PlatformNeutralTypeId("System.Windows.DynamicResourceExtension");
			PlatformTypes.Duration = new PlatformNeutralTypeId("System.Windows.Duration");
			PlatformTypes.EventArgs = new PlatformNeutralTypeId("System.EventArgs");
			PlatformTypes.Expression = new PlatformNeutralTypeId("System.Windows.Expression");
			PlatformTypes.FontFamily = new PlatformNeutralTypeId("System.Windows.Media.FontFamily");
			PlatformTypes.FontStretch = new PlatformNeutralTypeId("System.Windows.FontStretch");
			PlatformTypes.FontStyle = new PlatformNeutralTypeId("System.Windows.FontStyle");
			PlatformTypes.FontWeight = new PlatformNeutralTypeId("System.Windows.FontWeight");
			PlatformTypes.Frame = new PlatformNeutralTypeId("System.Windows.Controls.Frame");
			PlatformTypes.FrameworkContentElement = new PlatformNeutralTypeId("System.Windows.FrameworkContentElement");
			PlatformTypes.FrameworkElement = new PlatformNeutralTypeId("System.Windows.FrameworkElement");
			PlatformTypes.FrameworkTemplate = new PlatformNeutralTypeId("System.Windows.FrameworkTemplate");
			PlatformTypes.Glyphs = new PlatformNeutralTypeId("System.Windows.Documents.Glyphs");
			PlatformTypes.GridLength = new PlatformNeutralTypeId("System.Windows.GridLength");
			PlatformTypes.ICommand = new PlatformNeutralTypeId("System.Windows.Input.ICommand");
			PlatformTypes.CommandConverter = new PlatformNeutralTypeId("System.Windows.Input.CommandConverter");
			PlatformTypes.IFrameworkInputElement = new PlatformNeutralTypeId("System.Windows.IFrameworkInputElement");
			PlatformTypes.ImageSource = new PlatformNeutralTypeId("System.Windows.Media.ImageSource");
			PlatformTypes.InputMethod = new PlatformNeutralTypeId("System.Windows.Input.InputMethod");
			PlatformTypes.INotifyCollectionChanged = new PlatformNeutralTypeId("System.Collections.Specialized.INotifyCollectionChanged");
			PlatformTypes.INotifyPropertyChanged = new PlatformNeutralTypeId("System.ComponentModel.INotifyPropertyChanged");
			PlatformTypes.NotifyCollectionChangedEventHandler = new PlatformNeutralTypeId("System.Collections.Specialized.NotifyCollectionChangedEventHandler");
			PlatformTypes.KeySpline = new PlatformNeutralTypeId("System.Windows.Media.Animation.KeySpline");
			PlatformTypes.KeyTime = new PlatformNeutralTypeId("System.Windows.Media.Animation.KeyTime");
			PlatformTypes.Orientation = new PlatformNeutralTypeId("System.Windows.Controls.Orientation");
			PlatformTypes.PropertyChangedEventHandler = new PlatformNeutralTypeId("System.ComponentModel.PropertyChangedEventHandler");
			PlatformTypes.PropertyChangedEventArgs = new PlatformNeutralTypeId("System.ComponentModel.PropertyChangedEventArgs");
			PlatformTypes.PropertyMetadata = new PlatformNeutralTypeId("System.Windows.PropertyMetadata");
			PlatformTypes.PropertyPath = new PlatformNeutralTypeId("System.Windows.PropertyPath");
			PlatformTypes.RepeatBehavior = new PlatformNeutralTypeId("System.Windows.Media.Animation.RepeatBehavior");
			PlatformTypes.RelativeSource = new PlatformNeutralTypeId("System.Windows.Data.RelativeSource");
			PlatformTypes.RelativeSourceMode = new PlatformNeutralTypeId("System.Windows.Data.RelativeSourceMode");
			PlatformTypes.ResourceDictionary = new PlatformNeutralTypeId("System.Windows.ResourceDictionary");
			PlatformTypes.RoutedEvent = new PlatformNeutralTypeId("System.Windows.RoutedEvent");
			PlatformTypes.Setter = new PlatformNeutralTypeId("System.Windows.Setter");
			PlatformTypes.StaticResource = new PlatformNeutralTypeId("System.Windows.StaticResourceExtension");
			PlatformTypes.Style = new PlatformNeutralTypeId("System.Windows.Style");
			PlatformTypes.TemplateBinding = new PlatformNeutralTypeId("System.Windows.TemplateBindingExtension");
			PlatformTypes.AccessText = new PlatformNeutralTypeId("System.Windows.Controls.AccessText");
			PlatformTypes.AmbientLight = new PlatformNeutralTypeId("System.Windows.Media.Media3D.AmbientLight");
			PlatformTypes.AnimationTimeline = new PlatformNeutralTypeId("System.Windows.Media.Animation.AnimationTimeline");
			PlatformTypes.BeginStoryboard = new PlatformNeutralTypeId("System.Windows.Media.Animation.BeginStoryboard");
			PlatformTypes.BindingMode = new PlatformNeutralTypeId("System.Windows.Data.BindingMode");
			PlatformTypes.BitmapImage = new PlatformNeutralTypeId("System.Windows.Media.Imaging.BitmapImage");
			PlatformTypes.Border = new PlatformNeutralTypeId("System.Windows.Controls.Border");
			PlatformTypes.BrowsableAttribute = new PlatformNeutralTypeId("System.ComponentModel.BrowsableAttribute");
			PlatformTypes.Brush = new PlatformNeutralTypeId("System.Windows.Media.Brush");
			PlatformTypes.BulletDecorator = new PlatformNeutralTypeId("System.Windows.Controls.Primitives.BulletDecorator");
			PlatformTypes.Button = new PlatformNeutralTypeId("System.Windows.Controls.Button");
			PlatformTypes.ButtonBase = new PlatformNeutralTypeId("System.Windows.Controls.Primitives.ButtonBase");
			PlatformTypes.Camera = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Camera");
			PlatformTypes.Canvas = new PlatformNeutralTypeId("System.Windows.Controls.Canvas");
			PlatformTypes.CategoryAttribute = new PlatformNeutralTypeId("System.ComponentModel.CategoryAttribute");
			PlatformTypes.CheckBox = new PlatformNeutralTypeId("System.Windows.Controls.CheckBox");
			PlatformTypes.Color = new PlatformNeutralTypeId("System.Windows.Media.Color");
			PlatformTypes.ColumnDefinition = new PlatformNeutralTypeId("System.Windows.Controls.ColumnDefinition");
			PlatformTypes.ColumnDefinitionCollection = new PlatformNeutralTypeId("System.Windows.Controls.ColumnDefinitionCollection");
			PlatformTypes.ComboBox = new PlatformNeutralTypeId("System.Windows.Controls.ComboBox");
			PlatformTypes.ComboBoxItem = new PlatformNeutralTypeId("System.Windows.Controls.ComboBoxItem");
			PlatformTypes.Condition = new PlatformNeutralTypeId("System.Windows.Condition");
			PlatformTypes.ConstructorArgumentAttribute = new PlatformNeutralTypeId("System.Windows.Markup.ConstructorArgumentAttribute");
			PlatformTypes.ContentControl = new PlatformNeutralTypeId("System.Windows.Controls.ContentControl");
			PlatformTypes.CornerRadius = new PlatformNeutralTypeId("System.Windows.CornerRadius");
			PlatformTypes.DataTrigger = new PlatformNeutralTypeId("System.Windows.DataTrigger");
			PlatformTypes.Decorator = new PlatformNeutralTypeId("System.Windows.Controls.Decorator");
			PlatformTypes.DeepZoomImageTileSource = new PlatformNeutralTypeId("System.Windows.Media.DeepZoomImageTileSource");
			PlatformTypes.DescriptionAttribute = new PlatformNeutralTypeId("System.ComponentModel.DescriptionAttribute");
			PlatformTypes.DesignerSerializationVisibilityAttribute = new PlatformNeutralTypeId("System.ComponentModel.DesignerSerializationVisibilityAttribute");
			PlatformTypes.DictionaryEntry = new PlatformNeutralTypeId("System.Collections.DictionaryEntry");
			PlatformTypes.DiffuseMaterial = new PlatformNeutralTypeId("System.Windows.Media.Media3D.DiffuseMaterial");
			PlatformTypes.DirectionalLight = new PlatformNeutralTypeId("System.Windows.Media.Media3D.DirectionalLight");
			PlatformTypes.DocumentViewer = new PlatformNeutralTypeId("System.Windows.Controls.DocumentViewer");
			PlatformTypes.DoubleAnimationUsingPath = new PlatformNeutralTypeId("System.Windows.Media.Animation.DoubleAnimationUsingPath");
			PlatformTypes.DoubleCollection = new PlatformNeutralTypeId("System.Windows.Media.DoubleCollection");
			PlatformTypes.Drawing = new PlatformNeutralTypeId("System.Windows.Media.Drawing");
			PlatformTypes.DrawingBrush = new PlatformNeutralTypeId("System.Windows.Media.DrawingBrush");
			PlatformTypes.DrawingGroup = new PlatformNeutralTypeId("System.Windows.Media.DrawingGroup");
			PlatformTypes.DrawingImage = new PlatformNeutralTypeId("System.Windows.Media.DrawingImage");
			PlatformTypes.EditorBrowsableAttribute = new PlatformNeutralTypeId("System.ComponentModel.EditorBrowsableAttribute");
			PlatformTypes.Ellipse = new PlatformNeutralTypeId("System.Windows.Shapes.Ellipse");
			PlatformTypes.Effect = new PlatformNeutralTypeId("System.Windows.Media.Effects.Effect");
			PlatformTypes.EmissiveMaterial = new PlatformNeutralTypeId("System.Windows.Media.Media3D.EmissiveMaterial");
			PlatformTypes.BlurEffect = new PlatformNeutralTypeId("System.Windows.Media.Effects.BlurEffect");
			PlatformTypes.DropShadowEffect = new PlatformNeutralTypeId("System.Windows.Media.Effects.DropShadowEffect");
			PlatformTypes.EventTrigger = new PlatformNeutralTypeId("System.Windows.EventTrigger");
			PlatformTypes.FlowDocument = new PlatformNeutralTypeId("System.Windows.Documents.FlowDocument");
			PlatformTypes.FlowDocumentScrollViewer = new PlatformNeutralTypeId("System.Windows.Controls.FlowDocumentScrollViewer");
			PlatformTypes.Freezable = new PlatformNeutralTypeId("System.Windows.Freezable");
			PlatformTypes.GeometryModel3D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.GeometryModel3D");
			PlatformTypes.GradientBrush = new PlatformNeutralTypeId("System.Windows.Media.GradientBrush");
			PlatformTypes.GradientStop = new PlatformNeutralTypeId("System.Windows.Media.GradientStop");
			PlatformTypes.GradientStopCollection = new PlatformNeutralTypeId("System.Windows.Media.GradientStopCollection");
			PlatformTypes.Grid = new PlatformNeutralTypeId("System.Windows.Controls.Grid");
			PlatformTypes.GridView = new PlatformNeutralTypeId("System.Windows.Controls.GridView");
			PlatformTypes.GridViewColumn = new PlatformNeutralTypeId("System.Windows.Controls.GridViewColumn");
			PlatformTypes.GridViewColumnHeader = new PlatformNeutralTypeId("System.Windows.Controls.GridViewColumnHeader");
			PlatformTypes.GridViewHeaderRowPresenter = new PlatformNeutralTypeId("System.Windows.Controls.GridViewHeaderRowPresenter");
			PlatformTypes.GroupBox = new PlatformNeutralTypeId("System.Windows.Controls.GroupBox");
			PlatformTypes.GroupStyle = new PlatformNeutralTypeId("System.Windows.Controls.GroupStyle");
			PlatformTypes.HorizontalAlignment = new PlatformNeutralTypeId("System.Windows.HorizontalAlignment");
			PlatformTypes.WebBrowserBrush = new PlatformNeutralTypeId("System.Windows.Controls.WebBrowserBrush");
			PlatformTypes.Hyperlink = new PlatformNeutralTypeId("System.Windows.Documents.Hyperlink");
			PlatformTypes.IKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.IKeyFrame");
			PlatformTypes.IKeyFrameAnimation = new PlatformNeutralTypeId("System.Windows.Media.Animation.IKeyFrameAnimation");
			PlatformTypes.IValueConverter = new PlatformNeutralTypeId("System.Windows.Data.IValueConverter");
			PlatformTypes.Image = new PlatformNeutralTypeId("System.Windows.Controls.Image");
			PlatformTypes.ImageBrush = new PlatformNeutralTypeId("System.Windows.Media.ImageBrush");
			PlatformTypes.InkCanvas = new PlatformNeutralTypeId("System.Windows.Controls.InkCanvas");
			PlatformTypes.InkPresenter = new PlatformNeutralTypeId("System.Windows.Controls.InkPresenter");
			PlatformTypes.Inline = new PlatformNeutralTypeId("System.Windows.Documents.Inline");
			PlatformTypes.InlineCollection = new PlatformNeutralTypeId("System.Windows.Documents.InlineCollection");
			PlatformTypes.InlineUIContainer = new PlatformNeutralTypeId("System.Windows.Documents.InlineUIContainer");
			PlatformTypes.ItemCollection = new PlatformNeutralTypeId("System.Windows.Controls.ItemCollection");
			PlatformTypes.ItemsControl = new PlatformNeutralTypeId("System.Windows.Controls.ItemsControl");
			PlatformTypes.ItemsPanelTemplate = new PlatformNeutralTypeId("System.Windows.Controls.ItemsPanelTemplate");
			PlatformTypes.ItemsPresenter = new PlatformNeutralTypeId("System.Windows.Controls.ItemsPresenter");
			PlatformTypes.Light = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Light");
			PlatformTypes.LinearGradientBrush = new PlatformNeutralTypeId("System.Windows.Media.LinearGradientBrush");
			PlatformTypes.List = new PlatformNeutralTypeId("System.Windows.Documents.List");
			PlatformTypes.ListBox = new PlatformNeutralTypeId("System.Windows.Controls.ListBox");
			PlatformTypes.ListBoxItem = new PlatformNeutralTypeId("System.Windows.Controls.ListBoxItem");
			PlatformTypes.ListView = new PlatformNeutralTypeId("System.Windows.Controls.ListView");
			PlatformTypes.Material = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Material");
			PlatformTypes.MaterialGroup = new PlatformNeutralTypeId("System.Windows.Media.Media3D.MaterialGroup");
			PlatformTypes.Matrix = new PlatformNeutralTypeId("System.Windows.Media.Matrix");
			PlatformTypes.Matrix3D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Matrix3D");
			PlatformTypes.MatrixAnimationUsingPath = new PlatformNeutralTypeId("System.Windows.Media.Animation.MatrixAnimationUsingPath");
			PlatformTypes.MatrixCamera = new PlatformNeutralTypeId("System.Windows.Media.Media3D.MatrixCamera");
			PlatformTypes.MediaElement = new PlatformNeutralTypeId("System.Windows.Controls.MediaElement");
			PlatformTypes.MediaTimeline = new PlatformNeutralTypeId("System.Windows.Media.MediaTimeline");
			PlatformTypes.Menu = new PlatformNeutralTypeId("System.Windows.Controls.Menu");
			PlatformTypes.MenuItem = new PlatformNeutralTypeId("System.Windows.Controls.MenuItem");
			PlatformTypes.Model3D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Model3D");
			PlatformTypes.Model3DCollection = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Model3DCollection");
			PlatformTypes.Model3DGroup = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Model3DGroup");
			PlatformTypes.ModelVisual3D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.ModelVisual3D");
			PlatformTypes.ModelUIElement3D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.ModelUIElement3D");
			PlatformTypes.MultiScaleImage = new PlatformNeutralTypeId("System.Windows.Controls.MultiScaleImage");
			PlatformTypes.MultiScaleTileSource = new PlatformNeutralTypeId("System.Windows.Media.MultiScaleTileSource");
			PlatformTypes.ContainerUIElement3D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.ContainerUIElement3D");
			PlatformTypes.Viewport2DVisual3D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Viewport2DVisual3D");
			PlatformTypes.MultiTrigger = new PlatformNeutralTypeId("System.Windows.MultiTrigger");
			PlatformTypes.NavigationWindow = new PlatformNeutralTypeId("System.Windows.Navigation.NavigationWindow");
			PlatformTypes.ObjectDataProvider = new PlatformNeutralTypeId("System.Windows.Data.ObjectDataProvider");
			PlatformTypes.OrthographicCamera = new PlatformNeutralTypeId("System.Windows.Media.Media3D.OrthographicCamera");
			PlatformTypes.Page = new PlatformNeutralTypeId("System.Windows.Controls.Page");
			PlatformTypes.Panel = new PlatformNeutralTypeId("System.Windows.Controls.Panel");
			PlatformTypes.Paragraph = new PlatformNeutralTypeId("System.Windows.Documents.Paragraph");
			PlatformTypes.Path = new PlatformNeutralTypeId("System.Windows.Shapes.Path");
			PlatformTypes.Geometry = new PlatformNeutralTypeId("System.Windows.Media.Geometry");
			PlatformTypes.PathGeometry = new PlatformNeutralTypeId("System.Windows.Media.PathGeometry");
			PlatformTypes.PathFigure = new PlatformNeutralTypeId("System.Windows.Media.PathFigure");
			PlatformTypes.PathSegment = new PlatformNeutralTypeId("System.Windows.Media.PathSegment");
			PlatformTypes.PathSegmentCollection = new PlatformNeutralTypeId("System.Windows.Media.PathSegmentCollection");
			PlatformTypes.PointCollection = new PlatformNeutralTypeId("System.Windows.Media.PointCollection");
			PlatformTypes.ArcSegment = new PlatformNeutralTypeId("System.Windows.Media.ArcSegment");
			PlatformTypes.LineSegment = new PlatformNeutralTypeId("System.Windows.Media.LineSegment");
			PlatformTypes.QuadraticBezierSegment = new PlatformNeutralTypeId("System.Windows.Media.QuadraticBezierSegment");
			PlatformTypes.Quaternion = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Quaternion");
			PlatformTypes.BezierSegment = new PlatformNeutralTypeId("System.Windows.Media.BezierSegment");
			PlatformTypes.PolyLineSegment = new PlatformNeutralTypeId("System.Windows.Media.PolyLineSegment");
			PlatformTypes.Polygon = new PlatformNeutralTypeId("System.Windows.Shapes.Polygon");
			PlatformTypes.PolyQuadraticBezierSegment = new PlatformNeutralTypeId("System.Windows.Media.PolyQuadraticBezierSegment");
			PlatformTypes.PolyBezierSegment = new PlatformNeutralTypeId("System.Windows.Media.PolyBezierSegment");
			PlatformTypes.PathFigureCollection = new PlatformNeutralTypeId("System.Windows.Media.PathFigureCollection");
			PlatformTypes.PasswordBox = new PlatformNeutralTypeId("System.Windows.Controls.PasswordBox");
			PlatformTypes.PauseStoryboard = new PlatformNeutralTypeId("System.Windows.Media.Animation.PauseStoryboard");
			PlatformTypes.PerspectiveCamera = new PlatformNeutralTypeId("System.Windows.Media.Media3D.PerspectiveCamera");
			PlatformTypes.ProjectionCamera = new PlatformNeutralTypeId("System.Windows.Media.Media3D.ProjectionCamera");
			PlatformTypes.Point = new PlatformNeutralTypeId("System.Windows.Point");
			PlatformTypes.Point3D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Point3D");
			PlatformTypes.Point3DCollection = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Point3DCollection");
			PlatformTypes.Point4D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Point4D");
			PlatformTypes.PointAnimationUsingPath = new PlatformNeutralTypeId("System.Windows.Media.Animation.PointAnimationUsingPath");
			PlatformTypes.PointLight = new PlatformNeutralTypeId("System.Windows.Media.Media3D.PointLight");
			PlatformTypes.PointLightBase = new PlatformNeutralTypeId("System.Windows.Media.Media3D.PointLightBase");
			PlatformTypes.Popup = new PlatformNeutralTypeId("System.Windows.Controls.Primitives.Popup");
			PlatformTypes.ProgressBar = new PlatformNeutralTypeId("System.Windows.Controls.ProgressBar");
			PlatformTypes.RadioButton = new PlatformNeutralTypeId("System.Windows.Controls.RadioButton");
			PlatformTypes.RadialGradientBrush = new PlatformNeutralTypeId("System.Windows.Media.RadialGradientBrush");
			PlatformTypes.Rect = new PlatformNeutralTypeId("System.Windows.Rect");
			PlatformTypes.Rectangle = new PlatformNeutralTypeId("System.Windows.Shapes.Rectangle");
			PlatformTypes.RectangleGeometry = new PlatformNeutralTypeId("System.Windows.Media.RectangleGeometry");
			PlatformTypes.RemoveStoryboard = new PlatformNeutralTypeId("System.Windows.Media.Animation.RemoveStoryboard");
			PlatformTypes.RepeatButton = new PlatformNeutralTypeId("System.Windows.Controls.Primitives.RepeatButton");
			PlatformTypes.ResumeStoryboard = new PlatformNeutralTypeId("System.Windows.Media.Animation.ResumeStoryboard");
			PlatformTypes.ResizeGrip = new PlatformNeutralTypeId("System.Windows.Controls.Primitives.ResizeGrip");
			PlatformTypes.RichTextBox = new PlatformNeutralTypeId("System.Windows.Controls.RichTextBox");
			PlatformTypes.Rect3D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Rect3D");
			PlatformTypes.RotateTransform = new PlatformNeutralTypeId("System.Windows.Media.RotateTransform");
			PlatformTypes.RotateTransform3D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.RotateTransform3D");
			PlatformTypes.Rotation3D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Rotation3D");
			PlatformTypes.RowDefinition = new PlatformNeutralTypeId("System.Windows.Controls.RowDefinition");
			PlatformTypes.RowDefinitionCollection = new PlatformNeutralTypeId("System.Windows.Controls.RowDefinitionCollection");
			PlatformTypes.Run = new PlatformNeutralTypeId("System.Windows.Documents.Run");
			PlatformTypes.ScaleTransform = new PlatformNeutralTypeId("System.Windows.Media.ScaleTransform");
			PlatformTypes.ScaleTransform3D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.ScaleTransform3D");
			PlatformTypes.ScrollBar = new PlatformNeutralTypeId("System.Windows.Controls.Primitives.ScrollBar");
			PlatformTypes.ScrollContentPresenter = new PlatformNeutralTypeId("System.Windows.Controls.ScrollContentPresenter");
			PlatformTypes.ScrollViewer = new PlatformNeutralTypeId("System.Windows.Controls.ScrollViewer");
			PlatformTypes.Section = new PlatformNeutralTypeId("System.Windows.Documents.Section");
			PlatformTypes.Selector = new PlatformNeutralTypeId("System.Windows.Controls.Primitives.Selector");
			PlatformTypes.Separator = new PlatformNeutralTypeId("System.Windows.Controls.Separator");
			PlatformTypes.SetterBaseCollection = new PlatformNeutralTypeId("System.Windows.SetterBaseCollection");
			PlatformTypes.Shape = new PlatformNeutralTypeId("System.Windows.Shapes.Shape");
			PlatformTypes.Size = new PlatformNeutralTypeId("System.Windows.Size");
			PlatformTypes.Size3D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Size3D");
			PlatformTypes.SkewTransform = new PlatformNeutralTypeId("System.Windows.Media.SkewTransform");
			PlatformTypes.SkipStoryboardToFill = new PlatformNeutralTypeId("System.Windows.Media.Animation.SkipStoryboardToFill");
			PlatformTypes.Slider = new PlatformNeutralTypeId("System.Windows.Controls.Slider");
			PlatformTypes.SolidColorBrush = new PlatformNeutralTypeId("System.Windows.Media.SolidColorBrush");
			PlatformTypes.Span = new PlatformNeutralTypeId("System.Windows.Documents.Span");
			PlatformTypes.SpecularMaterial = new PlatformNeutralTypeId("System.Windows.Media.Media3D.SpecularMaterial");
			PlatformTypes.SpellCheck = new PlatformNeutralTypeId("System.Windows.Controls.SpellCheck");
			PlatformTypes.SpotLight = new PlatformNeutralTypeId("System.Windows.Media.Media3D.SpotLight");
			PlatformTypes.StackPanel = new PlatformNeutralTypeId("System.Windows.Controls.StackPanel");
			PlatformTypes.StaticExtension = new PlatformNeutralTypeId("System.Windows.Markup.StaticExtension");
			PlatformTypes.StopStoryboard = new PlatformNeutralTypeId("System.Windows.Media.Animation.StopStoryboard");
			PlatformTypes.Storyboard = new PlatformNeutralTypeId("System.Windows.Media.Animation.Storyboard");
			PlatformTypes.StreamGeometry = new PlatformNeutralTypeId("System.Windows.Media.StreamGeometry");
			PlatformTypes.TextAlignment = new PlatformNeutralTypeId("System.Windows.TextAlignment");
			PlatformTypes.TextBlock = new PlatformNeutralTypeId("System.Windows.Controls.TextBlock");
			PlatformTypes.TextBox = new PlatformNeutralTypeId("System.Windows.Controls.TextBox");
			PlatformTypes.TextOptions = new PlatformNeutralTypeId("System.Windows.Media.TextOptions");
			PlatformTypes.TextBoxBase = new PlatformNeutralTypeId("System.Windows.Controls.Primitives.TextBoxBase");
			PlatformTypes.TextDecorationCollection = new PlatformNeutralTypeId("System.Windows.TextDecorationCollection");
			PlatformTypes.TextElement = new PlatformNeutralTypeId("System.Windows.Documents.TextElement");
			PlatformTypes.TickBar = new PlatformNeutralTypeId("System.Windows.Controls.Primitives.TickBar");
			PlatformTypes.Thickness = new PlatformNeutralTypeId("System.Windows.Thickness");
			PlatformTypes.Thumb = new PlatformNeutralTypeId("System.Windows.Controls.Primitives.Thumb");
			PlatformTypes.Timeline = new PlatformNeutralTypeId("System.Windows.Media.Animation.Timeline");
			PlatformTypes.TimelineCollection = new PlatformNeutralTypeId("System.Windows.Media.Animation.TimelineCollection");
			PlatformTypes.TimeSpan = new PlatformNeutralTypeId("System.TimeSpan");
			PlatformTypes.TileBrush = new PlatformNeutralTypeId("System.Windows.Media.TileBrush");
			PlatformTypes.ToggleButton = new PlatformNeutralTypeId("System.Windows.Controls.Primitives.ToggleButton");
			PlatformTypes.ToolBar = new PlatformNeutralTypeId("System.Windows.Controls.ToolBar");
			PlatformTypes.ToolBarPanel = new PlatformNeutralTypeId("System.Windows.Controls.Primitives.ToolBarPanel");
			PlatformTypes.ToolBarTray = new PlatformNeutralTypeId("System.Windows.Controls.ToolBarTray");
			PlatformTypes.ToolTipService = new PlatformNeutralTypeId("System.Windows.Controls.ToolTipService");
			PlatformTypes.ToolTip = new PlatformNeutralTypeId("System.Windows.Controls.ToolTip");
			PlatformTypes.Track = new PlatformNeutralTypeId("System.Windows.Controls.Primitives.Track");
			PlatformTypes.Transform = new PlatformNeutralTypeId("System.Windows.Media.Transform");
			PlatformTypes.Transform3D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Transform3D");
			PlatformTypes.Transform3DCollection = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Transform3DCollection");
			PlatformTypes.Transform3DGroup = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Transform3DGroup");
			PlatformTypes.TransformCollection = new PlatformNeutralTypeId("System.Windows.Media.TransformCollection");
			PlatformTypes.TransformGroup = new PlatformNeutralTypeId("System.Windows.Media.TransformGroup");
			PlatformTypes.TranslateTransform = new PlatformNeutralTypeId("System.Windows.Media.TranslateTransform");
			PlatformTypes.TranslateTransform3D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.TranslateTransform3D");
			PlatformTypes.Trigger = new PlatformNeutralTypeId("System.Windows.Trigger");
			PlatformTypes.TriggerAction = new PlatformNeutralTypeId("System.Windows.TriggerAction");
			PlatformTypes.TriggerBase = new PlatformNeutralTypeId("System.Windows.TriggerBase");
			PlatformTypes.TypeConverter = new PlatformNeutralTypeId("System.ComponentModel.TypeConverter");
			PlatformTypes.TypeConverterAttribute = new PlatformNeutralTypeId("System.ComponentModel.TypeConverterAttribute");
			PlatformTypes.TriggerCollection = new PlatformNeutralTypeId("System.Windows.TriggerCollection");
			PlatformTypes.UIElement = new PlatformNeutralTypeId("System.Windows.UIElement");
			PlatformTypes.UIElementCollection = new PlatformNeutralTypeId("System.Windows.Controls.UIElementCollection");
			PlatformTypes.UniformGrid = new PlatformNeutralTypeId("System.Windows.Controls.Primitives.UniformGrid");
			PlatformTypes.UpdateSourceTrigger = new PlatformNeutralTypeId("System.Windows.Data.UpdateSourceTrigger");
			PlatformTypes.UserControl = new PlatformNeutralTypeId("System.Windows.Controls.UserControl");
			PlatformTypes.Vector = new PlatformNeutralTypeId("System.Windows.Vector");
			PlatformTypes.Vector3D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Vector3D");
			PlatformTypes.VerticalAlignment = new PlatformNeutralTypeId("System.Windows.VerticalAlignment");
			PlatformTypes.VideoBrush = new PlatformNeutralTypeId("System.Windows.Media.VideoBrush");
			PlatformTypes.Viewport3D = new PlatformNeutralTypeId("System.Windows.Controls.Viewport3D");
			PlatformTypes.Vector3DCollection = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Vector3DCollection");
			PlatformTypes.VectorCollection = new PlatformNeutralTypeId("System.Windows.Media.VectorCollection");
			PlatformTypes.VideoDrawing = new PlatformNeutralTypeId("System.Windows.Media.VideoDrawing");
			PlatformTypes.VirtualizingStackPanel = new PlatformNeutralTypeId("System.Windows.Controls.VirtualizingStackPanel");
			PlatformTypes.Visual = new PlatformNeutralTypeId("System.Windows.Media.Visual");
			PlatformTypes.Visual3D = new PlatformNeutralTypeId("System.Windows.Media.Media3D.Visual3D");
			PlatformTypes.VisualBrush = new PlatformNeutralTypeId("System.Windows.Media.VisualBrush");
			PlatformTypes.Window = new PlatformNeutralTypeId("System.Windows.Window");
			PlatformTypes.WebBrowser = new PlatformNeutralTypeId("System.Windows.Controls.WebBrowser");
			PlatformTypes.XmlDataProvider = new PlatformNeutralTypeId("System.Windows.Data.XmlDataProvider");
			PlatformTypes.BooleanAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.BooleanAnimationUsingKeyFrames");
			PlatformTypes.ByteAnimation = new PlatformNeutralTypeId("System.Windows.Media.Animation.ByteAnimation");
			PlatformTypes.ByteAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.ByteAnimationUsingKeyFrames");
			PlatformTypes.CharAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.CharAnimationUsingKeyFrames");
			PlatformTypes.ColorAnimation = new PlatformNeutralTypeId("System.Windows.Media.Animation.ColorAnimation");
			PlatformTypes.ColorAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.ColorAnimationUsingKeyFrames");
			PlatformTypes.DecimalAnimation = new PlatformNeutralTypeId("System.Windows.Media.Animation.DecimalAnimation");
			PlatformTypes.DecimalAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.DecimalAnimationUsingKeyFrames");
			PlatformTypes.DoubleAnimation = new PlatformNeutralTypeId("System.Windows.Media.Animation.DoubleAnimation");
			PlatformTypes.DoubleAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.DoubleAnimationUsingKeyFrames");
			PlatformTypes.Int16Animation = new PlatformNeutralTypeId("System.Windows.Media.Animation.Int16Animation");
			PlatformTypes.Int16AnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.Int16AnimationUsingKeyFrames");
			PlatformTypes.Int32Animation = new PlatformNeutralTypeId("System.Windows.Media.Animation.Int32Animation");
			PlatformTypes.Int32AnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.Int32AnimationUsingKeyFrames");
			PlatformTypes.Int64Animation = new PlatformNeutralTypeId("System.Windows.Media.Animation.Int64Animation");
			PlatformTypes.Int64AnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.Int64AnimationUsingKeyFrames");
			PlatformTypes.MatrixAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.MatrixAnimationUsingKeyFrames");
			PlatformTypes.ObjectAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.ObjectAnimationUsingKeyFrames");
			PlatformTypes.Point3DAnimation = new PlatformNeutralTypeId("System.Windows.Media.Animation.Point3DAnimation");
			PlatformTypes.Point3DAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.Point3DAnimationUsingKeyFrames");
			PlatformTypes.PointAnimation = new PlatformNeutralTypeId("System.Windows.Media.Animation.PointAnimation");
			PlatformTypes.PointAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.PointAnimationUsingKeyFrames");
			PlatformTypes.QuaternionAnimation = new PlatformNeutralTypeId("System.Windows.Media.Animation.QuaternionAnimation");
			PlatformTypes.QuaternionAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.QuaternionAnimationUsingKeyFrames");
			PlatformTypes.Rotation3DAnimation = new PlatformNeutralTypeId("System.Windows.Media.Animation.Rotation3DAnimation");
			PlatformTypes.Rotation3DAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.Rotation3DAnimationUsingKeyFrames");
			PlatformTypes.RectAnimation = new PlatformNeutralTypeId("System.Windows.Media.Animation.RectAnimation");
			PlatformTypes.RectAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.RectAnimationUsingKeyFrames");
			PlatformTypes.SingleAnimation = new PlatformNeutralTypeId("System.Windows.Media.Animation.SingleAnimation");
			PlatformTypes.SingleAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.SingleAnimationUsingKeyFrames");
			PlatformTypes.SizeAnimation = new PlatformNeutralTypeId("System.Windows.Media.Animation.SizeAnimation");
			PlatformTypes.SizeAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.SizeAnimationUsingKeyFrames");
			PlatformTypes.StringAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.StringAnimationUsingKeyFrames");
			PlatformTypes.ThicknessAnimation = new PlatformNeutralTypeId("System.Windows.Media.Animation.ThicknessAnimation");
			PlatformTypes.ThicknessAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.ThicknessAnimationUsingKeyFrames");
			PlatformTypes.Vector3DAnimation = new PlatformNeutralTypeId("System.Windows.Media.Animation.Vector3DAnimation");
			PlatformTypes.Vector3DAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.Vector3DAnimationUsingKeyFrames");
			PlatformTypes.VectorAnimation = new PlatformNeutralTypeId("System.Windows.Media.Animation.VectorAnimation");
			PlatformTypes.VectorAnimationUsingKeyFrames = new PlatformNeutralTypeId("System.Windows.Media.Animation.VectorAnimationUsingKeyFrames");
			PlatformTypes.BooleanKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.BooleanKeyFrame");
			PlatformTypes.ByteKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.ByteKeyFrame");
			PlatformTypes.CharKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.CharKeyFrame");
			PlatformTypes.ColorKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.ColorKeyFrame");
			PlatformTypes.DecimalKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DecimalKeyFrame");
			PlatformTypes.DoubleKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DoubleKeyFrame");
			PlatformTypes.Int16KeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.Int16KeyFrame");
			PlatformTypes.Int32KeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.Int32KeyFrame");
			PlatformTypes.Int64KeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.Int64KeyFrame");
			PlatformTypes.MatrixKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.MatrixKeyFrame");
			PlatformTypes.ObjectKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.ObjectKeyFrame");
			PlatformTypes.Point3DKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.Point3DKeyFrame");
			PlatformTypes.PointKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.PointKeyFrame");
			PlatformTypes.QuaternionKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.QuaternionKeyFrame");
			PlatformTypes.Rotation3DKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.Rotation3DKeyFrame");
			PlatformTypes.RectKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.RectKeyFrame");
			PlatformTypes.SingleKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SingleKeyFrame");
			PlatformTypes.SizeKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SizeKeyFrame");
			PlatformTypes.StringKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.StringKeyFrame");
			PlatformTypes.ThicknessKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.ThicknessKeyFrame");
			PlatformTypes.Vector3DKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.Vector3DKeyFrame");
			PlatformTypes.VectorKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.VectorKeyFrame");
			PlatformTypes.DiscreteBooleanKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteBooleanKeyFrame");
			PlatformTypes.DiscreteByteKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteByteKeyFrame");
			PlatformTypes.DiscreteCharKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteCharKeyFrame");
			PlatformTypes.DiscreteColorKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteColorKeyFrame");
			PlatformTypes.DiscreteDecimalKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteDecimalKeyFrame");
			PlatformTypes.DiscreteDoubleKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteDoubleKeyFrame");
			PlatformTypes.DiscreteInt16KeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteInt16KeyFrame");
			PlatformTypes.DiscreteInt32KeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteInt32KeyFrame");
			PlatformTypes.DiscreteInt64KeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteInt64KeyFrame");
			PlatformTypes.DiscreteMatrixKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteMatrixKeyFrame");
			PlatformTypes.DiscreteObjectKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteObjectKeyFrame");
			PlatformTypes.DiscretePoint3DKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscretePoint3DKeyFrame");
			PlatformTypes.DiscretePointKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscretePointKeyFrame");
			PlatformTypes.DiscreteQuaternionKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteQuaternionKeyFrame");
			PlatformTypes.DiscreteRotation3DKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteRotation3DKeyFrame");
			PlatformTypes.DiscreteRectKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteRectKeyFrame");
			PlatformTypes.DiscreteSingleKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteSingleKeyFrame");
			PlatformTypes.DiscreteSizeKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteSizeKeyFrame");
			PlatformTypes.DiscreteStringKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteStringKeyFrame");
			PlatformTypes.DiscreteThicknessKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteThicknessKeyFrame");
			PlatformTypes.DiscreteVector3DKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteVector3DKeyFrame");
			PlatformTypes.DiscreteVectorKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.DiscreteVectorKeyFrame");
			PlatformTypes.SplineByteKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SplineByteKeyFrame");
			PlatformTypes.SplineColorKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SplineColorKeyFrame");
			PlatformTypes.SplineDecimalKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SplineDecimalKeyFrame");
			PlatformTypes.SplineDoubleKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SplineDoubleKeyFrame");
			PlatformTypes.SplineInt16KeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SplineInt16KeyFrame");
			PlatformTypes.SplineInt32KeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SplineInt32KeyFrame");
			PlatformTypes.SplineInt64KeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SplineInt64KeyFrame");
			PlatformTypes.SplinePoint3DKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SplinePoint3DKeyFrame");
			PlatformTypes.SplinePointKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SplinePointKeyFrame");
			PlatformTypes.SplineQuaternionKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SplineQuaternionKeyFrame");
			PlatformTypes.SplineRotation3DKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SplineRotation3DKeyFrame");
			PlatformTypes.SplineRectKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SplineRectKeyFrame");
			PlatformTypes.SplineSingleKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SplineSingleKeyFrame");
			PlatformTypes.SplineSizeKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SplineSizeKeyFrame");
			PlatformTypes.SplineThicknessKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SplineThicknessKeyFrame");
			PlatformTypes.SplineVector3DKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SplineVector3DKeyFrame");
			PlatformTypes.SplineVectorKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.SplineVectorKeyFrame");
			PlatformTypes.IEasingFunction = new PlatformNeutralTypeId("System.Windows.Media.Animation.IEasingFunction");
			PlatformTypes.EasingFunctionBase = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingFunctionBase");
			PlatformTypes.EasingMode = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingMode");
			PlatformTypes.BackEase = new PlatformNeutralTypeId("System.Windows.Media.Animation.BackEase");
			PlatformTypes.BounceEase = new PlatformNeutralTypeId("System.Windows.Media.Animation.BounceEase");
			PlatformTypes.CircleEase = new PlatformNeutralTypeId("System.Windows.Media.Animation.CircleEase");
			PlatformTypes.CubicEase = new PlatformNeutralTypeId("System.Windows.Media.Animation.CubicEase");
			PlatformTypes.ElasticEase = new PlatformNeutralTypeId("System.Windows.Media.Animation.ElasticEase");
			PlatformTypes.ExponentialEase = new PlatformNeutralTypeId("System.Windows.Media.Animation.ExponentialEase");
			PlatformTypes.PowerEase = new PlatformNeutralTypeId("System.Windows.Media.Animation.PowerEase");
			PlatformTypes.QuadraticEase = new PlatformNeutralTypeId("System.Windows.Media.Animation.QuadraticEase");
			PlatformTypes.QuarticEase = new PlatformNeutralTypeId("System.Windows.Media.Animation.QuarticEase");
			PlatformTypes.QuinticEase = new PlatformNeutralTypeId("System.Windows.Media.Animation.QuinticEase");
			PlatformTypes.SineEase = new PlatformNeutralTypeId("System.Windows.Media.Animation.SineEase");
			PlatformTypes.EasingByteKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingByteKeyFrame");
			PlatformTypes.EasingColorKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingColorKeyFrame");
			PlatformTypes.EasingDecimalKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingDecimalKeyFrame");
			PlatformTypes.EasingDoubleKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingDoubleKeyFrame");
			PlatformTypes.EasingInt16KeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingInt16KeyFrame");
			PlatformTypes.EasingInt32KeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingInt32KeyFrame");
			PlatformTypes.EasingInt64KeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingInt64KeyFrame");
			PlatformTypes.EasingPoint3DKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingPoint3DKeyFrame");
			PlatformTypes.EasingPointKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingPointKeyFrame");
			PlatformTypes.EasingQuaternionKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingQuaternionKeyFrame");
			PlatformTypes.EasingRotation3DKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingRotation3DKeyFrame");
			PlatformTypes.EasingRectKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingRectKeyFrame");
			PlatformTypes.EasingSingleKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingSingleKeyFrame");
			PlatformTypes.EasingSizeKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingSizeKeyFrame");
			PlatformTypes.EasingThicknessKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingThicknessKeyFrame");
			PlatformTypes.EasingVector3DKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingVector3DKeyFrame");
			PlatformTypes.EasingVectorKeyFrame = new PlatformNeutralTypeId("System.Windows.Media.Animation.EasingVectorKeyFrame");
			PlatformTypes.Projection = new PlatformNeutralTypeId("System.Windows.Media.Projection");
			PlatformTypes.PlaneProjection = new PlatformNeutralTypeId("System.Windows.Media.PlaneProjection");
			PlatformTypes.HyperlinkButton = new PlatformNeutralTypeId("System.Windows.Controls.HyperlinkButton");
			PlatformTypes.INavigate = new PlatformNeutralTypeId("System.Windows.Controls.INavigate");
			PlatformTypes.CompositeTransform = new PlatformNeutralTypeId("System.Windows.Media.CompositeTransform");
			PlatformTypes.AnnotationManager = new PlatformNeutralTypeId("Microsoft.Expression.Platform.AnnotationManager");
			PlatformTypes.Annotation = new PlatformNeutralTypeId("Microsoft.Expression.Platform.Annotation");
			PlatformTypes.StandInPopup = new PlatformNeutralTypeId("Microsoft.Expression.Platform.WPF.InstanceBuilders.StandInPopup");
			PlatformTypes.DesignDataExtension = new PlatformNeutralTypeId(typeof(Microsoft.Expression.DesignModel.InstanceBuilders.DesignDataExtension).FullName);
			PlatformTypes.DesignInstanceExtension = new PlatformNeutralTypeId(typeof(Microsoft.Expression.DesignModel.InstanceBuilders.DesignInstanceExtension).FullName);
			PlatformTypes.NullTypeInstance = new PlatformTypes.NullType();
			HashSet<Assembly> assemblies = new HashSet<Assembly>();
			assemblies.Add(typeof(ITypeId).Assembly);
			assemblies.Add(typeof(PlatformTypes).Assembly);
			assemblies.Add(typeof(PropertyEntry).Assembly);
			PlatformTypes.designToolAssemblies = assemblies;
			PlatformTypes.SdkDescriptors = new Dictionary<FrameworkName, PlatformTypes.BlendSdkDescriptor>(PlatformTypes.BlendSdkDescriptor.EqualityComparerInstance)
			{
				{ new FrameworkName("Silverlight", new Version(3, 0), string.Empty), new PlatformTypes.BlendSdkDescriptor(new Version(2, 0, 5, 0), "31bf3856ad364e35", null) },
				{ new FrameworkName("Silverlight", new Version(4, 0), string.Empty), new PlatformTypes.BlendSdkDescriptor(new Version(4, 0, 5, 0), "31bf3856ad364e35", "1a2c496f4b3bbc64") },
				{ new FrameworkName(".NETFramework", new Version(3, 5), string.Empty), new PlatformTypes.BlendSdkDescriptor(new Version(3, 5, 0, 0), "31bf3856ad364e35", "31bf3856ad364e35") },
				{ new FrameworkName(".NETFramework", new Version(4, 0), string.Empty), new PlatformTypes.BlendSdkDescriptor(new Version(4, 0, 0, 0), "31bf3856ad364e35", "31bf3856ad364e35") },
				{ new FrameworkName("Silverlight", new Version(4, 0), "WindowsPhone"), new PlatformTypes.BlendSdkDescriptor(new Version(3, 7, 5, 0), "31bf3856ad364e35", "1a2c496f4b3bbc64") }
			};
		}

		protected PlatformTypes(IPlatformRuntimeContext platformRuntimeContext, IPlatformReferenceContext platformReferenceContext)
		{
			unsafe
			{
				this.platformRuntimeContext = platformRuntimeContext;
				this.platformReferenceContext = platformReferenceContext;
				this.typeMetadataFactory = this.CreateTypeMetadataFactory(this.DefaultTypeResolver);
				this.commonProperties = new Microsoft.Expression.DesignModel.Metadata.CommonProperties(this);
				this.platformCaches = new Dictionary<string, object>();
				this.TypeForwardingEnabled = true;
			}
		}

		public void AddAssemblyGroupMapping(AssemblyGroup assemblyGroup, string assemblyName)
		{
			if (this.assemblyGroupMapping == null)
			{
				int length = System.Enum.GetValues(typeof(AssemblyGroup)).Length;
				this.assemblyGroupMapping = new List<List<IAssemblyId>>(length);
				for (int i = 0; i < length; i++)
				{
					this.assemblyGroupMapping.Add(new List<IAssemblyId>());
				}
			}
			IAssemblyId assemblyId = AssemblyHelper.ParseAssemblyId(assemblyName);
			this.assemblyGroupMapping[(int)assemblyGroup].Add(assemblyId);
		}

		public static void AddCapabilitySettings(PlatformCapabilitySettings settings)
		{
			if (settings.RequiredTargetFramework == null)
			{
				return;
			}
			if (PlatformTypes.platformCapabilitySettings == null)
			{
				PlatformTypes.platformCapabilitySettings = new List<PlatformCapabilitySettings>();
			}
			PlatformTypes.platformCapabilitySettings.Add(settings);
		}

		private bool AllTypeArgumentsInPlatformAssemblies(System.Type type)
		{
			if (type.IsGenericType)
			{
				System.Type[] genericTypeArguments = PlatformTypeHelper.GetGenericTypeArguments(type);
				if (genericTypeArguments == null)
				{
					return false;
				}
				System.Type[] typeArray = genericTypeArguments;
				for (int i = 0; i < (int)typeArray.Length; i++)
				{
					if (this.GetPlatformAssembly(typeArray[i]) == null)
					{
						return false;
					}
				}
			}
			return true;
		}

		protected void ApplyCapabilitySettings()
		{
			if (PlatformTypes.platformCapabilitySettings != null)
			{
				foreach (PlatformCapabilitySettings platformCapabilitySetting in PlatformTypes.platformCapabilitySettings)
				{
					if (!platformCapabilitySetting.RequiredTargetFramework.Identifier.Equals(this.TargetFramework.Identifier, StringComparison.OrdinalIgnoreCase) || !platformCapabilitySetting.RequiredTargetFramework.Profile.Equals(this.TargetFramework.Profile, StringComparison.OrdinalIgnoreCase) || !(platformCapabilitySetting.RequiredTargetFramework.Version <= this.TargetFramework.Version) || !(platformCapabilitySetting.MaxFrameworkVersion == null) && !(platformCapabilitySetting.MaxFrameworkVersion >= this.TargetFramework.Version))
					{
						continue;
					}
					foreach (PlatformCapability key in platformCapabilitySetting.Capabilities.Keys)
					{
						this.SetCapabilityValue(key, platformCapabilitySetting.Capabilities[key]);
					}
				}
			}
		}

		protected PropertyReference ConvertPropertyReference(PropertyReference propertyReference, IPlatformMetadata destinationPlatformMetadata)
		{
			PlatformTypes platformType = (PlatformTypes)destinationPlatformMetadata;
			if (propertyReference.Count == 0 || propertyReference.PlatformMetadata == platformType)
			{
				return propertyReference;
			}
			ITypeResolver defaultTypeResolver = platformType.DefaultTypeResolver;
			List<ReferenceStep> referenceSteps = new List<ReferenceStep>(propertyReference.Count);
			for (int i = 0; i < propertyReference.Count; i++)
			{
				ReferenceStep item = propertyReference[i];
				IndexedClrPropertyReferenceStep indexedClrPropertyReferenceStep = item as IndexedClrPropertyReferenceStep;
				ClrPropertyReferenceStep clrPropertyReferenceStep = item as ClrPropertyReferenceStep;
				DependencyPropertyReferenceStep dependencyPropertyReferenceStep = item as DependencyPropertyReferenceStep;
				ITypeId declaringTypeIdFrom = PlatformTypes.GetDeclaringTypeIdFrom(item);
				ReferenceStep referenceStep = null;
				if (indexedClrPropertyReferenceStep != null)
				{
					System.Type runtimeType = PlatformTypeHelper.ConvertTypeId(declaringTypeIdFrom, platformType).RuntimeType;
					referenceStep = IndexedClrPropertyReferenceStep.GetReferenceStep(defaultTypeResolver, runtimeType, indexedClrPropertyReferenceStep.Index);
				}
				else if (clrPropertyReferenceStep != null)
				{
					ITypeId typeId = PlatformTypeHelper.ConvertTypeId(declaringTypeIdFrom, platformType);
					referenceStep = (ReferenceStep)typeId.GetMember(MemberType.Property, clrPropertyReferenceStep.Name, MemberAccessTypes.Public);
				}
				else if (dependencyPropertyReferenceStep != null)
				{
					ITypeId typeId1 = PlatformTypeHelper.ConvertTypeId(declaringTypeIdFrom, platformType);
					referenceStep = (ReferenceStep)typeId1.GetMember(MemberType.Property, dependencyPropertyReferenceStep.Name, MemberAccessTypes.Public);
				}
				if (referenceStep == null)
				{
					return null;
				}
				referenceSteps.Add(referenceStep);
			}
			return new PropertyReference(referenceSteps);
		}

		public static PropertyReference ConvertToPlatformPropertyReference(PropertyReference propertyReference, IPlatformMetadata destinationPlatformMetadata)
		{
			PlatformTypes platformType = (PlatformTypes)destinationPlatformMetadata;
			if (propertyReference.Count == 0 || propertyReference.PlatformMetadata == platformType)
			{
				return propertyReference;
			}
			ITypeResolver defaultTypeResolver = platformType.DefaultTypeResolver;
			List<ReferenceStep> referenceSteps = new List<ReferenceStep>(propertyReference.Count);
			for (int i = 0; i < propertyReference.Count; i++)
			{
				ReferenceStep item = propertyReference[i];
				IndexedClrPropertyReferenceStep indexedClrPropertyReferenceStep = item as IndexedClrPropertyReferenceStep;
				ClrPropertyReferenceStep clrPropertyReferenceStep = item as ClrPropertyReferenceStep;
				DependencyPropertyReferenceStep dependencyPropertyReferenceStep = item as DependencyPropertyReferenceStep;
				ITypeId declaringTypeIdFrom = PlatformTypes.GetDeclaringTypeIdFrom(item);
				ReferenceStep referenceStep = null;
				if (indexedClrPropertyReferenceStep != null)
				{
					System.Type runtimeType = PlatformTypeHelper.ConvertTypeId(declaringTypeIdFrom, platformType).RuntimeType;
					referenceStep = IndexedClrPropertyReferenceStep.GetReferenceStep(defaultTypeResolver, runtimeType, indexedClrPropertyReferenceStep.Index);
				}
				else if (clrPropertyReferenceStep != null)
				{
					ITypeId typeId = PlatformTypeHelper.ConvertTypeId(declaringTypeIdFrom, platformType);
					referenceStep = (ReferenceStep)typeId.GetMember(MemberType.Property, clrPropertyReferenceStep.Name, MemberAccessTypes.Public);
				}
				else if (dependencyPropertyReferenceStep != null)
				{
					ITypeId typeId1 = PlatformTypeHelper.ConvertTypeId(declaringTypeIdFrom, platformType);
					referenceStep = (ReferenceStep)typeId1.GetMember(MemberType.Property, dependencyPropertyReferenceStep.Name, MemberAccessTypes.Public);
				}
				if (referenceStep == null)
				{
					return null;
				}
				referenceSteps.Add(referenceStep);
			}
			return new PropertyReference(referenceSteps);
		}

		public IAssembly CreateAssembly(string name)
		{
			return new Microsoft.Expression.DesignModel.Metadata.RuntimeAssembly(name, AssemblySource.Unknown);
		}

		public IAssembly CreateAssembly(Assembly assembly, AssemblySource assemblySource)
		{
			return this.CreateAssembly(assembly, assemblySource, false);
		}

		public IAssembly CreateAssembly(Assembly assembly, AssemblySource assemblySource, bool isImplicitlyResolved)
		{
			return new Microsoft.Expression.DesignModel.Metadata.RuntimeAssembly(assembly, assemblySource, isImplicitlyResolved);
		}

		public ITypeMetadataFactory CreateTypeMetadataFactory(ITypeResolver typeResolver)
		{
			PlatformTypes platformType = this;
			return new Microsoft.Expression.DesignModel.Metadata.TypeMetadataFactory(typeResolver, new Action<ITypeMetadataFactory>(platformType.RegisterTypeMetadata));
		}

		public IMember CreateUnknownMember(IType declaringType, MemberType memberType, string memberName)
		{
			return new UnknownMember(declaringType, memberType, memberName);
		}

		public IType CreateUnknownType(ITypeResolver typeResolver, IAssembly assembly, string clrNamespace, string typeName)
		{
			return new UnknownType(typeResolver, assembly, clrNamespace, typeName);
		}

		public IType CreateUnknownType(ITypeResolver typeResolver, IXmlNamespace xmlNamespace, string typeName)
		{
			return new UnknownType(typeResolver, xmlNamespace, typeName);
		}

		public abstract XmlnsDefinitionMap CreateXmlnsDefinitionMap(ITypeResolver typeResolver, IEnumerable<IAssembly> assemblies, IAssembly targetAssembly);

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			List<object> objs;
			if (disposing && !this.isDisposed)
			{
				lock (this.platformCachesSyncLock)
				{
					objs = new List<object>(this.platformCaches.Values);
					this.platformCaches.Clear();
				}
				foreach (object obj in objs)
				{
					IDisposable disposable = obj as IDisposable;
					if (disposable == null)
					{
						continue;
					}
					disposable.Dispose();
				}
				this.isDisposed = true;
			}
		}

		public abstract bool EnsureAssemblyReferenced(ITypeResolver typeResolver, ITypeId type);

		protected static System.Type FindTypeInAssembly(string name, Assembly assembly)
		{
			System.Type type = assembly.GetType(name, false);
			if (type != null && type.IsPublic)
			{
				return type;
			}
			return null;
		}

		protected IAssembly GetAssembly(Assembly assembly, AssemblySource assemblySource)
		{
			IAssembly assembly1;
			if (assembly == null)
			{
				return null;
			}
			string name = AssemblyHelper.GetAssemblyName(assembly).Name;
			if (!this.assemblies.TryGetValue(name, out assembly1))
			{
				assembly1 = this.CreateAssembly(assembly, assemblySource);
				this.assemblies.Add(name, assembly1);
			}
			return assembly1;
		}

		public IEnumerable<IAssemblyId> GetAssemblyGroup(AssemblyGroup assemblyGroup)
		{
			return this.assemblyGroupMapping[(int)assemblyGroup];
		}

		public object GetCapabilityValue(PlatformCapability capability)
		{
			return this.platformCapabilities[(int)capability];
		}

		public virtual IType GetContentWrapperType(ITypeResolver typeResolver, IType parentTypeId, IType childTypeId)
		{
			IType type;
			System.Type runtimeType = childTypeId.NearestResolvedType.RuntimeType;
			System.Collections.IEnumerator enumerator = TypeUtilities.GetAttributes(parentTypeId.NearestResolvedType.RuntimeType).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ContentWrapperAttribute current = (Attribute)enumerator.Current as ContentWrapperAttribute;
					if (current == null)
					{
						continue;
					}
					IType type1 = typeResolver.GetType(current.ContentWrapper);
					IProperty defaultContentProperty = type1.Metadata.DefaultContentProperty;
					if (defaultContentProperty == null || !defaultContentProperty.PropertyType.NearestResolvedType.RuntimeType.IsAssignableFrom(runtimeType))
					{
						continue;
					}
					type = type1;
					return type;
				}
				return null;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return type;
		}

		public Attribute[] GetCustomAttributes(System.Reflection.MemberInfo memberInfo)
		{
			return this.GetCustomAttributes(memberInfo, null, true);
		}

		public Attribute[] GetCustomAttributes(System.Reflection.MemberInfo memberInfo, bool inherit)
		{
			return this.GetCustomAttributes(memberInfo, null, inherit);
		}

		public Attribute[] GetCustomAttributes(System.Reflection.MemberInfo memberInfo, System.Type attributeType, bool inherit)
		{
			if (this.IsPlatformInternal(memberInfo))
			{
				return new Attribute[0];
			}
			return TypeUtilities.GetAttributes(memberInfo, attributeType, inherit);
		}

		private static ITypeId GetDeclaringTypeIdFrom(ReferenceStep step)
		{
			ITypeId declaringTypeId = step.DeclaringTypeId;
			if (!((PlatformTypes)step.DeclaringType.PlatformMetadata).IsCapabilitySet(PlatformCapability.IsWpf) && declaringTypeId.Equals(PlatformTypes.TextElement))
			{
				declaringTypeId = PlatformTypes.Inline;
			}
			return declaringTypeId;
		}

		public System.Reflection.FieldInfo GetDependencyPropertyField(System.Type type, string propertyName)
		{
			System.Reflection.FieldInfo field = type.GetField(string.Concat(propertyName, "Property"), BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (field != null && PlatformTypes.DependencyProperty.IsAssignableFrom(this.GetType(field.FieldType)))
			{
				return field;
			}
			return null;
		}

		public static System.Windows.DependencyProperty GetDependencyPropertyFromName(string name, System.Type ownerType)
		{
			System.Type type = (typeof(System.Windows.DependencyObject).IsAssignableFrom(ownerType) ? ownerType : typeof(System.Windows.DependencyObject));
			DependencyPropertyDescriptor dependencyPropertyDescriptor = null;
			try
			{
				dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromName(name, ownerType, type);
			}
			catch (Exception exception)
			{
			}
			if (dependencyPropertyDescriptor != null)
			{
				return dependencyPropertyDescriptor.DependencyProperty;
			}
			try
			{
				System.Reflection.FieldInfo field = ownerType.GetField(string.Concat(name, "Property"));
				if (field != null && typeof(System.Windows.DependencyProperty).IsAssignableFrom(field.FieldType))
				{
					System.Windows.DependencyProperty value = (System.Windows.DependencyProperty)field.GetValue(null);
					if (value != null && value.Name == name)
					{
						return value;
					}
				}
			}
			catch (Exception exception1)
			{
			}
			return null;
		}

		public IProperty GetDesignTimeProperty(string propertyName, IType targetType)
		{
			return Microsoft.Expression.DesignModel.Metadata.DesignTimeProperties.FromName(propertyName, this, targetType);
		}

		public virtual IType GetDesignTimeType(ITypeResolver typeResolver, IXmlNamespace xmlNamespace, string typeName)
		{
			IType designTimeType = null;
			if (xmlNamespace == XmlNamespace.XamlXmlNamespace && typeName == "XData")
			{
				designTimeType = this.GetDesignTimeType(PlatformTypes.XData);
			}
			else if (xmlNamespace == XmlNamespace.DesignTimeXmlNamespace)
			{
				if (typeName == "DesignInstanceExtension")
				{
					designTimeType = typeResolver.GetType(typeof(Microsoft.Expression.DesignModel.InstanceBuilders.DesignInstanceExtension));
				}
				if (typeName == "DesignDataExtension")
				{
					designTimeType = typeResolver.GetType(typeof(Microsoft.Expression.DesignModel.InstanceBuilders.DesignDataExtension));
				}
			}
			return designTimeType;
		}

		public virtual IType GetDesignTimeType(ITypeId typeId)
		{
			IType xDataType = null;
			if (typeId == PlatformTypes.XData)
			{
				xDataType = new XDataType(this);
			}
			return xDataType;
		}

		public IAssembly GetDesignToolAssembly(Assembly assembly)
		{
			return this.GetAssembly(assembly, AssemblySource.Blend);
		}

		private System.Type GetForwardedType(System.Type type)
		{
			System.Type type1;
			if (!this.TypeForwardingEnabled)
			{
				return null;
			}
			System.Type declaringType = type;
			while (declaringType.IsNested)
			{
				declaringType = declaringType.DeclaringType;
			}
			System.Type type2 = typeof(TypeForwardedFromAttribute);
			using (IEnumerator<CustomAttributeData> enumerator = declaringType.GetCustomAttributesData().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CustomAttributeData current = enumerator.Current;
					if (!current.Constructor.DeclaringType.Equals(type2))
					{
						continue;
					}
					string value = current.ConstructorArguments[0].Value as string;
					if (string.IsNullOrEmpty(value))
					{
						continue;
					}
					Assembly assembly = this.platformReferenceContext.ResolveReferenceAssembly(new AssemblyName(value));
					if (assembly == null)
					{
						type1 = null;
						return type1;
					}
					else
					{
						type1 = assembly.GetType(type.FullName);
						return type1;
					}
				}
				return null;
			}
			return type1;
		}

		public static System.Type GetHandlerType(EventInfo eventInfo)
		{
			System.Type eventHandlerType;
			try
			{
				eventHandlerType = eventInfo.EventHandlerType;
			}
			catch (Exception exception)
			{
				eventHandlerType = null;
			}
			return eventHandlerType;
		}

		public static System.Type GetHandlerType(System.Windows.RoutedEvent routedEvent)
		{
			System.Type handlerType;
			try
			{
				handlerType = routedEvent.HandlerType;
			}
			catch (Exception exception)
			{
				handlerType = null;
			}
			return handlerType;
		}

		public abstract bool GetIsTypeItsOwnNameScope(ITypeId type);

		public abstract ISupportInitialize GetISupportInitialize(object target);

		public virtual IProperty GetNameProperty(ITypeResolver typeResolver, System.Type type)
		{
			IProperty property = null;
			if (!typeof(IDesignTimePropertyImplementor).IsAssignableFrom(type))
			{
				RuntimeNamePropertyAttribute attribute = TypeUtilities.GetAttribute<RuntimeNamePropertyAttribute>(type);
				if (attribute != null && !string.IsNullOrEmpty(attribute.Name))
				{
					property = ((PlatformTypes)typeResolver.PlatformMetadata).GetProperty(typeResolver, type, MemberType.LocalProperty, attribute.Name) as ReferenceStep;
				}
			}
			else
			{
				property = typeResolver.PlatformMetadata.ResolveProperty(typeResolver.PlatformMetadata.KnownProperties.DesignTimeXName);
			}
			return property;
		}

		public virtual System.Type GetNearestSupportedType(System.Type type)
		{
			return type;
		}

		protected ReadOnlyCollection<IAssembly> GetPlatformAssemblies()
		{
			List<IAssembly> assemblies = new List<IAssembly>();
			foreach (IAssembly defaultAssemblyReference in this.DefaultAssemblyReferences)
			{
				if (!defaultAssemblyReference.IsLoaded)
				{
					continue;
				}
				assemblies.Add(defaultAssemblyReference);
			}
			return new ReadOnlyCollection<IAssembly>(assemblies);
		}

		public IAssembly GetPlatformAssembly(Assembly assembly)
		{
			IAssembly assembly1;
			using (IEnumerator<IAssembly> enumerator = this.DefaultAssemblyReferences.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IAssembly current = enumerator.Current;
					if (!current.CompareTo(assembly))
					{
						continue;
					}
					assembly1 = current;
					return assembly1;
				}
				return this.GetPlatformAssembly(AssemblyHelper.GetAssemblyName(assembly));
			}
			return assembly1;
		}

		private IAssembly GetPlatformAssembly(AssemblyName assemblyName)
		{
			IAssembly runtimeAssembly = null;
			if (assemblyName != null && !this.platformAssemblyTable.TryGetValue(assemblyName, out runtimeAssembly) && this.platformRuntimeContext != null)
			{
				Assembly assembly = this.platformRuntimeContext.ResolveRuntimeAssembly(assemblyName);
				if (assembly != null)
				{
					runtimeAssembly = new Microsoft.Expression.DesignModel.Metadata.RuntimeAssembly(assembly, AssemblySource.Platform, false);
				}
				this.platformAssemblyTable[assemblyName] = runtimeAssembly;
			}
			return runtimeAssembly;
		}

		public IAssembly GetPlatformAssembly(string assemblyName)
		{
			IAssembly assembly;
			using (IEnumerator<IAssembly> enumerator = this.DefaultAssemblyReferences.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IAssembly current = enumerator.Current;
					if (current.Name != assemblyName)
					{
						continue;
					}
					assembly = current;
					return assembly;
				}
				return null;
			}
			return assembly;
		}

		private IAssembly GetPlatformAssembly(System.Type type)
		{
			Assembly assembly = type.Assembly;
			IAssembly platformAssemblyUsingAssemblyName = this.GetPlatformAssemblyUsingAssemblyName(assembly);
			if (platformAssemblyUsingAssemblyName == null && PlatformTypes.IsExpressionInteractiveType(type))
			{
				platformAssemblyUsingAssemblyName = this.GetDesignToolAssembly(assembly);
			}
			return platformAssemblyUsingAssemblyName;
		}

		protected abstract ReadOnlyCollection<IAssembly> GetPlatformAssemblyReferences();

		public IAssembly GetPlatformAssemblyUsingAssemblyName(Assembly assembly)
		{
			AssemblyName assemblyName = AssemblyHelper.GetAssemblyName(assembly);
			IAssembly platformAssembly = this.GetPlatformAssembly(assemblyName.Name);
			if (platformAssembly != null && !platformAssembly.IsLoaded)
			{
				((Microsoft.Expression.DesignModel.Metadata.RuntimeAssembly)platformAssembly).ReflectionAssembly = assembly;
			}
			if (platformAssembly == null)
			{
				platformAssembly = this.GetPlatformAssembly(assemblyName);
			}
			return platformAssembly;
		}

		public IAssembly GetPlatformAssemblyUsingAssemblyName(IAssembly assembly)
		{
			Assembly reflectionAssembly = AssemblyHelper.GetReflectionAssembly(assembly);
			if (reflectionAssembly == null)
			{
				return null;
			}
			return this.GetPlatformAssemblyUsingAssemblyName(reflectionAssembly);
		}

		public object GetPlatformCache(string cacheName)
		{
			object obj;
			lock (this.platformCachesSyncLock)
			{
				object obj1 = null;
				this.platformCaches.TryGetValue(cacheName, out obj1);
				obj = obj1;
			}
			return obj;
		}

		public IType GetPlatformType(string typeName)
		{
			return this.GetPlatformType(typeName, null);
		}

		private IType GetPlatformType(string typeName, ITypeId typeId)
		{
			IType type = null;
			System.Type type1 = null;
			if (!this.typeNameToType.TryGetValue(typeName, out type))
			{
				using (IEnumerator<IAssembly> enumerator = this.DefaultAssemblies.GetEnumerator())
				{
					do
					{
						if (!enumerator.MoveNext())
						{
							break;
						}
						type1 = PlatformTypeHelper.GetType(enumerator.Current, typeName);
					}
					while (type1 == null);
				}
				if (type1 == null && typeName.StartsWith("Microsoft.Expression", StringComparison.Ordinal))
				{
					using (IEnumerator<Assembly> enumerator1 = PlatformTypes.designToolAssemblies.GetEnumerator())
					{
						do
						{
							if (!enumerator1.MoveNext())
							{
								break;
							}
							type1 = PlatformTypeHelper.GetType(enumerator1.Current, typeName);
						}
						while (type1 == null);
					}
				}
				if (type1 == null)
				{
					ProjectNeutralTypeId projectNeutralTypeId = typeId as ProjectNeutralTypeId;
					if (projectNeutralTypeId != null)
					{
						IEnumerable<IAssemblyId> assemblyGroup = this.GetAssemblyGroup(projectNeutralTypeId.AssemblyGroup);
						if (assemblyGroup != null)
						{
							using (IEnumerator<IAssemblyId> enumerator2 = assemblyGroup.GetEnumerator())
							{
								do
								{
								Label0:
									if (!enumerator2.MoveNext())
									{
										break;
									}
									IAssemblyId current = enumerator2.Current;
									byte[] publicKeyToken = current.GetPublicKeyToken();
									if (publicKeyToken != null && (int)publicKeyToken.Length > 0)
									{
										AssemblyName assemblyName = new AssemblyName()
										{
											Name = current.Name
										};
										assemblyName.SetPublicKeyToken(current.GetPublicKeyToken());
										IAssembly platformAssembly = this.GetPlatformAssembly(assemblyName);
										if (platformAssembly != null)
										{
											type1 = PlatformTypeHelper.GetType(platformAssembly, typeName);
										}
										else
										{
											goto Label0;
										}
									}
									else
									{
										goto Label0;
									}
								}
								while (type1 == null);
							}
						}
					}
				}
				if (type1 != null)
				{
					type = this.GetType(type1);
				}
				if (type == null)
				{
					type = PlatformTypes.NullTypeInstance;
				}
				this.typeNameToType.Add(typeName, type);
			}
			return type;
		}

		protected virtual IType GetPlatformWorkaroundType(ITypeId neutralType)
		{
			return null;
		}

		public IProperty GetProperty(ITypeResolver typeResolver, System.Type targetType, MemberType memberTypes, string memberName)
		{
			ITypeId type = typeResolver.GetType(targetType);
			if (type == null)
			{
				return null;
			}
			return PlatformTypeHelper.GetProperty(typeResolver, type, memberTypes, memberName);
		}

		public abstract PropertyImplementationBase GetPropertyImplementation(System.Type type, string propertyName, ClrPropertyImplementationBase clrImplementation, MemberAccessTypes access);

		public abstract IEnumerable<IProperty> GetProxyProperties(ITypeResolver typeResolver);

		public abstract IEnumerable<ITypeId> GetProxyTypes(ITypeResolver typeResolver);

		internal abstract RoutedEventDescription[] GetRoutedEventDescriptions(System.Type type);

		private static string GetSdkAssemblyFullNameForVersion(PlatformTypes.BlendSdkAssemblyIdentifier assemblyIdentifier, FrameworkName framework, bool fullSigned)
		{
			PlatformTypes.BlendSdkDescriptor blendSdkDescriptor = null;
			if (!PlatformTypes.SdkDescriptors.TryGetValue(framework, out blendSdkDescriptor))
			{
				return null;
			}
			return blendSdkDescriptor.GetFullName(assemblyIdentifier, fullSigned);
		}

		public abstract System.Type GetStylePropertyTargetType(System.Type type, IPropertyId propertyKey);

		public virtual IType GetType(System.Type type)
		{
			IType type1;
			PlatformTypes.ExternalType externalType;
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (this.types.TryGetValue(type, out type1))
			{
				return type1;
			}
			IAssembly platformAssembly = this.GetPlatformAssembly(type);
			if (platformAssembly == null || !this.AllTypeArgumentsInPlatformAssemblies(type))
			{
				if (this.externalTypes.TryGetValue(type, out externalType))
				{
					return externalType;
				}
				return null;
			}
			System.Type type2 = null;
			if (AssemblyHelper.IsPlatformAssembly(platformAssembly) && !this.TryGetReferenceType(type, out type2))
			{
				this.types.Add(type, PlatformTypes.NullTypeInstance);
				return PlatformTypes.NullTypeInstance;
			}
			PlatformType platformType = new PlatformType(this);
			this.types.Add(type, platformType);
			platformType.Initialize(platformAssembly, type, type2);
			return platformType;
		}

		public virtual System.ComponentModel.TypeConverter GetTypeConverter(System.Reflection.MemberInfo memberInfo)
		{
			System.ComponentModel.TypeConverter typeConverter;
			System.Type type = memberInfo as System.Type;
			if (type != null)
			{
				try
				{
					typeConverter = MetadataStore.GetTypeConverter(type);
				}
				catch (Exception exception)
				{
					typeConverter = this.ResolveType(PlatformTypes.Object).TypeConverter;
				}
				return typeConverter;
			}
			Attribute[] customAttributes = this.GetCustomAttributes(memberInfo, typeof(System.ComponentModel.TypeConverterAttribute), false);
			System.ComponentModel.TypeConverter typeConverterFromAttributes = MetadataStore.GetTypeConverterFromAttributes(memberInfo.DeclaringType.Assembly, new AttributeCollection(customAttributes));
			if (typeConverterFromAttributes == null)
			{
				System.Type valueType = TypeUtilities.GetValueType(memberInfo);
				if (valueType != null)
				{
					typeConverterFromAttributes = MetadataStore.GetTypeConverter(valueType);
					if (typeConverterFromAttributes != null && typeConverterFromAttributes.GetType() == typeof(System.ComponentModel.TypeConverter))
					{
						typeConverterFromAttributes = null;
					}
				}
			}
			return typeConverterFromAttributes;
		}

		public abstract void Initialize();

		public bool IsAssignableFrom(ITypeId assignee, ITypeId assigner)
		{
			unsafe
			{
				BitArray bitArrays;
				PlatformType platformType = assignee as PlatformType;
				PlatformType baseType = assigner as PlatformType;
				if (platformType == null || baseType == null)
				{
					if (assignee.Equals(assigner))
					{
						return true;
					}
					if (assignee == PlatformTypes.NullTypeInstance || assigner == PlatformTypes.NullTypeInstance)
					{
						return false;
					}
					System.Type runtimeType = ((IType)assignee).NearestResolvedType.RuntimeType;
					return runtimeType.IsAssignableFrom(((IType)assigner).NearestResolvedType.RuntimeType);
				}
				IPlatformMetadata platformMetadata = platformType.PlatformMetadata;
				IPlatformMetadata platformMetadatum = baseType.PlatformMetadata;
				if (platformType.PlatformMetadata != this)
				{
					PlatformType type = this.GetType(platformType.RuntimeType) as PlatformType;
					if (type == null || !type.Equals(platformType))
					{
						return false;
					}
					platformType = type;
				}
				if (baseType.PlatformMetadata != this)
				{
					PlatformType type1 = this.GetType(baseType.RuntimeType) as PlatformType;
					if (type1 == null || !type1.Equals(baseType))
					{
						return false;
					}
					baseType = type1;
				}
				int uniqueId = (int)baseType.UniqueId;
                if ((ulong)platformType.UniqueId < (ulong)((int)this.isAssignableFromCache.Length) && uniqueId < (int)this.isAssignableFromCache.Length)
				{
					BitArray bitArrays1 = this.isAssignableFromCache[uniqueId];
					bitArrays = bitArrays1;
					if (bitArrays1 != null)
					{
						return bitArrays.Get((int)platformType.UniqueId);
					}
				}
				List<PlatformType> platformTypes = new List<PlatformType>(5);
				System.Type[] interfaces = null;
				try
				{
					interfaces = baseType.RuntimeType.GetInterfaces();
				}
				catch (Exception exception)
				{
					interfaces = System.Type.EmptyTypes;
				}
				System.Type[] typeArray = interfaces;
				for (int i = 0; i < (int)typeArray.Length; i++)
				{
					PlatformType platformType1 = this.GetType(typeArray[i]) as PlatformType;
					if (platformType1 != null)
					{
						platformTypes.Add(platformType1);
					}
				}
				while (baseType != null)
				{
					platformTypes.Add(baseType);
					baseType = baseType.BaseType as PlatformType;
				}
				platformTypes.Add((PlatformType)this.ResolveType(PlatformTypes.Object));
                if ((ulong)this.PlatformTypeUniqueId >= (ulong)((int)this.isAssignableFromCache.Length))
				{
					this.isAssignableFromCache = new BitArray[this.PlatformTypeUniqueId * 3 / 2];
				}
				bitArrays = new BitArray((int)this.isAssignableFromCache.Length);
				this.isAssignableFromCache[uniqueId] = bitArrays;
				for (int j = 0; j < platformTypes.Count; j++)
				{
					bitArrays.Set((int)platformTypes[j].UniqueId, true);
				}
				return bitArrays.Get((int)platformType.UniqueId);
			}
		}

		public abstract bool IsBinding(System.Type type);

		public bool IsCapabilitySet(PlatformCapability capability)
		{
			object obj = this.platformCapabilities[(int)capability];
			if (!(obj is bool))
			{
				return false;
			}
			return (bool)obj;
		}

		public static bool IsDesignTimeType(System.Type type)
		{
			string @namespace = type.Namespace;
			if (string.Equals(@namespace, PlatformTypes.XData.Namespace, StringComparison.Ordinal) && type.Name == "XData")
			{
				return true;
			}
			return string.Equals(@namespace, PlatformTypes.Annotation.Namespace, StringComparison.Ordinal);
		}

		public bool IsDesignToolAssembly(IAssembly assembly)
		{
			Assembly reflectionAssembly = AssemblyHelper.GetReflectionAssembly(assembly);
			if (reflectionAssembly == null)
			{
				return false;
			}
			return PlatformTypes.designToolAssemblies.Contains(reflectionAssembly);
		}

		public bool IsDesignToolType(System.Type type)
		{
			if (PlatformTypes.IsExpressionInteractiveType(type))
			{
				return true;
			}
			return RuntimeGeneratedTypesHelper.BlendAssemblies.Contains<IAssembly>(this.GetAssembly(type.Assembly, AssemblySource.BlendExtension));
		}

		public static bool IsEffectType(ITypeId type)
		{
			return PlatformTypes.Effect.IsAssignableFrom(type);
		}

		public abstract bool IsExpression(System.Type type);

		public static bool IsExpressionInteractiveType(System.Type type)
		{
			bool flag;
			string @namespace;
			if (type != null)
			{
				@namespace = type.Namespace;
			}
			else
			{
				@namespace = null;
			}
			string str = @namespace;
			if (str != null && str.StartsWith("Microsoft.", StringComparison.Ordinal))
			{
				Assembly assembly = type.Assembly;
				foreach (Assembly designToolAssembly in PlatformTypes.designToolAssemblies)
				{
					if (!assembly.Equals(designToolAssembly))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				string name = AssemblyHelper.GetAssemblyName(assembly).Name;
				using (IEnumerator<Assembly> enumerator = PlatformTypes.designToolAssemblies.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Assembly current = enumerator.Current;
						if (!name.Equals(AssemblyHelper.GetAssemblyName(current).Name, StringComparison.Ordinal))
						{
							continue;
						}
						flag = true;
						return flag;
					}
					return false;
				}
				return flag;
			}
			return false;
		}

		public static bool IsGenericTypeDefinitionOf(System.Type baseDefinition, System.Type targetType)
		{
			while (targetType != null)
			{
				System.Type genericTypeDefinition = PlatformTypeHelper.GetGenericTypeDefinition(targetType);
				if (genericTypeDefinition != null && baseDefinition.IsAssignableFrom(genericTypeDefinition))
				{
					return true;
				}
				targetType = targetType.BaseType;
			}
			return false;
		}

		public static bool IsInstance(object value, ITypeId type, ITypeResolver typeResolver)
		{
			if (value == null)
			{
				return false;
			}
			return type.IsAssignableFrom(typeResolver.GetType(value.GetType()));
		}

		public abstract bool IsLooselySupported(ITypeResolver typeResolver, ITypeId type);

		public bool IsNullType(ITypeId typeId)
		{
			if (typeId == null)
			{
				return true;
			}
			return typeId == PlatformTypes.NullTypeInstance;
		}

		public static bool IsOneOf(System.Type[] types, System.Type type)
		{
			System.Type[] typeArray = types;
			for (int i = 0; i < (int)typeArray.Length; i++)
			{
				if (typeArray[i].IsAssignableFrom(type))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsPlatformInternal(System.Reflection.MemberInfo memberInfo)
		{
			System.Type type = memberInfo as System.Type ?? memberInfo.DeclaringType;
			if (type != null)
			{
				IType type1 = this.GetType(type);
				if (type1 != null)
				{
					MemberAccessType access = type1.Access;
					if (this.IsNullType(type1))
					{
						access = PlatformTypeHelper.GetMemberAccess(type);
					}
					if (access != MemberAccessType.Public)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool IsPlatformType(ITypeId type)
		{
			if (type is PlatformNeutralTypeId)
			{
				return true;
			}
			return type is PlatformType;
		}

		public abstract bool IsResource(System.Type type);

		public virtual bool IsSupported(ITypeResolver typeResolver, ITypeId type)
		{
			IType type1 = typeResolver.ResolveType(type);
			if (type1 == null || type1 == PlatformTypes.NullTypeInstance)
			{
				return false;
			}
			return type1.PlatformMetadata == this;
		}

		public virtual bool IsTrimSurroundingWhitespace(IType typeId)
		{
			return false;
		}

		public static bool IsVisualStateManagerType(ITypeId typeId)
		{
			if (ProjectNeutralTypes.VisualStateManager.Equals(typeId) || ProjectNeutralTypes.VisualStateGroup.Equals(typeId) || ProjectNeutralTypes.VisualState.Equals(typeId))
			{
				return true;
			}
			return ProjectNeutralTypes.VisualTransition.Equals(typeId);
		}

		public abstract object MakePropertyPath(string path, params object[] parameters);

		protected IType NewExternalType(PlatformTypes platformTypes, IPlatformTypes sourcePlatformTypes, IAssembly assembly, System.Type type, ITypeId baseType, ITypeId neutralType, IXmlNamespace xmlNamespace, string clrNamespace, params PlatformTypes.ProxyPropertyDescription[] properties)
		{
			PlatformTypes.ExternalType externalType = new PlatformTypes.ExternalType(platformTypes, sourcePlatformTypes, assembly, type, this.ResolveType(baseType), neutralType, xmlNamespace, clrNamespace);
			this.externalTypes.Add(type, externalType);
			externalType.AddProperties(properties);
			return externalType;
		}

		protected IEvent NewProxyClrEvent(IType declaringType, string name, IType handlerType, MemberAccessType access)
		{
			return new Event(declaringType, handlerType, new ProxyEventImplementation(name, declaringType.RuntimeType, handlerType.RuntimeType, access));
		}

		protected ClrPropertyReferenceStep NewProxyClrProperty(IType declaringType, string name, ITypeId valueType, object defaultValue, string constructorArgument)
		{
			return new ClrPropertyReferenceStep(declaringType, name, this.ResolveType(valueType), new ProxyClrPropertyImplementation(this, this.ResolveType(declaringType).RuntimeType, name, this.ResolveType(declaringType).RuntimeType, this.ResolveType(valueType).RuntimeType, defaultValue, constructorArgument), PropertySortValue.NoValue);
		}

		protected ITypeId NewProxyTypeId(IAssembly assembly, string clrNamespace, string typeName, IType baseType, IXmlNamespace xmlNamespace, params PlatformTypes.ProxyPropertyDescription[] properties)
		{
			return new PlatformTypes.ProxyType(this, assembly, clrNamespace, typeName, baseType, xmlNamespace, properties);
		}

		public static bool PlatformsCompatible(IPlatformMetadata sourcePlatformMetadata, IPlatformMetadata targetPlatformMetadata)
		{
			if (sourcePlatformMetadata != targetPlatformMetadata && !(sourcePlatformMetadata.TargetFramework == targetPlatformMetadata.TargetFramework))
			{
				return false;
			}
			return true;
		}

		public abstract void RefreshAssemblies(ITypeResolver typeResolver, IEnumerable<Assembly> designTimeAssemblies);

		public virtual void RefreshProjectSpecificMetadata(ITypeResolver typeResolver, ITypeMetadataFactory typeMetadataFactory)
		{
		}

		public abstract void RegisterAssembly(Assembly assembly);

		protected abstract void RegisterTypeMetadata(ITypeMetadataFactory value);

		private System.Type ResolveGenericReferenceType(System.Type runtimeType, System.Type targetType)
		{
			System.Type genericArguments;
			System.Type type;
			if (runtimeType.IsGenericTypeDefinition == targetType.IsGenericTypeDefinition)
			{
				return targetType;
			}
			if (runtimeType.IsGenericTypeDefinition)
			{
				return targetType.GetGenericTypeDefinition();
			}
			System.Type[] typeArray = runtimeType.GetGenericArguments();
			System.Type[] typeArray1 = new System.Type[(int)typeArray.Length];
			for (int i = 0; i < (int)typeArray.Length; i++)
			{
				System.Type type1 = typeArray[i];
				if (PlatformTypes.IsExpressionInteractiveType(type1))
				{
					return null;
				}
				if (type1.IsGenericParameter)
				{
					if (!this.TryGetReferenceType(type1.DeclaringType, out type))
					{
						return null;
					}
					if (type == null)
					{
						return null;
					}
					genericArguments = type.GetGenericArguments()[type1.GenericParameterPosition];
				}
				else if (!this.TryGetReferenceType(type1, out genericArguments))
				{
					return null;
				}
				if (genericArguments == null)
				{
					return null;
				}
				typeArray1[i] = genericArguments;
			}
			return targetType.MakeGenericType(typeArray1);
		}

		public IMemberId ResolveMember(IMemberId member)
		{
			PlatformNeutralPropertyId platformNeutralPropertyId = member as PlatformNeutralPropertyId;
			if (platformNeutralPropertyId == null)
			{
				return member;
			}
			IMemberId memberId = null;
			if (this.memberCache.TryGetValue(platformNeutralPropertyId, out memberId))
			{
				return memberId;
			}
			ITypeId typeId = this.ResolveType(platformNeutralPropertyId.DeclaringTypeId);
			if (typeId != null)
			{
				memberId = typeId.GetMember(platformNeutralPropertyId.MemberType, platformNeutralPropertyId.Name, platformNeutralPropertyId.MemberAccessTypes);
			}
			else
			{
				memberId = null;
			}
			this.memberCache.Add(platformNeutralPropertyId, memberId);
			return memberId;
		}

		public IProperty ResolveProperty(IPropertyId property)
		{
			IProperty property1;
			if (property == null)
			{
				return null;
			}
			property1 = (property.MemberType != MemberType.DesignTimeProperty ? this.ResolveMember(property) as IProperty : Microsoft.Expression.DesignModel.Metadata.DesignTimeProperties.ResolveDesignTimePropertyKey(property, this));
			return property1;
		}

		public PropertyReference ResolvePropertyReference(PropertyReference reference)
		{
			return reference;
		}

		public IType ResolveType(ITypeId typeId)
		{
			unsafe
			{
				FullNameTypeId fullNameTypeId = typeId as FullNameTypeId;
				if (fullNameTypeId == null)
				{
					return (IType)typeId;
				}
				IType designTimeType = this.typeCache[fullNameTypeId.UniqueId];
				if (designTimeType != null)
				{
					return designTimeType;
				}
				designTimeType = this.GetDesignTimeType(fullNameTypeId) ?? this.GetPlatformType(fullNameTypeId.FullName, typeId);
				if (designTimeType == PlatformTypes.NullTypeInstance)
				{
					IType platformWorkaroundType = this.GetPlatformWorkaroundType(fullNameTypeId);
					if (platformWorkaroundType != null)
					{
						designTimeType = platformWorkaroundType;
					}
				}
				if (designTimeType == PlatformTypes.NullTypeInstance)
				{
					foreach (PlatformTypes.ExternalType value in this.externalTypes.Values)
					{
						if (value.NeutralType != fullNameTypeId)
						{
							continue;
						}
						designTimeType = value;
						break;
					}
				}
				this.typeCache[fullNameTypeId.UniqueId] = designTimeType;
				return designTimeType;
			}
		}

		protected void SetCapabilityValue(PlatformCapability capability, object value)
		{
			this.platformCapabilities[(int)capability] = value;
		}

		public void SetPlatformCache(string cacheName, object value)
		{
			lock (this.platformCachesSyncLock)
			{
				this.platformCaches[cacheName] = value;
			}
		}

		private bool TryGetReferenceType(System.Type runtimeType, out System.Type referenceType)
		{
			referenceType = null;
			if (this.platformReferenceContext == null || this.platformReferenceContext is DefaultPlatformReferenceContext)
			{
				return true;
			}
			System.Type genericTypeDefinition = runtimeType;
			if (runtimeType.IsGenericType && !runtimeType.IsGenericTypeDefinition)
			{
				genericTypeDefinition = runtimeType.GetGenericTypeDefinition();
			}
			if (genericTypeDefinition.FullName == null)
			{
				return true;
			}
			Assembly assembly = this.platformReferenceContext.ResolveReferenceAssembly(genericTypeDefinition.Assembly);
			if (assembly == null)
			{
				referenceType = this.GetForwardedType(genericTypeDefinition);
				if (referenceType != null && runtimeType.IsGenericType)
				{
					referenceType = this.ResolveGenericReferenceType(runtimeType, referenceType);
				}
				return true;
			}
			System.Type type = assembly.GetType(genericTypeDefinition.FullName);
			if (type == null)
			{
				type = this.GetForwardedType(genericTypeDefinition);
				if (type == null)
				{
					return false;
				}
			}
			referenceType = (!runtimeType.IsGenericType ? type : this.ResolveGenericReferenceType(runtimeType, type));
			return true;
		}

		protected IAssembly TryLoadInteractivityAssembly(IPlatformReferenceContext platformReferenceContext)
		{
			IAssembly assembly = null;
			bool flag = true;
			try
			{
				string sdkAssemblyFullNameForVersion = PlatformTypes.GetSdkAssemblyFullNameForVersion(PlatformTypes.BlendSdkAssemblyIdentifier.Interactivity, platformReferenceContext.TargetFramework, flag);
				if (!string.IsNullOrEmpty(sdkAssemblyFullNameForVersion))
				{
					assembly = this.GetAssembly(Assembly.Load(sdkAssemblyFullNameForVersion), AssemblySource.BlendExtension);
					this.InteractivityAssemblyFullName = sdkAssemblyFullNameForVersion;
					this.InteractionsAssemblyFullName = PlatformTypes.GetSdkAssemblyFullNameForVersion(PlatformTypes.BlendSdkAssemblyIdentifier.Interactions, platformReferenceContext.TargetFramework, flag);
				}
			}
			catch (IOException oException1)
			{
				try
				{
					string str = PlatformTypes.GetSdkAssemblyFullNameForVersion(PlatformTypes.BlendSdkAssemblyIdentifier.Interactivity, platformReferenceContext.TargetFramework, !flag);
					if (!string.IsNullOrEmpty(str))
					{
						assembly = this.GetAssembly(Assembly.Load(str), AssemblySource.BlendExtension);
						this.InteractivityAssemblyFullName = str;
						this.InteractionsAssemblyFullName = PlatformTypes.GetSdkAssemblyFullNameForVersion(PlatformTypes.BlendSdkAssemblyIdentifier.Interactions, platformReferenceContext.TargetFramework, !flag);
					}
				}
				catch (IOException oException)
				{
				}
				catch (BadImageFormatException badImageFormatException)
				{
				}
			}
			catch (BadImageFormatException badImageFormatException1)
			{
			}
			return assembly;
		}

		private class AssemblyNameComparer : IEqualityComparer<AssemblyName>
		{
			public AssemblyNameComparer()
			{
			}

			public bool Equals(AssemblyName assemblyName1, AssemblyName assemblyName2)
			{
				return AssemblyName.ReferenceMatchesDefinition(assemblyName1, assemblyName2);
			}

			public int GetHashCode(AssemblyName assemblyName)
			{
				return assemblyName.Name.GetHashCode();
			}
		}

		protected enum BlendSdkAssemblyIdentifier
		{
			Interactivity,
			Interactions
		}

		private class BlendSdkDescriptor
		{
			private const string SdkAssemblyFullNameFormatString = "{0}, Version={1}, Culture=neutral, PublicKeyToken={2}";

			private string signedKey;

			private string unsignedKey;

			public static IEqualityComparer<FrameworkName> EqualityComparerInstance
			{
				get
				{
					return PlatformTypes.BlendSdkDescriptor.FrameworkNameEqualityComparer.Instance;
				}
			}

			public string VersionString
			{
				get;
				private set;
			}

			public BlendSdkDescriptor(Version version, string signedKey, string unsignedKey)
			{
				this.VersionString = version.ToString();
				this.signedKey = signedKey;
				this.unsignedKey = unsignedKey;
			}

			public string GetAssemblyName(PlatformTypes.BlendSdkAssemblyIdentifier assembly)
			{
				string str = null;
				switch (assembly)
				{
					case PlatformTypes.BlendSdkAssemblyIdentifier.Interactivity:
					{
						str = "System.Windows.Interactivity";
						break;
					}
					case PlatformTypes.BlendSdkAssemblyIdentifier.Interactions:
					{
						str = "Microsoft.Expression.Interactions";
						break;
					}
					default:
					{
						str = "System.Windows.Interactivity";
						break;
					}
				}
				return str;
			}

			public string GetFullName(PlatformTypes.BlendSdkAssemblyIdentifier assemblyIdentifier, bool fullSigned)
			{
				string assemblyName = this.GetAssemblyName(assemblyIdentifier);
				string versionString = this.VersionString;
				string publicKeyToken = this.GetPublicKeyToken(fullSigned);
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				object[] objArray = new object[] { assemblyName, versionString, publicKeyToken };
				return string.Format(currentCulture, "{0}, Version={1}, Culture=neutral, PublicKeyToken={2}", objArray);
			}

			public string GetPublicKeyToken(bool fullSigned)
			{
				if (!fullSigned)
				{
					return this.unsignedKey;
				}
				return this.signedKey;
			}

			private class FrameworkNameEqualityComparer : IEqualityComparer<FrameworkName>
			{
				private static PlatformTypes.BlendSdkDescriptor.FrameworkNameEqualityComparer instance;

				public static PlatformTypes.BlendSdkDescriptor.FrameworkNameEqualityComparer Instance
				{
					get
					{
						if (PlatformTypes.BlendSdkDescriptor.FrameworkNameEqualityComparer.instance == null)
						{
							PlatformTypes.BlendSdkDescriptor.FrameworkNameEqualityComparer.instance = new PlatformTypes.BlendSdkDescriptor.FrameworkNameEqualityComparer();
						}
						return PlatformTypes.BlendSdkDescriptor.FrameworkNameEqualityComparer.instance;
					}
				}

				private FrameworkNameEqualityComparer()
				{
				}

				private static bool CompareIdentifiers(string identifier1, string identifier2)
				{
					return string.Equals(identifier1, identifier2, StringComparison.Ordinal);
				}

				private static bool CompareProfiles(string profile1, string profile2)
				{
					if (PlatformTypes.BlendSdkDescriptor.FrameworkNameEqualityComparer.IsDefaultOrClientProfile(profile1))
					{
						return PlatformTypes.BlendSdkDescriptor.FrameworkNameEqualityComparer.IsDefaultOrClientProfile(profile2);
					}
					return string.Equals(profile1, profile2, StringComparison.Ordinal);
				}

				private static bool CompareVersions(Version version1, Version version2)
				{
					return version1.Equals(version2);
				}

				public bool Equals(FrameworkName x, FrameworkName y)
				{
					if (object.ReferenceEquals(x, null))
					{
						return object.ReferenceEquals(y, null);
					}
					if (object.ReferenceEquals(y, null))
					{
						return false;
					}
					if (!PlatformTypes.BlendSdkDescriptor.FrameworkNameEqualityComparer.CompareIdentifiers(x.Identifier, y.Identifier) || !PlatformTypes.BlendSdkDescriptor.FrameworkNameEqualityComparer.CompareVersions(x.Version, y.Version))
					{
						return false;
					}
					return PlatformTypes.BlendSdkDescriptor.FrameworkNameEqualityComparer.CompareProfiles(x.Profile, y.Profile);
				}

				public int GetHashCode(FrameworkName obj)
				{
					int hashCode = obj.Identifier.GetHashCode() ^ obj.Version.GetHashCode();
					if (!PlatformTypes.BlendSdkDescriptor.FrameworkNameEqualityComparer.IsDefaultOrClientProfile(obj.Profile))
					{
						hashCode = hashCode ^ obj.Profile.GetHashCode();
					}
					return hashCode;
				}

				private static bool IsDefaultOrClientProfile(string profile)
				{
					if (string.IsNullOrEmpty(profile))
					{
						return true;
					}
					return string.Equals(profile, "Client", StringComparison.Ordinal);
				}
			}
		}

		private sealed class ExternalType : PlatformTypes.ProxyType
		{
			private readonly IPlatformTypes sourcePlatformTypes;

			private readonly System.Type type;

			private readonly ITypeId neutralType;

			private IList<IConstructor> constructors;

			private IConstructorArgumentProperties constructorArgumentProperties;

			private DelayedInstance<ITypeMetadata> metadata;

			public override bool IsBinding
			{
				get
				{
					return this.sourcePlatformTypes.IsBinding(this.type);
				}
			}

			public override bool IsExpression
			{
				get
				{
					return this.sourcePlatformTypes.IsExpression(this.type);
				}
			}

			public override bool IsResource
			{
				get
				{
					return this.sourcePlatformTypes.IsResource(this.type);
				}
			}

			public override ITypeMetadata Metadata
			{
				get
				{
					return this.metadata.Value;
				}
			}

			public override IType NearestResolvedType
			{
				get
				{
					return this;
				}
			}

			public ITypeId NeutralType
			{
				get
				{
					return this.neutralType;
				}
			}

			public override System.Type ReflectionType
			{
				get
				{
					return this.type;
				}
			}

			public override System.Type RuntimeType
			{
				get
				{
					return this.type;
				}
			}

			public ExternalType(PlatformTypes platformTypes, IPlatformTypes sourcePlatformTypes, IAssembly assembly, System.Type type, IType baseType, ITypeId neutralType, IXmlNamespace xmlNamespace, string clrNamespace) : base(platformTypes, assembly, clrNamespace, type.Name, baseType, xmlNamespace, new PlatformTypes.ProxyPropertyDescription[0])
			{
				PlatformTypes.ExternalType externalType = this;
				this.neutralType = neutralType;
				this.metadata = new DelayedInstance<ITypeMetadata>(() => platformTypes.TypeMetadataFactory.GetMetadata(externalType.type));
				this.sourcePlatformTypes = sourcePlatformTypes;
				this.type = type;
			}

			public override IConstructorArgumentProperties GetConstructorArgumentProperties()
			{
				if (this.constructorArgumentProperties == null)
				{
					this.constructorArgumentProperties = PlatformTypeHelper.GetConstructorArgumentProperties(this);
				}
				return this.constructorArgumentProperties;
			}

			public override IList<IConstructor> GetConstructors()
			{
				if (this.constructors == null)
				{
					this.constructors = PlatformTypeHelper.GetConstructors(new PlatformTypes.ExternalType.CorrespondingTypeResolver(this.sourcePlatformTypes.DefaultTypeResolver, base.PlatformMetadata), this);
				}
				return this.constructors;
			}

			public override bool HasDefaultConstructor(bool supportInternal)
			{
				return TypeUtilities.HasDefaultConstructor(this.type, supportInternal);
			}

			public override void InitializeClass()
			{
				RuntimeHelpers.RunClassConstructor(this.type.TypeHandle);
			}

			private class CorrespondingTypeResolver : ITypeResolver, IMetadataResolver
			{
				private IPlatformMetadata targetPlatform;

				private ITypeResolver sourcePlatformTypeResolver;

				public ICollection<IAssembly> AssemblyReferences
				{
					get
					{
						return this.sourcePlatformTypeResolver.AssemblyReferences;
					}
				}

				public IPlatformMetadata PlatformMetadata
				{
					get
					{
						return this.sourcePlatformTypeResolver.PlatformMetadata;
					}
				}

				public IAssembly ProjectAssembly
				{
					get
					{
						return this.sourcePlatformTypeResolver.ProjectAssembly;
					}
				}

				public IXmlNamespaceTypeResolver ProjectNamespaces
				{
					get
					{
						return this.sourcePlatformTypeResolver.ProjectNamespaces;
					}
				}

				public string ProjectPath
				{
					get
					{
						return this.sourcePlatformTypeResolver.ProjectPath;
					}
				}

				public string RootNamespace
				{
					get
					{
						return this.sourcePlatformTypeResolver.RootNamespace;
					}
				}

				public CorrespondingTypeResolver(ITypeResolver sourcePlatformTypeResolver, IPlatformMetadata targetPlatform)
				{
					this.targetPlatform = targetPlatform;
					this.sourcePlatformTypeResolver = sourcePlatformTypeResolver;
				}

				public bool EnsureAssemblyReferenced(string assemblyPath)
				{
					return this.sourcePlatformTypeResolver.EnsureAssemblyReferenced(assemblyPath);
				}

				public IAssembly GetAssembly(string assemblyName)
				{
					return this.sourcePlatformTypeResolver.GetAssembly(assemblyName);
				}

				public object GetCapabilityValue(PlatformCapability capability)
				{
					return this.sourcePlatformTypeResolver.GetCapabilityValue(capability);
				}

				public IType GetType(System.Type type)
				{
					IType type1 = this.sourcePlatformTypeResolver.GetType(type);
					return PlatformTypeHelper.ConvertTypeId(type1, this.targetPlatform);
				}

				public IType GetType(IXmlNamespace xmlNamespace, string typeName)
				{
					IType type = this.sourcePlatformTypeResolver.GetType(xmlNamespace, typeName);
					return PlatformTypeHelper.ConvertTypeId(type, this.targetPlatform);
				}

				public IType GetType(string assemblyName, string typeName)
				{
					IType type = this.sourcePlatformTypeResolver.GetType(assemblyName, typeName);
					return PlatformTypeHelper.ConvertTypeId(type, this.targetPlatform);
				}

				public IType GetType(IAssembly assembly, string typeName)
				{
					return PlatformTypeHelper.GetType(this, assembly, typeName);
				}

				public bool InTargetAssembly(IType typeId)
				{
					return this.sourcePlatformTypeResolver.InTargetAssembly(typeId);
				}

				public bool IsCapabilitySet(PlatformCapability capability)
				{
					return this.sourcePlatformTypeResolver.IsCapabilitySet(capability);
				}

				public IProperty ResolveProperty(IPropertyId propertyId)
				{
					return this.sourcePlatformTypeResolver.ResolveProperty(propertyId);
				}

				public IType ResolveType(ITypeId typeId)
				{
					return this.sourcePlatformTypeResolver.ResolveType(typeId);
				}

				public event EventHandler<TypesChangedEventArgs> TypesChanged
				{
					add
					{
					}
					remove
					{
					}
				}

				public event EventHandler<TypesChangedEventArgs> TypesChangedEarly
				{
					add
					{
					}
					remove
					{
					}
				}
			}
		}

		private sealed class NullType : IType, IMember, ITypeId, IMemberId
		{
			public MemberAccessType Access
			{
				get
				{
					return MemberAccessType.None;
				}
			}

			public IType BaseType
			{
				get
				{
					return null;
				}
			}

			public IType DeclaringType
			{
				get
				{
					return null;
				}
			}

			public ITypeId DeclaringTypeId
			{
				get
				{
					return null;
				}
			}

			public string FullName
			{
				get
				{
					return null;
				}
			}

			public Exception InitializationException
			{
				get
				{
					return null;
				}
			}

			public bool IsAbstract
			{
				get
				{
					return false;
				}
			}

			public bool IsArray
			{
				get
				{
					return false;
				}
			}

			public bool IsBinding
			{
				get
				{
					return false;
				}
			}

			public bool IsBuilt
			{
				get
				{
					return true;
				}
			}

			public bool IsExpression
			{
				get
				{
					return false;
				}
			}

			public bool IsGenericType
			{
				get
				{
					return false;
				}
			}

			public bool IsInterface
			{
				get
				{
					return false;
				}
			}

			public bool IsResolvable
			{
				get
				{
					return false;
				}
			}

			public bool IsResource
			{
				get
				{
					return false;
				}
			}

			public IType ItemType
			{
				get
				{
					return null;
				}
			}

			public MemberType MemberType
			{
				get
				{
					return MemberType.None;
				}
			}

			public ITypeId MemberTypeId
			{
				get
				{
					return null;
				}
			}

			public ITypeMetadata Metadata
			{
				get
				{
					return null;
				}
			}

			public string Name
			{
				get
				{
					return null;
				}
			}

			public string Namespace
			{
				get
				{
					return null;
				}
			}

			public IType NearestResolvedType
			{
				get
				{
					return null;
				}
			}

			public IType NullableType
			{
				get
				{
					return null;
				}
			}

			public IPlatformMetadata PlatformMetadata
			{
				get
				{
					return null;
				}
			}

			public IAssembly RuntimeAssembly
			{
				get
				{
					return null;
				}
			}

			public System.Type RuntimeType
			{
				get
				{
					return null;
				}
			}

			public bool SupportsNullValues
			{
				get
				{
					return false;
				}
			}

			public System.ComponentModel.TypeConverter TypeConverter
			{
				get
				{
					return null;
				}
			}

			public string UniqueName
			{
				get
				{
					return null;
				}
			}

			public string XamlSourcePath
			{
				get
				{
					return null;
				}
			}

			public IXmlNamespace XmlNamespace
			{
				get
				{
					return null;
				}
			}

			public NullType()
			{
			}

			public IMember Clone(ITypeResolver typeResolver)
			{
				return this;
			}

			public IConstructorArgumentProperties GetConstructorArgumentProperties()
			{
				return null;
			}

			public IList<IConstructor> GetConstructors()
			{
				return null;
			}

			public IEnumerable<IEvent> GetEvents(MemberAccessTypes access)
			{
				return null;
			}

			public IList<IType> GetGenericTypeArguments()
			{
				return null;
			}

			public IMemberId GetMember(MemberType memberTypes, string memberName, MemberAccessTypes access)
			{
				return null;
			}

			public IEnumerable<IProperty> GetProperties(MemberAccessTypes access)
			{
				return null;
			}

			public bool HasDefaultConstructor(bool supportInternal)
			{
				return false;
			}

			public void InitializeClass()
			{
			}

			public bool IsAssignableFrom(ITypeId type)
			{
				return false;
			}

			public bool IsInProject(ITypeResolver typeResolver)
			{
				return false;
			}
		}

		protected struct ProxyPropertyDescription
		{
			private readonly string name;

			private readonly ITypeId valueType;

			private readonly object defaultValue;

			private readonly string constructorArgument;

			public string ConstructorArgument
			{
				get
				{
					return this.constructorArgument;
				}
			}

			public object DefaultValue
			{
				get
				{
					return this.defaultValue;
				}
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public ITypeId ValueType
			{
				get
				{
					return this.valueType;
				}
			}

			public ProxyPropertyDescription(string name, ITypeId valueType, object defaultValue) : this(name, valueType, defaultValue, null)
			{
			}

			public ProxyPropertyDescription(string name, ITypeId valueType, object defaultValue, string constructorArgument)
			{
				this.name = name;
				this.valueType = valueType;
				this.defaultValue = defaultValue;
				this.constructorArgument = constructorArgument;
			}
		}

		private class ProxyType : IType, IMember, ITypeId, IMemberId, IReflectionType
		{
			private readonly PlatformTypes platformTypes;

			private readonly IAssembly assembly;

			private readonly string clrNamespace;

			private readonly string typeName;

			private readonly string fullName;

			private readonly IType baseType;

			private readonly IXmlNamespace xmlNamespace;

			private readonly Dictionary<string, IProperty> properties;

			private readonly int hashCode;

			public MemberAccessType Access
			{
				get
				{
					return MemberAccessType.Public;
				}
			}

			public virtual IType BaseType
			{
				get
				{
					return this.baseType;
				}
			}

			public IType DeclaringType
			{
				get
				{
					return null;
				}
			}

			public ITypeId DeclaringTypeId
			{
				get
				{
					return null;
				}
			}

			public string FullName
			{
				get
				{
					return this.fullName;
				}
			}

			public Exception InitializationException
			{
				get
				{
					return null;
				}
			}

			public bool IsAbstract
			{
				get
				{
					return false;
				}
			}

			public bool IsArray
			{
				get
				{
					return false;
				}
			}

			public virtual bool IsBinding
			{
				get
				{
					return false;
				}
			}

			public bool IsBuilt
			{
				get
				{
					return true;
				}
			}

			public virtual bool IsExpression
			{
				get
				{
					return false;
				}
			}

			public bool IsGenericType
			{
				get
				{
					return false;
				}
			}

			public bool IsInterface
			{
				get
				{
					return false;
				}
			}

			public bool IsResolvable
			{
				get
				{
					return true;
				}
			}

			public virtual bool IsResource
			{
				get
				{
					return false;
				}
			}

			public virtual IType ItemType
			{
				get
				{
					return this.baseType.ItemType;
				}
			}

			public MemberType MemberType
			{
				get
				{
					return MemberType.Type;
				}
			}

			public ITypeId MemberTypeId
			{
				get
				{
					return PlatformTypes.Type;
				}
			}

			public virtual ITypeMetadata Metadata
			{
				get
				{
					return this.baseType.Metadata;
				}
			}

			System.Type Microsoft.Expression.DesignModel.Metadata.IReflectionType.ReflectionType
			{
				get
				{
					return this.ReflectionType;
				}
			}

			public string Name
			{
				get
				{
					return this.typeName;
				}
			}

			public virtual string Namespace
			{
				get
				{
					return this.clrNamespace;
				}
			}

			public virtual IType NearestResolvedType
			{
				get
				{
					return this.baseType.NearestResolvedType;
				}
			}

			public IType NullableType
			{
				get
				{
					return this.baseType.NullableType;
				}
			}

			public IPlatformMetadata PlatformMetadata
			{
				get
				{
					return this.platformTypes;
				}
			}

			public virtual System.Type ReflectionType
			{
				get
				{
					return ((IReflectionType)this.baseType).ReflectionType;
				}
			}

			public virtual IAssembly RuntimeAssembly
			{
				get
				{
					return this.assembly;
				}
			}

			public virtual System.Type RuntimeType
			{
				get
				{
					return null;
				}
			}

			public bool SupportsNullValues
			{
				get
				{
					return this.baseType.SupportsNullValues;
				}
			}

			public virtual System.ComponentModel.TypeConverter TypeConverter
			{
				get
				{
					return this.baseType.TypeConverter;
				}
			}

			public string UniqueName
			{
				get
				{
					return this.Name;
				}
			}

			public string XamlSourcePath
			{
				get
				{
					return null;
				}
			}

			public virtual IXmlNamespace XmlNamespace
			{
				get
				{
					return this.xmlNamespace;
				}
			}

			public ProxyType(PlatformTypes platformTypes, IAssembly assembly, string clrNamespace, string typeName, IType baseType, IXmlNamespace xmlNamespace, params PlatformTypes.ProxyPropertyDescription[] propertyDescriptions)
			{
				this.platformTypes = platformTypes;
				this.assembly = assembly;
				this.clrNamespace = clrNamespace;
				this.typeName = typeName;
				this.fullName = Microsoft.Expression.DesignModel.Metadata.TypeHelper.CombineNamespaceAndTypeName(this.clrNamespace, this.typeName);
				this.baseType = baseType;
				this.xmlNamespace = xmlNamespace;
				this.properties = new Dictionary<string, IProperty>();
				if (propertyDescriptions != null && (int)propertyDescriptions.Length > 0)
				{
					this.AddProperties(propertyDescriptions);
				}
				this.hashCode = this.fullName.GetHashCode();
			}

			public void AddProperties(PlatformTypes.ProxyPropertyDescription[] propertyDescriptions)
			{
				PlatformTypes.ProxyPropertyDescription[] proxyPropertyDescriptionArray = propertyDescriptions;
				for (int i = 0; i < (int)proxyPropertyDescriptionArray.Length; i++)
				{
					PlatformTypes.ProxyPropertyDescription proxyPropertyDescription = proxyPropertyDescriptionArray[i];
					IProperty property = this.platformTypes.NewProxyClrProperty(this, proxyPropertyDescription.Name, proxyPropertyDescription.ValueType, proxyPropertyDescription.DefaultValue, proxyPropertyDescription.ConstructorArgument);
					this.properties.Add(proxyPropertyDescription.Name, property);
				}
			}

			public IMember Clone(ITypeResolver typeResolver)
			{
				if (this.PlatformMetadata == typeResolver.PlatformMetadata)
				{
					return this;
				}
				return typeResolver.GetType(this.XmlNamespace, this.Name);
			}

			public override bool Equals(object obj)
			{
				if (this == obj)
				{
					return true;
				}
				PlatformNeutralTypeId platformNeutralTypeId = obj as PlatformNeutralTypeId;
				if (platformNeutralTypeId == null)
				{
					return false;
				}
				return this.FullName == platformNeutralTypeId.FullName;
			}

			public virtual IConstructorArgumentProperties GetConstructorArgumentProperties()
			{
				return PlatformTypeHelper.EmptyConstructorArgumentProperties;
			}

			public virtual IList<IConstructor> GetConstructors()
			{
				return ReadOnlyCollections<IConstructor>.Empty;
			}

			public IEnumerable<IEvent> GetEvents(MemberAccessTypes access)
			{
				yield break;
			}

			public IList<IType> GetGenericTypeArguments()
			{
				return ReadOnlyCollections<IType>.Empty;
			}

			public override int GetHashCode()
			{
				return this.hashCode;
			}

			public IMemberId GetMember(MemberType memberTypes, string memberName, MemberAccessTypes access)
			{
				IProperty property;
				if (!Microsoft.Expression.DesignModel.Metadata.TypeHelper.IsSet(memberTypes, MemberType.Property) || !this.properties.TryGetValue(memberName, out property))
				{
					return this.baseType.GetMember(memberTypes, memberName, access);
				}
				if (Microsoft.Expression.DesignModel.Metadata.TypeHelper.IsSet(access, property.Access))
				{
					return property;
				}
				return null;
			}

			public IEnumerable<IProperty> GetProperties(MemberAccessTypes access)
			{
				foreach (IProperty value in this.properties.Values)
				{
					if (!Microsoft.Expression.DesignModel.Metadata.TypeHelper.IsSet(access, value.Access))
					{
						continue;
					}
					yield return value;
				}
			}

			public virtual bool HasDefaultConstructor(bool supportInternal)
			{
				return true;
			}

			public virtual void InitializeClass()
			{
				this.baseType.InitializeClass();
			}

			public bool IsAssignableFrom(ITypeId type)
			{
				return this == type;
			}

			public bool IsInProject(ITypeResolver typeResolver)
			{
				return typeResolver.AssemblyReferences.Contains(this.assembly);
			}

			public override string ToString()
			{
				return this.FullName;
			}
		}
	}
}