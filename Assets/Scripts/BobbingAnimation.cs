using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BobbingAnimation : MonoBehaviour
{
    [SerializeField] float maxBobAmount, speed;

    Vector2 originalPos;

    Coroutine coroutine;

    private void Awake() {
        originalPos = GetComponent<RectTransform>().anchoredPosition;
    }

    private void OnEnable() {
        coroutine = StartCoroutine(Bobbing(maxBobAmount, speed));
    }

    private void OnDisable() {
        StopCoroutine(coroutine);
    }

    IEnumerator Bobbing(float maxBobAmount, float speed)
    {
        float i = 0;
        Vector2 pos = GetComponent<RectTransform>().anchoredPosition;
        while(true)
        {
            for(; i< maxBobAmount; i+=(Time.deltaTime*speed*maxBobAmount)) 
            {
                GetComponent<RectTransform>().anchoredPosition = pos + (Vector2.up * i);
                yield return null;
            }
            for(; i>-maxBobAmount; i-=(Time.deltaTime*speed*maxBobAmount))
            {
                GetComponent<RectTransform>().anchoredPosition = pos + (Vector2.up * i);
                yield return null;
            }
        }
    }
}
