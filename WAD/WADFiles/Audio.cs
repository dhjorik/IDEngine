using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAD.WADFiles.Sounds;

namespace WAD.WADFiles
{
    public class Audio : IElements
    {
        private WADReader _Reader { get; }
        private MSamples _Samples = null;

        public Audio(WADReader reader)
        {
            _Reader = reader;

            this.Decode();
        }

        public void Decode()
        {
            bool found = false;
            string lumpName = "";

            lumpName = "PNAMES";
            found = _Reader.Entries.HasLumpByName(lumpName);
            if (found)
            {
                Entry entry = _Reader.Entries.LumpByName(lumpName);
                _Samples = new MSamples();
            }
        }
    }
}
