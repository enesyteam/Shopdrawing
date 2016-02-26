using System;
using System.ComponentModel;
using System.Windows;
using System.Collections.Generic;
using DynamicGeometry;
using Shopdrawing.Settings;

namespace Shopdrawing.Reinforcement
{

    public partial class Bar : CompositeFigure
    {
        private Transform _transform = Transform.Normal;
        public Transform Transform
        {
            get { return _transform; }
            set 
            { 
                _transform = value;
                RecreateChildren();
            }
        }
        private ViewDirection _viewDirection = ViewDirection.Front;
        public ViewDirection ViewDirection
        {
            get { return _viewDirection; }
            set 
            {
                if (value == _viewDirection) return;
                _viewDirection = value;
                RecreateChildren();
            }
        }

        private double _rotate = 0;
        public double Rotate
        {
            get { return _rotate; }
            set { 
            if(_rotate == value) return;
            _rotate = value;
            RecreateChildren();
            }
        }
        private BarShape2D _shape = BarShape2D.Shape00;
        public BarShape2D ShapeCode
        {
            get { return _shape; }
            set 
            { 
                _shape = value;
                Recreate();
            }
        }
        private Point _insertPoint = new Point(0, 0);
        public Point InsertPoint
        {
            get { return _insertPoint; }
            set
            {
                if (_insertPoint == value) return;
                _insertPoint = value;
            }
        }
        private BarStyle _barStyle = BarStyle.SingleLine;
        public BarStyle BarStyle { get { return _barStyle; } set { _barStyle = value; } }

        private Dimensions _dimensions = null;
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Dimensions Dimensions 
        {
            get { return _dimensions; }
            set {
                if (_dimensions == value) return;
                _dimensions = value;
                RecreateChildren();
            }
        }

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

        private double _step;

        private double _diameter;
        private double _length;
        private double _unitWeight;
        private double _quantities;
        private int _count;

        /// <summary>
        /// Đường kính thép
        /// </summary>
        [PropertyGridVisible]
        [PropertyGridName("Step")]
        public double Step
        {
            get { return _step; }
            set
            {
                if (value < 0)
                    throw new Exception("Value must grater than or equal 0!");
                _step = value;
            }
        }

        /// <summary>
        /// Lớp bê tông bảo vệ
        /// </summary>
        public double Cover { get; set; }

        #region Public Properties
        public string FullName
        {
            get { return Name + "-" + Diameter; }
        }
        /// <summary>
        /// Đường kính thép
        /// </summary>
        [PropertyGridVisible]
        [PropertyGridName("Diameter")]
        public double Diameter
        {
            get { return _diameter; }
            set
            {
                if (value <= 0)
                    throw new Exception("Value must grater than 0!");
                _diameter = value;
                RecreateChildren();
            }
        }
        /// <summary>
        /// Chiều dài thanh thép
        /// </summary>
        public double Length
        {
            get 
            {
                if (Children != null)
                {
                    return BarLength.GetLength(this);
                }
                return 0;
            }
            set
            {
                if (value <= 0)
                    throw new Exception("Value must grater than 0!");
                _length = value;
            }
        }
        /// <summary>
        /// Trọng lượng đơn vị thanh thép
        /// </summary>
        public double UnitWeight
        {
            get { return _unitWeight; }
            set
            {
                if (value <= 0)
                    throw new Exception("Value must grater than 0!");
                _unitWeight = value;
            }
        }
        /// <summary>
        /// Khối lượng thanh thép
        /// </summary>
        public double Quantities
        {
            get { return _quantities; }
            set
            {
                if (value <= 0)
                    throw new Exception("Value must grater than 0!");
                _quantities = value;
            }
        }
        /// <summary>
        /// Số lượng thanh thép
        /// </summary>
        public int Count
        {
            get { return _count; }
            set
            {
                if (value <= 0)
                    throw new Exception("Value must grater than 0!");
                _count = value;
            }
        }

        #endregion

        #region Constructors
        public Bar()
        {
        }
        public Bar(Drawing drawing) : this()
        {
            Drawing = drawing;
        }

        public void OnBarDimensionsChanged()
        {
            BarObservableCollection<Dimensions> dimension = new BarObservableCollection<Dimensions>();
            dimension.Add(this.Dimensions);
            dimension.ChildElementPropertyChanged += new
            BarObservableCollection<Dimensions>.ChildElementPropertyChangedEventHandler(dimension_DimensionsChanged);
        }

        void dimension_DimensionsChanged(BarObservableCollection<Dimensions>.ChildElementPropertyChangedEventArgs e)
        {
            RecreateChildren();
        }

        public Bar(double length)
        {
            Length = length;
        }
        #endregion

        #region Public voids
        public double GetQuantities()
        {
            return Count * Length * UnitWeight;
        }

        /// <summary>
        /// Vẽ lại thanh trong trường hợp hình dạng thay đổi
        /// </summary>
        public void Recreate()
        {
            if (!IsValidDimensions()) return;

            foreach (IFigure f in Children)
            {
                f.OnRemovingFromCanvas(Drawing.Canvas);
            }
            Children.Clear();
            CreateBar();

            foreach (IFigure f in Children)
            {
                f.OnAddingToCanvas(Drawing.Canvas);
            }
        }

        /// <summary>
        /// Trong trường hợp hình dạng không thay đổi
        /// </summary>
        public void RecreateChildren()
        {
            if (!IsValidDimensions()) return;
            foreach (var c in Children)
            {
                if (c is IBarShape)
                {
                    var bs = c as IBarShape;
                    bs.Rotate = this.Rotate;
                    bs.Dimensions = this.Dimensions;
                    bs.Diameter = this.Diameter;
                    bs.ViewDirection = this.ViewDirection;
                    bs.Transform = this.Transform;
                    bs.ReCreate();
                }
            }
        }

        /// <summary>
        /// Kiểm tra Dimensions có hợp lệ không và đưa ra thông báo
        /// </summary>
        /// <returns></returns>
        protected bool IsValidDimensions()
        {
            if (!ValidationDimensions.Check(this))
            {
                Drawing.RaiseStatusNotification(ValidationDimensions.GetNotifycation(this));
                return false;
            }
            return true;
        }

        #endregion

        #region Override
        public override string ToString()
        {
            return "Bar";
        }
        public override IFigure HitTest(Point point)
        {
            //double epsilon = ToLogical(Shape.StrokeThickness / 2 + DynamicGeometry.Math.CursorTolerance);
            if (DynamicGeometry.Math.IsPointOnPolygonalChain(Dependencies.ToPoints(), point, 5, false))
            {
                return this;
            }
            return null;
        }
        #endregion
    }
}
