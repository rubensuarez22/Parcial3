using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLAYGROUND
{
    public class Mtx
    {
        private float[,] matrix;

        public Mtx(float[,] matrix)
        {
            this.matrix = matrix;
        }

        public Vertex Mul(Vertex p)
        {
            Vertex result = new Vertex
            {
                X = matrix[0, 0] * p.X + matrix[0, 1] * p.Y + matrix[0, 2] * p.Z,
                Y = matrix[1, 0] * p.X + matrix[1, 1] * p.Y + matrix[1, 2] * p.Z,
                Z = matrix[2, 0] * p.X + matrix[2, 1] * p.Y + matrix[2, 2] * p.Z
            };
            return result;
        }
    }

}
