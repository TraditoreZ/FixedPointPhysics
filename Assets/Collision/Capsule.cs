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
        /// ????????????????????????
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
            TSVector startA = a.center + a.AxisY * ((a.height) * FP.HALF - a.radius);
            TSVector endA = a.center - a.AxisY * ((a.height) * FP.HALF - a.radius);
            TSVector startB = b.center + b.AxisY * ((b.height) * FP.HALF - b.radius);
            TSVector endB = b.center - b.AxisY * ((b.height) * FP.HALF - b.radius);
            return sqDistSegmentToSegment(startA, endA, startB, endB) <= (a.radius + b.radius) * (a.radius + b.radius);
        }


        public FP DistPointCapsule(TSVector p)
        {
            return FP.Sqrt(sqDistPointCapsule(p));
        }

        /// <summary>
        /// ??????????????????????????????
        /// </summary>
        /// <param name="p">???</param>
        /// <returns>?????????????????????</returns>
        public FP sqDistPointCapsule(TSVector p)
        {
            if (height <= radius * 2)
                return TSVector.Distance(center, p);
            TSVector start = center + AxisY * ((height) * FP.HALF - radius);
            TSVector end = center - AxisY * ((height) * FP.HALF - radius);
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
        ///  ??????????????????????????????
        /// </summary>
        /// <param name="p">???</param>
        /// <returns>????????????????????????</returns>
        public TSVector ClosestPointCapsule(TSVector p)
        {
            if (height <= radius * 2)
                return center;
            TSVector start = center + AxisY * ((height) * FP.HALF - radius);
            TSVector end = center - AxisY * ((height) * FP.HALF - radius);
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
        /// ????????????????????????????????????
        /// </summary>
        /// <param name="startA"></param>
        /// <param name="endA"></param>
        /// <param name="startB"></param>
        /// <param name="endB"></param>
        /// <returns></returns>
        public static FP sqDistSegmentToSegment(TSVector startA, TSVector endA, TSVector startB, TSVector endB)
        {
            //https://segmentfault.com/a/1190000006111226
            // ?????????????????????????????????????????????????????????????????????????????????
            // ??????????????????????????????????????????s???t,?????????s0???t0??????????????????u???v
            // ????????????????????????s1??????s0+sc*u??????t?????????t0+tc*v
            // ?????????w???(s0+sc*u)-(t0+tc*v),?????????w0=s0-t0
            // ???a=u*u???b=u*v???c=v*v???d=u*w0???e=v*w0??????(a)???
            // ??????u*w=???v*w=0??????w=-tc*v+w0+sc*u?????????????????????
            // (u*u)*sc - (u*v)*tc = -u*w0  (??????2)
            // (v*u)*sc - (v*v)*tc = -v*w0  (??????3)
            // ????????????(a)????????????sc=(be-cd)/(ac-b2)???tc=(ae-bd)/(ac-b2)?????????b???
            // ?????????ac-b2=|u|2|v|2-(|u||v|cosq)2=(|u||v|sinq)2?????????0
            // ???????????????????????????b?????????sc???tc?????????sc???tc???1????????????????????????????????????????????????
            // ???ac-b2=0??????(??????2)(??????3)??????????????????????????????????????????sc=0????????????tc
            // ????????????d???L1???L2???=|???P0-Q0)+[(be-cd)*u-(ae-bd)v]/(ac-b2)|
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
                // ???????????????
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