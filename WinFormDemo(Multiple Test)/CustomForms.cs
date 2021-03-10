using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoyskleyTech.ImageProcessing.Windows.Forms;
using MoyskleyTech.ImageProcessing.Image;
using Bitmap = System.Drawing.Bitmap;
using Graphics = System.Drawing.Graphics;
using Braphics = MoyskleyTech.ImageProcessing.Image.Graphics;
using Rectangle = MoyskleyTech.ImageProcessing.Image.Rectangle;
using Font = MoyskleyTech.ImageProcessing.Image.Font;
using IP = MoyskleyTech.ImageProcessing.Image;
using IPF = MoyskleyTech.ImageProcessing.Form;
using IPC = MoyskleyTech.ImageProcessing.Form.Control;
using MoyskleyTech.ImageProcessing.Form;
using CTRL = MoyskleyTech.ImageProcessing.Form.BaseControl;

namespace WinFormDemo_Multiple_Test_
{
    public partial class CustomForms : Form
    {
        int WIDTH => pbRPG.Width;
        int HEIGHT => pbRPG.Height;
     
        private Bitmap bmp;
        private Bitmap bmpTampon;
        private NativeGraphicsWrapper graphicsTampon;
        private Graphics graphics;
       
        private bool InButton=false;
        private bool ButtonClicked=false;
        private ControlRendering rootControl = new ControlRendering();
        private IPC.Window window;
        private IPC.Button btnOK;
        private IPC.Button btnCancel;
        public CustomForms()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer , true);
        }

        private void RPG_Load(object sender , EventArgs e)
        {
            InitGraphics();
            AddControls();
            CreateFrame();
            DisplayFrame();
        }

        private void AddControls()
        {
            rootControl.AddControl((window=new IPC.Window()
            {
                X = 0 ,
                Y = 0 ,
                Width = WIDTH ,
                Height = HEIGHT ,
                Font = new FontSizeF(new Font("Consolas") , 0.6f) ,
                Title = "Current False Form"
            }).AddRange(
            btnOK=new IPC.Button()
            {
                Font = new FontSizeF(new Font("Arial") , 0.6f) ,
                Text = "OK" ,
                IsHover = InButton ,
                IsPressed = ButtonClicked ,
                X = window.InnerWidth-100 ,
                Y = window.InnerHeight-20 ,
                Height = 20 ,
                Width = 50
            } ,
            btnCancel=new IPC.Button()
            {
                Font = new FontSizeF(new Font("Arial") , 0.6f) ,
                Text = "Cancel" ,
                IsHover = InButton ,
                IsPressed = ButtonClicked ,
                X = window.InnerWidth - 50 ,
                Y = window.InnerHeight - 20 ,
                Height = 20 ,
                Width = 50
            } ,
            new IPC.Panel() { X = 0 , Y = 0 , Width = 292, Height=20, BackColor = (IP.Brush)Pixels.Transparent , BorderColor = ( IP.Brush ) Pixels.Black }.AddRange(
            CheckBoxTest(false , 0 , true) ,
            CheckBoxTest(true , 20 , true) ,
            CheckBoxTest(null , 40 , true) ,
            CheckBoxTest(false , 60 , false) ,
            CheckBoxTest(true , 80 , false) ,
            CheckBoxTest(null , 100 , false)
            ) ,
            new IPC.TrackBar() { X = 10 , Y = 20 , Width = 100 , Height = 15 , Orientation = ProgressBarOrientation.LeftToRight } ,
            new IPC.TrackBar() { X = 10 , Y = 40 , Width = 100 , Height = 15 , Orientation = ProgressBarOrientation.RightToLeft } ,

            new IPC.TrackBar() { X = 120 , Y = 0 , Width = 15 , Height = 55 , Orientation = ProgressBarOrientation.TopToBottom } ,
            new IPC.TrackBar() { X = 140 , Y = 0 , Width = 15 , Height = 55 , Orientation = ProgressBarOrientation.BottomToTop }
            
            ));
            var bp = new IPC.BlockPanel() { X = 0 , Y = 0 , Width = window.InnerWidth , Height = window.InnerHeight , BackColor = ( IP.Brush ) Pixel.FromArgb(Pixels.Black , 50) };
            var btn = new IPC.Button(){ X=130,Y=90,Height=20,Width=60, Text="Close"};
            btn.CursorClick+= (e, v)=>{ bp.Visible = false;/* bp.Parent.RemoveControl(bp);*/ };
            bp.AddControl(btn);
            rootControl.Childrens.ElementAt(0).AddControl(bp);
            btnOK.CursorClick += (e , v) => { bp.Visible = true; };
            btnCancel.CursorClick += (e , v) => { MessageBox.Show("WHAT?"); };
        }

        private void InitGraphics()
        {
            bmp?.Dispose();
            bmpTampon?.Dispose();
            graphics?.Dispose();
            graphicsTampon?.Dispose();
            bmp = new Bitmap(WIDTH , HEIGHT);
            bmpTampon = new Bitmap(WIDTH , HEIGHT);
            graphics = Graphics.FromImage(bmp);
            graphicsTampon = new NativeGraphicsWrapper(Graphics.FromImage(bmpTampon));
        }
        
        private void CreateFrame()
        {
            graphicsTampon.Clear(Pixels.Black);
            rootControl.Render(graphicsTampon , 0 , 0 , 0 , 0);
            
        }

        private IControl CheckBoxTest(bool? chk , int x , bool round)
        {
            return new IPC.CheckBox()
            {
                CheckColor = ( IP.Brush ) Pixels.Green ,
                BackColor = ( IP.Brush ) Pixels.Silver ,
                IsChecked = chk ,
                CheckSize = 2 ,
                BorderSize = 1 ,
                Round = round ,
                BorderColor = ( IP.Brush ) Pixels.Black ,
                X = x ,
                Y = 0 ,
                IsRadio=round,
                Width = 18 ,
                Height = 18
            };
        }

        private void DisplayFrame()
        {
            graphics.DrawImage(bmpTampon , 0 , 0 , WIDTH , HEIGHT);
            pbRPG.Image = bmp;
            this.Invalidate();
        }

       

        private void pbRPG_MouseDown(object sender , MouseEventArgs e)
        {
            rootControl.HandleCursorDownAt(new IP.Point(e.X , e.Y) , e.Button == MouseButtons.Left);
            CreateFrame();
            DisplayFrame();
        }

        private void pbRPG_MouseMove(object sender , MouseEventArgs e)
        {
            rootControl.HandleCursorMove(new IP.Point(e.X , e.Y));

            CreateFrame();
            DisplayFrame();
        }



        private void pbRPG_MouseUp(object sender , MouseEventArgs e)
        {
            rootControl.HandleCursorUpAt(new IP.Point(e.X , e.Y) , e.Button == MouseButtons.Left);

            CreateFrame();
            DisplayFrame();
        }


    }
}
