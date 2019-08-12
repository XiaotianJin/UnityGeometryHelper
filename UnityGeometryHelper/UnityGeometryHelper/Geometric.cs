using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityGeometryHelper
{
    public static class Geometric
    {
        #region 基础面积计算
        /// <summary>
        /// AB X AC
        /// </summary>
        /// <param name="A">点A</param>
        /// <param name="B">点B</param>
        /// <param name="C">点C</param>
        /// <returns></returns>
        private static float cross(Vector3 A, Vector3 B, Vector3 C)
        {
            return (B.x - A.x) * (C.z - A.z) - (B.z - A.z) * (C.x - A.x);
        }

        /// <summary>
        /// 使用向量叉乘来计算任意多边形面积
        /// </summary>
        /// <param name="points">顺序描述的点集</param>
        /// <returns>面积</returns>
        public static float GetArea(params Vector3[] points)
        {
            //去重
            List<Vector3> toCalcualte = new List<Vector3>(points);
            for (int i = 0; i < toCalcualte.Count; i++)
            {
                for (int j = 0; j < toCalcualte.Count; j++)
                {
                    if (j != i && toCalcualte[i] == toCalcualte[j])
                    {
                        toCalcualte.RemoveAt(j);
                        j--;
                    }
                }
            }

            int n = toCalcualte.Count;
            for (int i = 0; i < n; i++)
            {
                if (Geometric.Meet(toCalcualte[i], toCalcualte[(i + 1) % n], toCalcualte[(i + 2) % n], toCalcualte[(i + 3) % n]))
                {
                    var temp = toCalcualte[(i + 2) % n];
                    toCalcualte[(i + 2) % n] = toCalcualte[(i + 1) % n];
                    toCalcualte[(i + 1) % n] = temp;
                }
            }

            points = toCalcualte.ToArray();

            float area = 0f;
            Vector3 ANXI = toCalcualte[0] + Vector3.one * 100;//要尽量远来补平舍入误差
            for (int i = 0; i < points.Length; i++)
            {
                area += cross(ANXI, points[i], points[(i + 1) % points.Length]);
            }
            area /= 2f;
            return Mathf.Abs(area);
        }

        #endregion

        #region 点面关系

        /// <summary>
        /// 检测点是否在区域内
        /// </summary>
        /// <param name="RegionVertexes">顺序描述的区域定点</param>
        /// <param name="toCheck">检查点</param>
        /// <returns></returns>
        public static bool IsPointInArea(Vector3[] RegionVertexes, Vector3 toCheck)
        {
            float minX = Single.MaxValue;
            float minZ = Single.MaxValue;
            float maxX = Single.MinValue;
            float maxZ = Single.MinValue;
            foreach (var vertex in RegionVertexes)
            {
                if (vertex.x < minX)
                {
                    minX = vertex.x;
                }

                if (vertex.x > maxX)
                {
                    maxX = vertex.x;
                }

                if (vertex.z < minZ)
                {
                    minZ = vertex.z;
                }

                if (vertex.z > maxZ)
                {
                    maxZ = vertex.z;
                }
            }

            if (toCheck.x < minX || toCheck.x > maxX || toCheck.z < minZ || toCheck.z > maxZ)
            {
                //Console.WriteLine(minX+ " ! " + minZ);
                return false;
            }//长方形校验不通过直接可以返回flase的

            return Pnpoly(RegionVertexes.Length, RegionVertexes, toCheck.x, toCheck.z);
        }


        public static bool IsPointAlmostInArea(Vector3[] rvs, Vector3 toCheck, float offset = 0.1f)
        {
            if (IsPointInArea(rvs, toCheck))
            {
                return true;
            }

            return IsPointOnAreaSide(rvs, toCheck, offset);
        }

        public static bool IsPointOnAreaSide(Vector3[] rvs, Vector3 toCheck, float offset = 0.1f)
        {
            for (int i = 0; i < rvs.Length; i++)
            {
                var thisStart = rvs[i];
                var thisEnd = rvs[(i + 1) % rvs.Length];
                if (IsPointAlmostOnSegment(thisStart, thisEnd, offset * 2,
                    toCheck))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsAreaFullyInAnotherArea(Vector3[] area1, Vector3[] area2)
        {
            int n1 = area1.Length;
            int n2 = area2.Length;
            for (int i = 0; i < n1; i++)
            {
                for (int j = 0; j < n2; j++)
                {
                    Vector3 point11 = area1[i];
                    Vector3 point12 = area1[(i + 1) % n1];
                    Vector3 point21 = area2[j];
                    Vector3 point22 = area2[(j + 1) % n2];

                    if (Meet(point11, point12, point21, point22))
                    {
                        return false;
                    }
                }
            }//首先保证边界没有交点

            bool res1 = true;
            bool res2 = true;
            foreach (var point in area1)
            {
                if (!IsPointInArea(area2, point))
                {
                    res1 = false;
                    break;
                }
            }

            foreach (var point in area2)
            {
                if (!IsPointInArea(area1, point))
                {
                    res2 = false;
                    break;
                }
            }

            return res1 ^ res2;//必然有一个全部在内，另一个全部在外
        }

        public static bool IsAreaPartlyInAnotherArea(Vector3[] area1, Vector3[] area2)
        {
            int n1 = area1.Length;
            int n2 = area2.Length;
            for (int i = 0; i < n1; i++)
            {
                for (int j = 0; j < n2; j++)
                {
                    Vector3 point11 = area1[i];
                    Vector3 point12 = area1[(i + 1) % n1];
                    Vector3 point21 = area2[j];
                    Vector3 point22 = area2[(j + 1) % n2];

                    if (Meet(point11, point12, point21, point22))
                    {
                        return true;
                    }
                }
            }//边界相交必然部分在内

            bool res1 = false;
            bool res2 = false;
            foreach (var point in area1)
            {
                if (IsPointInArea(area2, point))
                {
                    res1 = true;
                    break;
                }
            }

            foreach (var point in area2)
            {
                if (IsPointInArea(area1, point))
                {
                    res2 = true;
                    break;
                }
            }

            return res1 || res2;//边界没有相交的情况，除了完全分离的情况都是包含
        }

        /// <summary>
        /// Pnpoly内点检测算法
        /// </summary>
        /// <param name="polySides">边树</param>
        /// <param name="RegionVertexes">顺序描述的区域点集</param>
        /// <param name="x">x坐标</param>
        /// <param name="z">z坐标</param>
        /// <returns></returns>
        private static bool Pnpoly(int polySides, Vector3[] RegionVertexes, float x, float z)
        {
            return PNpoly_X(polySides, RegionVertexes, x, z) || PNpoly_Y(polySides, RegionVertexes, x, z);
        }

        private static bool PNpoly_Y(int polySides, Vector3[] RegionVertexes, float x, float z)
        {
            Vector3 pointLeft = new Vector3(x, 0, z - 9999);
            Vector3 pointRight = new Vector3(x, 0, z + 9999);
            Vector3 pointSelf = new Vector3(x, 0, z);

            int left = 0;
            int right = 0;
            for (int i = 0; i < polySides; i++)
            {
                Vector3 point1 = RegionVertexes[i];
                Vector3 point2 = RegionVertexes[(i + 1) % polySides];
                if (Meet(pointLeft, pointSelf, point1, point2)/* && !Geometric.IsDirParallel(pointLeft - pointSelf, point2 - point1)*/)
                {
                    left++;
                }
                else if (Meet(pointRight, pointSelf, point1, point2)/* && !Geometric.IsDirParallel(pointRight - pointSelf, point2 - point1)*/)
                {
                    right++;
                }
            }

            if ((left % 2 == 1) && right % 2 == 1)
            {
                return true;
            }

            return false;
        }

        private static bool PNpoly_X(int polySides, Vector3[] RegionVertexes, float x, float z)
        {
            Vector3 pointLeft = new Vector3(x - 9999, 0, z);
            Vector3 pointRight = new Vector3(x + 9999, 0, z);
            Vector3 pointSelf = new Vector3(x, 0, z);

            int left = 0;
            int right = 0;
            for (int i = 0; i < polySides; i++)
            {
                Vector3 point1 = RegionVertexes[i];
                Vector3 point2 = RegionVertexes[(i + 1) % polySides];
                if (Meet(pointLeft, pointSelf, point1, point2)/* && !Geometric.IsDirParallel(pointLeft-pointSelf, point2-point1)*/)
                {
                    left++;
                }
                else if (Meet(pointRight, pointSelf, point1, point2)/* && !Geometric.IsDirParallel(pointRight - pointSelf, point2 - point1)*/)
                {
                    right++;
                }
            }

            if ((left % 2 == 1) && right % 2 == 1)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region 求线段交点
        //这一套没有问题！
        const float eps = (float)1e-6;

        const float Pi = Mathf.PI;

        public static bool IsDirParallel(Vector3 a, Vector3 b, float offset = 3f)
        {
            float angle = Vector3.Angle(a, b);
            return (angle < offset || angle > (180 - offset));
        }
        public static bool IsDirSame(Vector3 a, Vector3 b, float offset = 3f)
        {
            float angle = Vector3.Angle(a, b);
            return (angle < offset);
        }
        /// <summary>
        /// 线段12-34的交点
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        /// <returns></returns>
        public static Vector3 GetCorssPointOfSegment(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
        {
            if (!Meet(point1, point2, point3, point4))
            {
                return default(Vector3);
            }

            if (IsDirParallel(point1 - point2, point3 - point4))
            {
                return default(Vector3);
            }

            return Inter(point1, point2, point3, point4);
        }

        public static Vector3 GetCrossPointOfLine(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
        {
            Vector3 _dir12 = (point2 - point1).normalized;
            Vector3 _dir34 = (point4 - point3).normalized;

            float _angle = Vector3.Angle(_dir12, _dir34);
            if (_angle < 0.1f || _angle > 179.9f)
            {
                return default(Vector3);
            }

            var res = GetCorssPointOfSegment(point1 + _dir12 * 99999, point2 - _dir12 * 99999, point3 + _dir34 * 99999,
                point4 - _dir34 * 99999);
            return res;
        }

        /// <summary>
        /// 获取x的符号
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        static int sgn(float x)
        {
            if (x < -eps)
            {
                return -1;
            }
            else
            {
                return x > eps ? 0 : 1;
            }
        }

        /// <summary>
        /// 向量叉乘
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <returns></returns>
        static float Cross(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            return (p2.x - p1.x) * (p4.z - p3.z) - (p2.z - p1.z) * (p4.x - p3.x);
        }

        static float Area(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return Cross(p1, p2, p1, p3);
        }

        static float fArea(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return Mathf.Abs((float)Area(p1, p2, p3));
        }

        /// <summary>
        /// 线段是否相交
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <returns></returns>
        public static bool Meet(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            return Mathf.Max(Mathf.Min(p1.x, p2.x), Mathf.Min(p3.x, p4.x)) <= Mathf.Min(Mathf.Max(p1.x, p2.x), Mathf.Max(p3.x, p4.x))
                && Mathf.Max(Mathf.Min(p1.z, p2.z), Mathf.Min(p3.z, p4.z)) <= Mathf.Min(Mathf.Max(p1.z, p2.z), Mathf.Max(p3.z, p4.z))
                && sgn(Cross(p3, p2, p3, p4) * Cross(p3, p4, p3, p1)) >= 0
                && sgn(Cross(p1, p4, p1, p2) * Cross(p1, p2, p1, p3)) >= 0;
        }

        public static bool Meet_crossWay(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            Vector3 crossP = GetCrossPointOfLine(p1, p2, p3, p4);
            return Geometric.IsPointOnSegment(p1, p2, crossP) && Geometric.IsPointOnSegment(p3, p4, crossP);
        }

        public static bool IsTwoLinePointPartCover(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float offset = 0.03f)
        {
            return Geometric.IsPointAlmostOnSegment(a, b, offset, c) || Geometric.IsPointAlmostOnSegment(a, b, offset, d) ||
                   Geometric.IsPointAlmostOnSegment(c, d, offset, a) || Geometric.IsPointAlmostOnSegment(c, d, offset, b);
        }

        static Vector3 Inter(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            //特判：某顶点在另外一条直线上时
            if (IsPointOnLine(p1, p2, p3))
            {
                return p3;
            }
            if (IsPointOnLine(p1, p2, p4))
            {
                return p4;
            }
            if (IsPointOnLine(p3, p4, p1))
            {
                return p1;
            }
            if (IsPointOnLine(p3, p4, p2))
            {
                return p2;
            }
            float k = fArea(p1, p2, p3) / fArea(p1, p2, p4);
            return new Vector3((float)(p3.x + k * p4.x) / (float)(1 + k), 0, (float)(p3.z + k * p4.z) / (float)(1 + k));
        }

        #endregion

        #region 点线关系

        public static bool IsPointAlmostOnSegment(Vector3 lineP1, Vector3 lineP2, float width, Vector3 toCheck)
        {
            Vector3 _lineDir = (lineP2 - lineP1).normalized;
            Quaternion q = QuaternionUtility.Euler(0, 90, 0);
            Vector3 _rightDir = (q * _lineDir).normalized;

            Vector3 diff = _rightDir * width - _lineDir * width;

            Vector3 lp1Right = lineP1 + diff;
            Vector3 lp1Left = lineP1 - diff;

            Vector3 lp2Right = lineP2 + diff;
            Vector3 lp2Left = lineP2 - diff;

            var res = Geometric.IsPointInArea(new[] { lp1Right, lp2Right, lp2Left, lp1Left }, toCheck);
            return res;
        }


        /// <summary>
        /// 点到线距离
        /// </summary>
        /// <param name="point">点</param>
        /// <param name="a">线点1</param>
        /// <param name="b">线点2</param>
        /// <returns></returns>
        public static float GetDistanceFromPointToLine(Vector3 point, Vector3 a, Vector3 b)
        {
            if (Mathf.Abs(a.x - b.x) < 1e-6)
            {
                return Mathf.Abs(point.x - a.x);
            }
            if (Mathf.Abs(a.z - b.z) < 1e-6)
            {
                return Mathf.Abs(point.z - a.z);
            }

            float area = GetArea(point, a, b);
            return (2 * area) / Vector3.Distance(a, b);
        }

        /// <summary>
        /// 点是否在线上
        /// </summary>
        /// <param name="line1">线段点1</param>
        /// <param name="line2">线段点2</param>
        /// <param name="p">检查点</param>
        /// <returns></returns>
        public static bool IsPointOnLine(Vector3 line1, Vector3 line2, Vector3 p)
        {
            float k = (line2.z - line1.z) / (line2.x - line1.x);
            float b = (line2.z * line1.x - line1.z * line2.x) / (line1.x - line2.x);

            if (Mathf.Abs(line1.x - line2.x) <= eps && Mathf.Abs(line1.x - p.x) <= eps)
            {
                return true;
            }

            if (Mathf.Abs((p.x * k + b) - p.z) < eps)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否在线段上
        /// </summary>
        /// <param name="line1">线段点1</param>
        /// <param name="line2">线段点2</param>
        /// <param name="p">检查点</param>
        /// <returns></returns>
        public static bool IsPointOnSegment(Vector3 line1, Vector3 line2, Vector3 p)
        {

            if (Mathf.Abs(line1.x - line2.x) <= eps && Mathf.Abs(line1.x - p.x) <= eps)
            {
                float bigZ = line1.z > line2.z ? line1.z : line2.z;
                float smallZ = line1.z < line2.z ? line1.z : line2.z;
                if (p.z <= bigZ && p.z >= smallZ)
                {
                    return true;
                }
            }

            float k = (line2.z - line1.z) / (line2.x - line1.x);
            float b = (line2.z * line1.x - line1.z * line2.x) / (line1.x - line2.x);
            if (Mathf.Abs((p.x * k + b) - p.z) < eps)
            {
                float maxX = line1.x > line2.x ? line1.x : line2.x;
                float minX = line1.x < line2.x ? line1.x : line2.x;
                float maxZ = line1.z > line2.z ? line1.z : line2.z;
                float minZ = line1.z < line2.z ? line1.z : line2.z;
                if (p.x <= maxX && p.x >= minX && p.z <= maxZ && p.z >= minZ)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 点群

        public static List<Vector3> SortPointsByAngle(List<Vector3> points)
        {
            DupKeyMap<float, Vector3> map = new DupKeyMap<float, Vector3>();
            Vector3 midP = new Vector3();
            foreach (var p in points)
            {
                midP += p;
            }
            midP /= points.Count;
            foreach (var p in points)
            {
                map.Add(AbsYAngle(midP, p), p);
            }

            map.SortedByKey((a, b) => { return a > b; });
            return map.AllValue;
        }

        /// <summary>
        /// 找出allP中离toCheck最近的点
        /// </summary>
        /// <param name="toCheck"></param>
        /// <param name="allP"></param>
        /// <returns></returns>
        public static Vector3 GetCloestPoint(Vector3 toCheck, List<Vector3> allP)
        {
            if (allP == null || allP.Count == 0)
            {
                return default(Vector3);
            }

            Vector3 res = allP[0];
            float minDis = Vector3.Distance(res, toCheck);
            for (int i = 0; i < allP.Count; i++)
            {
                float thisDis = Vector3.Distance(toCheck, allP[i]);
                if (thisDis < minDis)
                {
                    minDis = thisDis;
                    res = allP[i];
                }
            }

            return res;
        }

        /// <summary>
        /// 获取点群的凸包
        /// </summary>
        /// <param name="allPoints">点群集合</param>
        /// <returns>凸包点群集合（不保证点序），最后一点重复第一点</returns>
        public static List<Vector3> GetBoundary(List<Vector3> allPoints)
        {
            //Drawer.Points(allPoints.ToArray(), Color.blue);
            ConvexAogrithm ca = new ConvexAogrithm(allPoints);
            ca.GetNodesByAngle();

            var res = ca.SortedNodes.ToList();
            res.Add(res[0]);
            return res;
        }

        /// <summary>
        /// 获取点群的最大闭包矩形
        /// </summary>
        /// <param name="allPoints">点群集合</param>
        /// <returns>最大闭包矩形五点（保证点序，最后一点重复第一点）</returns>
        public static List<Vector3> GetMaxClosureRect(List<Vector3> allPoints)
        {
            List<Vector3> res = new List<Vector3>();

            float maxX = Single.MinValue;
            float maxZ = Single.MinValue;
            float minX = Single.MaxValue;
            float minZ = Single.MaxValue;

            for (int i = 0; i < allPoints.Count; i++)
            {
                if (allPoints[i].x > maxX)
                {
                    maxX = allPoints[i].x;
                }

                if (allPoints[i].z > maxZ)
                {
                    maxZ = allPoints[i].z;
                }

                if (allPoints[i].x < minX)
                {
                    minX = allPoints[i].x;
                }

                if (allPoints[i].z < minZ)
                {
                    minZ = allPoints[i].z;
                }
            }

            Vector3 leftUp = new Vector3(minX, 0, maxZ);
            Vector3 leftDown = new Vector3(minX, 0, minZ);
            Vector3 rightDown = new Vector3(maxX, 0, minZ);
            Vector3 rightUp = new Vector3(maxX, 0, maxZ);

            res.Add(leftUp);
            res.Add(leftDown);
            res.Add(rightDown);
            res.Add(rightUp);

            res.Add(leftUp);

            return res;
        }

        /// <summary>
        /// 点群线分
        /// </summary>
        /// <param name="a">线点1</param>
        /// <param name="b">线点2</param>
        /// <param name="points">点群</param>
        /// <returns></returns>
        public static List<List<Vector3>> DividePointsByLine(Vector3 a, Vector3 b,
            List<Vector3> points)
        {
            Vector3 abDir = (b - a).normalized;// 线方向

            Dictionary<bool, List<Vector3>> isOnRightMap =
                new Dictionary<bool, List<Vector3>>();
            isOnRightMap.Add(false, new List<Vector3>());
            isOnRightMap.Add(true, new List<Vector3>());
            for (int i = 0; i < points.Count; i++)
            {
                bool key = IsOnRight(a, b, points[i]);
                if (isOnRightMap.ContainsKey(key))
                {
                    isOnRightMap[key].Add(points[i]);
                }
                else
                {
                    isOnRightMap.Add(key, new List<Vector3>() { points[i] });
                }
            }

            return isOnRightMap.Values.ToList();
        }

        public static bool IsOnRight(Vector3 a, Vector3 b, Vector3 p)
        {
            Vector3 abDir = (b - a).normalized;// 线方向
            Vector3 apDir = (p - a).normalized;// 点方向

            return ClockYAngle(abDir, apDir) < 180;
        }

        public delegate Vector3 GetVector3<T>(T toGet);

        public static List<List<T>> DividePointsByLine<T>(Vector3 a, Vector3 b,
            List<T> points, GetVector3<T> getter)
        {
            Dictionary<Vector3, List<T>> pointTMap = new Dictionary<Vector3, List<T>>();
            List<Vector3> allP = new List<Vector3>();
            for (int i = 0; i < points.Count; i++)
            {
                var key = getter(points[i]);
                if (pointTMap.ContainsKey(key))
                {
                    pointTMap[key].Add(points[i]);
                }
                else
                {
                    pointTMap.Add(key, new List<T>() { points[i] });
                }
                allP.Add(getter(points[i]));
            }

            List<List<Vector3>> pRes = DividePointsByLine(a, b, allP);
            List<List<T>> res = new List<List<T>>();
            for (int i = 0; i < pRes.Count; i++)
            {
                List<T> row = new List<T>();
                for (int j = 0; j < pRes[i].Count; j++)
                {
                    for (int k = 0; k < pointTMap[pRes[i][j]].Count; k++)
                    {
                        row.Add(pointTMap[pRes[i][j]][k]);
                    }
                }
                res.Add(row);
            }

            return res;
        }

        #endregion

        #region V3操作


        public delegate float distanceCalc(Vector3 a, Vector3 b);
        /// <summary>
        /// 比较checker是不是比pin要离target更远
        /// </summary>
        /// <param name="target"></param>
        /// <param name="check"></param>
        /// <param name="pin"></param>
        /// <returns></returns>
        public static bool IsFarFrom(Vector3 target, Vector3 check, Vector3 pin, distanceCalc calc)
        {
            return calc(target, check) > calc(target, pin);
        }

        public static bool IsOnLeft(Vector3 forward, Vector3 toCheck)
        {
            return Vector3.Cross(forward, toCheck).y < 0;
        }

        /// <summary>
        /// 求y平面A-B的顺时针角度，
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float ClockAngle(Vector3 a, Vector3 b)
        {
            float angle = Vector3.Angle(a, b);
            if (angle.Equals(0))
            {
                return angle;
            }

            if (Vector3.Cross(a, b).y >= 0)
            {
                return angle;
            }
            else
            {
                return 360 - angle;
            }
        }

        /// <summary>
        /// 求y平面A-B的顺时针角度，
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float ClockYAngle(Vector3 dirA, Vector3 dirB)
        {
            float angle = Vector3.Angle(new Vector3(dirA.x, 0, dirA.z), new Vector3(dirB.x, 0, dirB.z));
            if (angle.Equals(0))
            {
                return angle;
            }

            if (Vector3.Cross(dirA, dirB).y >= 0)
            {
                return angle;
            }
            else
            {
                return 360 - angle;
            }
        }

        public static float AbsYAngle(Vector3 pointA, Vector3 pointB, Vector3 checkDir)
        {
            Vector3 ab = pointB - pointA;
            ab.y = 0;
            Vector3 anx = checkDir;
            if (Vector3.Cross(anx, ab).y <= 0)
            {
                return Vector3.Angle(anx, ab);
            }
            else
            {
                return 360 - Vector3.Angle(anx, ab);
            }
        }

        public static float AbsYAngle(Vector3 pointA, Vector3 pointB)
        {
            Vector3 ab = pointB - pointA;
            ab.y = 0;
            Vector3 anx = new Vector3(0, 0, 1);
            if (Vector3.Cross(anx, ab).y <= 0)
            {
                return Vector3.Angle(anx, ab);
            }
            else
            {
                return 360 - Vector3.Angle(anx, ab);
            }
        }

        /// <summary>
        /// 求y平面A-B的逆时针角度，
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float DeclockAngle(Vector3 a, Vector3 b)
        {
            float angle = Vector3.Angle(a, b);
            if (angle.Equals(0))
            {
                return angle;
            }

            if (Vector3.Cross(a, b).y >= 0)
            {
                return 360 - angle;
            }
            else
            {
                return angle;
            }
        }

        public static bool IsClose(Vector3 a, Vector3 b, float offset = 0.1f)
        {
            return Vector3.Distance(a, b) < offset;
        }

        public static bool Contain(List<Vector3> list, Vector3 toCheck, float offset = 0.1f)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (IsClose(list[i], toCheck, offset))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region LineRelated

        public static bool AlmostOnLine(Vector3 lineP1, Vector3 lineP2, float width, Vector3 toCheck)
        {
            Vector3 _lineDir = (lineP2 - lineP1).normalized;

            lineP1 += _lineDir*99;
            lineP2 -= _lineDir*99;

            Quaternion q = QuaternionUtility.Euler(0, 90, 0);
            Vector3 _rightDir = (q * _lineDir).normalized;

            Vector3 lp1Right = lineP1 + _rightDir * 0.5f * width - _lineDir * 0.5f * width;
            Vector3 lp1Left = lineP1 - _rightDir * 0.5f * width - _lineDir * 0.5f * width;

            Vector3 lp2Right = lineP2 + _rightDir * 0.5f * width + _lineDir * 0.5f * width;
            Vector3 lp2Left = lineP2 - _rightDir * 0.5f * width + _lineDir * 0.5f * width;

            return Geometric.IsPointInArea(new[] { lp1Right, lp2Right, lp2Left, lp1Left }, toCheck);
        }

        public static Vector3 GetHoverPoint(Vector3 lp1, Vector3 lp2, Vector3 modelP, float attachR)
        {
            var _area = Geometric.GetArea(lp1, lp2, modelP);//三角形面积
            var _lLength = Vector3.Distance(lp1, lp2);//lp1,lp2长度
            var dis = _lLength < 1e-3 ? attachR + 1 : 2 * _area / _lLength;//modelP到lp1,lp2的距离
            if (dis > attachR)//太远
            {
                return default(Vector3);
            }

            Vector3 lDir = lp1 - lp2;//直线方向
            Vector3 mDir = QuaternionUtility.Euler(0, 90, 0) * lDir;//xz平面上垂直直线方向
            Vector3 __S = modelP - mDir * 999;
            Vector3 __E = modelP + mDir * 999;//临时的两个线点

            if (!Geometric.Meet(__S, __E, lp1, lp2))//不相交
            {
                return default(Vector3);
            }

            //到这里还能执行的话是相交且接近, 就要计算点
            Vector3 res = Geometric.GetCorssPointOfSegment(__E, __S, lp1, lp2);
            return res;
        }

        public static float GetHoverDis(Vector3 lp1, Vector3 lp2, Vector3 modelP)
        {
            var _lp1 = new Vector3(lp1.x, 0, lp1.z);
            var _lp2 = new Vector3(lp2.x, 0, lp2.z);
            var _modelP = new Vector3(modelP.x, 0, modelP.z);

            var _area = Geometric.GetArea(_lp1, _lp2, _modelP);//三角形面积
            var _lLength = Vector3.Distance(_lp1, _lp2);//lp1,lp2长度
            var dis = 2 * _area / _lLength;//modelP到lp1,lp2的距离
            return dis;
        }

        public static bool IsHover(Vector3 lp1, Vector3 lp2, Vector3 modelP)
        {
            return !GetHoverPoint(lp1, lp2, modelP, 999).Equals(default(Vector3));
        }

        public static Vector3 GetHoverPointOfLine(Vector3 lp1, Vector3 lp2, Vector3 modelP, float attachR)
        {
            var _area = Geometric.GetArea(lp1, lp2, modelP);//三角形面积
            var _lLength = Vector3.Distance(lp1, lp2);//lp1,lp2长度
            var dis = _lLength < 1e-3 ? attachR + 1 : 2 * _area / _lLength;//modelP到lp1,lp2的距离
            if (dis > attachR)//太远
            {
                return default(Vector3);
            }

            Vector3 lDir = lp1 - lp2;//直线方向
            Vector3 mDir = QuaternionUtility.Euler(0, 90, 0) * lDir;//xz平面上垂直直线方向
            Vector3 __S = modelP - mDir * 99;
            Vector3 __E = modelP + mDir * 99;//临时的两个线点

            //if (!Geometric.Meet(__S, __E, lp1, lp2))//不相交
            //{
            //    return default(Vector3);
            //}

            //到这里还能执行的话是相交且接近, 就要计算点
            Vector3 res = Geometric.GetCrossPointOfLine(__E, __S, lp1, lp2);

            if ((Mathf.Abs(res.x - modelP.x) < 0.01f))
            {
                res.x = modelP.x;
            }

            if ((Mathf.Abs(res.z - modelP.z) < 0.01f))
            {
                res.z = modelP.z;
            }

            return res;
        }

        public static Vector3 GetHoverPointOf3DLine(Vector3 lp1, Vector3 lp2, Vector3 modelP)
        {
            float y = Vector3.Distance(lp2, modelP);
            float z = Vector3.Distance(lp1, modelP);
            float x = Vector3.Distance(lp1, lp2);
            float q = (x + y + z) * 0.5f;
            float area = Mathf.Sqrt(q * (q - x) * (q - y) * (q - z));

            float hoverDis = area / x;
            float fromyP = Mathf.Sqrt(y * y - hoverDis * hoverDis);

            return lp2 + (lp1 - lp2).normalized * fromyP;
        }
        #endregion

        /// <summary>
        /// 特殊的舍入进位算法
        /// </summary>
        /// <param name="value">需要变化的数值</param>
        /// <param name="atIndex">保留到10^atIndex位</param>
        /// <param name="step">位移步长，res=0+(i*step*2)</param>
        /// <returns></returns>
        public static double Round(double value, int atIndex = 0, float step = 0)
        {
            bool isNegative = false;
            //如果是负数
            if (value < 0)
            {
                isNegative = true;
                value = -value;
            }
            double absValue = Math.Abs(value);

            double iValue = Math.Pow(10, atIndex);// 扩张值

            double thisBigInt = Math.Round(value * iValue, 0);// 取出扩张后的整数位

            if (thisBigInt % 2 != 0)
            {
                if (thisBigInt < absValue * iValue)
                {
                    // 向下位移
                    thisBigInt -= step;
                }
                else
                {
                    // 向上位移
                    thisBigInt += step;
                }
            }

            double res = thisBigInt / iValue;

            if (isNegative)
            {
                res = -res;
            }

            return res;
        }
    }
}
