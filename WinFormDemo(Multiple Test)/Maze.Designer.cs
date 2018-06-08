namespace WinFormDemo_Multiple_Test_
{
    partial class Maze
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
            this.components = new System.ComponentModel.Container();
            this.pbRPG = new System.Windows.Forms.PictureBox();
            this.tmrRefresh = new System.Windows.Forms.Timer(this.components);
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
            // 
            // tmrRefresh
            // 
            this.tmrRefresh.Enabled = true;
            this.tmrRefresh.Interval = 10;
            this.tmrRefresh.Tick += new System.EventHandler(this.tmrRefresh_Tick);
            // 
            // Maze
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(524, 382);
            this.Controls.Add(this.pbRPG);
            this.Name = "Maze";
            this.Text = "RPG";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Maze_FormClosing);
            this.Load += new System.EventHandler(this.RPG_Load);
            this.ResizeEnd += new System.EventHandler(this.Maze_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.Maze_ResizeEnd);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Maze_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pbRPG)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbRPG;
        private System.Windows.Forms.Timer tmrRefresh;
    }
}