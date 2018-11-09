using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.Mathematics;

namespace MoyskleyTech.ImageProcessing.Drawing3D
{
    public interface IObject3D
    {
        void Render(IGraphics3D viewport3D);
    }
}