using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadomRotate : MonoBehaviour
{
    Quaternion rot;
    Quaternion targer;

    float speed = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        rot = transform.rotation;
        targer = RadomTarger();
    }

    // Update is called once per frame
    void Update()
    {
        if (Quaternion.Angle(transform.rotation, targer) <= 10)
        {
            targer = RadomTarger();
        }
        transform.Rotate((targer.eulerAngles - transform.rotation.eulerAngles) * speed * Time.deltaTime);
    }

    Quaternion RadomTarger()
    {
        return Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
    }
}
