using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        IList<IList<object>> list = ServerTest.Instance.ReadEntries(gameObject.name, "A1:C1");



        gameObject.transform.position = new Vector3(int.Parse((string)list[0][0]), int.Parse((string)list[0][1]), int.Parse((string)list[0][2]));
    }
}
