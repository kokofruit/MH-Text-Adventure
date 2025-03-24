using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager instance;
    public Room startingRoom;
    public Room currentRoom;
    public List<Room> rooms; // needed to restore room upon load
    public List<Exit> exits;

    public delegate void GameOver();
    public event GameOver onGameOver;

    public List<Exit> hiddenExits;
    public List<Exit> lockedExits;

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
        // get current description
        string description = currentRoom.descriptions[currentRoom.descIndex];
        // update description if applicable
        if (currentRoom.autoIncrement && currentRoom.descIndex < currentRoom.descriptions.Length-1) currentRoom.descIndex += 1;
        
        // get exit descriptions
        exitRooms.Clear();
        foreach (Exit e in currentRoom.exits)
        {
            if (e.isHidden) continue;
            description += "\n  - " + e.description;
            exitRooms.Add(e.direction.ToString(), e.room);
        }

        // display
        InputManager.instance.UpdateStory(description);

        // end game if no usable exits
        if (exitRooms.Count == 0 && onGameOver != null) onGameOver();
    }

    public int SwitchRooms(string dir)
    {
        if (exitRooms.ContainsKey(dir)) // if that exit exists
        {
            if (GetExit(dir).isLocked) return 4;
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

    Exit GetExit(string dir)
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
            if (currentRoom.descIndex < currentRoom.descriptions.Length-1) currentRoom.descIndex += 1;
            return true;
        }
        else return false;
    }
    
    public bool UseItem(string item){
        // if the item is not usable here, return false
        if (item != currentRoom.usuableItem) return false;

        // change room description
        if (currentRoom.descIndex < currentRoom.descriptions.Length-1) currentRoom.descIndex += 1;

        if (item == "key" || item == "handle"){
            // special dialogue
            if (item == "key") InputManager.instance.UpdateStory("You twist the key inside the eastern door's lock, and you hear a loud click.");
            if (item == "handle") InputManager.instance.UpdateStory("After some fiddling, you fit the handle into the cranking mechanism, and open up the north door.");
            // unlock exits
            foreach (Exit exit in currentRoom.exits){
                exit.isLocked = false;
            }
        }
        
        if (item == "orb" || item == "sword"){
            // special dialogue
            if (item == "orb") InputManager.instance.UpdateStory("As you place the orb on top of the pedestal, you hear mechanical humming beside you.");
            if (item == "sword") InputManager.instance.UpdateStory("The fight is short but precarious. After a few close calls, you slay the beast with a fatal strike to it's neck.");
            // unhide exits
            foreach (Exit exit in currentRoom.exits){
                exit.isHidden = false;
            }
        }

        SwitchRooms(currentRoom);

        return true;
    }

    public void ResetGame()
    {
        foreach (Exit exit in exits){
            exit.isHidden = exit.isHiddenByDefault;
            exit.isLocked = exit.isLockedByDefault;
        }

        foreach (Room room in rooms)
        {
            room.descIndex = 0;
        }

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
