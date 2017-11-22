using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ar.Com.Hjg.Pngcs
{
    class ChunkReader
    {
        protected ChunkReaderMode mode;
	    private ChunkRaw chunkRaw;

	    private bool crcCheck; // by default, this is false for SKIP, true elsewhere
	    protected int read = 0;
    	
        public ChunkReader(int clen, String id, long offsetInPng, ChunkReaderMode mode)
        {
            if (id.Length != 4 || clen < 0)
                throw new PngjExceptionInternal("Bad chunk paramenters: " + mode);
            this.mode = mode;
            chunkRaw = new ChunkRaw(clen, id, mode == ChunkReaderMode.BUFFER);
            chunkRaw.setOffset(offsetInPng);
            this.crcCheck = mode == ChunkReaderMode.SKIP ? false : true; // can be changed with setter
        }
    }
}
