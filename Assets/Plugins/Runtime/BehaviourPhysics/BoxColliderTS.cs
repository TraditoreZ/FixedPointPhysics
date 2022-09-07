using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TrueSync.Behaviour
{
    public class BoxColliderTS : BaseColliderTS
    {
        public Vector3 size = Vector3.one;

        private BoxCollider boxCollider;

        void Awake()
        {
            boxCollider = new BoxCollider(ColliderCenter,
            new TSVector((FP)size.x * (FP)transform.localScale.x, (FP)size.y * (FP)transform.localScale.y, (FP)size.z * (FP)transform.localScale.z),
            new TSQuaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w));
            baseCollider = boxCollider;
        }

        void FixedUpdate()
        {
            boxCollider.center = ColliderCenter;
            boxCollider.size = new TSVector((FP)size.x * (FP)transform.localScale.x, (FP)size.y * (FP)transform.localScale.y, (FP)size.z * (FP)transform.localScale.z);
            boxCollider.quaternion = new TSQuaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            GizmosExtend.DrawWireCube(ColliderCenter.ToVector3(), transform.rotation, new Vector3(size.x * transform.localScale.x, size.y * transform.localScale.y, size.z * transform.localScale.z));
        }

    }
}