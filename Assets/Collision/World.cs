using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrueSync
{
    public enum SpaceType
    {
        OCtree,
        BVH
    }

    public class World
    {
        public FP worldSize { get; private set; }

        public SpaceType spaceType { get; private set; }

        public List<BaseCollider> colliders;

        private List<BaseCollider> needRemove;

        public AABBOctree<BaseCollider> octree { get; set; }

        public BVH<BaseCollider> bvh { get; set; }

        private List<BaseCollider> PotentialCollision;

        private List<BVHNode<BaseCollider>> PotentialCollisionBVH;

        public World(FP worldSize, SpaceType spaceType = SpaceType.OCtree)
        {
            colliders = new List<BaseCollider>(128);
            PotentialCollision = new List<BaseCollider>(128);
            PotentialCollisionBVH = new List<BVHNode<BaseCollider>>(128);
            needRemove = new List<BaseCollider>();
            this.worldSize = worldSize;
            this.spaceType = spaceType;
            if (spaceType == SpaceType.OCtree)
                octree = new AABBOctree<BaseCollider>(worldSize, TSVector.zero, 1, 1.25f);
            else if (spaceType == SpaceType.BVH)
                bvh = new BVH<BaseCollider>(new BVHBaseColliderAdapter(), colliders);
        }


        public bool AddCollider(BaseCollider collider)
        {
            if (colliders.Contains(collider))
            {
                Debug.LogWarning("repeat add collider");
                return false;
            }
            colliders.Add(collider);
            if (spaceType == SpaceType.OCtree)
                octree.Add(collider, collider.bounds);
            else if (spaceType == SpaceType.BVH)
                bvh.Add(collider);
            return true;
        }

        public void RemoveCollider(BaseCollider collider)
        {
            needRemove.Add(collider);
        }

        public void Tick()
        {
            SpaceUpdate();
            CollectionsTick();
            RemoveColliderInList();
            ClearDirty();
        }


        public void GetColliders(ref List<BaseCollider> list)
        {
            list.Clear();
            for (int i = 0; i < colliders.Count; i++)
                list.Add(colliders[i]);
        }

        private void RemoveColliderInList()
        {
            for (int i = 0; i < needRemove.Count; i++)
            {
                colliders.Remove(needRemove[i]);
                if (spaceType == SpaceType.OCtree)
                    octree.Remove(needRemove[i]);
                else if (spaceType == SpaceType.BVH)
                    bvh.Remove(needRemove[i]);
            }
            needRemove.Clear();
        }

        private void SpaceUpdate()
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i].enable && colliders[i].dirty)
                {
                    if (spaceType == SpaceType.OCtree)
                    {
                        octree.Remove(colliders[i]);
                        octree.Add(colliders[i], colliders[i].bounds);
                    }
                    else if (spaceType == SpaceType.BVH)
                    {
                        bvh.MarkForUpdate(colliders[i]);
                    }
                }
            }
            if (spaceType == SpaceType.BVH)
            {
                bvh.Optimize();
            }
        }

        private void CollectionsTick()
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i].enable == false)
                {
                    continue;
                }
                if (colliders[i].rigidBody)
                {
                    // 从八叉树查找那些潜在的碰撞对象
                    PotentialCollision.Clear();
                    PotentialCollisionBVH.Clear();
                    if (spaceType == SpaceType.OCtree)
                    {
                        octree.GetColliding(PotentialCollision, colliders[i].bounds);
                    }
                    else if (spaceType == SpaceType.BVH)
                    {
                        bvh.Traverse(colliders[i].bounds, ref PotentialCollisionBVH);
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

                    // 与潜在对象做碰撞检测
                    for (int k = 0; k < PotentialCollision.Count; k++)
                    {
                        // TODO 临时写一个同层不检测 到时候应该根据碰撞层级去拆分
                        if (colliders[i].layer != 0 && colliders[i].layer == PotentialCollision[k].layer)
                            continue;
                        if (colliders[i].owner != PotentialCollision[k].owner && PotentialCollision[k].enable &&
                         colliders[i].shape.Intersects(PotentialCollision[k].shape))
                        {
                            ColliderTarget(colliders[i], PotentialCollision[k]);
                        }
                    }
                }
                colliders[i].ColliderTick();
                colliders[i].SendEventTick();
            }
        }


        private void ClearDirty()
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                colliders[i].ClearDirty();
            }
        }


        private void ColliderTarget(BaseCollider self, BaseCollider target)
        {
            // 双方加入彼此碰撞列表
            if (self.GetCollidingList().Contains(target) == false)
            {
                self.GetCollidingList().Add(target);
            }
            if (target.GetCollidingList().Contains(self) == false)
            {
                target.GetCollidingList().Add(self);
            }
        }

    }
}