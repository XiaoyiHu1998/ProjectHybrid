using Google.Apis.Auth.OAuth2;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

public class ServerTest : MonoBehaviour
{

    static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    static readonly string ApplicationName = "cUUckedWallpaper";
    static readonly string SpreadsheetId = "18sMDt3EbJkePeUkVRZx8J5IFbCT01eWH2Fy00AcYP-k";
    static readonly string sheet = "Server";
    static SheetsService service; 
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

        ReadEntries();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static void ReadEntries()
    {
        var range = $"{sheet}!A:F";
        SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(SpreadsheetId, range);

        var response = request.Execute();
        IList<IList<object>> values = response.Values;
        if (values != null && values.Count > 0)
        {
            foreach (var row in values)
            {
                // Print columns A to F, which correspond to indices 0 and 4.
                Debug.Log(row[0] + ", " + row[1] + ", " + row[2] + ", " +  row[3] + ", " + row[4] + ", " +  row[5]);
            }
        }
        else
        {
            Debug.Log("No data found.");
        }
    }
}
