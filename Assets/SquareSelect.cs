using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareSelect : MonoBehaviour
{
    private SpriteRenderer sprRenderer;

    private bool isSelected;
    public static bool mouseOverSquare;


    private Vector2 mousePos;

    private float dragOffsetX, dragOffsetY;

    void Start()
    {
        sprRenderer = GetComponent<SpriteRenderer>();
        isSelected = false;
        mouseOverSquare = false;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<BoxSelection>())
        {
            sprRenderer.color = new Color(1f, 0f, 0f, 1f);
            isSelected = true;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<BoxSelection>() && Input.GetMouseButton(0))
        {
            sprRenderer.color = new Color(1f, 1f, 1f, 1f);
            isSelected = false;
        }
    }
}
