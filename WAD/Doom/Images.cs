using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAD.Doom.Bitmaps;
using WAD.Doom.Colors;

namespace WAD.Doom
{
    public class Images : IElements
    {
        private WADReader _Reader { get; }

        private MWalls _Walls = null;
        private MTextures _Textures = null;

        private MFlats _Flats = null;
        private MSprites _Sprites = null;
        private MPatches _Patches = null;

        public Images(WADReader reader)
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
                _Walls = new MWalls(_Reader, entry);
            }

            lumpName = "TEXTURE1";
            found = _Reader.Entries.HasLumpByName(lumpName);
            if (found)
            {
                Entry entry = _Reader.Entries.LumpByName(lumpName);
                _Textures = new MTextures(_Reader, entry);
            }

            _Flats = new MFlats(_Reader);
            _Sprites = new MSprites(_Reader);
            _Patches = new MPatches(_Reader);
        }

        public MWalls Walls { get => _Walls; }
        public MTextures Textures { get => _Textures; }

        public MFlats Flats { get => _Flats; }
        public MSprites Sprites { get => _Sprites; }
        public MPatches Patches { get => _Patches; }


        public byte[] RenderPicture(MPicture pic, MColorMap cmap, MPalette palette)
        {
            byte[] render = new byte[pic.Width * pic.Height * 4];

            return render;
        }
    }
}
