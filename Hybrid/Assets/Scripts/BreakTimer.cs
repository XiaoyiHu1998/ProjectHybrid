using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

enum BreakTimerMode
{
    workMode,
    breakMode
}

public class BreakTimer : MonoBehaviour
{
    public UnityEvent onBreaktime;
    public UnityEvent onBreakStart;
    public UnityEvent onBreakEnd;
    public UnityEvent onPromptTimeLimit;

    //Dont access these values in script, only in unity editor.
    public int breakIntervalMinutes;
    public int breakTimeMinutes;
    public int breakTimeIncrementMinutes;
    public int snoozeTimeMinutes;
    public int promptTimeLimitMinutes;
    public int maxAFKTimeSeconds;

    //Conversion to seconds for internal use.
    private int breakIntervalSeconds { get { return breakIntervalMinutes * 60; } }
    private int snoozeTimeSeconds { get { return snoozeTimeMinutes * 60; } }
    private int breakTimeSeconds { get { return breakTimeMinutes * 60; } }
    private int promptTimeLimitSeconds { get { return promptTimeLimitMinutes * 60; } }

    //conditions
    private bool userBehindComputer;
    private bool showingPrompt;
    private BreakTimerMode mode;

    //Timers
    private double userWorkTime;
    private double userBreakTime;
    private double userGoneTime;
    private double promptTime;

    void Start()
    {
        //conditions
        userBehindComputer = true;
        showingPrompt = false;
        mode = BreakTimerMode.workMode;

        //Timers
        userWorkTime = 0;
        userGoneTime = 0;
        userBreakTime = 0;
        promptTime = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (mode)
        {
            case BreakTimerMode.workMode:
                workModeActions();
                break;
            case BreakTimerMode.breakMode:
                breakModeActions();
                break;
        }
    }

    private void workModeActions()
    {
        if (!userBehindComputer)
        {
            userGoneTime += Time.deltaTime;
        }
        else
        {
            userWorkTime += Time.deltaTime;
            if (showingPrompt)
            {
                promptTime += Time.deltaTime;
            }
        }

        if(userGoneTime >= maxAFKTimeSeconds)
        {
            onBreakStart.Invoke();
            userBreakTime += userGoneTime;
            mode = BreakTimerMode.breakMode;
        }
        else if(userWorkTime >= breakIntervalSeconds && !showingPrompt)
        {
            showingPrompt = true;
            onBreaktime.Invoke();
        }
        else if(promptTime >= promptTimeLimitSeconds && showingPrompt)
        {
            SkipBreak();
            onPromptTimeLimit.Invoke();
        }
    }

    private void breakModeActions()
    {
        userBreakTime += Time.deltaTime;

        if(userBreakTime >= breakTimeSeconds)
        {
            mode = BreakTimerMode.workMode;
            userBreakTime = 0;

            onBreakEnd.Invoke();
        }
    }

    public void UserAway()
    {
        userBehindComputer = false;
    }

    public void UserBack()
    {
        userBehindComputer = true;
        userGoneTime = 0;
        Debug.LogWarning("TODO: implement userWorkTime reduction based on userGoneTime");
    }

    public void AcceptBreak()
    {
        showingPrompt = false;
        userWorkTime = 0;
        userBreakTime = 0;
        breakIntervalMinutes += breakTimeIncrementMinutes;

        mode = BreakTimerMode.breakMode;
    }

    public void SnoozeBreak()
    {
        showingPrompt = false;
        userWorkTime -= snoozeTimeSeconds;
    }

    public void SkipBreak()
    {
        userWorkTime = 0;
        breakIntervalMinutes -= breakTimeIncrementMinutes;
        mode = BreakTimerMode.workMode;
    }
}
