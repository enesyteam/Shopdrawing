using System;

namespace Microsoft.Expression.Project
{
    public interface INamedProject : IDocumentItem, IDisposable
    {
        string Name
        {
            get;
        }
    }
}