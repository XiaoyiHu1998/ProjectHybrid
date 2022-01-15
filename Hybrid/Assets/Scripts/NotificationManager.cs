using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
public class NotificationManager : MonoBehaviour
{
    public List<AudioSource> audioSources;
    public List<AudioClip> clips;
    public List<string> names;

    private Dictionary<string, AudioClip> clipDictionary;
    private System.IntPtr unityWindow;


    [DllImport("user32.dll")] static extern uint GetActiveWindow();
    [DllImport("user32.dll")] static extern bool SetForegroundWindow(System.IntPtr hWnd);

    private void Start()
    {
        unityWindow = (System.IntPtr)GetActiveWindow();
        clipDictionary = new Dictionary<string, AudioClip>();
        int minListSize = Mathf.Min(clips.Count, names.Count);
        for(int i = 0; i < minListSize; i++)
        {
            clipDictionary.Add(names[i], clips[i]);
        }
    }

    public void NotifyBreak(string name)
    {
        if (!clipDictionary.ContainsKey(name))
            return;

        AudioClip clip = clipDictionary[name];
        for(int i = 0; i < audioSources.Count; i++)
        {
            if (!audioSources[i].isPlaying)
            {
                audioSources[i].clip = clip;
                audioSources[i].Play();
            }
        }

        //SetForegroundWindow(unityWindow);
    }

    public void SubscribeNotification(string name)
    {
        int index = names.IndexOf(name);
        clipDictionary.Add(names[index], clips[index]);
    }

    public void unSubscribeNotification(string name)
    {
        clipDictionary.Remove(name);
    }
}
