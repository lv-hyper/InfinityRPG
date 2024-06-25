using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InGame.Data;
using InGame.Data.BattleInstance;
using TMPro;
using System;

namespace InGame.UI
{
    public class BattleSkillController : MonoBehaviour
    {
        [SerializeField] BattleSkillSlot defaultSkillSlot;
        [SerializeField] List<BattleSkillSlot> skillSlots;

        static BattleSkillController instance = null;

        int selectedSkill;
        int nextPrimarySlot;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
                selectedSkill = -1;
                nextPrimarySlot = 0;
            }
        }

        private void OnEnable()
        {
            var currentEquipmentSet = Entity.Character.GetInstance().GetCurrentEquipmentSet();
            for (int i = 0; i < 6; ++i)
            {
                SetSkill(currentEquipmentSet.GetSkill(i), i);
            }
            UpdateNextSkill();
        }

        public static BattleSkillController GetInstance()
        {
            return instance;
        }

        public void SetSkill(Data.Skill.ActiveSkill skill, int index)
        {
            if (index == 0) defaultSkillSlot.SetSkill(skill);

            else if (index - 1 < skillSlots.Count)
                skillSlots[index - 1].SetSkill(skill);

            else Debug.LogError("Index Out of Bound");
        }

        public bool ValidateSkill(int slot)
        {
            var character = Entity.Character.GetInstance();
            var currentEquipmentSet = character.GetCurrentEquipmentSet();

            var skill = currentEquipmentSet.GetSkill(slot);

            if (!skill)
                return false;

            Data.Skill.SkillCollection.SkillStatus skillStatus = null;
            if (currentEquipmentSet.GetSkill(slot).GetSkillID() != null && currentEquipmentSet.GetSkill(slot).GetSkillID() != "")
                skillStatus = Data.Skill.SkillCollection.GetInstance().allSkillCollection[currentEquipmentSet.GetSkill(slot).GetSkillID()];

            // if upgrade count is 0
            if (skillStatus.GetCount() <= 0)
                return false;

            // if total turn count exists - later...

            // if skill is in cooldown
            if (GetSlot(slot).GetCool() > 0)
                return false;
            
            // if elemental damage amount is not enough to activate skill
            /*
            if (skill.GetElementalType() != EnumElemental.None)
            {
                if (skill.IsAttackSkill())
                {
                    if(
                        character.elementalStatus.ContainsKey(skill.GetElementalType()) &&
                        character.elementalStatus[skill.GetElementalType()] <= 0)
                        return false;

                    
                }
                else
                {
                    if(
                        Action.BattleController.GetInstance().
                            GetBattle().GetCharacterInstance().GetElementalDefence(skill.GetElementalType()) <= 0)
                        return false;
                }
                
            }
            */
            
            /*
            if (skill.GetElementalType() != EnumElemental.None &&
                !ElementalTreeCollection.GetInstance().GetElementalTree(skill.GetElementalType())
                .IsSkillAvailable(skill.GetSkillID()))
            {
                return false;
            }
            */
            
            // if mana is not enough to activate skill
            if (
                GetSlot(slot).GetSkill().GetManaCost(skillStatus.GetCount()) >
                Action.BattleController.GetInstance().GetBattle().GetCharacterInstance().GetMana()
            )   return false;

            return true;
        }

        public int GetPrimarySlot()
        {
            var currentEquipmentSet = Entity.Character.GetInstance().GetCurrentEquipmentSet();
            int primarySlot = 0;
            int highestPriority = -1;

            for (int i = 0; i < 6; ++i)
            {
                if (!ValidateSkill(i) || !GetSlot(i).GetOnOff())
                    continue;

                if (currentEquipmentSet.GetSkillPriority(i) > highestPriority)
                {
                    primarySlot = i;
                    highestPriority = currentEquipmentSet.GetSkillPriority(i);
                }
            }

            return primarySlot;
        }

        public void UpdateNextSkill()
        {

            if (selectedSkill != -1 && ValidateSkill(selectedSkill))
                nextPrimarySlot = selectedSkill;

            else
            {
                nextPrimarySlot = GetPrimarySlot();
                selectedSkill = -1;
            }

            for(int i=0;i<6;++i)
            {
                if (i == nextPrimarySlot)
                    GetSlot(i).Highlight();

                else
                    GetSlot(i).UnHighlight();
            }

        }

        public void SelectNextSkill(int index)
        {
            selectedSkill = index;

            UpdateNextSkill();
        }

        public BattleSkillSlot GetSlot(int index)
        {
            if (index == 0) return defaultSkillSlot;
            else return skillSlots[index - 1];
        }

        public int GetTargetSlot() { return nextPrimarySlot; }

        public void Turn()
        {
            for (int i = 0; i < 6; ++i)
            {
                GetSlot(i).Turn();
            }
        }
    }
}
