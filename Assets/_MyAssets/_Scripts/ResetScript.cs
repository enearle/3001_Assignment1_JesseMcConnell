using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScript : MonoBehaviour
{

    [SerializeField] private Transform ship;
    [SerializeField] private Transform planet;
    private Vector3 shipInitLoc;
    private Vector3 planetInitLoc;
    private Camera c;
    private List<GameObject> addedShips = new List<GameObject>();
   
    private void Awake()
    {
        shipInitLoc = ship.position;
        planetInitLoc = planet.position;
        c = Camera.main;
    }

    public void ResetLocations()
    {
        ship.position = shipInitLoc;
        planet.position = planetInitLoc;

        foreach (var s in addedShips)
        {
            Destroy(s);
        }
        addedShips.Clear();
    }

    public void AddShip()
    {
        Vector3 maxXY = c.ViewportToWorldPoint(new Vector3(1,1,c.nearClipPlane));
        Vector3 minXY = c.ViewportToWorldPoint(new Vector3(0,0,c.nearClipPlane));
        GameObject s = Instantiate(ship.gameObject);
        addedShips.Add(s);
        s.transform.position = new Vector3(Random.Range(minXY.x, maxXY.x), Random.Range(minXY.y, maxXY.y), 0);
    }
}
