using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    public Image background;
    public Text displayText;
    public Text toggleText;

    bool darkMode;
    
    void Start()
    {
        Toggle toggle = GetComponent<Toggle>();
        darkMode = toggle.isOn;
        SetTheme();
    }

    void SetTheme()
    {
        if (darkMode)
        {
            background.color = Color.black;
            displayText.color = Color.white;
            toggleText.color = Color.white;
        }
        else
        {
            background.color = Color.white;
            displayText.color = Color.black;
            toggleText.color = Color.black;
        }
    }
}
