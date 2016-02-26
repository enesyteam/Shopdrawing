// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataSourceNameValidator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class DataSourceNameValidator : IMessageBubbleValidator
  {
    private XmlDataSourceDialogModel model;
    private IMessageBubbleHelper messageBubble;

    public DataSourceNameValidator(XmlDataSourceDialogModel model)
    {
      this.model = model;
    }

    public void Initialize(UIElement targetElement, IMessageBubbleHelper helper)
    {
      this.model.PropertyChanged += new PropertyChangedEventHandler(this.NameChanged);
      this.messageBubble = helper;
    }

    private void NameChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "DataSourceName"))
        return;
      if (!this.model.IsNameValid)
        this.messageBubble.SetContent(new MessageBubbleContent(string.Format((IFormatProvider) CultureInfo.CurrentCulture, !PathHelper.IsDeviceName(this.model.DataSourceName) ? (!this.model.IsNameInUse ? StringTable.DataSourceNameErrorDescription : StringTable.DataSourceNameInUse) : StringTable.DataSourceNameDeviceNameConflict, new object[1]
        {
          (object) this.model.DataSourceName
        }), MessageBubbleType.Warning));
      else
        this.messageBubble.SetContent((MessageBubbleContent) null);
    }
  }
}
