using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityGeometryHelper
{

    internal class ConvexAogrithm
    {
        private List<Vector3> nodes;
        private Stack<Vector3> sortedNodes;
        public Vector3[] sor_nodes;

        public ConvexAogrithm(List<Vector3> points)
        {
            nodes = points;
        }

        private double DistanceOfNodes(Vector3 p0, Vector3 p1)
        {
            return Vector3.Distance(p0, p1);
        }

        public void GetNodesByAngle()
        {
            Vector3 a;
            GetNodesByAngle(out a);
        }

        public void GetNodesByAngle(out Vector3 p0)
        {
            LinkedList<Vector3> list_node = new LinkedList<Vector3>();
            p0 = GetMinYPoint();
            LinkedListNode<Vector3> node = new LinkedListNode<Vector3>(nodes[0]);
            list_node.AddFirst(node);
            for (int i = 1; i < nodes.Count; i++)
            {
                int direct = IsClockDirection(p0, node.Value, nodes[i]);
                if (direct == 1)
                {
                    list_node.AddLast(nodes[i]);
                    node = list_node.Last;
                    //node.Value = nodes[i]; 

                }
                else if (direct == -10)
                {
                    list_node.Last.Value = nodes[i];
                    //node = list_node.Last 
                    //node.Value = nodes[i]; 
                }
                else if (direct == 10)
                    continue;
                else if (direct == -1)
                {
                    LinkedListNode<Vector3> temp = node.Previous;
                    while (temp != null && IsClockDirection(p0, temp.Value, nodes[i]) == -1)
                    {
                        temp = temp.Previous;
                    }
                    if (temp == null)
                    {
                        list_node.AddFirst(nodes[i]);
                        continue;
                    }
                    if (IsClockDirection(p0, temp.Value, nodes[i]) == -10)
                        temp.Value = nodes[i];
                    else if (IsClockDirection(p0, temp.Value, nodes[i]) == 10)
                        continue;
                    else
                        list_node.AddAfter(temp, nodes[i]);
                }
            }
            sor_nodes = list_node.ToArray();
            sortedNodes = new Stack<Vector3>();
            sortedNodes.Push(p0);
            sortedNodes.Push(sor_nodes[0]);
            sortedNodes.Push(sor_nodes[1]);
            for (int i = 2; i < sor_nodes.Length; i++)
            {

                Vector3 p2 = sor_nodes[i];
                Vector3 p1 = sortedNodes.Pop();
                Vector3 p0_sec = sortedNodes.Pop();
                sortedNodes.Push(p0_sec);
                sortedNodes.Push(p1);

                if (IsClockDirection1(p0_sec, p1, p2) == 1)
                {
                    sortedNodes.Push(p2);
                    continue;
                }
                while (IsClockDirection1(p0_sec, p1, p2) != 1)
                {
                    sortedNodes.Pop();
                    p1 = sortedNodes.Pop();
                    p0_sec = sortedNodes.Pop();
                    sortedNodes.Push(p0_sec);
                    sortedNodes.Push(p1);
                }
                sortedNodes.Push(p2);
            }


        }

        private int IsClockDirection1(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            //Vector3 p0_p1 = new Vector3(p1.x - p0.x, p1.z - p0.z);
            Vector3 p0_p1 = new Vector3(p1.x - p0.x, 0, p1.z - p0.z);
            Vector3 p0_p2 = new Vector3(p2.x - p0.x, 0, p2.z - p0.z);
            return (p0_p1.x * p0_p2.z - p0_p2.x * p0_p1.z) > 0 ? 1 : -1;
        }

        private Vector3 GetMinYPoint()
        {
            Vector3 succNode;
            float miny = nodes.Min(r => r.z);
            IEnumerable<Vector3> pminYs = nodes.Where(r => r.z == miny);
            Vector3[] ps = pminYs.ToArray();
            if (pminYs.Count() > 1)
            {
                //succNode = pminYs.Single(r => r.x == pminYs.Min(t => t.x));//TODO:Linq换一下
                succNode = pminYs.First(r => r.x == pminYs.Min(t => t.x));
                nodes.Remove(succNode);
                return succNode;
            }
            else
            {
                nodes.Remove(ps[0]);
                return ps[0];
            }

        }

        private int IsClockDirection(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Vector3 p0_p1 = new Vector3(p1.x - p0.x, 0, p1.z - p0.z);
            Vector3 p0_p2 = new Vector3(p2.x - p0.x, 0, p2.z - p0.z);
            if ((p0_p1.x * p0_p2.z - p0_p2.x * p0_p1.z) != 0)
                return (p0_p1.x * p0_p2.z - p0_p2.x * p0_p1.z) > 0 ? 1 : -1;
            else
                return DistanceOfNodes(p0, p1) > DistanceOfNodes(p0, p2) ? 10 : -10;

        }

        public Stack<Vector3> SortedNodes
        {
            get { return sortedNodes; }
        }

    }
}