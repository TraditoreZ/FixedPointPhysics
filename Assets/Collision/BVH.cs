using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

// TODO: handle merge/split when LEAF_OBJ_MAX > 1 and objects move
// TODO: add sphere traversal

namespace TrueSync
{
    public enum Axis
    {
        X, Y, Z,
    }

    public delegate bool NodeTraversalTest(AABB box);

    public class BVHHelper
    {
        public static NodeTraversalTest RadialNodeTraversalTest(TSVector center, FP radius)
        {
            return (AABB bounds) =>
            {
                //find the closest point inside the bounds
                //Then get the difference between the point and the circle center
                FP deltaX = center.x - FP.Max(bounds.min.x, FP.Min(center.x, bounds.max.x));
                FP deltaY = center.y - FP.Max(bounds.min.y, FP.Min(center.y, bounds.max.y));
                FP deltaZ = center.z - FP.Max(bounds.min.z, FP.Min(center.z, bounds.max.z));

                //sqr magnitude < sqr radius = inside bounds!
                return (deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ) < (radius * radius);
            };
        }

        public static NodeTraversalTest AABBTraversalTest(AABB aabb)
        {
            return (AABB bounds) => { return bounds.Intersects(aabb); };
        }
    }

    public class BVH<T>
    {
        private Material _debugRenderMaterial = null;

        public BVHNode<T> rootBVH;
        public IBVHNodeAdapter<T> nAda;
        public readonly int LEAF_OBJ_MAX;
        public int nodeCount = 0;
        public int maxDepth = 0;

        public HashSet<BVHNode<T>> refitNodes = new HashSet<BVHNode<T>>();

        // internal functional traversal...
        private void _traverse(BVHNode<T> curNode, NodeTraversalTest hitTest, List<BVHNode<T>> hitlist)
        {
            if (curNode == null) { return; }
            if (hitTest(curNode.Box))
            {
                hitlist.Add(curNode);
                _traverse(curNode.Left, hitTest, hitlist);
                _traverse(curNode.Right, hitTest, hitlist);
            }
        }

        private void _traverse(BVHNode<T> curNode, AABB target, List<BVHNode<T>> hitlist)
        {
            if (curNode == null) { return; }
            if (curNode.Box.Intersects(target))
            {
                hitlist.Add(curNode);
                _traverse(curNode.Left, target, hitlist);
                _traverse(curNode.Right, target, hitlist);
            }
        }

        // public interface to traversal..
        public List<BVHNode<T>> Traverse(NodeTraversalTest hitTest)
        {
            var hits = new List<BVHNode<T>>();
            this._traverse(rootBVH, hitTest, hits);
            return hits;
        }

        public List<BVHNode<T>> Traverse(AABB aabb, ref List<BVHNode<T>> hits)
        {
            this._traverse(rootBVH, aabb, hits);
            return hits;
        }

        /*	
        public List<BVHNode<T> Traverse(Ray ray)
		{
			FP tnear = 0f, tfar = 0f;

			return Traverse(box => OpenTKHelper.intersectRayAABox1(ray, box, ref tnear, ref tfar));
		}
		public List<BVHNode<T>> Traverse(AABB volume)
		{
			return Traverse(box => box.IntersectsAABB(volume));
		}
		*/

        /// <summary>
        /// Call this to batch-optimize any object-changes notified through 
        /// ssBVHNode.refit_ObjectChanged(..). For example, in a game-loop, 
        /// call this once per frame.
        /// </summary>
        public void Optimize()
        {
            if (LEAF_OBJ_MAX != 1)
            {
                throw new Exception("In order to use optimize, you must set LEAF_OBJ_MAX=1");
            }

            while (refitNodes.Count > 0)
            {
                int maxdepth = refitNodes.Max(n => n.Depth);

                var sweepNodes = refitNodes.Where(n => n.Depth == maxdepth).ToList();
                for (int i = 0; i < sweepNodes.Count; i++)
                {
                    refitNodes.Remove(sweepNodes[i]);
                    // TODO 根据实际需求决定是否进行旋转计算
                    sweepNodes[i].TryRotate(this);
                }
            }
        }

        public void Add(T newOb)
        {
            AABB box = BoundsFromSphere(nAda.GetObjectPos(newOb), nAda.GetRadius(newOb));
            FP boxSAH = BVHNode<T>.SA(ref box);
            rootBVH.Add(nAda, newOb, ref box, boxSAH);
        }

        /// <summary>
        /// Call this when you wish to update an object. This does not update straight away, but marks it for update when Optimize() is called
        /// </summary>
        /// <param name="toUpdate"></param>
        public void MarkForUpdate(T toUpdate)
        {
            nAda.OnPositionOrSizeChanged(toUpdate);
        }

        //Modified from https://github.com/jeske/SimpleScene/blob/master/SimpleScene/Core/SSAABB.cs
        public static AABB BoundsFromSphere(TSVector pos, FP radius)
        {
            AABB bounds = new AABB
            {
                min = new TSVector(pos.x - radius, pos.y - radius, pos.z - radius),
                max = new TSVector(pos.x + radius, pos.y + radius, pos.z + radius)
            };
            return bounds;
        }

        public void Remove(T newObj)
        {
            var leaf = nAda.GetLeaf(newObj);
            leaf.Remove(nAda, newObj);
        }

        public int CountBVHNodes()
        {
            return rootBVH.CountBVHNodes();
        }

        /// <summary>
        /// initializes a BVH with a given nodeAdaptor, and object list.
        /// </summary>
        /// <param name="nodeAdaptor"></param>
        /// <param name="objects"></param>
        /// <param name="LEAF_OBJ_MAX">WARNING! currently this must be 1 to use dynamic BVH updates</param>
        public BVH(IBVHNodeAdapter<T> nodeAdaptor, List<T> objects, int LEAF_OBJ_MAX = 1)
        {
            this.LEAF_OBJ_MAX = LEAF_OBJ_MAX;
            nodeAdaptor.BVH = this;
            this.nAda = nodeAdaptor;

            if (objects.Count > 0)
            {
                rootBVH = new BVHNode<T>(this, objects);
            }
            else
            {
                rootBVH = new BVHNode<T>(this);
                rootBVH.GObjects = new List<T>(); // it's a leaf, so give it an empty object list
            }
        }

        //AABB rendering ( for debugging )
        //=======\/=======\/=======\/=======\/=======\/=======\/=======\/=======\/=======\/=======\/=======\/=======\/

        private static readonly List<Vector3> vertices = new List<Vector3>
        {
            new Vector3 (-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3 (-0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f),
        };

        private static readonly int[] indices =
        {
            0, 1, 1, 2, 2, 3, 3, 0, // face1
            4, 5, 5, 6, 6, 7, 7, 4, // face2
            0, 4, 1, 5, 2, 6, 3, 7  // interconnects
        };

        public void GetAllNodeMatriciesRecursive(BVHNode<T> n, ref List<Matrix4x4> matricies, int depth)
        {
            //rotate not required since AABB
            Matrix4x4 matrix = Matrix4x4.Translate(n.Box.center.ToVector3()) * Matrix4x4.Scale(n.Box.size.ToVector3());
            matricies.Add(matrix);

            if (n.Right != null) GetAllNodeMatriciesRecursive(n.Right, ref matricies, depth + 1);
            if (n.Left != null) GetAllNodeMatriciesRecursive(n.Left, ref matricies, depth + 1);
        }

        List<Matrix4x4> debugMatricies = new List<Matrix4x4>();
        Mesh debugMesh;
        public void RenderDebug()
        {
            if (!SystemInfo.supportsInstancing)
            {
                Debug.LogError("[BVH] Cannot render BVH. Mesh instancing not supported by system");
            }
            else
            {
                debugMatricies.Clear();

                GetAllNodeMatriciesRecursive(rootBVH, ref debugMatricies, 0);
                if (debugMesh == null)
                    debugMesh = new Mesh();
                debugMesh.SetVertices(vertices);
                debugMesh.SetIndices(indices, MeshTopology.Lines, 0);

                if (_debugRenderMaterial == null)
                {
                    _debugRenderMaterial = new Material(Shader.Find("Standard"))
                    {
                        enableInstancing = true
                    };
                }
                Graphics.DrawMeshInstanced(debugMesh, 0, _debugRenderMaterial, debugMatricies);
            }
        }
    }
}
