using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shopdrawing.Reinforcement
{
     public enum Transform
     {
         Normal = 0,
         Rotate180 = 1,
         Rotate90CW = 2,
         Rotate90CCW = 3,
         FlipHorizontal = 4,
         FlipVertical = 5,
         /// <summary>
         /// Quay với một góc xác định
         /// </summary>
         //Arbitrary = 6
     }
     public enum Direction
     { 
         Up=0,
         Down=1
     }
}
