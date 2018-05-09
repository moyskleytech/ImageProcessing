using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoyskleyTech.ImageProcessing.WinForm;
using MoyskleyTech.ImageProcessing.Image;
using Bitmap = System.Drawing.Bitmap;
using Graphics = System.Drawing.Graphics;
using Braphics = MoyskleyTech.ImageProcessing.Image.Graphics;
using Rectangle = MoyskleyTech.ImageProcessing.Image.Rectangle;
using Font = MoyskleyTech.ImageProcessing.Image.Font;
namespace WinFormDemo_Multiple_Test_
{
    public partial class RPG : Form
    {
        int WIDTH=>pbRPG.Width;
        int HEIGHT=> pbRPG.Height;
        int MESSAGE_TOP => HEIGHT - MESSAGE_HEIGHT;
        int MESSAGE_HEIGHT=>Math.Min(200,HEIGHT/3);
        const string FONT_NAME = "POKEMON GB";
        private Bitmap bmp;
        private Bitmap bmpTampon;
        private NativeGraphicsWrapper graphicsTampon;
        private Graphics graphics;
        private Braphics messagePane;
        private Rectangle messageArea;
        private Font rpgFont;
        private string txt;
        public RPG()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer , true);
        }

        private void RPG_Load(object sender , EventArgs e)
        {
            InitGraphics();
            rpgFont = FontLibrary.WindowsFonts.Get(FONT_NAME);
            CreateFrame();
            DisplayFrame();

            /*test*/
            var _z= new _332() { R = 3 , G = 3 , B = 2 };
            var conversion = ColorConvert.GetConversionFrom<_332 , HSL>();
            var hsl = conversion(_z);
            var conversion2 = ColorConvert.GetConversionFrom<HSL , BGR>();
            var px = conversion2(hsl);
            int i=5;
        }

        private void InitGraphics()
        {
            bmp?.Dispose();
            bmpTampon?.Dispose();
            graphics?.Dispose();
            graphicsTampon?.Dispose();
            messagePane?.Dispose();

            bmp = new Bitmap(WIDTH , HEIGHT);
            bmpTampon = new Bitmap(WIDTH , HEIGHT);
            messageArea = messageArea = new Rectangle(0 , MESSAGE_TOP , WIDTH , MESSAGE_HEIGHT);
            graphics = Graphics.FromImage(bmp);
            graphicsTampon = new NativeGraphicsWrapper(Graphics.FromImage(bmpTampon));
            messagePane = (Braphics)graphicsTampon.Proxy(messageArea);
        }

        private void ShowMessage(string message)
        {
            graphicsTampon.FillRectangle(Pixels.DarkBlue , messageArea.X, messageArea.Y, messageArea.Width, messageArea.Height);
            //messagePane.Clear(Pixels.DarkBlue);
            messagePane.DrawRectangle(Pixels.White , 1 , 1 , WIDTH - 2 , MESSAGE_HEIGHT - 2);
            var splitted = message.Split(' ');
            var str = "";
            var idx=0;
            var sidx=0;
            var line=0;
            var lineHeight = (int)messagePane.MeasureString(FONT_NAME,rpgFont,1).Height;
            var maxLineCount = (messageArea.Height-6)/lineHeight;
            while ( idx < splitted.Length && line<maxLineCount )
            {
                while (sidx==idx ||( idx < splitted.Length && messagePane.MeasureString(str + " " + splitted[idx] , rpgFont , 1).Width < messageArea.Width - 6 ))
                {
                    str += ((string.IsNullOrWhiteSpace(str))?"":" ") + splitted[idx];
                    idx++;
                }
                messagePane.DrawString(str , Pixels.White , 3 , 3 + line*(lineHeight+1) , rpgFont , 1);
                line++;
                sidx = idx;
                str = "";
            }
            
        }
        private void CreateFrame()
        {
            graphicsTampon.Clear(Pixels.Black);
            ShowMessage(txt??"Welcome, this is a test message, very long test message that should get across multiple lines");
        }
        private void DisplayFrame()
        {
            graphics.DrawImage(bmpTampon,0,0,WIDTH,HEIGHT);
            pbRPG.Image = bmp;
            this.Invalidate();
        }

        private void RPG_KeyPress(object sender , KeyPressEventArgs e)
        {
            var c = e.KeyChar;
            if ( c == '\r' )
                txt += '\n';
            else if ( c == '\b' )
                txt = txt.Substring(0 , txt.Length - 1);
            else txt += c;

            CreateFrame();
            DisplayFrame();
        }

        private void RPG_Resize(object sender , EventArgs e)
        {
            InitGraphics();
            CreateFrame();
            DisplayFrame();
        }
    }
}
