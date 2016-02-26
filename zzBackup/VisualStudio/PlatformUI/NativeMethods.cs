// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.NativeMethods
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Microsoft.VisualStudio.PlatformUI
{
  internal static class NativeMethods
  {
    public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
    public static readonly IntPtr HWND_TOP = new IntPtr(0);
    public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
    private const byte KeyDown = (byte) 128;
    public const uint MAPVK_VK_TO_VSC = 0U;
    public const int VK_LBUTTON = 1;
    public const int VK_RBUTTON = 2;
    public const int VK_MBUTTON = 4;
    public const int VK_SHIFT = 16;
    public const int VK_CONTROL = 17;
    public const int VK_MENU = 18;
    public const int VK_LSHIFT = 160;
    public const int VK_RSHIFT = 161;
    public const int VK_LCONTROL = 162;
    public const int VK_RCONTROL = 163;
    public const int VK_LMENU = 164;
    public const int VK_RMENU = 165;
    public const int VK_LWIN = 91;
    public const int VK_RWIN = 92;
    public const int VK_F1 = 112;
    internal const int PICTYPE_UNINITIALIZED = -1;
    internal const int PICTYPE_NONE = 0;
    internal const int PICTYPE_BITMAP = 1;
    internal const int PICTYPE_METAFILE = 2;
    internal const int PICTYPE_ICON = 3;
    internal const int PICTYPE_ENHMETAFILE = 4;
    internal const int BITMAPINFO_MAX_COLORSIZE = 256;
    internal const int DIB_RGB_COLORS = 0;
    internal const int DIB_PAL_COLORS = 1;
    internal const int BI_RGB = 0;
    internal const int BI_RLE8 = 1;
    internal const int BI_RLE4 = 2;
    internal const int BI_BITFIELDS = 3;
    internal const int BI_JPEG = 4;
    internal const int BI_PNG = 5;
    public const int TRUE = 1;
    public const int FALSE = 0;
    public const int ILD_NORMAL = 0;
    public const int ILD_TRANSPARENT = 1;
    public const int ILD_MASK = 16;
    public const int ILD_IMAGE = 32;
    public const int ILD_ROP = 64;
    public const int ILD_BLEND25 = 2;
    public const int ILD_BLEND50 = 4;
    public const int ILD_OVERLAYMASK = 3840;
    public const int ILD_SELECTED = 4;
    public const int ILD_FOCUS = 2;
    public const int ILD_BLEND = 4;
    public const int GW_FIRST = 0;
    public const int GW_LAST = 1;
    public const int GW_HWNDNEXT = 2;
    public const int GW_HWNDPREV = 3;
    public const int GW_OWNER = 4;
    public const int GW_CHILD = 5;
    public const int HTNOWHERE = 0;
    public const int HTCLIENT = 1;
    public const int HTCAPTION = 2;
    public const int HTLEFT = 10;
    public const int HTRIGHT = 11;
    public const int HTTOP = 12;
    public const int HTTOPLEFT = 13;
    public const int HTTOPRIGHT = 14;
    public const int HTBOTTOM = 15;
    public const int HTBOTTOMLEFT = 16;
    public const int HTBOTTOMRIGHT = 17;
    public const int ICON_BIG = 1;
    public const int ICON_SMALL = 0;
    public const int LWA_COLORKEY = 1;
    public const int LWA_ALPHA = 2;
    public const int LOGPIXELSX = 88;
    public const int LOGPIXELSY = 90;
    public const uint MONITOR_DEFAULTTONULL = 0U;
    public const uint MONITOR_DEFAULTTOPRIMARY = 1U;
    public const int MONITOR_DEFAULTTONEAREST = 2;
    public const int SW_HIDE = 0;
    public const int SW_SHOWNORMAL = 1;
    public const int SW_NORMAL = 1;
    public const int SW_SHOWMINIMIZED = 2;
    public const int SW_SHOWMAXIMIZED = 3;
    public const int SW_MAXIMIZE = 3;
    public const int SW_SHOWNOACTIVATE = 4;
    public const int SW_SHOW = 5;
    public const int SW_MINIMIZE = 6;
    public const int SW_SHOWMINNOACTIVE = 7;
    public const int SW_SHOWNA = 8;
    public const int SW_RESTORE = 9;
    public const int SW_SHOWDEFAULT = 10;
    public const int SW_FORCEMINIMIZE = 11;
    public const int SW_MAX = 11;
    public const int WA_INACTIVE = 0;
    public const int WA_ACTIVE = 1;
    public const int WA_CLICKACTIVE = 2;
    public const int SC_SIZE = 61440;
    public const int SC_MOVE = 61456;
    public const int SC_MINIMIZE = 61472;
    public const int SC_MAXIMIZE = 61488;
    public const int SC_NEXTWINDOW = 61504;
    public const int SC_PREVWINDOW = 61520;
    public const int SC_CLOSE = 61536;
    public const int SC_VSCROLL = 61552;
    public const int SC_HSCROLL = 61568;
    public const int SC_MOUSEMENU = 61584;
    public const int SC_KEYMENU = 61696;
    public const int SC_ARRANGE = 61712;
    public const int SC_RESTORE = 61728;
    public const int SC_TASKLIST = 61744;
    public const int SC_SCREENSAVE = 61760;
    public const int SC_HOTKEY = 61776;
    public const int SC_DEFAULT = 61792;
    public const int SC_MONITORPOWER = 61808;
    public const int SC_CONTEXTHELP = 61824;
    public const int SC_SEPARATOR = 61455;
    public const int SM_SWAPBUTTON = 23;
    public const int SWP_NOSIZE = 1;
    public const int SWP_NOMOVE = 2;
    public const int SWP_NOZORDER = 4;
    public const int SWP_NOREDRAW = 8;
    public const int SWP_NOACTIVATE = 16;
    public const int SWP_FRAMECHANGED = 32;
    public const int SWP_SHOWWINDOW = 64;
    public const int SWP_HIDEWINDOW = 128;
    public const int SWP_NOCOPYBITS = 256;
    public const int SWP_NOOWNERZORDER = 512;
    public const int SWP_NOSENDCHANGING = 1024;
    public const int SWP_DEFERERASE = 8192;
    public const int SWP_ASYNCWINDOWPOS = 16384;
    public const int WM_NULL = 0;
    public const int WM_CREATE = 1;
    public const int WM_DESTROY = 2;
    public const int WM_MOVE = 3;
    public const int WM_SIZE = 5;
    public const int WM_ACTIVATE = 6;
    public const int WM_SETFOCUS = 7;
    public const int WM_KILLFOCUS = 8;
    public const int WM_ENABLE = 10;
    public const int WM_SETREDRAW = 11;
    public const int WM_SETTEXT = 12;
    public const int WM_GETTEXT = 13;
    public const int WM_GETTEXTLENGTH = 14;
    public const int WM_PAINT = 15;
    public const int WM_CLOSE = 16;
    public const int WM_QUERYENDSESSION = 17;
    public const int WM_QUERYOPEN = 19;
    public const int WM_ENDSESSION = 22;
    public const int WM_QUIT = 18;
    public const int WM_ERASEBKGND = 20;
    public const int WM_SYSCOLORCHANGE = 21;
    public const int WM_SHOWWINDOW = 24;
    public const int WM_WININICHANGE = 26;
    public const int WM_SETTINGCHANGE = 26;
    public const int WM_DEVMODECHANGE = 27;
    public const int WM_ACTIVATEAPP = 28;
    public const int WM_FONTCHANGE = 29;
    public const int WM_TIMECHANGE = 30;
    public const int WM_CANCELMODE = 31;
    public const int WM_SETCURSOR = 32;
    public const int WM_MOUSEACTIVATE = 33;
    public const int WM_CHILDACTIVATE = 34;
    public const int WM_QUEUESYNC = 35;
    public const int WM_GETMINMAXINFO = 36;
    public const int WM_PAINTICON = 38;
    public const int WM_ICONERASEBKGND = 39;
    public const int WM_NEXTDLGCTL = 40;
    public const int WM_SPOOLERSTATUS = 42;
    public const int WM_DRAWITEM = 43;
    public const int WM_MEASUREITEM = 44;
    public const int WM_DELETEITEM = 45;
    public const int WM_VKEYTOITEM = 46;
    public const int WM_CHARTOITEM = 47;
    public const int WM_SETFONT = 48;
    public const int WM_GETFONT = 49;
    public const int WM_SETHOTKEY = 50;
    public const int WM_GETHOTKEY = 51;
    public const int WM_QUERYDRAGICON = 55;
    public const int WM_COMPAREITEM = 57;
    public const int WM_GETOBJECT = 61;
    public const int WM_COMPACTING = 65;
    public const int WM_COMMNOTIFY = 68;
    public const int WM_WINDOWPOSCHANGING = 70;
    public const int WM_WINDOWPOSCHANGED = 71;
    public const int WM_POWER = 72;
    public const int WM_COPYDATA = 74;
    public const int WM_CANCELJOURNAL = 75;
    public const int WM_NOTIFY = 78;
    public const int WM_INPUTLANGCHANGEREQUEST = 80;
    public const int WM_INPUTLANGCHANGE = 81;
    public const int WM_TCARD = 82;
    public const int WM_HELP = 83;
    public const int WM_USERCHANGED = 84;
    public const int WM_NOTIFYFORMAT = 85;
    public const int WM_CONTEXTMENU = 123;
    public const int WM_STYLECHANGING = 124;
    public const int WM_STYLECHANGED = 125;
    public const int WM_DISPLAYCHANGE = 126;
    public const int WM_GETICON = 127;
    public const int WM_SETICON = 128;
    public const int WM_NCCREATE = 129;
    public const int WM_NCDESTROY = 130;
    public const int WM_NCCALCSIZE = 131;
    public const int WM_NCHITTEST = 132;
    public const int WM_NCPAINT = 133;
    public const int WM_NCACTIVATE = 134;
    public const int WM_GETDLGCODE = 135;
    public const int WM_SYNCPAINT = 136;
    public const int WM_NCMOUSEMOVE = 160;
    public const int WM_NCLBUTTONDOWN = 161;
    public const int WM_NCLBUTTONUP = 162;
    public const int WM_NCLBUTTONDBLCLK = 163;
    public const int WM_NCRBUTTONDOWN = 164;
    public const int WM_NCRBUTTONUP = 165;
    public const int WM_NCRBUTTONDBLCLK = 166;
    public const int WM_NCMBUTTONDOWN = 167;
    public const int WM_NCMBUTTONUP = 168;
    public const int WM_NCMBUTTONDBLCLK = 169;
    public const int WM_NCXBUTTONDOWN = 171;
    public const int WM_NCXBUTTONUP = 172;
    public const int WM_NCXBUTTONDBLCLK = 173;
    public const int WM_INPUT = 255;
    public const int WM_KEYFIRST = 256;
    public const int WM_KEYDOWN = 256;
    public const int WM_KEYUP = 257;
    public const int WM_CHAR = 258;
    public const int WM_DEADCHAR = 259;
    public const int WM_SYSKEYDOWN = 260;
    public const int WM_SYSKEYUP = 261;
    public const int WM_SYSCHAR = 262;
    public const int WM_SYSDEADCHAR = 263;
    public const int WM_UNICHAR = 265;
    public const int WM_KEYLAST = 264;
    public const int WM_IME_STARTCOMPOSITION = 269;
    public const int WM_IME_ENDCOMPOSITION = 270;
    public const int WM_IME_COMPOSITION = 271;
    public const int WM_IME_KEYLAST = 271;
    public const int WM_INITDIALOG = 272;
    public const int WM_COMMAND = 273;
    public const int WM_SYSCOMMAND = 274;
    public const int WM_TIMER = 275;
    public const int WM_HSCROLL = 276;
    public const int WM_VSCROLL = 277;
    public const int WM_INITMENU = 278;
    public const int WM_INITMENUPOPUP = 279;
    public const int WM_MENUSELECT = 287;
    public const int WM_MENUCHAR = 288;
    public const int WM_ENTERIDLE = 289;
    public const int WM_MENURBUTTONUP = 290;
    public const int WM_MENUDRAG = 291;
    public const int WM_MENUGETOBJECT = 292;
    public const int WM_UNINITMENUPOPUP = 293;
    public const int WM_MENUCOMMAND = 294;
    public const int WM_CHANGEUISTATE = 295;
    public const int WM_UPDATEUISTATE = 296;
    public const int WM_QUERYUISTATE = 297;
    public const int WM_CTLCOLOR = 25;
    public const int WM_CTLCOLORMSGBOX = 306;
    public const int WM_CTLCOLOREDIT = 307;
    public const int WM_CTLCOLORLISTBOX = 308;
    public const int WM_CTLCOLORBTN = 309;
    public const int WM_CTLCOLORDLG = 310;
    public const int WM_CTLCOLORSCROLLBAR = 311;
    public const int WM_CTLCOLORSTATIC = 312;
    public const int WM_MOUSEFIRST = 512;
    public const int WM_MOUSEMOVE = 512;
    public const int WM_LBUTTONDOWN = 513;
    public const int WM_LBUTTONUP = 514;
    public const int WM_LBUTTONDBLCLK = 515;
    public const int WM_RBUTTONDOWN = 516;
    public const int WM_RBUTTONUP = 517;
    public const int WM_RBUTTONDBLCLK = 518;
    public const int WM_MBUTTONDOWN = 519;
    public const int WM_MBUTTONUP = 520;
    public const int WM_MBUTTONDBLCLK = 521;
    public const int WM_MOUSEWHEEL = 522;
    public const int WM_XBUTTONDOWN = 523;
    public const int WM_XBUTTONUP = 524;
    public const int WM_XBUTTONDBLCLK = 525;
    public const int WM_MOUSELAST = 525;
    public const int WM_PARENTNOTIFY = 528;
    public const int WM_ENTERMENULOOP = 529;
    public const int WM_EXITMENULOOP = 530;
    public const int WM_NEXTMENU = 531;
    public const int WM_SIZING = 532;
    public const int WM_CAPTURECHANGED = 533;
    public const int WM_MOVING = 534;
    public const int WM_POWERBROADCAST = 536;
    public const int WM_DEVICECHANGE = 537;
    public const int WM_MDICREATE = 544;
    public const int WM_MDIDESTROY = 545;
    public const int WM_MDIACTIVATE = 546;
    public const int WM_MDIRESTORE = 547;
    public const int WM_MDINEXT = 548;
    public const int WM_MDIMAXIMIZE = 549;
    public const int WM_MDITILE = 550;
    public const int WM_MDICASCADE = 551;
    public const int WM_MDIICONArANGE = 552;
    public const int WM_MDIGETACTIVE = 553;
    public const int WM_MDISETMENU = 560;
    public const int WM_ENTERSIZEMOVE = 561;
    public const int WM_EXITSIZEMOVE = 562;
    public const int WM_DROPFILES = 563;
    public const int WM_MDIREFRESHMENU = 564;
    public const int WM_IME_SETCONTEXT = 641;
    public const int WM_IME_NOTIFY = 642;
    public const int WM_IME_CONTROL = 643;
    public const int WM_IME_COMPOSITIONFULL = 644;
    public const int WM_IME_SELECT = 645;
    public const int WM_IME_CHAR = 646;
    public const int WM_IME_REQUEST = 648;
    public const int WM_IME_KEYDOWN = 656;
    public const int WM_IME_KEYUP = 657;
    public const int WM_MOUSEHOVER = 673;
    public const int WM_MOUSELEAVE = 675;
    public const int WM_NCMOUSELEAVE = 674;
    public const int WM_WTSSESSION_CHANGE = 689;
    public const int WM_TABLET_FIRST = 704;
    public const int WM_TABLET_LAST = 735;
    public const int WM_CUT = 768;
    public const int WM_COPY = 769;
    public const int WM_PASTE = 770;
    public const int WM_CLEAR = 771;
    public const int WM_UNDO = 772;
    public const int WM_RENDERFORMAT = 773;
    public const int WM_RENDERALLFORMATS = 774;
    public const int WM_DESTROYCLIPBOARD = 775;
    public const int WM_DRAWCLIPBOARD = 776;
    public const int WM_PAINTCLIPBOARD = 777;
    public const int WM_VSCROLLCLIPBOARD = 778;
    public const int WM_SIZECLIPBOARD = 779;
    public const int WM_ASKCBFORMATNAME = 780;
    public const int WM_CHANGECBCHAIN = 781;
    public const int WM_HSCROLLCLIPBOARD = 782;
    public const int WM_QUERYNEWPALETTE = 783;
    public const int WM_PALETTEISCHANGING = 784;
    public const int WM_PALETTECHANGED = 785;
    public const int WM_HOTKEY = 786;
    public const int WM_PRINT = 791;
    public const int WM_PRINTCLIENT = 792;
    public const int WM_APPCOMMAND = 793;
    public const int WM_THEMECHANGED = 794;
    public const int WM_HANDHELDFIRST = 856;
    public const int WM_HANDHELDLAST = 863;
    public const int WM_AFXFIRST = 864;
    public const int WM_AFXLAST = 895;
    public const int WM_PENWINFIRST = 896;
    public const int WM_PENWINLAST = 911;
    public const int WM_USER = 1024;
    public const int WM_REFLECT = 8192;
    public const int WM_APP = 32768;
    public const int WS_OVERLAPPED = 0;
    public const int WS_POPUP = -2147483648;
    public const int WS_CHILD = 1073741824;
    public const int WS_MINIMIZE = 536870912;
    public const int WS_VISIBLE = 268435456;
    public const int WS_DISABLED = 134217728;
    public const int WS_CLIPSIBLINGS = 67108864;
    public const int WS_CLIPCHILDREN = 33554432;
    public const int WS_MAXIMIZE = 16777216;
    public const int WS_CAPTION = 12582912;
    public const int WS_BORDER = 8388608;
    public const int WS_DLGFRAME = 4194304;
    public const int WS_VSCROLL = 2097152;
    public const int WS_HSCROLL = 1048576;
    public const int WS_SYSMENU = 524288;
    public const int WS_THICKFRAME = 262144;
    public const int WS_GROUP = 131072;
    public const int WS_TABSTOP = 65536;
    public const int WS_MINIMIZEBOX = 131072;
    public const int WS_MAXIMIZEBOX = 65536;
    public const int WS_TILED = 0;
    public const int WS_ICONIC = 536870912;
    public const int WS_SIZEBOX = 262144;
    public const int WS_TILEDWINDOW = 13565952;
    public const int WS_OVERLAPPEDWINDOW = 13565952;
    public const int WS_POPUPWINDOW = -2138570752;
    public const int WS_CHILDWINDOW = 1073741824;
    public const int WS_EX_DLGMODALFRAME = 1;
    public const int WS_EX_NOPARENTNOTIFY = 4;
    public const int WS_EX_TOPMOST = 8;
    public const int WS_EX_ACCEPTFILES = 16;
    public const int WS_EX_TRANSPARENT = 32;
    public const int WS_EX_MDICHILD = 64;
    public const int WS_EX_TOOLWINDOW = 128;
    public const int WS_EX_WINDOWEDGE = 256;
    public const int WS_EX_CLIENTEDGE = 512;
    public const int WS_EX_CONTEXTHELP = 1024;
    public const int WS_EX_RIGHT = 4096;
    public const int WS_EX_LEFT = 0;
    public const int WS_EX_RTLREADING = 8192;
    public const int WS_EX_LTRREADING = 0;
    public const int WS_EX_LEFTSCROLLBAR = 16384;
    public const int WS_EX_RIGHTSCROLLBAR = 0;
    public const int WS_EX_CONTROLPARENT = 65536;
    public const int WS_EX_STATICEDGE = 131072;
    public const int WS_EX_APPWINDOW = 262144;
    public const int WS_EX_OVERLAPPEDWINDOW = 768;
    public const int WS_EX_PALETTEWINDOW = 392;
    public const int WS_EX_LAYERED = 524288;
    public const int WS_EX_NOINHERITLAYOUT = 1048576;
    public const int WS_EX_LAYOUTRTL = 4194304;
    public const int WS_EX_COMPOSITED = 33554432;
    public const int WS_EX_NOACTIVATE = 134217728;
    public const int UIS_SET = 1;
    public const int UIS_CLEAR = 2;
    public const int UIS_INITIALIZE = 3;
    public const int UISF_HIDEFOCUS = 1;
    public const int UISF_HIDEACCEL = 2;
    public const int UISF_ACTIVE = 4;
    internal const uint CLSCTX_INPROC_SERVER = 1U;
    private const int VBM__BASE = 4096;
    public const int VSINPUT_PROCESSING_MSG = 4242;

    internal static ModifierKeys ModifierKeys
    {
      get
      {
        byte[] lpKeyState = new byte[256];
        NativeMethods.GetKeyboardState(lpKeyState);
        ModifierKeys modifierKeys = ModifierKeys.None;
        if (((int) lpKeyState[16] & 128) == 128)
          modifierKeys |= ModifierKeys.Shift;
        if (((int) lpKeyState[17] & 128) == 128)
          modifierKeys |= ModifierKeys.Control;
        if (((int) lpKeyState[18] & 128) == 128)
          modifierKeys |= ModifierKeys.Alt;
        if (((int) lpKeyState[91] & 128) == 128 || ((int) lpKeyState[92] & 128) == 128)
          modifierKeys |= ModifierKeys.Windows;
        return modifierKeys;
      }
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(ref POINT point);

    internal static Point GetCursorPos()
    {
      POINT point1 = new POINT()
      {
        x = 0,
        y = 0
      };
      Point point2 = new Point();
      if (NativeMethods.GetCursorPos(ref point1))
      {
        point2.X = (double) point1.x;
        point2.Y = (double) point1.y;
      }
      return point2;
    }

    [DllImport("user32.dll")]
    internal static extern int GetSystemMetrics(int index);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int RegisterWindowMessage(string lpString);

    [DllImport("user32.dll")]
    internal static extern short GetAsyncKeyState(int vKey);

    internal static bool IsLeftButtonPressed()
    {
      return NativeMethods.GetSystemMetrics(23) != 0 ? ((int) NativeMethods.GetAsyncKeyState(2) & 32768) != 0 : ((int) NativeMethods.GetAsyncKeyState(1) & 32768) != 0;
    }

    internal static bool IsRightButtonPressed()
    {
      return NativeMethods.GetSystemMetrics(23) != 0 ? ((int) NativeMethods.GetAsyncKeyState(1) & 32768) != 0 : ((int) NativeMethods.GetAsyncKeyState(2) & 32768) != 0;
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ScreenToClient(IntPtr hWnd, ref POINT point);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetKeyboardState(byte[] lpKeyState);

    [DllImport("user32.dll")]
    internal static extern uint MapVirtualKey(uint uCode, uint uMapType);

    [DllImport("user32.dll")]
    internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool BringWindowToTop(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumThreadWindows(uint dwThreadId, NativeMethods.EnumWindowsProc lpfn, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumChildWindows(IntPtr hwndParent, NativeMethods.EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowText(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string lpString);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PostMessage(IntPtr hWnd, int nMsg, IntPtr wParam, IntPtr lParam);

    [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

    [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

    [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

    [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, [MarshalAs(UnmanagedType.Bool)] bool redraw);

    internal static bool PostMessage(IntPtr hwnd, int msg)
    {
      return NativeMethods.PostMessage(hwnd, msg, IntPtr.Zero, IntPtr.Zero);
    }

    internal static bool PostMessage(IntPtr hwnd, int msg, IntPtr wParam)
    {
      return NativeMethods.PostMessage(hwnd, msg, wParam, IntPtr.Zero);
    }

    [DllImport("user32.dll")]
    internal static extern IntPtr SetCapture(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ReleaseCapture();

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int nMsg, IntPtr wParam, IntPtr lParam);

    internal static IntPtr SendMessage(IntPtr hwnd, int msg)
    {
      return NativeMethods.SendMessage(hwnd, msg, IntPtr.Zero, IntPtr.Zero);
    }

    internal static IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam)
    {
      return NativeMethods.SendMessage(hwnd, msg, wParam, IntPtr.Zero);
    }

    [DllImport("user32.dll")]
    public static extern IntPtr SetFocus(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr SetActiveWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr GetWindow(IntPtr hwnd, int nCmd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

    [DllImport("user32.dll")]
    public static extern int GetMessageTime();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowEnabled(IntPtr hwnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowVisible(IntPtr hwnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsWindow(IntPtr hWnd);

    [DllImport("ole32.dll")]
    public static extern int OleSetContainedObject(IntPtr punkObj, [MarshalAs(UnmanagedType.Bool)] bool fVisible);

    [DllImport("ole32.dll")]
    public static extern int OleRun(IntPtr punkObj);

    [DllImport("user32.dll")]
    public static extern int CopyAcceleratorTable(IntPtr hAccelSrc, out IntPtr lpAccelDst, int cAccelEntries);

    [DllImport("user32.dll")]
    public static extern IntPtr GetFocus();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);

    [DllImport("User32", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

    [DllImport("Comctl32", CharSet = CharSet.Auto)]
    public static extern int ImageList_Draw(HandleRef hImageList, int i, HandleRef hdc, int x, int y, int fStyle);

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    public static IntPtr GetWindowLongPtr(IntPtr hWnd, NativeMethods.GWLP nIndex)
    {
      if (IntPtr.Size == 8)
        return NativeMethods.GetWindowLongPtr(hWnd, (int) nIndex);
      return new IntPtr(NativeMethods.GetWindowLong(hWnd, (int) nIndex));
    }

    public static int GetWindowLong(IntPtr hWnd, NativeMethods.GWL nIndex)
    {
      return NativeMethods.GetWindowLong(hWnd, (int) nIndex);
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetClientRect(IntPtr hwnd, out RECT lpRect);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    public static IntPtr SetWindowLongPtr(IntPtr hWnd, NativeMethods.GWLP nIndex, IntPtr dwNewLong)
    {
      if (IntPtr.Size == 8)
        return NativeMethods.SetWindowLongPtr(hWnd, (int) nIndex, dwNewLong);
      return new IntPtr(NativeMethods.SetWindowLong(hWnd, (int) nIndex, dwNewLong.ToInt32()));
    }

    public static IntPtr SetWndProc(IntPtr hWnd, NativeMethods.WndProc newWndProc)
    {
      return NativeMethods.SetWindowLongPtr(hWnd, NativeMethods.GWLP.WNDPROC, Marshal.GetFunctionPointerForDelegate((Delegate) newWndProc));
    }

    public static int SetWindowLong(IntPtr hWnd, NativeMethods.GWL nIndex, int dwNewLong)
    {
      return NativeMethods.SetWindowLong(hWnd, (int) nIndex, dwNewLong);
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, int dwFlags);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern ushort RegisterClassEx(ref WNDCLASSEX lpWndClass);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern ushort RegisterClass(ref WNDCLASS lpWndClass);

    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr OpenEvent(uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, string lpName);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetEvent(IntPtr hEvent);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(IntPtr handle);

    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentThreadId();

    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentProcessId();

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(NativeMethods.WindowsHookType hookType, NativeMethods.WindowsHookProc hookProc, IntPtr module, uint threadId);

    [DllImport("user32.dll")]
    public static extern IntPtr CallNextHookEx(IntPtr hhk, NativeMethods.CbtHookAction code, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("gdi32.dll")]
    public static extern IntPtr GetStockObject(int fnObject);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr CreateWindowEx(int dwExStyle, IntPtr classAtom, string lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr CreateWindowEx(int dwExStyle, string className, string lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr GetModuleHandle(string moduleName);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool TranslateMessage([In] ref MSG lpMsg);

    [DllImport("user32.dll")]
    public static extern IntPtr DispatchMessage([In] ref MSG lpmsg);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateSolidBrush(int colorref);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteObject(IntPtr hObject);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShowWindow(IntPtr hwnd, int code);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int width, int height, [MarshalAs(UnmanagedType.Bool)] bool repaint);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyWindow(IntPtr hwnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnregisterClass(IntPtr classAtom, IntPtr hInstance);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FillRect(IntPtr hDC, ref RECT rect, IntPtr hbrush);

    [DllImport("user32.dll")]
    public static extern IntPtr GetParent(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr SetParent(IntPtr hChild, IntPtr hNewParent);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, SHGFI uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyIcon(IntPtr hIcon);

    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PathIsNetworkPath(string Path);

    [DllImport("comctl32.dll")]
    internal static extern int ImageList_GetImageCount(IntPtr himl);

    [DllImport("comctl32.dll")]
    internal static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, uint flags);

    [DllImport("comctl32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ImageList_GetImageInfo(IntPtr himl, int i, out IMAGEINFO pImageInfo);

    [DllImport("comctl32.dll")]
    internal static extern int ImageList_GetBkColor(IntPtr himl);

    [DllImport("gdi32.dll")]
    internal static extern int GetObject(IntPtr hGdiObj, int cbBuffer, IntPtr lpvObject);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, NativeMethods.EnumMonitorsDelegate lpfnEnum, IntPtr dwData);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IntersectRect(out RECT lprcDst, [In] ref RECT lprcSrc1, [In] ref RECT lprcSrc2);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO monitorInfo);

    [DllImport("oleaut32.dll", CharSet = CharSet.Ansi)]
    internal static extern int OleCreatePictureIndirect(ref NativeMethods.PictDescBitmap pictdesc, ref Guid iid, [MarshalAs(UnmanagedType.Bool)] bool fOwn, [MarshalAs(UnmanagedType.Interface)] out object ppVoid);

    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
    internal static extern void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);

    [DllImport("gdi32.dll", SetLastError = true)]
    internal static extern IntPtr CreateDIBSection(IntPtr hdc, ref NativeMethods.BITMAPINFO pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

    private static WINDOWPOS LParamToWindowPos(IntPtr lParam)
    {
      WINDOWPOS windowpos = new WINDOWPOS();
      Marshal.PtrToStructure(lParam, (object) windowpos);
      return windowpos;
    }

    public static uint GetWindowPosFlags(IntPtr lParam)
    {
      return NativeMethods.LParamToWindowPos(lParam).flags;
    }

    public static Size GetWindowPosSize(IntPtr lParam)
    {
      WINDOWPOS windowpos = NativeMethods.LParamToWindowPos(lParam);
      return new Size((double) windowpos.cx, (double) windowpos.cy);
    }

    [DllImport("user32.dll")]
    public static extern IntPtr MonitorFromRect([In] ref RECT lprc, uint dwFlags);

    internal static void FindMaximumSingleMonitorRectangle(RECT windowRect, out RECT screenSubRect, out RECT monitorRect)
    {
      List<RECT> rects = new List<RECT>();
      NativeMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (NativeMethods.EnumMonitorsDelegate) ((IntPtr hMonitor, IntPtr hdcMonitor, ref RECT rect, IntPtr lpData) =>
      {
        MONITORINFO monitorInfo = new MONITORINFO();
        monitorInfo.cbSize = (uint) Marshal.SizeOf(typeof (MONITORINFO));
        NativeMethods.GetMonitorInfo(hMonitor, ref monitorInfo);
        rects.Add(monitorInfo.rcWork);
        return true;
      }), IntPtr.Zero);
      long num1 = 0L;
      screenSubRect = new RECT()
      {
        Left = 0,
        Right = 0,
        Top = 0,
        Bottom = 0
      };
      monitorRect = new RECT()
      {
        Left = 0,
        Right = 0,
        Top = 0,
        Bottom = 0
      };
      foreach (RECT rect in rects)
      {
        RECT lprcSrc1 = rect;
        RECT lprcDst;
        NativeMethods.IntersectRect(out lprcDst, ref lprcSrc1, ref windowRect);
        long num2 = (long) (lprcDst.Width * lprcDst.Height);
        if (num2 > num1)
        {
          screenSubRect = lprcDst;
          monitorRect = rect;
          num1 = num2;
        }
      }
    }

    internal static void FindMaximumSingleMonitorRectangle(Rect windowRect, out Rect screenSubRect, out Rect monitorRect)
    {
      RECT screenSubRect1;
      RECT monitorRect1;
      NativeMethods.FindMaximumSingleMonitorRectangle(new RECT(windowRect), out screenSubRect1, out monitorRect1);
      screenSubRect = new Rect(screenSubRect1.Position, screenSubRect1.Size);
      monitorRect = new Rect(monitorRect1.Position, monitorRect1.Size);
    }

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    public delegate IntPtr WindowsHookProc(NativeMethods.CbtHookAction code, IntPtr wParam, IntPtr lParam);

    [Flags]
    public enum SyncObjectAccess
    {
      DELETE = 65536,
      READ_CONTROL = 131072,
      WRITE_DAC = 262144,
      WRITE_OWNER = 524288,
      SYNCHRONIZE = 1048576,
      EVENT_ALL_ACCESS = 2031619,
      EVENT_MODIFY_STATE = 2,
      MUTEX_ALL_ACCESS = 2031617,
      MUTEX_MODIFY_STATE = 1,
      SEMAPHORE_ALL_ACCESS = MUTEX_MODIFY_STATE | EVENT_MODIFY_STATE | SYNCHRONIZE | WRITE_OWNER | WRITE_DAC | READ_CONTROL | DELETE,
      SEMAPHORE_MODIFY_STATE = EVENT_MODIFY_STATE,
      TIMER_ALL_ACCESS = SEMAPHORE_MODIFY_STATE | MUTEX_MODIFY_STATE | SYNCHRONIZE | WRITE_OWNER | WRITE_DAC | READ_CONTROL | DELETE,
      TIMER_MODIFY_STATE = SEMAPHORE_MODIFY_STATE,
      TIMER_QUERY_STATE = MUTEX_MODIFY_STATE,
    }

    public enum WindowsHookType
    {
      WH_JOURNALRECORD,
      WH_JOURNALPLAYBACK,
      WH_KEYBOARD,
      WH_GETMESSAGE,
      WH_CALLWNDPROC,
      WH_CBT,
      WH_SYSMSGFILTER,
      WH_MOUSE,
      WH_HARDWARE,
      WH_DEBUG,
      WH_SHELL,
      WH_FOREGROUNDIDLE,
      WH_CALLWNDPROCRET,
      WH_KEYBOARD_LL,
      WH_MOUSE_LL,
    }

    public enum CbtHookAction
    {
      HCBT_MOVESIZE,
      HCBT_MINMAX,
      HCBT_QS,
      HCBT_CREATEWND,
      HCBT_DESTROYWND,
      HCBT_ACTIVATE,
      HCBT_CLICKSKIPPED,
      HCBT_KEYSKIPPED,
      HCBT_SYSCOMMAND,
      HCBT_SETFOCUS,
    }

    public enum StockObjects
    {
      WHITE_BRUSH = 0,
      LTGRAY_BRUSH = 1,
      GRAY_BRUSH = 2,
      DKGRAY_BRUSH = 3,
      BLACK_BRUSH = 4,
      HOLLOW_BRUSH = 5,
      NULL_BRUSH = 5,
      WHITE_PEN = 6,
      BLACK_PEN = 7,
      NULL_PEN = 8,
      OEM_FIXED_FONT = 10,
      ANSI_FIXED_FONT = 11,
      ANSI_VAR_FONT = 12,
      SYSTEM_FONT = 13,
      DEVICE_DEFAULT_FONT = 14,
      DEFAULT_PALETTE = 15,
      SYSTEM_FIXED_FONT = 16,
      DEFAULT_GUI_FONT = 17,
      DC_BRUSH = 18,
      DC_PEN = 19,
    }

    public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    internal delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

    internal struct PictDescBitmap
    {
      internal int cbSizeOfStruct;
      internal int pictureType;
      internal IntPtr hBitmap;
      internal IntPtr hPalette;
      internal int unused;

      internal static NativeMethods.PictDescBitmap Default
      {
        get
        {
          return new NativeMethods.PictDescBitmap()
          {
            cbSizeOfStruct = 20,
            pictureType = 1,
            hBitmap = IntPtr.Zero,
            hPalette = IntPtr.Zero
          };
        }
      }
    }

    internal struct BITMAPINFO
    {
      internal int biSize;
      internal int biWidth;
      internal int biHeight;
      internal short biPlanes;
      internal short biBitCount;
      internal int biCompression;
      internal int biSizeImage;
      internal int biXPelsPerMeter;
      internal int biYPelsPerMeter;
      internal int biClrUsed;
      internal int biClrImportant;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
      internal byte[] bmiColors;

      internal static NativeMethods.BITMAPINFO Default
      {
        get
        {
          return new NativeMethods.BITMAPINFO()
          {
            biSize = 40,
            biPlanes = (short) 1
          };
        }
      }
    }

    public enum DialogResult
    {
      IDOK = 1,
      IDCANCEL = 2,
      IDABORT = 3,
      IDRETRY = 4,
      IDIGNORE = 5,
      IDYES = 6,
      IDNO = 7,
      IDCLOSE = 8,
      IDHELP = 9,
      IDTRYAGAIN = 10,
      IDCONTINUE = 11,
    }

    public enum GWL
    {
      EXSTYLE = -20,
      STYLE = -16,
    }

    public enum GWLP
    {
      USERDATA = -21,
      ID = -12,
      HWNDPARENT = -8,
      HINSTANCE = -6,
      WNDPROC = -4,
    }
  }
}
