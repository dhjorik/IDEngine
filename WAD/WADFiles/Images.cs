using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAD.WADFiles.Bitmaps;
using WAD.WADFiles.Colors;

namespace WAD.WADFiles
{
    public class Images : IElements
    {
        private WADReader _Reader { get; }

        private MPatches _Patches = null;
        private MTextures _Textures = null;

        private MFlats _Flats = null;
        private MSprites _Sprites = null;
        private MParts _Parts = null;

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
                _Patches = new MPatches(_Reader, entry);
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
            _Parts = new MParts(_Reader);
        }

        public MPatches Patches { get => _Patches; }
        public MTextures Textures { get => _Textures; }

        public MFlats Flats { get => _Flats; }
        public MSprites Sprites { get => _Sprites; }
        public MParts PatchesParts { get => _Parts; } 
    }
}
