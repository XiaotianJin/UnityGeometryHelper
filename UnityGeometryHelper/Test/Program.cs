using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                new Vector3(0,0,0),
                new Vector3(0,0,5),
                new Vector3(2,0,5),
                new Vector3(2,0,3),
                new Vector3(5,0,3),
                new Vector3(5,0,5),
                new Vector3(7,0,5),
                new Vector3(7,0,0),
                new Vector3(0,0,0),
            };

            Vector3 checkP = new Vector3(3, 0, 5);
            Console.WriteLine(Geometric.IsPointInArea(points.ToArray(), checkP));

            Console.WriteLine(Geometric.Meet(points[2], points[3], checkP, new Vector3(0, 0, 5)));

            checkP = new Vector3(1,0,3);
            Console.WriteLine(Geometric.IsPointInArea(points.ToArray(), checkP));

        }
    }
}
