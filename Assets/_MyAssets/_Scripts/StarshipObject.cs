using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarshipObject : AgentObject
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;

    private Rigidbody2D rb;
    void Start()
    {
        Debug.Log("Starting Starship");
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TargetPostition != null)
        {
            SeekKinematic();
        }
    }

    private void SeekKinematic()
    {
        Vector2 desiredVelocity = (TargetPostition - transform.position);

        Vector2 steeringForce = desiredVelocity - rb.velocity;
        
        rb.AddForce(steeringForce);
    }
}
