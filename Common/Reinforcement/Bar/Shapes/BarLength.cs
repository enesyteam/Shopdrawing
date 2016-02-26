using System;

namespace Shopdrawing.Reinforcement
{
    public class BarLength
    {
        public static double GetLength(Bar bar)
        {
            switch (bar.ShapeCode)
            {
                case BarShape2D.Shape13:
                    {
                        return bar.Dimensions.A + 0.57 * bar.Dimensions.B + bar.Dimensions.C - 1.6 * bar.Diameter;
                    }

                default: return 0;
            }
        
        }
    }
}
