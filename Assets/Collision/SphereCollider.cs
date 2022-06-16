using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TrueSync
{
    public class SphereCollider : BaseCollider
    {

        public TSVector center
        {
            get { return GetSphereShape().center; }
            set
            {
                if (GetSphereShape().center != value)
                {
                    GetSphereShape().center = value;
                    bounds = GetAABB();
                    dirty = true;
                }
            }
        }

        public FP radius
        {
            get { return GetSphereShape().radius; }
            set
            {
                if (GetSphereShape().radius != value)
                {
                    GetSphereShape().radius = value;
                    bounds = GetAABB();
                    dirty = true;
                }
            }
        }


        public SphereCollider(TSVector center, FP radius) : base()
        {
            shape = new SphereShape(center, radius);
            bounds = GetAABB();
        }

        public SphereShape GetSphereShape()
        {
            return shape as SphereShape;
        }

        protected override AABB GetAABB()
        {
            return shape.ToAABB();
        }
    }
}