namespace FontEditor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fichierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ouvrirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enregistrerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nouvelleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pbChar = new System.Windows.Forms.PictureBox();
            this.btnChangeSize = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.tbChar = new System.Windows.Forms.TextBox();
            this.tbDemo = new System.Windows.Forms.TextBox();
            this.importerLensembleDesFontsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbChar)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fichierToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(543, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fichierToolStripMenuItem
            // 
            this.fichierToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ouvrirToolStripMenuItem,
            this.enregistrerToolStripMenuItem,
            this.nouvelleToolStripMenuItem,
            this.importerToolStripMenuItem,
            this.importerLensembleDesFontsToolStripMenuItem});
            this.fichierToolStripMenuItem.Name = "fichierToolStripMenuItem";
            this.fichierToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.fichierToolStripMenuItem.Text = "Fichier";
            // 
            // ouvrirToolStripMenuItem
            // 
            this.ouvrirToolStripMenuItem.Name = "ouvrirToolStripMenuItem";
            this.ouvrirToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.ouvrirToolStripMenuItem.Text = "Ouvrir";
            this.ouvrirToolStripMenuItem.Click += new System.EventHandler(this.OuvrirToolStripMenuItem_Click);
            // 
            // enregistrerToolStripMenuItem
            // 
            this.enregistrerToolStripMenuItem.Name = "enregistrerToolStripMenuItem";
            this.enregistrerToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.enregistrerToolStripMenuItem.Text = "Enregistrer";
            this.enregistrerToolStripMenuItem.Click += new System.EventHandler(this.EnregistrerToolStripMenuItem_Click);
            // 
            // nouvelleToolStripMenuItem
            // 
            this.nouvelleToolStripMenuItem.Name = "nouvelleToolStripMenuItem";
            this.nouvelleToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.nouvelleToolStripMenuItem.Text = "Nouvelle";
            this.nouvelleToolStripMenuItem.Click += new System.EventHandler(this.NouvelleToolStripMenuItem_Click);
            // 
            // importerToolStripMenuItem
            // 
            this.importerToolStripMenuItem.Name = "importerToolStripMenuItem";
            this.importerToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.importerToolStripMenuItem.Text = "Importer";
            this.importerToolStripMenuItem.Click += new System.EventHandler(this.ImporterToolStripMenuItem_Click);
            // 
            // pbChar
            // 
            this.pbChar.Location = new System.Drawing.Point(12, 53);
            this.pbChar.Name = "pbChar";
            this.pbChar.Size = new System.Drawing.Size(519, 427);
            this.pbChar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbChar.TabIndex = 2;
            this.pbChar.TabStop = false;
            this.pbChar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PbChar_MouseUp);
            // 
            // btnChangeSize
            // 
            this.btnChangeSize.Location = new System.Drawing.Point(138, 24);
            this.btnChangeSize.Name = "btnChangeSize";
            this.btnChangeSize.Size = new System.Drawing.Size(113, 23);
            this.btnChangeSize.TabIndex = 3;
            this.btnChangeSize.Text = "Change char size";
            this.btnChangeSize.UseVisualStyleBackColor = true;
            this.btnChangeSize.Click += new System.EventHandler(this.BtnChangeSize_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(418, 24);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(113, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "SaveChar";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.Button2_Click);
            // 
            // tbChar
            // 
            this.tbChar.Location = new System.Drawing.Point(12, 27);
            this.tbChar.MaxLength = 1;
            this.tbChar.Name = "tbChar";
            this.tbChar.Size = new System.Drawing.Size(100, 20);
            this.tbChar.TabIndex = 5;
            this.tbChar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TbChar_KeyPress);
            // 
            // tbDemo
            // 
            this.tbDemo.Location = new System.Drawing.Point(281, 27);
            this.tbDemo.MaxLength = 20;
            this.tbDemo.Name = "tbDemo";
            this.tbDemo.Size = new System.Drawing.Size(100, 20);
            this.tbDemo.TabIndex = 6;
            this.tbDemo.TextChanged += new System.EventHandler(this.TbDemo_TextChanged);
            // 
            // importerLensembleDesFontsToolStripMenuItem
            // 
            this.importerLensembleDesFontsToolStripMenuItem.Name = "importerLensembleDesFontsToolStripMenuItem";
            this.importerLensembleDesFontsToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.importerLensembleDesFontsToolStripMenuItem.Text = "Importer l\'ensemble des fonts";
            this.importerLensembleDesFontsToolStripMenuItem.Click += new System.EventHandler(this.ImporterLensembleDesFontsToolStripMenuItem_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 486);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(519, 23);
            this.progressBar1.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 512);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.tbDemo);
            this.Controls.Add(this.tbChar);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnChangeSize);
            this.Controls.Add(this.pbChar);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbChar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fichierToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ouvrirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enregistrerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nouvelleToolStripMenuItem;
        private System.Windows.Forms.PictureBox pbChar;
        private System.Windows.Forms.Button btnChangeSize;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox tbChar;
        private System.Windows.Forms.ToolStripMenuItem importerToolStripMenuItem;
        private System.Windows.Forms.TextBox tbDemo;
        private System.Windows.Forms.ToolStripMenuItem importerLensembleDesFontsToolStripMenuItem;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}

