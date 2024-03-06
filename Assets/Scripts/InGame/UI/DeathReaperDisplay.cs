using InGame.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace InGame.UI
{
    public class DeathReaperDisplay : MonoBehaviour, IDisplayComponent
    {
        [SerializeField] TMPro.TextMeshProUGUI deathReaperText;
        [SerializeField] GameObject deathReaperIndicator;

        private void Awake()
        {
            deathReaperIndicator.SetActive(false);
            deathReaperText.text = "";
        }

        public void Refresh(Subject subject)
        {
            if (subject.GetType() == typeof(Battle))
            {
                Battle battle = (Battle)subject;

                var deathReaperCount = battle.GetDeathReaperDebuffCount();

                if (deathReaperCount > 0)
                {
                    deathReaperIndicator.SetActive(true);
                    deathReaperText.text = string.Format("Death {0}", deathReaperCount);
                }
                else
                {
                    deathReaperIndicator.SetActive(false);
                    deathReaperText.text = "";
                }
            }
        }
    }
}
