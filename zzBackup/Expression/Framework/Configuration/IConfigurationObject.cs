// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Configuration.IConfigurationObject
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.ComponentModel;

namespace Microsoft.Expression.Framework.Configuration
{
  public interface IConfigurationObject : INotifyPropertyChanged
  {
    IConfigurationObject CreateConfigurationObject();

    IConfigurationObjectCollection CreateConfigurationObjectCollection();

    void Clear();

    bool HasProperty(string name);

    void ClearProperty(string name);

    object GetProperty(string name);

    object GetProperty(string name, object defaultValue);

    void SetProperty(string name, object value);

    void SetProperty(string name, object value, object defaultValue);
  }
}
