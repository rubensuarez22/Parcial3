using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLAYGROUND
{
    public class Transform
    {
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }

        public Transform()
        {
            RotationX = 0f;
            RotationY = 0f;
            RotationZ = 0f;
        }

        public void Rotate(float x, float y, float z)
        {
            RotationX += x;
            RotationY += y;
            RotationZ += z;
        }
    }
}
