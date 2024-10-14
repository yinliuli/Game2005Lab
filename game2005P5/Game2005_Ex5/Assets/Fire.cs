﻿using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public float speed;
    public float angle;
    public float dt = 0.02f;
    public Vector3 AccGravity = new Vector3(0, -10, 0);
    public Vector3 initialVelocity;
    public float Drag = 0.02f;
    public GameObject ballPrefab;
    public float radius = 0.5f;
    public List<BallMotion> balls = new List<BallMotion>();
    private int BallsCounts = 0;


    void Start()
    {
        transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        float angleInRadians = angle * Mathf.Deg2Rad;
        initialVelocity.x = Mathf.Cos(angleInRadians) * speed;
        initialVelocity.y = Mathf.Sin(angleInRadians) * speed;
        initialVelocity.z = 0.0f;

        if (Input.GetMouseButtonDown(0))
        {
            CreateBall();
        }
        CheckForCollisions();

    }

    void CreateBall()
    {
        GameObject ball = Instantiate(ballPrefab, transform.position, transform.rotation);
        BallMotion motion = ball.AddComponent<BallMotion>();

        motion.Initialize(initialVelocity, AccGravity, Drag, radius);
        BallsCounts++;
        ball.name = "Ball" + BallsCounts;
        balls.Add(motion);

    }
    void CheckForCollisions()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            for (int j = i + 1; j < balls.Count; j++)
            {
                float distance = Vector3.Distance(balls[i].transform.position, balls[j].transform.position);
                if (distance <= balls[i].radius + balls[j].radius)
                {
                    balls[i].SetColor(Color.red);
                    balls[j].SetColor(Color.red);
                    Debug.Log(balls[i].name + " is collision with " + balls[j].name);
                }
                else
                {
                    balls[i].SetColor(Color.white);
                    balls[j].SetColor(Color.white);

                }
            }
        }
    }





}




public class BallMotion : MonoBehaviour
{
    public Vector3 velocity;
    private Vector3 gravity;
    private float dragCoefficient;
    public float radius = 1;
    private Renderer ballRenderer;


    public void Initialize(Vector3 initialVelocity, Vector3 gravity, float dragCoefficient, float Radius)
    {
        this.velocity = initialVelocity;
        this.gravity = gravity;
        this.dragCoefficient = dragCoefficient;
        radius = Radius;
        transform.localScale = new Vector3(radius, radius, radius) * 2;

        ballRenderer = GetComponent<Renderer>();
        if (ballRenderer == null)
        {
            ballRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        ballRenderer.material.color = Color.white;

    }

    void FixedUpdate()
    {
        float deltaTime = Time.deltaTime;
        Vector3 PreviousPosition = transform.position;

        Vector3 drag = -dragCoefficient * velocity.sqrMagnitude * velocity.normalized;

        velocity += (gravity + drag) * deltaTime;

        transform.position += velocity * deltaTime;

        Debug.DrawLine(PreviousPosition, transform.position, Color.red, 30);


    }
    public void SetColor(Color color)
    {
        ballRenderer.material.color = color;
    }



}
