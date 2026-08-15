using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueLines
{
    public string lineText = "";
    public CharacterName characterName;
    public enum CharacterName
    {
        GunGuy,
        MC,
        NPC
    }

}
