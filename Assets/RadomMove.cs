using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadomMove : MonoBehaviour
{
    Vector3 point;
    Vector3 targer;

    float range = 5;

    float speed = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        point = transform.position;
        targer = RadomTarger();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, targer) <= 0.5f)
        {
            targer = RadomTarger();
        }
        transform.Translate((targer - transform.position).normalized * speed);
    }

    Vector3 RadomTarger()
    {
        return point + new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
    }
}
