// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.PathChange
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public abstract class PathChange
  {
    public DocumentCompositeNode DocumentNode { get; set; }

    public PathChangeInfo Change { get; set; }

    protected PathChange(PathChangeInfo change, DocumentCompositeNode documentNode)
    {
      this.Change = change;
      this.DocumentNode = documentNode;
    }

    public abstract void ApplyChange(SceneViewModel viewModel);

    public override string ToString()
    {
      if (this.Change != null && this.DocumentNode != null)
        return this.DocumentNode.ToString() + ": " + this.Change.ToString();
      if (this.Change != null)
        return this.Change.ToString();
      if (this.DocumentNode != null)
        return this.DocumentNode.ToString();
      return "Empty";
    }
  }
}
