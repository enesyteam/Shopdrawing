////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Windows;
////using System.Windows.Controls;
////using System.Windows.Media;
////using System.Windows.Shapes;
////using DynamicGeometry;
//using Shopdrawing.Utilities;
////using Shopdrawing.Settings;

////namespace Shopdrawing.Reinforcement
////{
////    public class MeshSection : MeshBase, IEnumerable<BarCrossView>, ICanRotate
////    {
////        public List<BarCrossView> Items { get; set; }

////        public BarsColection<PlanView> HorizontalBars { get; set; }
////        public BarsColection<BarCrossView> LongitudinalBars { get; set; }
       
////        public List<IFigure> Bounds { get; set; }
////        /// <summary>
////        /// Góc tạo bởi mặt cắt và phương ngang
////        /// </summary>
////        public double Alpha { get; set; }

////        public string BarName { get; set; }
////        public Bar HBar { get; set; }

////        private Transform _transform = Transform.Normal;
////        //[PropertyGridVisible]
////        //[PropertyGridName("Transform")]
////        public override Transform Transform 
////        {
////            get { return _transform; }
////            set { _transform = value; }
////        }
        
////        [PropertyGridVisible]
////        [PropertyGridName("HCount")]
////        public int Count { get { return LongitudinalBars.Count; } }
////        [PropertyGridName("LCount")]
////        public int LCount { get { return HorizontalBars.Count; } }

////        #region Constructors
////        public MeshSection(){ }
////        public MeshSection(DynamicGeometry.Drawing drawing, MeshContainer meshContainer, Bar bar, Bar hBar)
////            : base(drawing, meshContainer, bar)
////        {
////            Drawing = drawing;
////            Items = new List<BarCrossView>();
////            LongitudinalBars = new BarsColection<BarCrossView>();
////            HorizontalBars = new BarsColection<PlanView>();
////            Bounds = new List<IFigure>();

////            InsertPoint = meshContainer.Focus;
////            MeshContainer = meshContainer;

////            Bar = bar;
////            HBar = hBar;
////            Cover = bar.Cover;

////            Width = meshContainer.Width;
////            Height = meshContainer.Height;
////            Deep = meshContainer.Length;

////            VerticalBarsSpacing = bar.Step;
////            HorizontalBarsSpacing = bar.Step;
////        }
////        public MeshSection(DynamicGeometry.Drawing drawing, MeshContainer meshContainer, Bar bar, Bar hBar, Direction direction)
////            : this(drawing, meshContainer, bar, hBar)
////        {
////            Direction = direction;
////            HBar = hBar;
////        }
////        #endregion

////        [PropertyGridVisible]
////        [PropertyGridName("Update")]
////        public override void Update()
////        {
////            if (this != null)
////            {
////                try
////                {
////                    var newMesh = CloneObject(this) as MeshSection;
////                    newMesh.CreateMeshSection();
////                    newMesh.Dependencies.AddRange(this.Dependencies);
////                    newMesh.RegisterWithDependencies();
////                    Actions.ReplaceWithNew(this, newMesh);
////                }
////                catch(Exception ex)
////                {
////                    Drawing.RaiseStatusNotification("Can not create new instance of this!" + "\n" + ex.ToString());
////                }
////            }
////        }


////        public void GetBounds()
////        {
////            var pl = Factory.CreateFreePoint(Drawing, new Point(0,0));
////            var pr = Factory.CreateFreePoint(Drawing, new Point(0, 0));
////            if (Direction == Reinforcement.Direction.Up)
////            {
////                pl = Factory.CreateFreePoint(Drawing, new Point(this.InsertPoint.X - this.Width / 2, this.InsertPoint.Y - this.Height / 2));
////                pr = Factory.CreateFreePoint(Drawing, new Point(this.InsertPoint.X + this.Width / 2, this.InsertPoint.Y + this.Height / 2));
////            }
////            else
////            {
////                pl = Factory.CreateFreePoint(Drawing, new Point(this.InsertPoint.X - this.Width / 2, this.InsertPoint.Y + this.Height / 2));
////                pr = Factory.CreateFreePoint(Drawing, new Point(this.InsertPoint.X + this.Width / 2, this.InsertPoint.Y - this.Height / 2));

////            }

////            var p1 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(pl.Coordinates, 0, -Bar.Diameter));
////            var p2 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(pl.Coordinates, 0, Bar.Diameter));
////            var p3 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(pr.Coordinates, 0, Bar.Diameter));
////            var p4 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(pr.Coordinates, 0, -Bar.Diameter));

////            Bounds.Add(p1, p2, p3, p4, p1);
////            //List<IPoint> boundList = new List<IPoint>();
////            foreach (IPoint p in Bounds)
////            {
////                this.Dependencies.Add(p);
////            }
////            //var pol = Factory.CreatePolyline(Drawing, Bounds);
////            //this.Children.Add(pol);
////        }

////        public override int ZIndex
////        {
////            get
////            {
////                return (int)ZOrder.Reinforcement;
////            }
////            set
////            {
////                base.ZIndex = value;
////            }
////        }

////        public override IFigure HitTest(Point point)
////        {
////            List<Point> pList = new List<System.Windows.Point>();

////            foreach (IPoint p in this.Dependencies)
////            {
////                pList.Add(p.Coordinates);
////            }
////            bool isInside = pList.IsPointInPolygon(point);
////            return isInside ? this : null;
////        }

////        public override string ToString()
////        {
////            return "Mesh Section";
////        }


////        // IEnumerable<Bar> Members
////        public IEnumerator<BarCrossView> GetEnumerator()
////        {
////            return Items.GetEnumerator();
////        }
////        //IEnumerable Members
////        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
////        {
////            throw new NotImplementedException();
////        }

////        #region Draw Mesh Section
////        /// <summary>
////        /// Draw Mesh Section
////        /// </summary>
////        /// <param name="width">Width of container</param>
////        /// <param name="height">Height of container</param>
////        public void CreateMeshSection()
////        {
////            BarName = Bar.Name;

////            double width = MeshContainer.Width;
////            double height = MeshContainer.Height;
////            double length = MeshContainer.Length;

////            Items = new List<BarCrossView>();

////            IEnumerable<Point> listP = base.GetVisiblePoint(width, height);
////            using (Drawing.ActionManager.CreateTransaction())
////            {
////                try
////                {
////                    // Vẽ mặt cắt các thanh
////                    foreach (Point p in listP)
////                    {
////                        var pp = Factory.CreateFreePoint(this.Drawing, p);
////                        var pOnCircle = Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(pp.Coordinates, 0.5*Bar.Diameter, 0)); // Y=??

////                        List<IFigure> pointsList = new List<IFigure>();
////                        pointsList.Add(pp, pOnCircle);
////                        var b = BarCreator.CreateBarCross(this.Drawing, pointsList);
////                        b.Style = DefaultStyles.BarStyle;
////                        LongitudinalBars.Add(b);
////                        this.Children.Add(b);
////                        Dependencies.Add(pp, pOnCircle);

////                    }
////                    // Vẽ thanh ngang
////                    try
////                    {
////                        double p1XOffset = 0;
////                        double p1YOffset = 0;
////                        double p2XOffset = 0;
////                        double p2YOffset = 0;

////                        if (width == 0) // Mặt cắt theo phương thẳng đứng
////                        {
////                            p1YOffset = -0.5 * HBar.Length + HBar.Cover;
////                            p2YOffset = 0.5 * HBar.Length - HBar.Cover;
////                        }
////                        else
////                        {
////                            p1XOffset = -0.5 * HBar.Length + HBar.Cover;
////                            p2XOffset = 0.5 * HBar.Length - HBar.Cover;

////                            p1YOffset = -0.5 * Bar.Diameter;
////                            p2YOffset = -0.5 * Bar.Diameter;
////                        }
////                        // Xác định số thanh
////                        IEnumerable<Point> listPoints = base.GetVisiblePoint(length, height);

////                        var p1 = Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint, p1XOffset, p1YOffset));
////                        var p2 = Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint, p2XOffset, p2YOffset));
////                        List<IFigure> pList = new List<IFigure>();
////                        pList.Add(p1, p2);
////                        var bar = BarCreator.CreateBarPlanView(this.Drawing, pList);
////                        bar.Style = DefaultStyles.BarStyle;
////                        this.Children.Add(bar);
////                        Dependencies.Add(p1, p2);

////                        // Chỉ thêm vào nhằm mục đích Count (Cần xem lại thuật toán)
////                        foreach (Point p in listPoints) HorizontalBars.Add(bar);
////                    }
////                    catch
////                    {
////                        MessageBox.Show("HBar is not defined!");
////                    }
////                }
////                catch (Exception ex)
////                {
////                    MessageBox.Show(ex.ToString());
////                }
////            }
////            GetBounds();
////        }
////        #endregion

////        #region Flip
////        [PropertyGridVisible]
////        [PropertyGridName("Flip Vertical")]
////        public void TestFlip()
////        {
////            this.Direction = this.Direction == Reinforcement.Direction.Up ? Reinforcement.Direction.Down : Reinforcement.Direction.Up;
////            this.Update();
////        }
////        #endregion

////        private double _cover;
////        public override double Cover { get { return _cover; } set { _cover = value; } }

////        public double UniformSpacing { get { return HorizontalBarsSpacing; } }
////        public double Length { get { return MeshContainer.Width; } }

////        public IEnumerable<Point> GetAllBarsCenter()
////        {
////            foreach (BarCrossView item in Items)
////            {
////                yield return item.Center;
////            }
////        }

////        public override void Recalculate()
////        {
////            base.Recalculate();
////            // Trường hợp số thanh lẻ
////            if (Items.Count % 2 != 0)
////            {
////                var centerObject = Items[Items.Count / 2] as BarCrossView;
////                var center = centerObject.Dependencies[0] as IPoint;
////                this.InsertPoint = center.Coordinates;
////            }
////        }
////    }
////}
