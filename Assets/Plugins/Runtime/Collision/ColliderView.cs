using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrueSync
{

    public class ColliderView : MonoBehaviour
    {
        public bool drawCollisions;

        public bool drawAABB;

        public bool drawBVH;

        public static World myWorld;


        void Awake()
        {
            GameObject.DontDestroyOnLoad(gameObject);
        }

        void OnDestroy()
        {
            myWorld = null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (myWorld != null)
            {
                if (drawCollisions)
                    DrawCollisions();
                if (drawAABB)
                    DrawAABB();
                if (drawBVH)
                    DrawBVH();
            }
        }

        private void DrawCollisions()
        {
            foreach (var collider in myWorld.colliders)
            {
                if (collider.CollisionCount() > 0)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;
                if (collider.shape is BoxShape)
                {
                    BoxShape boxShape = (BoxShape)collider.shape;
                    GizmosExtend.DrawWireCube(boxShape.center.ToVector3(), boxShape.quaternion.ToQuaternion(), boxShape.size.ToVector3());
                }
                else if (collider.shape is SphereShape)
                {
                    SphereShape sphereShape = (SphereShape)collider.shape;
                    Gizmos.DrawWireSphere(sphereShape.center.ToVector3(), sphereShape.radius.AsFloat());
                }
                else if (collider.shape is CapsuleShape)
                {
                    CapsuleShape capsuleShape = (CapsuleShape)collider.shape;
                    GizmosExtend.DrawWireCapsule(capsuleShape.center.ToVector3(), capsuleShape.quaternion.ToQuaternion(), capsuleShape.radius.AsFloat(), capsuleShape.height.AsFloat());
                }
            }
        }

        private void DrawAABB()
        {
            foreach (var collider in myWorld.colliders)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(collider.bounds.center.ToVector3(), collider.bounds.size.ToVector3());
            }
        }

        static List<Matrix4x4> debugMatricies = new List<Matrix4x4>();
        private void DrawBVH()
        {
            debugMatricies.Clear();
            myWorld.bvh.GetAllNodeMatriciesRecursive(myWorld.bvh.rootBVH, ref debugMatricies, 0);
            foreach (var matrix in debugMatricies)
            {
                Gizmos.color = Color.white;
                Gizmos.matrix = matrix;
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }
        }

#endif
    }
}