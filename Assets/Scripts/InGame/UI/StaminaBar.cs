using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InGame.Data;
using TMPro;

namespace InGame.UI
{

    public class StaminaBar : MonoBehaviour, IDisplayComponent
    {
        [SerializeField] TextMeshProUGUI staminaText;
        [SerializeField] UnityEngine.UI.Image staminaImage;
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

                float currentStamina = character.GetStamina();
                float maxStamina = character.GetMaxStamina();

                staminaText.text =
                string.Format(
                    "Stamina ({0:0.0}/{1:0.0})",
                    currentStamina, maxStamina 
                );

                if(maxStamina == 0) {currentStamina = 0; maxStamina = 0.1f;}

                staminaImage.fillAmount = currentStamina / maxStamina;
            }
        }
    }
}