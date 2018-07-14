using UnityEngine;

namespace UnityGeometryHelper
{
    class QuaternionUtility
    {
        public static Quaternion Euler(float x, float y, float z)
        {
            Quaternion res = new Quaternion()
            {
                w = cos(x / 2f) * cos(y / 2f) * cos(z / 2f) + sin(x / 2f) * sin(y / 2f) * sin(z / 2f),
                x = sin(x / 2f) * cos(y / 2f) * cos(z / 2f) - cos(x / 2f) * sin(y / 2f) * sin(z / 2f),
                y = cos(x / 2f) * sin(y / 2f) * cos(z / 2f) + sin(x / 2f) * cos(y / 2f) * sin(z / 2f),
                z = cos(x / 2f) * cos(y / 2f) * sin(z / 2f) - sin(x / 2f) * sin(y / 2f) * cos(z / 2f),
            };

            return res;
        }

        public static Quaternion Axis(Vector3 axis, float angle)
        {
            var s = sin(angle / 2f);
            var c = cos(angle / 2f);

            Quaternion res = new Quaternion()
            {
                w = c,
                x = axis.x * s,
                y = axis.y * s,
                z = axis.z * s,
            };

            return res;
        }

        private static float cos(float a)
        {
            return Mathf.Cos((a / 180) * Mathf.PI);
        }

        private static float sin(float a)
        {
            return Mathf.Sin((a / 180) * Mathf.PI);
        }
    }
}
