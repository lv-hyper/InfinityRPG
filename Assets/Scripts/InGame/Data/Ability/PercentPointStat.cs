using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Ability
{
    [CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability/Percent Point Stat")]
    public class PercentPointStat : Ability
    {
        [SerializeField] List<PercentPointStat> percentPointStatDependency;

        public override double GetAmount(AbilitySet abilitySet, double amount = 0)
        {
            var abilities = abilitySet.GetAbilities();

            double totalPercentPoint = 0;

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

            amount += totalPercentPoint;

            return amount;
        }
    }
}
