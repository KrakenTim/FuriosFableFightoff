using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerKeyMapping
{
    public bool ready = false;
    public KeyCode enterKey;
    public List<KeyMapping> attackCodes;
}
