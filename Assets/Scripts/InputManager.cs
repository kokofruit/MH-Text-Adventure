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

    // first step is creating a using delegate
    public delegate void Restart(); // create delegate
    public event Restart onRestart; // create delegate event

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
        commands.Add("commands");
        commands.Add("current");
        commands.Add("go");
        commands.Add("get");
        commands.Add("inventory");
        commands.Add("use");
        commands.Add("restart"); // added to work with delegate example
        commands.Add("save");

        userInput.onEndEdit.AddListener(ProcessInput);
        story = storyText.text;

        NavigationManager.instance.onGameOver += EndGame;
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
                UpdateStory("\n> " + msg.ToUpper());
                switch (splitMsg[0])
                {
                    case "commands":
                        UpdateStory("\n Valid Commands:");
                        foreach (string command in commands){
                            string printOut = "  · " + command;
                            if (command == "go") printOut += " [direction]";
                            if (command == "get") printOut += " [item]";
                            if (command == "use") printOut += " [item]";
                            UpdateStory(printOut);
                        }
                        break;

                    case "current":
                        // print current description
                        NavigationManager.instance.SwitchRooms(NavigationManager.instance.currentRoom);
                        break;

                    case "go":
                        int exitCode = NavigationManager.instance.SwitchRooms(splitMsg[1]);
                        if (exitCode == 0)
                        {
                            //Successful move
                        }
                        else if (exitCode == 3)
                        {
                            UpdateStory("Invalid direction.");
                        }
                        else if (exitCode == 4)
                        {
                            UpdateStory("Door is locked.");
                        }
                        break;

                    case "get":
                        if (NavigationManager.instance.GetItem(splitMsg[1]))
                        {
                            if (GameManager.instance.inventory.Add(splitMsg[1]))
                            {
                                UpdateStory("You got: " + splitMsg[1].ToUpper() + " ★");
                                NavigationManager.instance.SwitchRooms(NavigationManager.instance.currentRoom);
                            }
                            else
                                UpdateStory("You already have this.");
                        }
                        else
                        {
                            UpdateStory("Invalid item.");
                        }
                        break;

                    case "inventory":
                        UpdateStory("\nCurrent Inventory:");
                        foreach (string item in GameManager.instance.inventory)
                        {
                            UpdateStory("  · " + item);
                        }
                        break;

                    case "use":
                        if (GameManager.instance.inventory.Contains(splitMsg[1])){
                            if (NavigationManager.instance.UseItem(splitMsg[1])){
                                // item used successfully
                            }
                            else UpdateStory("This item has no use here.");
                        }
                        else UpdateStory("You do not have this item. Use 'INVENTORY' to see your items.");
                        break;

                    case "restart":
                        if (onRestart != null) // if anyone is listening
                            onRestart(); // invoke the event
                        break;

                    case "save":
                        GameManager.instance.Save();
                        UpdateStory("Game saved.");
                        break;
                        
                    default:
                        break;
                } // end switch case
            } // end if valid 
            else UpdateStory("Invalid command. Enter 'COMMANDS' to see all valid commands.");
        } // end if not empty

        // reset for next input
        userInput.text = ""; // after input from user, reset
        userInput.ActivateInputField(); // like clicking back into it
    }

    void EndGame()
    {
        UpdateStory("Game Over! Enter 'RESTART' to play again.\n");
    }
}
