using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing.Image.SpecializedBrushes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Form
{
    public class LabelRenderer:IRenderer
    {
        public string Text { get; set; } = "Label 1";
        public FontSizeF Font { get; set; } = null;
        public bool AutoSize { get; set; } = true;
        public StringFormat TextAlign { get; set; } = new StringFormat() { Alignment = StringAlignment.Center , LineAlignment = StringAlignment.Center, EllipsisMode= EllipsisMode.Word };
        public Brush<Pixel> ForeColor { get; set; } = ( Brush ) Pixels.Black;

        public virtual void Render(Graphics<Pixel> g ,int x,int y, int width , int height)
        {
            if ( Font == null )
                Font = new FontSizeF(BaseFonts.Premia , 1);

            if ( AutoSize )
                g.DrawString(Text , Font , ForeColor , x , y);
            else
            {
                var textBound = new Rectangle(x,y,width,height);
                g.DrawString(Text , Font , ForeColor , textBound , TextAlign);
            }
        }
    }
}
