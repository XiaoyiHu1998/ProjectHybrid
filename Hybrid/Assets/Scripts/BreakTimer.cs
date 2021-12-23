using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BreakTimer : MonoBehaviour
{
    public UnityEvent onBreaktime;
    public int breakIntervalMinutes; //Dont access this value in script, only in unity editor.
    private int breakIntervalSeconds { get { return breakIntervalMinutes * 60; } } //Easy conversion to seconds for internal use.
    private bool userBehindComputer;
    private double userWorkTime;
    private double userGoneTime;

    void Start()
    {
        userBehindComputer = true;
        userWorkTime = 0;
        userGoneTime = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (userBehindComputer)
        {
            userWorkTime += Time.deltaTime;
        }
        else
        {
            userGoneTime += Time.deltaTime;
        }

        if(userWorkTime >= breakIntervalSeconds)
        {
            onBreaktime.Invoke();
        }
    }

    public void UserAway()
    {
        userBehindComputer = false;
        userGoneTime = 0;
    }

    public void UserBack()
    {
        userBehindComputer = true;
        Debug.LogWarning("TODO: implement userWorkTime reduction based on userGoneTime");
    }
}
