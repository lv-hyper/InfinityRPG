using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace InGame.UI
{
    public class BattleSkillSlot : MonoBehaviour
    {
        [SerializeField] Image skillIcon;
        [SerializeField] Sprite noneSkillIcon;
        [SerializeField] Outline skillIconOutline;

        [SerializeField] Image turnIndicator;
        [SerializeField] TextMeshProUGUI turnIndicatorText;

        [SerializeField] Outline skillToggleOutline;
        [SerializeField] TextMeshProUGUI skillToggleText;

        [SerializeField] Color defaultColor, notAvailableColor, disabledColor, highlightColor;

        [SerializeField] bool isDefaultSkillSlot;
        [SerializeField] InGame.Data.Skill.ActiveSkill skill;

        int cooldownCount;

        bool isOn;
        bool highlighted;

        private void Awake()
        {
            isOn = true;
            highlighted = false;
            cooldownCount = 0;
        }

        public void SetSkill(Data.Skill.ActiveSkill skill)
        {
            this.skill = skill;

            isOn = true;
            highlighted = false;
            cooldownCount = 0;

            if (skill != null)
            {
                skillIcon.sprite = skill.GetSprite();

                var skillStatus = Data.Skill.SkillCollection.GetInstance().allSkillCollection[skill.GetSkillID()];
                var skillStack = skillStatus.GetCount();

                if (skillStack <= 0)
                {
                    turnIndicator.gameObject.SetActive(false);

                    skillIconOutline.effectColor = notAvailableColor;
                    skillToggleOutline.effectColor = notAvailableColor;
                    skillToggleText.text = "Unavailable";
                }
                else
                {
                    turnIndicator.gameObject.SetActive(true);

                    turnIndicator.fillAmount = (float)cooldownCount / skill.GetSkillCoolDown(skillStack);
                    turnIndicatorText.text = skill.GetManaCost(skillStack).ToString();

                    if (highlighted)
                    {
                        skillIconOutline.effectColor = highlightColor;
                    }
                    else
                    {
                        skillIconOutline.effectColor = defaultColor;
                    }

                    if (isDefaultSkillSlot)
                    {
                        skillToggleOutline.effectColor = notAvailableColor;
                        skillToggleText.text = "Basic Skill";
                    }
                    else
                    {
                        skillToggleOutline.effectColor = defaultColor;
                        if (isOn)
                            skillToggleText.text = "Skill On";
                        else
                            skillToggleText.text = "Skill Off";
                    }
                }
            }
            else
            {
                skillIcon.sprite = noneSkillIcon;
                skillIconOutline.effectColor = notAvailableColor;

                turnIndicator.gameObject.SetActive(false);

                skillToggleOutline.effectColor = notAvailableColor;
                skillToggleText.text = "No Skill";
            }
        }

        public Data.Skill.ActiveSkill GetSkill() {return skill;}

        public void ToggleOnOff()
        {
            isOn = !isOn;
            if (isOn)
                skillToggleText.text = "Skill On";
            else
                skillToggleText.text = "Skill Off";

            BattleSkillController.GetInstance().UpdateNextSkill();
        }

        public bool GetOnOff() { return isOn; }

        public void Highlight()
        {
            highlighted = true;
            skillIconOutline.effectColor = highlightColor;
        }

        public void UnHighlight()
        {
            highlighted = false;
            skillIconOutline.effectColor = defaultColor;
        }

        public int GetCool() { return cooldownCount; }
        public void StartCool()
        {
            var skillStatus = Data.Skill.SkillCollection.GetInstance().allSkillCollection[skill.GetSkillID()];
            
            cooldownCount = (int)skill.GetSkillCoolDown(skillStatus.GetCount());
        }
        public void Turn()
        {
            if (cooldownCount > 0)
                --cooldownCount;

            if (skill == null) return;

            var skillStatus = Data.Skill.SkillCollection.GetInstance().allSkillCollection[skill.GetSkillID()];
            var skillStack = skillStatus.GetCount();

            turnIndicator.fillAmount = (float)cooldownCount / skill.GetSkillCoolDown(skillStack);
        }
    }
}
