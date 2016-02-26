// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.CodeAid.Xaml.XamlCompletion
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code;
using Microsoft.Expression.DesignModel.Code;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System.Windows.Media;

namespace Microsoft.Expression.Code.CodeAid.Xaml
{
  internal class XamlCompletion : ICompletion, ICodeAidCompletion
  {
    private static string resourcesDirectory = "Resources\\Intellisense\\";
    private static ImageSource AttributeIconImage = FileTable.GetImageSource(XamlCompletion.resourcesDirectory + "Attribute.png");
    private static ImageSource NamespacePrefixIconImage;
    private static ImageSource ElementIconImage;
    private static ImageSource ValueIconImage;
    private static ImageSource SyntaxIconImage;
    private static ImageSource EventIconImage;
    private static ImageSource XmlIconImage;
    private ITrackingSpan applicableTo;
    private XamlCompletion.CompletionMetadata completionMetadata;

    public ITrackingSpan ApplicableTo
    {
      get
      {
        return this.applicableTo;
      }
      private set
      {
        this.applicableTo = value;
      }
    }

    public IClassificationType ClassificationType { get; private set; }

    public string Description
    {
      get
      {
        return this.completionMetadata.MemberInfo.DescriptionText;
      }
    }

    public string DisplayText
    {
      get
      {
        return this.completionMetadata.MemberInfo.Name;
      }
    }

    public string InsertionText
    {
      get
      {
        return this.completionMetadata.MemberInfo.Name;
      }
    }

    public bool IsCommon
    {
      get
      {
        return false;
      }
    }

    public object ItemMetadata
    {
      get
      {
        return (object) this.completionMetadata;
      }
    }

    public ICompletionSession Session { get; private set; }

    public ICompletionProvider Source { get; private set; }

    public CompletionType CompletionType { get; private set; }

    public int CompletionCaretDelta
    {
      get
      {
        return 0;
      }
    }

    static XamlCompletion()
    {
      XamlCompletion.AttributeIconImage.Freeze();
      XamlCompletion.ElementIconImage = FileTable.GetImageSource(XamlCompletion.resourcesDirectory + "Type.png");
      XamlCompletion.ElementIconImage.Freeze();
      XamlCompletion.EventIconImage = FileTable.GetImageSource(XamlCompletion.resourcesDirectory + "Method.png");
      XamlCompletion.EventIconImage.Freeze();
      XamlCompletion.NamespacePrefixIconImage = FileTable.GetImageSource(XamlCompletion.resourcesDirectory + "Namespace.png");
      XamlCompletion.NamespacePrefixIconImage.Freeze();
      XamlCompletion.ValueIconImage = FileTable.GetImageSource(XamlCompletion.resourcesDirectory + "Enum.png");
      XamlCompletion.ValueIconImage.Freeze();
      XamlCompletion.SyntaxIconImage = FileTable.GetImageSource(XamlCompletion.resourcesDirectory + "Syntax.png");
      XamlCompletion.SyntaxIconImage.Freeze();
      XamlCompletion.XmlIconImage = FileTable.GetImageSource(XamlCompletion.resourcesDirectory + "Xml.png");
      XamlCompletion.XmlIconImage.Freeze();
    }

    internal XamlCompletion(ICompletionSession session, ITrackingSpan trackingSpan, IClassificationType classType, CompletionType completionType, XamlCompletionProvider provider, ICodeAidMemberInfo memberInfo)
    {
      this.ApplicableTo = trackingSpan;
      this.ClassificationType = classType;
      this.CompletionType = completionType;
      this.completionMetadata = new XamlCompletion.CompletionMetadata(memberInfo, this.GetBrushFromCompletionType(this.CompletionType));
      this.Session = session;
      this.Source = (ICompletionProvider) provider;
    }

    private ImageSource GetBrushFromCompletionType(CompletionType completionType)
    {
      switch (completionType)
      {
        case CompletionType.ClosingTag:
          return XamlCompletion.SyntaxIconImage;
        case CompletionType.XamlNamespaceMembers:
        case CompletionType.XmlnsMarkup:
        case CompletionType.Properties:
        case CompletionType.AttachedProperties:
          return XamlCompletion.AttributeIconImage;
        case CompletionType.EndCommentMarkup:
        case CompletionType.StartCommentMarkup:
          return XamlCompletion.XmlIconImage;
        case CompletionType.Prefixes:
        case CompletionType.AttachedPropertyTypesOnly:
          return XamlCompletion.NamespacePrefixIconImage;
        case CompletionType.EnumerationValues:
          return XamlCompletion.ValueIconImage;
        case CompletionType.Types:
          return XamlCompletion.ElementIconImage;
        default:
          return (ImageSource) null;
      }
    }

    private struct CompletionMetadata
    {
      public ICodeAidMemberInfo MemberInfo { get; private set; }

      public ImageSource Icon { get; private set; }

      public CompletionMetadata(ICodeAidMemberInfo memberInfo, ImageSource icon)
      {
        this = new XamlCompletion.CompletionMetadata();
        this.MemberInfo = memberInfo;
        this.Icon = icon;
      }
    }
  }
}
