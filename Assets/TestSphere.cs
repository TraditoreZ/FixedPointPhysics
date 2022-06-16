using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class TestSphere : MonoBehaviour
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
        Sphere ssa = new Sphere();
        ssa.c = new TSVector(A.position.x, A.position.y, A.position.z);
        ssa.r = 0.5f;

        Sphere ssb = new Sphere();
        ssb.c = new TSVector(B.position.x, B.position.y, B.position.z);
        ssb.r = 0.5f;

        if (ssa.Intersects(ssb))
        {
            Debug.Log("相交");
        }

    }
}
