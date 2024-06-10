
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;
    [SerializeField] private GameObject EnemyPrefab;
    [SerializeField] private GameObject FoodPrefab;
    [SerializeField] private GameObject RockPrefab;
    [SerializeField] private GameObject FishPrefab;
    [SerializeField] private int NumberOfFish = 5;
    public List<GameObject> food;
    private List<GameObject> rocks;
    private List<FishObject> fish;
    public List<GameObject> enemies;

    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        else
            Instance = this;
        
        MakeFishHappen();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            Debug.Log("Spawn Enemy");
            GameObject e = Instantiate(EnemyPrefab);
            Vector2 randPos = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized;
            randPos *= 10;
            e.transform.position = randPos;
            enemies.Add(e);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            GameObject f = Instantiate(FoodPrefab);
            f.transform.position = new Vector2(Random.Range(-6f, 6f), Random.Range(-4f, 4f));
            food.Add(f);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            GameObject r = Instantiate(RockPrefab);
            r.transform.position = new Vector2(Random.Range(-6f, 6f), Random.Range(-4f, 4f));
            rocks.Add(r);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneWipe();
            MakeFishHappen();
        }
    }

    private void MakeFishHappen()
    {
        for (int i = 0; i < NumberOfFish; i++)
        {
            /*
            FishObject f = Instantiate(FishPrefab).GetComponent<FishObject>();
            fish.Add(f);
            f.transform.position = new Vector2(Random.Range(-6f, 6f), Random.Range(-4f, 4f));
            Debug.Log("Fish added");
            */
        }

    }

    private void SceneWipe()
    {
        foreach (var f in fish)
        {
            Destroy(f.gameObject);
        }
        fish.Clear();

        foreach (var f in food)
        {
            Destroy(f.gameObject);
        }
        food.Clear();

        foreach (var r in rocks)
        {
            Destroy(r.gameObject);
        }
        rocks.Clear();

        foreach (var e in enemies)
        {
            Destroy(e.gameObject);
        }
        enemies.Clear();
    }
}
