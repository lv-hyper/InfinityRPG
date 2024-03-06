using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionDisplay : MonoBehaviour
{
    private void Awake() {
        GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0}",Application.version);
    }
}
