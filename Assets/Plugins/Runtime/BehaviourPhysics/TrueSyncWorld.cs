using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TrueSync.Behaviour
{
    [DefaultExecutionOrder(50)]
    public class TrueSyncWorld : MonoBehaviour
    {
        public static TrueSyncWorld instance { get; private set; }

        public static TrueSyncWorld CreateInstance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("_PhysicWorld");
                    instance = go.AddComponent<TrueSyncWorld>();
                }
                return instance;
            }
        }

        [SerializeField]
        private int initialWorldSize = 10;
        private World myWorld;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            myWorld = new World(initialWorldSize >= 0 ? initialWorldSize : 10);
            DontDestroyOnLoad(gameObject);
        }

        void FixedUpdate()
        {
            myWorld.Tick();
        }

        public World GetWorld()
        {
            return myWorld;
        }
    }
}