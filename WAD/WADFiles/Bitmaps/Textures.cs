﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace WAD.WADFiles.Bitmaps
{
    public class MTexPatch : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Offset = 0;

        private short _OriginX = 0;
        private short _OriginY = 0;
        private ushort _PatchIndex = 0;
        private short _StepDir = 0;
        private ushort _ColorMap = 0;

        public MTexPatch(WADReader reader, uint start, uint offset)
        {
            _Reader = reader;
            _Start = start;
            _Offset = offset;

            this.Decode();
        }

        public void Decode()
        {
            _OriginX = _Reader.ToInt16(_Start);
            _OriginY = _Reader.ToInt16(_Start + 2);
            _PatchIndex = _Reader.ToUInt16(_Start + 4);
            _StepDir = _Reader.ToInt16(_Start + 6);
            _ColorMap = _Reader.ToUInt16(_Start + 8);
        }

        public static uint LSize => sizeof(short) + sizeof(short) + sizeof(ushort) + sizeof(short) + sizeof(ushort);
    }

    public class MTexture : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Offset = 0;

        private string _Name = "";
        private uint _Masked = 0;
        private ushort _Width = 0;
        private ushort _Height = 0;
        private uint _Directory = 0;
        private ushort _NumPatches = 0;
        private List<MTexPatch> _Patches = null;

        public MTexture(WADReader reader, uint start, uint offset)
        {
            _Reader = reader;
            _Start = start;
            _Offset = offset;

            this.Decode();
        }

        public void Decode()
        {
            uint real_offset = _Reader.ToUInt32(_Start + _Offset);
            uint real_start = _Start + real_offset;

            _Name = _Reader.ToString(real_start, 8);
            _Masked = _Reader.ToUInt32(real_start + 8);
            _Width = _Reader.ToUInt16(real_start + 12);
            _Height = _Reader.ToUInt16(real_start + 14);
            _Directory = _Reader.ToUInt32(real_start + 16);
            _NumPatches = _Reader.ToUInt16(real_start + 20);


            _Patches = new List<MTexPatch>();
            uint offset = 22;

            for (uint i = 0; i < _NumPatches; i++)
            {
                MTexPatch ln = new MTexPatch(_Reader, real_start, offset);
                _Patches.Add(ln);
                offset += MTexPatch.LSize;
            }
        }

        public static uint LSize => 4; // Known size is offset

        public string Name { get => _Name; }

        public override string ToString()
        {
            return _Name;
        }
    }

    public class MTextures : IElements
    {
        private WADReader _Reader { get; }
        private Entry _Entry { get; }
        private List<MTexture> _Textures = null;

        public MTextures(WADReader reader, Entry entry)
        {
            _Entry = entry;
            _Reader = reader;
            this.Decode();
        }
        public void Decode()
        {
            _Textures = new List<MTexture>();
            uint size = _Entry.Size;
            uint start = _Entry.Offset;
            uint offset = 4;
            uint blocks = size / MTexture.LSize;
            uint count = _Reader.ToUInt32(_Entry.Offset);

#if DEBUG
            Console.Write("Textures: ");
            Console.Write(start);
            Console.Write(" - ");
            Console.Write(size);
            Console.Write(" - ");
            Console.Write(blocks);
            Console.Write(" - ");
            Console.Write(count);
            Console.WriteLine("");
#endif
            for (uint i = 0; i < count; i++)
            {
                MTexture ln = new MTexture(_Reader, start, offset);
                _Textures.Add(ln);
                offset += MTexture.LSize;
            }
        }

        public List<MTexture> Textures { get => _Textures; }

    }
}
