using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Skill
{
    [Serializable]
    public enum SkillType
    {
        Physical,
        Magical
    }
    [Serializable]
    public abstract class ActiveSkill : CharacterSkill
    {
        [SerializeField] protected long skillTurnCount;
        [SerializeField] protected long skillCoolDown;
        [SerializeField] protected long manaCost;

        [SerializeField] protected bool isAttackSkill;

        [SerializeField] protected SkillType skillType;

        [SerializeField] protected float skillPercentDamage;

        [SerializeField] protected AnimationClip skillAnimation;
        public abstract void OnActive(BattleInstance.AbstractInstance target);
        public abstract void Attack(
            BattleInstance.AbstractInstance from, 
            BattleInstance.AbstractInstance to
        );

        public abstract long GetSkillTurnCount(long stack);
        public abstract long GetSkillCoolDown(long stack);
        public abstract long GetManaCost(long stack);
        public abstract float GetSkillPercentDamage(long stack);

        public virtual double GetTotalElementalBonus(BattleInstance.AbstractInstance from, BattleInstance.AbstractInstance instance)
        {
            float sum = 0;
            for (var _elemental = EnumElemental.Air; _elemental <= EnumElemental.Count - 1; ++_elemental)
            {
                var diff = from.GetElementalDamage(_elemental) * (1 - (instance.GetElementalDefence(_elemental) / 100.0f));
                if (diff < 0) diff = 0;
                sum += diff;
            }

            double elementalBonus = sum / 100.0f;
            elementalBonus += 1;

            return elementalBonus;
        }

        public override string GetLongDescription(long stack)
        {
            string longDesc = "";

            longDesc += String.Format("{0} Active Skill, Skill Percentage : {1}%\n",
                Elemental.GetInfo(elementalType).abilityDescription,
                GetSkillPercentDamage(stack) * 100
            );

            longDesc += String.Format(
                "Mana Cost : {0}, Cool Down : {1}",
                GetManaCost(stack), 
                GetSkillCoolDown(stack)
            );

            return longDesc;
        }

        public bool IsAttackSkill() { return isAttackSkill; }

        public EnumElemental GetElementalType()
        {
            return elementalType;
        }
    }
}
