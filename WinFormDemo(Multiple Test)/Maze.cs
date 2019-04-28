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
using Point = MoyskleyTech.ImageProcessing.Image.Point;
using Font = MoyskleyTech.ImageProcessing.Image.Font;
using IP = MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing.Form;
using CTRL = MoyskleyTech.ImageProcessing.Form.BaseControl;
using MoyskleyTech.Mathematics;
using System.Threading;

namespace WinFormDemo_Multiple_Test_
{
    //http://weblog.jamisbuck.org/2011/1/27/maze-generation-growing-tree-algorithm
    public partial class Maze : Form
    {
        int WIDTH => pbRPG.Width;
        int HEIGHT => pbRPG.Height;
        const int CELL_WIDTH = 40;
        const int CELL_HEIGHT = 40;
        const int BASE_CLOCK=1000;
        private Bitmap bmp;
        private Bitmap bmpTampon;
        private Bitmap playerImage;
        private Dictionary<int,Bitmap> possibilities = new Dictionary<int, Bitmap>();
        private NativeGraphicsWrapper graphicsTampon;
        private Graphics graphics;
        private Dictionary<Point,MazeCell> cells = new Dictionary<Point, MazeCell>();
        private int top=0,left=0;
        private Random rnd = new Random();
        private IP.Brush colorRivers;
        int[] DX = new int[]{1,-1,0,0};
        int[] DY = new int[]{0,0,1,-1};
        private DateTime lastRefresh,lastFPSRefresh;
        double fps,fpst;
        LinkedList<double> fpss = new LinkedList<double>();
        ManualResetEvent evtEnd = new ManualResetEvent(false);
        Task t;
        int fpsCount=0;
        bool ended=false,moved=false,modified=true;
        private int cellCountH;
        private int cellCountV;
        int noGeneration=0;
        Point playerLocation;

        public Maze()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer , true);
        }

        private void RPG_Load(object sender , EventArgs e)
        {
            InitGraphics();
            CreateFrame();
            DisplayFrame();
            lastFPSRefresh = lastRefresh = DateTime.Now;
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
            cellCountH = ( int ) Math.Ceiling(( double ) WIDTH / CELL_WIDTH);
            cellCountV = ( int ) Math.Ceiling(( double ) Height / CELL_HEIGHT);

            if ( playerImage == null )
            {
                playerImage = new Bitmap(CELL_WIDTH , CELL_HEIGHT);
                var g = new NativeGraphicsWrapper(Graphics.FromImage(playerImage));

                var five = CELL_WIDTH/6;
                var ten = CELL_WIDTH/3;
                var fifteen = ten+five;
                var twenty = ten*2;
                g.FillEllipse(Pixels.DeepPink , five , five , CELL_WIDTH - ten , CELL_HEIGHT - ten);
                g.DrawEllipse(Pixels.Black , five , five , CELL_WIDTH - ten , CELL_HEIGHT - ten);

                g.FillEllipse(Pixels.White , ten , ten , five , five);
                g.FillEllipse(Pixels.White , CELL_WIDTH - fifteen , ten , five , five);

                g.DrawEllipse(Pixels.Black , ten , ten , five , five);
                g.DrawEllipse(Pixels.Black , CELL_WIDTH - fifteen , ten , five , five);

                g.DrawArc(Pixels.Black , ten , ten , CELL_WIDTH - twenty , CELL_HEIGHT - twenty , 0 , Math.PI);
                g.Dispose();
            }
            if ( colorRivers == null )
            {
                IP.Bitmap bmp = new IP.Bitmap(50,50);
                var g=Braphics.FromImage(bmp);
                g.Clear(Pixels.DeepSkyBlue);
                for ( var i = 0; i < 25; i++ )
                {
                    var pt = new Point(rnd.Next(0,46),rnd.Next(0,46));
                    g.DrawLine(Pixels.White , pt , pt.Move(7 , 3));
                }
                colorRivers = new ImageBrush(bmp);
            }
        }

        private void CreateFrame()
        {
            graphicsTampon.Clear(Pixels.White);
            MakeSureAllCellFitted();
            SetMazeToSeePlayer();
            for ( var i = 0; i < cellCountH; i++ )
            {
                for ( var j = 0; j < cellCountV; j++ )
                {
                    var pt = new Point(i+left,j+top);
                    if ( cells.ContainsKey(pt) )
                    {
                        var cell = cells[pt];

                        var cellTopLeft = new Point(i*CELL_WIDTH, j*CELL_HEIGHT);
                        var topLeft = new Point(0,0);
                        var topRight = new Point(CELL_WIDTH,0);
                        var bottomLeft = new Point(0,CELL_HEIGHT);
                        var bottomRight = new Point(CELL_WIDTH,CELL_HEIGHT);
                        var colorContour = colorRivers;
                        int thickness=CELL_WIDTH/2;

                        if ( !possibilities.ContainsKey(cell.Value) )
                        {
                            noGeneration++;
                            Bitmap bitmap = new Bitmap(CELL_WIDTH,CELL_HEIGHT);
                            var gt = Graphics.FromImage(bitmap);
                            var g = new NativeGraphicsWrapper(gt);

                            g.Clear(Pixels.DarkGreen);

                            for ( var c = 0; c < 100; c++ )
                                g.SetPixel(Pixels.LawnGreen , rnd.Next(CELL_WIDTH) , rnd.Next(CELL_HEIGHT));

                            //g.DrawString(noGeneration.ToString() , new FontSize(new Font("Consolas") , 1) , Pixels.Red , 5 , 5);

                            if ( !cell.Left )
                                g.DrawLine(colorContour , topLeft , bottomLeft , thickness);
                            if ( !cell.Right )
                                g.DrawLine(colorContour , topRight , bottomRight , thickness);
                            if ( !cell.Top )
                                g.DrawLine(colorContour , topLeft , topRight , thickness);
                            if ( !cell.Bottom )
                                g.DrawLine(colorContour , bottomLeft , bottomRight , thickness);

                            g.FillCircle(colorContour , topLeft , thickness / 2);
                            g.FillCircle(colorContour , topRight , thickness / 2);
                            g.FillCircle(colorContour , bottomLeft , thickness / 2);
                            g.FillCircle(colorContour , bottomRight , thickness / 2);

                            gt.Dispose();
                            possibilities[cell.Value] = bitmap;
                        }
                        graphicsTampon.DrawImage(possibilities[cell.Value] , i * CELL_WIDTH , j * CELL_HEIGHT);
                        if ( pt == playerLocation )
                            graphicsTampon.DrawImage(playerImage , i * CELL_WIDTH , j * CELL_HEIGHT);
                    }
                }
            }

            graphicsTampon.DrawString(fps.ToString("000.0") + " fps" , new FontSize(new Font("Consolas") , 4) , Pixels.White , 0 , 0);

        }

        private void MakeSureAllCellFitted()
        {

            if ( t == null )
            {
                t = Task.Run(async () =>
                {
                    if ( !cells.Any() )
                        AddRandomCell();
                    InitialFill(-100 , -100 , 200 , 200);
                    while ( !ended )
                    {
                        Fill();
                        await Task.Delay(5);
                    }
                    evtEnd.Set();
                });
            }
        }

        private void AddRandomCell()
        {
            playerLocation = new Point(cellCountH / 2 , cellCountV / 2);
            cells[playerLocation] = new MazeCell();
        }
        private void InitialFill(int left , int top , int cellCountH , int cellCountV)
        {
            //const int mx=BASE_CLOCK/CELL_WIDTH;
            List<Point> C = new List<Point>();
            if ( moved || modified )
                C.AddRange(
                    from x in cells
                    where x.Key.X >= left - 1 && x.Key.Y >= top - 1 && x.Key.X < left + cellCountH + 1 && x.Key.Y < top + cellCountV + 1
                    select x.Key);
            moved = modified = false;
            while ( C.Any() && !ended )
            {
                var idx = GetIdx(C);
                Point p = C[idx];
                var Directions = new int[]{0,1,2,3}.OrderBy((x)=>rnd.Next()).ToArray();
                bool found=false;
                foreach ( var dir in Directions )
                {
                    var nx = p.X + DX[dir];
                    var ny = p.Y +DY[dir];
                    var nloc = new Point(nx , ny);
                    if ( nx >= left && ny >= top && nx < left + cellCountH && ny < top + cellCountV && !cells.ContainsKey(nloc) )
                    {
                        var tmp =cells[p];
                        tmp.Value |= GetMask(dir);
                        cells[p] = tmp;
                        var ncell = new MazeCell(GetMask(Opposite(dir)));
                        cells[nloc] = ncell;
                        C.Add(nloc);
                        found = true;
                        modified = true;
                        break;
                    }
                }
                if ( !found )
                    C.RemoveAt(idx);
            }
        }
        private void Fill()
        {
            //const int mx=BASE_CLOCK/CELL_WIDTH;
            //int ct=mx;
            List<Point> C = new List<Point>();
            if ( moved || modified )
                C.AddRange(
                    from x in cells
                    where x.Key.X >= left - 1 && x.Key.Y >= top - 1 && x.Key.X < left + cellCountH + 1 && x.Key.Y < top + cellCountV + 1
                    select x.Key);
            moved = modified = false;
            while ( C.Any() && !ended && !moved )
            {
                var idx = GetIdx(C);
                Point p = C[idx];
                var Directions = new int[]{0,1,2,3}.OrderBy((x)=>rnd.Next()).ToArray();
                bool found=false;
                foreach ( var dir in Directions )
                {
                    var nx = p.X + DX[dir];
                    var ny = p.Y +DY[dir];
                    var nloc = new Point(nx , ny);
                    if ( nx >= left && ny >= top && nx < left + cellCountH && ny < top + cellCountV && !cells.ContainsKey(nloc) )
                    {
                        var tmp =cells[p];
                        tmp.Value |= GetMask(dir);
                        cells[p] = tmp;
                        var ncell = new MazeCell(GetMask(Opposite(dir)));
                        cells[nloc] = ncell;
                        C.Add(nloc);
                        found = true;
                        //ct--;
                        //if ( ct <= 0 )
                        //{
                        //    await Task.Delay(1);
                        //    ct = mx;
                        //}
                        modified = true;
                        break;
                    }
                }
                if ( !found )
                    C.RemoveAt(idx);
            }
        }

        private int GetIdx(List<Point> C)
        {
            //return C.Count - 1;
            //return C.Count /2;
            return rnd.Next(C.Count);
        }

        private void DisplayFrame()
        {
            graphics.DrawImage(bmpTampon , 0 , 0 , WIDTH , HEIGHT);
            pbRPG.Image = bmp;
            this.Invalidate();
        }
        public int Opposite(int dir)
        {
            if ( dir == 0 )
                return 1;
            if ( dir == 1 )
                return 0;
            if ( dir == 2 )
                return 3;
            if ( dir == 3 )
                return 2;
            return 0;
        }
        public byte GetMask(int dir)
        {
            if ( dir == 0 )
                return MazeCell.RIGHT_MASK;
            if ( dir == 1 )
                return MazeCell.LEFT_MASK;
            if ( dir == 2 )
                return MazeCell.BOTTOM_MASK;
            if ( dir == 3 )
                return MazeCell.TOP_MASK;
            return 0;
        }
        public class Dictionary<TKey1, TKey2, TValue> : Dictionary<Tuple<TKey1 , TKey2> , TValue>, IDictionary<Tuple<TKey1 , TKey2> , TValue>
        {

            public TValue this[TKey1 key1 , TKey2 key2]
            {
                get { return base[Tuple.Create(key1 , key2)]; }
                set { base[Tuple.Create(key1 , key2)] = value; }
            }

            public void Add(TKey1 key1 , TKey2 key2 , TValue value)
            {
                base.Add(Tuple.Create(key1 , key2) , value);
            }

            public bool ContainsKey(TKey1 key1 , TKey2 key2)
            {
                return base.ContainsKey(Tuple.Create(key1 , key2));
            }
        }

        private void Maze_ResizeEnd(object sender , EventArgs e)
        {
            InitGraphics();
            SetMazeToSeePlayer();
            CreateFrame();
            DisplayFrame();
        }

        private void SetMazeToSeePlayer()
        {
            MazeLeft = playerLocation.X - cellCountH / 2 + 1;
            MazeTop = playerLocation.Y - cellCountV / 2 + 1;
        }

        private void Maze_FormClosing(object sender , FormClosingEventArgs e)
        {
            ended = true;
            evtEnd.WaitOne();
        }

        private void Maze_KeyDown(object sender , KeyEventArgs e)
        {
            if ( e.KeyCode == Keys.Left && CanGoLeft )
            { moved = true; playerLocation.X--; }
            if ( e.KeyCode == Keys.Right && CanGoRight )
            { moved = true; playerLocation.X++; }
            if ( e.KeyCode == Keys.Up && CanGoUp )
            { moved = true; playerLocation.Y--; }
            if ( e.KeyCode == Keys.Down && CanGoDown )
            { moved = true; playerLocation.Y++; }
        }
        private int MazeLeft { get { return left; } set { left = value; } }
        private int MazeTop { get { return top; } set { top = value; } }
        private bool CanGoLeft { get => cells[playerLocation].Left; }
        private bool CanGoRight { get => cells[playerLocation].Right; }
        private bool CanGoUp { get => cells[playerLocation].Top; }
        private bool CanGoDown { get => cells[playerLocation].Bottom; }

        private void tmrRefresh_Tick(object sender , EventArgs e)
        {
            CreateFrame();
            DisplayFrame();

            fpst = ( double ) TimeSpan.FromSeconds(1).Ticks / ( DateTime.Now - lastRefresh ).Ticks;
            fpss.AddLast(fpst);

            if ( fpsCount >= 150 )
            {
                fps = fpss.Average();
                fpss.RemoveFirst();
            }
            else
                fpsCount++;
            lastRefresh = DateTime.Now;
        }

        private struct MazeCell
        {
            public const int LEFT_MASK = 0b1000;
            public const int TOP_MASK = 0b0100;
            public const int RIGHT_MASK = 0b0010;
            public const int BOTTOM_MASK = 0b0001;

            public MazeCell(byte v)
            {
                Value = v;
            }
            public byte Value { get; set; }
            public bool Left { get => ( Value & LEFT_MASK ) > 0; set => Value = ( byte ) ( ( Value & ~LEFT_MASK ) | ( value ? LEFT_MASK : 0 ) ); }
            public bool Top { get => ( Value & TOP_MASK ) > 0; set => Value = ( byte ) ( ( Value & ~TOP_MASK ) | ( value ? TOP_MASK : 0 ) ); }
            public bool Right { get => ( Value & RIGHT_MASK ) > 0; set => Value = ( byte ) ( ( Value & ~RIGHT_MASK ) | ( value ? RIGHT_MASK : 0 ) ); }
            public bool Bottom { get => ( Value & BOTTOM_MASK ) > 0; set => Value = ( byte ) ( ( Value & ~BOTTOM_MASK ) | ( value ? BOTTOM_MASK : 0 ) ); }
            public override string ToString()
            {
                return string.Format("L:{0},T:{1},R:{2},B:{3}" , Left , Top , Right , Bottom);
            }
        }
    }
}
