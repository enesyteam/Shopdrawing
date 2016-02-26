// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.SourceControl.SourceControlProcessModel
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.Framework.SourceControl
{
  public class SourceControlProcessModel
  {
    private SourceControlSuccess success = SourceControlSuccess.Failed;
    private Collection<ErrorMessage> nonfatalMessageList = new Collection<ErrorMessage>();
    private string[] fileNames;
    private string result;
    private string status;
    private SourceControlOnlineStatus online;
    private int itemCount;
    private int currentItem;
    private int countErrors;
    private int countWarnings;

    public string Result
    {
      get
      {
        return this.result;
      }
      set
      {
        this.result = value;
      }
    }

    public SourceControlSuccess Success
    {
      get
      {
        return this.success;
      }
      set
      {
        this.success = value;
        switch (this.success)
        {
          case SourceControlSuccess.None:
          case SourceControlSuccess.Failed:
          case SourceControlSuccess.Success:
            this.OnlineStatus = SourceControlOnlineStatus.Online;
            break;
          case SourceControlSuccess.Offline:
            this.OnlineStatus = SourceControlOnlineStatus.Offline;
            break;
        }
      }
    }

    public DispatcherHelper Dispatcher { get; set; }

    public SourceControlOnlineStatus OnlineStatus
    {
      get
      {
        return this.online;
      }
      set
      {
        this.online = value;
      }
    }

    public string Status
    {
      get
      {
        return this.status;
      }
    }

    public int ItemCount
    {
      get
      {
        return this.itemCount;
      }
      set
      {
        this.itemCount = value;
      }
    }

    public int CurrentItem
    {
      get
      {
        return this.currentItem;
      }
      set
      {
        this.currentItem = value < this.itemCount ? value : this.itemCount;
      }
    }

    public int CountErrors
    {
      get
      {
        return this.countErrors;
      }
      set
      {
        this.countErrors = value;
      }
    }

    public int CountWarnings
    {
      get
      {
        return this.countWarnings;
      }
      set
      {
        this.countWarnings = value;
      }
    }

    public Collection<ErrorMessage> NonfatalMessages
    {
      get
      {
        return this.nonfatalMessageList;
      }
    }

    public SourceControlProcessModel(string status, string[] fileNames)
    {
      this.status = status;
      this.fileNames = fileNames;
    }

    public SourceControlProcessModel(string status, string fileName)
      : this(status, new string[1]
      {
        fileName
      })
    {
    }

    public string[] GetFileNames()
    {
      return this.fileNames;
    }

    public void AddErrorMessage(ErrorLevel errorLevel, string message)
    {
      this.nonfatalMessageList.Add(new ErrorMessage(errorLevel, message));
      switch (errorLevel)
      {
        case ErrorLevel.Warning:
          ++this.countWarnings;
          break;
        case ErrorLevel.Error:
          ++this.countErrors;
          break;
      }
    }
  }
}
