using System;
using System.Collections.Generic;
using System.Text;

namespace Ar.Com.Hjg.Pngcs
{
    interface IBytesConsumer
    {
        int Consume(byte[ ] buffer , int offset , int tofeed);
    }
}
