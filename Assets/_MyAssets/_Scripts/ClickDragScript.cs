using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ClickDragScript : MonoBehaviour
{
    private bool isDragging = false;
    private Vector2 offset;
    private Rigidbody2D currentlyDraggedObject;

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.GetComponent<Rigidbody2D>() != null)
            {
                isDragging = true;
                currentlyDraggedObject = hit.collider.GetComponent<Rigidbody2D>();
                Debug.Log("Grabbed " + currentlyDraggedObject.gameObject.name);
                //offset = currentlyDraggedObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("Released " + currentlyDraggedObject.gameObject.name);
            isDragging = false;
            currentlyDraggedObject = null;
        }

        if (isDragging && currentlyDraggedObject != null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentlyDraggedObject.MovePosition(mousePosition + offset);
        }
    }
}
