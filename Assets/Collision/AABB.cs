namespace TrueSync
{
    public struct AABB
    {
        public AABB(TSVector center, TSVector size)
        {
            m_center = center;
            m_size = size;
            m_min = new TSVector(center.x - size.x * 0.5f, center.y - size.y * 0.5f, center.z - size.z * 0.5f);
            m_max = new TSVector(center.x + size.x * 0.5f, center.y + size.y * 0.5f, center.z + size.z * 0.5f);
        }

        private TSVector m_min;
        private TSVector m_max;
        private TSVector m_center;
        private TSVector m_size;

        public TSVector min
        {
            get
            {
                return m_min;
            }
            set
            {
                SetMinMax(value, max);
            }
        }

        public TSVector max
        {
            get
            {
                return m_max;
            }
            set
            {
                SetMinMax(min, value);
            }
        }

        public TSVector center
        {
            get
            {
                return m_center;
            }
            set
            {
                m_center = value;
                m_min = center - extents;
                m_max = center + extents;
            }
        }

        //The extents of the Bounding Box. This is always half of the size of the AABB.
        public TSVector extents
        {
            get { return size * 0.5f; }
            set { m_size = value * 2; }
        }

        public TSVector size
        {
            get
            {
                return m_size;
            }
            set
            {
                m_size = value;
                m_min = center - extents;
                m_max = center + extents;
            }
        }

        public static bool operator ==(AABB a, AABB b)
        {
            return a.min == b.min && a.max == b.max;
        }

        public static bool operator !=(AABB a, AABB b)
        {
            return a.min != b.min || a.max != b.max;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AABB)) return false;
            AABB other = (AABB)obj;
            return min == other.min && max == other.max;
        }

        public override int GetHashCode()
        {
            return min.GetHashCode() ^ max.GetHashCode();
        }

        public bool Intersects(AABB aabb)
        {
            if (aabb.max.x < this.min.x || aabb.min.x > this.max.x)
                return false;
            if (aabb.max.y < this.min.y || aabb.min.y > this.max.y)
                return false;
            if (aabb.max.z < this.min.z || aabb.min.z > this.max.z)
                return false;
            return true;
        }

        public bool Intersects(OBB obb)
        {
            // 拿到AABB中心点到OBB最近的那个点
            TSVector point = obb.ClosestPointOBB(this.center);
            TSVector v = TSVector.Abs(point - this.center);
            return (v.x <= extents.x && v.y <= extents.y && v.z <= extents.z);
        }

        public bool Intersects(Sphere other)
        {
            // 获取圆中心点到AABB的距离平方
            FP dist2 = SqDistPointAABB(other.c, this);
            return dist2 <= other.r * other.r;
        }

        public bool Intersects(Capsule capsule)
        {
            var point = capsule.ClosestPointCapsule(this.center);
            TSVector v = TSVector.Abs(point - this.center);
            return (v.x <= extents.x && v.y <= extents.y && v.z <= extents.z);
        }

        public void SetMinMax(TSVector min, TSVector max)
        {
            m_min = min;
            m_max = max;
            m_center = (min + max) * 0.5f;
            m_size = TSVector.Abs(min - max);
        }

        public bool Contains(TSVector point)
        {
            return min.x < point.x && min.y < point.y && min.z < point.z && max.x > point.x && max.y > point.y && max.z > point.z;
        }

        public static AABB ToAABB(Sphere sphere)
        {
            return new AABB(sphere.c, TSVector.one * sphere.r * 2);
        }

        public static FP SqDistPointAABB(TSVector p, AABB b)
        {
            FP sqDist = 0.0f;
            FP x = p.x;
            if (x < b.min.x) sqDist += (b.min.x - x) * (b.min.x - x);  // 点在左，取距离公式中x的平方
            if (x > b.max.x) sqDist += (x - b.max.x) * (x - b.max.x);

            FP y = p.y;
            if (y < b.min.y) sqDist += (b.min.y - y) * (b.min.y - y);
            if (y > b.max.y) sqDist += (y - b.max.y) * (y - b.max.y);

            FP z = p.z;
            if (z < b.min.z) sqDist += (b.min.z - z) * (b.min.z - z);
            if (z > b.max.z) sqDist += (z - b.max.z) * (z - b.max.z);

            return sqDist;
        }

        public void Encapsulate(AABB aabb)
        {
            TSVector min = TSVector.Min(this.min, aabb.min);
            TSVector max = TSVector.Max(this.max, aabb.max);
            SetMinMax(min, max);
        }

    }
}