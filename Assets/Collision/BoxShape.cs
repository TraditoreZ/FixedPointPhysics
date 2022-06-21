using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrueSync
{

    public class BoxShape : Shape
    {
        private OBB obb;

        public TSVector center { get { return obb.center; } set { obb.center = value; } }

        public TSVector size { get { return obb.extents * 2; } set { obb.extents = value * FP.Half; } }

        public TSQuaternion quaternion { get { return obb.Quaternion; } set { obb.Quaternion = value; } }

        public OBB _OBB { get { return obb; } }

        public BoxShape(TSVector center, TSVector size, TSQuaternion quaternion)
        {
            obb = new OBB(center, size, quaternion);
        }

        public override bool Intersects(Shape other)
        {
            switch (other)
            {
                case BoxShape box:
                    return _OBB.Intersects(box._OBB);
                case SphereShape sphereShape:
                    return _OBB.Intersects(sphereShape._Sphere);
            }
            return false;
        }

        public override bool Intersects(AABB aabb)
        {
            return aabb.Intersects(_OBB);
        }

        private TSVector[] aabbArray = new TSVector[8];
        public override AABB ToAABB()
        {
            TSVector x = obb.AxisX * obb.extents.x;
            TSVector y = obb.AxisY * obb.extents.y;
            TSVector z = obb.AxisZ * obb.extents.z;

            aabbArray[0] = obb.center + x + y + z;
            aabbArray[1] = obb.center - x + y + z;
            aabbArray[2] = obb.center + x - y + z;
            aabbArray[3] = obb.center + x + y - z;
            aabbArray[4] = obb.center + x - y - z;
            aabbArray[5] = obb.center - x - y + z;
            aabbArray[6] = obb.center - x + y - z;
            aabbArray[7] = obb.center - x - y - z;

            FP minX = FP.MaxValue;
            FP minY = FP.MaxValue;
            FP minZ = FP.MaxValue;
            FP maxX = FP.MinValue;
            FP maxY = FP.MinValue;
            FP maxZ = FP.MinValue;
            for (int i = 0; i < 8; i++)
            {
                minX = FP.Min(minX, aabbArray[i].x);
                minY = FP.Min(minY, aabbArray[i].y);
                minZ = FP.Min(minZ, aabbArray[i].z);
                maxX = FP.Max(maxX, aabbArray[i].x);
                maxY = FP.Max(maxY, aabbArray[i].y);
                maxZ = FP.Max(maxZ, aabbArray[i].z);
            }
            var aabb = new AABB();
            aabb.SetMinMax(new TSVector(minX, minY, minZ), new TSVector(maxX, maxY, maxZ));
            return aabb;

            // 这里并非是一个精准的AABB包围盒 但是算的比较快mm 
            // FP p = size.x > size.y ? size.x : size.y;
            // p = p > size.z ? p : size.z;
            // return new AABB(center, TSVector.one * p);
        }
    }
}