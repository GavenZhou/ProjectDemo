using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CharacterData {

    // 
    public int ID;
    public string name;
    public string model;
    public string icon;
    public int level;
    public float moveSpeed;
    public float rotSpeed;
    public float colliderLength;
    public float colliderWidth;
    public float colliderRadius;
    public float radius;
    public string beHurt;
    public long maxHP;
    public long maxMP;
    public long maxPHA;
    public long PHD;
    public long dogRATE;
    public long cirRATE;
    public long cirDAM;
    public float deadExpRadio;
    public int aiType;
    public int faction;
}

[System.Serializable]
public class CharacterTable : ScriptableObject {

    // 
    public List<CharacterData> lstCharacter = new List<CharacterData>();
}


