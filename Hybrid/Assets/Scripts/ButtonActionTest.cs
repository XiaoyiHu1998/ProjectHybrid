using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ButtonActionTest : ButtonAction
{
    public Text text;

    public override void LeftClick()
    {
        text.text = "Status: LeftClick";
    }

    public override void LeftClickRelease()
    {
        text.text = "Status: LeftClickRelease";
    }
}
