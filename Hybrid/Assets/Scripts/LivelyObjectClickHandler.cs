using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LivelyObjectClickHandler : MonoBehaviour
{
    public Canvas canvas;
    public float rayLength;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastObjects();
        }
    }

    private void RaycastObjects()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, rayLength))
        {
            if (hit.transform.gameObject.tag == "Object")
                ObjectClick(hit.transform.gameObject);
        }

    }

    private void ObjectClick(GameObject gameObject)
    {
        gameObject.GetComponent<LivelyEvent>().Invoke();
    }

}
