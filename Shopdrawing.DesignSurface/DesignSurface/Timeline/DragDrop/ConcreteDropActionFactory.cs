// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.ConcreteDropActionFactory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public abstract class ConcreteDropActionFactory : IDropActionFactory
  {
    public abstract IDropAction CreateInstance(DragDropContext context);

    protected ISceneInsertionPoint GetInsertionPoint(object data, DragDropContext context)
    {
      this.CheckNullArgument(data, "data");
      if (context.Target != null)
      {
        ISceneInsertionPoint insertionPoint = context.Target.GetInsertionPoint(data, context);
        if (insertionPoint != null)
          return (ISceneInsertionPoint) SmartInsertionPoint.From(insertionPoint, context.Descriptor.DropIndex);
      }
      return (ISceneInsertionPoint) null;
    }

    protected void CheckNullArgument(object argument, string argumentName)
    {
      if (argument == null)
        throw new ArgumentNullException(argumentName);
    }
  }
}
