using InGame.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace InGame.UI
{
    public class CharacterClassController : Common.UI.MenuPage
    {
        [SerializeField] EnumEntityClass currentClass;

        [SerializeField] GameObject skillBar;
        [SerializeField] Transform skillBarTransform;

        [SerializeField] TextMeshProUGUI statusText, pointText, skillDescriptionText;

        List<SkillBar> skillBars;

        // Start is called before the first frame update

        private void Awake()
        {
            skillBars = new List<SkillBar>();
            SelectClass(currentClass);
        }
        public void SelectClass(EnumEntityClass chClass)
        {
            ClearTransform();

            currentClass = chClass;

            var skills = Data.Skill.SkillCollection.GetInstance().allSkillCollection;

            List<Data.Skill.SkillCollection.SkillStatus> skillStatusList
                = new List<Data.Skill.SkillCollection.SkillStatus>();

            int index = 0;

            List<Data.Skill.SkillCollection.SkillStatus> rawSkillStatusList = 
                new List<Data.Skill.SkillCollection.SkillStatus>(skills.Values);

            rawSkillStatusList.Sort(
                (a, b) => a.GetSkill().GetRequiredLevel().CompareTo(b.GetSkill().GetRequiredLevel())
            );


            foreach(var skillStatus in rawSkillStatusList)
            {
                Data.Skill.CharacterSkill skill = skillStatus.GetSkill();
                if (skill.GetTargetCharacterClassType() == chClass && skill.IsClassSkill())
                {
                    skillStatusList.Add(skillStatus);
                    GameObject skillBarInstance = 
                        Instantiate(skillBar, skillBarTransform, false);

                    skillBars.Add(skillBarInstance.GetComponent<SkillBar>());

                    Vector2 pos = skillBarInstance.GetComponent<RectTransform>().anchoredPosition;
                    pos.y = -20 - 150 * index;
                    skillBarInstance.GetComponent<RectTransform>().anchoredPosition = pos;
                    skillBarInstance.GetComponent<SkillBar>().Init(skillStatus, this);
                    ++index;
                }
            }

            int height = index * 150 + 10;

            Vector2 v = skillBarTransform.GetComponent<RectTransform>().sizeDelta;
            v.y = height;
            skillBarTransform.GetComponent<RectTransform>().sizeDelta = v;

            statusText.text = string.Format("{0} Lv.{1} ({2:N0}/{3:N0})",
                chClass.ToString(),
                Data.Character.CharacterClassData.GetCharacterClassLevel(chClass),
                Data.Character.CharacterClassData.GetAccumulatedCharacterLevel(chClass),
                Data.Character.CharacterClassData.LevelUpRequirement(
                    Data.Character.CharacterClassData.GetCharacterClassLevel(chClass)
                )
            );

            pointText.text = string.Format("{0} Character Point",
                Data.Character.CharacterClassData.GetCharacterClassPoint(chClass)
            );

            skillDescriptionText.text = "Skill information will displayed here.";
        }

        public void SelectClass(string characterClass)
        {
            switch(characterClass)
            {
                case "warrior":
                    SelectClass(EnumEntityClass.Warrior);
                    break;
                case "mage":
                    SelectClass(EnumEntityClass.Mage);
                    break;
                case "archer":
                    SelectClass(EnumEntityClass.Archer);
                    break;
                default:
                    break;
            }
        }

        public EnumEntityClass GetCurrentClass() { return currentClass; }

        void ClearTransform()
        {
            skillBars.Clear();
            foreach(Transform t in skillBarTransform)
            {
                Destroy(t.gameObject);
            }
        }

        public void UpdateData()
        {
            statusText.text = string.Format("{0} Lv.{1} ({2:N0}/{3:N0})",
                currentClass.ToString(),
                Data.Character.CharacterClassData.GetCharacterClassLevel(currentClass),
                Data.Character.CharacterClassData.GetAccumulatedCharacterLevel(currentClass),
                Data.Character.CharacterClassData.LevelUpRequirement(
                    Data.Character.CharacterClassData.GetCharacterClassLevel(currentClass)
                )
            );

            pointText.text = string.Format("{0} Character Point",
                Data.Character.CharacterClassData.GetCharacterClassPoint(currentClass)
            );

            foreach (SkillBar skillBar in skillBars)
            {
                skillBar.Refresh();
            }
        }

        public EnumEntityClass GetCharacterClass() { return currentClass; }

        public void OpenDialog() { gameObject.SetActive(true); SelectClass(currentClass); }

        public void SelectSkill(Data.Skill.SkillCollection.SkillStatus skillStatus)
        {
            string longDesc = ""; // skill.GetLongDescription();

            //long stack = skillStatus.GetCount() + 1;
            long stack = skillStatus.GetCount();
            if (stack > skillStatus.getMaxCount()) stack = skillStatus.getMaxCount();

            Data.Skill.CharacterSkill skill = skillStatus.GetSkill();

            longDesc = skill.GetLongDescription(stack);

            skillDescriptionText.text = longDesc;
        }

        public void ResetSkill()
        {
            Data.Character.CharacterClassData.ResetSpecificSkill(currentClass);
            Data.Skill.SkillCollection.GetInstance().ClearSpecificClassSkill(currentClass);
            UpdateData();
            Data.SaveData.SkillSaveDataManager.SaveSkillData();
            Entity.Character.GetInstance().ApplySkillAbilities();
            skillDescriptionText.text = "Skill information will displayed here.";
        }
    }
}
