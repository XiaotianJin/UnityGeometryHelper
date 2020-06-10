using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGeometryHelper;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Vector3> points = new List<Vector3>()
            {
                new Vector3(-8623,0,-11289),
                new Vector3(11808,0,-11289),
                new Vector3(11808,0,5688),
                new Vector3(-8623,0,5688),
                new Vector3(-8623,0,5688),
            };

            Vector3 checkP = new Vector3(-5836,600,1031);

            Console.WriteLine(Geometric.IsPointInArea(points.ToArray(), checkP));
        }
    }
}
