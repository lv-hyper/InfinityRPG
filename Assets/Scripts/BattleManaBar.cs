using UnityEngine;
using UnityEngine.UI;
using InGame.Data;
using InGame.Data.BattleInstance;
using TMPro;

namespace InGame.UI
{
    public class BattleManaBar : MonoBehaviour, IDisplayComponent
    {
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
        }
        public void Refresh(Subject subject)
        {
            if (image == null) image = GetComponent<Image>();
            if (subject.GetType() == typeof(Battle))
            {
                Battle battle = (Battle)subject;
                var character = battle.GetCharacterInstance();
                System.Numerics.BigInteger currentMana = character.GetMana();
                System.Numerics.BigInteger maxMana = character.GetMaxMana();

                if (text != null)
                {
                    text.text = string.Format("{0:N0} / {1:N0}", currentMana, maxMana);
                }

                image.fillAmount = (float)currentMana / (float)maxMana;
            }
        }
        public void RegisterToBattle(Battle battle)
        {
            battle.AddDisplayComponent(this);
        }
    }
}