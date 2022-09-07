using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    void OnGUI()
    {
        if (GUILayout.Button("创建子弹 速度 10"))
        {
            CreateBullet(10);
        }

        if (GUILayout.Button("创建子弹 速度 20"))
        {
            CreateBullet(20);
        }

        if (GUILayout.Button("创建子弹 速度 100 (目前无法检测到)"))
        {
            CreateBullet(100);
        }
    }

    void CreateBullet(float speed)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;
        go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        go.AddComponent<Bullet>().speed = speed;
        TSVector center = new TSVector(go.transform.position.x, go.transform.position.y, go.transform.position.z);
        TSVector size = new TSVector(go.GetComponent<UnityEngine.BoxCollider>().size.x * go.transform.localScale.x, go.GetComponent<UnityEngine.BoxCollider>().size.y * go.transform.localScale.y, go.GetComponent<UnityEngine.BoxCollider>().size.z * go.transform.localScale.z);
        TSQuaternion qua = new TSQuaternion(go.transform.rotation.x, go.transform.rotation.y, go.transform.rotation.z, go.transform.rotation.w);
        TrueSync.BoxCollider bc = new TrueSync.BoxCollider(center, size, qua);
        bc.owner = go;
        TestWord.instance.myWorld.AddCollider(bc);
        TestWord.instance.tempDic.Add(go, bc);
        bc.OnEnterEvent += TestWord.instance.OnEnter;
        bc.OnStayEvent += TestWord.instance.OnStay;
        bc.OnLeaveEvent += TestWord.instance.OnLeave;
    }



}
