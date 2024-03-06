using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Ability
{
    [CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability/Percent Stat")]
    public class PercentStat : Ability
    {
        [SerializeField] List<PercentStat> percentStatDependency;
        public override AbilityData AggregateAbility(AbilityData originalAbility, AbilityData newAbility)
        {
            double amount = (100.0 + originalAbility.GetRawAmount()) * (100.0 + newAbility.GetRawAmount()) / 100.0;
            amount -= 100.0;

            AbilityData ability = new AbilityData(
                originalAbility.GetAbility(),
                amount
            );

            return ability;
        }

        public override double GetAmount(AbilitySet abilitySet, double amount = 0)
        {
            var abilities = abilitySet.GetAbilities();

            double totalPercent = 100;

            foreach (var dep in percentStatDependency)
            {
                if (abilities.ContainsKey(dep.name))
                {
                    totalPercent *= (100 + dep.GetAmount(abilitySet, abilities[dep.name].GetAmount(abilitySet)));
                    totalPercent /= 100.0;
                }
                else
                {
                    totalPercent *= (100 + dep.GetAmount(abilitySet, 0));
                    totalPercent /= 100.0;
                }
            }

            return ((100.0 + amount) * totalPercent / 100.0) - 100.0;
        }
    }
}
