// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.ModifierKeyBehaviorManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.Tools
{
  public sealed class ModifierKeyBehaviorManager
  {
    private readonly List<Key> modifierKeys = new List<Key>()
    {
      Key.LeftCtrl,
      Key.RightCtrl,
      Key.LeftShift,
      Key.RightShift,
      Key.LeftAlt,
      Key.RightAlt,
      Key.Space
    };
    private List<IModifierKeyBehaviorFactory> behaviorFactories = new List<IModifierKeyBehaviorFactory>();
    private IModifierKeyBehaviorFactory activeModifierKeyBehaviorFactory;
    private ToolBehavior activeModifierKeyBehavior;
    private EventRouter eventRouter;
    private bool suspendingMode;

    public ModifierKeyBehaviorManager(EventRouter eventRouter)
    {
      if (eventRouter == null)
        throw new ArgumentNullException("eventRouter");
      this.eventRouter = eventRouter;
    }

    public void Register(IModifierKeyBehaviorFactory behaviorFactory)
    {
      this.behaviorFactories.Add(behaviorFactory);
    }

    public void Unregister(IModifierKeyBehaviorFactory behaviorFactory)
    {
      this.behaviorFactories.Remove(behaviorFactory);
    }

    public void TrySwitchModifierKeyBehavior(KeyEventArgs args)
    {
      if (this.eventRouter.ActiveBehavior == null || this.eventRouter.IsDragging || (this.eventRouter.IsButtonDown || !this.eventRouter.ActiveBehavior.ToolBehaviorContext.View.Artboard.IsMouseOver) || this.eventRouter.IsEditingText)
        return;
      if (this.activeModifierKeyBehavior != null && !this.eventRouter.ContainsBehavior(this.activeModifierKeyBehavior))
      {
        this.activeModifierKeyBehavior = (ToolBehavior) null;
        this.activeModifierKeyBehaviorFactory = (IModifierKeyBehaviorFactory) null;
      }
      ExtendedModifierKeys extendedModifierKeys = this.ComputeExtendedModifierKeys();
      if (this.activeModifierKeyBehaviorFactory != null && !this.activeModifierKeyBehaviorFactory.ShouldActivate(extendedModifierKeys))
        this.RemoveActiveModifierKeyBehavior();
      if (args != null && !this.eventRouter.IsDragging)
      {
        Key key = args.Key == Key.System ? args.SystemKey : args.Key;
        if (extendedModifierKeys == ExtendedModifierKeys.None)
        {
          this.suspendingMode = false;
          return;
        }
        if (!this.modifierKeys.Contains(key) && key != Key.ImeProcessed)
        {
          this.RemoveActiveModifierKeyBehavior();
          this.suspendingMode = true;
        }
        if (this.suspendingMode)
          return;
      }
      if (this.activeModifierKeyBehaviorFactory != null)
        return;
      foreach (IModifierKeyBehaviorFactory keyBehaviorFactory in this.behaviorFactories)
      {
        if (keyBehaviorFactory.ShouldActivate(extendedModifierKeys))
        {
          ToolBehavior instance = keyBehaviorFactory.CreateInstance(this.eventRouter.ActiveBehavior.ToolBehaviorContext);
          if (instance != null)
          {
            this.activeModifierKeyBehavior = instance;
            this.activeModifierKeyBehaviorFactory = keyBehaviorFactory;
            this.eventRouter.PushBehavior(this.activeModifierKeyBehavior);
            break;
          }
        }
      }
    }

    private ExtendedModifierKeys ComputeExtendedModifierKeys()
    {
      ExtendedModifierKeys extendedModifierKeys = (ExtendedModifierKeys) Keyboard.Modifiers;
      if (Keyboard.IsKeyDown(Key.Space))
        extendedModifierKeys |= ExtendedModifierKeys.Space;
      return extendedModifierKeys;
    }

    private void RemoveActiveModifierKeyBehavior()
    {
      if (this.activeModifierKeyBehaviorFactory == null)
        return;
      for (ToolBehavior activeBehavior = this.eventRouter.ActiveBehavior; activeBehavior != this.activeModifierKeyBehavior; activeBehavior = this.eventRouter.ActiveBehavior)
        this.eventRouter.PopBehavior();
      this.eventRouter.PopBehavior();
      this.eventRouter.ActiveBehavior.ToolBehaviorContext.ToolManager.OverrideTool = (Tool) null;
      this.activeModifierKeyBehavior = (ToolBehavior) null;
      this.activeModifierKeyBehaviorFactory = (IModifierKeyBehaviorFactory) null;
    }
  }
}
