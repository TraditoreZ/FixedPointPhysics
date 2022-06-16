using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TrueSync
{

    public static class Collision
    {
        private static List<BaseCollider> PotentialCollision = new List<BaseCollider>(32);


        public static void OverlapBoxNonAlloc(ref List<BaseCollider> colliders, World world, TSVector center, TSVector size, TSQuaternion quaternion)
        {
            PotentialCollision.Clear();
            colliders.Clear();
            BoxShape box = new BoxShape(center, size, quaternion);
            world.octree.GetColliding(PotentialCollision, box.ToAABB());
            for (int i = 0; i < PotentialCollision.Count; i++)
            {
                if (box.Intersects(PotentialCollision[i].shape))
                {
                    colliders.Add(PotentialCollision[i]);
                }
            }
        }



        public static void OverlapSphereNonAlloc(ref List<BaseCollider> colliders, World world, TSVector center, FP radius)
        {
            PotentialCollision.Clear();
            colliders.Clear();
            SphereShape sphere = new SphereShape(center, radius);
            world.octree.GetColliding(PotentialCollision, sphere.ToAABB());
            for (int i = 0; i < PotentialCollision.Count; i++)
            {
                if (sphere.Intersects(PotentialCollision[i].shape))
                {
                    colliders.Add(PotentialCollision[i]);
                }
            }
        }

    }
}