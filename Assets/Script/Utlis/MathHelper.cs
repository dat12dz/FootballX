using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Utlis
{
    internal class MathHelper
    {

        public static float DistanceNoSqrt(Vector3 a,Vector3 b)
        {
            var diffx = a.x - b.x;
            var diffy =a .y - b.y;
            var diffz = a .z - b.z;
            return diffx * diffx + diffy * diffy + diffz * diffz;
        }
        public static float DistanceNoSqrt(Vector2 a, Vector2 b)
        {
            var diffx = a.x - b.x;
            var diffy = a.y - b.y;
            return diffx * diffx + diffy * diffy;
        }

        public static float Power2(float a)
        {
            return a * a;
        }
       public static unsafe float FastSqrt(float number)
        {
            long i;
            float x2, y;
            const float threehalfs = 1.5F;

            x2 = number * 0.5F;
            y = number;
            i = *(long*)&y;                       // evil floating point bit level hacking
            i = 0x5f3759df - (i >> 1);               // what the fuck?
            y = *(float*)&i;
            y = y * (threehalfs - (x2 * y * y));   // 1st iteration
                                                   // y  = y * ( threehalfs - ( x2 * y * y ) );   // 2nd iteration, this can be removed

            return 1/ y;
        }
    }
}
