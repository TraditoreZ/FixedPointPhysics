using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrueSync
{
    public class BoxCollider : BaseCollider
    {
        public TSVector center
        {
            get { return GetBoxShape().center; }
            set
            {
                if (GetBoxShape().center != value)
                {
                    GetBoxShape().center = value;
                    bounds = GetAABB();
                    dirty = true;
                }
            }
        }

        public TSVector size
        {
            get { return GetBoxShape().size; }
            set
            {
                if (GetBoxShape().size != value)
                {
                    GetBoxShape().size = value;
                    bounds = GetAABB();
                    dirty = true;
                }
            }
        }

        public TSQuaternion quaternion
        {
            get { return GetBoxShape().quaternion; }
            set
            {
                if (GetBoxShape().quaternion != value)
                {
                    GetBoxShape().quaternion = value;
                    bounds = GetAABB();
                    dirty = true;
                }
            }
        }

        public BoxCollider(TSVector center, TSVector size, TSQuaternion quaternion) : base()
        {
            shape = new BoxShape(center, size, quaternion);
            bounds = GetAABB();
        }

        private BoxShape GetBoxShape()
        {
            return shape as BoxShape;
        }

        protected override AABB GetAABB()
        {
            return shape.ToAABB();
        }
    }
}