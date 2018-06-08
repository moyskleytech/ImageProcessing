namespace WinFormDemo_Multiple_Test_
{
    partial class RPG
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
            this.pbRPG = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbRPG)).BeginInit();
            this.SuspendLayout();
            // 
            // pbRPG
            // 
            this.pbRPG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbRPG.Location = new System.Drawing.Point(0, 0);
            this.pbRPG.Name = "pbRPG";
            this.pbRPG.Size = new System.Drawing.Size(524, 382);
            this.pbRPG.TabIndex = 0;
            this.pbRPG.TabStop = false;
            this.pbRPG.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbRPG_MouseDown);
            this.pbRPG.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbRPG_MouseMove);
            this.pbRPG.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbRPG_MouseUp);
            // 
            // RPG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(524, 382);
            this.Controls.Add(this.pbRPG);
            this.Name = "RPG";
            this.Text = "RPG";
            this.Load += new System.EventHandler(this.RPG_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RPG_KeyPress);
            this.Resize += new System.EventHandler(this.RPG_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pbRPG)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbRPG;
    }
}