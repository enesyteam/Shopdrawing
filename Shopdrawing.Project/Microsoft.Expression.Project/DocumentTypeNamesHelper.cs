using System;

namespace Microsoft.Expression.Project
{
	public static class DocumentTypeNamesHelper
	{
		public readonly static string ApplicationDefinition;

		public readonly static string Assembly;

		public readonly static string Xaml;

		public readonly static string Html;

		public readonly static string Licx;

		public readonly static string Image;

		public readonly static string WpfMedia;

		public readonly static string SilverlightAndWpfMedia;

		public readonly static string Folder;

		public readonly static string Font;

		public readonly static string ProjectReference;

		public readonly static string Xml;

		public readonly static string CSharp;

		public readonly static string VisualBasic;

		public readonly static string JavaScript;

		public readonly static string LimitedXaml;

		public readonly static string ComReference;

		public readonly static string DeepZoom;

		public readonly static string Xap;

		static DocumentTypeNamesHelper()
		{
			DocumentTypeNamesHelper.ApplicationDefinition = "ApplicationDefinition";
			DocumentTypeNamesHelper.Assembly = "Assembly";
			DocumentTypeNamesHelper.Xaml = "XAML";
			DocumentTypeNamesHelper.Html = "HTML";
			DocumentTypeNamesHelper.Licx = "LicX";
			DocumentTypeNamesHelper.Image = "Image";
			DocumentTypeNamesHelper.WpfMedia = "WpfMedia";
			DocumentTypeNamesHelper.SilverlightAndWpfMedia = "SilverlightAndWpfMedia";
			DocumentTypeNamesHelper.Folder = "Folder";
			DocumentTypeNamesHelper.Font = "Font";
			DocumentTypeNamesHelper.ProjectReference = "ProjectReference";
			DocumentTypeNamesHelper.Xml = "Xml Document";
			DocumentTypeNamesHelper.CSharp = "C#";
			DocumentTypeNamesHelper.VisualBasic = "VB";
			DocumentTypeNamesHelper.JavaScript = "Javascript";
			DocumentTypeNamesHelper.LimitedXaml = "LimitedXaml";
			DocumentTypeNamesHelper.ComReference = "ComReference";
			DocumentTypeNamesHelper.DeepZoom = "DeepZoom";
			DocumentTypeNamesHelper.Xap = "Xap";
		}
	}
}