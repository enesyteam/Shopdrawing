using System;

namespace Microsoft.Expression.Project.Documents
{
	public interface ISetCaretPosition
	{
		void SetCaretPosition(int line, int column);
	}
}