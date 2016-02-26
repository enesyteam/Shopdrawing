using Microsoft.Expression.DesignModel.DocumentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignModel.Core
{
	internal static class PointSerializationHelper
	{
		public static void AppendPoint(StringBuilder builder, Point point)
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] x = new object[] { point.X, point.Y };
			builder.AppendFormat(invariantCulture, "{0:G8},{1:G8}", x);
		}

		public static string GetPoint3DCollectionAsString(Point3DCollection collection)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < collection.Count; i++)
			{
				Point3D item = collection[i];
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				object[] x = new object[] { item.X, item.Y, item.Z };
				stringBuilder.AppendFormat(invariantCulture, "{0:G8},{1:G8},{2:G8}", x);
				if (i % 13 == 12)
				{
					stringBuilder.AppendLine();
				}
				else if (i < collection.Count - 1)
				{
					stringBuilder.Append(' ');
				}
			}
			return stringBuilder.ToString();
		}

		public static string GetPointCollectionAsString(PointCollection collection)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < collection.Count; i++)
			{
				PointSerializationHelper.AppendPoint(stringBuilder, collection[i]);
				if (i % 23 == 22)
				{
					stringBuilder.AppendLine();
				}
				else if (i < collection.Count - 1)
				{
					stringBuilder.Append(' ');
				}
			}
			return stringBuilder.ToString();
		}

		public static string GetVector3DCollectionAsString(Vector3DCollection collection)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < collection.Count; i++)
			{
				Vector3D item = collection[i];
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				object[] x = new object[] { item.X, item.Y, item.Z };
				stringBuilder.AppendFormat(invariantCulture, "{0:G8},{1:G8},{2:G8}", x);
				if (i % 13 == 12)
				{
					stringBuilder.AppendLine();
				}
				else if (i < collection.Count - 1)
				{
					stringBuilder.Append(' ');
				}
			}
			return stringBuilder.ToString();
		}

		public static string SerializeDoubleCollectionAsAttribute(DocumentCompositeNode compositeNode)
		{
			string str;
			StringBuilder stringBuilder = new StringBuilder();
			using (IEnumerator<DocumentNode> enumerator = compositeNode.Children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DocumentPrimitiveNode current = enumerator.Current as DocumentPrimitiveNode;
					if (current != null)
					{
						double value = current.GetValue<double>();
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(' ');
						}
						CultureInfo invariantCulture = CultureInfo.InvariantCulture;
						object[] objArray = new object[] { value };
						stringBuilder.AppendFormat(invariantCulture, "{0:G8}", objArray);
					}
					else
					{
						str = null;
						return str;
					}
				}
				return stringBuilder.ToString();
			}
			return str;
		}
	}
}