using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TrueSync
{
    public class CapsuleShape : Shape
    {
        private Capsule capsule;

        public TSVector center { get { return capsule.center; } set { capsule.center = value; } }

        public FP radius { get { return capsule.radius; } set { capsule.radius = value; } }

        public FP height { get { return capsule.height; } set { capsule.height = value; } }

        public TSQuaternion quaternion { get { return capsule.Quaternion; } set { capsule.Quaternion = value; } }

        public Capsule _Capsule { get { return capsule; } }

        public CapsuleShape(TSVector center, FP radius, FP height, TSQuaternion quaternion)
        {
            capsule = new Capsule(center, radius, height, quaternion);
        }

        public override bool Intersects(Shape other)
        {
            switch (other)
            {
                case BoxShape box:
                    return Capsule.Intersects(_Capsule, box._OBB);
                case SphereShape sphereShape:
                    return Capsule.Intersects(_Capsule, sphereShape._Sphere);
                case CapsuleShape capsuleShape:
                    return Capsule.Intersects(capsuleShape._Capsule, _Capsule);
            }
            return false;
        }

        public override bool Intersects(AABB aabb)
        {
            return aabb.Intersects(capsule);
        }

        private static TSVector[] aabbArray = new TSVector[8];
        public override AABB ToAABB()
        {
            TSVector x = capsule.AxisX * capsule.radius;
            TSVector y = capsule.AxisY * capsule.height * FP.Half;
            TSVector z = capsule.AxisZ * capsule.radius;

            aabbArray[0] = capsule.center + x + y + z;
            aabbArray[1] = capsule.center - x + y + z;
            aabbArray[2] = capsule.center + x - y + z;
            aabbArray[3] = capsule.center + x + y - z;
            aabbArray[4] = capsule.center + x - y - z;
            aabbArray[5] = capsule.center - x - y + z;
            aabbArray[6] = capsule.center - x + y - z;
            aabbArray[7] = capsule.center - x - y - z;

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
        }
    }
}