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
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Skills/Active/General Buff Skill")]
    public class GeneralBuffSkill : ActiveSkill
    {
        [SerializeField] List<BattleEffect.Buff> buffList;
        public override void OnActive(AbstractInstance target)
        {
            long stack = SkillCollection.GetInstance().allSkillCollection[skillID].GetCount();
            foreach (BattleEffect.Buff buff in buffList)
            {
                var _buff = Instantiate(buff);
                _buff.level = (int)stack;
                target.Buff(_buff);
            }
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
            return skillPercentDamage + ((stack-defaultUpgradeCount) * skillPercentDamageIncreateRate);
        }

        public override string GetLongDescription(long stack)
        {
            string longDesc = "";

            longDesc += String.Format("{0} Active Skill\n",
                Elemental.GetInfo(elementalType).abilityDescription
            );

            longDesc += "This skill give you buff :\n";

            foreach(var buff in buffList)
            {
                var _buff = Instantiate(buff);
                _buff.level = (int)stack;
                longDesc += _buff.GetDescription();
            }

            longDesc += String.Format(
                "Mana Cost : {0}, Cool Down : {1}",
                GetManaCost(stack),
                GetSkillCoolDown(stack)
            );

            return longDesc;
        }
    }
}
