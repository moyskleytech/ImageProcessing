using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing;
using Microsoft.VisualBasic;
using System.Drawing.Text;
using MoyskleyTech.ImageProcessing.WinForm;

namespace FontEditor
{

    public partial class Form1 : Form
    {
        Font current= new MoyskleyTech.ImageProcessing.Image.Font("");
        //Bitmap bmp;
        Graphics<Pixel> g,gu;
        
        System.Drawing.Bitmap img;
        int currentCharIndex=0;
        bool[,] currentChar= new bool[0,0];
        Pixel grid,black,transparent;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender , EventArgs e)
        {
            // bmp = new Bitmap(pbChar.Width , pbChar.Height);
            img = new System.Drawing.Bitmap(pbChar.Width , pbChar.Height);
            grid = Pixel.FromArgb(0xFF0072FF);
            black = Pixel.FromArgb(0xFF000000);
            transparent = Pixel.FromArgb(0);
            g = (gu=new NativeGraphicsWrapper(System.Drawing.Graphics.FromImage(img))).Proxy(new Rectangle(0 , 0 , img.Width , img.Height));
            //g = Graphics.FromImage(bmp);
        }

        private void OuvrirToolStripMenuItem_Click(object sender , EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Font | *.bin"
            };
            if ( ofd.ShowDialog() == DialogResult.OK )
            {
                var fs =System.IO.File.Open(ofd.FileName , System.IO.FileMode.Open , System.IO.FileAccess.Read);
                current = MoyskleyTech.ImageProcessing.Image.Font.FromFileStream(fs);
                fs.Close();
            }
        }

        private void EnregistrerToolStripMenuItem_Click(object sender , EventArgs e)
        {
            SaveFileDialog svd = new SaveFileDialog
            {
                Filter = "Font | *.bin"
            };
            if ( svd.ShowDialog() == DialogResult.OK )
            {
                var fs =System.IO.File.Open(svd.FileName , System.IO.FileMode.OpenOrCreate , System.IO.FileAccess.Write);
                current.ToFileStream(fs);
                fs.Close();
            }
        }

        private void NouvelleToolStripMenuItem_Click(object sender , EventArgs e)
        {
            current = new MoyskleyTech.ImageProcessing.Image.Font(Microsoft.VisualBasic.Interaction.InputBox("Quel nom?"));
        }

        private void Button2_Click(object sender , EventArgs e)
        {
            current.SetChar(currentCharIndex , currentChar);
        }

        private void BtnChangeSize_Click(object sender , EventArgs e)
        {
            string i1,i2;
            i1 = Interaction.InputBox("Lignes");
            i2 = Interaction.InputBox("Colonnes");

            int w,h;
            h = int.Parse(i1);
            w = int.Parse(i2);

            currentChar = new bool[h , w];

            ShowChar();
        }

        private void TbChar_KeyPress(object sender , KeyPressEventArgs e)
        {
            tbDemo.Text = tbChar.Text = e.KeyChar.ToString();
            LoadChar(e.KeyChar);
        }

        private void PbChar_MouseUp(object sender , MouseEventArgs e)
        {
            int charw = currentChar.GetLength(1);
            int charh = currentChar.GetLength(0);
            if ( charh != 0 && charw != 0 )
            {
                int width = img.Width/charw;
                int height = img.Height/charh;

                int x,y;
                x = e.X / width;
                y = e.Y / height;
                if ( x < charw && y < charh )
                    currentChar[y , x] = !currentChar[y , x];
            }
            ShowChar();
        }

        private void ImporterToolStripMenuItem_Click(object sender , EventArgs e)
        {
            FontDialog fd = new FontDialog
            {
                ShowApply = true
            };
            fd.Apply += Fd_Apply;
            if ( fd.ShowDialog() == DialogResult.OK )
            {
                Font f = new MoyskleyTech.ImageProcessing.Image.Font(fd.Font.Name);
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(100,100);
                var g = System.Drawing.Graphics.FromImage(bmp) ;
                double maxx=0;
                double maxy=0;
                for ( var i = 32 ; i < 255 ; i++ )
                {
                    char c = (char)i;
                    var measure = g.MeasureString(c.ToString() , fd.Font);
                    maxx = Math.Max(maxx , measure.Width);
                    maxy = Math.Max(maxy , measure.Height);
                }
                for ( var i = 32 ; i < 255 ; i++ )
                {
                    char c = (char)i;
                    HandleChar(fd.Font , f , bmp , g , c,1);
                }
                HandleChar(fd.Font , f , bmp , g , ' ' , 1);
                HandleChar(fd.Font , f , bmp , g , '\t' , 1);
                current = f;
            }

            ShowChar();
        }

        private static void HandleChar(System.Drawing.Font fd , Font f , System.Drawing.Bitmap bmp , System.Drawing.Graphics g , char c,int size)
        {
            g.Clear(System.Drawing.Color.White);
            var measure = g.MeasureString(c.ToString() , fd);
            g.DrawString(c.ToString() , fd , System.Drawing.Brushes.Black , -2 , 0);

            int w = (int)Math.Ceiling(measure.Width)-4*size;
            int h = (int)Math.Ceiling(measure.Height);

            bool[,] tmp = new bool[h,w];
            if ( c == ' ' )
                tmp = new bool[h , (int)measure.Width];
            else if ( c == '\t' )
                tmp = new bool[h , ( int ) measure.Width/2];
            else
                for ( var x = 0 ; x < w ; x++ )
                {
                    for ( var y = 0 ; y < h ; y++ )
                    {
                        tmp[y , x] = bmp.GetPixel(x , y).B < 200;
                    }
                }

            f.SetChar(c , tmp);
        }

        private void Fd_Apply(object sender , EventArgs e)
        {
            FontDialog fd = (FontDialog)sender;
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(currentChar.GetLength(1),currentChar.GetLength(0));
            var g=System.Drawing.Graphics.FromImage(bmp);
            g.DrawString("a" , fd.Font , System.Drawing.Brushes.Black , 0 , 0);
            g.Dispose();
            pbChar.Image = bmp;
        }

        private void TbDemo_TextChanged(object sender , EventArgs e)
        {
            ShowChar();
        }

        private async void ImporterLensembleDesFontsToolStripMenuItem_Click(object sender , EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog
            {
                ShowNewFolderButton = true
            };
            if ( fd.ShowDialog() == DialogResult.OK )
            {
                await ImportFonts(fd,1);
                await ImportFonts(fd , 2);
            }
        }

        private async Task ImportFonts(FolderBrowserDialog fd,int size)
        {
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();

            // Get the array of FontFamily objects.
            var fontFamilies = installedFontCollection.Families;
            progressBar1.Maximum = fontFamilies.Length;
            progressBar1.Value = 0;
            foreach ( var ff in fontFamilies )
            {
                await Task.Run(() =>
                {
                    System.Drawing.Font f2 = new System.Drawing.Font(ff,10*size);
                    Font f = new MoyskleyTech.ImageProcessing.Image.Font(ff.Name);
                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(100,100);
                    var g = System.Drawing.Graphics.FromImage(bmp) ;
                    double maxx=0;
                    double maxy=0;
                    for ( var i = 32 ; i < 255 ; i++ )
                    {
                        char c = (char)i;
                        var measure = g.MeasureString(c.ToString() , f2);
                        maxx = Math.Max(maxx , measure.Width);
                        maxy = Math.Max(maxy , measure.Height);
                    }
                    for ( var i = 32 ; i < 255 ; i++ )
                    {
                        char c = (char)i;
                        HandleChar(f2 , f , bmp , g , c,size);
                    }
                    HandleChar(f2 , f , bmp , g , ' ' , size);
                    HandleChar(f2 , f , bmp , g , '\t' , size);
                    current = f;
                    var suffix = "";
                    if ( size > 1 )
                    {
                        suffix = "_" + size + "x";
                    }
                    var fs=System.IO.File.OpenWrite(fd.SelectedPath + "\\" + ff.Name+suffix + ".bin");
                    f.ToFileStream(fs);
                    fs.Close();
                });
                progressBar1.Value++;
            }
        }

        private void LoadChar(int i)
        {
            current.SetChar(currentCharIndex , currentChar);
            currentCharIndex = i;
            currentChar = current.GetChar(( char ) i);

            ShowChar();
        }

        private void ShowChar()
        {
            gu.Clear(transparent);

            int charw = currentChar.GetLength(1);
            int charh = currentChar.GetLength(0);
            if ( charh != 0 && charw != 0 )
            {
                int width = img.Width/charw;
                int height = img.Height/charh;

                for ( var i = 0 ; i < charh ; i++ )
                {
                    gu.DrawLine(grid , 0 , i * height , img.Width , i * height);
                }

                for ( var i = 0 ; i < charh ; i++ )
                {
                    gu.DrawLine(grid , i * width , 0 , i * width , img.Height);
                    gu.DrawLine(grid , ( i + 1 ) * width , 0 , ( i + 1 ) * width , img.Height);
                }

                for ( var i = 0 ; i < charh ; i++ )
                {
                    for ( var j = 0 ; j < charw ; j++ )
                    {
                        if ( currentChar[i , j] )
                            gu.FillRectangle(black , j * width , i * height , width , height);
                            //for ( var k = 0 ; k < width ; k++ )
                            //    gu.DrawLine(black , j * width + k , i * height , j * width + k , i * height + height);
                    }
                }

            }
            g.DrawString(tbDemo.Text.Replace("\\t","\t") , grid , 3 , 3 , current , 1);
            //var loc = img.LockBits(new System.Drawing.Rectangle(0 , 0 , bmp.Width , bmp.Height) , System.Drawing.Imaging.ImageLockMode.WriteOnly , System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //bmp.CopyToBGRA(loc.Scan0);
            //img.UnlockBits(loc);
            pbChar.Image = img;
            pbChar.Invalidate();
        }
    }
}
