using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictUsingMaxLevel : MonoBehaviour
{
    [SerializeField] long targetLv;
    [SerializeField] UnityEngine.UI.Button button;
    private void Awake()
    {
        long maxLevel = long.Parse(
            PlayerPrefs.GetString("MaxLevel", "0")
        );

        if (maxLevel < targetLv)
            button.interactable = false;
        else
            button.interactable = true;

    }
}
