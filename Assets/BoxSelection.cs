using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSelection : MonoBehaviour
{
    private LineRenderer lineRend;
    private Vector2 initialMousePosition, currentMousePosition;
    private BoxCollider2D boxColl;

    // Start is called before the first frame update
    void Start()
    {
        lineRend = GetComponent<LineRenderer>();
        lineRend.positionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // When left mouse button is pressed and mouse pointer is not over any soldier
        // I create four points at mouse position

        if (Input.GetMouseButtonDown(0) && !SquareSelect.mouseOverSquare)
        {
            lineRend.positionCount = 4;
            initialMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lineRend.SetPosition(0, new Vector2(initialMousePosition.x, initialMousePosition.y));
            lineRend.SetPosition(1, new Vector2(initialMousePosition.x, initialMousePosition.y));
            lineRend.SetPosition(2, new Vector2(initialMousePosition.x, initialMousePosition.y));
            lineRend.SetPosition(3, new Vector2(initialMousePosition.x, initialMousePosition.y));

            // This BoxSelection game object gets a box collider which is set as a trigger
            // Center of this collider is at BoxSelection position

            boxColl = gameObject.AddComponent<BoxCollider2D>();
            boxColl.isTrigger = true;
            boxColl.offset = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }

        // While mouse button is being held down I can draw a rectangle
        // Those four points get corresponding coordinates depending on
        // mouse initial position when button was pressed for the first time
        // and its current position

        if (Input.GetMouseButton(0) && !SquareSelect.mouseOverSquare)
        {
            currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lineRend.SetPosition(0, new Vector2(initialMousePosition.x, initialMousePosition.y));
            lineRend.SetPosition(1, new Vector2(initialMousePosition.x, currentMousePosition.y));
            lineRend.SetPosition(2, new Vector2(currentMousePosition.x, currentMousePosition.y));
            lineRend.SetPosition(3, new Vector2(currentMousePosition.x, initialMousePosition.y));

            // BoxSelection gameobjects position is at the middle of the box drawn

            transform.position = (currentMousePosition + initialMousePosition) / 2;

            // Box collider boundaries outline that box drawn

            boxColl.size = new Vector2(
                Mathf.Abs(initialMousePosition.x - currentMousePosition.x),
                Mathf.Abs(initialMousePosition.y - currentMousePosition.y));
        }

        // When mouse button is released box is wiped, collider is destroyed
        // and BoxSelection gameobject goes back to the center of the scene

        if (Input.GetMouseButtonUp(0))
        { 
            lineRend.positionCount = 0;
            Destroy(boxColl);
            transform.position = Vector3.zero;
        }
    }
}
