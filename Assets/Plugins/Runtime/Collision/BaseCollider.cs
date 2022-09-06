using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TrueSync
{

    public abstract class BaseCollider : ICollider
    {
        public event ICollider.ColliderDelegate OnEnterEvent;
        public event ICollider.ColliderDelegate OnStayEvent;
        public event ICollider.ColliderDelegate OnLeaveEvent;

        public bool enable { get; set; }
        public bool dirty { get; protected set; }
        public object owner { get; set; }
        /// <summary>层级 最大支持到32</summary>
        public byte layer;
        public int layerCollisionMatrix { get; private set; }
        /// <summary>刚体:  两个物体发生碰撞必须其中一个为刚体</summary>
        public bool rigidBody { get; set; }

        public Shape shape { get; protected set; }

        public AABB bounds { get; protected set; }

        protected List<BaseCollider> enterCollider;
        protected List<BaseCollider> stayCollider;
        protected List<BaseCollider> leaveCollider;
        protected List<BaseCollider> colliding;



        protected List<BaseCollider> enterEventCollider;
        protected List<BaseCollider> stayEventCollider;
        protected List<BaseCollider> leaveEventCollider;


        public BaseCollider()
        {
            enterCollider = new List<BaseCollider>();
            stayCollider = new List<BaseCollider>();
            leaveCollider = new List<BaseCollider>();
            enterEventCollider = new List<BaseCollider>();
            stayEventCollider = new List<BaseCollider>();
            leaveEventCollider = new List<BaseCollider>();
            colliding = new List<BaseCollider>();
            dirty = true;
            enable = true;
        }

        protected abstract AABB GetAABB();

        public void ClearDirty() { dirty = false; }

        public void Destroy()
        {
            enterCollider.Clear();
            stayCollider.Clear();
            leaveCollider.Clear();
            enterEventCollider.Clear();
            stayEventCollider.Clear();
            leaveEventCollider.Clear();
            colliding.Clear();
            OnEnterEvent = null;
            OnStayEvent = null;
            OnLeaveEvent = null;
        }

        public void ColliderTick()
        {
            // stay
            for (int i = 0; i < enterCollider.Count; i++)
            {
                stayCollider.Add(enterCollider[i]);
            }

            //enter
            enterCollider.Clear();
            for (int i = 0; i < colliding.Count; i++)
            {
                if (stayCollider.Contains(colliding[i]) == false)
                {
                    enterCollider.Add(colliding[i]);
                }
            }

            //leave
            leaveCollider.Clear();
            for (int i = 0; i < stayCollider.Count; i++)
            {
                if (colliding.Contains(stayCollider[i]) == false)
                {
                    leaveCollider.Add(stayCollider[i]);
                }
            }
            for (int i = 0; i < leaveCollider.Count; i++)
            {
                stayCollider.Remove(leaveCollider[i]);
            }
            colliding.Clear();

            CopyList<BaseCollider>(enterCollider, enterEventCollider);
            CopyList<BaseCollider>(stayCollider, stayEventCollider);
            CopyList<BaseCollider>(leaveCollider, leaveEventCollider);
        }

        public void SendEventTick()
        {
            if (enterEventCollider.Count > 0)
            {
                OnEnterEvent?.Invoke(owner, enterEventCollider);
            }
            if (stayEventCollider.Count > 0)
            {
                OnStayEvent?.Invoke(owner, stayEventCollider);
            }
            if (leaveEventCollider.Count > 0)
            {
                OnLeaveEvent?.Invoke(owner, leaveEventCollider);
            }
        }

        public List<BaseCollider> GetCollidingList()
        {
            return colliding;
        }

        private void CopyList<T>(List<T> source, List<T> target)
        {
            target.Clear();
            for (int i = 0; i < source.Count; i++)
            {
                target.Add(source[i]);
            }
        }

        public int CollisionCount()
        {
            return stayCollider.Count;
        }

        public void SetLayerCollisionMatrix(int value)
        {
            layerCollisionMatrix = value;
        }
    }
}