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
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Skills/Low HP Passive Skill")]
    public class LowHPPassiveSkill : PassiveSkill
    {
        [SerializeField] float ratio;
        public override bool activateCondition(Battle battle)
        {
            if (battle == null) return false;

            BigInteger hp = battle.GetCharacterInstance().GetHealth();
            BigInteger mhp = battle.GetCharacterInstance().GetMaxHealth();
            return ((double)hp / (double)mhp) <= ratio;
        }

        public override bool deactivateCondition(Battle battle)
        {
            if (battle == null) return true;

            BigInteger hp = battle.GetCharacterInstance().GetHealth();
            BigInteger mhp = battle.GetCharacterInstance().GetMaxHealth();
            return ((float)hp / (float)mhp) > ratio;
        }

        public override string GetLongDescription(long stack)
        {
            string longDesc = string.Format("Passive, Activates under {0:0.00}% of Maximium HP\n", ratio*100);

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
