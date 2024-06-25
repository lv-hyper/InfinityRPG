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
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Skills/Active/General Skill")]
    public class GeneralSkill : ActiveSkill
    {
        [SerializeField] int criticalMode = -1;
        public override void OnActive(BattleInstance.AbstractInstance target)
        {
        }

        public override void Attack(
            BattleInstance.AbstractInstance from,
            BattleInstance.AbstractInstance to
        )
        {
            BigInteger damageAmount = from.RollDamage();

            damageAmount = (BigInteger)((double)damageAmount * GetTotalElementalBonus(from, to) * GetSkillPercentDamage(
                SkillCollection.GetInstance().allSkillCollection[skillID].GetCount()
            ));

            bool isCritical;

            if (criticalMode == -1)
                isCritical = from.GetCritical();

            else
                isCritical = (criticalMode == 1);

            if (isCritical)
            {
                damageAmount = (BigInteger)((double)damageAmount * (UnityEngine.Random.Range(0.9f, 1.1f) + from.GetCriticalDamage()));
            }

            to.Damage(from, damageAmount, isCritical, "");
            from.Recover((BigInteger)((double)damageAmount / to.GetDefenceRatio(from)));
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

            longDesc += String.Format(
                "Mana Cost : {0}, Cool Down : {1}\n",
                GetManaCost(stack),
                GetSkillCoolDown(stack)
            );

            if(criticalMode != -1)
            {
                longDesc += "This skill always do ";
                longDesc += criticalMode == 1 ? "critical" : "normal";
                longDesc += " attack.\n";
            }

            return longDesc;
        }
    }
}
