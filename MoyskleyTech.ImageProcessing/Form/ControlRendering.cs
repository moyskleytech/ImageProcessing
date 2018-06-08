using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoyskleyTech.ImageProcessing.Image;

namespace MoyskleyTech.ImageProcessing.Form
{
    public class ControlRendering : BaseControl, IRenderer
    {
        public virtual void Render(Graphics<Pixel> g , int x , int y , int w , int h)
        {
            if ( Visible && DisplayChildren )
                foreach ( var c in Childrens )
                    DrawControl(g , c);
        }
        private static void DrawControl(Graphics<Pixel> g , IControl c , int offx = 0 , int offy = 0)
        {
            if ( c.Visible )
                c.Renderer?.Render(g , c.X + offx , c.Y + offy , c.Width , c.Height);
            if (c.Visible && c.DisplayChildren )
                foreach ( var ch in c.Childrens )
                {
                    DrawControl(g , ch , c.X + offx + c.ChildrenOffsetX , c.Y + offy + c.ChildrenOffsetY);
                }
        }
        public void HandleCursorUpAt(Point point , bool left)
        {
            foreach ( var c in this.Childrens )
            {
                CheckControlForMouseUp(point , left , c);
            }
        }
        private static void CheckControlForMouseUp(Point p , bool left , IControl c , int offx = 0 , int offy = 0)
        {
            if ( !c.Visible )
                return;
            if ( c.Renderer is ICheckHoverController renderer )
            {
                if ( renderer.IsPressed )
                {
                    renderer.IsPressed = false;
                    p = new Point(p.X - offx - c.X , p.Y - offy - c.Y);
                    renderer.RaiseClick(p , left);
                    renderer.RaiseCursorUp(p , left);
                }
            }
            else if ( c is ICheckHoverController ctrl )
            {
                if ( ctrl.IsPressed )
                {
                    ctrl.IsPressed = false;
                    p = new Point(p.X - offx - c.X , p.Y - offy - c.Y);
                    ctrl.RaiseClick(p , left);
                    ctrl.RaiseCursorUp(p , left);
                }
            }
            if ( c.DisplayChildren )
                foreach ( var ch in c.Childrens.ToArray() )
                    CheckControlForMouseUp(p , left , ch , offx + c.X + c.ChildrenOffsetX , offy + c.Y + c.ChildrenOffsetY);
        }
        public void HandleCursorDownAt(Point point , bool left)
        {
            foreach ( var c in this.Childrens )
            {
                CheckControlForMouseDown(point , left , c);
            }
        }
        private static void CheckControlForMouseDown(Point p , bool left , IControl c , int offx = 0 , int offy = 0)
        {
            if ( !c.Visible )
                return;
            if ( c.Renderer is ICheckHoverController renderer )
            {
                if ( renderer.IsHover )
                {
                    renderer.IsPressed = true;
                    p = new Point(p.X - offx - c.X , p.Y - offy - c.Y);
                    renderer.RaiseCursorDown(p , left);
                }
            }
            else if ( c is ICheckHoverController ctrl )
            {
                if ( ctrl.IsHover )
                {
                    ctrl.IsPressed = true;
                    p = new Point(p.X - offx - c.X , p.Y - offy - c.Y);
                    ctrl.RaiseCursorDown(p , left);
                }
            }
            if ( c.DisplayChildren )
                foreach ( var ch in c.Childrens.ToArray() )
                    CheckControlForMouseDown(p , left , ch , offx + c.X + c.ChildrenOffsetX , offy + c.Y + c.ChildrenOffsetY);
        }
        public void HandleCursorMove(Point point)
        {
            IControl hasMouseHover = null;
            Point relativeToElement = point;
            foreach ( var c in this.Childrens )
            {
                CheckControlForMouseHover(point , c , ref hasMouseHover , ref relativeToElement , 0 , 0);
            }
            if ( hasMouseHover != null )
            {
                if ( hasMouseHover is ICheckHoverController ctrl )
                {
                    ctrl.IsHover = true;
                    ctrl.RaiseHover(relativeToElement);
                }
                else if ( hasMouseHover.Renderer is ICheckHoverController renderer )
                {
                    renderer.IsHover = true;
                    renderer.RaiseHover(relativeToElement);
                }
            }
        }
        private static void CheckControlForMouseHover(Point p , IControl c , ref IControl hasMouseHover , ref Point relativeToElement , int offx = 0 , int offy = 0)
        {
            if ( !c.Visible )
                return;
            if ( c.Renderer is ICheckHoverController renderer )
            {
                renderer.IsHover = false;
                if ( p.X - offx > c.X && p.X - offx < c.X + c.Width && p.Y - offy > c.Y && p.Y - offy < c.Y + c.Height )
                {
                    hasMouseHover = c;
                    relativeToElement = new Point(p.X - offx - c.X , p.Y - offy - c.Y);
                }
            }
            else if ( c is ICheckHoverController ctrl )
            {
                ctrl.IsHover = false;
                if ( p.X - offx > c.X && p.X - offx < c.X + c.Width && p.Y - offy > c.Y && p.Y - offy < c.Y + c.Height )
                {
                    hasMouseHover = c;
                    relativeToElement = new Point(p.X - offx - c.X , p.Y - offy - c.Y);
                }
            }
            if ( c.DisplayChildren )
                foreach ( var ch in c.Childrens.ToArray() )
                    CheckControlForMouseHover(p , ch , ref hasMouseHover , ref relativeToElement , offx + c.X + c.ChildrenOffsetX , offy + c.Y + c.ChildrenOffsetY);
        }
    }
}
