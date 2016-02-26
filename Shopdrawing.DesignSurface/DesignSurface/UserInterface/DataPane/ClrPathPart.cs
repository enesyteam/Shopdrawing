// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ClrPathPart
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ClrPathPart
  {
    private static ClrPathPart currentItem = new ClrPathPart("/", ClrPathPartCategory.CurrentItem);
    private string newPath;

    public static ClrPathPart CurrentItem
    {
      get
      {
        return ClrPathPart.currentItem;
      }
    }

    public string Path { get; private set; }

    public ClrPathPartCategory Category { get; private set; }

    public string NewPath
    {
      get
      {
        return this.newPath;
      }
      set
      {
        this.newPath = value;
      }
    }

    public ClrPathPart(string pathPart, ClrPathPartCategory partCategory)
    {
      this.Category = partCategory;
      this.Path = pathPart;
      this.newPath = pathPart;
    }

    public override string ToString()
    {
      return string.Concat(new object[4]
      {
        !(this.NewPath != this.Path) ? (object) this.Path : (object) (this.Path + "->" + this.NewPath),
        (object) " [",
        (object) this.Category,
        (object) "]"
      });
    }
  }
}
