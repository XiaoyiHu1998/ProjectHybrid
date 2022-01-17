using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum Status{
    Working,
    Break,
    Offline
}

public class peoplemanager : MonoBehaviour
{
    Worker yourworker;
    List<Worker> coworkers;
    public float linespacing;
    public int fontsize;
    public Vector2 leftTopCorner;
    public TextMeshPro template;
    public float headerclearance;
    TextMeshPro header; 
    TextMeshPro youtext; 
    TextMeshPro coworkertext;

    // Start is called before the first frame update
    void Start()
    {
        coworkers = new List<Worker>();

        header = Instantiate(template, posbyid(0),Quaternion.identity);
        youtext = Instantiate(template, posbyid(1), Quaternion.identity);
        coworkertext = Instantiate(template, posbyid(3), Quaternion.identity);
        yourworker = new Worker("yourname", posbyid(2), template, fontsize, this, true); 
    }

    // Update is called once per frame
    void Update()
    {
        //hacky code for debuging, for deployment place this code in 'start'
        header.fontSize = fontsize;
        header.text = "Digital Workplace";
        header.alignment = TextAlignmentOptions.Center;

        youtext.fontSize = fontsize;
        youtext.text = "You:";
        youtext.alignment = TextAlignmentOptions.Left;

        coworkertext.fontSize = fontsize;
        coworkertext.text = "Co-workers:";
        coworkertext.alignment = TextAlignmentOptions.Left;
        //this part is worse, can be deleted on deployment:
        header.transform.position = posbyid(0);
        youtext.transform.position = posbyid(1);
        coworkertext.transform.position = posbyid(3);
        //end of hacky code

        
    }

    public void setYourName(string name)
    {
        yourworker.setName(name);
    }


    //Add a coworker to the list:
    public void AddCoWorker(string name)
    {
        coworkers.Add(new Worker(name, posbyid(coworkers.Count + 4), template, fontsize, this));
    }

    //Set Coworker online/offline indicator:
    public void SetCoworkerOnline(string name)
    {
        SetCoWorkerOnlineStatus(name, Status.Working);
    }

    public void SetCoworkerOffline(string name)
    {
        SetCoWorkerOnlineStatus(name, Status.Offline);
    }

    public void SetCoworkerBreak(string name)
    {
        SetCoWorkerOnlineStatus(name, Status.Break);
    }

    //Set users own online/ofline status
    public void SetYouOnline()
    {
        SetYourOnlineStatus(Status.Working);
    }

    public void SetYouOffline()
    {
        SetYourOnlineStatus(Status.Offline);
    }

    //Gets called when clicking to notify a specific coworker
    public void OnSendNotificationToCoworker(string name)
    {
        Debug.LogError("notified " + name);
        ServerTest.Instance.PingNotification(name);
    }

    private void SetYourOnlineStatus(Status status)
    {
        yourworker.setOnlineStatus(status);
    }

    private void SetCoWorkerOnlineStatus(string name, Status status)
    {
        foreach(Worker w in coworkers)
        {
            if(w.name == name)
            {
                w.setOnlineStatus(status);
            }
        }
    }

    private Vector3 posbyid(int yorder, float zpos = -0.5f)
    {
        return new Vector3(leftTopCorner.x, ypos(yorder), zpos);
    }

    private float ypos(int yorder)
    {
        if(yorder == 0 )
        {
            return leftTopCorner.y;
        }
        return leftTopCorner.y - (linespacing * (yorder + headerclearance));
    }

    private void OnGUI()
    {
        //code to test the code :)
        /*
        if(GUI.Button(new Rect(100,100,100,100), "new user"))
        {
            AddCoWorker(Random.Range((int)0, (int)5).ToString());
        }
        if (GUI.Button(new Rect(200, 200, 100, 100), "random offline"))
        {
            int r = Random.Range((int)0, (int)5);
            Debug.LogError("offline " + r.ToString());
            SetCoworkerOffline(r.ToString());
        }
        */
    }
}

public class Worker
{
    public static float xoffset = 10f;
    GameObject onlineIndicator;
    GameObject pingbutton;
    TextMeshPro nameDisplay;
    public string name;
    public Vector3 position;
    public GameObject dot;
    public bool behindComputer { get; private set; }
    public bool notifyThisPerson;
    peoplemanager parent;

    public Worker(string name, Vector3 position, TextMeshPro templat, int fontsize, peoplemanager parent, bool isYou = false)
    {
        this.name = name;
        this.position = position;
        this.parent = parent;

        onlineIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        onlineIndicator.transform.position = position + new Vector3(-xoffset, 0, 0);
        onlineIndicator.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);

        if (!isYou)
        {
            pingbutton = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pingbutton.transform.position = position + new Vector3(xoffset, 0, 0);
            pingbutton.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            pingbutton.AddComponent<NotificationButtonScript>();
            pingbutton.GetComponent<NotificationButtonScript>().setReference(this);

        }

        nameDisplay = Object.Instantiate(templat, position, Quaternion.identity);

        nameDisplay.fontSize = fontsize;
        nameDisplay.text = name;
        nameDisplay.alignment = TextAlignmentOptions.Center;

        setOnlineStatus(Status.Working);
    }

    public void setOnlineStatus(Status status)
    {
        switch(status){
            case Status.Working:
                onlineIndicator.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                break;
            case Status.Break:
                onlineIndicator.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                break;
            case Status.Offline:
                onlineIndicator.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                break;
            default:
                break;
        }
    }

    public void setName(string name)
    {
        this.name = name;
        nameDisplay.text = this.name;
    }

    public void notifyThisUser()
    {
        parent.OnSendNotificationToCoworker(name);
    }
}
