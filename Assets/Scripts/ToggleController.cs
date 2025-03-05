using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    public Image background;
    public List<Text> textList;

    bool darkMode;
    
    void Start()
    {
        Toggle toggle = GetComponent<Toggle>();
        
        int pref = PlayerPrefs.GetInt("theme", 1); //uses 1 as default if not already set
        darkMode = toggle.isOn = pref == 1; // set darkmode and toggle to match pref
        SetTheme();

        toggle.onValueChanged.AddListener(ProcessChange);
    }

    void ProcessChange(bool value)
    {
        darkMode = value;
        PlayerPrefs.SetInt("theme", darkMode ? 1 : 0);
        SetTheme();
    }

    void SetTheme()
    {
        Color textColor;
        if (darkMode)
        {
            background.color = Color.black;
            textColor = Color.white;
        }
        else
        {
            background.color = Color.white;
            textColor = Color.black;
        }
        foreach (Text t in textList)
        {
            t.color = textColor;
        }
    }
}
