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


    void Start()
    {
        transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float angleInRadians = angle * Mathf.Deg2Rad;
        initialVelocity.x = Mathf.Cos(angleInRadians) * speed;
        initialVelocity.y = Mathf.Sin(angleInRadians) * speed;
        initialVelocity.z = 0.0f;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateBall();
        }

    }

    void CreateBall()
    {
        GameObject ball = Instantiate(ballPrefab, transform.position, transform.rotation);
        BallMotion motion = ball.AddComponent<BallMotion>();

        motion.Initialize(initialVelocity, AccGravity, Drag);
    }

}

public class BallMotion : MonoBehaviour
{
    public Vector3 velocity;
    private Vector3 gravity;
    private float dragCoefficient;
    public void Initialize(Vector3 initialVelocity, Vector3 gravity, float dragCoefficient)
    {
        this.velocity = initialVelocity;
        this.gravity = gravity;
        this.dragCoefficient = dragCoefficient;
    }

    void FixedUpdate()
    {
        float deltaTime = Time.deltaTime;
        Vector3 PreviousPosition = transform.position;

        Vector3 drag = -dragCoefficient * velocity;

        velocity += (gravity + drag) * deltaTime;

        transform.position += velocity * deltaTime;

        Debug.DrawLine(PreviousPosition, transform.position, Color.red, 15);

    }
}
