using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrueSync
{

    public class SphereShape : Shape
    {
        private Sphere sphere;

        public TSVector center { get { return sphere.c; } set { sphere.c = value; } }

        public FP radius { get { return sphere.r; } set { sphere.r = value; } }

        public Sphere _Sphere { get { return sphere; } }

        public SphereShape(TSVector center, FP radius)
        {
            sphere = new Sphere(center, radius);
        }

        public override bool Intersects(Shape other)
        {
            switch (other)
            {
                case BoxShape box:
                    return box._OBB.Intersects(_Sphere);
                case SphereShape sphereShape:
                    return _Sphere.Intersects(sphereShape._Sphere);
            }
            return false;
        }

        public override bool Intersects(AABB aabb)
        {
            return aabb.Intersects(_Sphere);
        }

        public override AABB ToAABB()
        {
            return new AABB(center, TSVector.one * radius * 2);
        }
    }
}