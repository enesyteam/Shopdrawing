using System;
using DynamicGeometry;
using System.ComponentModel;
using System.Windows;
using System.Collections.Generic;
using Shopdrawing.Utilities;
using Shopdrawing.Settings;

namespace Shopdrawing.Reinforcement
{
    public interface IBarShape : IEquatable<IBarShape>
    {
        IList<IFigure> Parts { get; set; }
        ViewDirection ViewDirection { get; set; }
        BarStyle BarStyle { get; set; }
        Dimensions Dimensions { get; set; }
        Point InsertPoint { get; set; }
        double Diameter { get; set; }
        double Length { get; set; }
        DrawType DrawType { get; set; }
        Transform Transform { get; set; }
        double Rotate { get; set; }
        void Create();
        void ReDraw();
        void ReCreate();
    }

    public enum DrawType
    { 
        /// <summary>
        /// Dạng đường có độ rộng mặc định
        /// </summary>
        RegularLine = 0,
        /// <summary>
        /// Bề rộng nét bằng đường kính thanh
        /// </summary>
        LineWeight = 1,
        /// <summary>
        /// Dạng không fill
        /// </summary>
        NotFilled = 2
    }

    public class BarShapeBase : CompositeFigure, IBarShape
    {
        public virtual IList<IFigure> Parts { get; set; }
        private ViewDirection _viewDirection = ViewDirection.Front;
        public virtual ViewDirection ViewDirection
        {
            get { return _viewDirection; }
            set {
                if (_viewDirection == value) return;
                _viewDirection = value;
            }
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual Dimensions Dimensions { get; set; }
        public virtual double Length { get; set; }
        private DrawType _drawType = DrawType.RegularLine;
        public DrawType DrawType
        {
            get { return _drawType; }
            set {
                if (_drawType == value) return;
                _drawType = value;
            }
        }
        private Point _insertPoint = new Point(0,0);
        public Point InsertPoint
        {
            get { return _insertPoint; }
            set {
                if (_insertPoint == value) return;
                _insertPoint = value;
            }
        }
        private double _diameter = 1.0;
        public double Diameter
        {
            get { return _diameter; }
            set {
                if (_diameter == value) return;
                _diameter = value;
            }
        }
        private Transform _transform = Transform.Normal;
        public Transform Transform
        {
            get { return _transform; }
            set {
                if (_transform == value) return;
                _transform = value;
            }
        }
        private double _rotate = 0;
        public double Rotate
        {
            get { return _rotate; }
            set {
                if (_rotate == value) return;
                _rotate = value; 
            }
        }
        public double RotateRAD
        {
            get { return Rotate * System.Math.PI / 180; }
        }
        private BarStyle _barStyle = BarStyle.SingleLine;
        public BarStyle BarStyle { get { return _barStyle; } set { _barStyle = value; } }
        public virtual void Create() { }
        public virtual void ReDraw() { }
        public virtual void ReCreate() { }

        public override IFigureStyle Style
        {
            get
            {
                return DefaultStyles.BarStyle;
            }
            set
            {
                base.Style = value;
            }
        }

        public bool Equals(IBarShape other)
        {
            return object.ReferenceEquals(this, other);
        }

        public IList<IFigure> GetCrossPartDependencies()
        {
            switch (this.ViewDirection)
            {
                case Reinforcement.ViewDirection.Cross:
                    {
                        List<IFigure> result = new List<IFigure>();
                        result.Add(Factory.CreateFreePoint(this.Drawing, InsertPoint));
                        result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(InsertPoint, 0.5 * Diameter, 0)));
                        return result;
                    }
                default: return null;
            }
        }
    }
}
