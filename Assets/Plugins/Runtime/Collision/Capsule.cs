using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TrueSync
{
    public struct Capsule
    {
        public TSVector center { get; set; }
        public FP radius { get; set; }
        public FP height { get; set; }
        public TSVector AxisX { get; private set; }
        public TSVector AxisY { get; private set; }
        public TSVector AxisZ { get; private set; }
        private TSQuaternion m_Quaternion;
        public TSQuaternion Quaternion
        {
            get
            {
                return m_Quaternion;
            }
            set
            {
                SetQuaternion(value);
            }
        }

        public Capsule(TSVector center, FP radius, FP height, TSQuaternion quaternion)
        {
            this.center = center;
            this.radius = radius;
            this.height = height;
            AxisX = TSVector.zero;
            AxisY = TSVector.zero;
            AxisZ = TSVector.zero;
            m_Quaternion = TSQuaternion.identity;
            SetQuaternion(quaternion);
        }

        public static bool operator ==(Capsule a, Capsule b)
        {
            return a.center == b.center && a.radius == b.radius && a.height == b.height;
        }

        public static bool operator !=(Capsule a, Capsule b)
        {
            return a.center != b.center || a.radius != b.radius || a.height != b.height;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Capsule)) return false;
            Capsule other = (Capsule)obj;
            return center == other.center && radius == other.radius && height == other.height;
        }

        public override int GetHashCode()
        {
            return center.GetHashCode() ^ radius.GetHashCode() ^ height.GetHashCode();
        }

        private void SetQuaternion(TSQuaternion quaternion)
        {
            m_Quaternion = quaternion;
            AxisX = quaternion * TSVector.right;
            AxisY = quaternion * TSVector.up;
            AxisZ = quaternion * TSVector.forward;
        }

        /// <summary>
        /// 测试胶囊与球相交
        /// </summary>
        public static bool Intersects(Capsule a, Sphere b)
        {
            var p = a.sqDistPointCapsule(b.c);
            return p <= (a.radius + b.r) * (a.radius + b.r);
        }


        public static bool Intersects(Capsule a, OBB b)
        {
            TSVector point = b.ClosestPointOBB(a.center);
            var p = a.sqDistPointCapsule(point);
            return p <= a.radius * a.radius;
        }


        public static bool Intersects(Capsule a, Capsule b)
        {
            TSVector startA = a.center + a.AxisY * ((a.height) * FP.Half - a.radius);
            TSVector endA = a.center - a.AxisY * ((a.height) * FP.Half - a.radius);
            TSVector startB = b.center + b.AxisY * ((b.height) * FP.Half - b.radius);
            TSVector endB = b.center - b.AxisY * ((b.height) * FP.Half - b.radius);
            return sqDistSegmentToSegment(startA, endA, startB, endB) <= (a.radius + b.radius) * (a.radius + b.radius);
        }


        public FP DistPointCapsule(TSVector p)
        {
            return FP.Sqrt(sqDistPointCapsule(p));
        }

        /// <summary>
        /// 点到胶囊最近距离平方
        /// </summary>
        /// <param name="p">点</param>
        /// <returns>最近的距离平方</returns>
        public FP sqDistPointCapsule(TSVector p)
        {
            if (height <= radius * 2)
                return TSVector.Distance(center, p);
            TSVector start = center + AxisY * ((height) * FP.Half - radius);
            TSVector end = center - AxisY * ((height) * FP.Half - radius);
            FP cross = (end.x - start.x) * (p.x - start.x) + (end.y - start.y) * (p.y - start.y) + (end.z - start.z) * (p.z - start.z);
            if (cross <= 0)
                return (p.x - start.x) * (p.x - start.x) + (p.y - start.y) * (p.y - start.y) + (p.z - start.z) * (p.z - start.z);
            FP d2 = (end.x - start.x) * (end.x - start.x) + (end.y - start.y) * (end.y - start.y) + (end.z - start.z) * (end.z - start.z);
            if (cross >= d2)
                return (p.x - end.x) * (p.x - end.x) + (p.y - end.y) * (p.y - end.y) + (p.z - end.z) * (p.z - end.z);
            FP r = cross / d2;
            FP px = start.x + r * (end.x - start.x);
            FP py = start.y + r * (end.y - start.y);
            FP pz = start.z + r * (end.z - start.z);
            return (p.x - px) * (p.x - px) + (p.y - py) * (p.y - py) + (p.z - pz) * (p.z - pz);
        }

        /// <summary>
        ///  点到胶囊最近距离坐标
        /// </summary>
        /// <param name="p">点</param>
        /// <returns>到胶囊最近的坐标</returns>
        public TSVector ClosestPointCapsule(TSVector p)
        {
            if (height <= radius * 2)
                return center;
            TSVector start = center + AxisY * ((height) * FP.Half - radius);
            TSVector end = center - AxisY * ((height) * FP.Half - radius);
            FP cross = (end.x - start.x) * (p.x - start.x) + (end.y - start.y) * (p.y - start.y) + (end.z - start.z) * (p.z - start.z);
            if (cross <= 0)
                return start + (p - start).normalized * radius;
            FP d2 = (end.x - start.x) * (end.x - start.x) + (end.y - start.y) * (end.y - start.y) + (end.z - start.z) * (end.z - start.z);
            if (cross >= d2)
                return end + (p - end).normalized * radius;
            FP r = cross / d2;
            return start + r * (end - start) + (p - start - r * (end - start)).normalized * radius;
        }

        /// <summary>
        /// 返回两线段最短距离平方根
        /// </summary>
        /// <param name="startA"></param>
        /// <param name="endA"></param>
        /// <param name="startB"></param>
        /// <param name="endB"></param>
        /// <returns></returns>
        public static FP sqDistSegmentToSegment(TSVector startA, TSVector endA, TSVector startB, TSVector endB)
        {
            //https://segmentfault.com/a/1190000006111226
            // 解析几何通用解法，可以求出点的位置，判断点是否在线段上
            // 算法描述：设两条无限长度直线s、t,起点为s0、t0，方向向量为u、v
            // 最短直线两点：在s1上为s0+sc*u，在t上的为t0+tc*v
            // 记向量w为(s0+sc*u)-(t0+tc*v),记向量w0=s0-t0
            // 记a=u*u，b=u*v，c=v*v，d=u*w0，e=v*w0——(a)；
            // 由于u*w=、v*w=0，将w=-tc*v+w0+sc*u带入前两式得：
            // (u*u)*sc - (u*v)*tc = -u*w0  (公式2)
            // (v*u)*sc - (v*v)*tc = -v*w0  (公式3)
            // 再将前式(a)带入可得sc=(be-cd)/(ac-b2)、tc=(ae-bd)/(ac-b2)——（b）
            // 注意到ac-b2=|u|2|v|2-(|u||v|cosq)2=(|u||v|sinq)2不小于0
            // 所以可以根据公式（b）判断sc、tc符号和sc、tc与1的关系即可分辨最近点是否在线段内
            // 当ac-b2=0时，(公式2)(公式3)独立，表示两条直线平行。可令sc=0单独解出tc
            // 最终距离d（L1、L2）=|（P0-Q0)+[(be-cd)*u-(ae-bd)v]/(ac-b2)|
            FP ux = endA.x - startA.x;
            FP uy = endA.y - startA.y;
            FP uz = endA.z - startA.z;
            FP vx = endB.x - startB.x;
            FP vy = endB.y - startB.y;
            FP vz = endB.z - startB.z;
            FP wx = startA.x - startB.x;
            FP wy = startA.y - startB.y;
            FP wz = startA.z - startB.z;
            FP a = ux * ux + uy * uy + uz * uz;
            FP b = ux * vx + uy * vy + uz * vz;
            FP c = vx * vx + vy * vy + vz * vz;
            FP d = ux * wx + uy * wy + uz * wz;
            FP e = vx * wx + vy * wy + vz * wz;
            FP dt = a * c - b * b;

            FP sd = dt;
            FP td = dt;

            FP sn = 0;
            FP tn = 0;

            if (dt <= FP.EN7)
            {
                // 两直线平行
                sn = 0;
                sd = 1;

                tn = e;
                td = c;
            }
            else
            {
                sn = b * e - c * d;
                tn = a * e - b * d;
                if (sn < 0)
                {
                    sn = 0;
                    tn = e;
                    td = c;
                }
                else if (sn > sd)
                {
                    sn = sd;
                    tn = e + b;
                    td = c;
                }
            }
            if (tn < 0)
            {
                tn = 0;
                if (-d < 0)
                {
                    sn = 0;
                }
                else if (-d > a)
                {
                    sn = sd;
                }
                else
                {
                    sn = -d;
                    sd = a;
                }
            }
            else if (tn > td)
            {
                tn = td;
                if ((-d + b) < 0)
                {
                    sn = 0;
                }
                else if ((-d + b) > a)
                {
                    sn = sd;
                }
                else
                {
                    sn = -d + b;
                    sd = a;
                }
            }
            FP sc = 0;
            FP tc = 0;
            if (sn <= FP.EN7)
            {
                sc = 0;
            }
            else
            {
                sc = sn / sd;
            }

            if (tn <= FP.EN7)
            {
                tc = 0;
            }
            else
            {
                tc = tn / td;
            }

            FP dx = wx + (sc * ux) - (tc * vx);
            FP dy = wy + (sc * uy) - (tc * vy);
            FP dz = wz + (sc * uz) - (tc * vz);
            return dx * dx + dy * dy + dz * dz;
        }


    }
}