using UnityEngine;

namespace TrueSync
{
	public interface IBVHNodeAdapter<T>
	{
		BVH<T> BVH { get; set; }
		TSVector GetObjectPos(T obj);
		FP GetRadius(T obj);
		void MapObjectToBVHLeaf(T obj, BVHNode<T> leaf);
        void OnPositionOrSizeChanged(T changed);
        void UnmapObject(T obj);
		void CheckMap(T obj);
		BVHNode<T> GetLeaf(T obj);
	}
}