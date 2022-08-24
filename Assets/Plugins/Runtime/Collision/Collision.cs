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
            BoxShape box = new BoxShape(center, size, quaternion);
            OverlapShapeNonAlloc(ref colliders, world, box);
        }


        public static void OverlapSphereNonAlloc(ref List<BaseCollider> colliders, World world, TSVector center, FP radius)
        {
            SphereShape sphere = new SphereShape(center, radius);
            OverlapShapeNonAlloc(ref colliders, world, sphere);
        }


        public static void OverlapCapsuleNonAlloc(ref List<BaseCollider> colliders, World world, TSVector center, FP radius, FP height, TSQuaternion quaternion)
        {
            CapsuleShape capsule = new CapsuleShape(center, radius, height, quaternion);
            OverlapShapeNonAlloc(ref colliders, world, capsule);
        }


        public static void OverlapShapeNonAlloc(ref List<BaseCollider> colliders, World world, Shape shape)
        {
            PotentialCollision.Clear();
            PotentialCollisionBVH.Clear();
            colliders.Clear();
            world.bvh.Traverse(shape.ToAABB(), ref PotentialCollisionBVH);
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
            for (int i = 0; i < PotentialCollision.Count; i++)
            {
                if (shape.Intersects(PotentialCollision[i].shape))
                {
                    colliders.Add(PotentialCollision[i]);
                }
            }
        }

    }
}