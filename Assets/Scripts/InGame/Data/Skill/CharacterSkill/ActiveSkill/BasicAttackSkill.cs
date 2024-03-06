using InGame.Data.BattleInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Skill
{

    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Skills/Active/Basic Attack Skill")]
    public class BasicAttackSkill : ActiveSkill
    {
        public override void OnActive(AbstractInstance target)
        {
            
        }

        public override void Attack(
            BattleInstance.AbstractInstance from,
            BattleInstance.AbstractInstance to
        )
        {
            from.Attack(to); //from.SkillAttack(to, this, skillAnimation);
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
            return skillPercentDamage + ((stack-defaultUpgradeCount) * 0.1f);
        }
    }
}
