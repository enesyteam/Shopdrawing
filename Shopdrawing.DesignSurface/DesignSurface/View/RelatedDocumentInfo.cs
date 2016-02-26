// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.RelatedDocumentInfo
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface.Documents;

namespace Microsoft.Expression.DesignSurface.View
{
  public class RelatedDocumentInfo
  {
    private uint changeStamp = uint.MaxValue;
    private SceneDocument document;
    private DocumentNodeChangeList damage;

    public uint ChangeStamp
    {
      get
      {
        return this.changeStamp;
      }
    }

    public DocumentNodeChangeList Damage
    {
      get
      {
        return this.damage;
      }
    }

    public IDocumentRoot DocumentRoot
    {
      get
      {
        return (IDocumentRoot) this.document.XamlDocument;
      }
    }

    public RelatedDocumentInfo(SceneDocument document)
    {
      this.document = document;
      this.damage = new DocumentNodeChangeList();
      document.XamlDocument.RegisterChangeList(this.damage);
      this.UpdateChangeStamp();
    }

    public void UpdateChangeStamp()
    {
      this.changeStamp = this.document.XamlDocument.ChangeStamp;
    }

    public void ClearDamage()
    {
      this.damage.Clear();
    }

    public void Unregister()
    {
      this.document.XamlDocument.UnregisterChangeList(this.damage);
      this.damage.Clear();
      this.damage = (DocumentNodeChangeList) null;
    }
  }
}
