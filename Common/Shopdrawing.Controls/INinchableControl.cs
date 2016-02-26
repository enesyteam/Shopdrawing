using System;

namespace Shopdrawing.Controls
{
    public interface INinchableControl
    {
        bool IsNinched
        {
            get;
        }

        void Ninch();
    }
}