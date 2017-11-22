using System.IO;

namespace MoyskleyTech.ImageProcessing.Image
{
    public interface IBitmapCodec
    {
        IBitmapDecoder CheckSignature( string signature,Stream f );
        int SignatureLength { get;}
        void Save(Bitmap bmp,Stream s);
        Bitmap DecodeStream(Stream s);
    }
    public interface IBitmapDecoder
    {
        void SetStream(BufferedStream s);
        bool ReadHeader();
        Bitmap ReadBitmap();
    }
    
}