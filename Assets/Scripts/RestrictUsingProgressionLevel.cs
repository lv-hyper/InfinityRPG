using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictUsingProgressionLevel : MonoBehaviour
{
    [SerializeField] int targetLv;
    [SerializeField] UnityEngine.UI.Button button;
    private void Awake()
    {
        int lv = PlayerPrefs.GetInt("ProgressionLevel", 0);

        if (lv < targetLv)
            button.interactable = false;
        else
            button.interactable = true;

    }
}
