using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InGame.UI
{
    public class SkillListController : MonoBehaviour
    {
        Dictionary<string, Data.Skill.SkillCollection.SkillStatus> skillCollection;
        List<GameObject> skillList;

        int currentChoice;

        [SerializeField] GameObject skillElementPrefab;
        [SerializeField] GameObject skillListContent;
        [SerializeField] TMPro.TextMeshProUGUI currentStatusText;

        private void Awake()
        {
            skillCollection = new Dictionary<string, Data.Skill.SkillCollection.SkillStatus>();
            skillList = new List<GameObject>();
        }
        public void ActivateSkillList(int choice)
        {
            skillCollection = Data.Skill.SkillCollection.GetInstance().GetCollection("active");

            currentChoice = choice;

            InitDisplay();
        }

        public void InitDisplay()
        {
            skillList.Clear();

            foreach(Transform transform in skillListContent.transform)
            {
                Destroy(transform.gameObject);
            }

            var collectionKeyList = skillCollection.Values.ToList();

            collectionKeyList.Sort((a, b) => {

                var skillA = a.GetSkill();
                var skillB = b.GetSkill();

                if (skillA.GetRequiredLevel() != skillB.GetRequiredLevel())
                {
                    return skillA.GetRequiredLevel().ToString().CompareTo(skillB.GetRequiredLevel().ToString());
                }
                else
                {
                    return skillA.name.CompareTo(skillB.name);
                }
            });

            foreach(var skill in collectionKeyList)
            {
                var _skill = skill.GetSkill();
                if (
                    _skill == null || 
                    (_skill.GetTargetCharacterClassType() != Entity.Character.GetInstance().GetCurrentClass() &&
                    _skill.GetTargetCharacterClassType() != Data.EnumEntityClass.None)
                )
                    continue;
                GameObject skillInstance = GameObject.Instantiate(skillElementPrefab, skillListContent.transform);
                skillInstance.GetComponent<SkillElementController>().SetSkill(skill.GetSkill().GetSkillID(), this);

                skillList.Add(skillInstance);
            }

            RefreshDisplay();
        }

        public void RefreshDisplay()
        {
            int y = -30;

            long ownSkillCount = 0;
            long totalSkillCount = 0;
            foreach(var skillElement in skillList)
            {
                Vector2 pos = skillElement.GetComponent<RectTransform>().anchoredPosition;
                pos.y = y;

                skillElement.GetComponent<RectTransform>().anchoredPosition = pos;

                SkillElementController skillElementController = skillElement.GetComponent<SkillElementController>();

                if(!skillElementController.IsDescActive())
                {
                    y-=200;
                }
                else
                {
                    y-=700;
                }

                if(skillCollection[skillElementController.GetSkill().GetSkillID()].GetCount() > 0)
                    ++ownSkillCount;

                ++totalSkillCount;

                skillElementController.UpdateDisplay();
            }

            float xSize = skillListContent.GetComponent<RectTransform>().sizeDelta.x;
            skillListContent.GetComponent<RectTransform>().sizeDelta = new Vector2(xSize, -y);

            

            currentStatusText.text = string.Format(
                "Skill : {0:N0}/{1:N0}", 
                ownSkillCount, totalSkillCount, Entity.Character.GetInstance().GetGold()
            );
        }

        public void OpenSkillList(
            Dictionary<string, Data.Skill.SkillCollection.SkillStatus> skillCollection)
        {
            this.skillCollection = skillCollection;

            gameObject.SetActive(true);

            InitDisplay();
        }

        public void SelectSkill(Data.Skill.ActiveSkill skill)
        {
            Entity.Character character = Entity.Character.GetInstance();
            character.GetCurrentEquipmentSet().SetSkill(currentChoice, skill);
            Entity.Character.GetInstance().SaveEquipmentData();
            RefreshDisplay();
        }

        public Dictionary<string, Data.Skill.SkillCollection.SkillStatus> GetSkillCollection(){return skillCollection;}

        public int GetCurrentChoice(){return currentChoice;}
    }

}
