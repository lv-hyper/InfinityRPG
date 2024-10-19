using UnityEngine;
using System;
using System.Collections.Generic;

namespace InGame.Data.Skill
{
    public class SkillCollection
    {
        [Serializable]
        public class SkillStatus
        {
            [SerializeField] CharacterSkill skill;
            [SerializeField] long skillUpgradeCount;

            long maxSkillUpgradeCount
            {
                get
                {
                    return skill.GetMaxUpgradeCount();
                }
            }

            public string name { get { return skill.name; } }

            public SkillStatus(CharacterSkill skill)
            {
                this.skill = skill;
                this.skillUpgradeCount = skill.GetDefaultUpgradeCount();
            }

            public CharacterSkill GetSkill() { return skill; }
            public long GetCount() { return skillUpgradeCount; }
            public long getMaxCount() { return maxSkillUpgradeCount; }

            public void upgradeSkill(long count = 1) { skillUpgradeCount += count; }
            public void ClearSkill() { skillUpgradeCount = skill.GetDefaultUpgradeCount(); }
            public void SetSkillUpgradeCount(long count) { skillUpgradeCount = count;}
        }

        private static SkillCollection instance;
        public Dictionary<string, SkillStatus> allSkillCollection;

        Dictionary<string, SkillStatus> activeSkillCollection;
        Dictionary<string, SkillStatus> passiveSkillCollection;
        Dictionary<string, SkillStatus> classSkillCollection;
        Dictionary<string, SkillStatus> warriorSkillCollection;
        Dictionary<string, SkillStatus> mageSkillCollection;
        Dictionary<string, SkillStatus> archerSkillCollection;
        Dictionary<string, SkillStatus> elementalSkillCollection;

        Dictionary<string, bool> isPassiveSkillActivated;

        public static SkillCollection GetInstance()
        {
            if (instance == null)
            {
                instance = new SkillCollection();
            }

            return instance;
        }

        SkillCollection()
        {
            allSkillCollection = new Dictionary<string, SkillStatus>();

            activeSkillCollection = new Dictionary<string, SkillStatus>();
            passiveSkillCollection = new Dictionary<string, SkillStatus>();
            
            classSkillCollection = new Dictionary<string, SkillStatus>();
            warriorSkillCollection = new Dictionary<string, SkillStatus>();
            mageSkillCollection = new Dictionary<string, SkillStatus>();
            archerSkillCollection = new Dictionary<string, SkillStatus>();

            elementalSkillCollection = new Dictionary<string, SkillStatus>();

            isPassiveSkillActivated = new Dictionary<string, bool>();
        }

        public void AddSkill(CharacterSkill skill)
        {
            SkillStatus skillStatus = new SkillStatus(skill);

            if (allSkillCollection.ContainsKey(skill.GetSkillID()))
            {
                return;
            }
            else
            {
                allSkillCollection.Add(skill.GetSkillID(), skillStatus);
            }

            if (typeof(ActiveSkill).IsAssignableFrom(skill.GetType()))
            {
                activeSkillCollection.Add(skill.GetSkillID(), skillStatus);
            }
            else if (typeof(PassiveSkill).IsAssignableFrom(skill.GetType()))
            {
                passiveSkillCollection.Add(skill.GetSkillID(), skillStatus);
            }

            if(skill.IsClassSkill())
            {
                switch(skill.GetTargetCharacterClassType())
                {
                    case EnumEntityClass.Warrior: 
                        warriorSkillCollection.Add(skill.GetSkillID(), skillStatus);
                        break;
                    case EnumEntityClass.Mage:
                        mageSkillCollection.Add(skill.GetSkillID(), skillStatus);
                        break;
                    case EnumEntityClass.Archer:
                        archerSkillCollection.Add(skill.GetSkillID(), skillStatus);
                        break;
                    default:
                        break;
                }

                classSkillCollection.Add(skill.GetSkillID(), skillStatus);
            }

            else if(skill.GetElementalType() != EnumElemental.None)
            {
                elementalSkillCollection.Add(skill.GetSkillID(), skillStatus);
            }
        }

        public void UpgradeSkill(string skillID, long count = 1)
        {
            SkillStatus skillStatus = allSkillCollection[skillID];

            if (skillStatus.getMaxCount() < skillStatus.GetCount() + count)
            {
                count = skillStatus.getMaxCount() - skillStatus.GetCount();
                if (count == 0) return;
            }

            skillStatus.upgradeSkill(count);
            UpdateCollection(skillID, skillStatus);
        }

        public void DegradeSkill(string skillID, long count = 1)
        {
            SkillStatus skillStatus = allSkillCollection[skillID];

            if (0 <= skillStatus.GetCount() - count)
            {
                skillStatus.upgradeSkill(-count);
                UpdateCollection(skillID, skillStatus);
            }
        }

        public void SetSkillUpgradeCount(string skillID, long count)
        {
            SkillStatus skillStatus = null;
            if (allSkillCollection.ContainsKey(skillID))
            {
                skillStatus = allSkillCollection[skillID];
            }

            if (skillStatus == null)
            {
                Debug.LogWarning("Error : No such skill : " + skillID);
                return;
            }

            if (skillStatus.getMaxCount() < count)
            {
                count = skillStatus.getMaxCount();
            }

            skillStatus.SetSkillUpgradeCount(count);
            UpdateCollection(skillID, skillStatus); ;
        }

        public void ClearSkill(string skillID)
        {
            SkillStatus skillStatus = null;
            if (allSkillCollection.ContainsKey(skillID))
            {
                skillStatus = allSkillCollection[skillID];
            }

            if (skillStatus == null)
            {
                Debug.LogError("Error : No such item : " + skillID);
                return;
            }

            skillStatus.ClearSkill();
            UpdateCollection(skillID, skillStatus);
        }

        public void ClearAll()
        {
            foreach(var skillStatus in allSkillCollection.Values)
            {
                skillStatus.ClearSkill();
            }
            Entity.Character.GetInstance().ResetSkillSet();

            isPassiveSkillActivated.Clear();
        }

        public void ClearAllClassSkill()
        {
            foreach (var skillStatus in classSkillCollection.Values)
            {
                skillStatus.ClearSkill();
                isPassiveSkillActivated.Remove(skillStatus.GetSkill().GetSkillID());
            }

            //Entity.Character.GetInstance().CorrectSkillSet();
        }

        public void ClearSpecificClassSkill(EnumEntityClass chClass)
        {
            switch (chClass)
            {
                case EnumEntityClass.Warrior:
                    ClearWarriorClassSkill();
                    break;
                case EnumEntityClass.Mage:
                    ClearMageClassSkill();
                    break;
                case EnumEntityClass.Archer:
                    ClearArcherClassSkill();
                    break;
                default:
                    break;
            }
        }

        public void ClearWarriorClassSkill()
        {
            foreach (var skillStatus in warriorSkillCollection.Values)
            {
                skillStatus.ClearSkill();
                isPassiveSkillActivated.Remove(skillStatus.GetSkill().GetSkillID());
            }
        }

        public void ClearMageClassSkill()
        {
            foreach (var skillStatus in mageSkillCollection.Values)
            {
                skillStatus.ClearSkill();
                isPassiveSkillActivated.Remove(skillStatus.GetSkill().GetSkillID());
            }
        }

        public void ClearArcherClassSkill()
        {
            foreach (var skillStatus in archerSkillCollection.Values)
            {
                skillStatus.ClearSkill();
                isPassiveSkillActivated.Remove(skillStatus.GetSkill().GetSkillID());
            }
        }

        public void ClearAllElementalSkill()
        {
            foreach(var skillStatus in elementalSkillCollection.Values)
            {
                skillStatus.ClearSkill();
                isPassiveSkillActivated.Remove(skillStatus.GetSkill().GetSkillID());
            }

            //Entity.Character.GetInstance().CorrectSkillSet();

        }

        public void ClearElementalSkill(EnumElemental elemental)
        {
            foreach (var skillStatus in elementalSkillCollection.Values)
            {
                if (skillStatus.GetSkill().GetElementalType() != elemental)
                    continue;
                skillStatus.ClearSkill();
                isPassiveSkillActivated.Remove(skillStatus.GetSkill().GetSkillID());
            }
        }

        public void UpdateCollection(string itemID, SkillStatus skillStatus)
        {
            allSkillCollection[itemID] = skillStatus;


            if (typeof(ActiveSkill).IsAssignableFrom(skillStatus.GetSkill().GetType()))
            {
                activeSkillCollection[itemID] = skillStatus;
            }
            else if (typeof(PassiveSkill).IsAssignableFrom(skillStatus.GetSkill().GetType()))
            {
                passiveSkillCollection[itemID] = skillStatus;
            }

            if (skillStatus.GetSkill().IsClassSkill())
            {
                classSkillCollection[itemID] = skillStatus;
            }

            else if (skillStatus.GetSkill().GetElementalType() != EnumElemental.None)
            {
                elementalSkillCollection[itemID] = skillStatus;
            }
        }


        public Dictionary<string, SkillStatus> GetCollection(string choice)
        {
            Dictionary<string, SkillStatus> itemCollection;
            switch (choice)
            {
                case "active":
                    itemCollection = SkillCollection.GetInstance().activeSkillCollection;
                    break;
                case "passive":
                    itemCollection = SkillCollection.GetInstance().passiveSkillCollection;
                    break;
                case "class":
                    itemCollection = SkillCollection.GetInstance().classSkillCollection;
                    break;
                case "elemental":
                    itemCollection = SkillCollection.GetInstance().elementalSkillCollection;
                    break;
                default:
                    itemCollection = SkillCollection.GetInstance().allSkillCollection;
                    break;
            }
            return itemCollection;
        }

        public void ClearCollection()
        {
            ResetDictionary(allSkillCollection);
            ResetDictionary(activeSkillCollection);
            ResetDictionary(passiveSkillCollection);
            ResetDictionary(classSkillCollection);
            ResetDictionary(elementalSkillCollection);
        }

        void ResetDictionary(Dictionary<string, SkillStatus> dictionary)
        {
            foreach (var element in dictionary)
            {
                element.Value.ClearSkill();
            }
        }

        public List<SkillAbility> GetCurrentCharacterBuff(Battle battle)
        {
            List<SkillAbility> skillAbilities = new List<SkillAbility>();

            foreach (var element in passiveSkillCollection)
            {
                PassiveSkill passiveSkill = element.Value.GetSkill() as PassiveSkill;

                if (element.Value.GetCount() <= 0) continue;

                if (
                    element.Value.GetSkill().GetTargetCharacterClassType() != Entity.Character.GetInstance().GetCurrentClass() && 
                    element.Value.GetSkill().GetTargetCharacterClassType() != EnumEntityClass.None
                )
                {
                    continue;
                }

                if(!isPassiveSkillActivated.ContainsKey(passiveSkill.GetSkillID()))
                {
                    if(passiveSkill.activateCondition(battle))
                    {
                        isPassiveSkillActivated[passiveSkill.GetSkillID()] = true;
                        
                        var skillCount = SkillCollection.GetInstance()
                            .allSkillCollection[passiveSkill.GetSkillID()].GetCount();
                        
                        var adjustedSkillAbilities = new List<SkillAbility>();

                        for (int i = 0; i < passiveSkill.skillAbilities.Count; ++i)
                        {
                            var skillAbility = passiveSkill.skillAbilities[i];
                            skillAbility.abilityData *= (int)skillCount;
                            adjustedSkillAbilities.Add(skillAbility);
                        }
                        
                        skillAbilities.AddRange(adjustedSkillAbilities);
                    }
                }
                else
                {
                    if (!passiveSkill.deactivateCondition(battle))
                    {
                        var skillCount = SkillCollection.GetInstance()
                            .allSkillCollection[passiveSkill.GetSkillID()].GetCount();
                        
                        var adjustedSkillAbilities = new List<SkillAbility>();

                        for (int i = 0; i < passiveSkill.skillAbilities.Count; ++i)
                        {
                            var skillAbility = passiveSkill.skillAbilities[i];
                            skillAbility.abilityData *= (int)skillCount;
                            adjustedSkillAbilities.Add(skillAbility);
                        }
                        
                        skillAbilities.AddRange(adjustedSkillAbilities);
                    }
                    else
                    {
                        isPassiveSkillActivated.Remove(passiveSkill.GetSkillID());
                    }
                }
            }

            return skillAbilities;
        }
    }

}