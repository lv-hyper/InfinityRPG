using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Ability
{
    [CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability/Multiply Rate Stat")]
    public class MultiplyRateStat : Ability
    {
        public override AbilityData AggregateAbility(AbilityData originalAbility, AbilityData newAbility)
        {
            double amount = originalAbility.GetRawAmount() * newAbility.GetRawAmount();

            AbilityData ability = new AbilityData(
                originalAbility.GetAbility(),
                amount
            );

            return ability;
        }

        public override double GetInitialAmount()
        {
            return 1;
        }
    }
}
