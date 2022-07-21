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
                    DrawWireCube(boxShape.center.ToVector3(), boxShape.quaternion.ToQuaternion(), boxShape.size.ToVector3());
                }
                else if (collider.shape is SphereShape)
                {
                    SphereShape sphereShape = (SphereShape)collider.shape;
                    Gizmos.DrawWireSphere(sphereShape.center.ToVector3(), sphereShape.radius.AsFloat());
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

        private void DrawWireCube(Vector3 position, Quaternion rotation, Vector3 size)
        {
            Vector3 AxisX = rotation * Vector3.right;
            Vector3 AxisY = rotation * Vector3.up;
            Vector3 AxisZ = rotation * Vector3.forward;
            Vector3 extents = size * 0.5f;
            Vector3 x = AxisX * extents.x;
            Vector3 y = AxisY * extents.y;
            Vector3 z = AxisZ * extents.z;

            Vector3 p1 = position + x + y + z;
            Vector3 p2 = position - x + y + z;
            Vector3 p3 = position + x - y + z;
            Vector3 p4 = position + x + y - z;
            Vector3 p5 = position + x - y - z;
            Vector3 p6 = position - x - y + z;
            Vector3 p7 = position - x + y - z;
            Vector3 p8 = position - x - y - z;

            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p6);
            Gizmos.DrawLine(p6, p3);
            Gizmos.DrawLine(p3, p1);

            Gizmos.DrawLine(p4, p7);
            Gizmos.DrawLine(p7, p8);
            Gizmos.DrawLine(p8, p5);
            Gizmos.DrawLine(p5, p4);

            Gizmos.DrawLine(p1, p4);
            Gizmos.DrawLine(p2, p7);
            Gizmos.DrawLine(p3, p5);
            Gizmos.DrawLine(p6, p8);
        }

#endif
    }
}