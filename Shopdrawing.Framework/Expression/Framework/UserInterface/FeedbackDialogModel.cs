// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.FeedbackDialogModel
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Feedback;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Expression.Framework.UserInterface
{
  public class FeedbackDialogModel : INotifyPropertyChanged
  {
    private bool? feedbackChoice;
    private bool canSetFeedbackPolicy;
    private string applicationName;
    private IFeedbackService feedbackService;

    public bool? FeedbackChoice
    {
      get
      {
        return this.feedbackChoice;
      }
      set
      {
        this.feedbackChoice = value;
        this.OnPropertyChanged("FeedbackChoice");
        this.OnPropertyChanged("IsAcceptButtonEnabled");
      }
    }

    public bool IsPolicySettingEnabled
    {
      get
      {
        return this.canSetFeedbackPolicy;
      }
    }

    public bool IsAcceptButtonEnabled
    {
      get
      {
        if (this.canSetFeedbackPolicy)
          return this.feedbackChoice.HasValue;
        return false;
      }
    }

    public string HeaderText
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.FeedbackDialogHeaderText, new object[1]
        {
          (object) this.applicationName
        });
      }
    }

    public string ParagraphText
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.FeedbackDialogParagraphText, new object[1]
        {
          (object) this.applicationName
        });
      }
    }

    public string OptInText
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.FeedbackDialogOptInText, new object[1]
        {
          (object) this.applicationName
        });
      }
    }

    public string OptOutText
    {
      get
      {
        return StringTable.FeedbackDialogOptOutText;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public FeedbackDialogModel(string applicationName, IFeedbackService feedbackService)
    {
      this.feedbackService = feedbackService;
      this.feedbackChoice = feedbackService.AggregateFeedbackPolicy;
      this.canSetFeedbackPolicy = feedbackService.CanSetFeedbackPolicy;
      this.applicationName = applicationName;
    }

    public void CommitChanges()
    {
      bool? customerFeedbackPolicy = this.feedbackService.CustomerFeedbackPolicy;
      bool? nullable = this.feedbackChoice;
      if ((customerFeedbackPolicy.GetValueOrDefault() != nullable.GetValueOrDefault() ? true : (customerFeedbackPolicy.HasValue != nullable.HasValue ? true : false)) == false)
        return;
      this.feedbackService.CustomerFeedbackPolicy = this.feedbackChoice;
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
