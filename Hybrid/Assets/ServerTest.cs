using Google.Apis.Auth.OAuth2;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Data = Google.Apis.Sheets.v4.Data;

public class ServerTest : MonoBehaviour
{

    static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    static readonly string ApplicationName = "cUUckedWallpaper";
    static readonly string SpreadsheetId = "18sMDt3EbJkePeUkVRZx8J5IFbCT01eWH2Fy00AcYP-k";
    static SheetsService service;
    List<string> clients = new List<string>();
    List<string> spawnedclients = new List<string>();
    public NotificationManager notifier;
    public string username;
    public GameObject Dummy;
    bool present = false;

    private static ServerTest _instance;

    public static ServerTest Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ServerTest>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        GoogleCredential credential;
        using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(Scopes);
        }
        // Create Google Sheets API service.
        service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        JoinServer(username);
    }

    public IList<IList<object>> ReadEntries(string sheet, string inputrange)
    {
        var range = $"{sheet}!{inputrange}";
        SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(SpreadsheetId, range);

        var response = request.Execute();
        IList<IList<object>> values = response.Values;
        if (values != null && values.Count > 0)
        {
            return values;
        }
        else
        {
            return null;
        }
    }
    static void CreateEntry(string sheet, string inputrange, List<object> oblist)
    {
        var range = $"{sheet}!{inputrange}";
        var valueRange = new ValueRange();

        valueRange.Values = new List<IList<object>> { oblist };

        var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
        var appendReponse = appendRequest.Execute();
    }

    static void UpdateEntry(string sheet, string inputrange, List<object> oblist)
    {
        var range = $"{sheet}!{inputrange}";
        var valueRange = new ValueRange();

        valueRange.Values = new List<IList<object>> { oblist };

        var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        var appendReponse = updateRequest.Execute();
    }

    static void DeleteEntry(string sheet, string inputrange)
    {
        var range = $"{sheet}!{inputrange}";
        var requestBody = new ClearValuesRequest();

        var deleteRequest = service.Spreadsheets.Values.Clear(requestBody, SpreadsheetId, range);
        var deleteReponse = deleteRequest.Execute();
    }

    void JoinServer(string username)
    {
        var oblist = new List<object>() { username };
        CreateEntry("Server", "A:A", oblist);
        present = true;


        List<Data.Request> requests = new List<Data.Request>();
        Request Join = new Request();
        SheetProperties properties = new SheetProperties();
        AddSheetRequest sheet = new AddSheetRequest();
        properties.Title = username;
        sheet.Properties = properties;
        Join.AddSheet = sheet;
        BatchUpdateSpreadsheetRequest body
            = new BatchUpdateSpreadsheetRequest();
        requests.Add(Join);
        body.Requests = requests;
        SpreadsheetsResource.BatchUpdateRequest request = service.Spreadsheets.BatchUpdate(body, SpreadsheetId);
        Data.BatchUpdateSpreadsheetResponse response = request.Execute();
        var oblist2 = new List<object>() { present };
        UpdateEntry(username, "A1:A1", oblist2);
        InvokeRepeating("UpdateServer", 1f, 1f); 
    }



    void UpdateServer()
    {
        UpdateClients();

        foreach (string client in clients)
        {
            if (!spawnedclients.Contains(client))
            {
                Instantiate(Dummy).name = client;
                spawnedclients.Add(client);
            }
        }
        UpdateNotifications();
    }

    void UpdateNotifications()
    {
        IList<IList<object>> list = ReadEntries(username, "B:B");
        List<string> filteredlist = new List<string>();
        foreach (var item in list)
        {
            if ((string)item[0] != username)
            {
                if (!filteredlist.Contains((string)item[0]))
                {
                    filteredlist.Add((string)item[0]);
                    notifier.NotifyBreak((string)item[0]);
                }
            }
        }


        DeleteEntry(username, "B:B");
    }

    void UpdateClients()
    {
        clients.Clear();
        IList<IList<object>> list = ReadEntries("Server", "A1:A50");

        foreach (var item in list)
        {
            if((string)item[0] != username)
            {
                clients.Add(item[0].ToString());
            }
        }
    }


    public void TogglePresence(bool _present)
    {
        Debug.Log(_present);
        present = _present;
        var oblist = new List<object>() { present };
        UpdateEntry(username, "A1:A1", oblist);
    }

    public void PingNotification(string colleague)
    {
        var oblist = new List<object>() { username };
        CreateEntry(colleague, "B:B", oblist);
    }

}
