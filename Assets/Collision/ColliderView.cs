using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrueSync
{

    public class ColliderView : MonoBehaviour
    {
        public bool drawAllBounds;

        public bool drawAllObjects;

        public bool drawCollisionChecks;

        public bool drawCollisions;

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
                {
                    DrawCollisions();
                }
            }
        }

        private void DrawCollisions()
        {
            foreach (var collider in myWorld.colliders)
            {
                if (collider.ColliderCount() > 0)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;
                if (collider.shape is BoxShape)
                {
                    BoxShape boxShape = (BoxShape)collider.shape;
                    Gizmos.DrawWireCube(boxShape.center.ToVector3(), boxShape.size.ToVector3());
                    //Gizmos.DrawWireMesh(cubeMesh, boxShape.center.ToVector3(), boxShape.quaternion.ToQuaternion(), boxShape.size.ToVector3());
                }
                else if (collider.shape is SphereShape)
                {
                    SphereShape sphereShape = (SphereShape)collider.shape;
                    Gizmos.DrawWireSphere(sphereShape.center.ToVector3(), sphereShape.radius.AsFloat());
                }
            }
        }

        private void DrawSelectCollision()
        {

        }

#endif
    }
}