using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionLevelDisplay : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI progressionLevelText;

    private void OnEnable()
    {
        progressionLevelText.text = string.Format(
            "Boss Lv. {0}", PlayerPrefs.GetInt("ProgressionLevel", 0)
        );
    }
}
