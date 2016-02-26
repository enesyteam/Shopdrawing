/*
 * BoxCulvertSection Class
 * Copyright (c) 2015: congnvc@gmail.com
 */

#define DEBUG

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using DynamicGeometry;
using Shopdrawing.Reinforcement;
using Shopdrawing.Settings;
using Shopdrawing.Utilities;
using System.Windows.Controls.WpfPropertyGrid;
using System.Reflection;

using netDxf;
using netDxf.Header;
using System.Windows.Media;
using Shopdrawing.Dimensions;
using Shopdrawing.Core.Utilities;
using System.Windows.Controls.WpfPropertyGrid;

namespace Shopdrawing.Structures.Culvert
{
    /// <summary>
    /// Mặt cắt ngang cống hộp
    /// </summary>
    public partial class BoxCulvertSection : CompositeFigure, ICanRotate, IShapeWithInterior
    {
        public double Area
        {
            get
            {
                return CulvertBound.Dependencies.ToPoints().Area();
            }
        }
        /// <summary>
        /// Đơn vị độ dài hiện tại
        /// </summary>
        public static Units CurrentUnit = Units.mm;
        #region Properties
        /// <summary>
        /// Parent Culvert of this Section
        /// </summary>
        [Category("Parent culvert")]
        [Description("Specify parent culvert of this section")]
        [DisplayName("Parent culvert")]
        [Browsable(false)]
        public Boxculvert ParentCulver { get; set; }

        [Browsable(false)]
        public Shopdrawing.Reinforcement.Transform Transform { get; set; }

        private NumberOfCells _cells = NumberOfCells.OneCells;

        /// <summary>
        /// Tổng chiều cao mặt cắt ngang
        /// </summary>
        public double TotalHeight
        {
            get { return H + TopThickness + BottomThickness; }
        }
        /// <summary>
        /// Tổng bề rộng mặt cắt ngang
        /// </summary>
        public double TotalWidth
        {
            get
            {
                return B2Left + ExternalWallThickness + B * (NumberOfCellsToInt(NumberOfCells))
                    + (NumberOfCellsToInt(NumberOfCells) - 1) * InternalWallThickness
                    + ExternalWallThickness + B2Right;
            }
        }
        /// <summary>
        /// Số Cells trên mặt cắt ngang cống
        /// </summary>
        [Category("General")]
        [Description("Số Cells trên mặt cắt ngang cống")]
        [DisplayName("Cells")]
        [DefaultValue(NumberOfCells.FiveCells)]
        public NumberOfCells NumberOfCells
        {
            get { return _cells; }
            set
            {
                if (value != _cells)
                {
                    _cells = value;
                    ReDrawWidthCellsChanged(value);
                    RaisePropertyChanged("NumberOfCells");
                }
            }
        }

        private double _b = 100;
        /// <summary>
        /// Bề rộng mỗi Cell
        /// </summary>
        [PropertyGridVisible]
        //[Domain(0, 8000)] // Có thể giới hạn bề rộng cống hộp
        [Category("General")]
        [Description("Specify width of cells of culvert section")]
        [DisplayName("Span")]
        public double B
        {
            get { return _b; }
            set
            {
                if (value != _b)
                {
                    if (value <= 0)
                    {
                        drawing.RaiseStatusNotification("Value must be > 0!");
                    }
                    else
                    {
                        _b = value;
                        RaisePropertyChanged("B");
                        //ReDraw();
                        //RedrawDims(this);
                        Recalculate();
                    }
                }

            }
        }

        private double _h = 100;
        /// <summary>
        /// Chiều cao mỗi Cell
        /// </summary>
        [Category("General")]
        [Description("Specify height of cells of culvert section")]
        [DisplayName("Rise")]
        public double H
        {
            get { return _h; }
            set
            {
                if (value != _h)
                {
                    if (value <= 0)
                    {
                        drawing.RaiseStatusNotification("Value must be > 0!");
                    }
                    else
                    {
                        _h = value;
                        RaisePropertyChanged("H");
                        //ReDraw();
                        //RedrawDims(this);
                        Recalculate();
                    }
                }
            }
        }
        private double _topThickness = 30;
        /// <summary>
        /// Chiều dày bản trên
        /// </summary>
        [Category("Slab Thickness")]
        [Description("Specify top slab thickness")]
        [DisplayName("Top")]
        public double TopThickness
        {
            get { return _topThickness; }
            set
            {
                if (value != _topThickness)
                {
                    if (value <= 0)
                    {
                        drawing.RaiseStatusNotification("Value must be > 0!");
                    }
                    else
                    {
                        _topThickness = value;
                        RedrawBound();
                    }
                }
            }
        }
        private double _bottomThickness = 30;
        /// <summary>
        /// Chiều dày bản dưới
        /// </summary>
        [Category("Slab Thickness")]
        [Description("Specify bottom slab thickness")]
        [DisplayName("Bottom")]
        public double BottomThickness
        {
            get { return _bottomThickness; }
            set
            {
                if (value != _bottomThickness)
                {
                    if (value <= 0)
                    {
                        drawing.RaiseStatusNotification("Value must be > 0!");
                    }
                    else
                    {
                        _bottomThickness = value;
                        RedrawBound();
                    }
                }
            }
        }
        private double _internalWallThickness = 25;
        /// <summary>
        /// Chiều dày thành trong
        /// </summary>
        [Category("Wall Thickness")]
        [Description("Specify bottom slab thickness")]
        [DisplayName("Internal T")]
        public double InternalWallThickness
        {
            get { return _internalWallThickness; }
            set
            {
                if (value != _internalWallThickness)
                {
                    if (value <= 0)
                    {
                        drawing.RaiseStatusNotification("Value must be > 0!");
                    }
                    else
                    {
                        _internalWallThickness = value;
                        ReDraw();
                    }
                }
            }
        }
        private double _externalWallThickness = 30;
        /// <summary>
        /// Chiều dày thành ngoài
        /// </summary>
        [Category("Wall Thickness")]
        [Description("Specify bottom slab thickness")]
        [DisplayName("External T")]
        public double ExternalWallThickness
        {
            get { return _externalWallThickness; }
            set
            {
                if (value != _externalWallThickness)
                {
                    if (value <= 0)
                    {
                        drawing.RaiseStatusNotification("Value must be > 0!");
                    }
                    else
                    {
                        _externalWallThickness = value;
                        ReDraw();
                    }
                }
            }
        }
        private bool _hasApproachSlabOnTheLeft = true;
        /// <summary>
        /// Có bản quá độ phía bên trái
        /// </summary>
        [Category("Approach slab")]
        [Description("Specify this culvert has or hasn't approach on the left")]
        [DisplayName("Slab on left")]
        //[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        //[ReadOnly(false)]
        public bool HasApproachSlabOnTheLeft
        {
            get { return _hasApproachSlabOnTheLeft; }
            set
            {
                _hasApproachSlabOnTheLeft = value;
                RedrawBound();

                //PropertyItem prop = propertyGrid.Properties["Name"];
                //if (prop != null)
                //{
                //    //prop.IsReadOnly = !prop.IsReadOnly;
                //    prop.IsBrowsable = !prop.IsBrowsable;
                //}
                //PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this.GetType())["H1Left"];
                //ReadOnlyAttribute attribute = (ReadOnlyAttribute)
                //                              descriptor.Attributes[typeof(ReadOnlyAttribute)];
                //FieldInfo fieldToChange = attribute.GetType().GetField("isReadOnly",
                //                                 System.Reflection.BindingFlags.NonPublic |
                //                                 System.Reflection.BindingFlags.Instance);
                //fieldToChange.SetValue(attribute, _hasApproachSlabOnTheLeft != false);
            }

        }
        private bool _hasApproachSlabOnTheRight = true;
        /// <summary>
        /// Có bản quá độ phía bên trái
        /// </summary>
        [Category("Approach slab")]
        [Description("Specify this culvert has or hasn't approach on the left")]
        [DisplayName("Slab on right")]
        public bool HasApproachSlabOnTheRight
        {
            get { return _hasApproachSlabOnTheRight; }
            set
            {
                _hasApproachSlabOnTheRight = value;
                RedrawBound();
            }
        }

        #region Các kích thước chi tiết của mẫu đỡ bản quá độ
        private double _h1Left = 30;
        private double _h2Left = 30;
        private double _h3Left = 30;
        private double _b1Left = 30;
        [Category("Approach slab")]
        [Description("Specify heigh of approach slab holder on the left")]
        [DisplayName("H1L")]
        //[ReadOnly(true)]
        public double H1Left
        {
            get { return _h1Left; }
            set
            {
                if (value < 0 || value > TotalHeight - H2Left - H3Left)
                {
                    drawing.RaiseStatusNotification("Giá trị gán không đúng!");
                }
                else
                {
                    _h1Left = value;
                    RedrawBound();
                }
            }
        }
        [Category("Approach slab")]
        [Description("Specify heigh of approach slab holder on the left")]
        [DisplayName("H2L")]
        public double H2Left
        {
            get { return _h2Left; }
            set
            {
                if (value < 0 || value > TotalHeight - H1Left - H3Left)
                {
                    drawing.RaiseStatusNotification("Giá trị gán không đúng!");
                }
                else
                {
                    _h2Left = value;
                    RedrawBound();
                }
            }
        }
        [Category("Approach slab")]
        [Description("Specify heigh of approach slab holder on the left")]
        [DisplayName("H3L")]
        public double H3Left
        {
            get { return _h3Left; }
            set
            {
                if (value < 0 || value > TotalHeight - H1Left - H2Left)
                {
                    drawing.RaiseStatusNotification("Giá trị gán không đúng!");

                }
                else
                {
                    _h3Left = value;
                    RedrawBound();
                }
            }
        }
        [Category("Approach slab")]
        [Description("Specify width of approach slab holder on the left")]
        [DisplayName("B1L")]
        public double B1Left
        {
            get { return _b1Left; }
            set
            {
                _b1Left = value;
                RedrawBound();
            }
        }

        private double _h1Right = 30;
        private double _h2Right = 30;
        private double _h3Right = 30;
        private double _b1Right = 30;
        [Category("Approach slab")]
        [Description("Specify heigh of approach slab holder on the left")]
        [DisplayName("H1 (Right)")]
        public double H1Right
        {
            get { return _h1Right; }
            set
            {
                if (value < 0 || value > TotalHeight - H2Right - H3Right)
                {
                    drawing.RaiseStatusNotification("Giá trị gán không đúng!");

                }
                else
                {
                    _h1Right = value;
                    RedrawBound();
                }
            }
        }
        [Category("Approach slab")]
        [Description("Specify heigh of approach slab holder on the left")]
        [DisplayName("H2 (Right)")]
        public double H2Right
        {
            get { return _h2Right; }
            set
            {
                if (value < 0 || value > TotalHeight - H1Right - H3Right)
                {
                    drawing.RaiseStatusNotification("Giá trị gán không đúng!");

                }
                else
                {
                    _h2Right = value;
                    RedrawBound();
                }
            }
        }
        [Category("Approach slab")]
        [Description("Specify heigh of approach slab holder on the left")]
        [DisplayName("H3 (Right)")]
        public double H3Right
        {
            get { return _h3Right; }
            set
            {
                if (value < 0 || value > TotalHeight - H1Right - H2Right)
                {
                    drawing.RaiseStatusNotification("Giá trị gán không đúng!");

                }
                else
                {
                    _h3Right = value;
                    RedrawBound();
                }
            }
        }
        [Category("Approach slab")]
        [Description("Specify heigh of approach slab holder on the left")]
        [DisplayName("B1 (Right)")]
        public double B1Right
        {
            get { return _b1Right; }
            set
            {
                _b1Right = value;
                RedrawBound();
            }
        }
        public Point _insertPoint = new Point(0, 0);
        [Browsable(true)]
        public virtual Point InsertPoint
        {
            get { return _insertPoint; }
            set 
            {
                _insertPoint = value;
                if(BasePoint != null) BasePoint.MoveTo(value);
                RaisePropertyChanged("InsertPoint");
                ReDraw();
            }
        }
        //public event PropertyChangedEventHandler PropertyChanged;
        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
        #endregion

        #region Haunch
        private double _topChamfer = 15;
        [Category("Haunch")]
        [Description("Specify chamfer distance on top of cell")]
        [DisplayName("Top Haunch")]
        public double TopChamfer
        {
            get { return _topChamfer; }
            set
            {
                if (value < 0)
                    throw new Exception("Value must be non negative!");
                else if (value > H)
                    throw new Exception("Value must be smaller than cell height!");
                _topChamfer = value;
                if (CellList == null) return;
                for (int i = 0; i < CellList.Count; i++)
                {
                    CellList[i].Dependencies = GetCellDependencies(i);
                    CellList[i].RecalculateAndUpdateVisual();
                }
            }
        }
        private double _bottomChamfer = 15;
        [Category("Haunch")]
        [Description("Specify chamfer distance on top of cell")]
        [DisplayName("Bottom Haunch")]
        public double BottomChamfer
        {
            get { return _bottomChamfer; }
            set
            {
                if (value < 0)
                    throw new Exception("Value must be non negative!");
                else if (value > H)
                    throw new Exception("Value must be smaller than cell height!");
                _bottomChamfer = value;
                if (CellList == null) return;
                for (int i = 0; i < CellList.Count; i++)
                {
                    CellList[i].Dependencies = GetCellDependencies(i);
                    CellList[i].RecalculateAndUpdateVisual();
                }
            }
        }

        [Browsable(false)]
        public bool IsChamferAtTop { get { return TopChamfer > 0; } }
        [Browsable(false)]
        public bool IsChamferAtBottom { get { return BottomChamfer > 0; } }

        #endregion
        #region Mở rộng bệ móng
        private double _h4Left = 0;
        [Category("Foundation expand")]
        [Description("Specify heigh of foundation expand")]
        [DisplayName("H4 (Left)")]
        public double H4Left
        {
            get { return _h4Left; }
            set
            {
                if (value < 0)
                    drawing.RaiseStatusNotification("Value must be non negative!");
                _h4Left = value;
                RedrawBound();
            }
        }
        private double _h4Right = 0;
        [Category("Foundation expand")]
        [Description("Specify heigh of foundation expand")]
        [DisplayName("H4 (Right)")]
        public double H4Right
        {
            get { return _h4Right; }
            set
            {
                if (value < 0)
                    drawing.RaiseStatusNotification("Value must be non negative!");
                _h4Right = value;
                RedrawBound();
            }
        }
        private double _b2Left = 0;
        [Category("Foundation expand")]
        [Description("Specify width of foundation expand")]
        [DisplayName("B2 (Left)")]
        public double B2Left
        {
            get { return _b2Left; }
            set
            {
                if (value < 0)
                    drawing.RaiseStatusNotification("Value must be non negative!");
                _b2Left = value;
                RedrawBound();
            }
        }
        private double _b2Right = 0;
        [Category("Foundation expand")]
        [Description("Specify width of foundation expand")]
        [DisplayName("B2 (Right)")]
        public double B2Right
        {
            get { return _b2Right; }
            set
            {
                if (value < 0)
                    drawing.RaiseStatusNotification("Value must be non negative!");
                _b2Right = value;
                RedrawBound();
            }
        }

        bool _isBottomExpandOnLeft = true;
        [Category("Foundation expand")]
        [Description("Specify width of foundation expand")]
        [DisplayName("Left Expand")]
        public bool IsBottomExpandOnLeft
        {
            get
            {
                return _isBottomExpandOnLeft;
            }
            set
            {
                _isBottomExpandOnLeft = value;
                RaisePropertyChanged("IsBottomExpandOnLeft");
                RedrawBound();
            }
        }
        private bool _isBottomExpandOnRight = true;
        [Category("Foundation expand")]
        [Description("Specify width of foundation expand")]
        [DisplayName("Right Expand")]
        public bool IsBottomExpandOnRight
        {
            get
            {
                return _isBottomExpandOnRight;
            }
            set
            {
                _isBottomExpandOnRight = value;
                RedrawBound();
            }
        }
        #endregion

        #endregion

        #region Constructors
        public BoxCulvertSection()
        {
            GetDefaultSettingsSection();
        }

        public BoxCulvertSection(DynamicGeometry.Drawing drawing)
            : this()
        {
            Drawing = drawing;
            DrawSection();
            Drawing.Figures.Add(this);
        }

        public BoxCulvertSection(DynamicGeometry.Drawing drawing, Point insertpoint)
            : this()
        {
            Drawing = drawing;
            InsertPoint = insertpoint;
            DrawSection();
            Drawing.Figures.Add(this);
        }
        #endregion
        double _dimScale = 100;
        [PropertyEditor(typeof(DoubleEditor))]
        public double DimScale 
        {
            get { return _dimScale; }
            set 
            {
                _dimScale = value;
                RaisePropertyChanged("DimScale");
                foreach (LinearDimension dim in this.Dims)
                {
                    dim.Dimscale = value;
                }
            }
        }
        public void CreateDimss(BoxCulvertSection bs)
        {
            var p1 = Shopdrawing.Core.Utilities.PointsHelper.Offset(bs.BasePoint.Coordinates, 0, bs.H / 2);
            var p2 = Shopdrawing.Core.Utilities.PointsHelper.Offset(p1, bs.B, 0);
            d1 = new LinearDimension(Drawing, p1, p2, p1);
            //Dims.Add(dim);
            d1.Dimpost = "B=";
            d1.Dimscale = this.DimScale;
            this.Children.Add(d1);
            this.Dependencies.Add(d1);
            d1.OnAddingToCanvas(Drawing.Canvas);
            d1.UpdateVisual();
            Dims.Add(d1);
        }

        public void RedrawDims(BoxCulvertSection bs)
        {
            if (d1 != null)
            {
                var p1 = Shopdrawing.Core.Utilities.PointsHelper.Offset(bs.BasePoint.Coordinates, 0, bs.H / 2);
                var p2 = Shopdrawing.Core.Utilities.PointsHelper.Offset(p1, bs.B, 0);
                d1.XLine1Point = p1;
                d1.XLine2Point = p2;
                d1.DimLinePoint = p1;
                d1.Dimscale = this.DimScale;
                d1.UpdateVisual();
            }
        }
        #region Public Methods

#if DEBUG
        /// <summary>
        /// Test
        /// </summary>
        public void GetDefaultSettingsSection()
        {
            this.HasApproachSlabOnTheLeft = true;
            this.HasApproachSlabOnTheRight = true;
            this.B2Left = BottomThickness / 2;
            this.B2Right = BottomThickness / 2;
            this.H4Left = BottomThickness;
            this.H4Right = BottomThickness;

            this.LineStyle = Settings.DefaultStyles.StructureLineStyle;
        }
#endif


        #endregion

        #region override
        public override string ToString()
        {
            return "Cross Sectiossssn";
        }
        public override IFigure HitTest(Point point)
        {
            //if (AjustCellPoint != null)
            //{
            //    var p = AjustCellPoint.HitTest(point);
            //    if (p != null) return p;
            //}

            //bool isInside = this.DependencyPoints.IsPointInPolygon(point);
            //List<Point> culvertBoundPoints = new List<Point>();
            //foreach (IPoint p in CulvertBound.Dependencies)
            //{
            //    culvertBoundPoints.Add(p.Coordinates);
            //}
            bool isInside = CulvertBound.Dependencies.ToPoints().IsPointInPolygon(point);
            return isInside ? this : null;
        }

        #endregion

        [Category("ZTest")]
        public bool CreateBar
        {
            get { return false; }
            set { TestCreateBar(); }
        }
        public void TestCreateBar()
        {
            Bar b = new Bar(this.Drawing)
            {
                Dimensions = new Reinforcement.Dimensions() { A = 500, B = 100, C = 200},
                Diameter = 5,
                ViewDirection = ViewDirection.Front,
                Transform = Reinforcement.Transform.Rotate90CW,
                Style = DefaultStyles.BarStyle,
                ShapeCode = BarShape2D.Shape13,
                Rotate = 45,
                BarStyle = BarStyle.SingleLine,
                InsertPoint = this.InsertPoint,
                
            };
            b.CreateBar();
            Actions.Add(Drawing, b);     
        }

        #region Giải pháp thay thế cho việc Clone objects, set properties

        private List<Point> DependencyPoints { get; set; }
        public override void Recalculate()
        {
            if (BasePoint != null) InsertPoint = BasePoint.Coordinates;
        }


        private DynamicGeometry.Polyline _culvertBound = null;
        [Browsable(false)]
        public DynamicGeometry.Polyline CulvertBound
        {
            get { return _culvertBound; }
            set { _culvertBound = value; }
        }
        private List<DynamicGeometry.Polyline> _cellList = null;
        [Browsable(false)]
        public List<DynamicGeometry.Polyline> CellList
        {
            get { return _cellList; }
            set { _cellList = value; }
        }

        FreePoint _basepoint = null;
        public FreePoint BasePoint 
        {
            get { return _basepoint; }
            set 
            {
                _basepoint = value;
                RaisePropertyChanged("BasePoint");
                ReDraw();
            }
        }
        public void DrawSection()
        {
            // Tạo 1 điểm BasePoint
            BasePoint = Factory.CreateFreePoint(Drawing, InsertPoint);
            BasePoint.Visible = false;
            this.Children.Add(BasePoint);
            this.Dependencies.Add(BasePoint);

            CreateBound();
            CreateCells();
        }
        public override IFigure MouseHover(Point point)
        {
            bool isInside = CulvertBound.Dependencies.ToPoints().IsPointInPolygon(point);
            return isInside ? this : null;
        }
        /// <summary>
        /// Tạo bound
        /// </summary>
        private void CreateBound()
        {
            CulvertBound = Factory.CreatePolyline(Drawing, GetBoundDependencies());
            CulvertBound.Name = "Culvert bound";
            CulvertBound.Style = this.Selected ? DefaultStyles.StructureLineStyleSelected : DefaultStyles.StructureLineStyle;
            this.Dependencies.Add(CulvertBound);
            this.Children.Add(CulvertBound);
        }

        public LineStyle LineStyle { get; set; }
        public List<LinearDimension> Dims { get; set; }
        /// <summary>
        /// Tạo cells
        /// </summary>
        /// <returns></returns>
        private void CreateCells()
        {
            Dims = new List<LinearDimension>();
            // Số cells trên mặt cắt ngang
            int cellsNumber = NumberOfCellsToInt(this.NumberOfCells);
            CellList = new List<DynamicGeometry.Polyline>();

            for (int i = 0; i < cellsNumber; i++)
            {
                var cell = Factory.CreatePolyline(Drawing, GetCellDependencies(i));
                cell.Dependencies = GetCellDependencies(i);
                cell.Name = "Cell" + (i+1);
                cell.Style = DefaultStyles.StructureLineStyle;
                CellList.Add(cell);
                this.Children.Add(cell);
                this.Dependencies.Add(cell);
            }
            
        }
        /// <summary>
        /// Dim for B
        /// </summary>
        public LinearDimension d1 { get; set; }

        private void AdjustCells(NumberOfCells newCellValue)
        {
            //int currentCellsNumber = CellList.Count;
            int newValue = NumberOfCellsToInt(newCellValue);
            if (newValue == CellList.Count) return;
            if (newValue > CellList.Count)
            {
                // Thêm cells
                for (int i = CellList.Count; i < newValue; i++)
                {
                    var cell = Factory.CreatePolyline(Drawing, GetCellDependencies(i));
                    cell.Name = "Cell" + (i + 1);
                    cell.Style = DefaultStyles.StructureLineStyle;
                    CellList.Add(cell);
                    this.Children.Add(cell);
                    this.Dependencies.Add(cell);
                    cell.OnAddingToCanvas(Drawing.Canvas);
                    cell.Selected = this.Selected;
                }
            }
            else
            {

                // Remove cell
                for (int i = CellList.Count; i > newValue; i--)
                {
                    RemoveCell();
                }
            }

            if (CellList == null) return;
            for (int i = 0; i < CellList.Count; i++)
            {
                CellList[i].Dependencies = GetCellDependencies(i);
                CellList[i].RecalculateAndUpdateVisual();
            }

        }

        protected void RemoveCell()
        {
            var index = CellList.Count - 1;
            var cell = CellList[index];
            CellList.RemoveLast();
            Children.Remove(cell);
            
            if (Drawing != null)
            {
                cell.OnRemovingFromCanvas(Drawing.Canvas);
            }
            //foreach (IPoint p in cell.Dependencies)
            //    this.Dependencies.Remove(p);
        }

        // Trường hợp số cell không thay đổi
        private void ReDraw()
        {
            if (CellList == null) return;
            CulvertBound.Dependencies = GetBoundDependencies();
            CulvertBound.RecalculateAndUpdateVisual();
            for (int i = 0; i < CellList.Count; i++)
            {
                CellList[i].Dependencies = GetCellDependencies(i);
                CellList[i].UpdateVisual();
            }
            RedrawDims(this);
        }

        // Trường hợp số cell thay đổi
        private void ReDrawWidthCellsChanged(NumberOfCells newValue)
        {
            if (CellList == null) return;
            AdjustCells(newValue); // Thay đổi số Cell
            RedrawBound();
            RedrawDims(this);
        }
        
        private void RedrawBound()
        {
            if (CulvertBound == null) return;
            CulvertBound.Dependencies = GetBoundDependencies();
            CulvertBound.UpdateVisual();
        }
        /// <summary>
        /// Tính toán các điểm Dependencies của Bound
        /// </summary>
        protected List<IFigure> GetBoundDependencies()
        {
            var boundDependencies = new List<IFigure>();

            // Số cells trên mặt cắt ngang
            int cellsNumber = NumberOfCellsToInt(this.NumberOfCells);

            // base point is lower left point at the corner of culvert
            // the first cell rectangle is determined by p1, p2, p3, p4

            System.Windows.Point p1 = BasePoint.Coordinates;
            System.Windows.Point p2 = new Point(p1.X, p1.Y + H);

            var culvertBounds = Factory.CreatePolyline(drawing, null);

            Point p5 = new Point(p1.X - this.ExternalWallThickness, p1.Y - this.BottomThickness);
            Point p5a = new Point(p5.X - B2Left, p5.Y);
            Point p5b = new Point(p5a.X, p5.Y + H4Left);
            Point p5c = new Point(p5.X, p5.Y + H4Left);
            Point p51 = new System.Windows.Point(p5.X, p5.Y + TotalHeight - (H1Left + H2Left + H3Left));
            Point p52 = new System.Windows.Point(p5.X - B1Left, p51.Y + H1Left);
            Point p53 = new System.Windows.Point(p5.X - B1Left, p52.Y + H2Left);
            Point p54 = new System.Windows.Point(p5.X, p53.Y);

            Point p6 = new Point(p2.X - this.ExternalWallThickness, p2.Y + this.TopThickness);
            Point p7 = new Point(p6.X + cellsNumber * B +
                +(cellsNumber - 1) * this.InternalWallThickness +
                this.ExternalWallThickness * 2, p6.Y);
            Point p71 = new System.Windows.Point(p7.X, p7.Y - H3Right);
            Point p72 = new System.Windows.Point(p71.X + B1Right, p71.Y);
            Point p73 = new System.Windows.Point(p72.X, p72.Y - H2Right);
            Point p74 = new System.Windows.Point(p71.X, p73.Y - H1Right);

            Point p7a = new System.Windows.Point(p7.X, p7.Y - TotalHeight + H4Right);
            Point p7b = new System.Windows.Point(p7a.X + B2Right, p7a.Y);
            Point p7c = new System.Windows.Point(p7b.X, p7b.Y - H4Right);

            Point p8 = new Point(p7.X, p5.Y);

            var pp5 = Factory.CreateFreePoint(drawing, p5);
            var pp5a = Factory.CreateFreePoint(drawing, p5a);
            var pp5b = Factory.CreateFreePoint(drawing, p5b);
            var pp5c = Factory.CreateFreePoint(drawing, p5c);
            var pp51 = Factory.CreateFreePoint(drawing, p51);
            var pp52 = Factory.CreateFreePoint(drawing, p52);
            var pp53 = Factory.CreateFreePoint(drawing, p53);
            var pp54 = Factory.CreateFreePoint(drawing, p54);
            var pp6 = Factory.CreateFreePoint(drawing, p6);
            var pp7 = Factory.CreateFreePoint(drawing, p7);
            var pp71 = Factory.CreateFreePoint(drawing, p71);
            var pp72 = Factory.CreateFreePoint(drawing, p72);
            var pp73 = Factory.CreateFreePoint(drawing, p73);
            var pp74 = Factory.CreateFreePoint(drawing, p74);
            var pp7a = Factory.CreateFreePoint(drawing, p7a);
            var pp7b = Factory.CreateFreePoint(drawing, p7b);
            var pp7c = Factory.CreateFreePoint(drawing, p7c);
            var pp8 = Factory.CreateFreePoint(drawing, p8);

            if (!IsBottomExpandOnLeft)
            {
                boundDependencies.Add(pp5);
            }
            else
            {
                boundDependencies.Add(pp5a);
                boundDependencies.Add(pp5b);
                boundDependencies.Add(pp5c);
            }
            if (HasApproachSlabOnTheLeft)
            {
                boundDependencies.Add(pp51);
                boundDependencies.Add(pp52);
                boundDependencies.Add(pp53);
                boundDependencies.Add(pp54);
            }
            boundDependencies.Add(pp6);
            boundDependencies.Add(pp7);
            if (HasApproachSlabOnTheRight)
            {
                boundDependencies.Add(pp71);
                boundDependencies.Add(pp72);
                boundDependencies.Add(pp73);
                boundDependencies.Add(pp74);
            }
            if (IsBottomExpandOnRight)
            {
                boundDependencies.Add(pp7a);
                boundDependencies.Add(pp7b);
                boundDependencies.Add(pp7c);
            }
            boundDependencies.Add(pp8);
            if (!IsBottomExpandOnLeft)
            {
                boundDependencies.Add(pp5);
            }
            else
                boundDependencies.Add(pp5a);

            //foreach (IPoint p in boundDependencies) 
            //    this.Dependencies.Add(p);

            return boundDependencies;

        }
        protected List<IFigure> GetCellDependencies(int cellOrder)
        {
            var cellDependencies = new List<IFigure>();

            System.Windows.Point p1 = new System.Windows.Point(this.BasePoint.Coordinates.X
                                            + (cellOrder) * (B + InternalWallThickness), this.BasePoint.Coordinates.Y);
            System.Windows.Point p11 = new Point(p1.X, p1.Y + BottomChamfer);
            System.Windows.Point p12 = new Point(p1.X, p1.Y + H - TopChamfer);
            System.Windows.Point p2 = new Point(p1.X, p1.Y + H);
            System.Windows.Point p21 = new Point(p2.X + TopChamfer, p2.Y);
            System.Windows.Point p22 = new Point(p2.X + B - TopChamfer, p2.Y);
            System.Windows.Point p3 = new Point(p1.X + B, p2.Y);
            System.Windows.Point p31 = new Point(p3.X, p3.Y - TopChamfer);
            System.Windows.Point p32 = new Point(p3.X, p3.Y - H + BottomChamfer);
            System.Windows.Point p4 = new Point(p3.X, p1.Y);
            System.Windows.Point p41 = new Point(p4.X - BottomChamfer, p4.Y);
            System.Windows.Point p42 = new Point(p1.X + BottomChamfer, p1.Y);

            var pp1 = Factory.CreateFreePoint(drawing, p1);
            var pp11 = Factory.CreateFreePoint(drawing, p11);
            var pp12 = Factory.CreateFreePoint(drawing, p12);
            var pp2 = Factory.CreateFreePoint(drawing, p2);
            var pp21 = Factory.CreateFreePoint(drawing, p21);
            var pp22 = Factory.CreateFreePoint(drawing, p22);
            var pp3 = Factory.CreateFreePoint(drawing, p3);
            var pp31 = Factory.CreateFreePoint(drawing, p31);
            var pp32 = Factory.CreateFreePoint(drawing, p32);
            var pp4 = Factory.CreateFreePoint(drawing, p4);
            var pp41 = Factory.CreateFreePoint(drawing, p41);
            var pp42 = Factory.CreateFreePoint(drawing, p42);

            if (!IsChamferAtBottom)
            {
                cellDependencies.Add(pp1);
            }
            else
            {
                cellDependencies.Add(pp11);
            }
            if (IsChamferAtTop)
            {
                cellDependencies.Add(pp12);
            }
            else
            {
                cellDependencies.Add(pp2);
            }
            if (IsChamferAtTop)
            {
                cellDependencies.Add(pp21);
                cellDependencies.Add(pp22);
            }
            else
            {
                cellDependencies.Add(pp3);
            }
            if (IsChamferAtTop)
            {
                cellDependencies.Add(pp31);
            }
            if (IsChamferAtBottom)
            {
                cellDependencies.Add(pp32);
            }
            else
            {
                cellDependencies.Add(pp4);
            }
            if (IsChamferAtBottom)
            {
                cellDependencies.Add(pp41);
                cellDependencies.Add(pp42);
                cellDependencies.Add(pp11);
            }
            else
            {
                cellDependencies.Add(pp1);
            }
            return cellDependencies;
        }

        #endregion

        public int NumberOfCellsToInt(NumberOfCells cells)
        {
            object o = Convert.ChangeType(cells, cells.GetTypeCode());
            return Convert.ToInt16(o) + 1;
        }

        #region SAVE
        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            if (HasApproachSlabOnTheLeft)
            {
                writer.WriteAttributeBool("HasApproachSlabOnTheLeft", HasApproachSlabOnTheLeft);
            }
            writer.WriteAttributeDouble("H", this.H);
            writer.WriteAttributeDouble("B", this.B);
            writer.WriteAttributeDouble("B1Left", this.B1Left);
            writer.WriteAttributeDouble("B1Right", this.B1Right);
            writer.WriteAttributeDouble("B2Left", this.B2Left);
            writer.WriteAttributeDouble("B2Right", this.B2Right);
            writer.WriteAttributeDouble("BottomChamfer", this.BottomChamfer);
            writer.WriteAttributeDouble("BottomThickness", this.BottomThickness);
            writer.WriteAttributeDouble("ExternalWallThickness", this.ExternalWallThickness);
            writer.WriteAttributeDouble("H1Left", this.H1Left);
            writer.WriteAttributeDouble("H1Right", this.H1Right);
            writer.WriteAttributeDouble("H2Left", this.H2Left);
            writer.WriteAttributeDouble("H2Right", this.H2Right);
            writer.WriteAttributeDouble("H3Left", this.H3Left);
            writer.WriteAttributeDouble("H3Right", this.H3Right);
            writer.WriteAttributeDouble("H4Left", this.H4Left);
            writer.WriteAttributeDouble("H4Right", this.H4Right);
            writer.WriteAttributeDouble("InternalWallThickness", this.InternalWallThickness);
            
        }
        public override void ReadXml(System.Xml.Linq.XElement element)
        {
            base.ReadXml(element);
            H = element.ReadDouble("H");
            B = element.ReadDouble("B");
            B1Left = element.ReadDouble("B1Left");
            B1Right = element.ReadDouble("B1Right");
            B2Left = element.ReadDouble("B2Left");
            B2Right = element.ReadDouble("B2Right");
            BottomChamfer = element.ReadDouble("BottomChamfer");
            BottomThickness = element.ReadDouble("BottomThickness");
            ExternalWallThickness = element.ReadDouble("ExternalWallThickness");
            H1Left = element.ReadDouble("H1Left");
            H1Right = element.ReadDouble("H1Right");
            H2Left = element.ReadDouble("H2Left");
            H2Right = element.ReadDouble("H2Right");
            H3Left = element.ReadDouble("H3Left");
            H3Right = element.ReadDouble("H3Right");
            H4Left = element.ReadDouble("H4Left");
            H4Right = element.ReadDouble("H4Right");
            InternalWallThickness = element.ReadDouble("InternalWallThickness");
            
        }
        #endregion


        public override void ToDxf(DxfDocument dxf)
        {
             //= new DxfDocument();
            // Bound
            List<netDxf.Entities.LwPolylineVertex> polyVertexes = new List<netDxf.Entities.LwPolylineVertex>();

            foreach (IFigure p in this.CulvertBound.Dependencies)
            {
                if (p is IPoint)
                {
                    var point = p as IPoint;
                    polyVertexes.Add(new netDxf.Entities.LwPolylineVertex(point.Coordinates.X, point.Coordinates.Y));
                }
            }

            // Cells
            // Số cells trên mặt cắt ngang
            int cellsNumber = NumberOfCellsToInt(this.NumberOfCells);
            for (int i = 0; i < cellsNumber; i++)
            {
                List<netDxf.Entities.LwPolylineVertex> cellVertexes = new List<netDxf.Entities.LwPolylineVertex>();
                foreach (IPoint p in GetCellDependencies(i))
                {
                    cellVertexes.Add(new netDxf.Entities.LwPolylineVertex(p.Coordinates.X, p.Coordinates.Y));
                }
                netDxf.Entities.LwPolyline cellPolyline = new netDxf.Entities.LwPolyline(cellVertexes, true);
                cellPolyline.Layer = new netDxf.Tables.Layer("polyline2d");
                cellPolyline.Layer.Color.Index = ColorHelper.ColorToAutoCADColor(this.LineStyle.Color);
                //polyline2d.Normal = new Vector3(1, 1, 1);
                //polyline2d.Elevation = 0.0f;
                dxf.AddEntity(cellPolyline);
            }
            netDxf.Entities.LwPolyline polyline2d = new netDxf.Entities.LwPolyline(polyVertexes, true);
            polyline2d.Layer = new netDxf.Tables.Layer("polyline2d");
            polyline2d.Layer.Color.Index = ColorHelper.ColorToAutoCADColor(this.LineStyle.Color);
            //polyline2d.Normal = new Vector3(1, 1, 1);
            //polyline2d.Elevation = 0.0f;
            dxf.AddEntity(polyline2d);
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2000;
            //dxf.Save(fileName);

            foreach (IFigure f in Children)
            {
                var child = f as FigureBase;
                child.ToDxf(dxf);
            }
        }

        #region Dimensions
        private bool _isShowDimensions = true;
        public bool IsShowDimensions
        {
            get { return _isShowDimensions; }
            set {
                _isShowDimensions = value;
                RaisePropertyChanged("IsShowDimensions");
                foreach (IFigure f in this.Children)
                {
                    if (f is LinearDimension)
                    {
                        var dim = f as LinearDimension;
                        dim.Visible = value;
                    }
                }
            }
        }
        #endregion
    }
}
