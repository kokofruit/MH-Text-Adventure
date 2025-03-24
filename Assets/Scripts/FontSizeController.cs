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
        int pref = PlayerPrefs.GetInt("fontSize", 14);
        fontSize = pref;
        UpdateFontSize();

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

    void UpdateFontSize(){
        foreach (Text text in textList)
        {
            text.fontSize = fontSize;
        }
        PlayerPrefs.SetInt("fontSize", fontSize);
    }
}
