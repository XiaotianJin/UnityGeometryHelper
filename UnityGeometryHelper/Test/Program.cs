using System;
using UnityEngine;
using UnityGeometryHelper;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            float z = -3.728099f;
            Vector3 a = new Vector3(-0.52486f, 0, z);
            Vector3 b = new Vector3(-4.176628f, 0, z);
            Vector3 p = new Vector3(0.433864f, 0, z);

            float thick = 0.24f;

            Console.WriteLine(Geometric.AlmostOnLine(a, b, thick, p));
        }
    }
}
