using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoyskleyTech.ImageProcessing.Image;

namespace MoyskleyTech.ImageProcessing.Form
{
    public class BaseControl:IControl
    {
        public int X { get; set; }
        public int Y { get; set; }
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }
        public bool Visible { get; set; } = true;
        public bool DisplayChildren { get; set; } = true;

        private List<IControl> childs=new List<IControl>();
        public IEnumerable<IControl> Childrens => childs;
        public IControl Parent { get; set; }
        public IRenderer Renderer { get; set; }
        public virtual int ChildrenOffsetX { get; set; }
        public virtual int ChildrenOffsetY { get; set; }

        public void AddControl(IControl child)
        {
            if ( child.Parent != null )
                child.Parent.RemoveControl(child);
            childs.Add(child);
            child.Parent = this;
        }
        public void RemoveControl(IControl child)
        {
            childs.Remove(child);
            child.Parent = null;
        }
        public BaseControl AddRange(params IControl[ ] childrens)
        {
            foreach ( var c in childrens )
                AddControl(c); 
            return this;
        }
        
    }
}
