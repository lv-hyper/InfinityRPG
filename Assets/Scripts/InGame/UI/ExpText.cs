using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InGame.Data;
using TMPro;

namespace InGame.UI
{
    public class ExpText : MonoBehaviour, IDisplayComponent
    {
        [SerializeField] UnityEngine.UI.Image expBar;
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

                GetComponent<TextMeshProUGUI>().text =
                string.Format(
                    "{0}/{1}",
                    Utility.MinimizeNumText.Minimize(character.GetEXP()),
                    Utility.MinimizeNumText.Minimize(character.GetMaxEXP())
                );


                expBar.fillAmount = (float)System.Numerics.BigInteger.Divide(character.GetEXP()*1000, character.GetMaxEXP()) / 1000.0f;
            }
        }

        private void OnDestroy() {
            if(Entity.Character.GetInstance() != null)
                Entity.Character.GetInstance().RemoveDisplayComponent(this);
        }
    }
}
