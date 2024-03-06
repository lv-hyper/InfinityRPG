using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class scrollViewSize : MonoBehaviour
{
    [SerializeField] RectTransform content;
    [SerializeField] TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(text.rectTransform.sizeDelta.y);
        content.sizeDelta.Set(1020, text.rectTransform.sizeDelta.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
