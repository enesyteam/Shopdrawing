// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.NewItemFactory
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal;
using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public class NewItemFactory
  {
    private Type[] NoTypes = new Type[0];

    public virtual Stream GetImageStream(Type type, Size desiredSize, out string imageName)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      ExtensibleIconLookup extensibleIconLookup = new ExtensibleIconLookup(type, (int) desiredSize.Width, (int) desiredSize.Height);
      imageName = extensibleIconLookup.ResourceName;
      return extensibleIconLookup.Stream;
    }

    public virtual string GetDisplayName(Type type)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      return type.Name;
    }

    public virtual object CreateInstance(Type type)
    {
      if (type == null)
        throw new ArgumentNullException("type");
      ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, (Binder) null, this.NoTypes, (ParameterModifier[]) null);
      if (constructor != null)
        return constructor.Invoke((object[]) null);
      return (object) null;
    }
  }
}
