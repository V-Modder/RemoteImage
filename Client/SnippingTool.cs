using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class SnippingTool : Form
    {
        private static Screen _Screen;
        private static Size BitmapSize;
        private static Graphics Graph;
        
        private Point P;
        private Image m_Image;
        private Rectangle rcSelect = new Rectangle();
        private Point pntStart;

        public SnippingTool(Image screenshot, Point p)
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

            using(SnippingTool snipper = new SnippingTool(bmp, new Point(screen.Bounds.Left, screen.Bounds.Top)))
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

        private void SnippingTool_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;
            pntStart = e.Location;
            rcSelect = new Rectangle(e.Location, new Size(0, 0));
            this.Invalidate();
        }

        private void SnippingTool_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;
            int x1 = Math.Min(e.X, pntStart.X);
            int y1 = Math.Min(e.Y, pntStart.Y);
            int x2 = Math.Max(e.X, pntStart.X);
            int y2 = Math.Max(e.Y, pntStart.Y);
            rcSelect = new Rectangle(x1, y1, x2 - x1, y2 - y1);
            this.Invalidate();
        }

        private void SnippingTool_MouseUp(object sender, MouseEventArgs e)
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

        private void SnippingTool_Paint(object sender, PaintEventArgs e)
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

        private void SnippingTool_Load(object sender, EventArgs e)
        {
            this.Size = new Size(_Screen.Bounds.Width, _Screen.Bounds.Height);
            Rectangle area = _Screen.WorkingArea;
            this.Location = P;
            this.ShowInTaskbar = false;
            Graph.CopyFromScreen(area.X, area.Y, area.Y, area.Y, BitmapSize);
        }

        private void SnippingTool_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
