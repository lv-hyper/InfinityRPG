using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Ability
{
    [CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability/Base Stat")]
    public class BaseStat : Ability
    {
        [SerializeField] List<AdditionalStat> additionalStatDependency;
        [SerializeField] List<PercentPointStat> percentPointStatDependency;
        [SerializeField] List<PercentStat> percentStatDependency;
        public override double GetAmount(AbilitySet abilitySet, double amount = 0)
        {
            var abilities = abilitySet.GetAbilities();

            double totalPercentPoint = 100;
            double totalPercent = 100;

            foreach(var dep in additionalStatDependency)
            {
                if(abilities.ContainsKey(dep.name))
                {
                    amount += dep.GetAmount(abilitySet, abilities[dep.name].GetAmount(abilitySet));
                }
                else
                {
                    amount += dep.GetAmount(abilitySet, 0);
                }
            }

            foreach (var dep in percentPointStatDependency)
            {
                if (abilities.ContainsKey(dep.name))
                {
                    totalPercentPoint += dep.GetAmount(abilitySet, abilities[dep.name].GetAmount(abilitySet));
                }
                else
                {
                    totalPercentPoint += dep.GetAmount(abilitySet, 0);
                }
            }

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

            amount = amount * totalPercent * totalPercentPoint / 10000.0;

            return amount;
        }
    }
}

