using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Skill
{

    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Skills/Active/Drain Skill")]
    public class DrainSkill : ActiveSkill
    {
        [SerializeField] float percentage;
        public override void OnActive(BattleInstance.AbstractInstance target)
        {
        }

        public override void Attack(
            BattleInstance.AbstractInstance from,
            BattleInstance.AbstractInstance to
        )
        {
            BigInteger damageAmount = from.RollDamage();
            damageAmount = (BigInteger)((double)damageAmount * GetTotalElementalBonus(from, to) *
                GetSkillPercentDamage(
                    SkillCollection.GetInstance().allSkillCollection[skillID].GetCount()
                )
            );

            bool isCritical = from.GetCritical();

            if (isCritical)
                damageAmount = (BigInteger)((double)damageAmount * (UnityEngine.Random.Range(0.9f, 1.1f) + from.GetCriticalDamage()));

            to.Damage(from, damageAmount, isCritical, "");
            from.Recover((BigInteger)((double)damageAmount / to.GetDefenceRatio(from)));
            from.RegenerateHP((BigInteger)((double)damageAmount / to.GetDefenceRatio(from) * percentage));
        }

        public override long GetUpgradeCost()
        {
            long stack = SkillCollection.GetInstance().allSkillCollection[skillID].GetCount();
            if (stack >= maxUpgradeCount) stack = maxUpgradeCount - 1;

            return upgradeCost * (stack+1);
        }

        public override long GetSkillTurnCount(long stack)
        {
            return skillTurnCount;
        }

        public override long GetSkillCoolDown(long stack)
        {
            return skillCoolDown;
        }

        public override long GetManaCost(long stack)
        {
            return manaCost;
        }

        public override float GetSkillPercentDamage(long stack)
        {
            return skillPercentDamage + ((stack-defaultUpgradeCount) * skillPercentDamageIncreateRate);
        }

        public override string GetLongDescription(long stack)
        {
            string longDesc = "";

            longDesc += String.Format("{0} Active Skill, Skill Percentage : {1}%\n",
                Elemental.GetInfo(elementalType).abilityDescription,
                GetSkillPercentDamage(stack) * 100
            );

            longDesc += String.Format("Drains enemy and give you HP.\nAmount : {0}% of your Damage\n",
                percentage * 100
            );

            longDesc += String.Format(
                "Mana Cost : {0}, Cool Down : {1}",
                GetManaCost(stack),
                GetSkillCoolDown(stack)
            );

            return longDesc;
        }
    }
}
