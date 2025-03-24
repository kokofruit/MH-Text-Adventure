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
            currentRoom = NavigationManager.instance.currentRoom.name,
            currentInventory = inventory,
            exitStatuses = new(),
            roomDescIndexes = new(),
        };
        foreach (Exit exit in NavigationManager.instance.exits)
        {
            bool[] exitStatus = {exit.isHidden, exit.isLocked};
            playerState.exitStatuses.Add(exitStatus);
        }
        foreach (Room room in NavigationManager.instance.rooms)
        {
            playerState.roomDescIndexes.Add(room.descIndex);
        }

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
            BinaryFormatter bf = new();
            FileStream afile = File.Open(Application.persistentDataPath + "/player.save", FileMode.Open);
            SaveState playerState = (SaveState) bf.Deserialize(afile);
            afile.Close();

            print(playerState.currentRoom);
            Room room = NavigationManager.instance.GetRoomFromName(playerState.currentRoom);
            if (room != null)
            {
                NavigationManager.instance.SwitchRooms(room);
            }
            print(room.name);

            inventory = playerState.currentInventory;

            for (int i = 0; i < NavigationManager.instance.exits.Count; i++)
            {
                NavigationManager.instance.exits[i].isHidden = playerState.exitStatuses[i][0];
                NavigationManager.instance.exits[i].isLocked = playerState.exitStatuses[i][1];
            }

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
