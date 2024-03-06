using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InGame.Data;
using TMPro;

namespace InGame.UI
{
    public class SwitchSetButtonController : MonoBehaviour, IDisplayComponent
    {
        [SerializeField] TextMeshProUGUI switchSetText;
        [SerializeField] bool minimizedText;
        
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

                string setName = character.GetCurrentEquipmentSet().GetSetName();

                if(minimizedText)
                {
                    switchSetText.text = setName;
                }
                else
                {
                    switchSetText.text = string.Format("Current : {0}", setName);
                }
            }
        }
    }
}
