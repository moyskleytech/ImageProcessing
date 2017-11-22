using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace LMST.Sportif.DLL.Windows.Controls
{
    public class VerticalLineGradient:Control
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Height <= 0)
                return;
            if (this.Width <= 0)
                return;
            Point top = new Point(this.Width / 2, 0);
            Point mid = new Point(this.Width / 2, this.Height / 2);
            Point bot = new Point(this.Width / 2, this.Height);
            LinearGradientBrush l = new LinearGradientBrush(top, mid, BackColor, ForeColor);
            LinearGradientBrush l2 = new LinearGradientBrush(bot, mid, BackColor, ForeColor);
            Pen p = new Pen(l);
            Pen p2 = new Pen(l2);
            e.Graphics.DrawLine(p, top, mid);
            e.Graphics.DrawLine(p2, bot, mid);
            base.OnPaint(e);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
    }
}
