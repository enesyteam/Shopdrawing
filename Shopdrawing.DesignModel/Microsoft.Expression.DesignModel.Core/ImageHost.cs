using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.ViewObjects;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.DesignModel.Core
{
	public abstract class ImageHost : Border, IDisposable
	{
		public abstract object HitTestRoot
		{
			get;
		}

		public abstract ImageSource InternalSource
		{
			get;
		}

		public abstract bool IsLive
		{
			get;
			protected set;
		}

		public abstract object OverlayLayer
		{
			get;
		}

		public abstract object RenderRoot
		{
			get;
		}

		public abstract object RootInstance
		{
			get;
			set;
		}

		protected ImageHost()
		{
		}

		public abstract void Activate();

		public abstract void AddLiveControl(IViewControl control);

		public abstract void Attach(ImageHost parent);

		public abstract void Deactivate();

		public abstract void Detach(ImageHost parent);

		public abstract void Dispose();

		public abstract Rect GetDocumentBounds(IViewObject content);

		public abstract void Redraw(bool force);

		public abstract void RemoveLiveControl(IViewControl control);

		public abstract void SetTransformMatrix(Matrix matrix, Vector artboardToSilverlightRootScale);

		public abstract void ShutdownVisualTree();

		public abstract void TrySetApplicationResourceDictionary(IInstanceBuilderContext context, IEnumerable<ViewNode> resources);

		public abstract event EventHandler DocumentSizeChanged;
	}
}