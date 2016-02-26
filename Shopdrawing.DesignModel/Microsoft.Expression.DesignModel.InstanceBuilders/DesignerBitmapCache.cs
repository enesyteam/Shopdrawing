using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public static class DesignerBitmapCache
	{
		private const int maxCacheSize = 300;

		private static Dictionary<Uri, WeakReference> internalCache;

		public readonly static IPropertyId UriSourceProperty;

		public readonly static IPropertyId CacheOptionProperty;

		static DesignerBitmapCache()
		{
			DesignerBitmapCache.internalCache = new Dictionary<Uri, WeakReference>();
			DesignerBitmapCache.UriSourceProperty = (IPropertyId)PlatformTypes.BitmapImage.GetMember(MemberType.LocalProperty, "UriSource", MemberAccessTypes.Public);
			DesignerBitmapCache.CacheOptionProperty = (IPropertyId)PlatformTypes.BitmapImage.GetMember(MemberType.LocalProperty, "CacheOption", MemberAccessTypes.Public);
		}

		public static BitmapImage GetImageSource(Uri sourceUri, DocumentNode valueNode)
		{
			WeakReference weakReference;
			BitmapImage target;
			bool flag = DesignerBitmapCache.HasMorePropertiesThanJustUri(valueNode);
			bool flag1 = (!sourceUri.IsAbsoluteUri || !sourceUri.IsFile ? false : !flag);
			BitmapImage bitmapImage = null;
			lock (DesignerBitmapCache.internalCache)
			{
				if (!flag1 || !DesignerBitmapCache.internalCache.TryGetValue(sourceUri, out weakReference) || !weakReference.IsAlive)
				{
					if (DesignerBitmapCache.internalCache.Count == 300)
					{
						DesignerBitmapCache.Scavenge();
					}
					if (!flag)
					{
						try
						{
							bitmapImage = new BitmapImage();
							bitmapImage.BeginInit();
							bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
							bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
							bitmapImage.UriSource = sourceUri;
							bitmapImage.EndInit();
						}
						catch (NotSupportedException notSupportedException)
						{
							bitmapImage = new BitmapImage();
							CultureInfo currentCulture = CultureInfo.CurrentCulture;
							string imageIsNotSupported = StringTable.ImageIsNotSupported;
							object[] originalString = new object[] { sourceUri.OriginalString };
							throw new NotSupportedException(string.Format(currentCulture, imageIsNotSupported, originalString));
						}
					}
					if (flag1 && DesignerBitmapCache.internalCache.Count < 300)
					{
						DesignerBitmapCache.internalCache[sourceUri] = new WeakReference(bitmapImage);
					}
					return bitmapImage;
				}
				else
				{
					target = weakReference.Target as BitmapImage;
				}
			}
			return target;
		}

		public static bool HasMorePropertiesThanJustUri(DocumentNode valueNode)
		{
			DocumentCompositeNode documentCompositeNode = valueNode as DocumentCompositeNode;
			if (documentCompositeNode == null)
			{
				return false;
			}
			if (documentCompositeNode.Properties.Count > 1)
			{
				return true;
			}
			IPropertyId uriSourceProperty = DesignerBitmapCache.UriSourceProperty;
			KeyValuePair<IProperty, DocumentNode> keyValuePair = documentCompositeNode.Properties.First<KeyValuePair<IProperty, DocumentNode>>();
			return !uriSourceProperty.Equals(keyValuePair.Key);
		}

		public static void InvalidateUri(Uri sourceUri)
		{
			lock (DesignerBitmapCache.internalCache)
			{
				DesignerBitmapCache.internalCache.Remove(sourceUri);
			}
		}

		private static void Scavenge()
		{
			List<Uri> uris = new List<Uri>();
			foreach (KeyValuePair<Uri, WeakReference> keyValuePair in DesignerBitmapCache.internalCache)
			{
				if (keyValuePair.Value.IsAlive)
				{
					continue;
				}
				uris.Add(keyValuePair.Key);
			}
			foreach (Uri uri in uris)
			{
				DesignerBitmapCache.internalCache.Remove(uri);
			}
		}
	}
}