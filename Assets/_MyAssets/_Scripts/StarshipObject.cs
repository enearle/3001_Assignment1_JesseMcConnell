using System;
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
            //SeekKinematic();
            SeekForward();
        }
    }

    private void SeekKinematic()
    {
        Vector2 desiredVelocity = (TargetPostition - transform.position).normalized * (movementSpeed * Time.fixedDeltaTime);

        Vector2 steeringForce = desiredVelocity - rb.velocity;
        
        rb.AddForce(steeringForce);
    }

    private void SeekForward()
    {
        Vector2 directionToTarget = (TargetPostition - transform.position).normalized;

        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg + 90f;

        float angleDifference = Mathf.DeltaAngle(targetAngle, transform.eulerAngles.z);

        float rotationStep = rotationSpeed* Time.fixedDeltaTime;

        float rotationAmount = Mathf.Clamp(angleDifference, -rotationStep, rotationStep);
        
        transform.Rotate(Vector3.forward, rotationAmount);

        rb.velocity = transform.up * (movementSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Target")
            GetComponent<AudioSource>().Play();
    }
}
