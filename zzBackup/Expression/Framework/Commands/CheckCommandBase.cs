using System;

namespace Microsoft.Expression.Framework.Commands
{
    public abstract class CheckCommandBase : Command
    {
        protected CheckCommandBase()
        {
        }

        public override void Execute()
        {
            this.SetProperty("IsChecked", !(bool)this.GetProperty("IsChecked"));
        }

        public override object GetProperty(string propertyName)
        {
            if (propertyName == "IsChecked")
            {
                return this.IsChecked();
            }
            return base.GetProperty(propertyName);
        }

        protected abstract bool IsChecked();

        protected abstract void OnCheckedChanged(bool isChecked);

        public override void SetProperty(string propertyName, object propertyValue)
        {
            if (propertyName != "IsChecked")
            {
                base.SetProperty(propertyName, propertyValue);
                return;
            }
            this.OnCheckedChanged((bool)propertyValue);
        }
    }
}