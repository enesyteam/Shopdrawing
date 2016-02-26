////#define DEBUG


////using System;
////using System.Collections.Generic;
////using System.ComponentModel;
////using System.Linq;
////using System.Text;
////using System.Windows;
////using System.Windows.Input;
////using System.Windows.Media;
////using System.Xml;
////using System.Xml.Linq;
////using DynamicGeometry;
////using Shopdrawing.Utilities;
////using Shopdrawing.Reinforcement;
////using Shopdrawing.Settings;
////using System.Reflection;

////namespace Shopdrawing.Structures.Culvert
////{
////    public partial class BoxculvertCrossSectionBars : BoxCulvertSection
////    {
////        #region Override Properties
////        // TODO: Disable Properties display
////        #endregion
////        #region Properties
////        //public Boxculvert ParentCulvert { get; set; }

////        ////[PropertyGridVisible]
////        ////[PropertyGridName("Mesh Sections")]
////        ////public List<MeshSection> MeshSectionsCollection { get; set; }

////        [PropertyGridVisible]
////        [PropertyGridName("Meshes Count")]
////        public int MeshSectionsCount { get { return this.MeshSectionsCollection.Count; }}

////        private BoxCulvertSection _crossSection = null;

////        /// <summary>
////        /// Mặt cắt ngang để vẽ cốt thép
////        /// </summary>
////        //[PropertyGridVisible]
////        //[PropertyGridName("Parent Section")]
////        public BoxCulvertSection CrossSection 
////        {
////            get { return _crossSection; }
////            set { _crossSection = value; }
////        }

////        #region Mesh Sections
////        ////[PropertyGridVisible]
////        ////[Browsable(false)]
////        ////public MeshSection M1 { get; set; }
////        ////public MeshSection M2 { get; set; }
////        ////public MeshSection M3 { get; set; }
////        ////public MeshSection M4 { get; set; }
////        ////public MeshSection M5 { get; set; }
////        ////public MeshSection M6 { get; set; }
////        ////public MeshSection M7 { get; set; }
////        ////public MeshSection M8 { get; set; }
////        ////public MeshSection M9 { get; set; }
////        ////public MeshSection M10 { get; set; }
////        ////#endregion


////        #endregion
////        #region Constructors
////        public BoxculvertCrossSectionBars()
////        {
////        }
////        public BoxculvertCrossSectionBars(BoxCulvertSection crossSection)
////        {
////            this.CrossSection = crossSection;
////            //this.ParentCulver = crossSection.ParentCulver;
////            GetSettingsForSection();

////            // TODO: Tập hợp các mặt cắt lưới thép
////            //MeshSectionsCollection = new List<MeshSection>();

////        }

////        public BoxculvertCrossSectionBars(DynamicGeometry.Drawing drawing)
////        {
////        }
////        #endregion

        
////#if DEBUG
////        /// <summary>
////        /// Lấy các thuộc tính của mặt cắt
////        /// </summary>
////        public void GetSettingsForSection()
////        {
////            //var newSections = CloneObject(this.CrossSection) as BoxCulvertSection;
////            this.Drawing = this.CrossSection.Drawing;
////            this.HasApproachSlabOnTheLeft = this.CrossSection.HasApproachSlabOnTheLeft;
////            this.HasApproachSlabOnTheRight = this.CrossSection.HasApproachSlabOnTheRight;
////            this.B2Left = this.CrossSection.B2Left;
////            this.B2Right = this.CrossSection.B2Right;
////            this.H4Left = this.CrossSection.H4Left;
////            this.H4Right = this.CrossSection.H4Right;
////            this.InsertPoint = PointsHelper.Offset(this.CrossSection.InsertPoint, 0, -this.CrossSection.TotalHeight - 50);
////            this.NumberOfCells = this.CrossSection.NumberOfCells;
////            this.B = this.CrossSection.B;
////            this.H = this.CrossSection.H;
////            this.TopChamfer = this.CrossSection.TopChamfer;
////            this.BottomChamfer = this.CrossSection.BottomChamfer;
////            this.BottomThickness = this.CrossSection.BottomThickness;
////            this.TopThickness = this.CrossSection.TopThickness;
////            this.ExternalWallThickness = this.CrossSection.ExternalWallThickness;
////            this.InternalWallThickness = this.CrossSection.InternalWallThickness;
////        }


////#endif

        
       
////        /// <summary>
////        /// Ký hiệu mặt cắt
////        /// </summary>
////        /// <returns></returns>
////        public void CreateSectionTitle()
////        {
////            TextStyle labelsStyle = new TextStyle()
////            {
////                Color = Colors.Red,
////                FontSize = 500.0,
////                Name = "LabelsStyle",
////            };

////            var pointToInsert = Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint, 0, this.TotalHeight));
////            //var title = Factory.CreateLabel(this.Drawing, pointToInsert);
////            var label = Factory.CreateLabel(Drawing);
////            label.Text = this.Name;
////            label.Style = labelsStyle;
////            label.MoveTo(pointToInsert.Coordinates);
////            //Actions.Add(Drawing, label);
////            this.Children.Add(label);
////        }


////        public override string ToString()
////        {
////            return "Rebars Cross Section";
////        }
////        #region override
////        public override IFigure HitTest(Point point)
////        {
////            List<Point> pList = new List<System.Windows.Point>();

////            foreach (IFigure p in this.Dependencies)
////            {
////                if (p is IPoint)
////                {
////                    var ip = p as IPoint;
////                    pList.Add(ip.Coordinates);
////                }
                
////            }
////            bool isInside = pList.IsPointInPolygon(point);
////            return isInside ? this : null;
////        }
////        public override bool Selected
////        {
////            get
////            {
////                List<IFigure> lst = new List<IFigure>();
////                foreach (IFigure f in this.Children)
////                {
////                    if (!(f is MeshSection))
////                    {
////                        return f.Selected;
////                    }
////                }
////                return false;
////            }
////            set
////            {
////                foreach (IFigure f in this.Children)
////                {
////                    if (!(f is MeshSection))
////                    {
////                        f.Selected = value;
////                    }
////                }
////            }
////        }
////        //public override void Recalculate()
////        //{
////        //    base.Recalculate();
////        //    List<Point> pList = new List<System.Windows.Point>();

////        //    foreach (IFigure p in this.Dependencies)
////        //    {
////        //        if (p is IPoint)
////        //        {
////        //            var ip = p as IPoint;
////        //            pList.Add(ip.Coordinates);
////        //        }
////        //    }
////        //    this.InsertPoint = pList.Midpoint();
////        //}

////#if !PLAYER
////        //public override void WriteXml(XmlWriter writer)
////        //{
////        //    if (!Visible)
////        //    {
////        //        writer.WriteAttributeString("Visible", "false");
////        //    }
////        //    if (Locked)
////        //    {
////        //        writer.WriteAttributeString("Locked", "true");
////        //    }
////        //    if (Arrow.Style != null)
////        //    {
////        //        writer.WriteAttributeString("Style", Arrow.Style.Name);
////        //    }
////        //}

////#endif
////        //public override void ReadXml(XElement element)
////        //{
////        //    // Do not use CompositeFigure.ReadXml() because there are no children to read. Children are created by constructor.
////        //    Visible = element.ReadBool("Visible", true);
////        //    Locked = element.ReadBool("Locked", false);
////        //    IsHitTestVisible = element.ReadBool("IsHitTestVisible", true);
////        //    var styleAttribute = element.Attribute("Style");
////        //    if (styleAttribute != null
////        //        && Drawing != null
////        //        && Drawing.StyleManager != null)
////        //    {
////        //        var style = Drawing.StyleManager[styleAttribute.Value];
////        //        if (style != null)
////        //        {
////        //            this.Arrow.Style = style;
////        //        }
////        //    }
////        //}

////        #endregion


////        public IFigure Bouder { get; set; }

////        public Arrow Arrow { get; set; }
////        public double Length { get; set; }

////        #region Draw the cross sections of culvert
////        public void CreateMeshSections()
////        {
////            MeshSectionsCollection = new List<MeshSection>();

////            #region M1
////            double m1Width = this.H4Left >= this.BottomThickness ? this.TotalWidth : this.TotalWidth - B2Left * 2;
////            MeshContainer M1Container = new MeshContainer()
////            {
////                Drawing = this.Drawing,
////                Width = m1Width,
////                Height = 0,
////                Length = this.ParentCulver.Length,
////            };
////            try
////            {
////                Bar bar = new Bar() { Name = "B01", Diameter = 2, Step = 10, Cover = 5, Length = M1Container.Length };
////                Bar hBar = new Bar() { Name = "B01a", Diameter = 2, Step = 10, Cover = 5, Length = M1Container.Width };
////                M1Container.Focus = PointsHelper.Offset(this.Center, 0, -(bar.Cover + bar.Diameter / 2));
////                M1 = new MeshSection(this.Drawing, M1Container, bar, hBar);
////                M1.CreateMeshSection();
////                this.Children.Add(M1);
////                MeshSectionsCollection.Add(M1);
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show(ex.ToString());
////            }
////            #endregion
////            #region M2
////            MeshContainer M2Container = new MeshContainer()
////            {
////                Drawing = this.Drawing,
////                Width = this.TotalWidth,
////                Height = 0,
////                Length = this.ParentCulver.Length,
////            };

////            try
////            {
////                Bar bar = new Bar() { Name = "B02", Diameter = 2, Step = 15, Cover = 5, Length = M2Container.Length };
////                Bar hBar = new Bar() { Name = "B02a", Diameter = 2, Step = 10, Cover = 5, Length = M2Container.Width };
////                M2Container.Focus = PointsHelper.Offset(this.Center, 0, -(this.BottomThickness - bar.Cover - bar.Diameter / 2));
////                M2 = new MeshSection(this.Drawing, M2Container, bar, hBar);
////                M2.CreateMeshSection();
////                this.Children.Add(M2);
////                MeshSectionsCollection.Add(M2);
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show(ex.ToString());
////            }
////            #endregion
////            #region M3
////            MeshContainer M3Container = new MeshContainer()
////            {
////                Drawing = this.Drawing,
////                Width = this.TotalWidth - B2Left - B2Right,
////                Height = 0,
////                Length = this.ParentCulver.Length,
////            };

////            try
////            {
////                Bar bar = new Bar() { Name = "B03", Diameter = 2, Step = 20, Cover = 5, Length = M3Container.Length };
////                Bar hBar = new Bar() { Name = "B03a", Diameter = 2, Step = 15, Cover = 5, Length = M3Container.Width };
////                M3Container.Focus = PointsHelper.Offset(this.Center, 0, (this.H + this.TopThickness - bar.Cover - bar.Diameter / 2));
////                M3 = new MeshSection(this.Drawing, M3Container, bar, hBar);
////                M3.CreateMeshSection();
////                this.Children.Add(M3);
////                MeshSectionsCollection.Add(M3);
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show(ex.ToString());
////            }
////            #endregion
////            #region M4
////            MeshContainer m4Container = new MeshContainer()
////            {
////                Drawing = this.Drawing,
////                Width = this.TotalWidth - B2Left - B2Right,
////                Height = 0,
////                Length = this.ParentCulver.Length,
////            };

////            try
////            {
////                Bar bar = new Bar() { Name = "B04", Diameter = 2, Step = 30, Cover = 5, Length = m4Container.Length };
////                Bar hBar = new Bar() { Name = "B04a", Diameter = 2, Step = 7.5, Cover = 5, Length = m4Container.Width };
////                m4Container.Focus = PointsHelper.Offset(this.Center, 0, (this.H + bar.Cover + bar.Diameter / 2));
////                M4 = new MeshSection(this.Drawing, m4Container, bar, hBar);
////                M4.CreateMeshSection();
////                this.Children.Add(M4);
////                MeshSectionsCollection.Add(M4);
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show(ex.ToString());
////            }
////            #endregion

////            #region M5
////            try
////            {
////                MeshContainer m5Container = new MeshContainer()
////                {
////                    Drawing = this.Drawing,
////                    Width = 0,
////                    Height = this.H,
////                    Length = this.ParentCulver.Length,
////                };
////                Bar bar = new Bar() { Name = "B05", Diameter = 2, Step = 20, Cover = 5, Length = m5Container.Length };
////                Bar hBar = new Bar() { Name = "B05a", Diameter = 2, Step = 12.5, Cover = 5, Length = m5Container.Height };
////                m5Container.Focus = PointsHelper.Offset(this.Center,
////                    -((NumberOfCellsToInt(this.NumberOfCells) - 1) * 0.5 * this.InternalWallThickness + NumberOfCellsToInt(this.NumberOfCells) * B * 0.5
////                    + ExternalWallThickness - bar.Cover - bar.Diameter / 2),
////                    H / 2);
////                M5 = new MeshSection(this.Drawing, m5Container, bar, hBar);
////                M5.CreateMeshSection();
////                this.Children.Add(M5);
////                MeshSectionsCollection.Add(M5);
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show(ex.ToString());
////            }
////            #endregion
////            #region M6
////            try
////            {
////                MeshContainer m6Container = new MeshContainer()
////                {
////                    Drawing = this.Drawing,
////                    Width = 0,
////                    Height = this.H,
////                    Length = this.ParentCulver.Length,
////                };
////                Bar bar = new Bar() { Name = "B06", Diameter = 2, Step = 12.5, Cover = 5, Length = m6Container.Length };
////                Bar hBar = new Bar() { Name = "B06a", Diameter = 2, Step = 12.5, Cover = 5, Length = m6Container.Height };
////                m6Container.Focus = PointsHelper.Offset(this.Center,
////                    -((NumberOfCellsToInt(this.NumberOfCells) - 1) * 0.5 * this.InternalWallThickness + NumberOfCellsToInt(this.NumberOfCells) * B * 0.5
////                    + bar.Cover + bar.Diameter / 2),
////                    H / 2);
////                M6 = new MeshSection(this.Drawing, m6Container, bar, hBar);
////                M6.CreateMeshSection();
////                this.Children.Add(M6);
////                MeshSectionsCollection.Add(M6);
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show(ex.ToString());
////            }
////            #endregion
////            #region M7
////            try
////            {
////                MeshContainer m7Container = new MeshContainer()
////                {
////                    Drawing = this.Drawing,
////                    Width = 0,
////                    Height = this.H,
////                    Length = this.ParentCulver.Length,
////                };
////                Bar bar = new Bar() { Name = "B07", Diameter = 2, Step = 12.5, Cover = 5, Length = m7Container.Length };
////                Bar hBar = new Bar() { Name = "B07a", Diameter = 2, Step = 12.5, Cover = 5, Length = m7Container.Height };
////                m7Container.Focus = PointsHelper.Offset(this.Center,
////                    ((NumberOfCellsToInt(this.NumberOfCells) - 1) * 0.5 * this.InternalWallThickness + NumberOfCellsToInt(this.NumberOfCells) * B * 0.5
////                    + bar.Cover + bar.Diameter / 2),
////                    H / 2);
////                M7 = new MeshSection(this.Drawing, m7Container, bar, hBar);
////                M7.CreateMeshSection();
////                this.Children.Add(M7);
////                MeshSectionsCollection.Add(M7);
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show(ex.ToString());
////            }
////            #endregion
////            #region M8
////            try
////            {
////                MeshContainer m8Container = new MeshContainer()
////                {
////                    Drawing = this.Drawing,
////                    Width = 0,
////                    Height = this.H,
////                    Length = this.ParentCulver.Length,
////                };
////                Bar bar = new Bar() { Name = "B08", Diameter = 2, Step = 12.5, Cover = 5, Length = m8Container.Length };
////                Bar hBar = new Bar() { Name = "B08a", Diameter = 2, Step = 12.5, Cover = 5, Length = m8Container.Height };
////                m8Container.Focus = PointsHelper.Offset(this.Center,
////                    ((NumberOfCellsToInt(this.NumberOfCells) - 1) * 0.5 * this.InternalWallThickness + NumberOfCellsToInt(this.NumberOfCells) * B * 0.5
////                    + ExternalWallThickness - bar.Cover - bar.Diameter / 2),
////                    H / 2);
////                M8 = new MeshSection(this.Drawing, m8Container, bar, hBar);
////                M8.CreateMeshSection();
////                this.Children.Add(M8);
////                MeshSectionsCollection.Add(M8);
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show(ex.ToString());
////            }
////            #region Cells Meshes
////            try
////            {
////                int cellAmount = NumberOfCellsToInt(this.NumberOfCells);
////                for (int i = 1; i < cellAmount; i++)
////                {
////                    #region Left mesh
////                    MeshContainer cellMeshContainer = new MeshContainer()
////                    {
////                        Drawing = this.Drawing,
////                        Width = 0,
////                        Height = this.H,
////                        Length = this.ParentCulver.Length,
////                    };
////                    Bar bar = new Bar() { Name = "B09", Diameter = 2, Step = 12.5, Cover = 5, Length = cellMeshContainer.Length };
////                    Bar hBar = new Bar() { Name = "B09a", Diameter = 2, Step = 12.5, Cover = 5, Length = cellMeshContainer.Height };
////                    cellMeshContainer.Focus = PointsHelper.Offset(this.Center,
////                    -((0.5 * cellAmount) * B + (0.5 * (cellAmount - 1)) * InternalWallThickness)
////                    + i * B + (i - 1) * InternalWallThickness + bar.Cover + bar.Diameter / 2,
////                    H / 2
////                    );
////                    MeshSection m = new MeshSection(this.Drawing, cellMeshContainer, bar, hBar);
////                    m.CreateMeshSection();
////                    this.Children.Add(m);
////                    MeshSectionsCollection.Add(m);
////                    #endregion

////                    #region Right mesh
////                    MeshContainer cellMeshContainer2 = new MeshContainer()
////                    {
////                        Drawing = this.Drawing,
////                        Width = 0,
////                        Height = this.H,
////                        Length = this.ParentCulver.Length,
////                    };
////                    Bar bar2 = new Bar() { Name = "B10", Diameter = 2, Step = 12.5, Cover = 5, Length = cellMeshContainer2.Length };
////                    Bar hBar2 = new Bar() { Name = "B10a", Diameter = 2, Step = 12.5, Cover = 5, Length = cellMeshContainer2.Height };
////                    cellMeshContainer2.Focus = PointsHelper.Offset(this.Center,
////                    -((0.5 * cellAmount) * B + (0.5 * (cellAmount - 1)) * InternalWallThickness)
////                    + i * B + i * InternalWallThickness - bar2.Cover - bar2.Diameter / 2,
////                    H / 2
////                    );
////                    MeshSection m2 = new MeshSection(this.Drawing, cellMeshContainer2, bar2, hBar2);
////                    m2.CreateMeshSection();
////                    this.Children.Add(m2);
////                    MeshSectionsCollection.Add(m2);
////                    #endregion
////                }
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show(ex.ToString());
////            }
////            #endregion
////            #endregion

////            #region Tails

////            if (this.HasApproachSlabOnTheLeft)
////            {
////                #region t1
////                try
////                {
////                    MeshContainer tailMesh1 = new MeshContainer()
////                    {
////                        Drawing = this.Drawing,
////                        Width = B1Left,
////                        Height = H1Left,
////                        Length = this.ParentCulver.Length,
////                    };
////                    Bar bar = new Bar() { Name = "T01", Diameter = 2, Step = 12.5, Cover = 5, Length = tailMesh1.Length };
////                    Bar hBar = new Bar() { Name = "T01a", Diameter = 2, Step = 12.5, Cover = 5, Length = tailMesh1.Width };
////                    tailMesh1.Focus = PointsHelper.Offset(this.Center,
////                    -((NumberOfCellsToInt(this.NumberOfCells) - 1) * 0.5 * this.InternalWallThickness + NumberOfCellsToInt(this.NumberOfCells) * B * 0.5
////                    + ExternalWallThickness + 0.5 * B1Left) + bar.Cover, H + TopThickness - H3Left - H2Left - 0.5 * H1Left);
////                    MeshSection m = new MeshSection(this.Drawing, tailMesh1, bar, hBar, Direction.Down);
////                    m.CreateMeshSection();
////                    //this.Children.Add(m);
////                    //MeshSectionsCollection.Add(m);
////                }
////                catch (Exception ex)
////                {
////                    MessageBox.Show(ex.ToString());
////                }
////                #endregion
////                #region t2
////                try
////                {
////                    MeshContainer tailMesh1 = new MeshContainer()
////                    {
////                        Drawing = this.Drawing,
////                        Width = 0,
////                        Height = H2Left,
////                        Length = this.ParentCulver.Length,

////                        Focus = PointsHelper.Offset(this.Center,
////                    -((NumberOfCellsToInt(this.NumberOfCells) - 1) * 0.5 * this.InternalWallThickness + NumberOfCellsToInt(this.NumberOfCells) * B * 0.5
////                    + ExternalWallThickness + B1Left), H + TopThickness - H3Left - 0.5*H2Left), // Trọng tâm
////                    };
////                    // Top meshes
////                    Bar bar = new Bar() { Name = "T01", Diameter = 2, Step = 10, Cover = 5, Length = tailMesh1.Length };
////                    Bar hBar = new Bar() { Name = "T01a", Diameter = 2, Step = 12.5, Cover = 5, Length = tailMesh1.Height };
////                    tailMesh1.Focus = PointsHelper.Offset(tailMesh1.Focus, bar.Cover + 0.5 * bar.Diameter, 0);
////                    MeshSection m = new MeshSection(this.Drawing, tailMesh1, bar, hBar);
////                    m.CreateMeshSection();
////                    //this.Children.Add(m);
////                    //MeshSectionsCollection.Add(m);
////                }
////                catch (Exception ex)
////                {
////                    MessageBox.Show(ex.ToString());
////                }
////                #endregion
////                #region t3
////                try
////                {
////                    MeshContainer tailMesh1 = new MeshContainer()
////                    {
////                        Drawing = this.Drawing,
////                        Width = B1Left,
////                        Height = 0,
////                        Length = this.ParentCulver.Length,

////                        Focus = PointsHelper.Offset(this.Center,
////                    -((NumberOfCellsToInt(this.NumberOfCells) - 1) * 0.5 * this.InternalWallThickness + NumberOfCellsToInt(this.NumberOfCells) * B * 0.5
////                    + ExternalWallThickness + 0.5*B1Left), H + TopThickness - H3Left), // Trọng tâm
////                    };
////                    // Top meshes
////                    Bar bar = new Bar() { Name = "T01", Diameter = 2, Step = 10, Cover = 5, Length = tailMesh1.Length };
////                    Bar hBar = new Bar() { Name = "T01a", Diameter = 2, Step = 10, Cover = 5, Length = tailMesh1.Width };
////                    tailMesh1.Focus = PointsHelper.Offset(tailMesh1.Focus, 0, -bar.Cover-0.5*bar.Diameter);
////                    MeshSection m = new MeshSection(this.Drawing, tailMesh1, bar, hBar);
////                    m.CreateMeshSection();
////                    //this.Children.Add(m);
////                    //MeshSectionsCollection.Add(m);
////                }
////                catch (Exception ex)
////                {
////                    MessageBox.Show(ex.ToString());
////                }
////                #endregion
                
////            }
////            #endregion
////            // Update Dependencies
////            foreach (MeshSection m in MeshSectionsCollection)
////            {
////                Dependencies.Add(m);
////            }
////        }
////        #endregion

////        #region Test
////        [PropertyGridVisible]
////        [PropertyGridName("Draw all Meshes")]
////        public void DrawMeshM1()
////        {
////            int index = 0;
////            foreach (MeshSection m in this.MeshSectionsCollection)
////            {
////                double distance = m.Width >= m.Height ? m.Width : m.Height;
////                RectangleMesh rm1 = new RectangleMesh(this.Drawing, m, PointsHelper.Offset(m.InsertPoint, index * distance + 20, 0));
////                rm1.Name = m.Name;
////                Actions.Add(Drawing, rm1);
////                index++;
////            }
////        }

////        [PropertyGridVisible]
////        [PropertyGridName("Rectangle Mesh")]
////        public void AddMesh()
////        {
////            try
////            {
////                MeshContainer mesh8Container = new MeshContainer()
////                {
////                    Drawing = this.Drawing,
////                    Width = 500,
////                    Height = 0,
////                    Focus = PointsHelper.Offset(this.Center, 0, -200) // Trọng tâm
////                };
////                // Top meshes
////                Bar bar = new Bar() { Name = "B08", Diameter = 2, Step = 12.5, Cover = 5, Length = mesh8Container.Width};
////                Bar hBar = new Bar() { Name = "B08a", Diameter = 2, Step = 12.5, Cover = 5, Length = mesh8Container.Width };
////                MeshSection m8 = new MeshSection(this.Drawing, mesh8Container, bar, hBar);
////                // Xác định lại kích thước thanh

////                bar.Length = m8.Width;

////                m8.CreateMeshSection();
////                Actions.Add(Drawing, m8);
////                //this.Children.Add(m8);
////                MeshSectionsCollection.Add(m8);
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show(ex.ToString());
////            }   
////        }
////        #endregion
////    }
////}
