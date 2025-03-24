using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontSizeController : MonoBehaviour
{
    public List<Text> textList;
    
    public Button minusButton;
    public Button plusButton;

    int fontSize;

    void Start()
    {
        // load fontsize from playerprefs, 14 by default
        int pref = PlayerPrefs.GetInt("fontSize", 14);
        fontSize = pref;
        UpdateFontSize();

        // add listeners to buttons
        minusButton.onClick.AddListener(DecreaseFontSize);
        plusButton.onClick.AddListener(IncreaseFontSize);
    }

    public void DecreaseFontSize(){
        fontSize -= 1;
        UpdateFontSize();
    }

    public void IncreaseFontSize(){
        fontSize += 1;
        UpdateFontSize();
    }

    // set all text's font to the fontsize
    void UpdateFontSize(){
        foreach (Text text in textList)
        {
            text.fontSize = fontSize;
        }
        PlayerPrefs.SetInt("fontSize", fontSize);
    }
}
