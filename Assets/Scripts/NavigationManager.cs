using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager instance;
    public Room startingRoom;
    public Room currentRoom;
    public List<Room> rooms; // needed to restore room upon load

    public delegate void GameOver();
    public event GameOver onGameOver;

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
        InputManager.instance.onRestart += ResetGame;
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

        if (exitRooms.Count == 0 && onGameOver != null) onGameOver();
    }

    public int SwitchRooms(string dir)
    {
        if (exitRooms.ContainsKey(dir)) // if that exit exists
        {
            if (getExit(dir).isLocked && !GameManager.instance.inventory.Contains("key")) return 4;
            currentRoom = exitRooms[dir];
            InputManager.instance.UpdateStory("You go " + dir.ToUpper() +".");
            Unpack();
            return 0;
        }
        return 3;
    }

    public void SwitchRooms(Room room)
    {
        currentRoom = room;
        Unpack();
    }

    Exit getExit(string dir)
    {
        foreach (Exit e in currentRoom.exits)
        {
            if (e.direction.ToString() == dir)
            {
                return e;
            }
        }
        return null;
    }

    public bool GetItem(string item)
    {
        if (item == currentRoom.containedItem){
            if (item == "orb") toKeyNorth.isHidden = false;
            return true;
        }
        else return false;
    }
    
    public bool UseItem(string item){
        if (item != currentRoom.usuableItem) return false;

        if (item == "key" || item == "handle"){
            foreach (Exit exit in currentRoom.exits){
                exit.isLocked = false;
            }
        }
        
        if (item == "orb" || item == "sword"){
            foreach (Exit exit in currentRoom.exits){
                exit.isHidden = false;
            }
        }

        return true;
    }

    public void ResetGame()
    {
        toKeyNorth.isHidden = true;

        currentRoom = startingRoom;
        Unpack();
    }

    public Room GetRoomFromName(string name)
    {
        foreach (Room r in rooms)
        {
            if (r.name == name) return r;
        }
        return null;
    }
}
