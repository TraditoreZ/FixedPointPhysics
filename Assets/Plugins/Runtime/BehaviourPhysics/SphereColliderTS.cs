using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TrueSync.Behaviour
{
    public class SphereColliderTS : BaseColliderTS
    {
        public float radius = 0.5f;

        private SphereCollider sphereCollider;

        void Awake()
        {
            sphereCollider = new SphereCollider(ColliderCenter, (FP)radius * (FP)maxSize);
            baseCollider = sphereCollider;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            sphereCollider.center = ColliderCenter;
            sphereCollider.radius = (FP)radius * (FP)maxSize;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(ColliderCenter.ToVector3(), radius * maxSize);
        }

    }
}