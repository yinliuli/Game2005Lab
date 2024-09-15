using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class moving : MonoBehaviour
{

    Vector3 begin = new Vector3 (0, 0, 0);
    public float frequency = 1;
    public float amplitude = 1;

    void Start()
    {

        transform.position = (begin);        

    }

    void Update()
    {
        float gt = Time.time;
        float dt = Time.fixedDeltaTime;
        float objectX = -Mathf.Sin(gt * amplitude) * amplitude * frequency * dt;
        float objectY = -Mathf.Cos(gt * amplitude) * amplitude * frequency * dt;

        transform.position = new Vector3(transform.position.x + objectX, transform.position.y + objectY, transform.position.z);

    }



}
