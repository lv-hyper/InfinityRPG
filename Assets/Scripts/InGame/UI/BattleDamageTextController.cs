using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.UI
{
    public class BattleDamageTextController : MonoBehaviour
    {
        [SerializeField] GameObject characterDamageTextPrefab, mobDamageTextPrefab;
        [SerializeField] Transform characterDamageTransform, mobDamageTransform;

        List<GameObject> damageObjectList;

        static BattleDamageTextController instance;

        public static BattleDamageTextController GetInstance()
        {
            return instance;
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                damageObjectList = new List<GameObject>();
            }
        }

        public void GenerateCharacterDamageText(string text)
        {
            GameObject damageText = Instantiate(characterDamageTextPrefab, characterDamageTransform);
            damageText.GetComponent<Menu.DamageText>().SetText(text);

            damageObjectList.Add(damageText);
        }
        public void GenerateMobDamageText(string text)
        {
            GameObject damageText = Instantiate(mobDamageTextPrefab, mobDamageTransform);
            damageText.GetComponent<Menu.DamageText>().SetText(text);

            damageObjectList.Add(damageText);
        }

        public void ClearDamageTexts()
        {
            foreach(GameObject damageText in damageObjectList)
            {
                Destroy(damageText);
            }

            damageObjectList.Clear();
        }
    }
}
