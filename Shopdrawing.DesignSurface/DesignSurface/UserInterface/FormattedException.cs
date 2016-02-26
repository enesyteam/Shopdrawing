// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.FormattedException
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class FormattedException
  {
    private readonly ICollection<IAssembly> assemblies;
    private readonly Exception exception;
    private FormattedException innerException;
    private List<StackFrame> stackFrames;
    private string stackTrace;

    public Exception SourceException
    {
      get
      {
        return this.exception;
      }
    }

    public string Message
    {
      get
      {
        return this.exception.Message;
      }
    }

    public string TypeName
    {
      get
      {
        if (this.exception is InstanceBuilderException)
          return StringTable.DefaultExceptionTypeName;
        return this.exception.GetType().Name;
      }
    }

    public string HeaderMessage
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ExceptionHeaderMessage, new object[2]
        {
          (object) this.TypeName,
          (object) this.exception.Message
        });
      }
    }

    public string InnerExceptionMessage
    {
      get
      {
        if (this.InnerException == null)
          return "";
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.InnerExceptionMessage, new object[1]
        {
          (object) this.InnerException.Message
        });
      }
    }

    public FormattedException InnerException
    {
      get
      {
        if (this.innerException == null)
        {
          Exception innerException = this.exception.InnerException;
          if (innerException != null)
            this.innerException = new FormattedException(this.assemblies, innerException);
        }
        return this.innerException;
      }
    }

    public ICollection<StackFrame> StackFrames
    {
      get
      {
        if (this.stackFrames == null)
        {
          this.stackFrames = new List<StackFrame>();
          StackFrame[] frames = new System.Diagnostics.StackTrace(this.exception).GetFrames();
          if (frames != null)
          {
            foreach (StackFrame stackFrame in frames)
            {
              if (this.ContainsAssembly(stackFrame.GetMethod().DeclaringType.Assembly))
                this.stackFrames.Add(stackFrame);
              else
                break;
            }
          }
        }
        return (ICollection<StackFrame>) this.stackFrames;
      }
    }

    public string StackTrace
    {
      get
      {
        if (this.stackTrace == null)
        {
          StringBuilder stringBuilder = new StringBuilder();
          foreach (StackFrame frame in (IEnumerable<StackFrame>) this.StackFrames)
          {
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(frame);
            stringBuilder.Append(stackTrace.ToString());
          }
          this.stackTrace = stringBuilder.ToString();
          if (this.stackTrace.EndsWith("\r\n", StringComparison.Ordinal))
            this.stackTrace = this.stackTrace.Substring(0, this.stackTrace.Length - 2);
        }
        return this.stackTrace;
      }
    }

    public FormattedException(ICollection<IAssembly> assemblies, Exception exception)
    {
      if (assemblies == null)
        throw new ArgumentNullException("assemblies");
      if (exception == null)
        throw new ArgumentNullException("exception");
      this.assemblies = assemblies;
      this.exception = exception;
    }

    private bool ContainsAssembly(Assembly assembly)
    {
      foreach (IAssembly assembly1 in (IEnumerable<IAssembly>) this.assemblies)
      {
        if (assembly1.CompareTo(assembly))
          return true;
      }
      return false;
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ExceptionWithStackTrace, new object[2]
      {
        (object) this.Message,
        (object) this.StackTrace
      });
    }
  }
}
