using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

            POINT cursorPos = new POINT();
            GetCursorPos(out cursorPos);

            this.Text = cursorPos.X.ToString() + " | " + cursorPos.Y.ToString();

            EnumWindows(delegate (IntPtr wnd, IntPtr param)
            {
                if (IsWindowVisible(wnd))
                {
                    if (GetWindowText(wnd) != "")
                    {
                        Rect wRect = new Rect();
                        GetWindowRect(wnd, ref wRect);


                        if ((cursorPos.X > wRect.Left && cursorPos.X < (wRect.Right - wRect.Left)) && (cursorPos.Y > wRect.Top && cursorPos.Y < (wRect.Bottom - wRect.Top)))
                        {
                            richTextBox1.AppendText(GetWindowText(wnd) + " | " + wRect.Left.ToString() + ":" + wRect.Top.ToString() + Environment.NewLine);
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
