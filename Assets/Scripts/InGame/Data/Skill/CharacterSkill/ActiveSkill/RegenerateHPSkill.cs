using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Skill
{

    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Skills/Active/Regenerate HP Skill")]
    public class RegenerateHPSkill : ActiveSkill
    {
        [SerializeField] float percentage;
        public override void OnActive(BattleInstance.AbstractInstance target)
        {
            target.RegenerateHP((System.Numerics.BigInteger)((double)target.GetMaxHealth() * percentage));
        }

        public override void Attack(
            BattleInstance.AbstractInstance from,
            BattleInstance.AbstractInstance to
        )
        {
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

            longDesc += String.Format("{0} Active Skill\n",
                Elemental.GetInfo(elementalType).abilityDescription
            );

            longDesc += String.Format("This skill give you HP.\nAmount : {0}% of your total HP\n",
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
