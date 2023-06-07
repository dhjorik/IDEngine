using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAD.Doom.Levels;

namespace WAD.Doom.Sounds
{
    public class MMusic : IElement
    {
        private WADReader _Reader { get; }
        private Entry _Entry { get; }

        private string _Sig = null;
        private ushort _Length = 0;
        private ushort _Offset = 0;
        private ushort _NumChannel1 = 0;
        private ushort _NumChannel2 = 0;
        private ushort _NumInstruments = 0;
        private ushort _Empty = 0;
        private ushort[] _Instruments = null;

        public MMusic(WADReader reader, Entry entry)
        {
            _Entry = entry;
            _Reader = reader;
            this.Decode();
        }

        public void Decode()
        {
            _Sig = _Reader.ToString(_Entry.Offset, 4);
            _Length = _Reader.ToUInt16(_Entry.Offset + 4);
            _Offset = _Reader.ToUInt16(_Entry.Offset + 6);
            _NumChannel1 = _Reader.ToUInt16(_Entry.Offset + 8);
            _NumChannel2 = _Reader.ToUInt16(_Entry.Offset + 10);
            _NumInstruments = _Reader.ToUInt16(_Entry.Offset + 12);
            _Empty = _Reader.ToUInt16(_Entry.Offset + 12);

            _Instruments = new ushort[_Length];
            for (uint i = 0; i < _Length; i++)
            {
                _Instruments[i] = _Reader.ToUInt16(_Entry.Offset + 16 + i * 2);
            }
        }

        public override string ToString()
        {
            StringBuilder sb=new StringBuilder();
            
            string name = _Entry.Name.ToUpper();
            sb.Append(name);
            sb.Append(": ");
            sb.Append(_Sig);
            sb.Append(" - ");
            sb.Append(_Offset);
            sb.Append(" - ");
            sb.Append(_Length);
            sb.Append(" - ");
            sb.Append(_Entry.Size);
            sb.Append(" - ");
            sb.AppendLine(_NumInstruments.ToString());
            return sb.ToString();
        }
    }

    public class MMusics : IElements
    {
        private WADReader _Reader { get; }
        private List<MMusic> _Musics = null;

        public MMusics(WADReader reader)
        {
            _Reader = reader;
            this.Decode();
        }
        public void Decode()
        {
            _Musics = new List<MMusic>();
            foreach (string sname in Infos.BGMNames.Values)
            {
                string lumpo_name = "D_" + sname.ToUpper();
                if (_Reader.Entries.HasLumpByName(lumpo_name))
                {
                    Entry entry = _Reader.Entries.LumpByName(lumpo_name);
                    MMusic mMusic = new MMusic(_Reader, entry);
                    _Musics.Add(mMusic);
                }
            }
        }

        public List<MMusic> Musics { get { return _Musics; } }
    }
}
