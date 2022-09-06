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


        public BVH<BaseCollider> bvh { get; set; }

        private List<BaseCollider> PotentialCollision;

        private List<BVHNode<BaseCollider>> PotentialCollisionBVH;


        public World(FP worldSize)
        {
            colliders = new List<BaseCollider>(128);
            PotentialCollision = new List<BaseCollider>(128);
            PotentialCollisionBVH = new List<BVHNode<BaseCollider>>(128);
            needRemove = new List<BaseCollider>();
            this.worldSize = worldSize;
            bvh = new BVH<BaseCollider>(new BVHBaseColliderAdapter(), colliders);
            InitDefaultLayerCollisionMatrix();
        }


        public bool AddCollider(BaseCollider collider)
        {
            if (colliders.Contains(collider))
            {
                Debug.LogWarning("repeat add collider");
                return false;
            }
            colliders.Add(collider);
            bvh.Add(collider);
            if (collider.layer > 32)
            {
                Debug.LogError("collider layer > 32;  owner:" + collider.owner.ToString());
                return false;
            }
            collider.SetLayerCollisionMatrix(CollisionLayer.layerCollisionMatrix[collider.layer]);
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
                    bvh.MarkForUpdate(colliders[i]);
                }
            }
            bvh.Optimize();
            if (bvh.rootBVH.Box.min.x < -worldSize || bvh.rootBVH.Box.min.y < -worldSize || bvh.rootBVH.Box.min.z < -worldSize ||
                bvh.rootBVH.Box.max.x > worldSize || bvh.rootBVH.Box.max.y > worldSize || bvh.rootBVH.Box.max.z > worldSize)
            {
                Debug.Log($"Physics worldSize expand:{worldSize} => {worldSize * 2}");
                worldSize *= 2;
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
                    // 从BVH空间查找那些潜在的碰撞对象
                    PotentialCollision.Clear();
                    PotentialCollisionBVH.Clear();
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

                    // 与潜在对象做碰撞检测
                    for (int k = 0; k < PotentialCollision.Count; k++)
                    {
                        if (CollisionLayer.GetLayerByMatrix(colliders[i].layerCollisionMatrix, PotentialCollision[k].layer) == false)
                            continue;
                        if (colliders[i].owner != PotentialCollision[k].owner && PotentialCollision[k].enable &&
                         colliders[i].bounds.Intersects(PotentialCollision[k].bounds) &&
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


        private void InitDefaultLayerCollisionMatrix()
        {
            CollisionLayer.SetLayer(0, 0, true);
        }

    }
}