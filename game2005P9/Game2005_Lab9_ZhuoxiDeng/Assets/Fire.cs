
﻿using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Member;

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
    [Range(0f, 1f)]
    public float FrictionCoefficient;
    public bool showingtrack;
    private Vector3 SaveVelocityDate;

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

        if (CheckingForPlane)
        {
            CheckForPlane();
        }
        CheckForCollisions();

    }

    void CreateBall(Vector3 position)
    {
        GameObject ball = Instantiate(ballPrefab, position, transform.rotation);
        BallMotion motion = ball.AddComponent<BallMotion>();

        motion.Initialize(initialVelocity, AccGravity, Drag, radius, showingtrack, FrictionCoefficient, Mass);
        BallsCounts++;
        ball.name = "Ball" + BallsCounts;
        balls.Add(motion);
    }

    void MovingCollisionsBalls(float distance, BallMotion ball1, BallMotion ball2)
    {
        Vector3 collisionNormal = (ball1.transform.position - ball2.transform.position).normalized;

        Vector3 relativeVelocity = ball1.velocity - ball2.velocity;
        float velocityAlongNormal = Vector3.Dot(relativeVelocity, collisionNormal);

        if (velocityAlongNormal > 0)
            return;

        Vector3 newVelocity1 = ball1.velocity - (2 * ball2.Mass / (ball1.Mass + ball2.Mass)) * velocityAlongNormal * collisionNormal;
        Vector3 newVelocity2 = ball2.velocity + (2 * ball1.Mass / (ball1.Mass + ball2.Mass)) * velocityAlongNormal * collisionNormal;

        ball1.velocity = newVelocity1;
        ball2.velocity = newVelocity2;

        float overlap = (ball1.radius + ball2.radius) - distance;
        if (overlap > 0)
        {
            Vector3 correction = collisionNormal * (overlap / 2);
            ball1.transform.position += correction;
            ball2.transform.position -= correction;
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
                //balls[i].SetColor(Color.red);
                //Debug.Log(balls[i].name + " is low than the plane");
                MovingCollisionPlane(balls[i], planeToBall);
            }
            else
            {
                //balls[i].SetColor(Color.white);
            }


        }
    }
    void MovingCollisionPlane(BallMotion TheBall, Vector3 DisPlaneToBall)
    {

        Vector3 gravityForce = AccGravity * TheBall.Mass;
        Vector3 planeNormal = plane.transform.up;

        Vector3 normalForce = -Vector3.Project(TheBall.velocity, planeNormal);

        Vector3 frictionForce = -(gravityForce + normalForce);
        frictionForce *= TheBall.BallFrictionCoefficient;

        Vector3 ForceNet = gravityForce + normalForce + frictionForce;

        if (TheBall.Mass != 0)
        {
            ForceNet = ForceNet / TheBall.Mass;
            //TheBall.transform.position += ForceNet * dt;
        }
        if (TheBall.FirstCollision)
        {
            SaveVelocityDate = TheBall.velocity;
            Debug.Log(TheBall.name + ForceNet);
            TheBall.FirstCollision = false;
        };

        float penetrationDepth = TheBall.radius - Vector3.Dot(DisPlaneToBall, planeNormal);
        if (penetrationDepth > 0)
        {
            TheBall.transform.position += planeNormal * penetrationDepth;
        }

        TheBall.velocity = Vector3.zero;
        TheBall.velocity = normalForce;
        Debug.Log(normalForce);

    }
}




public class BallMotion : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 gravity;
    public float dragCoefficient;
    public float radius = 1;
    private Renderer ballRenderer;
    public Vector3 Force;
    public bool showtrack;
    public bool FirstCollision = true;
    [Range(0f, 1f)]
    public float BallFrictionCoefficient;
    public float Mass;


    public void Initialize(Vector3 initialVelocity, Vector3 gravity, float dragCoefficient, float Radius, bool Istracking, float FrictionCoefficient, float Mass)
    {
        this.velocity = initialVelocity;
        this.gravity = gravity;
        this.dragCoefficient = dragCoefficient;
        this.showtrack = Istracking;
        this.BallFrictionCoefficient = FrictionCoefficient;
        this.Mass = Mass;

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
