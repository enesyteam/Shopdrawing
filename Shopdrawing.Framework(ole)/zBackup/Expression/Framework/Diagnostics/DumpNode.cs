// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.DumpNode
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public class DumpNode
  {
    private ObservableCollection<DumpNode> children = new ObservableCollection<DumpNode>();
    private string nameLower = string.Empty;
    private string typeNameLower = string.Empty;
    private string name;
    private string typeName;
    private int totalChildren;
    private Visual visual;

    public ObservableCollection<DumpNode> Children
    {
      get
      {
        return this.children;
      }
    }

    public Visual Visual
    {
      get
      {
        return this.visual;
      }
    }

    public DumpNode(Visual visual)
    {
      this.visual = visual;
      FrameworkElement frameworkElement = visual as FrameworkElement;
      this.name = frameworkElement != null ? frameworkElement.Name : string.Empty;
      this.typeName = visual.GetType().Name;
      this.nameLower = this.name.ToLower();
      this.typeNameLower = this.typeName.ToLower();
      for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount((DependencyObject) visual); ++childIndex)
      {
        Visual visual1 = VisualTreeHelper.GetChild((DependencyObject) visual, childIndex) as Visual;
        if (visual1 != null)
          this.Children.Add(new DumpNode(visual1));
      }
    }

    public override string ToString()
    {
      return this.name + (object) " (" + this.typeName + ") " + (string) (object) this.totalChildren;
    }

    public int UpdateChildrenCount()
    {
      this.totalChildren = 0;
      foreach (DumpNode dumpNode in (Collection<DumpNode>) this.Children)
        this.totalChildren += dumpNode.UpdateChildrenCount();
      return this.totalChildren + 1;
    }

    public bool Filter(string value)
    {
      return this.typeNameLower.Contains(value) || this.nameLower.Contains(value);
    }
  }
}
