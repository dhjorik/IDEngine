using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAD.Doom.Sounds;

namespace WAD.Doom
{
    public class Audio : IElements
    {
        private WADReader _Reader { get; }
        private MSamples _Samples = null;
        private MMusics _Musics = null;

        public Audio(WADReader reader)
        {
            _Reader = reader;

            this.Decode();
        }

        public void Decode()
        {
            _Samples = new MSamples(_Reader);
            _Musics = new MMusics(_Reader);
        }

        public MSamples Samples { get { return _Samples; } }
        public MMusics Musics { get { return _Musics; } }
    }
}
