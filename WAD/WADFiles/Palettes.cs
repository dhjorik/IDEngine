using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAD.WADFiles.Colors;

namespace WAD.WADFiles
{
    public class Palettes : IElements
    {
        private WADReader _Reader { get; }

        private MPlayPal _PlayPal = null;
        private MColorMaps _ColorMaps = null;

        public Palettes(WADReader reader)
        {
            _Reader = reader;

            this.Decode();
        }

        public void Decode()
        {
            bool found = false;
            string lumpName = "";

            lumpName = "PLAYPAL";
            found = _Reader.Entries.HasLumpByName(lumpName);
            if (found)
            {
                Entry entry = _Reader.Entries.LumpByName(lumpName);
                _PlayPal = new MPlayPal(_Reader, entry);
            }

            lumpName = "COLORMAP";
            found = _Reader.Entries.HasLumpByName(lumpName);
            if (found)
            {
                Entry entry = _Reader.Entries.LumpByName(lumpName);
                _ColorMaps = new MColorMaps(_Reader, entry);
            }
        }

        public MPlayPal PlayPal { get => _PlayPal; }
        public MColorMaps ColorMaps { get => _ColorMaps; }
    }
}
