using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class TestAABBAndOBB : MonoBehaviour
{
    public Transform aabb;

    public Transform obb;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Transform temp = aabb;
        AABB aaa = new AABB(new TSVector(temp.position.x, temp.position.y, temp.position.z), new TSVector(temp.localScale.x, temp.localScale.y, temp.localScale.z));

        OBB obb1 = new OBB();
        obb1.center = new TSVector(obb.position.x, obb.position.y, obb.position.z);
        obb1.extents = new TSVector(obb.localScale.x * 0.5f, obb.localScale.y * 0.5f, obb.localScale.z * 0.5f);
        obb1.Quaternion = new TSQuaternion(obb.rotation.x, obb.rotation.y, obb.rotation.z, obb.rotation.w);


        if (aaa.Intersects(obb1))
        {
            Debug.Log("AABB 与OBB 相交");
        }
    }
}
