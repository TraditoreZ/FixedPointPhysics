using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using System;

public class TestWord : MonoBehaviour
{
    World myWorld;

    List<BaseCollider> tempColliders = new List<BaseCollider>();
    Dictionary<GameObject, BaseCollider> tempDic = new Dictionary<GameObject, BaseCollider>();
    // Start is called before the first frame update
    void Start()
    {
        myWorld = new World(20, SpaceType.BVH);

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

    private void OnLeave(object owner, List<BaseCollider> array)
    {
        // Debug.LogWarning("碰撞离开" + array.Count);
        GameObject item = owner as GameObject;
        item.GetComponent<Renderer>().material.color = Color.white;
    }

    private void OnStay(object owner, List<BaseCollider> array)
    {
        //Debug.Log("碰撞停留" + array.Count);
    }

    private void OnEnter(object owner, List<BaseCollider> array)
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
            }
            if (item.GetComponent<UnityEngine.SphereCollider>() != null)
            {
                TSVector center = new TSVector(item.transform.position.x, item.transform.position.y, item.transform.position.z);
                FP radius = (FP)(item.GetComponent<UnityEngine.SphereCollider>().radius);

                (c as TrueSync.SphereCollider).center = center;
                (c as TrueSync.SphereCollider).radius = radius;
            }
        }


        myWorld.Tick();
    }



    List<BaseCollider> collidingWith = new List<BaseCollider>();

    private void OnDrawGizmos()
    {
        if (myWorld != null && myWorld.octree != null)
        {
            myWorld.octree.DrawAllBounds(); // Draw node boundaries
            myWorld.octree.DrawAllObjects(); // Draw object boundaries
            myWorld.octree.DrawCollisionChecks(); // 绘制最后一个* numcollisionstosave *碰撞检查边界
        }
        GameObject select = UnityEditor.Selection.activeGameObject;
        if (select != null && myWorld != null && myWorld.octree != null && tempDic.ContainsKey(select))
        {
            collidingWith.Clear();
            myWorld.octree.GetColliding(collidingWith, tempDic[select].bounds);
            foreach (var item in collidingWith)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube((item.owner as GameObject).transform.position, (item.owner as GameObject).transform.localScale);
            }
        }
        if (select != null && tempDic.ContainsKey(select))
        {
            DrawAABB(tempDic[select]);
        }
    }

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
