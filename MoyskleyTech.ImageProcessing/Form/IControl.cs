using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Form
{
    public interface IControl
    {
        int X { get; set; }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        int ChildrenOffsetX { get; set; }
        int ChildrenOffsetY { get; set; }
        IEnumerable<IControl> Childrens { get; }
        void AddControl(IControl child);
        void RemoveControl(IControl child);
        IRenderer Renderer { get; set; }
        IControl Parent { get; set; }
        bool DisplayChildren { get; set; }
        bool Visible { get; set; }
    }
    public interface ICheckHoverController
    {
        bool IsHover { get; set; }
        bool IsPressed { get; set; }
        bool IsPressedRight { get; set; }
        void RaiseHover(Point location);
        void RaiseClick(Point location , bool left);
        void RaiseCursorDown(Point location , bool left);
        void RaiseCursorUp(Point location , bool left);
        event EventHandler<Point> CursorHover;
        event EventHandler<ClickEvent> CursorClick;
        event EventHandler<ClickEvent> CursorDown;
        event EventHandler<ClickEvent> CursorUp;
    }

    public class ClickEvent
    {
        public Point Location { get; internal set; }
        public bool IsLeftButton { get; internal set; }
    }
}
