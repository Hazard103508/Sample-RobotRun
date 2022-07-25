using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    public Location location;
}

public enum Location
{
    Left,
    Center,
    Right
}
