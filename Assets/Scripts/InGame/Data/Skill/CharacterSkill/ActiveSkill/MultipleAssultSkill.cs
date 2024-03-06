using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Skill
{

    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Skills/Active/Multiple Assult Skill")]
    public class MultipleAssultSkill : ActiveSkill
    {
        [SerializeField] List<float> damagePercentList;
        public override void OnActive(BattleInstance.AbstractInstance target)
        {

        }

        public override void Attack(
            BattleInstance.AbstractInstance from,
            BattleInstance.AbstractInstance to
        )
        {
            for (int i = 0; i < damagePercentList.Count; ++i)
            {
                System.Numerics.BigInteger damageAmount = from.RollDamage();
                damageAmount = (System.Numerics.BigInteger)(
                    (double)damageAmount * 
                    GetTotalElementalBonus(from, to) * 
                    damagePercentList[i] * 
                    GetSkillPercentDamage(SkillCollection.GetInstance().allSkillCollection[skillID].GetCount())
                );

                bool isCritical = from.GetCritical();

                if(isCritical)
                    damageAmount = (System.Numerics.BigInteger)((double)damageAmount * (UnityEngine.Random.Range(0.9f, 1.1f) + from.GetCriticalDamage()));

                to.Damage(from, damageAmount, isCritical, "");
                from.Recover((System.Numerics.BigInteger)((double)damageAmount / to.GetDefenceRatio(from)));
            }
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
        public override string GetLongDescription(long stack)
        {
            string longDesc = "";

            longDesc += String.Format("{0} Active Skill\n",
                Elemental.GetInfo(elementalType).abilityDescription,
                GetSkillPercentDamage(stack) * 100
            );
            longDesc += String.Format("This skill do {0} times of attack at once\n", damagePercentList.Count);

            longDesc += "Damage Percent : ";
            foreach(float damagePercent in damagePercentList)
            {
                longDesc += String.Format("{0}%, ", damagePercent*100);
            }
            longDesc += "\n";

            longDesc += String.Format(
                "Mana Cost : {0}, Cool Down : {1}",
                GetManaCost(stack),
                GetSkillCoolDown(stack)
            );

            return longDesc;
        }
    }
}
