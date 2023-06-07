using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD.Doom.Colors
{
    public class MColor : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Index = 0;

        private byte _Red = 0;
        private byte _Green = 0;
        private byte _Blue = 0;

        public MColor(WADReader reader, uint offset, uint index)
        {
            _Reader = reader;
            _Start = offset;
            _Index = index;

            this.Decode();
        }

        public static uint LSize => sizeof(byte) * 3;

        public void Decode()
        {
            _Red = _Reader.ToUInt8(_Start);
            _Green = _Reader.ToUInt8(_Start + 1);
            _Blue = _Reader.ToUInt8(_Start + 2);
        }

        public byte R { get => _Red; }
        public byte G { get => _Green; }
        public byte B { get => _Blue; }
    }

    public class MPalette : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Index = 0;

        private MColor[] _Colors = new MColor[256];

        public MPalette(WADReader reader, uint offset, uint index)
        {
            _Reader = reader;
            _Start = offset;
            _Index = index;

            this.Decode();
        }

        public void Decode()
        {
            _Colors = new MColor[256];
            uint offset = 0;
            for (uint idx = 0; idx < 256; idx++)
            {
                _Colors[idx] = new MColor(_Reader, _Start + offset, idx);
                offset += MColor.LSize;
            }
        }

        public static uint LSize => sizeof(byte) * 3 * 256;

        public MColor[] Colors { get => _Colors; }

    }

    public class MPlayPal : IElements
    {
        private WADReader _Reader { get; }
        private Entry _Entry { get; }
        private List<MPalette> _Palettes = null;

        public MPlayPal(WADReader reader, Entry entry)
        {
            _Entry = entry;
            _Reader = reader;
            this.Decode();
        }
        public void Decode()
        {
            _Palettes = new List<MPalette>();
            uint size = _Entry.Size;
            uint start = _Entry.Offset;
            uint offset = 0;
            uint blocks = size / MPalette.LSize;

            for (uint i = 0; i < blocks; i++)
            {
                MPalette ln = new MPalette(_Reader, start + offset, i);
                _Palettes.Add(ln);
                offset += MPalette.LSize;
            }
        }

        public List<MPalette> Palettes { get => _Palettes; }
    }
}
