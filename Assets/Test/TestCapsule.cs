using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
public class TestCapsule : MonoBehaviour
{

    public Transform _Cylinder;

    public Transform Sphere;

    public Transform aabbOBJ;

    public Transform obbOBJ;

    public Transform capsuleOBJ;

    public static TestCapsule instance;

    private Transform point;
    private Transform point2;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        point = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        point.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        point.GetComponent<Renderer>().material.color = Color.red;


        point2 = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        point2.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        point2.GetComponent<Renderer>().material.color = Color.yellow;
    }

    // Update is called once per frame
    void Update()
    {
        Transform temp = _Cylinder;
        UnityEngine.CapsuleCollider cap = temp.GetComponent<UnityEngine.CapsuleCollider>();
        Capsule c = new Capsule(new TSVector(temp.position.x + cap.center.x, temp.position.y + cap.center.y, temp.position.z + cap.center.z),
        cap.radius, cap.height, new TSQuaternion(temp.rotation.x, temp.rotation.y, temp.rotation.z, temp.rotation.w));
        Sphere ssa = new Sphere();
        ssa.c = new TSVector(Sphere.position.x, Sphere.position.y, Sphere.position.z);
        ssa.r = 0.5f;
        if (Capsule.Intersects(c, ssa))
        {
            _Cylinder.GetComponent<Renderer>().material.color = Color.red;
            Sphere.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            _Cylinder.GetComponent<Renderer>().material.color = Color.white;
            Sphere.GetComponent<Renderer>().material.color = Color.white;
        }


        temp = aabbOBJ;
        AABB aaa = new AABB(new TSVector(temp.position.x, temp.position.y, temp.position.z), new TSVector(temp.localScale.x, temp.localScale.y, temp.localScale.z));


        if (aaa.Intersects(c))
        {
            _Cylinder.GetComponent<Renderer>().material.color = Color.red;
            aabbOBJ.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            _Cylinder.GetComponent<Renderer>().material.color = Color.white;
            aabbOBJ.GetComponent<Renderer>().material.color = Color.white;
        }

        Transform A = obbOBJ;
        OBB obb1 = new OBB();
        obb1.center = new TSVector(A.position.x, A.position.y, A.position.z);
        obb1.extents = new TSVector(A.localScale.x * 0.5f, A.localScale.y * 0.5f, A.localScale.z * 0.5f);
        obb1.Quaternion = new TSQuaternion(A.rotation.x, A.rotation.y, A.rotation.z, A.rotation.w);


        var ff = c.ClosestPointCapsule(obb1.center);
        point.position = ff.ToVector3();

        var sssfd = obb1.ClosestPointOBB(c.center);
        point2.position = sssfd.ToVector3();

        if (Capsule.Intersects(c, obb1))
        {
            _Cylinder.GetComponent<Renderer>().material.color = Color.red;
            obbOBJ.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            _Cylinder.GetComponent<Renderer>().material.color = Color.white;
            obbOBJ.GetComponent<Renderer>().material.color = Color.white;
        }


        temp = capsuleOBJ;
        UnityEngine.CapsuleCollider cap2 = temp.GetComponent<UnityEngine.CapsuleCollider>();
        Capsule c2 = new Capsule(new TSVector(temp.position.x + cap.center.x, temp.position.y + cap.center.y, temp.position.z + cap.center.z),
        cap.radius, cap.height, new TSQuaternion(temp.rotation.x, temp.rotation.y, temp.rotation.z, temp.rotation.w));
        if (Capsule.Intersects(c, c2))
        {
            _Cylinder.GetComponent<Renderer>().material.color = Color.red;
            capsuleOBJ.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            _Cylinder.GetComponent<Renderer>().material.color = Color.white;
            capsuleOBJ.GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
