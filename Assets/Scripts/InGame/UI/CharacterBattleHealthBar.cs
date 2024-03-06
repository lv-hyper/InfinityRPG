using UnityEngine;
using UnityEngine.UI;
using InGame.Data;
using InGame.Data.BattleInstance;
using TMPro;

namespace InGame.UI
{
    public class CharacterBattleHealthBar : MonoBehaviour, IDisplayComponent
    {
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] Image image;
        
        private void Awake() {
            //image = GetComponent<Image>();
        }
        public void Refresh(Subject subject)
        {
            //if (image == null) image = GetComponent<Image>();
            if(subject.GetType() == typeof(Battle))
            {
                Battle battle = (Battle)subject;

                var character = battle.GetCharacterInstance();
                System.Numerics.BigInteger currentHP = character.GetHealth();
                System.Numerics.BigInteger maxHP = character.GetMaxHealth();

                if(text != null)
                {
                    text.text = string.Format("{0:N0} / {1:N0}",currentHP,maxHP);
                }

                image.fillAmount = (float)currentHP / (float)maxHP;
            }
        }
    }
}