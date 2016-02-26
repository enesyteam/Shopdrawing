using Microsoft.Expression.DesignModel.Text;

namespace Microsoft.Expression.DesignModel.Code
{
	public interface ITextEditorService
	{
		ITextEditor CreateTextEditor(ITextBuffer textBuffer);
	}
}