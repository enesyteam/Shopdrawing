using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DynamicGeometry;
using System.IO;
using System.ComponentModel;

namespace Shopdrawing.Controls
{
    /// <summary>
    /// Interaction logic for SdrToolbar.xaml
    /// </summary>
    public partial class SdrToolbar : UserControl, INotifyPropertyChanged
    {
        #region Command
        #region Shopdrawing Commands
        /// <summary>Command: New Game</summary>
        public static readonly RoutedUICommand ShowGridCommand = new RoutedUICommand("Show _Grid", "ShowGrid", typeof(SdrToolbar));
        /// <summary>Command: Undo</summary>
        public static readonly RoutedUICommand UndoCommand = new RoutedUICommand("_Undo", "Undo", typeof(SdrToolbar));
        public static readonly RoutedUICommand LockCommand = new RoutedUICommand("_Lock", "Lock", typeof(SdrToolbar));
        /// <summary>Command: Redo</summary>
        public static readonly RoutedUICommand RedoCommand = new RoutedUICommand("_Redo", "Redo", typeof(SdrToolbar));
        /// <summary>Command: Save Game</summary>
        public static readonly RoutedUICommand SaveGameCommand = new RoutedUICommand("_Save Game", "SaveGame", typeof(SdrToolbar));
        /// <summary>Command: Load Game</summary>
        public static readonly RoutedUICommand LoadFileCommand = new RoutedUICommand("_Load Game", "LoadGame", typeof(SdrToolbar));
        #endregion

        ///// <summary>Command: New Game</summary>
        //public static readonly RoutedUICommand NewGameCommand = new RoutedUICommand("_New Game", "NewGame", typeof(SdrToolbar));

        ///// <summary>Command: Create Game</summary>
        //public static readonly RoutedUICommand CreateGameCommand = new RoutedUICommand("_Create Game", "CreateGame", typeof(SdrToolbar));

        ///// <summary>Command: Save Game in PGN</summary>
        //public static readonly RoutedUICommand SaveGameInPGNCommand = new RoutedUICommand("Save Game _To PGN", "SaveGameToPGN", typeof(SdrToolbar));
        ///// <summary>Command: Quit</summary>
        //public static readonly RoutedUICommand QuitCommand = new RoutedUICommand("_Quit", "Quit", typeof(SdrToolbar));

        ///// <summary>Command: Hint</summary>
        //public static readonly RoutedUICommand HintCommand = new RoutedUICommand("_Hint", "Hint", typeof(SdrToolbar));
        
       
        ///// <summary>Command: Revert Board</summary>
        //public static readonly RoutedUICommand RevertBoardCommand = new RoutedUICommand("Revert _Board", "RevertBoard", typeof(SdrToolbar));
        ///// <summary>Command: Player Against Player</summary>
        //public static readonly RoutedUICommand PlayerAgainstPlayerCommand = new RoutedUICommand("_Player Against Player", "PlayerAgainstPlayer", typeof(SdrToolbar));
        ///// <summary>Command: Automatic Play</summary>
        //public static readonly RoutedUICommand AutomaticPlayCommand = new RoutedUICommand("_Automatic Play", "AutomaticPlay", typeof(SdrToolbar));
        ///// <summary>Command: Fast Automatic Play</summary>
        //public static readonly RoutedUICommand FastAutomaticPlayCommand = new RoutedUICommand("_Fast Automatic Play", "FastAutomaticPlay", typeof(SdrToolbar));
        ///// <summary>Command: Cancel Play</summary>
        //public static readonly RoutedUICommand CancelPlayCommand = new RoutedUICommand("_Cancel Play", "CancelPlay", typeof(SdrToolbar));
        ///// <summary>Command: Design Mode</summary>
        //public static readonly RoutedUICommand DesignModeCommand = new RoutedUICommand("_Design Mode", "DesignMode", typeof(SdrToolbar));

        ///// <summary>Command: Search Mode</summary>
        //public static readonly RoutedUICommand SearchModeCommand = new RoutedUICommand("_Search Mode...", "SearchMode", typeof(SdrToolbar));
        ///// <summary>Command: Flash Piece</summary>
        //public static readonly RoutedUICommand FlashPieceCommand = new RoutedUICommand("_Flash Piece", "FlashPiece", typeof(SdrToolbar));
        ///// <summary>Command: PGN Notation</summary>
        //public static readonly RoutedUICommand PGNNotationCommand = new RoutedUICommand("_PGN Notation", "PGNNotation", typeof(SdrToolbar));
        ///// <summary>Command: Board Settings</summary>
        //public static readonly RoutedUICommand BoardSettingCommand = new RoutedUICommand("_Board Settings...", "BoardSettings", typeof(SdrToolbar));

        ///// <summary>Command: Create a Book</summary>
        //public static readonly RoutedUICommand CreateBookCommand = new RoutedUICommand("_Create a Book...", "CreateBook", typeof(SdrToolbar));
        ///// <summary>Command: Filter a PGN File</summary>
        //public static readonly RoutedUICommand FilterPGNFileCommand = new RoutedUICommand("_Filter a PGN File...", "FilterPGNFile", typeof(SdrToolbar));
        ///// <summary>Command: Test Board Evaluation</summary>
        //public static readonly RoutedUICommand TestBoardEvaluationCommand = new RoutedUICommand("_Test Board Evaluation...", "TestBoardEvaluation", typeof(SdrToolbar));

        ///// <summary>Command: Test Board Evaluation</summary>
        //public static readonly RoutedUICommand AboutCommand = new RoutedUICommand("_About...", "About", typeof(SdrToolbar));


        ///// <summary>Command: Cancel Play</summary>
        //public static readonly RoutedUICommand TestCommand = new RoutedUICommand("_Test", "Test", typeof(SdrToolbar));

        /// <summary>List of all supported commands</summary>
        private static readonly RoutedUICommand[] m_arrCommands = new RoutedUICommand[] {

            ShowGridCommand,
            LockCommand,
            UndoCommand,
            RedoCommand,
            SaveGameCommand,
            LoadFileCommand
        };
        #endregion
        #region Command Handling
        /// <summary>
        /// Executes the specified command
        /// </summary>
        /// <param name="sender">   Sender object</param>
        /// <param name="e">        Routed event argument</param>
        public virtual void OnExecutedCmd(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == LoadFileCommand)
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = drawingFileFilter;

                if (openFileDialog.ShowDialog().Value)
                {
                    this.Load(openFileDialog.FileName);
                }
            }
            else if (e.Command == ShowGridCommand)
            {
                Drawing.CoordinateGrid.Visible = !Drawing.CoordinateGrid.Visible;
            }
            else if (e.Command == SaveGameCommand)
            {
                Drawing.ToDxf();
            }
            else if (e.Command == LockCommand)
            {
                var drawingHost = Drawing.Canvas.Parent as DrawingHost;
                if (drawingHost != null)
                {
                    foreach (IFigure f in drawingHost.CurrentDrawing.GetSelectedFigures())
                    {
                        f.Locked = !f.Locked;
                    }
                }
            }
            else if (e.Command == UndoCommand)
            {
                var drawingHost = Drawing.Canvas.Parent as DrawingHost;
                if (drawingHost != null)
                {
                    drawingHost.DrawingControl.Undo();
                }
            }
            else if (e.Command == RedoCommand)
            {
                var drawingHost = Drawing.Canvas.Parent as DrawingHost;
                if (drawingHost != null)
                {
                    drawingHost.DrawingControl.Redo();
                }
            }
            else
            {
                e.Handled = false;
            }
        }

        private void NewGame()
        {
            Drawing.RaiseStatusNotification("New command");
        }

        /// <summary>
        /// Determine if a command can be executed
        /// </summary>
        /// <param name="sender">   Sender object</param>
        /// <param name="e">        Routed event argument</param>
        public virtual void OnCanExecuteCmd(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Drawing == null) return;
            if (e.Command == SaveGameCommand)
            {
                e.CanExecute = Drawing.Figures.Count > 1;
            }
            else if (e.Command == LoadFileCommand)
            {
                e.CanExecute = true;
            }
            else if (e.Command == ShowGridCommand)
            {
                e.CanExecute = true;
            }
            else if (e.Command == UndoCommand)
            {
                e.CanExecute = Drawing.ActionManager.CanUndo;
            }
            else if (e.Command == LockCommand)
            {
                e.CanExecute = Drawing.GetSelectedFigures().Count() > 0;
            }
            else if (e.Command == RedoCommand)
            {
                e.CanExecute = Drawing.ActionManager.CanRedo;
            }
            else
            {
                e.Handled = false;
            }
        }
        #endregion
        public SdrToolbar()
        {
            InitializeComponent();
            ExecutedRoutedEventHandler onExecutedCmd;
            CanExecuteRoutedEventHandler onCanExecuteCmd;
            onExecutedCmd = new ExecutedRoutedEventHandler(OnExecutedCmd);
            onCanExecuteCmd                     = new CanExecuteRoutedEventHandler(OnCanExecuteCmd);
            foreach (RoutedUICommand cmd in m_arrCommands)
            {
                CommandBindings.Add(new CommandBinding(cmd, onExecutedCmd, onCanExecuteCmd));
            }
        }

        public void Load(string path)
        {
            if (path != null && File.Exists(path))
            {
                switch (System.IO.Path.GetExtension(path))
                {
                    case ".DXF":
                        break;
                
                    default:
                        HandleExceptions(() =>
                            Drawing = DynamicGeometry.Drawing.Load(path, Drawing.Canvas)
                        );
                        break;
                }
            }
        }
        public void HandleExceptions(Action code)
        {
            try
            {
                code();
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //private DynamicGeometry.Drawing _drawing = null;
        //public DynamicGeometry.Drawing Drawing 
        //{
        //    get { return _drawing; }
        //    set { _drawing = value;
        //    OnPropertyChanged("Drawing");       
        //    }
        //}

        /// <summary>
        /// TestTimer Dependency Property
        /// </summary>
        public static readonly DependencyProperty DrawingProperty =
            DependencyProperty.Register("Drawing", typeof(DynamicGeometry.Drawing), typeof(SdrToolbar),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the TestTimer property.  This dependency property 
        /// indicates a test timer that elapses evry one second (just for binding test).
        /// </summary>
        public DynamicGeometry.Drawing Drawing
        {
            get { return (DynamicGeometry.Drawing)GetValue(DrawingProperty); }
            set { SetValue(DrawingProperty, value); }
        }

        const string extension = "sdr";
        const string dxfExtension = "dxf";
        const string lgfFileFilter = "Shopdrawing file (*." + extension + ")|*." + extension;
        const string pngFileFilter = "PNG image (*.png)|*.png";
        const string bmpFileFilter = "BMP image (*.bmp)|*.bmp";
        const string dxfFileFilter = "DXF file (*.dxf)|*.dxf";
        const string allFileFilter = "All files (*.*)|*.*";
        const string drawingFileFilter = lgfFileFilter + "|" + dxfFileFilter + "|" + allFileFilter;
        const string fileFilter = lgfFileFilter
                          + "|" + pngFileFilter
                          + "|" + bmpFileFilter
                          + "|" + allFileFilter;


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
