using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Project
{
    public interface IDocumentItem
    {
        IEnumerable<IDocumentItem> Descendants
        {
            get;
        }

        DocumentReference DocumentReference
        {
            get;
        }

        bool IsDirectory
        {
            get;
        }

        bool IsReference
        {
            get;
        }

        bool IsVirtual
        {
            get;
        }

        Version VersionNumber
        {
            get;
        }
    }
}