using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpireSearchPDFCoordinates
{

    public class Annotation
    {
        public int Page { get; set; }
        public string Name { get; set; } = string.Empty;
        public Coordinate LLX { get; set; }
        public Coordinate LLY { get; set; }
        public Coordinate URX { get; set; }
        public Coordinate URY { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
    }

    public class Coordinate
    {

        public int IntValue { get; set; }
        public long LongValue { get; set; }
        public double DoubleValue { get; set; }
        public float FloatValue { get; set; }
        public int Length { get; set; }
        public int Type { get; set; }
        public object IndRef { get; set; }
    }

    public class Annotation_Spire
    {
        public int Page { get; set; }
        public string Name { get; set; } = string.Empty;
        public float X { get; set; }
        public float Y { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
    }
}
