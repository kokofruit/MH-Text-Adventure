using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager instance;
    public Room startingRoom;
    public Room currentRoom;

    public Exit toKeyNorth; //needed to turn exit to visible from hidden

    private Dictionary<string, Room> exitRooms = new();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        DontDestroyOnLoad(instance);
    }

    void Start()
    {
        toKeyNorth.isHidden = true;

        currentRoom = startingRoom;
        Unpack();
    }

    void Unpack()
    {
        string description = currentRoom.description;
        
        exitRooms.Clear();
        foreach (Exit e in currentRoom.exits)
        {
            if (e.isHidden) continue;
            description += "\n  - " + e.description;
            exitRooms.Add(e.direction.ToString(), e.room);
        }

        InputManager.instance.UpdateStory(description);
    }

    public bool SwitchRooms(string dir)
    {
        if (exitRooms.ContainsKey(dir)) // if that exit exists
        {
            currentRoom = exitRooms[dir];
            InputManager.instance.UpdateStory("\nYou go " + dir.ToUpper() +".");
            Unpack();
            return true;
        }
        return false;
    }

    public bool GetItem(string item)
    {
        if (item == "key" && currentRoom.hasKey)
            return true;
        else if (item == "orb" && currentRoom.hasOrb)
        {
            toKeyNorth.isHidden = false;
            //currentRoom.hasOrb = false;
            currentRoom.description = "There used to be a blue orb in here.";
            return true;
        }
        else return false;
    }
}
