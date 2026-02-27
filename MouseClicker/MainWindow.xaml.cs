using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static MouseClicker.API;
using SystemInformation = System.Windows.Forms.SystemInformation;
using WinFormRect = System.Drawing.Rectangle;

namespace MouseClicker
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.TITLE.MouseLeftButtonDown += (s, e) =>
            {
                DependencyObject originalSource = e.OriginalSource as DependencyObject;
                if (originalSource is Border border)
                {
                    if (border == CLOSE || border == MINI)
                    {
                        return;
                    }
                }
                this.DragMove();
            };
            this.CLOSE.MouseLeftButtonUp += (s, e) =>
            {
                this.Close();
            };
            this.MINI.MouseLeftButtonUp += (s, e) =>
            {
                this.WindowState = WindowState.Minimized;
            };
            this.Type1.Checked += Type_Checked;
            this.Type2.Checked += Type_Checked;
            this.Type3.Checked += Type_Checked;
            this.Type1.Unchecked += Type_Unchecked;
            this.Type2.Unchecked += Type_Unchecked;
            this.Type3.Unchecked += Type_Unchecked;
            this.Delay.Checked += Flag_Checked;
            this.Move.Checked += Flag_Checked;
            this.MovePlus.Checked += Flag_Checked;
            this.Delay.Unchecked += Flag_Unchecked;
            this.Move.Unchecked += Flag_Unchecked;
            this.MovePlus.Unchecked += Flag_Unchecked;
            this.Time.PreviewTextInput += Time_PreviewTextInput;
            this.Time.TextChanged += Time_TextChanged;
            this.Get.MouseLeftButtonUp += Get_MouseLeftButtonUp;
            this.Button.MouseLeftButtonUp += Button_MouseLeftButtonUp;

            this.ContentRendered += (s, e) =>
            {
                T = new Thread(Run) { IsBackground = true };
                T.Start();
                this.HOOKPROC = Win32CallBack;
                KeyHook = SetWindowsHookEx(WH_KEYBOARD_LL, this.HOOKPROC, IntPtr.Zero, 0);
                if (KeyHook == IntPtr.Zero)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error(),
                            $"挂接全局HOOK挂钩过程中出现Win32异常({Marshal.GetLastWin32Error().ToString().PadLeft(4, '0')})。");
                    });
                }
            };
            this.Closing += (s, e) =>
            {
                if (!UnhookWindowsHookEx((int)KeyHook))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error(),
                            $"挂接全局HOOK挂钩过程中出现Win32异常({Marshal.GetLastWin32Error().ToString().PadLeft(4, '0')})。");
                    });
                }
                if (T != null && T.IsAlive)
                {
                    T.Abort();
                }
            };
        }

        private IntPtr KeyHook;

        private HOOKPROC HOOKPROC;

        private int Win32CallBack(int nCode, int wParam, IntPtr lParam)
        {
            KeyBoardHookStruct keyBoardHookStruct = new KeyBoardHookStruct();
            try
            {
                keyBoardHookStruct = (KeyBoardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyBoardHookStruct));
            }
            catch (Exception) { }
            finally
            {
                if (keyBoardHookStruct == null)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(),
                        $"LocalLowHook捕获发生异常。");
                }
            }
            if (wParam == WM_SYSKEYDOWN)
            {
                if (keyBoardHookStruct.vkCode == VK_F1 && (keyBoardHookStruct.flags & LLKHF_ALTDOWN) != 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        Button_MouseLeftButtonUp(null, null);
                    });
                }
            }
            return CallNextHookEx((int)KeyHook, nCode, wParam, lParam);
        }

        private enum TypeEnum : int
        {
            Left = 0,
            Middle = 1,
            Right = 2
        }

        private TypeEnum TypeInfo = TypeEnum.Left;

        private void Type_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                if (rb == Type1)
                {
                    TypeInfo = TypeEnum.Left;
                }
                if (rb == Type2)
                {
                    TypeInfo = TypeEnum.Middle;
                }
                if (rb == Type3)
                {
                    TypeInfo = TypeEnum.Right;
                }
                rb.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0));
                rb.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0));
            }
        }

        private void Type_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                rb.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x20, 0x20, 0x20));
                rb.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x80, 0x80, 0x80));
            }
        }

        private void Flag_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb)
            {
                if (cb == Delay)
                {
                    this.DelaySTD.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0));
                }
                if (cb == Move)
                {
                    this.MoveSTD.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0));
                }
                if (cb == MovePlus)
                {
                    this.MovePlusSTD.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0));
                }
                cb.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0));
            }
        }

        private void Flag_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb)
            {
                if (cb == Delay)
                {
                    this.DelaySTD.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x80, 0x80, 0x80));
                }
                if (cb == Move)
                {
                    this.MoveSTD.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x80, 0x80, 0x80));
                    if (this.MovePlus.IsChecked.HasValue && this.MovePlus.IsChecked.Value)
                    {
                        this.MovePlus.IsChecked = false;
                    }
                }
                if (cb == MovePlus)
                {
                    this.MovePlusSTD.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x80, 0x80, 0x80));
                }
                cb.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x20, 0x20, 0x20));
            }
        }

        private int _TimeInfo = 0;

        private int TimeInfo
        {
            get
            {
                return _TimeInfo > 0 ? _TimeInfo : 1;
            }
            set
            {
                _TimeInfo = value;
            }
        }

        private int TimeDynamic
        {
            get
            {
                int td = TimeInfo / 10;
                if (td < 1) td = 1;
                if (td > 100) td = 100;
                return td;
            }
        }

        private void Time_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (int.TryParse(e.Text, out int Textint))
            {
                TimeInfo = TimeInfo * 10 + Textint;
                if (TimeInfo > 0)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Time_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(Time.Text))
            {
                TimeInfo = 0;
            }
            else
            {
                if (Time.Text == TimeInfo.ToString())
                {
                    return;
                }
                else
                {
                    if (int.TryParse(Time.Text, out int Textint) && Textint > 0)
                    {
                        Time.Text = Textint.ToString();
                        TimeInfo = Textint;
                    }
                    else
                    {
                        if (TimeInfo > 0)
                        {
                            Time.Text = TimeInfo.ToString();
                        }
                        else
                        {
                            Time.Text = string.Empty;
                        }
                    }
                }
            }
        }

        private Point MousePoint = new Point(0, 0);

        private bool getstate = false;

        private void Get_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (getstate) return;
            getstate = true;
            this.Get.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0));
            this.GetArrow.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x30, 0x30, 0x30));
            void Flag_A(object _s, EventArgs _e)
            {
                Flag();
            }
            void Flag_B(object _s, MouseButtonEventArgs _e)
            {
                Flag();
            }
            void Flag()
            {
                bool result = GetCursorPos(out POINT P);
                if (result)
                {
                    MousePoint = new Point(P.X, P.Y);
                    this.PointSTD.Text = $"{P.X},{P.Y}";
                }
                else
                {
                    MousePoint = new Point(0, 0);
                    this.PointSTD.Text = "0,0";
                }
                this.Deactivated -= Flag_A;
                this.MouseDown -= Flag_B;
                this.Get.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x30, 0x30, 0x30));
                this.GetArrow.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0));
                getstate = false;
            }
            this.Deactivated += Flag_A;
            this.MouseDown += Flag_B;
        }

        private Thread T = null;

        private volatile bool Is_Running = false;

        private volatile bool Is_Waiting = false;

        private readonly AutoResetEvent RunSign = new AutoResetEvent(false);

        private void Run()
        {
            AutoResetEvent timer = new AutoResetEvent(false);
            Stopwatch time = new Stopwatch();
            do
            {
                if (!Is_Running)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ReadSet(true);
                        ElementSet();
                    });
                    Is_Waiting = true;
                    RunSign.WaitOne();
                    Is_Waiting = false;
                }
                time.Restart();
                bool state;
                if (_IsMove)
                {
                    Point offset = new Point(0, 0);
                    if (_IsMovePlus)
                    {
                        offset.X = 5 * Rdm.Next(0,101) / 100 * (Convert.ToBoolean(Rdm.Next(0, 2)) ? 1 : -1);
                        offset.Y = 5 * Rdm.Next(0, 101) / 100 * (Convert.ToBoolean(Rdm.Next(0, 2)) ? 1 : -1);
                    }
                    WinFormRect virtualscreen = SystemInformation.VirtualScreen;
                    int normalizedX = (int)(_MoveLocation.X - virtualscreen.Left + offset.X) * (65535 / virtualscreen.Width);
                    int normalizedY = (int)(_MoveLocation.Y - virtualscreen.Top + offset.Y) * (65535 / virtualscreen.Height);
                    INPUT[] inputs_move = new INPUT[1];
                    inputs_move[0] = new INPUT
                    {
                        type = INPUT_TYPE.MOUSE,
                        union = new INPUT.Union
                        {
                            mi = new MOUSEINPUT
                            {
                                dx = normalizedX,
                                dy = normalizedY,
                                dwFlags = MOUSEEVENTF.MOVE | MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.VIRTUALDESK
                            }
                        }
                    };
                    uint _ = SendInput(1, inputs_move, Marshal.SizeOf<INPUT>());
                    state = _ != 0;
                }
                else
                {
                    state = true;
                }
                if (state)
                {
                    MOUSEEVENTF D1;
                    MOUSEEVENTF D2;
                    switch (Set_Type)
                    {
                        case TypeEnum.Left:
                            goto default;
                        case TypeEnum.Middle:
                            D1 = MOUSEEVENTF.MIDDLEDOWN;
                            D2 = MOUSEEVENTF.MIDDLEUP;
                            break;
                        case TypeEnum.Right:
                            D1 = MOUSEEVENTF.RIGHTDOWN;
                            D2 = MOUSEEVENTF.RIGHTUP;
                            break;
                        default:
                            D1 = MOUSEEVENTF.LEFTDOWN;
                            D2 = MOUSEEVENTF.LEFTUP;
                            break;
                    }
                    INPUT[] inputs = new INPUT[2];
                    inputs[0] = new INPUT
                    {
                        type = INPUT_TYPE.MOUSE,
                        union = new INPUT.Union
                        {
                            mi = new MOUSEINPUT
                            {
                                dwFlags = D1
                            }
                        }
                    };
                    inputs[1] = new INPUT
                    {
                        type = INPUT_TYPE.MOUSE,
                        union = new INPUT.Union
                        {
                            mi = new MOUSEINPUT
                            {
                                dwFlags = D2
                            }
                        }
                    };
                    _ = SendInput(2, inputs, Marshal.SizeOf<INPUT>());
                }
                int delaytime = _LoopTime;
                if (_IsDelay)
                {
                    double pct = Rdm.Next(0, 101) / 100;
                    if (pct < 1) pct = 1;
                    delaytime += (int)(_DelayTime * pct * (Convert.ToBoolean(Rdm.Next(0, 2)) ? 1 : -1));
                }
                time.Stop();
                delaytime -= (int)time.ElapsedMilliseconds;
                timer.WaitOne(delaytime < 1 ? 1 : delaytime);
            } while (Thread.CurrentThread.IsAlive);
        }

        private Random Rdm = null;

        private TypeEnum Set_Type = TypeEnum.Left;

        private int _LoopTime = 1;

        private bool _IsDelay = false;

        private int _DelayTime = 1;

        private bool _IsMove = false;

        private bool _IsMovePlus = false;

        private Point _MoveLocation = new Point(0, 0);

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void ReadSet(bool reset)
        {
            if (string.IsNullOrEmpty(this.Time.Text)) this.Time.Text = "1";
            if (reset)
            {
                Rdm = null;
                Set_Type = TypeEnum.Left;
                _LoopTime = 1;
                _IsDelay = false;
                _DelayTime = 1;
                _IsMove = false;
                _IsMovePlus = false;
                _MoveLocation = new Point(0, 0);
                TimeEndPeriod(1);
            }
            else
            {
                Rdm = new Random(Environment.TickCount);
                Set_Type = TypeInfo;
                _LoopTime = TimeInfo;
                _IsDelay = this.Delay.IsChecked.HasValue && this.Delay.IsChecked.Value;
                _DelayTime = TimeDynamic;
                _IsMove = this.Move.IsChecked.HasValue && this.Move.IsChecked.Value;
                _IsMovePlus = this.MovePlus.IsChecked.HasValue && this.MovePlus.IsChecked.Value;
                _MoveLocation = this.MousePoint;
                TimeBeginPeriod(1);
            }
        }

        private void Button_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Is_Running)
            {
                Is_Running = false;
            }
            else
            {
                if (Is_Waiting)
                {
                    ReadSet(false);
                    Is_Running = true;
                    this.ButtonSTD.Text = "停止点击（Alt+F1）";
                    RunSign.Set();
                }
            }
        }

        private void ElementSet()
        {
            this.ButtonSTD.Text = "开始点击（Alt+F1）";
        }
    }

    public class API
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public INPUT_TYPE type;

            public Union union;

            [StructLayout(LayoutKind.Explicit)]
            public struct Union
            {
                [FieldOffset(0)] public MOUSEINPUT mi;
                [FieldOffset(0)] public KEYBDINPUT ki;
                [FieldOffset(0)] public HARDWAREINPUT hi;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public MOUSEEVENTF dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public KEYEVENTF dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        public enum INPUT_TYPE : uint
        {
            MOUSE = 0,
            KEYBOARD = 1,
            HARDWARE = 2
        }

        [Flags]
        public enum MOUSEEVENTF : uint
        {
            MOVE = 0x0001,
            LEFTDOWN = 0x0002,
            LEFTUP = 0x0004,
            RIGHTDOWN = 0x0008,
            RIGHTUP = 0x0010,
            MIDDLEDOWN = 0x0020,
            MIDDLEUP = 0x0040,
            VIRTUALDESK = 0x4000,
            ABSOLUTE = 0x8000
        }

        [Flags]
        public enum KEYEVENTF : uint
        {
            KEYUP = 0x0002,
            SCANCODE = 0x0008,
            UNICODE = 0x0004
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(
            uint nInputs,
            [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs,
            int cbSize
        );

        public delegate int HOOKPROC(int nCode, int wParam, IntPtr lParam);

        public const int WH_KEYBOARD_LL = 13;

        public const int WM_SYSKEYDOWN = 0x0104;

        public const int WM_SYSKEYUP = 0x0105;
        
        public const int LLKHF_ALTDOWN = 0x20;

        public const int VK_F1 = 0x70;

        [StructLayout(LayoutKind.Sequential)]
        public class KeyBoardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HOOKPROC lpfn, IntPtr hmod, int dwThreadId);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]
        public static extern uint TimeBeginPeriod(uint uMilliseconds);

        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]
        public static extern uint TimeEndPeriod(uint uMilliseconds);
    }
}
