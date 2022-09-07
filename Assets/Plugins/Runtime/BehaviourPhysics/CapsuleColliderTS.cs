using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TrueSync.Behaviour
{
    public class CapsuleColliderTS : BaseColliderTS
    {
        public float radius = 0.5f;

        public float height = 2;

        private CapsuleCollider capsuleCollider;

        void Awake()
        {
            capsuleCollider = new CapsuleCollider(ColliderCenter,
            (FP)radius * (FP)(transform.localScale.x > transform.localScale.z ? transform.localScale.x : transform.localScale.z),
            (FP)height * (FP)transform.localScale.y,
            new TSQuaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w));
            baseCollider = capsuleCollider;
        }

        void FixedUpdate()
        {
            capsuleCollider.center = ColliderCenter;
            capsuleCollider.radius = (FP)radius * (FP)(transform.localScale.x > transform.localScale.z ? transform.localScale.x : transform.localScale.z);
            capsuleCollider.height = (FP)height * (FP)transform.localScale.y;
            capsuleCollider.quaternion = new TSQuaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            GizmosExtend.DrawWireCapsule(ColliderCenter.ToVector3(), transform.rotation, radius * (transform.localScale.x > transform.localScale.z ? transform.localScale.x : transform.localScale.z), height * transform.localScale.y);
        }

    }
}