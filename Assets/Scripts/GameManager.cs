using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public HashSet<string> inventory = new();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InputManager.instance.onRestart += ResetGame;
        Load();
    }

    private void ResetGame()
    {
        inventory.Clear();
    }

    public void Save()
    {
        // set up data to save
        SaveState playerState = new()
        {
            // current room by name
            currentRoom = NavigationManager.instance.currentRoom.name,
            // inventory as hashset
            currentInventory = inventory,
            // exit statuses as temporary empty list of bool arrays
            exitStatuses = new(),
            // room description indexes as temporary list of ints
            roomDescIndexes = new(),
        };
        // fill exitStatuses list with arrays of exit conditions
        foreach (Exit exit in NavigationManager.instance.exits)
        {
            bool[] exitStatus = {exit.isHidden, exit.isLocked};
            playerState.exitStatuses.Add(exitStatus);
        }
        // fill roomDescIndexes list with room indexes
        foreach (Room room in NavigationManager.instance.rooms)
        {
            playerState.roomDescIndexes.Add(room.descIndex);
        }

        // serialize and close
        BinaryFormatter bf = new();
        FileStream afile = File.Create(Application.persistentDataPath + "/player.save");
        print(Application.persistentDataPath);
        bf.Serialize(afile, playerState);
        afile.Close();
    }

    void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/player.save"))
        {
            // open, deserialize, and close file
            BinaryFormatter bf = new();
            FileStream afile = File.Open(Application.persistentDataPath + "/player.save", FileMode.Open);
            SaveState playerState = (SaveState) bf.Deserialize(afile);
            afile.Close();

            // set current room
            Room room = NavigationManager.instance.GetRoomFromName(playerState.currentRoom);
            if (room != null)
            {
                NavigationManager.instance.SwitchRooms(room);
            }

            // set inventory
            inventory = playerState.currentInventory;

            // set exit conditions
            for (int i = 0; i < NavigationManager.instance.exits.Count; i++)
            {
                NavigationManager.instance.exits[i].isHidden = playerState.exitStatuses[i][0];
                NavigationManager.instance.exits[i].isLocked = playerState.exitStatuses[i][1];
            }

            // set room descriptions
            for (int i = 0; i < NavigationManager.instance.rooms.Count; i++)
            {
                NavigationManager.instance.rooms[i].descIndex = playerState.roomDescIndexes[i];
            }
        }
        else //New player
        {
            NavigationManager.instance.ResetGame();
        }
    }
}
