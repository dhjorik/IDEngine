using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD.Doom.Bitmaps
{
    public class MSprite : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Index = 0;

        private string _Name = "";
        private MPicture _Picture = null;

        public MSprite(WADReader reader, uint index)
        {
            _Reader = reader;
            _Index = index;

            this.Decode();
        }

        public void Decode()
        {
            Entry entry = _Reader.Entries.LumpByIndex(this._Index);
            _Name = entry.Name;
            _Picture = new MPicture(_Reader, _Name);
        }

        public string Name { get { return _Name; } }
        public MPicture Picture { get { return _Picture; } }

        public override string ToString()
        {
            return _Name;
        }
    }

    public class MSprites : IElements
    {
        private static readonly string SUFFIX_START = "_START";
        private static readonly string SUFFIX_END = "_END";

        private static readonly string PREFIX = "S";

        private WADReader _Reader { get; }

        private List<MSprite> _Sprites = null;

        public MSprites(WADReader reader)
        {
            _Reader = reader;
            this.Decode();
        }

        public void Decode()
        {
            string name = "";
            _Sprites = new List<MSprite>();

            name = string.Concat(PREFIX, SUFFIX_START);
            uint start_all = _Reader.Entries.LumpIndexByName(name);
            name = string.Concat(PREFIX, SUFFIX_END);
            uint end_all = _Reader.Entries.LumpIndexByName(name);

            for (uint i = start_all + 1; i < end_all; i++)
            {
                Entry entry = _Reader.Entries.LumpByIndex(i);
                if (entry.Size > 0)
                {
                    MSprite ln = new MSprite(_Reader, i);
                    _Sprites.Add(ln);
                }
            }
        }

        public List<MSprite> Sprites { get => _Sprites; }

        public MSprite SpriteByName(string name)
        {
            MSprite value = _Sprites.Find(x => x.Name == name);
            return value;
        }
    }
}
