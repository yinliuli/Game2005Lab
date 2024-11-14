
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
    public Vector3 PlaceToCreateBall;
    public float Mass;
    public float Friction;
    public bool showingtrack;




    public Transform plane;  
    public bool CheckingForPlane;

    void Start()
    {
        transform.position = Vector3.zero;
    }

    void Update()
    {
        float angleInRadians = angle * Mathf.Deg2Rad;
        initialVelocity.x = Mathf.Cos(angleInRadians) * speed;
        initialVelocity.y = Mathf.Sin(angleInRadians) * speed;
        initialVelocity.z = 0.0f;
        
        if (Input.GetMouseButtonDown(0))
        {
            CreateBall(PlaceToCreateBall);
        }
        if(CheckingForPlane) 
        {
            CheckForPlane();
        }
        else
        {
            CheckForCollisions();
        }
        
    }

    void CreateBall(Vector3 position)
    {
        GameObject ball = Instantiate(ballPrefab, position, transform.rotation);
        BallMotion motion = ball.AddComponent<BallMotion>();

        motion.Initialize(initialVelocity, AccGravity, Drag, radius, showingtrack);
        BallsCounts++;
        ball.name = "Ball" + BallsCounts;
        balls.Add(motion);

    }
    void MovingCollisionsBalls(float distance, BallMotion ball1, BallMotion ball2)
    {
        Vector3 Displacement = ball1.transform.position - ball2.transform.position;
        float overlap = (ball1.radius + ball2.radius) - distance;
        if (overlap > 0.0f)
        {
            Vector3 collisionNormalBToA = Displacement / distance;
            Vector3 mtv = collisionNormalBToA * overlap;
            ball1.transform.position += mtv;
        }

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
                    MovingCollisionsBalls(distance, balls[i], balls[j]);
                }
                else
                {
                    balls[i].SetColor(Color.white);
                    balls[j].SetColor(Color.white);

                }
            }
        }
    }

    void CheckForPlane()
    {
        Vector3 planeNormal = plane.transform.up; 
        for (int i = 0; i < balls.Count; i++)
        {
            Vector3 planeToBall = balls[i].transform.position - plane.transform.position;
            float positionAlongNormal = Vector3.Dot(planeToBall, planeNormal);
            if (positionAlongNormal <= balls[i].radius)
            {
                balls[i].SetColor(Color.red);
                Debug.Log(balls[i].name + " is low than the plane");
                MovingCollisionPlane(balls[i], planeToBall, positionAlongNormal);
            }
            else
            {
                balls[i].SetColor(Color.white);
            }


        }
    }
    void MovingCollisionPlane(BallMotion TheBall, Vector3 DisPlaneToBall, float distance)
    {
        Vector3 planeNormal = plane.transform.up;
        TheBall.velocity = Vector3.Reflect(TheBall.velocity, planeNormal);
        Vector3 collisionForce = planeNormal * (TheBall.radius - distance);
        TheBall.transform.position += collisionForce;
        Vector3 normalVelocity = Vector3.Project(TheBall.velocity, planeNormal); 
        Vector3 tangentVelocity = TheBall.velocity - normalVelocity;
        tangentVelocity *= (1 - Friction);
        TheBall.velocity = normalVelocity + tangentVelocity;



        Vector3 GetAbsOfVelocity = new Vector3(Mathf.Abs(TheBall.velocity.x), Mathf.Abs(TheBall.velocity.y), Mathf.Abs(TheBall.velocity.z) );
        Debug.DrawLine(TheBall.transform.position, TheBall.transform.position + -tangentVelocity, Color.black);

        Debug.DrawLine(TheBall.transform.position, TheBall.transform.position + GetAbsOfVelocity, Color.yellow);
    }
}




public class BallMotion : MonoBehaviour
{
    public Vector3 velocity;
    private Vector3 gravity;
    public float dragCoefficient;
    public float radius = 1;
    private Renderer ballRenderer;
    public Vector3 Force;
    public bool showtrack;
    public void Initialize(Vector3 initialVelocity, Vector3 gravity, float dragCoefficient, float Radius, bool Istracking)
    {
        this.velocity = initialVelocity;
        this.gravity = gravity;
        this.dragCoefficient = dragCoefficient;
        this.showtrack = Istracking;

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

        velocity += Force;
        Force = Vector3.zero;


        Debug.DrawLine(transform.position, transform.position + gravity, Color.green);
        if (showtrack) 
        { 
            Debug.DrawLine(PreviousPosition, transform.position, Color.red, 30);
        
        };

    }
    public void SetColor(Color color)
    {
        ballRenderer.material.color = color;
    }



}
