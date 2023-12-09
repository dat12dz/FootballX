using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Assets.Script.Utlis
{
    internal class MathHelper
    {
        public static Vector3 FindIntersectionPoint(Vector3 line1Origin, Vector3 line1Direction, Vector3 line2Origin, Vector3 line2Direction)
        {
            // Direction vectors of the lines
            Vector3 dir1 = line1Direction.normalized;
            Vector3 dir2 = line2Direction.normalized;

            // Cross product of the direction vectors
            Vector3 cross = Vector3.Cross(dir1, dir2);

            // Check if the lines are parallel (cross product magnitude close to zero)
            if (cross.sqrMagnitude < 0.0001f)
            {
                // Lines are parallel, return some default value or handle as needed
                Debug.LogError("Lines are parallel, no intersection point.");
                return Vector3.zero;
            }

            // Vector from line1's origin to line2's origin
            Vector3 startVector = line2Origin - line1Origin;

            // Calculate the scaling factors for the intersection point along each line
            float s = Vector3.Dot(cross, startVector) / cross.sqrMagnitude;

            // Calculate the intersection point
            Vector3 intersectionPoint = line1Origin + s * dir1;

            return intersectionPoint;
        }

      public static Vector2? FindIntersectionPoint(Vector2 start1, Vector2 dir1, Vector2 start2, Vector2 dir2)
        {
            // Calculate the denominator for solving the parametric equations
            float denominator = dir1.x * dir2.y - dir1.y * dir2.x;

            // Check if the vectors are parallel (denominator close to zero)
            if (Mathf.Approximately(denominator, 0f))
            {
                // Vectors are parallel, return some default value or handle as needed
                Debug.LogError("Vectors are parallel, no intersection point.");
                return null;
            }

            // Vector from start2 to start1
            Vector2 startVector = start1 - start2;

            // Calculate the scaling factors for the intersection point along each vector
            float t = (dir2.y * startVector.x - dir2.x * startVector.y) / denominator;
            float s = (dir1.y * startVector.x - dir1.x * startVector.y) / denominator;

            // Calculate the intersection point
            Vector2 intersectionPoint = start1 + t * dir1;

            return intersectionPoint;
        }
        public static float Angle(Vector3 from, Vector3 to)
        {
            float num = (float)Math.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            if (num < 1E-15f)
            {
                return 0f;
            }
            float num2 = (Vector3.Dot(from, to) / num);
            return (float)Math.Acos(num2) * 57.29578f;
        }
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
