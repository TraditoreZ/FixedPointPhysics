using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class TestAABB : MonoBehaviour
{
    public Transform A;

    public Transform B;


    // Start is called before the first frame update
    void Start()
    {

    }

    private AABB lastA;

    private AABB lastB;

    // Update is called once per frame
    void Update()
    {
        Transform temp = A;
        AABB aaa = new AABB(new TSVector(temp.position.x, temp.position.y, temp.position.z), new TSVector(temp.localScale.x, temp.localScale.y, temp.localScale.z));

        temp = B;
        AABB bbb = new AABB(new TSVector(temp.position.x, temp.position.y, temp.position.z), new TSVector(temp.localScale.x, temp.localScale.y, temp.localScale.z));

        if (aaa == lastA && bbb == lastB)
        {
            return;
        }
        lastA = aaa;
        lastB = bbb;
        if (aaa.Intersects(bbb))
        {
            Debug.Log("AABB 相交");
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(A.position, Vector3.one);
        Gizmos.DrawWireCube(B.position, Vector3.one);
    }


}
