using System;
using UnityEngine;
using UnityGeometryHelper;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Vector3 a = new Vector3(-2000, 0, -3000);
            Vector3 b = new Vector3(-2000, 0, -6000);
            Vector3 p = new Vector3(-2000, 0, -2999);

            float thick = 240;

            Console.WriteLine(Geometric.IsPointAlmostOnSegment(a, b, thick, p));
        }
    }
}
