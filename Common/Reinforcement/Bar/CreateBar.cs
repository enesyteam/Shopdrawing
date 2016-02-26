using System;
using DynamicGeometry;
using System.ComponentModel;
using System.Windows;
using System.Collections.Generic;
using Shopdrawing.Utilities;

namespace Shopdrawing.Reinforcement
{
    public partial class Bar : CompositeFigure
    {
        #region Creators
        /// <summary>
        /// See BS 8666:2005
        /// </summary>
        public void CreateBar()
        {
            switch (ShapeCode)
            {
                case BarShape2D.Shape00:
                    {
                        if (!ValidationDimensions.Check(this)) return;
                        BarShape00 bs00 = new BarShape00(this.Drawing, this.Dimensions, this.ViewDirection)
                        {
                            Diameter = this.Diameter,
                            Transform = this.Transform,
                            Rotate = this.Rotate,
                            InsertPoint = this.InsertPoint
                        };
                        bs00.Create();
                        this.Children.Add(bs00);
                        this.Dependencies = bs00.Dependencies;
                        break;
                    }
                case BarShape2D.Shape01:
                    {
                        if (!ValidationDimensions.Check(this)) return;
                        BarShape01 bs01 = new BarShape01(this.Drawing, this.Dimensions, this.ViewDirection)
                        {
                            Diameter = this.Diameter,
                            Transform = this.Transform,
                            Rotate = this.Rotate,
                            InsertPoint = this.InsertPoint
                        };
                        bs01.Create();
                        this.Children.Add(bs01);
                        this.Dependencies = bs01.Dependencies;
                        break;
                    }
                case BarShape2D.Shape11:
                    {
                        if (!ValidationDimensions.Check(this)) return;
                        BarShape11 bs11 = new BarShape11(this.Drawing, this.Dimensions, this.ViewDirection) 
                        {
                            Diameter = this.Diameter,
                            Transform = this.Transform,
                            Rotate = this.Rotate,
                            InsertPoint = this.InsertPoint
                        };
                        bs11.Create();
                        this.Children.Add(bs11);
                        this.Dependencies = bs11.Dependencies;
                        break;
                    }
                case BarShape2D.Shape12:
                    {

                        break;
                    }
                case BarShape2D.Shape13:
                    {
                       if(!ValidationDimensions.Check(this)) return;

                        BarShape13 bs13 = new BarShape13(this.Drawing, this.Dimensions, this.ViewDirection, this.InsertPoint)
                        {
                            //Diameter = this.Diameter,
                            //Transform = this.Transform,
                            //Rotate = this.Rotate,
                            //InsertPoint = this.InsertPoint
                        };
                        bs13.Create();
                        this.Children.Add(bs13);
                        this.Dependencies = bs13.Dependencies;
                        break;
                    }
                case BarShape2D.Shape14:
                    {
                        //ValidationDimensions.Check(this);
                        //BarShape14 bs14 = new BarShape14(this.Drawing, this.Dimensions, this.ViewDirection)
                        //{
                        //    Diameter = this.Diameter,
                        //    Transform = this.Transform,
                        //};
                        //bs14.Create();
                        //this.Children.Add(bs14);
                        //this.Dependencies = bs14.Dependencies;
                        break;
                    }
            }

            // 
            OnBarDimensionsChanged();
        }
        #endregion
    }
}
