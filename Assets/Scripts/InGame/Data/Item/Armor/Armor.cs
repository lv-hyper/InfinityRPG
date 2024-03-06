using UnityEngine;
using System;
using InGame.Data.Item;
using System.Collections.Generic;

namespace InGame.Data.Item.Armor
{
    [Serializable]
    public abstract class Armor : Item
    {
        private void Reset() {
            maxStack = 10;
        }

        public override List<ItemAbility> GetItemAbility()
        {
            return ItemAbility.AmpiflyAbility(
                new List<string> {
                    "Attack Percent Point",
                    "Attack",
                    "All Defence Percent Point",
                    "Additional Defence",
                    "Melee Defence Percent Point",
                    "Additional Melee Defence",
                    "Magical Defence Percent Point",
                    "Additional Magical Defence",
                    "Ranged Defence Percent Point",
                    "Additional Ranged Defence",
                    "Vitality Percent",
                    "Vitality Percent Point",
                    "Additional Vitality",
                    "Endurance Percent",
                    "Endurance Percent Point",
                    "Additional Endurance",
                },
                base.GetItemAbility(),
                ItemCollection.GetInstance().allCollection[itemID].getCount()
            );
        }

    }    
}
