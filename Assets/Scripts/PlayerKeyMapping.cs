using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerKeyMapping
{
    [HideInInspector] public bool ready = false;
    [HideInInspector] public PlayerModel playerModel;
    [HideInInspector] public int side = 0;
    public KeyCode enterKey;
    public List<KeyMapping> attackCodes;
}
