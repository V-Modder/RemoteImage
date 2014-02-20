namespace Client
{
    partial class WindowSnipping
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // WindowSnipping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "WindowSnipping";
            this.Text = "WindowSnipping";
            this.Load += new System.EventHandler(this.WindowSnipping_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.WindowSnipping_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WindowSnipping_KeyDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.WindowSnipping_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.WindowSnipping_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}