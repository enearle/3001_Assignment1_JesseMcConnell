
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector2 TargetPosition;
    private float time = 0;
    private void Start()
    {
        TargetPosition = -transform.position;
        Vector2 directionToTarget = ((Vector3)TargetPosition - transform.position).normalized;

        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        
        transform.Rotate(Vector3.forward, targetAngle);
    }

    private void Update()
    {
        time += Time.deltaTime / 10;
        if (time > 1)
        {
            SceneManager.Instance.enemies.Remove(this.gameObject);
            Destroy(gameObject);
        }
            
        transform.position = Vector3.Lerp(transform.position, TargetPosition, time/1000);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FishObject f = other.GetComponent<FishObject>();
        
        if(f != null)
            f.SetFleeing(transform);
        else
        {
            Debug.Log("Cast failed");
        }
    }
}
