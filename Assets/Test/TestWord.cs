using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using System;

public class TestWord : MonoBehaviour
{
    public bool showOnGUI;
    public World myWorld;

    public List<BaseCollider> tempColliders = new List<BaseCollider>();
    public Dictionary<GameObject, BaseCollider> tempDic = new Dictionary<GameObject, BaseCollider>();
    // Start is called before the first frame update
    public static TestWord instance;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        myWorld = new World(20);

        var objs = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var item in objs)
        {
            if (item.activeSelf == false)
            {
                continue;
            }
            if (item.GetComponent<UnityEngine.BoxCollider>() != null)
            {
                TSVector center = new TSVector(item.transform.position.x, item.transform.position.y, item.transform.position.z);
                TSVector size = new TSVector(item.GetComponent<UnityEngine.BoxCollider>().size.x, item.GetComponent<UnityEngine.BoxCollider>().size.y, item.GetComponent<UnityEngine.BoxCollider>().size.z);
                TSQuaternion qua = new TSQuaternion(item.transform.rotation.x, item.transform.rotation.y, item.transform.rotation.z, item.transform.rotation.w);
                TrueSync.BoxCollider bc = new TrueSync.BoxCollider(center, size, qua);
                bc.owner = item;
                myWorld.AddCollider(bc);
                tempDic.Add(item, bc);
                bc.OnEnterEvent += OnEnter;
                bc.OnStayEvent += OnStay;
                bc.OnLeaveEvent += OnLeave;
            }
            else if (item.GetComponent<UnityEngine.SphereCollider>() != null)
            {
                TSVector center = new TSVector(item.transform.position.x, item.transform.position.y, item.transform.position.z);
                FP radius = (FP)(item.GetComponent<UnityEngine.SphereCollider>().radius);

                TrueSync.SphereCollider sc = new TrueSync.SphereCollider(center, radius);
                sc.owner = item;
                myWorld.AddCollider(sc);
                tempDic.Add(item, sc);
                sc.OnEnterEvent += OnEnter;
                sc.OnStayEvent += OnStay;
                sc.OnLeaveEvent += OnLeave;
            }
        }
    }

    public void OnLeave(object owner, List<BaseCollider> array)
    {
        // Debug.LogWarning("碰撞离开" + array.Count);
        GameObject item = owner as GameObject;
        item.GetComponent<Renderer>().material.color = Color.white;
    }

    public void OnStay(object owner, List<BaseCollider> array)
    {
        //Debug.Log("碰撞停留" + array.Count);
    }

    public void OnEnter(object owner, List<BaseCollider> array)
    {
        //Debug.Log("碰撞进入" + array.Count);
        GameObject item = owner as GameObject;
        item.GetComponent<Renderer>().material.color = Color.red;
    }


    // Update is called once per frame
    void FixedUpdate()
    {

        myWorld.GetColliders(ref tempColliders);
        foreach (var c in tempColliders)
        {
            GameObject item = c.owner as GameObject;
            if (!item.activeSelf)
            {
                continue;
            }
            if (item.GetComponent<UnityEngine.BoxCollider>() != null)
            {
                TSVector center = new TSVector(item.transform.position.x, item.transform.position.y, item.transform.position.z);
                TSVector size = new TSVector(item.GetComponent<UnityEngine.BoxCollider>().size.x, item.GetComponent<UnityEngine.BoxCollider>().size.y, item.GetComponent<UnityEngine.BoxCollider>().size.z);
                TSQuaternion qua = new TSQuaternion(item.transform.rotation.x, item.transform.rotation.y, item.transform.rotation.z, item.transform.rotation.w);

                (c as TrueSync.BoxCollider).center = center;
                (c as TrueSync.BoxCollider).size = size;
                (c as TrueSync.BoxCollider).quaternion = qua;
                c.rigidBody = true;
            }
            if (item.GetComponent<UnityEngine.SphereCollider>() != null)
            {
                TSVector center = new TSVector(item.transform.position.x, item.transform.position.y, item.transform.position.z);
                FP radius = (FP)(item.GetComponent<UnityEngine.SphereCollider>().radius);

                (c as TrueSync.SphereCollider).center = center;
                (c as TrueSync.SphereCollider).radius = radius;
                c.rigidBody = true;
            }
        }


        myWorld.Tick();
    }



    List<BaseCollider> collidingWith = new List<BaseCollider>();
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        GameObject select = UnityEditor.Selection.activeGameObject;
        if (select != null && tempDic.ContainsKey(select))
        {
            DrawAABB(tempDic[select]);
        }
    }
#endif
    void DrawAABB(BaseCollider c)
    {
        BoxShape box = c.shape as BoxShape;
        if (box == null)
        {
            return;
        }
        OBB obb = box._OBB;
        TSVector x = obb.AxisX * obb.extents.x;
        TSVector y = obb.AxisY * obb.extents.y;
        TSVector z = obb.AxisZ * obb.extents.z;

        TSVector p1 = obb.center + x + y + z;
        TSVector p2 = obb.center - x + y + z;
        TSVector p3 = obb.center + x - y + z;
        TSVector p4 = obb.center + x + y - z;
        TSVector p5 = obb.center + x - y - z;
        TSVector p6 = obb.center - x - y + z;
        TSVector p7 = obb.center - x + y - z;
        TSVector p8 = obb.center - x - y - z;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(p1.ToVector3(), 0.1f);
        Gizmos.DrawWireSphere(p2.ToVector3(), 0.1f);
        Gizmos.DrawWireSphere(p3.ToVector3(), 0.1f);
        Gizmos.DrawWireSphere(p4.ToVector3(), 0.1f);
        Gizmos.DrawWireSphere(p5.ToVector3(), 0.1f);
        Gizmos.DrawWireSphere(p6.ToVector3(), 0.1f);
        Gizmos.DrawWireSphere(p7.ToVector3(), 0.1f);
        Gizmos.DrawWireSphere(p8.ToVector3(), 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(c.bounds.center.ToVector3(), c.bounds.size.ToVector3());
    }

    void Update()
    {
        if (myWorld != null && myWorld.bvh != null)
        {
            myWorld.bvh.RenderDebug();
        }
    }

    void OnGUI()
    {
        if (!showOnGUI)
            return;
        if (GUILayout.Button("创建100个球体 最大范围10M"))
        {
            for (int i = 0; i < 100; i++)
            {
                GameObject item = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                item.transform.position = new Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10));
                //item.transform.localScale = new Vector3(UnityEngine.Random.Range(0.5f, 2), UnityEngine.Random.Range(0.5f, 2), UnityEngine.Random.Range(0.5f, 2));
                TSVector center = new TSVector(item.transform.position.x, item.transform.position.y, item.transform.position.z);
                FP radius = (FP)(item.GetComponent<UnityEngine.SphereCollider>().radius);
                TrueSync.SphereCollider sc = new TrueSync.SphereCollider(center, radius);
                sc.owner = item;
                sc.rigidBody = true;
                myWorld.AddCollider(sc);
                tempDic.Add(item, sc);
                sc.OnEnterEvent += OnEnter;
                sc.OnStayEvent += OnStay;
                sc.OnLeaveEvent += OnLeave;

                item.AddComponent<RadomMove>();
            }
        }
        if (GUILayout.Button("创建100个正方体 最大范围10M"))
        {
            for (int i = 0; i < 100; i++)
            {
                GameObject item = GameObject.CreatePrimitive(PrimitiveType.Cube);
                item.transform.position = new Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10));
                item.transform.localScale = new Vector3(UnityEngine.Random.Range(0.5f, 2), UnityEngine.Random.Range(0.5f, 2), UnityEngine.Random.Range(0.5f, 2));
                TSVector center = new TSVector(item.transform.position.x, item.transform.position.y, item.transform.position.z);
                TSVector size = new TSVector(item.GetComponent<UnityEngine.BoxCollider>().size.x * item.transform.localScale.x, item.GetComponent<UnityEngine.BoxCollider>().size.y * item.transform.localScale.y, item.GetComponent<UnityEngine.BoxCollider>().size.z * item.transform.localScale.z);
                TSQuaternion qua = new TSQuaternion(item.transform.rotation.x, item.transform.rotation.y, item.transform.rotation.z, item.transform.rotation.w);
                TrueSync.BoxCollider bc = new TrueSync.BoxCollider(center, size, qua);
                bc.owner = item;
                bc.rigidBody = true;
                myWorld.AddCollider(bc);
                tempDic.Add(item, bc);
                bc.OnEnterEvent += OnEnter;
                bc.OnStayEvent += OnStay;
                bc.OnLeaveEvent += OnLeave;

                item.AddComponent<RadomMove>();
            }
        }
        if (GUILayout.Button("创建100个球体 最大范围2M"))
        {
            for (int i = 0; i < 100; i++)
            {
                GameObject item = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                item.transform.position = new Vector3(UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-2, 2));
                //item.transform.localScale = new Vector3(UnityEngine.Random.Range(0.5f, 2), UnityEngine.Random.Range(0.5f, 2), UnityEngine.Random.Range(0.5f, 2));
                TSVector center = new TSVector(item.transform.position.x, item.transform.position.y, item.transform.position.z);
                FP radius = (FP)(item.GetComponent<UnityEngine.SphereCollider>().radius);
                TrueSync.SphereCollider sc = new TrueSync.SphereCollider(center, radius);
                sc.owner = item;
                sc.rigidBody = true;
                myWorld.AddCollider(sc);
                tempDic.Add(item, sc);
                sc.OnEnterEvent += OnEnter;
                sc.OnStayEvent += OnStay;
                sc.OnLeaveEvent += OnLeave;

                item.AddComponent<RadomMove>();
            }
        }
        if (GUILayout.Button("创建100个正方体 最大范围2M"))
        {
            for (int i = 0; i < 100; i++)
            {
                GameObject item = GameObject.CreatePrimitive(PrimitiveType.Cube);
                item.transform.position = new Vector3(UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-2, 2));
                item.transform.localScale = new Vector3(UnityEngine.Random.Range(0.5f, 2), UnityEngine.Random.Range(0.5f, 2), UnityEngine.Random.Range(0.5f, 2));
                TSVector center = new TSVector(item.transform.position.x, item.transform.position.y, item.transform.position.z);
                TSVector size = new TSVector(item.GetComponent<UnityEngine.BoxCollider>().size.x * item.transform.localScale.x, item.GetComponent<UnityEngine.BoxCollider>().size.y * item.transform.localScale.y, item.GetComponent<UnityEngine.BoxCollider>().size.z * item.transform.localScale.z);
                TSQuaternion qua = new TSQuaternion(item.transform.rotation.x, item.transform.rotation.y, item.transform.rotation.z, item.transform.rotation.w);
                TrueSync.BoxCollider bc = new TrueSync.BoxCollider(center, size, qua);
                bc.owner = item;
                bc.rigidBody = true;
                myWorld.AddCollider(bc);
                tempDic.Add(item, bc);
                bc.OnEnterEvent += OnEnter;
                bc.OnStayEvent += OnStay;
                bc.OnLeaveEvent += OnLeave;

                item.AddComponent<RadomMove>();
            }
        }


        if (GUILayout.Button("创建5个正方体 最大范围2M 不自由移动"))
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject item = GameObject.CreatePrimitive(PrimitiveType.Cube);
                item.transform.position = new Vector3(UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-2, 2));
                //item.transform.localScale = new Vector3(UnityEngine.Random.Range(0.5f, 2), UnityEngine.Random.Range(0.5f, 2), UnityEngine.Random.Range(0.5f, 2));
                TSVector center = new TSVector(item.transform.position.x, item.transform.position.y, item.transform.position.z);
                TSVector size = new TSVector(item.GetComponent<UnityEngine.BoxCollider>().size.x * item.transform.localScale.x, item.GetComponent<UnityEngine.BoxCollider>().size.y * item.transform.localScale.y, item.GetComponent<UnityEngine.BoxCollider>().size.z * item.transform.localScale.z);
                TSQuaternion qua = new TSQuaternion(item.transform.rotation.x, item.transform.rotation.y, item.transform.rotation.z, item.transform.rotation.w);
                TrueSync.BoxCollider bc = new TrueSync.BoxCollider(center, size, qua);
                bc.rigidBody = true;
                bc.owner = item;
                myWorld.AddCollider(bc);
                tempDic.Add(item, bc);
                bc.OnEnterEvent += OnEnter;
                bc.OnStayEvent += OnStay;
                bc.OnLeaveEvent += OnLeave;
            }
        }
    }

}
