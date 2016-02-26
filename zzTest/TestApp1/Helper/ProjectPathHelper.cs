using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BridgeFS.Helper
{
    public class ProjectPathHelper
    {
        private static char[] invalidProjectNameCharacters;

        static ProjectPathHelper()
        {
            ProjectPathHelper.invalidProjectNameCharacters = new char[] { '&', '#', '%', ';' };
        }
        public static bool IsValidPath(string path)
        {
            bool flag = PathHelper.IsValidPath(path);
            if (flag)
            {
                int num = path.IndexOf(':');
                if (num > 0 && !PathHelper.IsValidDrive(path[num - 1]))
                {
                    flag = false;
                }
            }
            return flag;
        }
        public static bool IsValidNewSolutionPathFileName(string name)
        {
            bool flag = PathHelper.IsValidPath(name, false);
            if (flag && name != null && PathHelper.FileExists(name))
            {
                flag = false;
            }
            return flag;
        }
        public static bool IsValidProjectFileName(string name)
        {
            if (name.IndexOfAny(ProjectPathHelper.invalidProjectNameCharacters) != -1)
            {
                return false;
            }
            return PathHelper.IsValidFileOrDirectoryName(name);
        }
        public static string GetFolderPath(string dialogDescription, string vistaDialogTitle, string initialDirectory)
        {
            string selectedPath;
            if (!ExpressionFileDialog.CanPickFolders)
            {
                using (ModalDialogHelper modalDialogHelper = new ModalDialogHelper())
                {
                    FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
                    {
                        Description = dialogDescription,
                        SelectedPath = initialDirectory
                    };
                    if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                    {
                        return null;
                    }
                    else
                    {
                        selectedPath = folderBrowserDialog.SelectedPath;
                    }
                }
                return selectedPath;
            }
            else
            {
                ExpressionOpenFileDialog expressionOpenFileDialog = new ExpressionOpenFileDialog()
                {
                    Title = vistaDialogTitle,
                    InitialDirectory = initialDirectory,
                    PickFolders = true
                };
                bool? nullable = expressionOpenFileDialog.ShowDialog();
                if (nullable.HasValue && nullable.Value)
                {
                    return expressionOpenFileDialog.FileName;
                }
            }
            return null;
        }
    }
}
