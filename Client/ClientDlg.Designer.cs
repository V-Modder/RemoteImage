namespace Client
{
    partial class ClientDlg
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_picture = new System.Windows.Forms.Button();
            this.btn_snipping = new System.Windows.Forms.Button();
            this.txt_serverip = new System.Windows.Forms.TextBox();
            this.btn_connect = new System.Windows.Forms.Button();
            this.lbl_connected = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_picture
            // 
            this.btn_picture.Enabled = false;
            this.btn_picture.Location = new System.Drawing.Point(12, 14);
            this.btn_picture.Name = "btn_picture";
            this.btn_picture.Size = new System.Drawing.Size(75, 66);
            this.btn_picture.TabIndex = 2;
            this.btn_picture.Text = "Bild";
            this.btn_picture.UseVisualStyleBackColor = true;
            this.btn_picture.Click += new System.EventHandler(this.btn_picture_Click);
            // 
            // btn_snipping
            // 
            this.btn_snipping.Enabled = false;
            this.btn_snipping.Location = new System.Drawing.Point(106, 14);
            this.btn_snipping.Name = "btn_snipping";
            this.btn_snipping.Size = new System.Drawing.Size(75, 66);
            this.btn_snipping.TabIndex = 3;
            this.btn_snipping.Text = "Snipping";
            this.btn_snipping.UseVisualStyleBackColor = true;
            this.btn_snipping.Click += new System.EventHandler(this.btn_snipping_Click);
            // 
            // txt_serverip
            // 
            this.txt_serverip.Location = new System.Drawing.Point(207, 14);
            this.txt_serverip.Name = "txt_serverip";
            this.txt_serverip.Size = new System.Drawing.Size(134, 20);
            this.txt_serverip.TabIndex = 4;
            this.txt_serverip.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_serverip_KeyDown);
            // 
            // btn_connect
            // 
            this.btn_connect.Location = new System.Drawing.Point(347, 12);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(75, 23);
            this.btn_connect.TabIndex = 5;
            this.btn_connect.Text = "Verbinden";
            this.btn_connect.UseVisualStyleBackColor = true;
            this.btn_connect.Click += new System.EventHandler(this.btn_connect_Click);
            // 
            // lbl_connected
            // 
            this.lbl_connected.Location = new System.Drawing.Point(204, 50);
            this.lbl_connected.Name = "lbl_connected";
            this.lbl_connected.Size = new System.Drawing.Size(218, 59);
            this.lbl_connected.TabIndex = 6;
            this.lbl_connected.Text = "Getrennt";
            // 
            // ClientDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 109);
            this.Controls.Add(this.lbl_connected);
            this.Controls.Add(this.btn_connect);
            this.Controls.Add(this.txt_serverip);
            this.Controls.Add(this.btn_snipping);
            this.Controls.Add(this.btn_picture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ClientDlg";
            this.Text = "RomoteImage - Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_picture;
        private System.Windows.Forms.Button btn_snipping;
        private System.Windows.Forms.TextBox txt_serverip;
        private System.Windows.Forms.Button btn_connect;
        private System.Windows.Forms.Label lbl_connected;
    }
}

