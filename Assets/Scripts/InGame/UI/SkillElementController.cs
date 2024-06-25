using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using InGame.Data.Item;
using InGame.Data;

namespace InGame.UI
{
    public class SkillElementController : MonoBehaviour{

        string skillID;
        SkillListController skillListController;
        [SerializeField] UnityEngine.UI.Image image;
        [SerializeField] TextMeshProUGUI skillUpgradeCount, skillTitle, skillShortDesc;

        [SerializeField] GameObject skillDesc;
        [SerializeField] TextMeshProUGUI skillDescContent;
        [SerializeField] UnityEngine.UI.Button selectButton;
        [SerializeField] TextMeshProUGUI selectButtonText;

        [SerializeField] Sprite noSkillSprite;

        Entity.Character character;

        private void Awake() {
            character = Entity.Character.GetInstance();
        }

        public void SetSkill(string skillID, SkillListController skillListController)
        {
            this.skillID = skillID;
            this.skillListController = skillListController;
            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            var skill = skillListController.GetSkillCollection()[skillID];
            var _skill = (Data.Skill.ActiveSkill) skill.GetSkill();

            var currentEquipmentSet = character.GetCurrentEquipmentSet();

            image.sprite = _skill.GetSprite();

            skillUpgradeCount.text = string.Format("{0}/{1}",skill.GetCount(), skill.getMaxCount());
            skillTitle.text = skill.name;
            skillShortDesc.text = _skill.GetSkillDescription();

            string longDesc = _skill.GetLongDescription(skill.GetCount());

            longDesc = longDesc.Replace("+-", "-");

            skillDescContent.text = longDesc;

            var currentCharacterSkill = currentEquipmentSet.GetSkill(skillListController.GetCurrentChoice());
            string currentCharacterSkillID = "";

            if (currentCharacterSkill != null) currentCharacterSkillID = currentCharacterSkill.GetSkillID();

            var elementalType = skill.GetSkill().GetElementalType();


            if (skill.GetCount() == 0 && (elementalType == EnumElemental.None || 
                    InGame.Data.ElementalSoulData.GetInstance().GetGainedSoulCount(elementalType) <= 0))
            {
                selectButton.interactable = false;
                selectButtonText.text = "No Skill";

                image.sprite = noSkillSprite;
                skillTitle.text = "????";
                skillShortDesc.text = "????";
                skillDescContent.text = "This skill is not obtained. You probably can get this skill by some action.";

                skillUpgradeCount.text = "0/?";

            }
            else
            {
                if (skill.GetCount() == 0)
                {
                    image.color = new Color(0.1f, 0.1f, 0.1f);
                }
                else
                {
                    image.color = Color.white;
                }
                
                if (currentEquipmentSet.isSkillEquiped(_skill))
                {
                    var equippedSkill = currentEquipmentSet.GetSkill(skillListController.GetCurrentChoice());

                    string equippedSkillID = "";
                    string currentSkillID = ""; 

                    if(equippedSkill != null)
                        equippedSkillID = equippedSkill.GetSkillID();

                    if(_skill.GetSkillID() != null)
                        currentSkillID = _skill.GetSkillID();

                    if (equippedSkillID == currentSkillID && equippedSkillID != "")
                    {
                        selectButton.interactable = true;

                        selectButtonText.text = "Unselect";
                    }
                    else
                    {
                        selectButton.interactable = false;

                        selectButtonText.text = "Selected";
                    }
                }
                else
                {
                    selectButton.interactable = true;

                    selectButtonText.text = "Select";
                }
            }
            
        }

        public void ToggleDesc()
        {
            if(IsDescActive())
                skillDesc.SetActive(false);
            else
                skillDesc.SetActive(true);
            
            skillListController.RefreshDisplay();
        }

        public bool IsDescActive(){return skillDesc.activeSelf;}

        public void SelectSkill()
        {
            if (selectButtonText.text == "Unselect")
            {
                skillListController.SelectSkill(null);
            }
            else
            {
                skillListController.SelectSkill(GetSkill());
            }
        }

        public Data.Skill.ActiveSkill GetSkill(){
            return (Data.Skill.ActiveSkill) skillListController.GetSkillCollection()[skillID].GetSkill();
        }
    }
}