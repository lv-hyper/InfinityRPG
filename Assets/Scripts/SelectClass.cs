using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectClass : MonoBehaviour
{
    [SerializeField] InGame.Data.EnumEntityClass characterClass;

    public void Execute() { 
        PlayerPrefs.SetString("CharacterClassType", characterClass.ToString()); 
    }
}
