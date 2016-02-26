using System;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewStoryboard : IViewObject
	{
		ViewStoryboardState CurrentState
		{
			get;
		}

		double CurrentTime
		{
			get;
		}

		IViewObject Target
		{
			get;
		}

		void Apply(IViewObject context, IViewObject target, ViewStoryboardApplyOptions flags);

		void Pause();

		void Play();

		void Remove();

		void Seek(double time);

		event EventHandler<EventArgs> CurrentStateChanged;

		event EventHandler<EventArgs> CurrentTimeChanged;
	}
}