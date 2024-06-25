using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Ability
{
    [CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability/Base Rate Stat")]
    public class BaseRateStat : Ability
    {
        [SerializeField] List<AdditionalRateStat> additionalRateDependency;
        [SerializeField] List<MultiplyRateStat> multipliyRateDependency;
        public override double GetAmount(AbilitySet abilitySet, double amount = 0)
        {
            var abilities = abilitySet.GetAbilities();

            double totalMultiplier = 1;

            foreach (var dep in additionalRateDependency)
            {
                if (abilities.ContainsKey(dep.name))
                {
                    amount += dep.GetAmount(abilitySet, abilities[dep.name].GetAmount(abilitySet));
                }
                else
                {
                    amount += dep.GetAmount(abilitySet, 0);
                }
            }

            foreach (var dep in multipliyRateDependency)
            {
                if (abilities.ContainsKey(dep.name))
                {
                    totalMultiplier *= dep.GetAmount(abilitySet, abilities[dep.name].GetAmount(abilitySet));
                }
                else
                {
                    totalMultiplier *= dep.GetAmount(abilitySet, 1);
                }
            }

            amount = ((amount + 100.0f) * totalMultiplier) - 100.0f;

            return amount;
        }
    }
}