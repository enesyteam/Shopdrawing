using System;

namespace Microsoft.Expression.Framework.Commands
{
    public abstract class Command : ICommand
    {
        public virtual bool IsAvailable
        {
            get
            {
                return true;
            }
        }

        public virtual bool IsEnabled
        {
            get
            {
                return this.IsAvailable;
            }
        }

        protected Command()
        {
        }

        public abstract void Execute();

        public virtual object GetProperty(string propertyName)
        {
            if (propertyName == "IsEnabled")
            {
                return this.IsEnabled;
            }
            if (propertyName == "IsAvailable")
            {
                return this.IsAvailable;
            }
            if (propertyName != "IsVisible")
            {
                return null;
            }
            return this.IsAvailable;
        }

        public virtual void SetProperty(string propertyName, object propertyValue)
        {
        }
    }
}