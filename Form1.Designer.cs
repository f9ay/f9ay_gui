namespace f9ay
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnOpen = new Button();
            btnExport = new Button();
            cmbFormat = new ComboBox();
            picturePreview = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)picturePreview).BeginInit();
            SuspendLayout();
            // 
            // btnOpen
            // 
            btnOpen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpen.Location = new Point(1337, 12);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(168, 38);
            btnOpen.TabIndex = 0;
            btnOpen.Text = "Import";
            btnOpen.UseVisualStyleBackColor = true;
            btnOpen.Click += btnOpen_Click;
            // 
            // btnExport
            // 
            btnExport.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnExport.Enabled = false;
            btnExport.Location = new Point(1337, 921);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(168, 38);
            btnExport.TabIndex = 1;
            btnExport.Text = "Export";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += button2_Click;
            // 
            // cmbFormat
            // 
            cmbFormat.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmbFormat.FormattingEnabled = true;
            cmbFormat.Items.AddRange(new object[] { "PNG", "JPEG", "BMP" });
            cmbFormat.Location = new Point(1337, 854);
            cmbFormat.Name = "cmbFormat";
            cmbFormat.Size = new Size(168, 38);
            cmbFormat.TabIndex = 1;
            cmbFormat.Text = " Format";
            cmbFormat.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // picturePreview
            // 
            picturePreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picturePreview.BackColor = SystemColors.ActiveBorder;
            picturePreview.Location = new Point(12, 12);
            picturePreview.Name = "picturePreview";
            picturePreview.Size = new Size(1319, 947);
            picturePreview.SizeMode = PictureBoxSizeMode.Zoom;
            picturePreview.TabIndex = 3;
            picturePreview.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(14F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1517, 971);
            Controls.Add(picturePreview);
            Controls.Add(cmbFormat);
            Controls.Add(btnExport);
            Controls.Add(btnOpen);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)picturePreview).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnOpen;
        private Button btnExport;
        private ComboBox cmbFormat;
        private PictureBox picturePreview;
    }
}
