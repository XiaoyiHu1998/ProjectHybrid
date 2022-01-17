using Google.Apis.Auth.OAuth2;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Data = Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;

public class ServerTest : MonoBehaviour
{
    
    static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    static readonly string ApplicationName = "cUUckedWallpaper";
    static readonly string SpreadsheetId = "18sMDt3EbJkePeUkVRZx8J5IFbCT01eWH2Fy00AcYP-k";
    static SheetsService service;
    int? sheetId;
    List<string> clients = new List<string>();
    List<string> workingClients = new List<string>();
    List<string> offlineClients = new List<string>();
    List<string> breakingClients = new List<string>();
    List<string> guiClients = new List<string>();


    public NotificationManager notifier;
    public peoplemanager people;

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

        string spreadsheetId = "18sMDt3EbJkePeUkVRZx8J5IFbCT01eWH2Fy00AcYP-k";  // TODO: Update placeholder value.

        // The ranges to retrieve from the spreadsheet.
        List<string> ranges = new List<string>();  // TODO: Update placeholder value.

        // True if grid data should be returned.
        // This parameter is ignored if a field mask was set in the request.
        bool includeGridData = false;  // TODO: Update placeholder value.

        SpreadsheetsResource.GetRequest request2 = service.Spreadsheets.Get(spreadsheetId);
        request2.Ranges = ranges;
        request2.IncludeGridData = includeGridData;

        // To execute asynchronously in an async method, replace `request.Execute()` as shown:
        Data.Spreadsheet response2 = request2.Execute();
        // Data.Spreadsheet response = await request.ExecuteAsync();

        // TODO: Change code below to process the `response` object:
   
        sheetId = response2.Sheets[response2.Sheets.Count-1].Properties.SheetId;

        var oblist2 = new List<object>() { present };
        UpdateEntry(username, "A1:A1", oblist2);
        InvokeRepeating("UpdateServer", 1f, 20f); 
    }



    void UpdateServer()
    {
        UpdateClients();

        foreach (string client in clients)
        {
            IList<IList<object>> list = ReadEntries(client, "A1:A1");
            if (bool.Parse((string)list[0][0]))
            {
                workingClients.Add(client);
                breakingClients.Remove(client);

            }
            else
            {
                workingClients.Remove(client);
                breakingClients.Add(client);
            }
        }
        foreach (string workingClient in workingClients)
        {
            people.SetCoworkerOnline(workingClient);
        }
        foreach (string breakingClient in breakingClients)
        {
            people.SetCoworkerBreak(breakingClient);
        }
        foreach (string offlineClient in offlineClients)
        {
            people.SetCoworkerOffline(offlineClient);
        }

        UpdateNotifications();
    }

    void UpdateNotifications()
    {
        IList<IList<object>> list = ReadEntries(username, "B:B");
        if(list == null)
        {
            return;
        }
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
        offlineClients.Clear();
        workingClients.Clear();
        breakingClients.Clear();

        IList<IList<object>> list = ReadEntries("Server", "A:A");
        
        foreach (var item in list)
        {
            string name = (string)item[0];
            Debug.Log(name);
            if (name != username)
            {
                if (!offlineClients.Contains(name))
                {
                    offlineClients.Add(name);
                }
                if (!guiClients.Contains(name))
                {
                    people.AddCoWorker(name);
                    Debug.Log(name);
                    guiClients.Add(name);
                }


                if (clients.Contains(name))
                {
                    clients.Remove(name);
                }
                else
                {
                    clients.Add(name);
                }
            }
        }
        foreach (string client in clients)
        {
            offlineClients.Remove(client);
        }
    }


    public void TogglePresence(bool _present)
    {
        Debug.Log(_present);
        present = _present;
        var oblist = new List<object>() { present };
        UpdateEntry(username, "A1:A1", oblist);
    }
    
    public void LogOff()
    {
        var oblist = new List<object>() { username };
        CreateEntry("Server", "A:A", oblist);


    

        


        List<Data.Request> requests = new List<Data.Request>();
        Request Join = new Request();
        SheetProperties properties = new SheetProperties();
        DeleteSheetRequest sheet = new DeleteSheetRequest();
        properties.Title = username;
        sheet.SheetId = sheetId;
        Join.DeleteSheet = sheet;
        BatchUpdateSpreadsheetRequest body
            = new BatchUpdateSpreadsheetRequest();
        requests.Add(Join);
        body.Requests = requests;
        SpreadsheetsResource.BatchUpdateRequest request2 = service.Spreadsheets.BatchUpdate(body, SpreadsheetId);
        Data.BatchUpdateSpreadsheetResponse response2 = request2.Execute();
    }

    public void PingNotification(string colleague)
    {
        var oblist = new List<object>() { username };
        CreateEntry(colleague, "B:B", oblist);
    }

    private void OnApplicationQuit()
    {
        LogOff();
    }
}
