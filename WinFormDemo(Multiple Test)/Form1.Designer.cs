namespace WinFormDemo_Multiple_Test_
{
    partial class Form1
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
            if ( disposing && ( components != null ) )
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
            this.imgSrc = new System.Windows.Forms.PictureBox();
            this.imgDest = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgSrc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgDest)).BeginInit();
            this.SuspendLayout();
            // 
            // imgSrc
            // 
            this.imgSrc.BackColor = System.Drawing.Color.Black;
            this.imgSrc.Location = new System.Drawing.Point(12, 12);
            this.imgSrc.Name = "imgSrc";
            this.imgSrc.Size = new System.Drawing.Size(128, 237);
            this.imgSrc.TabIndex = 0;
            this.imgSrc.TabStop = false;
            this.imgSrc.Click += new System.EventHandler(this.imgSrc_Click);
            // 
            // imgDest
            // 
            this.imgDest.BackColor = System.Drawing.Color.Black;
            this.imgDest.Location = new System.Drawing.Point(146, 12);
            this.imgDest.Name = "imgDest";
            this.imgDest.Size = new System.Drawing.Size(126, 237);
            this.imgDest.TabIndex = 1;
            this.imgDest.TabStop = false;
            this.imgDest.MouseUp += new System.Windows.Forms.MouseEventHandler(this.imgDest_MouseUp);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.imgDest);
            this.Controls.Add(this.imgSrc);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.imgSrc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgDest)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox imgSrc;
        private System.Windows.Forms.PictureBox imgDest;
    }
}

