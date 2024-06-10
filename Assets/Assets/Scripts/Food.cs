
using System;
using UnityEngine;

public class Food : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        SceneManager.Instance.food.Remove(gameObject);
        Destroy(gameObject);
    }
}
