using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Windows.Input;

public class Move : MonoBehaviour
{
    Renderer shader;
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        shader = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime;
        transform.position += Vector3.one * Time.deltaTime;
        if (Input.GetKey(KeyCode.P))
        {
            shader.material.color = Color.red;
        }
        text.text = "pos = " + Input.mousePosition + ", down = " + Input.GetKey(KeyCode.Mouse0) + " W = " + Input.GetKey(KeyCode.Space);
    }

    public void SetColor()
    {
        shader.material.color = Color.red;
    }
}
