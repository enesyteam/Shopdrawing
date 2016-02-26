////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Reflection;
////using System.Text;
////using System.Windows;
////using System.Windows.Media;
////using DynamicGeometry;
//using Shopdrawing.Utilities;
////using Shopdrawing.Settings;

////namespace Shopdrawing.Reinforcement
////{
////    public enum RectangleMeshDirection
////    {
////        Horizontal = 0, Vertical = 1
////    }

////    public class RectangleMesh : CompositeFigure
////    {
////        /// <summary>
////        /// Mặt cắt lưới theo phương ngang
////        /// </summary>
////        public MeshSection HorizontalMeshSection { get; set; }
////        /// <summary>
////        /// Mặt cắt lưới theo phương đứng
////        /// </summary>
////        public MeshSection VerticalMeshSection { get; set; }

////        public List<PlanView> Items { get; set; }

////        public BarsColection<PlanView> HorizontalBars { get; set; }
////        public BarsColection<BarCrossView> LongitudinalBars { get; set; }

////        public Point InsertPoint { get; set; }

////        public SectionDirection SectionDirection { get; set; }
////        public void GetHorizontalSection() { }
////        public void GetVerticalSection() { }

////        [PropertyGridVisible]
////        public string HorizontalBarName { get; set; }
////        [PropertyGridVisible]
////        public string VerticalBarName { get; set; }

////        #region Constructors
////        public RectangleMesh()
////        { }
////        public RectangleMesh(DynamicGeometry.Drawing drawing, 
////            MeshSection horizontalMeshSection, MeshSection verticalMeshSection, Point insertPoint)
////            //: base(drawing, horizontalMeshSection, verticalMeshSection)
////        {
////            Drawing = drawing;
////            HorizontalMeshSection = horizontalMeshSection;
////            VerticalMeshSection = verticalMeshSection;

////            InsertPoint = insertPoint;
////            CreateRectangleMesh();
////            CreateContainer();
////        }
////        public RectangleMesh(DynamicGeometry.Drawing drawing,
////    MeshSection meshSection, Point insertPoint)
////        //: base(drawing, horizontalMeshSection, verticalMeshSection)
////        {
////            Drawing = drawing;
////            HorizontalMeshSection = meshSection;

////            // Tạo lưới theo phương còn lại
////            MeshSection vMesh = new MeshSection(HorizontalMeshSection.Drawing,
////                HorizontalMeshSection.MeshContainer, HorizontalMeshSection.HBar, HorizontalMeshSection.Bar);
////            vMesh.CreateMeshSection();
////            VerticalMeshSection = vMesh;
            

////            InsertPoint = insertPoint;
////            CreateRectangleMeshBars();
////            CreateContainer();
////        }
////        #endregion

////        #region Update
////        [PropertyGridVisible]
////        [PropertyGridName("Update")]
////        public void Update()
////        {
////            if (this != null)
////            {
////                try
////                {
////                    var newRectangleMesh = CloneObject(this) as RectangleMesh;
////                    newRectangleMesh.CreateRectangleMesh();
////                    newRectangleMesh.CreateContainer();
////                    newRectangleMesh.Dependencies.AddRange(this.Dependencies);
////                    newRectangleMesh.RegisterWithDependencies();
////                    Actions.ReplaceWithNew(this, newRectangleMesh);
////                }
////                catch (Exception ex)
////                {
////                    Drawing.RaiseStatusNotification("Can not create new instance of this!" + "\n" + ex.ToString());
////                }
////            }
////        }
////        #endregion

////        #region Vẽ mặt bằng lưới
////        private RectangleMeshDirection _drawDirection = RectangleMeshDirection.Horizontal;
////        [PropertyGridVisible]
////        [PropertyGridName("Direction")]
////        public RectangleMeshDirection DrawDirection
////        {
////            get { return _drawDirection; }
////            set { _drawDirection = value; }
////        }
////        IFigure container = null;
////        public void CreateRectangleMesh()
////        {
////            // Lấy tên
////            HorizontalBarName = HorizontalMeshSection.BarName;
////            VerticalBarName = VerticalMeshSection.BarName;
////            Items = new List<PlanView>();

////            // Xác định hướng vẽ: Horizontal hoặc Vertical

////            #region Horizontal Bars
////            var hCount = HorizontalMeshSection.Count;
////            var hLength = HorizontalMeshSection.Deep - 2 * HorizontalMeshSection.Cover;
////            var hStep = HorizontalMeshSection.UniformSpacing;
////            using (Drawing.ActionManager.CreateTransaction())
////            {
////                if ((hCount % 2) == 0)
////                {
////                    MessageBox.Show("Chẵn");
////                }
////                else
////                {
////                    for (int i = 0; i < hCount; i++)
////                    {
////                        var xOffset = (i - hCount / 2) * hStep;

////                        var linePoints = new List<IFigure>();

////                        var p1 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(InsertPoint, xOffset, -0.5*hLength));
////                        var p2 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(p1.Coordinates, 0, hLength));

////                        linePoints.Add(p1, p2);

////                        var bar = BarCreator.CreateBarPlanView(Drawing, linePoints);
////                        bar.Name = HorizontalMeshSection.Bar.Name;
////                        bar.Style = DefaultStyles.BarStyle;
////                        Items.Add(bar);
////                        Children.Add(bar);
////                        Dependencies.Add(p1, p2);
////                    }
////                }

////            }
////            #endregion

////            #region Vertical Bars
////            var vCount = VerticalMeshSection.Count;
////            var vLength = VerticalMeshSection.Length - 2*VerticalMeshSection.Cover;
////            var vStep = VerticalMeshSection.UniformSpacing;
////            using (Drawing.ActionManager.CreateTransaction())
////            {
////                if ((hCount % 2) == 0)
////                {
////                    MessageBox.Show("Chẵn");
////                }
////                else
////                {
////                    for (int i = 0; i < vCount; i++)
////                    {
////                        var yOffset = (i - vCount / 2) * vStep;

////                        var linePoints = new List<IFigure>();

////                        var p1 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(InsertPoint, -0.5 * vLength, yOffset));
////                        var p2 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(p1.Coordinates, vLength, 0));

////                        linePoints.Add(p1, p2);

////                        var bar = BarCreator.CreateBarPlanView(Drawing, linePoints);
////                        bar.Name = VerticalMeshSection.Bar.Name;
////                        bar.Style = DefaultStyles.BarStyle;
////                        Items.Add(bar);
////                        Children.Add(bar);
////                        Dependencies.Add(p1, p2);
////                    }
////                }

////            }
////            #endregion

////        }
////        public void CreateRectangleMeshBars()
////        {
////            // Lấy tên
////            HorizontalBarName = HorizontalMeshSection.BarName;
////            VerticalBarName = VerticalMeshSection.BarName;
////            Items = new List<PlanView>();

////            // Xác định hướng vẽ: Horizontal hoặc Vertical

////            #region Horizontal Bars
////            var hCount = HorizontalMeshSection.Count;
////            var hLength = HorizontalMeshSection.Deep - 2 * HorizontalMeshSection.Cover;
////            var hStep = HorizontalMeshSection.UniformSpacing;
////            using (Drawing.ActionManager.CreateTransaction())
////            {
////                if ((hCount % 2) == 0)
////                {
////                    MessageBox.Show("Chẵn");
////                }
////                else
////                {
////                    for (int i = 0; i < hCount; i++)
////                    {
////                        var xOffset = (i - hCount / 2) * hStep;

////                        var linePoints = new List<IFigure>();

////                        var p1 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(InsertPoint, xOffset, -0.5 * hLength));
////                        var p2 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(p1.Coordinates, 0, hLength));

////                        linePoints.Add(p1, p2);

////                        var bar = BarCreator.CreateBarPlanView(Drawing, linePoints);
////                        bar.Name = HorizontalMeshSection.Bar.Name;
////                        bar.Style = DefaultStyles.BarStyle;
////                        Items.Add(bar);
////                        Children.Add(bar);
////                        Dependencies.Add(p1, p2);
////                    }
////                }

////            }
////            #endregion

////            #region Vertical Bars
////            double width = VerticalMeshSection.MeshContainer.Width;
////            double height = VerticalMeshSection.MeshContainer.Height;
////            double length = VerticalMeshSection.MeshContainer.Length;

////            var vCount = VerticalMeshSection.LCount;
////            double vLength = 0;
////            if (width != 0)
////            {
////                vLength = VerticalMeshSection.Width - 2 * VerticalMeshSection.Cover;
////            }
////            else
////            {
////                vLength = height - 2 * VerticalMeshSection.Cover;
////            }
////            var vStep = VerticalMeshSection.UniformSpacing;
////            using (Drawing.ActionManager.CreateTransaction())
////            {
////                if ((hCount % 2) == 0)
////                {
////                    MessageBox.Show("Chẵn");
////                }
////                else
////                {
////                    for (int i = 0; i < vCount; i++)
////                    {
////                        var yOffset = (i - vCount / 2) * vStep;

////                        var linePoints = new List<IFigure>();

////                        var p1 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(InsertPoint, -0.5 * vLength, yOffset));
////                        var p2 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(p1.Coordinates, vLength, 0));

////                        linePoints.Add(p1, p2);

////                        var bar = BarCreator.CreateBarPlanView(Drawing, linePoints);
////                        bar.Name = VerticalMeshSection.Bar.Name;
////                        bar.Style = DefaultStyles.BarStyle;
////                        Items.Add(bar);
////                        Children.Add(bar);
////                        Dependencies.Add(p1, p2);
////                    }
////                }

////            }
////            #endregion

////        }
////        #endregion

////        #region Container
////        public void CreateContainer()
////        {
////            double width = HorizontalMeshSection.MeshContainer.Width;
////            double height = HorizontalMeshSection.MeshContainer.Height;
////            double length = HorizontalMeshSection.MeshContainer.Length;

////            double xOffset = 0;
////            double yOffset = 0;

////            if (width == 0 && height != 0)
////            {
////                xOffset = 0.5 * height;
////                yOffset = 0.5 * length + HorizontalMeshSection.HBar.Cover;
////            }
////            else if (height == 0)
////            {
////                xOffset = 0.5 * width;
////                yOffset = 0.5 * length + HorizontalMeshSection.HBar.Cover;
////            }
////            else
////            {
////                xOffset = 0.5 * width;
////                yOffset = 0.5 * height;
////            }

////            List<IFigure> containerPoints = new List<IFigure>();
////            containerPoints.Add(Factory.CreateFreePoint(Drawing, PointsHelper.Offset(this.InsertPoint, -xOffset, -yOffset)));
////            containerPoints.Add(Factory.CreateFreePoint(Drawing, PointsHelper.Offset(this.InsertPoint, -xOffset, yOffset)));
////            containerPoints.Add(Factory.CreateFreePoint(Drawing, PointsHelper.Offset(this.InsertPoint, xOffset, yOffset)));
////            containerPoints.Add(Factory.CreateFreePoint(Drawing, PointsHelper.Offset(this.InsertPoint, xOffset, -yOffset)));
////            containerPoints.Add(Factory.CreateFreePoint(Drawing, PointsHelper.Offset(this.InsertPoint, -xOffset, -yOffset)));

////            container = Factory.CreatePolyline(Drawing, containerPoints);
////            container.Style = DefaultStyles.StructureLineStyle;
////            //container.Visible = this.Selected;

////            Children.Add(container);
////            foreach (IFigure f in containerPoints)
////                Dependencies.Add(f);
////        }
////        #endregion

////        /// <summary>
////        /// Copy tất cả thuộc tính 1 đối tượng sang đối tượng mới
////        /// </summary>
////        /// <param name="o"></param>
////        /// <returns></returns>
////        public object CloneObject(object o)
////        {
////            Type t = o.GetType();
////            PropertyInfo[] properties = t.GetProperties();

////            Object p = t.InvokeMember("", System.Reflection.BindingFlags.CreateInstance,
////                null, o, null);

////            foreach (PropertyInfo pi in properties)
////            {
////                if (pi.CanWrite)
////                {
////                    pi.SetValue(p, pi.GetValue(o, null), null);
////                }
////            }

////            return p;
////        }
////        public override string ToString()
////        {
////            return "Rectangle Mesh";
////        }
////        public override IFigure HitTest(Point point)
////        {
////            List<Point> pList = new List<System.Windows.Point>();

////            foreach (IPoint p in container.Dependencies)
////            {
////                pList.Add(p.Coordinates);
////            }
////            bool isInside = pList.IsPointInPolygon(point);
////            return isInside ? this : null;
////        }
////        //public override bool Selected
////        //{
////        //    get
////        //    {
////        //        return this.container.Selected;
////        //    }
////        //    set
////        //    {
////        //        base.Selected = value;
////        //        container.Visible = value;
////        //        //this.container.Selected = value;
////        //    }
////        //}

////        public override void Recalculate()
////        {
////            base.Recalculate();
////            List<Point> pList = new List<System.Windows.Point>();

////            foreach (IPoint p in container.Dependencies)
////            {
////                pList.Add(p.Coordinates);
////            }
////            pList.RemoveLast(); // điểm cuối cùng bị trùng với điểm đầu tiên
////            this.InsertPoint = pList.Midpoint();
////        }
////    }
////}
