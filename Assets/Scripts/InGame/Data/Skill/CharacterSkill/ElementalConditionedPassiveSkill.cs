using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Skill
{
    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Skills/Elemental Conditioned Passive Skill")]
    public class ElementalConditionedPassiveSkill : PassiveSkill
    {
        [SerializeField] EnumElemental elemental;
        public override bool activateCondition(Battle battle)
        {
            if (battle == null) return false;

            return battle.GetMobInstance().GetElementalDamage(elemental) > 0;
        }

        public override bool deactivateCondition(Battle battle)
        {
            if (battle == null) return true;

            return battle.GetMobInstance().GetElementalDamage(elemental) <= 0;
        }

        public override string GetLongDescription(long stack)
        {
            string longDesc = string.Format("Passive, Only affects when fighting against {0} enemy\n", Elemental.GetInfo(elementalType).abilityDescription);

            foreach (var ability in skillAbilities)
            {
                if (ability.GetAmount() == 0) continue;

                longDesc += ability.GetAbilityInString(stack);
                longDesc += ", ";
            }
            longDesc = longDesc.Substring(0, longDesc.Length - 2);

            return longDesc;
        }
    }

}
