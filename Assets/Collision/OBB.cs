namespace TrueSync
{
    public struct OBB
    {
        public OBB(TSVector center, TSVector size, TSQuaternion quaternion)
        {
            this.center = center;
            extents = size * 0.5;
            m_Quaternion = TSQuaternion.identity;
            AxisX = TSVector.zero;
            AxisY = TSVector.zero;
            AxisZ = TSVector.zero;
            SetQuaternion(quaternion);
        }

        public TSVector center { get; set; }
        public TSVector AxisX { get; private set; }
        public TSVector AxisY { get; private set; }
        public TSVector AxisZ { get; private set; }

        //The extents of the Bounding Box. This is always half of the size of the OBB.
        public TSVector extents { get; set; }

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

        public static bool operator ==(OBB a, OBB b)
        {
            return a.center == b.center && a.extents == b.extents;
        }

        public static bool operator !=(OBB a, OBB b)
        {
            return a.center != b.center || a.extents != b.extents;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is OBB)) return false;
            OBB other = (OBB)obj;
            return center == other.center && extents == other.extents;
        }

        public override int GetHashCode()
        {
            return center.GetHashCode() ^ extents.GetHashCode();
        }

        private void SetQuaternion(TSQuaternion quaternion)
        {
            m_Quaternion = quaternion;
            AxisX = quaternion * TSVector.right;
            AxisY = quaternion * TSVector.up;
            AxisZ = quaternion * TSVector.forward;
        }




        /// <summary>
        /// 检查所选轴之间是否有分离平面
        /// </summary>
        /// <returns></returns>
        public static bool GetSeparatingPlane(TSVector RPos, TSVector Plane, OBB box1, OBB box2)
        {
            return (FP.Abs(RPos * Plane) >
                (FP.Abs((box1.AxisX * box1.extents.x) * Plane) +
                FP.Abs((box1.AxisY * box1.extents.y) * Plane) +
                FP.Abs((box1.AxisZ * box1.extents.z) * Plane) +
                FP.Abs((box2.AxisX * box2.extents.x) * Plane) +
                FP.Abs((box2.AxisY * box2.extents.y) * Plane) +
                FP.Abs((box2.AxisZ * box2.extents.z) * Plane)));
        }

        /// <summary>
        /// 在所有15个轴上进行分离平面的测试
        /// </summary>
        /// <param name="box1"></param>
        /// <param name="box2"></param>
        /// <returns></returns>
        public bool Intersects(OBB other)
        {
            TSVector RPos;
            RPos = other.center - this.center;
            return !(GetSeparatingPlane(RPos, this.AxisX, this, other) ||
                GetSeparatingPlane(RPos, this.AxisY, this, other) ||
                GetSeparatingPlane(RPos, this.AxisZ, this, other) ||
                GetSeparatingPlane(RPos, other.AxisX, this, other) ||
                GetSeparatingPlane(RPos, other.AxisY, this, other) ||
                GetSeparatingPlane(RPos, other.AxisZ, this, other) ||
                GetSeparatingPlane(RPos, this.AxisX ^ other.AxisX, this, other) ||
                GetSeparatingPlane(RPos, this.AxisX ^ other.AxisY, this, other) ||
                GetSeparatingPlane(RPos, this.AxisX ^ other.AxisZ, this, other) ||
                GetSeparatingPlane(RPos, this.AxisY ^ other.AxisX, this, other) ||
                GetSeparatingPlane(RPos, this.AxisY ^ other.AxisY, this, other) ||
                GetSeparatingPlane(RPos, this.AxisY ^ other.AxisZ, this, other) ||
                GetSeparatingPlane(RPos, this.AxisZ ^ other.AxisX, this, other) ||
                GetSeparatingPlane(RPos, this.AxisZ ^ other.AxisY, this, other) ||
                GetSeparatingPlane(RPos, this.AxisZ ^ other.AxisZ, this, other));
        }

        public bool Intersects(Sphere s)
        {
            TSVector p = TSVector.zero;
            return Intersects(s, this, ref p);
        }

        public bool Intersects(Sphere s, ref TSVector point)
        {
            return Intersects(s, this, ref point);
        }

        /// <summary>
        /// 测试球与OBB相交，并取最近点
        /// </summary>
        public static bool Intersects(Sphere s, OBB b, ref TSVector p)
        {
            p = b.ClosestPointOBB(s.c);
            TSVector v = p - s.c;
            return TSVector.Dot(v, v) <= s.r * s.r;
        }

        /// <summary>
        /// 点到OBB的最近点
        /// </summary>
        public TSVector ClosestPointOBB(TSVector p)
        {
            TSVector d = p - this.center;
            TSVector q = this.center;
            FP distX = TSVector.Dot(d, this.AxisX);       // dist为x轴中p到中心点的距离，由x=(P-C)·Ux得来
            if (distX > this.extents.x) distX = this.extents.x;           // 距离超过边界时，为Othisthis范围的一半
            if (distX < -this.extents.x) distX = -this.extents.x;
            q += distX * this.AxisX;

            FP distY = TSVector.Dot(d, this.AxisY);
            if (distY > this.extents.y) distY = this.extents.y;
            if (distY < -this.extents.y) distY = -this.extents.y;
            q += distY * this.AxisY;

            FP distZ = TSVector.Dot(d, this.AxisZ);
            if (distZ > this.extents.z) distZ = this.extents.z;
            if (distZ < -this.extents.z) distZ = -this.extents.z;
            q += distZ * this.AxisZ;

            return q;
        }





    }
}