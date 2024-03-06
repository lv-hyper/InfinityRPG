using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace InGame.UI.Menu
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] float remainingTime;

        void Awake()
        {
            StartCoroutine(FadeOut());
            transform.Rotate(new Vector3(0, 0, Random.Range(-15, 15)));
        }

        IEnumerator FadeOut()
        {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            Color color = text.color;

            for(float t=0; t<remainingTime; t+=Time.deltaTime)
            {
                // Move Damage Text 
                transform.Translate(0, speed*Time.deltaTime, 0);

                // Map 0~t into 1~0
                color.a = 1-(t/remainingTime);

                text.color = color;

                yield return null;
            }

            Destroy(gameObject);
        }

        public void SetText(string text)
        {
            GetComponent<TextMeshProUGUI>().text = text;
        }
    }
}
