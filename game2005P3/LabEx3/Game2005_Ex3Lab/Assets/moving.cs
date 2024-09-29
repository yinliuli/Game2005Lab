using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moving : MonoBehaviour
{
    public float speed;
    public float angle;
    bool StartMoving = false;
    public float dt = 0.02f;
    public Vector3 AccGravity = new Vector3(0, -10, 0);
    public Vector3 velocity;


    void Start()
    {
        transform.position = Vector3.zero;
        StartMoving = false;


    }

    // Update is called once per frame
    void FixedUpdate()
    {

        float angleInRadians = angle * Mathf.Deg2Rad;

        Vector3 velocity = new Vector3(Mathf.Cos(angleInRadians) * speed, Mathf.Sin(angleInRadians) * speed, 0);

        if (Input.GetKey(KeyCode.Space))
        {
            transform.position = Vector3.zero;
            StartMoving = (true);
            Debug.Log("Start");
            AccGravity = new Vector3(0, -10, 0);
        }

        if (StartMoving)
        {
            Vector3 p = transform.position;
            AccGravity += AccGravity * dt;
            transform.position = transform.position + velocity * dt + AccGravity * dt;

            Debug.DrawLine(p, transform.position, Color.red, 10);

        }

    }

}


