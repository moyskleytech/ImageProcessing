namespace MoyskleyTech.ImageProcessing.Image
{
    public enum RotateFlipType
    {
       

        FlipNone = 0,
        FlipX = 0x40,
        FlipY = 0x80,
        FlipXY = 0xC0,
        /*
00000000 = FLIPNONE
01000000 = FLIPX
10000000 = FLIPY
11000000 = FLIPXY*/
        RotateNone=0,
        Rotate90=1,
        Rotate180=2,
        Rotate270=3,
        /*
00000000 = RotateNone
00000001 = Rotate90
00000010 = Rotate180
00000011 = Rotate270*/


        Rotate180FlipNone=  0x02,
        Rotate180FlipX=     0x42,
        Rotate180FlipXY=    0xC2,
        Rotate180FlipY=     0x82,
        Rotate270FlipNone=  0x03,
        Rotate270FlipX=     0x43,
        Rotate270FlipXY=    0xC3,
        Rotate270FlipY=     0x83,
        Rotate90FlipNone=   0x01,
        Rotate90FlipX=      0x41,
        Rotate90FlipXY=     0xC1,
        Rotate90FlipY=      0x81,
        RotateNoneFlipNone= 0x00,
        RotateNoneFlipX=    0x40,
        RotateNoneFlipXY=   0xC0,
        RotateNoneFlipY=    0x80,
    }
}
 