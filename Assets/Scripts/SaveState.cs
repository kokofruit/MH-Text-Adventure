using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveState
{
    public string currentRoom;
    public HashSet<string> currentInventory;
    public List<bool[]> exitStatuses;
    public List<int> roomDescIndexes;
}
