using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TrueSync
{

    public abstract class Shape
    {
        public ICollider collider;

        public abstract bool Intersects(Shape other);

        public abstract bool Intersects(AABB aabb);

        public abstract AABB ToAABB();
    }
}