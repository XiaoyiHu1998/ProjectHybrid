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
    public UnityEvent onStartedWorkingForAwhile;

    //Dont access these values in script, only in unity editor.
    public int minBreakTimeMinutes;
    public int maxBreakTimeMinutes;
    public int breakIntervalMinutes;
    public int breakTimeMinutes;
    public int breakTimeIncrementMinutes;
    public int snoozeTimeMinutes;
    public int promptTimeLimitMinutes;
    public int autoCloseBreakOverMinutes;
    public int maxAFKTimeSeconds;

    //Conversion to seconds for internal use.
    private int breakIntervalSeconds { get { return breakIntervalMinutes * 60; } }
    private int snoozeTimeSeconds { get { return snoozeTimeMinutes * 60; } }
    private int breakTimeSeconds { get { return breakTimeMinutes * 60; } }
    private int promptTimeLimitSeconds { get { return promptTimeLimitMinutes * 60; } }
    private int autoCloseBreakOverSeconds { get { return autoCloseBreakOverMinutes * 60; } }

    //conditions
    private bool userBehindComputer;
    private bool showingPrompt;
    private bool firedWorkingForAWhileEvent;
    private BreakTimerMode mode;

    //Internal Timers
    private double userWorkTime;
    private double userBreakTime;
    private double userGoneTime;
    private double promptTime;
    private double autoCloseBreakOver;

    void Start()
    {
        //conditions
        userBehindComputer = true;
        showingPrompt = false;
        firedWorkingForAWhileEvent = false;
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
            autoCloseBreakOver += Time.deltaTime;
            if (showingPrompt)
            {
                promptTime += Time.deltaTime;
            }
        }

        if(userGoneTime >= maxAFKTimeSeconds)
        {
            AcceptBreak();
            userBreakTime += userGoneTime;
        }
        else if(userWorkTime >= breakIntervalSeconds && !showingPrompt)
        {
            showingPrompt = true;
            onBreaktime.Invoke();
        }
        else if(promptTime >= promptTimeLimitSeconds && showingPrompt)
        {
            showingPrompt = false;
            promptTime = 0;
            SkipBreak();
            onPromptTimeLimit.Invoke();
        }
        if(autoCloseBreakOver >= autoCloseBreakOverSeconds && !firedWorkingForAWhileEvent)
        {
            onStartedWorkingForAwhile.Invoke();
            firedWorkingForAWhileEvent = true;
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
        userGoneTime = 0;
    }

    public void UserBack()
    {
        userBehindComputer = true;
    }

    public void AcceptBreak()
    {
        showingPrompt = false;
        firedWorkingForAWhileEvent = false;
        userWorkTime = 0;
        userBreakTime = 0;
        breakIntervalMinutes = Mathf.Clamp(breakIntervalMinutes + breakTimeIncrementMinutes, minBreakTimeMinutes, maxBreakTimeMinutes);
        onBreakStart.Invoke();

        mode = BreakTimerMode.breakMode;
    }

    public void SnoozeBreak()
    {
        showingPrompt = false;
        userWorkTime -= snoozeTimeSeconds;
    }

    public void SkipBreak()
    {
        showingPrompt = false;
        userWorkTime = 0;
        breakIntervalMinutes = Mathf.Clamp(breakIntervalMinutes - breakTimeIncrementMinutes, minBreakTimeMinutes, maxBreakTimeMinutes);
        mode = BreakTimerMode.workMode;
    }
}
