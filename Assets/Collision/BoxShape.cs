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

        public override AABB ToAABB()
        {
            // 这里并非是一个精准的AABB包围盒 但是算的比较快
            FP p = size.x > size.y ? size.x :size.y;
            p = p > size.z ? p : size.z;
            return new AABB(center, TSVector.one * p);
        }
    }
}