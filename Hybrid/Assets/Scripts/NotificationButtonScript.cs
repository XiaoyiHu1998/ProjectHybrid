using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationButtonScript : MonoBehaviour
{
    Worker reference;
    
    public void setReference(Worker refer)
    {
        reference = refer;
    }



    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            reference.notifyThisUser();
        }
    }
}
