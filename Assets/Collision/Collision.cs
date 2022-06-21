using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TrueSync
{

    public static class Collision
    {
        private static List<BaseCollider> PotentialCollision = new List<BaseCollider>(32);
        private static List<BVHNode<BaseCollider>> PotentialCollisionBVH = new List<BVHNode<BaseCollider>>(32);

        public static void OverlapBoxNonAlloc(ref List<BaseCollider> colliders, World world, TSVector center, TSVector size, TSQuaternion quaternion)
        {
            PotentialCollision.Clear();
            PotentialCollisionBVH.Clear();
            colliders.Clear();
            BoxShape box = new BoxShape(center, size, quaternion);
            if (world.spaceType == SpaceType.OCtree)
            {
                world.octree.GetColliding(PotentialCollision, box.ToAABB());
            }
            else if (world.spaceType == SpaceType.BVH)
            {
                world.bvh.Traverse(box.ToAABB(), ref PotentialCollisionBVH);
                for (int b = 0; b < PotentialCollisionBVH.Count; b++)
                {
                    if (PotentialCollisionBVH[b].GObjects != null)
                    {
                        for (int g = 0; g < PotentialCollisionBVH[b].GObjects.Count; g++)
                        {
                            PotentialCollision.Add(PotentialCollisionBVH[b].GObjects[g]);
                        }
                    }
                }
            }
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
            PotentialCollisionBVH.Clear();
            colliders.Clear();
            SphereShape sphere = new SphereShape(center, radius);
            if (world.spaceType == SpaceType.OCtree)
            {
                world.octree.GetColliding(PotentialCollision, sphere.ToAABB());
            }
            else if (world.spaceType == SpaceType.BVH)
            {
                world.bvh.Traverse(sphere.ToAABB(), ref PotentialCollisionBVH);
                for (int b = 0; b < PotentialCollisionBVH.Count; b++)
                {
                    if (PotentialCollisionBVH[b].GObjects != null)
                    {
                        for (int g = 0; g < PotentialCollisionBVH[b].GObjects.Count; g++)
                        {
                            PotentialCollision.Add(PotentialCollisionBVH[b].GObjects[g]);
                        }
                    }
                }
            }


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