using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrueSync
{
    public struct Sphere
    {
        public Sphere(TSVector center, FP radius)
        {
            c = center;
            r = radius;
        }

        public TSVector c;
        public FP r;


        public static bool operator ==(Sphere a, Sphere b)
        {
            return a.c == b.c && a.r == b.r;
        }

        public static bool operator !=(Sphere a, Sphere b)
        {
            return a.c != b.c || a.r != b.r;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Sphere)) return false;
            Sphere other = (Sphere)obj;
            return c == other.c && r == other.r;
        }

        public override int GetHashCode()
        {
            return c.GetHashCode() ^ r.GetHashCode();
        }

        public bool Intersects(Sphere other)
        {
            TSVector d = this.c - other.c;
            FP dist2 = TSVector.Dot(d, d); // 点积的妙用，同一个向量时，cos0=1,结果|a||b|为长度的平方
            FP rSum = this.r + other.r;
            return dist2 <= rSum * rSum;
        }

        public bool Intersects(AABB other)
        {
            // 获取圆中心点到AABB的距离平方
            FP dist2 = AABB.SqDistPointAABB(this.c, other);
            return dist2 <= this.r * this.r;
        }
    }

}
