using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace WAD.Doom.Levels
{
    public class MPoint
    {
        private double _X;
        private double _Y;

        public MPoint(double x, double y)
        {
            _X = x;
            _Y = y;
        }

        public double X { get => _X; }
        public double Y { get => _Y; }

        public override string ToString()
        {
            List<string> log = new List<string>
            {
                "X,Y: [" + _X.ToString("0.##"),
                "," + _Y.ToString("0.##"),
                "]"
            };

            return string.Join("", log.ToArray());
        }
    }

    /// <summary>
    /// Vertex (WAD format)
    /// </summary>
    public class MVertex : IElement
    {
        private WADReader _Reader { get; }
        private uint _Start = 0;
        private uint _Index = 0;

        private short _X;
        private short _Y;

        public MVertex(WADReader reader, uint offset, uint index)
        {
            _Reader = reader;
            _Start = offset;
            _Index = index;

            this.Decode();
        }

        public void Decode()
        {
            _X = _Reader.ToInt16(_Start);
            _Y = _Reader.ToInt16(_Start + 2);
        }

        public static uint LSize => sizeof(short) + sizeof(short);

        public short X { get => _X; }
        public short Y { get => _Y; }

        public override string ToString()
        {
            List<string> log = new List<string>
            {
                "[" + _Index.ToString(),
                "] X,Y: " + _X.ToString(),
                "," + _Y.ToString()
            };

            return string.Join("", log.ToArray());
        }

    }

    /// <summary>
    /// List of vertex (WAD format)
    /// </summary>
    public class MVertexes : IElements
    {
        private WADReader _Reader { get; }
        private Entry _Entry { get; }
        private List<MVertex> _Vertexes = null;

        private short _MinX = 0;
        private short _MinY = 0;
        private short _MaxX = 0;
        private short _MaxY = 0;

        public MVertexes(WADReader reader, Entry entry)
        {
            _Entry = entry;
            _Reader = reader;
            this.Decode();
        }

        public void Decode()
        {
            _Vertexes = new List<MVertex>();
            uint size = _Entry.Size;
            uint start = _Entry.Offset;
            uint offset = 0;
            uint blocks = size / MVertex.LSize;

            for (uint i = 0; i < blocks; i++)
            {
                MVertex v = new MVertex(_Reader, start + offset, i);
                _Vertexes.Add(v);
                offset += MVertex.LSize;
            }
            this.Calc_Bounds();
        }

        private void Calc_Bounds()
        {
            _MinX = _Vertexes.Min(v => v.X);
            _MinY = _Vertexes.Min(v => v.Y);
            _MaxX = _Vertexes.Max(v => v.X);
            _MaxY = _Vertexes.Max(v => v.Y);
        }

        public List<MPoint> Scale_Vertexes(double sx, double sy, double width, double height)
        {
            List<MPoint> ret_val = new List<MPoint>();
            double dx = _MaxX - _MinX;
            double dy = _MaxY - _MinY;
            double scale = dx > dy ? dx : dy;

            foreach (MVertex v in _Vertexes)
            {
                //double x = sx + (width * (v.X - _MinX) / dx);
                //double y = sy + height - (height * (v.Y - _MinY) / dy);
                double x = sx + (width * (v.X - _MinX) / scale);
                double y = sy + height - (height * (v.Y - _MinY) / scale);

                MPoint point = new MPoint(x, y);
                ret_val.Add(point);
            }

            return ret_val;
        }

        public List<MVertex> Vertexes { get => _Vertexes; }

        public override string ToString()
        {
            List<string> log = new List<string>
            {
                "Vertexes: "
            };

            return string.Join("", log.ToArray());
        }

        public string BBoxString()
        {
            List<string> log = new List<string>
            {
                "Vertexes: ",
                "BoundingBox: [" + _MinX.ToString(),
                ", " + _MaxX.ToString(),
                "]: [" + _MinY.ToString(),
                ", " + _MaxY.ToString(),
                "]"
            };

            return string.Join("", log.ToArray());
        }
    }
}
