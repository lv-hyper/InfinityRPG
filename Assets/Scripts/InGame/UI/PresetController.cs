using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InGame.Data;
using TMPro;

namespace InGame.UI
{
    public class PresetController : MonoBehaviour, IDisplayComponent
    {
        [SerializeField] List<GameObject> itemTabs;
        [SerializeField] int currentItemTab = 0;

        [SerializeField] bool isExpanded = true;

        void Awake()
        {
            StartCoroutine(RegisterToCharacter());
        }

        IEnumerator RegisterToCharacter()
        {
            Entity.Character character = null;

            while (true)
            {
                character = Entity.Character.GetInstance();
                if (character != null) break;
                yield return null;
            }

            character.AddDisplayComponent(this);
        }

        public void Refresh(Subject subject)
        {
            if (subject.GetType() == typeof(Entity.Character))
            {
                Entity.Character character = (Entity.Character)subject;
            }
        }

        public void Toggle()
        {
            isExpanded = !isExpanded;
            if (isExpanded)
            {
                GetComponent<RectTransform>().anchoredPosition = new Vector2(20, -20);
            }
            else
            {
                GetComponent<RectTransform>().anchoredPosition = new Vector2(20, 262);
            }
        }

        public void NextItemTab()
        {
            currentItemTab++;
            if(currentItemTab >= itemTabs.Count)
            {
                currentItemTab = 0;
            }

            foreach(var itemTab in itemTabs)
            {
                itemTab.SetActive(false);
            }

            itemTabs[currentItemTab].SetActive(true);
        }
    }
}
