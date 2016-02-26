using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shopdrawing.Reinforcement
{
    public enum SectionDirection
    {
        HorizontalSection = 0,
        VerticalSection = 1
    }

    public interface IHaveCrossSection
    {
        SectionDirection SectionDirection { get; set; }
        void GetHorizontalSection();
        void GetVerticalSection();
    }
}
