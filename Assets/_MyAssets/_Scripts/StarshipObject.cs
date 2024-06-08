using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
/*
 *  For a whiskers implementation, I was relatively happy with the steering behaviour as it was, and instead opted
 *  to use the additional whiskers to alter the velocity.
 *
 *  You can think of this behaviour almost like a flight controller that sets protocol for extreme angles of
 *  collision.(T-Bone)
 *
 *  If a ship is detected to the far left/right we accelerate to get out of the way.
 *
 *  Inversely, if we detect anything in front of us we slow down, yielding passage.
 *
 *  Also, I set the far right/left whiskers to be slightly angled, as this will help prevent ships racing each other.
 *
 *  I created a cherry picked scenario where this kinda works but the steering interferes. It would be better to
 *  incorporate the velocity of the object we're trying to avoid, so we dont steer into it's path. Or use an additional forward extended
 *  collision capsule on ships to function as the projected path.
 */
public class StarshipObject : AgentObject
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float slowDownRadius = 2;
    [SerializeField] private float arriveRadius = 0.05f;
    [SerializeField] private float whiskerLength = 1f;
    [SerializeField] private float farWhiskerLength = 0.6f;
    [SerializeField] private float centerWhiskerLength = 1.8f;
    [SerializeField]private float whiskerAngle = 30;
    [SerializeField] private float avoidanceWeight;
    
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private float velocityWeight = 1f;
    void Start()
    {
        Debug.Log("Starting Starship");
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (TargetPostition != null)
        {
            //SeekKinematic();
            if((TargetPostition - transform.position).magnitude <= arriveRadius)
                rb.velocity = Vector2.zero;
            else if ((TargetPostition - transform.position).magnitude <= slowDownRadius)
                SeekForward(rb,TargetPostition, movementSpeed * 0.5f, rotationSpeed);
            else
                SeekForward(rb,TargetPostition, movementSpeed, rotationSpeed);
            
            AvoidObstacles();
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
                Debug.Log("Obstacle detected.");
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
                Debug.Log("Ship detected.");
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Target")
            GetComponent<AudioSource>().Play();
    }
}
