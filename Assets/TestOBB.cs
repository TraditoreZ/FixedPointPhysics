using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class TestOBB : MonoBehaviour
{
    public Transform A;

    public Transform B;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        OBB obb1 = new OBB();
        obb1.center = new TSVector(A.position.x, A.position.y, A.position.z);
        obb1.extents = new TSVector(A.localScale.x * 0.5f, A.localScale.y * 0.5f, A.localScale.z * 0.5f);
        obb1.Quaternion = new TSQuaternion(A.rotation.x, A.rotation.y, A.rotation.z, A.rotation.w);


        OBB obb2 = new OBB();
        obb2.center = new TSVector(B.position.x, B.position.y, B.position.z);
        obb2.extents = new TSVector(B.localScale.x * 0.5f, B.localScale.y * 0.5f, B.localScale.z * 0.5f);
        obb2.Quaternion = new TSQuaternion(B.rotation.x, B.rotation.y, B.rotation.z, B.rotation.w);


        if (obb1.Intersects(obb2))
        {
            Debug.Log("OBB 相交");
        }

    }

}
