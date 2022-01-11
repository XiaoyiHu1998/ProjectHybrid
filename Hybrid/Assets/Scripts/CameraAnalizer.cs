using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraAnalizer : MonoBehaviour
{
    public UnityEvent leaveComputer;
    public UnityEvent returnToComputer;

    //settings:
    public float noiseTreshold = 0.05f;
    public float movementTreshold = 35;

    public bool useDefaultCamera = true;
    public bool debugPrintDevices = false;

    public bool cameraToGray = false;
    public bool negativesToBlack = true;

    int buffersize = 3;
    int frameCompareDiference = 1; //MAKE SURE THIS IS STRICTLY LESS THAN BUFFERSIZE

    int nrOframeAverages = 20;

    int nrOmoveFrames = 200;

    //Some necery changing variables:
    WebCamTexture camTexture;
    Renderer renderer;

    Texture2D[] buffer;
    float[] bufferTimes;
    int bufferPointer;

    int[,] frameaverages;
    int frameAvgPointer;

    bool[] moveFrames;
    public int moveFrPointer;

    Texture2D newFrame;
    Texture2D oldFrame;
    Texture2D toShow;

    //Outputs
    public bool atComputer;
    public float totalTimeAtComputer;
    public float timeSinceLastLeave;

    //tempvars:
    bool lastAtComputerState;

    // Start is called before the first frame update
    void Start()
    {
        //Pick the first availible webcam and set it as camTexture
        InitializeWebcam(useDefaultCamera, debugPrintDevices);

        //Show the webcam on this object:
        renderer = GetComponent<Renderer>();
        camTexture.Play();

        //initialize buffer and other looping arrays:
        buffer = new Texture2D[buffersize];
        bufferTimes = new float[buffersize];
        bufferPointer = 0;
        for(int i = 0; i < buffersize; i ++)
        {
            Texture2D temp = new Texture2D(camTexture.width, camTexture.height);
            temp.Apply();
            buffer[i] = temp;

        }

        frameaverages = new int[nrOframeAverages, 2];
        frameAvgPointer = 0;

        moveFrames = new bool[nrOmoveFrames];
        moveFrPointer = 0;

        //Reasignable frame for comparison
        toShow = new Texture2D(camTexture.width, camTexture.height);
        toShow.Apply();

        oldFrame = new Texture2D(camTexture.width, camTexture.height);
        oldFrame.Apply();

        //Prep outputs:
        atComputer = true;
        lastAtComputerState = true;
        totalTimeAtComputer = 0f;
        timeSinceLastLeave = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //update timers:
        if(atComputer)
        {
            totalTimeAtComputer += Time.deltaTime;
            timeSinceLastLeave += Time.deltaTime;
        }
        else
        {
            timeSinceLastLeave = 0;
        }

        //Make new frame to avoid shalow copy problems
        newFrame = buffer[bufferPointer];
        newFrame.SetPixels(camTexture.GetPixels()); //Set new frame to the pixels of current camera frame
        newFrame.Apply();

        FrameToBuffer(newFrame);

        oldFrame.SetPixels( buffer[(bufferPointer - 1 + frameCompareDiference) % buffersize].GetPixels());
        oldFrame.Apply();

        Color[] oldColor = oldFrame.GetPixels();
        Color[] newColor = newFrame.GetPixels();

        Color[] processedFrame = FrameDifference(newColor, oldColor, cameraToGray, negativesToBlack);
        toShow.SetPixels(processedFrame);
        toShow.Apply();

        renderer.material.mainTexture = toShow;
    }

    private void InitializeWebcam(bool pickFirst = true, bool printList = false)
    {
        WebCamDevice[] webcamdevices = WebCamTexture.devices;
        if(pickFirst)
        {
            camTexture = new WebCamTexture(webcamdevices[0].name);
        }
        if(printList)
        { 
            Debug.LogError("These are all availible devices (first one is default) theres currently no way to select any other webcam");
            for(int i =0; i < webcamdevices.Length; i ++)
            {
                Debug.LogError(webcamdevices[i].name);
            }
        }
    }

    private void FrameToBuffer(Texture2D addTexture)
    {
        //Add frame to the buffer
        buffer[bufferPointer] = addTexture;
        bufferTimes[bufferPointer] = Time.deltaTime;
        bufferPointer = (bufferPointer + 1) % buffersize;
    }

    private Color[] FrameDifference(Color[] newcolor, Color[] oldcolor, bool setRestToGray, bool negativesToBlack)
    {
        int nrOpixels = newcolor.Length;
        Color[] output = new Color[nrOpixels];

        //variables for determiening the center of all moving pixels:
        int xconcec = 0; 
        int yconsec = 0;
        int nrOconsec = 0;

        for(int i = 0; i < nrOpixels; i++)
        {
            float oldGray = oldcolor[i].grayscale;
            float newGray = newcolor[i].grayscale;
            if(oldGray + noiseTreshold < newGray)
            {
                output[i] = Color.white;
                xconcec += i % camTexture.width;
                yconsec += i / camTexture.width;
                nrOconsec++;
            }
            else if(negativesToBlack && newGray + noiseTreshold < oldGray)
            {
                output[i] = Color.black;
                xconcec += i % camTexture.width;
                yconsec += i / camTexture.width;
                nrOconsec++;
            }
            else if(setRestToGray)
            {
                output[i] = Color.gray;
            }
            else
            {
                output[i] = newcolor[i];
            }
            
        }

        //find averages of past frames and draw them:
        int xframesum = 0;
        int yframesum = 0;
        int countframesum = 0;

        for (int j = 0; j < nrOframeAverages; j++)
        {
            int locx = frameaverages[j, 0];
            int locy = frameaverages[j, 1];
            output[locx + locy * camTexture.width] = Color.yellow;

            xframesum += locx;
            yframesum += locy;
            countframesum++;
        }

        ////Find multyframe average position and draw big red dot there
        //int xloc = xframesum / countframesum;
        //int yloc = yframesum / countframesum;
        //DrawRectangle(output, xloc, yloc, 5, camTexture.width, Color.red);


        //calculate average active point this update and add it to the list
        if (nrOconsec > 0)
        {
            int xavg = xconcec / nrOconsec;
            int yavg = yconsec / nrOconsec;
            frameaverages[frameAvgPointer, 0] = xavg;
            frameaverages[frameAvgPointer, 1] = yavg;
            frameAvgPointer = (frameAvgPointer + 1) % nrOframeAverages;
        }

        //TODO remove this block of code:
        //temptracker[temptrackerpointer] = nrOconsec;
        //temptrackerpointer = (temptrackerpointer + 1) % nrotemptrecker;
        //int temp = 0;
        //foreach(int nr in temptracker)
        //{
        //    if (nr > temp)
        //        temp = nr;
        //}
        //Debug.LogError(Time.realtimeSinceStartup + " : max this batch " + temp);


        bool movedThisFrame = nrOconsec >= movementTreshold;
        
        moveFrames[moveFrPointer] = movedThisFrame;
        moveFrPointer = (moveFrPointer + 1) % nrOmoveFrames;

        atComputer = false;
        foreach(bool b in moveFrames)
        {
            if(b)
            {
                atComputer = b;
            }
        }

        if(atComputer != lastAtComputerState)
        {
            if (atComputer)
                leaveComputer.Invoke();
            else
                returnToComputer.Invoke();
            Debug.LogError("At computer state changed to: " + atComputer + " at " + ((int)Time.realtimeSinceStartup / 60) + ':' + (Mathf.Round(Time.realtimeSinceStartup) % 60));
        }
        lastAtComputerState = atComputer;

        return output;
    }

    private void DrawRectangle(Color[] input, int x, int y, int size, int width, Color fill)
    {
        for(int i = -width/2; i < width/2; i++)
        {
            for(int j = -width/2; i < width/2; j++)
            {
                int pos = x + i + j * width + y * width;
                if(pos < input.Length && pos >= 0)
                {
                    input[pos] = fill;
                }
            }
        }
    }
}
