using UnityEngine;
using InGame;
using InGame.UI;
using System.Collections;
using System.Collections.Generic;

namespace InGame.Data
{
    public class Battle : Subject
    {
        public static readonly int maxTurn = 150;
        public static readonly int[] deathReaperThreshold = 
            {70, 90, 110, 130, 150};

        Entity.Character characterEntity;
        Data.Mob.AbstractMob mobEntity;

        BattleInstance.Character character;
        BattleInstance.Mob mob;

        System.Numerics.BigInteger currentTurn;


        //BattleHealthBar characterHealthBar, mobHealthBar;

        BattleResult battleResult;

        public Battle(Entity.Character characterEntity, Data.Mob.AbstractMob mobEntity)
        {
            this.characterEntity = characterEntity;
            this.mobEntity = mobEntity;

            character = characterEntity.GenerateBattleInstance();
            mob = new BattleInstance.Mob(mobEntity);

            battleResult = null;
            
            displayComponents = new List<IDisplayComponent>();

            currentTurn = 1 - character.GetDelayDeath();
        }

        public List<IDisplayComponent> displayComponents{get; set;}

        public void AddDisplayComponent(IDisplayComponent component)
        {
            displayComponents.Add(component);
        }

        public void UpdateBattleStatus()
        {
            foreach (var component in displayComponents)
            {
                component.Refresh(this);
            }
        }

        public void CharacterTurn()
        {
            // TODO : Refactor those messy codes
            Entity.Character.GetInstance().ApplySkillAbilities(this);
            //character.Attack(mob);
            
            var skillSlot = BattleSkillController.GetInstance().GetSlot(
                BattleSkillController.GetInstance().GetTargetSlot()
            );

            character.SkillAttack(mob, skillSlot.GetSkill());
            skillSlot.StartCool();

            CheckHealth();

            UpdateBattleStatus();
        } 

        public void MobTurn()
        {
            mob.ApplySkillAbilities(this);

            var _mobSkill = mob.GetSkill();

            if (_mobSkill != null)
            {
                mob.SkillAttack(character, _mobSkill);
            }
            else
            {
                mob.Attack(character);
            }

            mob.ReduceCooldown();

            CheckHealth();

            UpdateBattleStatus();
        }

        public void IncrementTurn()
        {
            currentTurn++;
            //if (currentTurn > maxTurn) battleResult = new BattleResult(false);
            if (GetDeathReaperDebuffCount() >= deathReaperThreshold.Length)
                battleResult = new BattleResult(false);
            UpdateBattleStatus();
        }
        public System.Numerics.BigInteger GetCurrentTurn()
        {
            return currentTurn;
        }

        public int GetDeathReaperDebuffCount()
        {
            int deathReaperCount = 0;
            foreach(var threshold in deathReaperThreshold)
            {
                if (currentTurn < threshold)
                    break;
                deathReaperCount++;
            }

            return deathReaperCount;
        }

        public void CheckHealth()
        {
            if (character.GetHealth() < 0)
            {
                battleResult = new BattleResult(false);
            }
            if (mob.GetHealth() < 0)
            {
                battleResult = new BattleResult(true);
            }
        }

        public BattleResult GetBattleResult()
        {
            return battleResult;
        }       

        public BattleInstance.Character GetCharacterInstance(){return character;}

        public BattleInstance.Mob GetMobInstance(){return mob;}
    }
}