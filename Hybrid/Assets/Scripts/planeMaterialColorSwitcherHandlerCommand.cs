using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeMaterialColorSwitcherHandlerCommand : MonoBehaviour
{
    Material mat;
    // Start is called before the first frame update
    public List<Color> colors;
    int colorIndex = 0;
    void Start()
    {
        mat = gameObject.GetComponent<Renderer>().material;
    }

    public void IncrementColor()
    {
        colorIndex = (colorIndex + 1) % colors.Count;
        mat.color = colors[colorIndex];
    }
}
