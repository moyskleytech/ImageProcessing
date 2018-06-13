using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Class for all bitmap creation from stream
    /// </summary>
    public class BitmapFactory
    {
        private List<IBitmapCodec> Codecs=new List<IBitmapCodec>();
        private static List<IBitmapCodec> RegisteredCodecs=new List<IBitmapCodec>();
        /// <summary>
        /// Create basic factory
        /// </summary>
        public BitmapFactory()
        {
            Codecs.AddRange(RegisteredCodecs);
        }
        static BitmapFactory()
        {
            RegisteredCodecs.Add(new BitmapCodec());
        }
      
        private IBitmapDecoder FindDecoder(Stream file)
        {

            int maxlen = 0;

            // iterate through list of registered codecs
            for (int  i = 0; i < Codecs.Count; i++ )
            {
                int len = Codecs[i].SignatureLength;
                maxlen = System.Math.Max(maxlen , len);
            }

            // Open the file
            var f = file;
                       
            // read the file signature
            string signature="";
            for ( var i = 0; i < maxlen; i++ )
            {
                signature += ( char ) f.ReadByte();
            }
            // compare signature against all decoders
            for (int i = 0; i < Codecs.Count; i++ )
            {
                var decoder= Codecs[i].CheckSignature(signature,f);
                if ( decoder != null )
                    return decoder;
            }

            // If no decoder was found, return base type
            return null;
        }
        /// <summary>
        /// Allow registering systemwide codec
        /// </summary>
        /// <param name="codec"></param>
        public static void RegisterCodec(IBitmapCodec codec)
        {
            if ( !RegisteredCodecs.Contains(codec) )
                RegisteredCodecs.Add(codec);
        }
        /// <summary>
        /// Allow registering of codec in this instance only
        /// </summary>
        /// <param name="codec"></param>
        public void AddCodec(IBitmapCodec codec)
        {
            if(!Codecs.Contains(codec))
                Codecs.Add(codec);
        }
        /// <summary>
        /// Remove codec from current instance
        /// </summary>
        /// <param name="codec"></param>
        public void RemoveCodec(IBitmapCodec codec)
        {
            Type t = codec.GetType();
            Codecs.RemoveAll((x) => x.GetType() == t);
            Codecs.Remove(codec);
        }
        /// <summary>
        /// Allow Decoding of stream
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Image<Pixel> Decode(Stream s)
        {
            var decoder= FindDecoder(s);
            if ( decoder == null )
                throw new InvalidDataException("No Codec found for specified Stream");
        
            if(!decoder.ReadHeader())
                throw new InvalidDataException("Header was in wrong format");
            return decoder.ReadBitmap();
        }
    }
}
