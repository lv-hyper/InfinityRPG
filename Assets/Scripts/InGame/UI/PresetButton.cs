using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InGame.Data;
using TMPro;

namespace InGame.UI
{
    public class PresetButton : MonoBehaviour, IDisplayComponent
    {
        [SerializeField] string minimizedSetName, actualSetName; 
        void Awake()
        {
            StartCoroutine(RegisterToCharacter());
            transform.Find("Text").GetComponent<TextMeshProUGUI>().text = minimizedSetName;
        }

        void OnEnable()
        {
            var character = Entity.Character.GetInstance();
            if (character != null)
                Refresh(character);
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

        public void OnClick()
        {
            var character = Entity.Character.GetInstance();
            if (character != null)
            {
                character.SetEquipmentSet(actualSetName);
            }
        }
        public void Refresh(Subject subject)
        {
            if (subject.GetType() == typeof(Entity.Character))
            {
                Entity.Character character = (Entity.Character)subject;

                string setName = character.GetCurrentEquipmentSet().GetSetName();

                if(setName == actualSetName)
                {
                    GetComponent<UnityEngine.UI.Image>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
                }
                else
                {
                    GetComponent<UnityEngine.UI.Image>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                }
            }
        }
    }
}
