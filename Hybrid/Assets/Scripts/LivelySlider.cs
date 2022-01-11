using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivelySlider : MonoBehaviour
{
    RectTransform rectTransform;
    Slider slider;
    bool isSliding = false;

    Vector3 mousePosition;
    float upperBound;
    float lowerBound;
    float leftBound;
    float rightBound;

    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        slider = gameObject.GetComponent<Slider>();

        float halfWidth = gameObject.GetComponent<RectTransform>().rect.width / 2f;
        float halfHeight = gameObject.GetComponent<RectTransform>().rect.height / 2f;
        leftBound  = gameObject.transform.position.x - halfWidth;
        rightBound = gameObject.transform.position.x + halfWidth;

        upperBound = gameObject.transform.position.y + halfHeight;
        lowerBound = gameObject.transform.position.y - halfHeight;
    }

    private bool CheckMouseInBounds(Vector3 mousePosition)
    {
        return mousePosition.x >= leftBound && mousePosition.x <= rightBound && mousePosition.y >= lowerBound && mousePosition.y <= upperBound;
    }

    // Update is called once per frame
    void Update()
    {
        isSliding = Input.GetKey(KeyCode.Mouse0) && CheckMouseInBounds(Input.mousePosition);
        if (isSliding)
        {
            mousePosition = Input.mousePosition;
            float clampedMousePos = Mathf.Clamp(mousePosition.x, leftBound, rightBound) - leftBound;

            slider.value = clampedMousePos / (rightBound - leftBound);
        }
    }
}
