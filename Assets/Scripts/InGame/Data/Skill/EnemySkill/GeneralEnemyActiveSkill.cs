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
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Skills/Enemy/Active/Basic Attack Skill")]
    public class GeneralEnemyActiveSkill : EnemyActiveSkill
    {
        [SerializeField] string damagePrefix;

        public override void OnActive(BattleInstance.AbstractInstance target)
        {
        }

        public override void Attack(
            BattleInstance.AbstractInstance from,
            BattleInstance.AbstractInstance to
        )
        {
            System.Numerics.BigInteger rawDamage = from.RollDamage();

            double elementalAdvantage = 1.0f;

            if (to.GetElementalDefence(Elemental.GetWeakerElemental(elementalType)) > 0)
            {
                elementalAdvantage = 1.5f;
            }

            to.Damage(
                from,
                (System.Numerics.BigInteger)(from.GetTotalElementalDamageBonus(to) * elementalAdvantage * skillPercentDamage * (double)rawDamage / 100.0f),
                false,
                damagePrefix
            );
        }

        public override long GetSkillCoolDown()
        {
            return skillCoolDown;
        }

        public override float GetSkillPercentDamage()
        {
            return skillPercentDamage;
        }
    }
}
