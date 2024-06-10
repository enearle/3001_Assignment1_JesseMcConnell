using System;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;


public class FishObject : AgentObject
{
    [SerializeField] private float movementSpeed = 100;
    private float speed;
    [SerializeField] private float rotationSpeed = 60;
    [SerializeField] private float slowDownRadius = 2;
    [SerializeField] private float arriveRadius = 0.05f;
    [SerializeField] private float whiskerLength = 1f;
    [SerializeField] private float farWhiskerLength = 0.6f;
    [SerializeField] private float centerWhiskerLength = 1.8f;
    [SerializeField] private float whiskerAngle = 30;
    [SerializeField] private float avoidanceWeight;

    private Vector2 fleeFrom;
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private float velocityWeight = 1f;
    private float WanderTimer = 0;
    private const float WanderDurationMax = 3f;
    private bool mouseDown = false;
    private enum State
    {
        Wander,
        Flee,
        Seek,
        Eat
    }

    private State state = State.Wander;
    
    void Start()
    {
        //Debug.Log("Fish start");
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        speed = movementSpeed;

        if (state != State.Flee)
        {
            if (SceneManager.Instance.food.Count > 0)
                state = State.Eat;
            else if (Input.GetMouseButton(0))
            {
                if(Input.GetMouseButtonDown(0))
                    Debug.Log("Seeking");
                state = State.Seek;
            }
            else if (mouseDown)
            {
                state = State.Wander;
                mouseDown = false;
            }
            else
            {
                state = State.Wander;
            }
        }

        
        switch (state)
        {
            case State.Wander:
                Wander();
                break;
            case State.Seek:
                TargetMouse();
                break;
            case State.Flee:
                Flee();
                break;
            case State.Eat:
                Eat();
                break;
        }
        
        if (TargetPosition != null)
        {
            //SeekKinematic();
            if((TargetPosition - transform.position).magnitude <= arriveRadius)
                rb.velocity = Vector2.zero;
            else if ((TargetPosition - transform.position).magnitude <= slowDownRadius)
                SeekForward(rb,TargetPosition, speed * 0.75f, rotationSpeed);
            else
                SeekForward(rb,TargetPosition, speed, rotationSpeed);
            
            AvoidObstacles();
        }
    }

    private void Eat()
    {
        WanderTimer = 0;
        TargetPosition = SceneManager.Instance.food[0].transform.position;
    }

    public void SetFleeing(Transform enemy)
    {
        TargetPosition = ((Vector2)transform.position - fleeFrom)*5;
        WanderTimer = 0;
        state = State.Flee;
        fleeFrom = enemy.position;
        Debug.Log("Fleeing");
    }
    private void Flee()
    {
        speed = movementSpeed * 2;
        if ((TargetPosition - transform.position).magnitude < 0.1)
        {
            Debug.Log("Wandering");
            state = State.Wander;
        }
            
    }

    private void TargetMouse()
    {
        WanderTimer = 0;
        TargetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        TargetPosition.z = 0f;
        mouseDown = true;
    }

    private void Wander()
    {
        
        if (WanderTimer == 0)
        {
            TargetPosition = new Vector3(Random.Range(-6f, 6f), Random.Range(-4f, 4f), 0);
        }
        WanderTimer += Time.fixedDeltaTime;
        if (WanderTimer > WanderDurationMax || (TargetPosition - transform.position).magnitude < 0.1)
        {
            WanderTimer = 0;
        }
    }

    void AvoidObstacles()
    {
        bool hitLeft = CastObstacleDetectionWhisker(whiskerAngle, Color.blue, whiskerLength);
        bool hitRight = CastObstacleDetectionWhisker(-whiskerAngle, Color.red, whiskerLength);
        bool hitFarLeft = CastShipDetectionWhisker(whiskerAngle + 50f, Color.cyan,farWhiskerLength);
        bool hitFarRight = CastShipDetectionWhisker(-whiskerAngle - 50f, Color.magenta,farWhiskerLength);
        bool hitCenter = CastObstacleDetectionWhisker(0f, Color.yellow, centerWhiskerLength);

        if (hitCenter)
            velocityWeight = 0.5f;
        else if (hitFarLeft || hitFarRight)
            velocityWeight = 2f;
        else
            velocityWeight = 1f;
            
        // The scenario where both whiskers are triggered could also be addressed with an alteration to velocity
        if(hitLeft)
            RotateClockwise();
        else if (hitRight) 
            RotateCounterClockwise();
    }

    private void RotateCounterClockwise()
    {
        transform. Rotate(Vector3.forward, rotationSpeed * avoidanceWeight * Time.fixedDeltaTime);
    }
    private void RotateClockwise()
    {
        transform.Rotate(Vector3.forward, -rotationSpeed * avoidanceWeight * Time.fixedDeltaTime);
    }

    // Detects all other colliders
    private bool CastObstacleDetectionWhisker(float angle, Color rayColour, float length)
    {
        bool hitResult = false;
        
        Vector2 whiskerDirection = Quaternion.Euler(0f, 0f, angle) * transform.up;
        
        RaycastHit2D[] hit;
        
        hit = Physics2D.RaycastAll(transform.position, whiskerDirection, length);

        foreach (var h in hit)
        {
            if (h.collider != null && h.collider != bc)
            {
                //Debug.Log("Obstacle detected.");
                rayColour = Color.green;
                hitResult = true;
                break;
            }
        }

        
        Debug.DrawRay(transform.position, whiskerDirection * length, rayColour);
        
        return hitResult;
    }
    
    // Detects other ships on the principle that ships use box colliders, can obviously be made more specific
    private bool CastShipDetectionWhisker(float angle, Color rayColour, float length)
    {
        bool hitResult = false;
        
        Vector2 whiskerDirection = Quaternion.Euler(0f, 0f, angle) * transform.up;
        
        RaycastHit2D[] hit;
        
        hit = Physics2D.RaycastAll(transform.position, whiskerDirection, length);

        foreach (var h in hit)
        {
            if (h.collider as BoxCollider2D != null && h.collider != bc)
            {
                //Debug.Log("Fish detected.");
                rayColour = Color.green;
                hitResult = true;
                break;
            }
        }
        
        Debug.DrawRay(transform.position, whiskerDirection * length, rayColour);
        
        return hitResult;
    }

    public static void SeekKinematic(Rigidbody2D rb, Vector2 target, float speed)
    {
        Vector2 desiredVelocity = ((Vector3)target - rb.transform.position).normalized * (speed * Time.fixedDeltaTime);

        Vector2 steeringForce = desiredVelocity - rb.velocity;
        
        rb.AddForce(steeringForce);
    }
    
    public static void SeekForward(Rigidbody2D rb, Vector2 target, float speed, float rotSpeed)
    {
        Vector2 directionToTarget = ((Vector3)target - rb.transform.position).normalized;

        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg + 90f;

        float angleDifference = Mathf.DeltaAngle(targetAngle, rb.transform.eulerAngles.z);

        float rotationStep = rotSpeed* Time.fixedDeltaTime;

        float rotationAmount = Mathf.Clamp(angleDifference, -rotationStep, rotationStep);
        
        rb.transform.Rotate(Vector3.forward, rotationAmount);

        rb.velocity = rb.transform.up * (speed * Time.fixedDeltaTime);
    }
    

}
