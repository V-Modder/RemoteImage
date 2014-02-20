using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowsAPI;

namespace Client
{
    public partial class WindowSnipping : Form
    {
        private static Screen _Screen;
        private static Size BitmapSize;
        private static Graphics Graph;
        private static IDictionary<IntPtr, WindowInfo> windows;


        private Point P;
        private Image m_Image;
        private Rectangle rcSelect = new Rectangle();

        public WindowSnipping(Image screenshot, Point p)
        {
            InitializeComponent();
            P = p;
            this.BackgroundImage = screenshot;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.DoubleBuffered = true;
        }

        public static Image Snip(Screen screen)
        {
            _Screen = screen;
            Bitmap bmp = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics gr = Graphics.FromImage(bmp);
            Graph = gr;

            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            BitmapSize = bmp.Size;

            windows = OpenWindowGetter.GetOpenWindows();

            using (WindowSnipping snipper = new WindowSnipping(bmp, new Point(screen.Bounds.Left, screen.Bounds.Top)))
            {
                if (snipper.ShowDialog() == DialogResult.OK)
                {
                    return snipper.Image;
                }
            }
            return null;
        }

        public Image Image
        {
            get { return m_Image; }
            set { m_Image = value; }
        }

        private Rectangle GetRectFromPoint(Point p)
        {
            foreach (KeyValuePair<IntPtr, WindowInfo> lWindow in windows)
            {
                if (p.X >= lWindow.Value.Recangle.Left && p.X <= lWindow.Value.Recangle.Right &&
                    p.Y >= lWindow.Value.Recangle.Top && p.Y <= lWindow.Value.Recangle.Bottom)
                    return new Rectangle(lWindow.Value.Recangle.Left, lWindow.Value.Recangle.Top,
                        lWindow.Value.Recangle.Right-lWindow.Value.Recangle.Left, lWindow.Value.Recangle.Bottom - lWindow.Value.Recangle.Top);
            }
            return Rectangle.Empty;
        }

        private void WindowSnipping_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle r = GetRectFromPoint(Cursor.Position);
            if(r != Rectangle.Empty)
                rcSelect = r;
            this.Invalidate();
        }

        private void WindowSnipping_MouseUp(object sender, MouseEventArgs e)
        {
            if (rcSelect.Width <= 0 || rcSelect.Height <= 0)
            {
                return;
            }
            else
            {
                Image = new Bitmap(rcSelect.Width, rcSelect.Height);
                using (Graphics gr = Graphics.FromImage(Image))
                {
                    gr.DrawImage(this.BackgroundImage, new Rectangle(0, 0, Image.Width, Image.Height), rcSelect, GraphicsUnit.Pixel);
                }
                DialogResult = DialogResult.OK;
            }
        }

        private void WindowSnipping_Paint(object sender, PaintEventArgs e)
        {
            using (Brush br = new SolidBrush(Color.FromArgb(120, Color.White)))
            {
                int x1 = rcSelect.X;
                int x2 = rcSelect.X + rcSelect.Width;
                int y1 = rcSelect.Y;
                int y2 = rcSelect.Y + rcSelect.Height;
                e.Graphics.FillRectangle(br, new Rectangle(0, 0, x1, this.Height));
                e.Graphics.FillRectangle(br, new Rectangle(x2, 0, this.Width - x2, this.Height));
                e.Graphics.FillRectangle(br, new Rectangle(x1, 0, x2 - x1, y1));
                e.Graphics.FillRectangle(br, new Rectangle(x1, y2, x2 - x1, this.Height - y2));
            }
            using (Pen pen = new Pen(Color.Red, 3))
            {
                e.Graphics.DrawRectangle(pen, rcSelect);
            }
        }

        private void WindowSnipping_Load(object sender, EventArgs e)
        {
            this.Size = new Size(_Screen.Bounds.Width, _Screen.Bounds.Height);
            Rectangle area = _Screen.WorkingArea;
            this.Location = P;
            this.ShowInTaskbar = false;
            Graph.CopyFromScreen(area.X, area.Y, area.Y, area.Y, BitmapSize);
        }

        private void WindowSnipping_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
