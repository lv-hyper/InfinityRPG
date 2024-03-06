using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using InGame.Entity;

public class CharacterFollower : MonoBehaviour
{
    private Character character;
    private void Awake()
    {
        StartCoroutine(RegisterToCharacter());
    }
    
    IEnumerator RegisterToCharacter()
    {
        Character character = null;

        while (true)
        {
            character = Character.GetInstance();
            if (character != null) break;
            yield return null;
        }

        character.AddCharacterFollower(this);

    }

    public void OnMove(Character character)
    {
        transform.position = character.transform.position;
    }
}
