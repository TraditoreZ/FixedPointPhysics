using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrueSync
{

    public class BVHBaseColliderAdapter : IBVHNodeAdapter<BaseCollider>
    {


        private BVH<BaseCollider> _bvh;
        Dictionary<BaseCollider, BVHNode<BaseCollider>> gameObjectToLeafMap = new Dictionary<BaseCollider, BVHNode<BaseCollider>>();
        private event Action<BaseCollider> _onPositionOrSizeChanged;

        BVH<BaseCollider> IBVHNodeAdapter<BaseCollider>.BVH
        {
            get
            {
                return _bvh;
            }
            set
            {
                _bvh = value;
            }
        }

        public void CheckMap(BaseCollider obj)
        {
            if (!gameObjectToLeafMap.ContainsKey(obj))
            {
                Debug.LogError("missing map for shuffled child");
            }
        }

        public BVHNode<BaseCollider> GetLeaf(BaseCollider obj)
        {
            return gameObjectToLeafMap[obj];
        }

        public TSVector GetObjectPos(BaseCollider obj)
        {
            return obj.bounds.center;
        }

        public FP GetRadius(BaseCollider obj)
        {
            return FP.Max(FP.Max(obj.bounds.extents.x, obj.bounds.extents.y), obj.bounds.extents.z);
        }

        public void MapObjectToBVHLeaf(BaseCollider obj, BVHNode<BaseCollider> leaf)
        {
            gameObjectToLeafMap[obj] = leaf;
        }

        public void OnPositionOrSizeChanged(BaseCollider changed)
        {
            gameObjectToLeafMap[changed].RefitObjectChanged(this, changed);
        }

        public void UnmapObject(BaseCollider obj)
        {
            gameObjectToLeafMap.Remove(obj);
        }
    }

}