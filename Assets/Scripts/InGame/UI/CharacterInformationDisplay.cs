using InGame.Data;
using System.Collections;
using System.Collections.Generic;
using Common.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace InGame.UI
{
    public class CharacterInformationDisplay : MenuPage, IDisplayComponent
    {
        [SerializeField] TextMeshProUGUI content;
        [SerializeField] TMP_Dropdown dropdown;
        Entity.Character character = null;

        void Awake()
        {
            StartCoroutine(RegisterToCharacter());
        }

        IEnumerator RegisterToCharacter()
        {
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
                RefreshDisplay();
            }
        }

        public void RefreshDisplay()
        {
            long currentGameZoneLv = 5;

            if (character.GetCurrentGameZone() != null) 
                currentGameZoneLv = character.GetCurrentGameZone().GetBaseLv();


            var atkAbility = Data.Character.CharacterStat.GetInstance().GetAbilitySet().GetAbility("Attack");

            var defAbility = Data.Character.CharacterStat.GetInstance().GetAbilitySet().GetAbility("Defence");
            var meleeDefAbility = Data.Character.CharacterStat.GetInstance().GetAbilitySet().GetAbility("Melee Defence");
            var magicalDefAbility = Data.Character.CharacterStat.GetInstance().GetAbilitySet().GetAbility("Magical Defence");
            var rangedDefAbility = Data.Character.CharacterStat.GetInstance().GetAbilitySet().GetAbility("Ranged Defence");


            var strAbility = Data.Character.CharacterStat.GetInstance().GetAbilitySet().GetAbility("Strength");
            var intAbility = Data.Character.CharacterStat.GetInstance().GetAbilitySet().GetAbility("Intelligence");
            var dexAbility = Data.Character.CharacterStat.GetInstance().GetAbilitySet().GetAbility("Dexiterity");
            var vitAbility = Data.Character.CharacterStat.GetInstance().GetAbilitySet().GetAbility("Vitality");
            var endAbility = Data.Character.CharacterStat.GetInstance().GetAbilitySet().GetAbility("Endurance");

            switch (dropdown.options[dropdown.value].text)
            {
                case "Attack":
                    content.text = GetDescription(
                        "Attack",
                        (long)((atkAbility != null) ? atkAbility.GetRawAmount() : 0),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Additional Attack"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Attack Percent Point"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Attack")
                    );
                    break;
                case "Defence":
                    content.text = "";
                    content.text +=  GetDescription(
                        "Defence",
                        (long)((defAbility != null) ? defAbility.GetRawAmount() : 0),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Additional Defence"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("All Defence Percent Point"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Defence")
                    );
                    content.text += "\n" + GetDescription(
                        "Melee Defence",
                        (long)((meleeDefAbility != null) ? meleeDefAbility.GetRawAmount() : 0),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Additional Melee Defence"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Melee Defence Percent Point"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Melee Defence")
                    ); 
                    content.text += "\n" + GetDescription(
                         "Magical Defence",
                        (long)((magicalDefAbility != null) ? magicalDefAbility.GetRawAmount() : 0),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Additional Magical Defence"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Magical Defence Percent Point"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Magical Defence")
                     );
                    content.text += "\n" + GetDescription(
                         "Ranged Defence",
                        (long)((rangedDefAbility != null) ? rangedDefAbility.GetRawAmount() : 0),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Additional Ranged Defence"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Ranged Defence Percent Point"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Ranged Defence")
                     );
                    break;
                case "STR":
                    content.text = GetDescription(
                        "STR",
                        (long)((strAbility != null) ? strAbility.GetRawAmount() : 0),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Additional Strength"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Strength Percent Point"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Strength")
                    );
                    break;
                case "INT":
                    content.text = GetDescription(
                        "INT",
                        (long)((intAbility != null) ? intAbility.GetRawAmount() : 0),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Additional Intelligence"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Intelligence Percent Point"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Intelligence")
                    );
                    break;
                case "DEX":
                    content.text = GetDescription(
                        "DEX",
                        (long)((dexAbility != null) ? dexAbility.GetRawAmount() : 0),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Additional Dexiterity"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Dexiterity Percent Point"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Dexiterity")
                    );
                    break;
                case "VIT":
                    content.text = GetDescription(
                        "VIT",
                        (long)((vitAbility != null) ? vitAbility.GetRawAmount() : 0),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Additional Vitality"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Vitality Percent Point"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Vitality")
                    );
                    break;
                case "END":
                    content.text = GetDescription(
                        "END",
                        (long)((endAbility != null) ? endAbility.GetRawAmount() : 0),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Additional Endurance"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Endurance Percent Point"),
                        (long)Data.Character.CharacterStat.GetAbilityAmount("Endurance")
                    );
                    break;
                case "ETC":
                    content.text = string.Format("Additional Droprate : {0:N0}%\n", (long)Data.Character.CharacterStat.GetAbilityAmount("Base Droprate"));
                    content.text += string.Format("Additional Gold Rate : {0:N0}%\n", (long)Data.Character.CharacterStat.GetAbilityAmount("Base Gold Rate"));
                    content.text += string.Format("Combo Rate : {0:N0}%\n", (long)Data.Character.CharacterStat.GetAbilityAmount("Combo Rate"));
                    content.text += string.Format("Avoid Rate : {0:N0}%\n", (long)Data.Character.CharacterStat.GetAbilityAmount("Avoid Rate"));
                    content.text += string.Format("Critical Rate : {0:N0}%\n", (long)Data.Character.CharacterStat.GetAbilityAmount("Critical Rate"));
                    content.text += string.Format(
                        "Critical Damage +{0:0.00}% ~ {1:0.00}%\n", 
                        ((long)Data.Character.CharacterStat.GetAbilityAmount("Critical Damage") / 100.0f + 0.9f) * 100.0f,
                        ((long)Data.Character.CharacterStat.GetAbilityAmount("Critical Damage") / 100.0f + 1.1f) * 100.0f
                    );
                    content.text += string.Format("Additional EXP : +{0:N0}%\n",
                        (long)Data.Character.CharacterStat.GetAbilityAmount("EXP"));
                    break;
                default:
                    break;
            }
        }

        private void OnDestroy()
        {
            if (Entity.Character.GetInstance() != null)
                Entity.Character.GetInstance().RemoveDisplayComponent(this);
        }

        public string GetDescription(string statName, long baseStat, long additionalStat, float statPercent, long totalStat)
        {
            string description = string.Format("Base {0} : {1:N0}\n", statName, baseStat);
            description += string.Format("Additional {0} : {1:N0}\n", statName, additionalStat);
            description += string.Format("Additional {0} Percent : {1:N0}%\n", statName, statPercent);
            description += string.Format("Total {0} : {1:N0}\n", statName, totalStat);
            return description;
        }
    }
}
