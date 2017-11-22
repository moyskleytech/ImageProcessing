using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Charting.Diagram
{
    public class TreeDiagram
    {
        public int MaxShownLevel { get; set; } = int.MaxValue;
        public Pixel Background { get; set; } = Pixels.Transparent;
        public Pixel NodeBackground { get; set; } = Pixels.White;
        public Pixel NodeBorder { get; set; } = Pixels.Black;
        public Pixel NodeForeground { get; set; } = Pixels.Black;
        public int MarginVertical { get; set; } = 10;
        public int MarginHorizontal { get; set; } = 100;
        public int ItemHeight { get; set; } = 100;
        public int ItemWidth { get; set; } = 150;
        public int NodeBorderThickness { get; set; } = 1;
        public FontSize Font { get; set; }
        public TreeGenerator Generator { get; set; } = new TreeGenerator();
        private TreeItem root;

        public object Root { get; set; }
        public TreeDiagram()
        {
            Font = new FontSize(BaseFonts.Premia , 2);
        }
        public void Generate()
        {
            root = GenerateItem(Root , 0);
        }
        private TreeItem GenerateItem(object o , int depth)
        {
            TreeItem item = Generator.GetItem(o);
            LinkedList<TreeItem> successors = new LinkedList<TreeItem>();
            if ( depth < MaxShownLevel )
            {
                var succs = Generator.GetSucessors(o);
                foreach ( var s in succs )
                {
                    successors.AddLast(GenerateItem(s , depth+1));
                }
            }
            item.successors = successors;
            return item;
        }
        public int Height
        {
            get
            {
                return root.GetHeight(MarginVertical , ItemHeight);
            }
        }
        public int Width
        {
            get
            {
                return root.Depth * ( MarginHorizontal + ItemWidth ) + MarginHorizontal;
            }
        }
        public void Draw(Graphics g , int offsetX = 0 , int offsetY = 0)
        {
            int levelHeight = root.GetHeight(MarginVertical, ItemHeight);
            
            g.Clear(Background);
            
            DrawNode(g , root , MarginHorizontal + offsetX , ( levelHeight - ItemHeight ) / 2 + offsetY);
            DrawSuccessors(g , root , MarginHorizontal + offsetX , ( levelHeight - ItemHeight ) / 2 + offsetY , levelHeight);
        }
        public void Draw(Bitmap destination , int offsetX = 0 , int offsetY = 0)
        {
            Graphics g = Graphics.FromImage(destination);
            Draw(g , offsetX , offsetY);
        }
        public Bitmap Draw()
        {
            Bitmap bmp = new Bitmap(Width,Height);
            Draw(bmp);
            return bmp;
        }
        private void DrawSuccessors(Graphics g , TreeItem i , int x , int y,int rootHeight)
        {
            int rootX = x+ItemWidth;
            int rootY = y+ItemHeight/2;

            y = rootY - rootHeight / 2;

            foreach ( var succ in i.successors )
            {
                int levelHeight = succ.GetHeight(MarginVertical, ItemHeight);
                int nx=rootX + MarginHorizontal;
                int ny= y + ( levelHeight - ItemHeight )/ 2;
                DrawNode(g , succ ,nx , ny);
                g.DrawLine(NodeBorder , rootX , rootY , nx , ny+ItemHeight/2 , NodeBorderThickness);

                DrawSuccessors(g , succ , nx , ny ,levelHeight);

                y += levelHeight+MarginVertical;
            }
        }
        private void DrawNode(Graphics g , TreeItem i , int x , int y)
        {
            g.FillRectangle(NodeBackground , x , y , ItemWidth , ItemHeight);
            g.DrawRectangle(NodeBorder , x , y , ItemWidth , ItemHeight , NodeBorderThickness);
            g.DrawString(i.Title , Font , NodeForeground , x , y);
        }
    }
    public class TreeItem
    {
        public string Title { get; set; }
        public Pixel Color { get; set; } = Pixel.FromArgb(0xFF0072ff);
        public int GetHeight(int margin , int itemHeight)
        {
            int ct = successors.Count();
            if ( ct == 0 )
                return itemHeight;
            else
                return successors.Sum((x)=>x.GetHeight(margin,itemHeight)+margin) - margin;
        }
        internal IEnumerable<TreeItem> successors;
        public int Depth
        {
            get
            {
                if ( successors.Count() > 0 )
                    return 1 + successors.Max((x) => x.Depth);
                else
                    return 1;
            }
        }
    }
    public class TreeGenerator
    {
        public virtual TreeItem GetItem(object o) { return null; }
        public virtual IEnumerable<object> GetSucessors(object o) { return new object[0]; }
    }
}
