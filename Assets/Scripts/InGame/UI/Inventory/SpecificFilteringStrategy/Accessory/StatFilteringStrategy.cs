﻿using InGame.Data.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace InGame.UI
{
    public class StatFilteringStrategy : AbstractInventoryFilteringStrategy
    {
        public override Dictionary<string, ItemCollection.ItemStatus> FilterItemCollection(Dictionary<string, ItemCollection.ItemStatus> data)
        {
            var filtered = data.Where(x => x.Value.getItem().findAbility(new List<string>{"Strength", "Intelligence", "Dexiterity", "Endurance", "Vitality", "All Stat", "Attack Percent"})).ToDictionary(x => x.Key, x => x.Value);
            return filtered;
        }

        public override string GetName()
        {
            return "Stat";
        }

        public override bool IsApplicable(string currentInventoryChoice)
        {
            return true;
        }
    }
}