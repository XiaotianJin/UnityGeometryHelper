//using System.Collections.Generic;
//using g3;
//using UnityEngine;

//namespace UnityGeometryHelper
//{
//    public static class Geometric3D
//    {
//        /// <summary>
//        /// 求过lp1,lp2与三角形pp1,pp2,pp3所在平面的交点
//        /// </summary>
//        /// <param name="planeP1">平面三角形1</param>
//        /// <param name="planeP2">平面三角形2</param>
//        /// <param name="planeP3">平面三角形3</param>
//        /// <param name="lineP1">直线1</param>
//        /// <param name="lineP2">直线2</param>
//        /// <returns></returns>
//        public static Vector3 GetCrossPointOfPlaneAndLine(Vector3 planeP1, Vector3 planeP2, Vector3 planeP3,
//            Vector3 lineP1, Vector3 lineP2)
//        {
//            Vector3 planeNor = (Vector3.Cross((planeP1 - planeP2), (planeP3 - planeP2))).normalized;

//            Vector3 planeP = planeP2;

//            float x1 = lineP1.x;
//            float x2 = lineP2.x;
//            float y1 = lineP1.y;
//            float y2 = lineP2.y;
//            float z1 = lineP1.z;
//            float z2 = lineP2.z;

//            float v1 = x2 - x1;
//            float m1 = x1;
//            float v2 = y2 - y1;
//            float m2 = y1;
//            float v3 = z2 - z1;
//            float m3 = z1;

//            float n1 = planeP.x;
//            float n2 = planeP.y;
//            float n3 = planeP.z;

//            float vp1 = planeNor.x;
//            float vp2 = planeNor.y;
//            float vp3 = planeNor.z;

//            float under = vp1 * v1 + vp2 * v2 + vp3 * v3;
//            if (under.Equals(0f))
//            {
//                return default(Vector3);
//            }

//            float up = (n1 - m1) * vp1 + (n2 - m2) * vp2 + (n3 - m3) * vp3;

//            float resX = (((x2 - x1) * up) / under) + x1;
//            float resY = (((y2 - y1) * up) / under) + y1;
//            float resZ = (((z2 - z1) * up) / under) + z1;
//            return new Vector3(resX, resY, resZ);
//        }

//        /// <summary>
//        /// 直线是否与空间中的体相交
//        /// </summary>
//        /// <param name="lineP1">直线线点1</param>
//        /// <param name="lineP2">直线线点2</param>
//        /// <param name="verts">mesh顶点</param>
//        /// <param name="tris">mesh三角</param>
//        /// <returns></returns>
//        public static bool IsLineCrossSpace(Vector3 lineP1, Vector3 lineP2, Vector3[] verts, int[] tris)
//        {
//            List<Vector3f> vertfs = new List<Vector3f>();
//            for (int i = 0; i < verts.Length; i++)
//            {
//                vertfs.Add(new Vector3f(verts[i].x, verts[i].y, verts[i].z));
//            }

//            DMesh3 mesh = DMesh3Builder.Build<Vector3f, int, int>(vertfs, tris);

//            DMeshAABBTree3 spatial = new DMeshAABBTree3(mesh);
//            spatial.Build();

//            Vector3 l12 = lineP2 - lineP1;

//            Vector3f origin1 = new Vector3f(lineP1.x, lineP1.y, lineP1.z);
//            Vector3f origin2 = new Vector3f(lineP2.x, lineP2.y, lineP2.z);
//            Vector3f direction = new Vector3f(l12.x, l12.y, l12.z);
//            Ray3d ray = new Ray3d(origin1, direction);

//            int hit_tid = spatial.FindNearestHitTriangle(ray);
//            bool isHit1 = hit_tid != DMesh3.InvalidID;

//            Ray3d ray2 = new Ray3d(origin2, -direction);
//            hit_tid = spatial.FindNearestHitTriangle(ray2);
//            bool isHit2 = hit_tid != DMesh3.InvalidID;

//            return isHit1 || isHit2;
//        }

//        /// <summary>
//        /// 射线是否和空间中的体相交
//        /// </summary>
//        /// <param name="origin"></param>
//        /// <param name="direction"></param>
//        /// <param name="verts"></param>
//        /// <param name="tris"></param>
//        /// <returns></returns>
//        public static bool IsRayCrossSpace(Vector3 origin, Vector3 direction, Vector3[] verts, int[] tris)
//        {
//            List<Vector3f> vertfs = new List<Vector3f>();
//            for (int i = 0; i < verts.Length; i++)
//            {
//                vertfs.Add(new Vector3f(verts[i].x, verts[i].y, verts[i].z));
//            }

//            DMesh3 mesh = DMesh3Builder.Build<Vector3f, int, int>(vertfs, tris);

//            DMeshAABBTree3 spatial = new DMeshAABBTree3(mesh);
//            spatial.Build();

//            Vector3f originf = new Vector3f(origin.x, origin.y, origin.z);
//            Vector3f directionf = new Vector3f(direction.x, direction.y, direction.z);
//            Ray3d ray = new Ray3d(originf, directionf);

//            int hit_tid = spatial.FindNearestHitTriangle(ray);
//            bool isHit1 = hit_tid != DMesh3.InvalidID;

//            return isHit1;
//        }

//        public static bool IsSegmentCrossSpace(Vector3 segmentStart, Vector3 segmentEnd, 
//            Vector3[] verts, int[] tris)
//        {
//            List<Vector3f> vertfs = new List<Vector3f>();
//            for (int i = 0; i < verts.Length; i++)
//            {
//                vertfs.Add(new Vector3f(verts[i].x, verts[i].y, verts[i].z));
//            }

//            DMesh3 mesh = DMesh3Builder.Build<Vector3f, int, int>(vertfs, tris);

//            DMeshAABBTree3 spatial = new DMeshAABBTree3(mesh);
//            spatial.Build();

//            Vector3 l12 = segmentStart - segmentEnd;

//            Vector3f origin1 = new Vector3f(segmentStart.x, segmentStart.y, segmentStart.z);
//            Vector3f origin2 = new Vector3f(segmentEnd.x, segmentEnd.y, segmentEnd.z);
//            Vector3f direction = new Vector3f(l12.x, l12.y, l12.z);
//            Ray3d ray = new Ray3d(origin1, direction);
//            Ray3d ray2 = new Ray3d(origin2, -direction);
//            int hit_tid1 = spatial.FindNearestHitTriangle(ray);
//            int hit_tid2 = spatial.FindNearestHitTriangle(ray2);

//            if (hit_tid1 == DMesh3.InvalidID && hit_tid1 == DMesh3.InvalidID)
//            {
//                return false;
//            }

//            if (hit_tid1 != DMesh3.InvalidID)
//            {
//                Vector3 hitPoint = GetCrossPointOfTri(verts, tris, hit_tid1, segmentStart, segmentEnd);
//                if (Vector3.Distance(hitPoint, segmentStart) + Vector3.Distance(hitPoint, segmentEnd) >
//                    Vector3.Distance(segmentEnd, segmentStart))
//                {
//                    return false;
//                }
//            }

//            if (hit_tid2 != DMesh3.InvalidID)
//            {
//                Vector3 hitPoint = GetCrossPointOfTri(verts, tris, hit_tid2, segmentStart, segmentEnd);
//                if (Vector3.Distance(hitPoint, segmentStart) + Vector3.Distance(hitPoint, segmentEnd) >
//                    Vector3.Distance(segmentEnd, segmentStart))
//                {
//                    return false;
//                }
//            }

//            return true;
//        }

//        public static Vector3 GetCrossPointOfTri(Vector3[] verts, int[] tris, int triId, Vector3 a, Vector3 b)
//        {
//            List<Vector3> triHit = new List<Vector3>()
//            {
//                verts[tris[triId]],
//                verts[tris[triId + 1]],
//                verts[tris[triId + 2]],
//            };

//            return GetCrossPointOfPlaneAndLine(triHit[0], triHit[1], triHit[2], a, b);
//        }
//    }
//}
