using System;
using System.Collections.Generic;

namespace dtgw
{
    public class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Value { get; set; }
        public Cell Parent { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public int F { get { return G + H; } }


    }
}
