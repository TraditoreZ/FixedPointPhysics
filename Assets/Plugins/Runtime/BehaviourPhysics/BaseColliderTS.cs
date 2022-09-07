using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrueSync.Behaviour
{
    [DefaultExecutionOrder(100)]
    public abstract class BaseColliderTS : MonoBehaviour
    {

        public Vector3 center;

        protected BaseCollider baseCollider;

        public ICollider ICollider { get { return baseCollider; } }

        protected TSVector ColliderCenter
        {
            get
            {
                return new TSVector((FP)transform.position.x + (FP)center.x * (FP)transform.localScale.x, (FP)transform.position.y + (FP)center.y * (FP)transform.localScale.y, (FP)transform.position.z + (FP)center.z * (FP)transform.localScale.z);
            }
        }

        protected float maxSize
        {
            get
            {
                return transform.localScale.z > (transform.localScale.x > transform.localScale.y ? transform.localScale.x : transform.localScale.y) ? transform.localScale.z : (transform.localScale.x > transform.localScale.y ? transform.localScale.x : transform.localScale.y);
            }
        }

        void Start()
        {
            TrueSyncWorld.CreateInstance.GetWorld().AddCollider(baseCollider);
            baseCollider.isStatic = gameObject.isStatic;
            baseCollider.enable = gameObject.activeSelf;
            baseCollider.layer = (byte)gameObject.layer;
            baseCollider.owner = gameObject;
        }

        void OnEnable()
        {
            baseCollider.enable = true;
        }

        void OnDisable()
        {
            baseCollider.enable = false;
        }

        void OnDestroy()
        {
            TrueSyncWorld.instance?.GetWorld().RemoveCollider(baseCollider);
        }
    }
}