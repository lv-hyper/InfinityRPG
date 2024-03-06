using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Skill
{
    public abstract class PassiveSkill : CharacterSkill
    {
        [SerializeField] public List<SkillAbility> skillAbilities;
        public abstract bool activateCondition(Battle battle);
        public abstract bool deactivateCondition(Battle battle);

        public override string GetLongDescription(long stack)
        {
            string longDesc = "Passive | ";

            foreach (var ability in skillAbilities)
            {
                if (ability.GetAmount() == 0) continue;

                if (!ability.GetTarget())
                    longDesc += "Enemy ";
                longDesc += ability.GetAbilityInString(stack);
                longDesc += ", ";
            }
            longDesc = longDesc.Substring(0, longDesc.Length - 2);

            return longDesc;
        }

    }
}
