using Microsoft.Expression.Framework.Controls;
using System;

namespace Microsoft.Expression.Project.UserInterface
{
    public class ProjectDialog : Dialog
    {
        private ProjectDialog.ProjectDialogResult result = ProjectDialog.ProjectDialogResult.Cancel;

        internal static string[] ReservedNames;

        public ProjectDialog.ProjectDialogResult Result
        {
            get
            {
                return this.result;
            }
            protected set
            {
                this.result = value;
            }
        }

        static ProjectDialog()
        {
            string[] strArrays = new string[] { ".", "..", "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };
            ProjectDialog.ReservedNames = strArrays;
        }

        internal ProjectDialog()
        {
        }

        public enum ProjectDialogResult
        {
            Ok,
            Discard,
            Cancel
        }
    }
}