using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls; // Test context menu

namespace DynamicGeometry
{
    [Category(BehaviorCategories.Selection)]
    [Order(1)]
    public partial class Dragger : Behavior
    {
        protected List<IMovable> moving = null;
        IFigure found = null;
        List<IFigure> toRecalculate = null;
        Point offsetFromFigureLeftTopCorner;
        protected Point oldCoordinates;
        protected Point coordinatesOnMouseDown;
        bool startedMoving = false;

        bool mousePressed = false; //CC
        //MouseButton pressedButton = MouseButton.Left; //CC

        public override void MouseDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = this.ParentCanvas as Canvas;
            bool isLocked = false;

            moving = new List<IMovable>();
            IEnumerable<IFigure> roots = null;
#if !SILVERLIGHT

            if (e.ChangedButton == MouseButton.Left)
            {
                offsetFromFigureLeftTopCorner = Coordinates(e, false, false, false);
                oldCoordinates = offsetFromFigureLeftTopCorner;
                coordinatesOnMouseDown = offsetFromFigureLeftTopCorner;
                startedMoving = false;

                //found = Drawing.Figures.MouseHover(offsetFromFigureLeftTopCorner);

                found = Drawing.Figures.HitTest(offsetFromFigureLeftTopCorner);
                IMovable oneMovable = found as IMovable;
                //canvas.Cursor = Cursors.Arrow;
                if (oneMovable != null)
                {
                    if (!found.Locked)
                    {
                        if (oneMovable is IPoint)
                        {
                            // when we drag a point, we want it to snap to the cursor
                            // so that the point center is directly under the tip of the mouse
                            offsetFromFigureLeftTopCorner = new Point();
                            oldCoordinates = oneMovable.Coordinates;
                        }
                        else
                        {
                            // however when we drag other stuff (such as text labels)
                            // we want the mouse to always touch the part of the draggable
                            // where it first touched during MouseDown
                            // we don't want the draggable to "snap" to the cursor like points do
                            offsetFromFigureLeftTopCorner = offsetFromFigureLeftTopCorner.Minus(oneMovable.Coordinates);
                        }
                        roots = DependencyAlgorithms.FindRoots(f => f.Dependents, found);
                        if (roots.All(root => (!root.Locked)))
                        {
                            moving.Add(oneMovable);
                            roots = found.AsEnumerable();
                        }
                        else
                        {
                            isLocked = true;
                        }
                    }
                    else
                    {
                        isLocked = true;
                    }
                }
                else if (found != null && !found.Locked)
                {
                    if (!found.Locked)
                    {
                        roots = DependencyAlgorithms.FindRoots(f => f.Dependencies, found);
                        if (roots.All(root => root is IMovable))
                        {
                            if (roots.All(root => ((IMovable)root).AllowMove()))
                            {
                                moving.AddRange(roots.OfType<IMovable>());
                            }
                            else
                            {
                                isLocked = true;
                            }
                        }
                    }
                    else
                    {
                        isLocked = true;
                    }
                }

                if (roots != null)
                {
                    toRecalculate = DependencyAlgorithms.FindDescendants(f => f.Dependents, roots);
                    toRecalculate.Reverse();
                }
                else
                {
                    toRecalculate = null;
                }

            }
            // Chuột giữa
            else if (e.ChangedButton == MouseButton.Middle)
            {
                canvas.Cursor = Cursors.Hand;
                
                // zoom extend
                if (e.ClickCount == 2)
                {
                    Drawing.CoordinateSystem.ZoomExtend();
                    return;
                }
                // Di chuyển lưới
                if (moving.IsEmpty() && !isLocked && !Drawing.CoordinateGrid.Locked)
                {
                    moving.Add(Drawing.CoordinateSystem);
                    //var allFigures = Drawing.Figures.GetAllFiguresRecursive();
                    //roots = DependencyAlgorithms.FindRoots(f => f.Dependencies, allFigures);
                    //moving.AddRange(roots.OfType<IMovable>());
                    //roots = null;
                    //toRecalculate = null; // Figures;
                }
            }
#endif
            
                // Chuột phải hiển thị context menu
            else if(e.ChangedButton == MouseButton.Right)
            {
                ContextMenu menu = new ContextMenu();
                MenuItem file = new MenuItem() { Header = "Reactor" };
                MenuItem options = new MenuItem() { Header = "Options", InputGestureText = "Ctrl+O" };
                options.Click += options_Click;

                menu.Items.Add(file);
                menu.Items.Add(new Separator());
                menu.Items.Add(options);

                var open = new MenuItem() { Header = "_Open", InputGestureText = "Ctrl+O" };
                open.Click += Open_Click;
                var save = new MenuItem() { Header = "_Save...", InputGestureText = "Ctrl+S" };
                //save.Click += Save_Click;
                var print = new MenuItem() { Header = "Print" };
                //print.Click += Print_Click;
                var exit = new MenuItem() { Header = "Exit" };
                //exit.Click += Exit_Click;

                var items = new UIElement[] {
                open,
                save,
                print,
                new Separator(),
                exit
            };

                file.ItemsSource = items;

                canvas.ContextMenu = menu;
                
                
            }
            
        }

        private void options_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            
        }

       
        public delegate void DraggerMouseMoveHandler(Point previousPoint, ref Point currentPoint);

        public event DraggerMouseMoveHandler PreviewMouseMoveCoordinates;

        IFigure currentFigure;
        public override void MouseMove(object sender, MouseEventArgs e)
        {
            //var canvas = this.ParentCanvas as Canvas;
            //currentFigure = null;
            var currentCoordinates = Coordinates(e);
            currentCoordinates = AdjustCoordinates(currentCoordinates);
            // CC
            //Point thisPoint = Coordinates(e, false, false, false);
            //var drawingHost = this.ParentCanvas.Parent as DrawingHost;
            //if (Settings.ShowCoordinationsWhenMouseHover)
            //{
                
            //    drawingHost.StatusBar.CoordinatesText = thisPoint.ToString();
            //    drawingHost.StatusBar.Visibility = Visibility.Visible;
            //}

            //if (!mousePressed)
            //{
            //    //canvas.Cursor = Cursors.Arrow;

            //    currentFigure = Drawing.Figures.MouseHover(thisPoint);
            //    if (currentFigure != null)
            //    {
            //        currentFigure.IsMouseHover = true;
            //        //drawingHost.CurrentDrawing.RaiseStatusNotification(currentFigure.Name);
            //        //currentFigure.Recalculate();
            //    }
            //    else
            //    {
            //        //currentFigure.RecalculateAndUpdateVisual();
            //        //drawingHost.CurrentDrawing.ClearStatus();
            //        //drawingHost.CurrentDrawing.Figures.ClearSelection();
            //    }
            //}
            //
            //canvas.Cursor = Cursors.Hand;

            if (!startedMoving)
            {
                if (currentCoordinates == coordinatesOnMouseDown)
                {
                    return;
                }
                startedMoving = true;
            }
            if (!moving.IsEmpty())
            {
                var offset = currentCoordinates.Minus(oldCoordinates);
                Actions.Move(Drawing, moving, offset, toRecalculate);
            }

            // OK attention here. This is a very tricky spot. At the beginning
            // of this method, we call Coordinates(e) to get the logical mouse
            // coordinates. We could just reuse currentCoordinates, BUT!
            // If you're dragging the coordinate plane itself, the Origin changes
            // so you'll have to re-get the point coordinates in the new 
            // coordinate system.
            oldCoordinates = Coordinates(e);
            if (moving != null
                && moving.Count == 1
                && moving[0] is IPoint
                && found != null
                && found == moving[0])
            {
                oldCoordinates = moving[0].Coordinates;
            }
        }

        private Point AdjustCoordinates(Point currentCoordinates)
        {
            if (PreviewMouseMoveCoordinates != null)
            {
                PreviewMouseMoveCoordinates(oldCoordinates, ref currentCoordinates);
            }
            return currentCoordinates;
        }

        public override void MouseUp(object sender, MouseButtonEventArgs e)
        {
            //CC
            var canvas = this.ParentCanvas as Canvas;
            canvas.Cursor = Cursors.Arrow;
            // Nếu chuột đang pressed
            mousePressed = e.ButtonState == MouseButtonState.Pressed;
            //

            if (Coordinates(e) == coordinatesOnMouseDown)
            {
                UpdateSelection();
                Drawing.RaiseSelectionChanged(Drawing.GetSelectedFigures());
            }

            startedMoving = false;
            moving = null;
            found = null;
        }

#if !PLAYER

        public override void KeyDown(object sender, KeyEventArgs e)
        {
            var selectedFigures = Drawing.GetSelectedFigures();
            if (e.Key == Key.Delete && !selectedFigures.IsEmpty())
            {
                Drawing.DeleteSelection();
                e.Handled = true;
            }
        }

#endif

        private void UpdateSelection()
        {
            if (IsCtrlPressed())
            {
                if (found != null)
                {
                    found.Selected = !found.Selected;
                }
            }
            else
            {
                Drawing.Figures.ClearSelection();
                if (found != null)
                {
                    found.Selected = true;
                }
            }
        }

        public override FrameworkElement CreateIcon()
        {
            Point[] points = 
            {
                new Point(10, 5),
                new Point(10, 21),
                new Point(14, 17),
                new Point(18, 25),
                new Point(19, 25),
                new Point(20, 24),
                new Point(17, 17),
                new Point(17, 16),
                new Point(21, 16),
                new Point(10, 5)
            };
            var builder = IconBuilder.BuildIcon();
            var polygon = builder.AddPolygon(
                    points.Select(p => new Point(p.X / 32, p.Y / 32)));
            polygon.Fill = new SolidColorBrush(Colors.White);
            polygon.Stroke = new SolidColorBrush(Colors.Black);
            return builder.Canvas;
        }

        public override string Name
        {
            get { return "Drag"; }
        }

        public override string HintText
        {
            get { return "Use this tool to drag points and figures."; }
        }
    }
}