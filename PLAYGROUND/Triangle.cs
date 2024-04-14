using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLAYGROUND
{
    public class Triangle
    {
        public Vertex A { get; set; }
        public Vertex B { get; set; }
        public Vertex C { get; set; }

        public Triangle(Vertex a, Vertex b, Vertex c)
        {
            A = a;
            B = b;
            C = c;
        }
    }
}
