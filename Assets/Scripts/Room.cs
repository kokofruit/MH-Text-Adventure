using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Text/Room")]
public class Room : ScriptableObject
{
    public string roomName;
    
    [TextArea]
    public string[] descriptions;
    public int descIndex;
    public bool autoIncrement;

    public Exit[] exits;
    
    public string containedItem;
    public string usuableItem;
}
