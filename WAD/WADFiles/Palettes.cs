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

        public List<MColor> GetPalette(byte palette, byte light)
        {
            byte real = 0;
            MColor rcolor = null;
            List<MColor> result = new List<MColor>();
            MColorMap cmap = ColorMaps.ColorMaps[light];
            MPalette ppal = PlayPal.Palettes[palette];
            for (int color = 0; color < 256; color++)
            {
                real = cmap.Colors[color];
                rcolor = ppal.Colors[real];
                result.Add(rcolor);
            }
            return result;
        }

        public MColor getColor(byte color, byte palette, byte light)
        {
            byte real = ColorMaps.ColorMaps[light].Colors[color];
            MColor rcolor = PlayPal.Palettes[palette].Colors[real];
            return rcolor;
        }
    }
}
