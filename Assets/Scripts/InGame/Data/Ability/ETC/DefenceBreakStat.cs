using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Ability
{
    [CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability/Defence Break")]
    public class DefenceBreakStat : Ability
    {
        public override AbilityData AggregateAbility(AbilityData originalAbility, AbilityData newAbility)
        {
            double amount = originalAbility.GetRawAmount();

            double newAmount = (100.0 - amount) * newAbility.GetRawAmount();

            newAmount /= 100.0f;

            AbilityData ability = new AbilityData(
                originalAbility.GetAbility(),
                amount + newAmount
            );

            return ability;
        }
    }
}
