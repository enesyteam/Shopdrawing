using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using DynamicGeometry;
using Shopdrawing.Reinforcement;
using Shopdrawing.Settings;
using Shopdrawing.Core.Utilities;


namespace Shopdrawing.Structures
{
    public class MainAbutmentSection : CompositeFigure
    {
        #region Properties
        [Category("General")]
        [Description("Góc chéo của mố trên mặt bằng so với tim tuyến")]
        [DisplayName("Skew Angle")]
        public double SkewAngle { get; set; }

        #region Bệ mố
        private double _pileCapLength = 600;
        [Category("Pile Cap")]
        [Description("Bề rộng bệ mố theo phương dọc cầu")]
        [DisplayName("L")]
        public double PileCapLength 
        {
            get { return _pileCapLength; }
            set
            {
                if (value <= 0)
                {
                    Drawing.RaiseStatusNotification("Giá trị nhập không đúng!");
                    return;
                }
                _pileCapLength = value;
                RedrawBound();
            }
        }
        private double _pileCapFrontLength = 200;
        [Category("Pile Cap")]
        [Description("Khoảng cách từ mép trước bệ đến mép tường thân")]
        [DisplayName("L1")]
        public double PileCapFrontLength
        {
            get { return _pileCapFrontLength; }
            set
            {
                if (value <= 0)
                {
                    Drawing.RaiseStatusNotification("Giá trị nhập không đúng!");
                    return;
                }
                _pileCapFrontLength = value;
                RedrawBound();
            }
        }
        [Category("Pile Cap")]
        [Description("Khoảng cách từ mép sau bệ đến mép tường thân")]
        [DisplayName("L2")]
        public double PileCapBackLength
        {
            get { return PileCapLength - BodyDeep -PileCapFrontLength; }
            set{}
        }
        private double _pileCapWidth = 1100;
        [Category("Pile Cap")]
        [Description("Bề rộng bệ mố theo phương ngang cầu")]
        [DisplayName("W")]
        public double PileCapWidth
        {
            get { return _pileCapWidth; }
            set
            {
                if (value <= 0)
                {
                    Drawing.RaiseStatusNotification("Giá trị nhập không đúng!");
                    return;
                }
                _pileCapWidth = value;
                RedrawBound();
            }
        }
        private double _pileCapDeep = 200;
        [Category("Pile Cap")]
        [Description("Chiều dày bệ mố")]
        [DisplayName("D")]
        public double PileCapDeep
        {
            get { return _pileCapDeep; }
            set
            {
                if (value <= 0)
                {
                    Drawing.RaiseStatusNotification("Giá trị nhập không đúng!");
                    return;
                }
                _pileCapDeep = value;
                RedrawBound();
            }
        }
        #endregion

        #region Thân mố
        private double _bodyDeep = 140;
        [Category("Body wall")]
        [Description("Chiều dày thân mố")]
        [DisplayName("D")]
        public double BodyDeep
        {
            get { return _bodyDeep; }
            set
            {
                if (value <= 0)
                {
                    Drawing.RaiseStatusNotification("Giá trị nhập không đúng!");
                    return;
                }
                _bodyDeep = value;
                RedrawBound();
            }
        }
        private double _bodyHeight = 500;
        [Category("Body wall")]
        [Description("Chiều cao thân mố")]
        [DisplayName("H")]
        public double BodyHeight
        {
            get { return _bodyHeight; }
            set
            {
                if (value <= 0)
                {
                    Drawing.RaiseStatusNotification("Giá trị nhập không đúng!");
                    return;
                }
                _bodyHeight = value;
                RedrawBound();
            }
        }
        #endregion

        #region Tường đỉnh mố
        private double _headWallHeight = 200;
        [Category("Head Wall")]
        [Description("Chiều cao tường đỉnh mố")]
        [DisplayName("H")]
        public double HeadWallHeight
        {
            get { return _headWallHeight; }
            set
            {
                if (value <= 0)
                {
                    Drawing.RaiseStatusNotification("Giá trị nhập không đúng!");
                    return;
                }
                _headWallHeight = value;
                RedrawBound();
            }
        }
        private double _headWallDeep = 40;
        [Category("Head Wall")]
        [Description("Bề dày tường đỉnh mố")]
        [DisplayName("D")]
        public double HeadWallDeep
        {
            get { return _headWallDeep; }
            set
            {
                if (value <= 0)
                {
                    Drawing.RaiseStatusNotification("Giá trị nhập không đúng!");
                    return;
                }
                _headWallDeep = value;
                RedrawBound();
            }
        }
        #endregion

        #region Mẫu đỡ bản quá độ
        private double _appBlockFromTop = 60;
        [Category("Approach Block")]
        [Description("Khoảng cách từ tường đỉnh đến mấu đỡ bản quá độ")]
        [DisplayName("H1")]
        public double AppBlockFromTop
        {
            get { return _appBlockFromTop; }
            set
            {
                if (value <= 0)
                {
                    Drawing.RaiseStatusNotification("Giá trị nhập không đúng!");
                    return;
                }
                _appBlockFromTop = value;
                RedrawBound();
            }
        }
        private double _appBlockWidth = 30;
        [Category("Approach Block")]
        [Description("Bề rộng mẫu đỡ bản quá độ")]
        [DisplayName("W")]
        public double AppBlockWidth
        {
            get { return _appBlockWidth; }
            set
            {
                if (value <= 0)
                {
                    Drawing.RaiseStatusNotification("Giá trị nhập không đúng!");
                    return;
                }
                _appBlockWidth = value;
                RedrawBound();
            }
        }
        private double _appBlockDeep = 30;
        [Category("Approach Block")]
        [Description("Chiều dày mẫu đỡ bản quá độ")]
        [DisplayName("D")]
        public double AppBlockDeep
        {
            get { return _appBlockDeep; }
            set
            {
                if (value <= 0)
                {
                    Drawing.RaiseStatusNotification("Giá trị nhập không đúng!");
                    return;
                }
                _appBlockDeep = value;
                RedrawBound();
            }
        }
        #endregion

        private Point _insertPoint = new Point(0, 0);
        public Point InsertPoint 
        {
            get { return _insertPoint; }
            set { _insertPoint = value; }
        }
        public int DependenciesCount
        {
            get { return Dependencies.Count; }
            set { }
        }

        private DynamicGeometry.Polyline _mainBound = null;
        [Browsable(false)]
        public DynamicGeometry.Polyline MainBound
        {
            get { return _mainBound; }
            set { _mainBound = value; }
        }
        #endregion

        #region Constructors
        public MainAbutmentSection()
        {
            DefaultSettings();
        }

        public MainAbutmentSection(DynamicGeometry.Drawing drawing)
            : this()
        {
            Drawing = drawing;
            CreateMainSection();
            Drawing.Figures.Add(this);
        }
        #endregion

        #region Default
        void DefaultSettings()
        {
            //PileCapLength = 500;
            //PileCapWidth = 1100;
            //PileCapDeep = 200;
        }
        #endregion

        #region Redraw
        public void RedrawBound()
        {

            this.Dependencies.Clear();
            if (MainBound == null) return;
            MainBound.Dependencies = GetBoundDependencies();
            MainBound.RecalculateAndUpdateVisual();
            //Recalculate();
            //UpdateVisual();
        }
        #endregion
        #region Tests
        [Category("ZTest")]
        [Description("Vẽ Dependencies")]
        public bool Test { get { return false; } set { DrawDependencies(); } }

        public void DrawDependencies()
        {
            foreach (IPoint p in this.Dependencies)
                Actions.Add(Drawing, Factory.CreateFreePoint(Drawing, p.Coordinates));
        }
        #endregion

        #region Public Methods
        public void CreateMainSection()
        {
            this.Dependencies.Clear();
            MainBound = Factory.CreatePolyline(Drawing, GetBoundDependencies());
            MainBound.Style = DefaultStyles.StructureLineStyle;
            
            this.Children.Add(MainBound);
            
        }
        /// <summary>
        /// Tính toán các điểm Dependencies của Bound
        /// </summary>
        protected List<IFigure> GetBoundDependencies()
        {
            //this.Dependencies.Clear();
            var boundDependencies = new List<IFigure>();

            #region Bệ mố
            // bệ
            System.Windows.Point p1 = this.InsertPoint;
            System.Windows.Point p2 = PointsHelper.Offset(p1, 0, PileCapDeep);
            // thân
            System.Windows.Point p3 = PointsHelper.Offset(p2, PileCapBackLength, 0);
            System.Windows.Point p4 = PointsHelper.Offset(p3, 0, BodyHeight + HeadWallHeight - AppBlockFromTop - 2*AppBlockDeep);
            System.Windows.Point p5 = PointsHelper.Offset(p4, - AppBlockWidth, AppBlockDeep);
            System.Windows.Point p6 = PointsHelper.Offset(p5, 0, AppBlockDeep);
            System.Windows.Point p7 = PointsHelper.Offset(p6, AppBlockWidth, 0);
            System.Windows.Point p8 = PointsHelper.Offset(p7, 0, AppBlockFromTop);
            System.Windows.Point p9 = PointsHelper.Offset(p8, HeadWallDeep, 0);
            System.Windows.Point p10 = PointsHelper.Offset(p9, 0, -HeadWallHeight);
            System.Windows.Point p11 = PointsHelper.Offset(p10, BodyDeep - HeadWallDeep, 0);
            System.Windows.Point p12 = PointsHelper.Offset(p11, 0, -BodyHeight);
            System.Windows.Point p13 = PointsHelper.Offset(p12, PileCapFrontLength, 0);
            System.Windows.Point p14 = PointsHelper.Offset(p13, 0, -PileCapDeep);

            var pp1 = Factory.CreateFreePoint(Drawing, p1);
            var pp2 = Factory.CreateFreePoint(Drawing, p2);
            var pp3 = Factory.CreateFreePoint(Drawing, p3);
            var pp4 = Factory.CreateFreePoint(Drawing, p4);
            var pp5 = Factory.CreateFreePoint(Drawing, p5);
            var pp6 = Factory.CreateFreePoint(Drawing, p6);
            var pp7 = Factory.CreateFreePoint(Drawing, p7);
            var pp8 = Factory.CreateFreePoint(Drawing, p8);
            var pp9 = Factory.CreateFreePoint(Drawing, p9);
            var pp10 = Factory.CreateFreePoint(Drawing, p10);
            var pp11 = Factory.CreateFreePoint(Drawing, p11);
            var pp12 = Factory.CreateFreePoint(Drawing, p12);
            var pp13 = Factory.CreateFreePoint(Drawing, p13);
            var pp14 = Factory.CreateFreePoint(Drawing, p14);
            #endregion
            

            boundDependencies.Add(pp1, pp2, pp3, pp4, pp5, pp6, pp7, pp8, pp9, pp10, pp11, pp12, pp13, pp14, pp1);
            foreach (IPoint p in boundDependencies)
            {
                this.Dependencies.Add(p);
            }
            return boundDependencies;

        }
        #endregion

        #region override
        public override IFigure HitTest(Point point)
        {
            List<Point> abutBoundPoints = new List<Point>();
            foreach (IPoint p in MainBound.Dependencies)
            {
                abutBoundPoints.Add(p.Coordinates);
            }
            bool isInside = abutBoundPoints.IsPointInPolygon(point);
            return isInside ? this : null;
        }
        public override IFigure MouseHover(Point point)
        {
            List<Point> abutBoundPoints = new List<Point>();
            foreach (IPoint p in MainBound.Dependencies)
            {
                abutBoundPoints.Add(p.Coordinates);
            }
            bool isInside = abutBoundPoints.IsPointInPolygon(point);
            return isInside ? this : null;
        }
        public override void Recalculate()
        {
            IPoint point = (IPoint)MainBound.Dependencies[0];
            point = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(point.Coordinates, 0, 0));
            InsertPoint = PointsHelper.Offset(point.Coordinates, 0, 0);
            base.Recalculate();
        }
        #endregion

    }
}
