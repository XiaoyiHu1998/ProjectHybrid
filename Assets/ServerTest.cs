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

    public string usernametest;
    public Transform player;
    public GameObject Dummy;

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

        JoinServer(usernametest);
        InitializeClient(usernametest);
    }

    public IList<IList<object>> ReadEntries(string sheet, string inputrange)
    {
        var range = $"{sheet}!{inputrange}";
        SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(SpreadsheetId, range);

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

    static void JoinServer(string username)
    {
        var oblist = new List<object>() { username };
        CreateEntry("Server", "A:A", oblist);


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
    }

    static void InitializeClient(string username)
    {
        var oblist = new List<object>() { 0, 0, 0 };
        CreateEntry(username, "A:C", oblist);
    }

    void Update()
    {
        UpdateClients();
        var oblist = new List<object>() { player.position.x, player.position.y, player.position.z };
        UpdateEntry(usernametest, "A1:C1", oblist);
        foreach (string client in clients)
        {
            if (!spawnedclients.Contains(client))
            {
                Instantiate(Dummy).name = client;
                spawnedclients.Add(client);
            }
        }
    }

    void UpdateClients()
    {
        clients.Clear();
        IList<IList<object>> list = ReadEntries("Server", "A1:A50");

        foreach (var item in list)
        {
            if((string)item[0] != usernametest)
            {
                clients.Add(item[0].ToString());
            }
        }
    }


}
