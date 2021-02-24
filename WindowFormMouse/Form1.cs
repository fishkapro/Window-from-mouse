using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WindowFormMouse
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            richTextBox1.Clear();

            var cursorPos = new POINT();
            GetCursorPos(out cursorPos);

            this.Text = $"X:{cursorPos.X}, Y:{cursorPos.Y}";

            EnumWindows(delegate (IntPtr wnd, IntPtr param)
            {
                var winText = GetWindowText(wnd);

                if (IsWindowVisible(wnd) && string.IsNullOrEmpty(winText) == false)
                {
                    Rect wRect = new Rect();
                    GetWindowRect(wnd, ref wRect);

                    if ((cursorPos.X > wRect.Left
                         && cursorPos.X < wRect.Right)
                         && (cursorPos.Y > wRect.Top
                         && cursorPos.Y < wRect.Bottom))
                    {
                        if (winText.StartsWith("X")
                         || winText.StartsWith(nameof(WindowFormMouse)))
                        {
                            richTextBox1.AppendText("Курсор мыши на окне нашей программы."
                                + Environment.NewLine);
                        }
                        else
                        {
                            richTextBox1.AppendText($"Окно: {winText} в прямоугольнике: {wRect}"
                                + Environment.NewLine);
                        }
                    }
                }

                return true;
            }, IntPtr.Zero);
        }

        #region WINAPI
        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }

            public override string ToString()
            {
                return $"Верх: {Top}, низ: {Bottom}, лево: {Left}, право: {Right}";
            }
        }
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        public static string GetWindowText(IntPtr hWnd)
        {
            int size = GetWindowTextLength(hWnd);
            if (size > 0)
            {
                var builder = new StringBuilder(size + 1);
                GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }

            return String.Empty;
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        #endregion
    }
}
