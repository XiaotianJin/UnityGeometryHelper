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
            Vector3 a = new Vector3(0,0,0);
            Vector3 b = new Vector3(0,0,2);
            Vector3 c = new Vector3(0,0,1);
            Vector3 d = new Vector3(1,0,1);
            Console.WriteLine(Geometric.Meet(a,b,c,d));
        }
    }
}
