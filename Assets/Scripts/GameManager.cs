using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<string> inventory = new();

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
        SaveState playerState = new();
        playerState.currentRoom = NavigationManager.instance.currentRoom.name;

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

            Room room = NavigationManager.instance.GetRoomFromName(playerState.currentRoom);
            if (room != null)
            {
                NavigationManager.instance.SwitchRooms(room);
            }
        }
        else //New player
        {
            NavigationManager.instance.ResetGame();
        }
    }
}
