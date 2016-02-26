// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataBindingProcessingContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System.Text;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class DataBindingProcessingContext
  {
    public ProcessingContextScope Scope { get; private set; }

    public DataBindingProcessingContext OuterContext { get; private set; }

    public DataBindingProcessingContext ParentContext { get; private set; }

    public DocumentNode DocumentNode { get; private set; }

    public DocumentCompositeNode DocumentCompositeNode { get; private set; }

    public IProperty Property { get; private set; }

    public RawDataSourceInfoBase DataContext { get; set; }

    public bool IsEmptyDataContext
    {
      get
      {
        if (this.DataContext.SourceNode == null)
          return string.IsNullOrEmpty(this.DataContext.NormalizedClrPath);
        return false;
      }
    }

    public RawDataSourceInfoBase ParentDataContext
    {
      get
      {
        if (this.ParentContext != null)
          return this.ParentContext.DataContext;
        return (RawDataSourceInfoBase) null;
      }
    }

    public RawDataSourceInfoBase GrandparentDataContext
    {
      get
      {
        if (this.ParentContext != null && this.ParentContext.ParentContext != null)
          return this.ParentContext.ParentContext.DataContext;
        return (RawDataSourceInfoBase) RawDataSourceInfo.NewEmpty;
      }
    }

    public DataBindingProcessingContext RootContext
    {
      get
      {
        DataBindingProcessingContext processingContext1 = this;
        for (DataBindingProcessingContext processingContext2 = this; processingContext2 != null; processingContext2 = processingContext2.ParentContext)
          processingContext1 = processingContext2;
        return processingContext1;
      }
    }

    public DocumentCompositeNode ParentNode
    {
      get
      {
        if (this.ParentContext == null)
          return (DocumentCompositeNode) null;
        return this.DocumentNode.Parent;
      }
    }

    public bool IsStyleOrControlTemplateScope
    {
      get
      {
        if (this.Scope != ProcessingContextScope.Style)
          return this.Scope == ProcessingContextScope.ControlTemplate;
        return true;
      }
    }

    public DataBindingProcessingContext(DocumentNode rootNode, DataBindingProcessingContext outerContext)
    {
      this.DocumentNode = rootNode;
      this.DocumentCompositeNode = this.DocumentNode as DocumentCompositeNode;
      this.DataContext = outerContext != null ? outerContext.DataContext : (RawDataSourceInfoBase) RawDataSourceInfo.NewEmpty;
      this.Scope = !PlatformTypes.DataTemplate.IsAssignableFrom((ITypeId) rootNode.Type) ? (!PlatformTypes.Style.IsAssignableFrom((ITypeId) rootNode.Type) ? (!PlatformTypes.ControlTemplate.IsAssignableFrom((ITypeId) rootNode.Type) ? ProcessingContextScope.Normal : ProcessingContextScope.ControlTemplate) : ProcessingContextScope.Style) : ProcessingContextScope.DataTemplate;
      this.OuterContext = outerContext;
    }

    public DataBindingProcessingContext(DataBindingProcessingContext parentContext, DocumentNode childNode, IProperty parentProperty)
    {
      this.ParentContext = parentContext;
      this.DocumentNode = childNode;
      this.DocumentCompositeNode = this.DocumentNode as DocumentCompositeNode;
      this.Property = parentProperty;
      if (parentContext != null)
      {
        this.Scope = parentContext.Scope;
        this.OuterContext = parentContext.OuterContext;
      }
      if (!PlatformTypes.ResourceDictionary.IsAssignableFrom((ITypeId) childNode.Type))
        return;
      this.Scope = ProcessingContextScope.ResourceDictionary;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.DataContext != null)
      {
        stringBuilder.Append(this.DataContext.ToString());
        stringBuilder.Append("; ");
      }
      if (this.DocumentNode != null)
      {
        stringBuilder.Append("[Current: ");
        stringBuilder.Append(this.DocumentNode.ToString());
        if (this.ParentNode != null)
        {
          stringBuilder.Append("=");
          stringBuilder.Append(this.ParentNode.ToString());
          stringBuilder.Append("[");
          if (this.Property != null)
            stringBuilder.Append(this.Property.ToString());
          stringBuilder.Append("]");
        }
        stringBuilder.Append("]; ");
      }
      string str = stringBuilder.ToString();
      if (string.IsNullOrEmpty(str))
        return "Empty";
      return str;
    }
  }
}
