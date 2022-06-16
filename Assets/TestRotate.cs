using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class TestRotate : MonoBehaviour
{
    public Transform A;

    public Transform B;

    public Transform targer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        A.LookAt(targer);


        Vector3 dir = (targer.position - B.position).normalized;
        Quaternion quaternion = Quaternion.LookRotation(dir);
        quaternion *= Quaternion.Euler(0, 30, 0);
        //TSQuaternion quaternion = TSQuaternion.LookRotation(new TSVector(dir.x, dir.y, dir.z), TSVector.forward);
        //B.rotation = quaternion.ToQuaternion();
        //B.rotation = quaternion * Quaternion.Euler(30, 0, 0);
        // TSVector f = quaternion.eulerAngles + new TSVector(0, 0, 0);
        // Debug.Log(quaternion.eulerAnglesNew);
        // Debug.LogWarning(quaternion.ToQuaternion().eulerAngles);
        //quaternion = quaternion * TSQuaternion.AngleAxis(30, TSVector.right);

        //B.rotation = quaternion.ToQuaternion();
        B.rotation = quaternion;
    }

    /// <summary> 四元数求物体Z轴正方向 </summary>
    /// <param name="qua">角度</param>
    /// <returns>正方向</returns>
    public static TSVector QuaternionToForward(TSQuaternion qua)
    {
        return new TSVector(FP.Sin(qua.eulerAngles.y * FP.Deg2Rad) * FP.Cos(qua.eulerAngles.x * FP.Deg2Rad), -FP.Sin(qua.eulerAngles.x * FP.Deg2Rad), FP.Cos(qua.eulerAngles.y * FP.Deg2Rad) * FP.Cos(qua.eulerAngles.x * FP.Deg2Rad));
    }


}
