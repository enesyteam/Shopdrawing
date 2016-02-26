#define FUCK

using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DynamicGeometry;
using System.Linq;
using System.Reflection;
using System;

namespace Shopdrawing.Reinforcement
{
    public class MeshBase : CompositeFigure
    {
        private Transform _transform = Transform.Normal;
        public virtual Transform Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }
        private Direction _direction = Direction.Up;
        [PropertyGridVisible]
        [PropertyGridName("Direction")]
        public virtual Direction Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        public MeshContainer MeshContainer { get; set; }
        public Point InsertPoint { get; set; }
        public virtual double HorizontalBarsSpacing { get; set; }
        public virtual double VerticalBarsSpacing { get; set; }
        [PropertyGridVisible]
        [PropertyGridName("Width")]
        public virtual double Width { get; set; }
        [PropertyGridVisible]
        [PropertyGridName("Height")]
        public virtual double Height { get; set; }
        [PropertyGridVisible]
        [PropertyGridName("Deep")]
        public virtual double Deep { get; set; }

        private Bar _bar = new Bar() { Diameter = 1, Name = "B00" };
        public Bar Bar
        {
            get { return _bar; }
            set { _bar = value; }
        }
        public virtual double Cover { get; set; }

        #region Constructors
        public MeshBase()
        { }
        public MeshBase(DynamicGeometry.Drawing drawing, MeshContainer meshContainer, Bar bar)
        {
            Drawing = drawing;
            MeshContainer = meshContainer;
            InsertPoint = meshContainer.Focus;
            Bar = bar;
            Cover = bar.Cover;
            Width = MeshContainer.Width;
            VerticalBarsSpacing = Bar.Step;
            HorizontalBarsSpacing = Bar.Step;
        }

        #endregion

        [PropertyGridVisible]
        [PropertyGridName("Update")]
        public virtual void Update()
        {
        }
        /// <summary>
        /// Copy tất cả thuộc tính 1 đối tượng sang đối tượng mới
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public virtual object CloneObject(object o)
        {
            Type t = o.GetType();
            PropertyInfo[] properties = t.GetProperties();

            Object p = t.InvokeMember("", System.Reflection.BindingFlags.CreateInstance,
                null, o, null);

            foreach (PropertyInfo pi in properties)
            {
                if (pi.CanWrite)
                {
                    pi.SetValue(p, pi.GetValue(o, null), null);
                }
            }

            return p;
        }

        /// <summary>
        /// Tìm các điểm trên đoạn thẳng cách đều một khoảng delta
        /// với góc nghiêng của đoạn biết trước
        /// </summary>
        /// <param name="width">Bề rộng bao</param>
        /// <param name="width">Chiều cao bao</param>
        /// <returns></returns>
        public IEnumerable<Point> GetVisiblePoint(double width, double height)
        {
            SinAlpha = height / System.Math.Sqrt(height * height + width * width);
            CosAlpha = width / System.Math.Sqrt(height * height + width * width);

            IEnumerable<double> XPoints = new List<double>();
            IEnumerable<double> YPoints = new List<double>();

            if (width != 0 && height != 0)
            {
                XPoints = GetVisibleX(width);
                YPoints = GetVisibleY(height);

                for (int i = 0; i < XPoints.Count(); i++)
                {
                    yield return new Point(XPoints.ElementAt(i), YPoints.ElementAt(i));
                }
            }
            else if (width == 0)
            {
                XPoints = GetVisibleXHorizontal(width);
                YPoints = GetVisibleYVertical(height);


                for (int i = 0; i < YPoints.Count(); i++)
                {
                    yield return new Point(XPoints.ElementAt(0), YPoints.ElementAt(i));
                }
            
            }
            else
            {
                XPoints = GetVisibleXHorizontal(width);
                YPoints = GetVisibleYVertical(height);

                for (int i = 0; i < XPoints.Count(); i++)
                {
                    yield return new Point(XPoints.ElementAt(i), YPoints.ElementAt(0));
                }
            
            }
        }

        #region Chia một đoạn thẳng thành các đoạn đều nhau
        /// <summary>
        /// -------------------
        /// |                   |
        /// |    /              | 
        /// |  / Alpha          |
        /// |/------------------|
        /// </summary>
        public double SinAlpha { get; set; }
        public double CosAlpha { get; set; }

        public IEnumerable<double> GetVisibleX(double width)
        {
            //               |------------------->
            List<double> result = new List<double>();
            for (var x = InsertPoint.X;
                    x <= (InsertPoint.X + width / 2 - Bar.Cover * CosAlpha);
                    x += VerticalBarsSpacing * CosAlpha)
            {
                if (x == InsertPoint.X) continue;
                //odd = System.Math.Abs((MeshContainer.Focus.X + width / 2 - Bar.Cover * CosAlpha) - x);
                //if (System.Math.Abs(odd) < VerticalBarsSpacing * CosAlpha) continue;
                result.Add(x);
            }
            // Middle
            result.Add(InsertPoint.X);
            //<----------------|
            for (var x = InsertPoint.X;
                    x >= (InsertPoint.X - width / 2 + Bar.Cover * CosAlpha);
                    x -= VerticalBarsSpacing * CosAlpha)
            {
                if (x == InsertPoint.X) continue;
                //odd = System.Math.Abs((MeshContainer.Focus.X - width / 2 + Bar.Cover * CosAlpha) - x);
                //if (System.Math.Abs(odd) < VerticalBarsSpacing * CosAlpha) continue;
                result.Add(x);
            }
            result.Sort();

            return result;
        }

        public IEnumerable<double> GetVisibleY(double height)
        {
            //               |------------------->
            List<double> result = new List<double>();
            for (var y = InsertPoint.Y;
                    y <= (InsertPoint.Y + height / 2 - Bar.Cover * SinAlpha);
                    y += HorizontalBarsSpacing * SinAlpha)
            {
                if (y == MeshContainer.Focus.Y) continue;
                result.Add(y);
            }
            // Middle
            result.Add(InsertPoint.Y);
            //<----------------|
            for (var y = InsertPoint.Y;
                    y >= (InsertPoint.Y - height / 2 + Bar.Cover * SinAlpha);
                    y -= HorizontalBarsSpacing * SinAlpha)
            {
                if (y == InsertPoint.Y) continue;
                result.Add(y);
            }
            result.Sort();
            return Direction == Reinforcement.Direction.Up ? result : result.OrderByDescending(i => i).ToList();
        }
        /// <summary>
        /// Tìm điểm trên đoạn thẳng nằm ngang
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public IEnumerable<double> GetVisibleXHorizontal(double width)
        {
            //               |------------------->
            List<double> result = new List<double>();
            for (var x = InsertPoint.X;
                    x <= (InsertPoint.X + width / 2 - Bar.Cover);
                    x += VerticalBarsSpacing)
            {
                if (x == InsertPoint.X) continue;
                result.Add(x);
            }

            // Middle
            result.Add(InsertPoint.X);

            //<----------------|
            for (var x = InsertPoint.X;
                    x >= (InsertPoint.X - width / 2 + Bar.Cover);
                    x -= VerticalBarsSpacing)
            {
                if (x == InsertPoint.X) continue;
                result.Add(x);
            }
            result.Sort();
            return result;
        }
        /// <summary>
        /// Tìm điểm trên đoạn thẳng đứng
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public IEnumerable<double> GetVisibleYVertical(double height)
        {
            //double odd = 0;
            //               |------------------->
            List<double> result = new List<double>();
            for (var y = InsertPoint.Y;
                    y <= (InsertPoint.Y + height / 2 - Bar.Cover);
                    y += HorizontalBarsSpacing)
            {
                if (y == MeshContainer.Focus.Y) continue;
                result.Add(y);
            }
            // Middle
            result.Add(InsertPoint.Y);
            //<----------------|
            for (var y = InsertPoint.Y;
                    y >= (InsertPoint.Y - height / 2 + Bar.Cover);
                    y -= HorizontalBarsSpacing)
            {
                if (y == InsertPoint.Y) continue;
                result.Add(y);
            }
            result.Sort();
            return result;
        }
        #endregion

        public override int ZIndex
        {
            get
            {
                return (int)ZOrder.Reinforcement;
            }
            set
            {
                base.ZIndex = value;
            }
        }
    }
}
