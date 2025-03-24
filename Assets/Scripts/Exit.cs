using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Text/Exit")]
public class Exit : ScriptableObject
{
    public enum Direction { north, south, east, west };
    public Direction direction;
    [TextArea]
    public string description;
    public Room room;
    // used for gameplay
    public bool isLocked;
    public bool isHidden;
    // used for resetting exits
    public bool isLockedByDefault;
    public bool isHiddenByDefault;
}
