using InGame.Data;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;

namespace InGame.UI
{
    public class SkillBar : MonoBehaviour
    {
        [SerializeField] EnumEntityClass currentClass;

        [SerializeField] UnityEngine.UI.Image skillImage;
        [SerializeField] TextMeshProUGUI skillTitle, skillDescription, levelRequirement;

        [SerializeField] UnityEngine.UI.Button upgradeButton, degradeButton;

        CharacterClassController characterClassController;

        string skillID;
        long requiredLv;
        long upgradeCost;
        
        public void UpgradeSkill()
        {
            upgradeCost = Data.Skill.SkillCollection.GetInstance().allSkillCollection[skillID].GetSkill().GetUpgradeCost();
            Data.Character.CharacterClassData.LoseCharacterClassPoint(characterClassController.GetCharacterClass(), upgradeCost);

            Data.Skill.SkillCollection.GetInstance().UpgradeSkill(skillID);
            upgradeCost = Data.Skill.SkillCollection.GetInstance().allSkillCollection[skillID].GetSkill().GetUpgradeCost();

            characterClassController.UpdateData();
            SetSkill();

            Data.SaveData.SkillSaveDataManager.SaveSkillData();
            Entity.Character.GetInstance().ApplySkillAbilities();
        }

        public void DegradeSkill()
        {
            upgradeCost = Data.Skill.SkillCollection.GetInstance().allSkillCollection[skillID].GetSkill().GetUpgradeCost(); // Actually points you get from degrade
            Data.Character.CharacterClassData.AddCharacterClassPoint(characterClassController.GetCharacterClass(), upgradeCost);

            Data.Skill.SkillCollection.GetInstance().DegradeSkill(skillID);
            upgradeCost = Data.Skill.SkillCollection.GetInstance().allSkillCollection[skillID].GetSkill().GetUpgradeCost();

            characterClassController.UpdateData();
            SetSkill();

            Data.SaveData.SkillSaveDataManager.SaveSkillData();
            Entity.Character.GetInstance().ApplySkillAbilities();            
        }


        public void Init(Data.Skill.SkillCollection.SkillStatus skillStatus, CharacterClassController characterClassController)
        {
            currentClass = characterClassController.GetCurrentClass();
            Data.Skill.CharacterSkill skill = skillStatus.GetSkill();
            skillID = skill.GetSkillID();
            skillImage.sprite = skill.GetSprite();
            skillTitle.text = string.Format(
                "{0} Lv.{1}/{2} ({3})",
                skill.name, skillStatus.GetCount(), skillStatus.getMaxCount(), skill.GetUpgradeCost()
            );
            skillDescription.text = skill.GetSkillDescription();
            upgradeCost = skill.GetUpgradeCost();
            requiredLv = skill.GetRequiredLevel();

            this.characterClassController = characterClassController;

            Refresh();
        }

        public void Refresh()
        {
            Data.Skill.SkillCollection.SkillStatus skillStatus =
                Data.Skill.SkillCollection.GetInstance().allSkillCollection[skillID];

            if (
                upgradeCost >
                Data.Character.CharacterClassData.GetCharacterClassPoint(characterClassController.GetCharacterClass())
            )
            {
                upgradeButton.interactable = false;
            }
            else if (skillStatus.GetCount() >= skillStatus.getMaxCount() || skillStatus.GetSkill().GetUpgradeCost() == 0)
            {
                upgradeButton.interactable = false;
            }
            else
            {
                upgradeButton.interactable = true;
            }

            if (skillStatus.GetCount() == 0 || skillStatus.GetSkill().GetUpgradeCost() == 0)
            {
                degradeButton.interactable = false;
            }
            else
            {
                degradeButton.interactable = true;
            }


            skillTitle.text = string.Format(
                "{0} Lv.{1}/{2} ({3})",
                skillStatus.name, 
                skillStatus.GetCount(), 
                skillStatus.getMaxCount(), 
                skillStatus.GetSkill().GetUpgradeCost()
            );

            BigInteger charLv = Data.Character.CharacterClassData.GetCharacterClassLevel(
                characterClassController.GetCharacterClass()
            );
            levelRequirement.text = string.Format("Lv {0}", requiredLv);
            if (requiredLv > charLv)
            {
                upgradeButton.interactable = false;
                levelRequirement.color = new Color(1, 0, 0);
            }
            else
            {
                levelRequirement.color = new Color(1, 1, 1);
            }
        }
        public void SetSkill()
        {
            characterClassController.SelectSkill(
                Data.Skill.SkillCollection.GetInstance().allSkillCollection[skillID]
            );
        }
    }
}
