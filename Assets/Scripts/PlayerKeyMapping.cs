using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerKeyMapping
{
    public bool ready = false;
    public Transform transform;
    public KeyCode enterKey;
    public List<KeyMapping> attackCodes;
}
