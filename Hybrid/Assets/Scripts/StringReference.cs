using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringReference
{
    public bool useConstant = false;
    public string constantValue;
    public StringVariable variable;

    public string Value
    {
        get { return useConstant ? constantValue : variable.value; }
        set { variable.value = value; }
    }
}
