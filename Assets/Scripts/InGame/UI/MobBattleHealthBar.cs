using UnityEngine;
using UnityEngine.UI;
using InGame.Data;
using InGame.Data.BattleInstance;
using TMPro;
using System.Collections.Generic;
using InGame.Data.Mob;
using InGame.Entity;

namespace InGame.UI
{
    public class MobBattleHealthBar : MonoBehaviour, IDisplayComponent
    {
        [SerializeField] TextMeshProUGUI text, tilteText;
        [SerializeField] Image fg, bg;

        [SerializeField] List<Color> mobHealthColor;
        
        public void Refresh(Subject subject)
        {
            if(subject.GetType() == typeof(Battle))
            {
                Battle battle = (Battle)subject;

                var mob = battle.GetMobInstance();
                System.Numerics.BigInteger currentHP = mob.GetHealth();
                System.Numerics.BigInteger maxHP = mob.GetMaxHealth();

                int healthBarCount = mob.GetHealthBarCount();
                
                if (healthBarCount > mobHealthColor.Count - 1)
                    healthBarCount = mobHealthColor.Count - 1;

                int currentHealthBar = (int)System.Math.Floor((double)currentHP / (double)maxHP * healthBarCount);

                if (currentHealthBar >= healthBarCount) currentHealthBar = healthBarCount - 1;
                if (currentHealthBar < 0) currentHealthBar = 0;

                bg.color = mobHealthColor[currentHealthBar];
                fg.color = mobHealthColor[currentHealthBar + 1];

                if(text != null)
                {
                    text.text = string.Format("{0:N0} / {1:N0}", currentHP, maxHP);
                }

                if(tilteText != null)
                {
                    tilteText.text = string.Format("Lv. {0:N0}\n{1}", mob.GetLV(), mob.GetName());


                    if (mob.GetTagList().Count > 0)
                    {
                        tilteText.text += " [";
                        foreach (var tag in mob.GetTagList())
                        {
                            tilteText.text += tag;
                        }
                        tilteText.text += "]";
                    }

                    if (PlayerPrefs.GetString("passcode", "").ToLower() == "another")
                        tilteText.text = "A" + tilteText.text;
                }

                fg.fillAmount = (float)((double)currentHP / (double)maxHP * healthBarCount) - currentHealthBar;

            }
        }
    }
}