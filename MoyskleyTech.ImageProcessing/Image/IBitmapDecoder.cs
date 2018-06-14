using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Base interface for bitmap Codec
    /// </summary>
    public interface IBitmapCodec
    {
        /// <summary>
        /// Check if specified signature match the codec's
        /// </summary>
        /// <param name="signature">Signature from stream</param>
        /// <param name="f">Stream</param>
        /// <returns></returns>
        IBitmapDecoder CheckSignature( string signature,Stream f );
        /// <summary>
        /// Length of signature to check
        /// </summary>
        int SignatureLength { get;}
        /// <summary>
        /// Save a bitmap using the codec
        /// </summary>
        /// <param name="bmp">Bitmap to save</param>
        /// <param name="s">Destination</param>
        void Save<T>(ImageProxy<T> bmp , Stream s)
            where T : unmanaged;
        /// <summary>
        /// Create a bitmap from a stream using the codec
        /// </summary>
        /// <param name="s">Source</param>
        /// <returns></returns>
        Image<Pixel> DecodeStream(Stream s);
        IEnumerable<ColorPoint<T>> ReadData<T>(Stream s)
            where T : unmanaged;
        Image<T> ReadImage<T>(Stream s) where T : unmanaged;
    }
    /// <summary>
    /// Base interface for decoding, should be used inside of Codec only
    /// </summary>
    public interface IBitmapDecoder
    {
        /// <summary>
        /// Set the stream to the decoder after the signature check
        /// </summary>
        /// <param name="s"></param>
        void SetStream(BufferedStream s);
        /// <summary>
        /// Prepare the header
        /// </summary>
        /// <returns></returns>
        bool ReadHeader();
        /// <summary>
        /// Read the data
        /// </summary>
        /// <returns></returns>
        Image<Pixel> ReadBitmap();

        IEnumerable<ColorPoint<T>> ReadData<T>()
            where T : unmanaged;
        int Height { get; }
        int Width { get; }
    }
    
}