using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DynamicGeometry;

namespace Shopdrawing.Reinforcement
{
    public enum ContainerShape
    {
        /// <summary>
        /// All sides equal, interior angles 60 degree
        /// </summary>
        EquilaterialTriangle = 0,
        /// <summary>
        /// All sides equal, all angles 90 degree
        /// </summary>
        Square = 1,
        /// <summary>
        /// 2 sides equal, 2 congruent angles
        /// </summary>
        IsoscelesTriangle = 2,
        /// <summary>
        /// Opposite sides equal, all angles 90
        /// </summary>
        Rectangle = 3
    }


    public class MeshContainer : CompositeFigure
    {
        public ContainerShape Shape { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Length { get; set; }
        public Point Focus { get; set; } // need a more elegent name
        public double Cover { get; set; } // Lớp bê tông bảo vệ

        public MeshContainer()
        { 
        
        }
    }
}
