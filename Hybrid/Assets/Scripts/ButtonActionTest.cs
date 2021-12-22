using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ButtonActionTest : MonoBehaviour
{
    public Text text;

    public void LeftClick()
    {
        text.text = "Status: LeftClick";
    }

    public void LeftClickRelease()
    {
        text.text = "Status: LeftClickRelease";
    }
}
