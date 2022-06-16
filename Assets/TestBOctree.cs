using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBOctree : MonoBehaviour
{
    // Start is called before the first frame update
    BoundsOctree<GameObject> boundsTree;
    void Start()
    {
        boundsTree = new BoundsOctree<GameObject>(15, Vector3.zero, 1, 1.25f);

        var objs = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var item in objs)
        {
            if (item.GetComponent<UnityEngine.BoxCollider>() != null)
            {
                boundsTree.Add(item, item.GetComponent<UnityEngine.BoxCollider>().bounds);
            }
            if (item.GetComponent<UnityEngine.SphereCollider>() != null)
            {
                boundsTree.Add(item, item.GetComponent<UnityEngine.SphereCollider>().bounds);
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
    }

    List<GameObject> collidingWith = new List<GameObject>();

    void OnDrawGizmos()
    {
        if (boundsTree != null)
        {
            boundsTree.DrawAllBounds(); // Draw node boundaries
            boundsTree.DrawAllObjects(); // Draw object boundaries
            boundsTree.DrawCollisionChecks(); // 绘制最后一个* numcollisionstosave *碰撞检查边界
        }
        GameObject select = UnityEditor.Selection.activeGameObject;
        if (select != null && select.GetComponent<Collider>() != null)
        {
            collidingWith.Clear();
            boundsTree.GetColliding(collidingWith, select.GetComponent<Collider>().bounds);
            foreach (var item in collidingWith)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(item.transform.position, item.transform.localScale);
            }
        }
    }

}
