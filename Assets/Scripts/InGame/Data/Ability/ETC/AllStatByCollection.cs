using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Ability
{
    [CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability/All Stat By Collection")]
    public class AllStatByCollection : Ability
    {
        public override double GetAmount(AbilitySet abilitySet, double amount = 0)
        {
            long itemCountWithStack = 0;

            var itemCollection = InGame.Data.Item.ItemCollection.GetInstance().allCollection;
            foreach (var item in itemCollection)
            {
                if (item.Value.getItem().GetType() == typeof(InGame.Data.Item.Material))
                    continue;

                itemCountWithStack += item.Value.getCount();
            }

            return itemCountWithStack * amount;
        }
    }
}
