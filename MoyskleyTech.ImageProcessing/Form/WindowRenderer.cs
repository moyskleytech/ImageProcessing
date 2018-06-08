using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing.Image.SpecializedBrushes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Form
{
    public class WindowRenderer : IRenderer
    {
        public Brush<Pixel> BackColor { get; set; } = ( Brush ) Pixels.LightGray;
        public Brush<Pixel> BorderColor { get; set; } = ( Brush ) Pixels.LightSalmon;
        public int BorderSize { get; set; } = 5;
        public int TopBorderSize { get; set; } = 15;
        public string Title { get; set; } = "Form";
        public FontSizeF Font { get; set; } = null;
        public StringFormat TextAlign { get; set; } = new StringFormat() { Alignment = StringAlignment.Center , LineAlignment = StringAlignment.Center , EllipsisMode = EllipsisMode.Word };
        public Brush<Pixel> ForeColor { get; set; } = ( Brush ) Pixels.Black;
        
        public virtual void Render(Graphics<Pixel> g , int x , int y , int width , int height)
        {
            if ( Font == null )
                Font = new FontSizeF(BaseFonts.Premia , 1);
            g.FillRectangle(BackColor , x , y , width , height);
            g.FillRectangle(BorderColor , x , y , width , TopBorderSize);
            var topRect= new Rectangle(x,y,width,TopBorderSize);
            g.DrawRectangle(BorderColor , x , y , width , height,BorderSize);
            g.DrawString(Title , Font , ForeColor , topRect , new StringFormat() { EllipsisMode = EllipsisMode.None });
        }
    }
}
