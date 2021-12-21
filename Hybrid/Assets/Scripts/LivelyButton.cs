using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivelyButton : MonoBehaviour
{
    public RectTransform rectTransform;
    public ButtonAction buttonAction;

    float upperBound;
    float lowerBound;
    float leftBound;
    float rightBound;

    bool clicked;

    void Start()
    {
        Vector3 position = rectTransform.position;
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        upperBound = (float)position.y + 0.5f * height;
        lowerBound = (float)position.y - 0.5f * height;
        leftBound  = (float)position.x - 0.5f * width;
        rightBound = (float)position.x + 0.5f * width;

        clicked = false;
    }

    private bool CheckMouseInBounds(Vector3 mousePosition)
    {
        return mousePosition.x >= leftBound && mousePosition.x <= rightBound && mousePosition.y >= lowerBound && mousePosition.y <= upperBound;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && CheckMouseInBounds(Input.mousePosition))
        {
            buttonAction.LeftClick();
            clicked = true;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && CheckMouseInBounds(Input.mousePosition))
        {
            buttonAction.RightClick();
            clicked = true;
        }
        if (clicked && Input.GetKeyUp(KeyCode.Mouse0))
        {
            buttonAction.LeftClickRelease();
        }
        if (clicked && Input.GetKeyUp(KeyCode.Mouse1))
        {
            buttonAction.RightClickRelease();
        }
    }
}
