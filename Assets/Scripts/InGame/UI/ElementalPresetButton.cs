using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InGame.Data;
using TMPro;

namespace InGame.UI
{
    public class ElementalPresetButton : MonoBehaviour, IDisplayComponent
    {
        [SerializeField] EnumElemental elemental;
        void Awake()
        {
            var elementalSoulData = Data.ElementalSoulData.GetInstance();
            elementalSoulData.AddDisplayComponent(this);

            Refresh(elementalSoulData);
        }

        private void OnDestroy()
        {
            Data.ElementalSoulData.GetInstance().RemoveDisplayComponent(this);
        }

        public void OnClick()
        {
            Data.Skill.SkillCollection.GetInstance().ClearElementalSkill(elemental);
            Data.ElementalSoulData.GetInstance().NextPreset(elemental);

            var preset = Data.ElementalSoulData.GetInstance().GetCurrentPreset(elemental);

            foreach (var perk in preset.GetPerks())
            {
                var skillElement = Data.ElementalTreeCollection.GetInstance().GetElementalTree(elemental).GetTier(perk.tier).elements[perk.order];

                var currentSkillUpgradeCount =
                    Data.Skill.SkillCollection.GetInstance().allSkillCollection[skillElement.GetSkill().GetSkillID()].GetCount();
                var targetCount = skillElement.GetUpgradeCount();

                if (targetCount > currentSkillUpgradeCount)
                {
                    Data.Skill.SkillCollection.GetInstance().SetSkillUpgradeCount(
                        skillElement.GetSkill().GetSkillID(),
                        targetCount
                    );
                }
            }

            Data.SaveData.SkillSaveDataManager.SaveSkillData();
            Entity.Character.GetInstance().ApplySkillAbilities();

        }

        public void Refresh(Subject subject)
        {
            if (subject.GetType() == typeof(ElementalSoulData))
            {
                var data = (ElementalSoulData)subject;

                transform.Find("Text").GetComponent<TextMeshProUGUI>().text = 
                    data.GetCurrentPreset(elemental).GetMinifiedName();
            }
        }
    }
}
