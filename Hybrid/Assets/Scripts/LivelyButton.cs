using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LivelyButton : MonoBehaviour
{
    RectTransform rectTransform;
    public UnityEvent onClickEvents;

    float upperBound;
    float lowerBound;
    float leftBound;
    float rightBound;

    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        
        Vector3 position = rectTransform.position;
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        upperBound = (float)position.y + 0.5f * height;
        lowerBound = (float)position.y - 0.5f * height;
        leftBound  = (float)position.x - 0.5f * width;
        rightBound = (float)position.x + 0.5f * width;
    }

    private bool CheckMouseInBounds(Vector3 mousePosition)
    {
        return mousePosition.x >= leftBound && mousePosition.x <= rightBound && mousePosition.y >= lowerBound && mousePosition.y <= upperBound;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && CheckMouseInBounds(Input.mousePosition))
        {
            onClickEvents.Invoke();
        }
    }
}
