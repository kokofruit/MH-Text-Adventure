using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public Text storyText; // the story 
    public InputField userInput; // the input field object
    public Text inputText; // part of the input field where user enters response
    public Text placeHolderText; // part of the input field for initial placeholder text
    
    private string story; // holds the story to display
    private List<string> commands = new(); //valid user commands

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        commands.Add("go");
        commands.Add("get");

        userInput.onEndEdit.AddListener(ProcessInput);
        story = storyText.text;
    }

    public void UpdateStory(string msg)
    {
        story += "\n" + msg;
        storyText.text = story;
    }

    void ProcessInput(string msg)
    {
        if (msg != "")
        {
            char[] splitInfo = { ' ' };
            string[] splitMsg = msg.ToLower().Split(splitInfo); // 'go north' -> ['go', 'north']

            if (splitMsg.Length > 0 && commands.Contains(splitMsg[0])) // if valid command
            {
                UpdateStory("> " + msg.ToUpper());
                if (splitMsg[0] == "go")
                {
                    if (NavigationManager.instance.SwitchRooms(splitMsg[1]))
                    {
                        //TODO
                    }
                    else
                    {
                        UpdateStory("Invalid direction.");
                    }
                }
                
                else if (splitMsg[0] == "get")
                {
                    if (NavigationManager.instance.GetItem(splitMsg[1]))
                    {
                        GameManager.instance.inventory.Add(splitMsg[1]);
                        UpdateStory("You got: " + splitMsg[1].ToUpper() + " ★");
                    }
                    else
                    {
                        UpdateStory("Invalid item.");
                    }
                }
            }
        }

        // reset for next input
        userInput.text = ""; // after input from user, reset
        userInput.ActivateInputField(); // like clicking back into it
    }

}
