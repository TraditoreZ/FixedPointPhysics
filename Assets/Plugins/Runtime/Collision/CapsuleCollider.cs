using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TrueSync
{
    public class CapsuleCollider : BaseCollider
    {
        public TSVector center
        {
            get { return GetCapsuleShape().center; }
            set
            {
                if (GetCapsuleShape().center != value)
                {
                    GetCapsuleShape().center = value;
                    bounds = GetAABB();
                    dirty = true;
                }
            }
        }

        public FP radius
        {
            get { return GetCapsuleShape().radius; }
            set
            {
                if (GetCapsuleShape().radius != value)
                {
                    GetCapsuleShape().radius = value;
                    bounds = GetAABB();
                    dirty = true;
                }
            }
        }

        public FP height
        {
            get { return GetCapsuleShape().height; }
            set
            {
                if (GetCapsuleShape().height != value)
                {
                    GetCapsuleShape().height = value;
                    bounds = GetAABB();
                    dirty = true;
                }
            }
        }

        public TSQuaternion quaternion
        {
            get { return GetCapsuleShape().quaternion; }
            set
            {
                if (GetCapsuleShape().quaternion != value)
                {
                    GetCapsuleShape().quaternion = value;
                    bounds = GetAABB();
                    dirty = true;
                }
            }
        }

        public CapsuleCollider(TSVector center, FP radius, FP height, TSQuaternion quaternion) : base()
        {
            shape = new CapsuleShape(center, radius, height, quaternion);
            bounds = GetAABB();
        }

        private CapsuleShape GetCapsuleShape()
        {
            return shape as CapsuleShape;
        }

        protected override AABB GetAABB()
        {
            return shape.ToAABB();
        }
    }
}