using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingLayer : MonoBehaviour
{
    static LoadingLayer instance;

    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] Image loadingBarFG;

    public static LoadingLayer GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public void SetValue(float value){
        loadingBarFG.fillAmount = value;
        loadingText.text = string.Format("Loading... ({0:0.00}%)", value*100);
    }
}
