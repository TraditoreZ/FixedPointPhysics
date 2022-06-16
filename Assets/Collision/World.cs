using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrueSync
{
    public class World
    {
        public FP worldSize { get; private set; }

        public List<BaseCollider> colliders;

        private List<BaseCollider> needRemove;

        public AABBOctree<BaseCollider> octree { get; set; }

        private List<BaseCollider> PotentialCollision;

        public World(FP worldSize)
        {
            colliders = new List<BaseCollider>(256);
            PotentialCollision = new List<BaseCollider>(256);
            needRemove = new List<BaseCollider>();
            this.worldSize = worldSize;
            octree = new AABBOctree<BaseCollider>(worldSize, TSVector.zero, 1, 1.25f);
        }


        public bool AddCollider(BaseCollider collider)
        {
            if (colliders.Contains(collider))
            {
                Debug.LogWarning("repeat add collider");
                return false;
            }
            colliders.Add(collider);
            return true;
        }

        public void RemoveCollider(BaseCollider collider)
        {
            needRemove.Add(collider);
        }

        public void Tick()
        {
            OctreeUpdate();
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
            }
            needRemove.Clear();
        }

        private void OctreeUpdate()
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i].dirty)
                {
                    octree.Remove(colliders[i]);
                    octree.Add(colliders[i], colliders[i].bounds);
                }
            }
        }

        private void CollectionsTick()
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                // 从八叉树查找那些潜在的碰撞对象
                PotentialCollision.Clear();
                octree.GetColliding(PotentialCollision, colliders[i].bounds);

                // 与潜在对象做碰撞检测
                for (int k = 0; k < PotentialCollision.Count; k++)
                {
                    if (colliders[i].owner != PotentialCollision[k].owner && colliders[i].shape.Intersects(PotentialCollision[k].shape))
                    {
                        // Debug.Log("发生碰撞:" + colliders[i].owner + "  " + PotentialCollision[k].owner);
                        ColliderTarget(colliders[i], PotentialCollision[k]);
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


        private void ColliderTarget(BaseCollider self, BaseCollider targer)
        {
            // 双方加入彼此碰撞列表
            if (self.GetCollidingList().Contains(targer) == false)
            {
                self.GetCollidingList().Add(targer);
            }
            if (targer.GetCollidingList().Contains(self) == false)
            {
                targer.GetCollidingList().Add(self);
            }
        }

    }
}