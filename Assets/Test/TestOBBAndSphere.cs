using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class TestOBBAndSphere : MonoBehaviour
{
    public Transform obb;

    public Transform sphere;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Transform A = obb;
        OBB obb1 = new OBB();
        obb1.center = new TSVector(A.position.x, A.position.y, A.position.z);
        obb1.extents = new TSVector(A.localScale.x * 0.5f, A.localScale.y * 0.5f, A.localScale.z * 0.5f);
        obb1.Quaternion = new TSQuaternion(A.rotation.x, A.rotation.y, A.rotation.z, A.rotation.w);

        Transform B = sphere;

        Sphere ssb = new Sphere();
        ssb.c = new TSVector(B.position.x, B.position.y, B.position.z);
        ssb.r = 0.5f;


        TSVector p = TSVector.zero;

        if (obb1.Intersects(ssb))
        {
            Debug.Log("圆与OBB 相交");
        }

    }
}
