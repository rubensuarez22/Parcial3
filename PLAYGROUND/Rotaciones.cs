using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLAYGROUND
{
    public static class Rotaciones
    {
        public static Vertex Rot(float angle, Vertex p, char axis)
        {
            float[,] axisRotationMatrix;
            Mtx rotationMatrix;
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            switch (axis)
            {
                case 'x':
                case 'X':
                    axisRotationMatrix = new float[,] {
                    { 1, 0, 0 },
                    { 0, cos, -sin },
                    { 0, sin, cos }
                };
                    break;
                case 'y':
                case 'Y':
                    axisRotationMatrix = new float[,] {
                    { cos, 0, sin },
                    { 0, 1, 0 },
                    { -sin, 0, cos }
                };
                    break;
                case 'z':
                case 'Z':
                    axisRotationMatrix = new float[,] {
                    { cos, -sin, 0 },
                    { sin, cos, 0 },
                    { 0, 0, 1 }
                };
                    break;
                default:
                    throw new ArgumentException("Axis must be 'x', 'y', or 'z'.", nameof(axis));
            }

            rotationMatrix = new Mtx(axisRotationMatrix);

            return rotationMatrix.Mul(p);
        }
    }
}
