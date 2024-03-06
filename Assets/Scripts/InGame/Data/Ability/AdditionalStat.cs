using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Ability
{
    [Serializable]
    public class AdditionalStatDependencyElement
    {
        public Ability ability;
        public double multiplier;
    }

    [CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability/Additional Stat")]
    public class AdditionalStat : Ability
    {
        [SerializeField] List<AdditionalStatDependencyElement> additionalStatDependency;
        public override double GetAmount(AbilitySet abilitySet, double amount = 0)
        {
            var abilities = abilitySet.GetAbilities();

            foreach (var dep in additionalStatDependency)
            {
                if (abilities.ContainsKey(dep.ability.name))
                {                    
                    double _amount = abilitySet.GetAbilityAmount(dep.ability.name);
                    amount += (_amount * dep.multiplier);
                }
                else
                {
                    double _amount = dep.ability.GetAmount(abilitySet, 0);
                    amount += (_amount * dep.multiplier);
                }
            }

            return amount;
        }
    }
}
