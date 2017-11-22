using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LMST.Sportif.DLL.Windows.Controls
{
    /// <summary>
    /// Inherits from PictureBox; adds Interpolation Mode Setting
    /// </summary>
    public class PictureBoxWithInterpolationMode : PictureBox
    {
        InterpolationMode mdoe;
        public InterpolationMode InterpolationMode { get { return mdoe; } set { mdoe = value;Invalidate(); } }

        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            paintEventArgs.Graphics.InterpolationMode = mdoe;
            base.OnPaint(paintEventArgs);
        }
    }
}
