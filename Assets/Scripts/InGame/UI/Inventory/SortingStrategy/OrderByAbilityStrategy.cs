using InGame.Data.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace InGame.UI
{
    public class OrderByAbilityStrategy : AbstractInventorySortingStrategy
    {
        private string itemType;

        public OrderByAbilityStrategy(string itemType)
        {
            this.itemType = itemType;
        }
        public override Dictionary<string, ItemCollection.ItemStatus> SortItemCollection(Dictionary<string, ItemCollection.ItemStatus> data, string itemType, bool isAscending = true)
        {
            if(itemType == "weapon")
            {
                if(isAscending)
                {
                    return DefaultInventorySortingChain.GetChain(
                    data.OrderBy(x => x.Value.getItem().getAbilityPower("Attack Percent")), true)
                    .ToDictionary(x => x.Key, x => x.Value);
                }
                else
                {
                    return DefaultInventorySortingChain.GetChain(
                    data.OrderByDescending(x => x.Value.getItem().getAbilityPower("Attack Percent")), false)
                    .ToDictionary(x => x.Key, x => x.Value);                    
                }
            }

            else if(itemType == "helmet" || itemType == "robe" || itemType == "glove" || itemType == "greave")
            {
                if(isAscending)
                {
                    return DefaultInventorySortingChain.GetChain(
                    data.OrderBy(x => x.Value.getItem().getAbilityPower("All Defence Percent")), true)
                    .ToDictionary(x => x.Key, x => x.Value);
                }
                else
                {
                    return DefaultInventorySortingChain.GetChain(
                    data.OrderByDescending(x => x.Value.getItem().getAbilityPower("All Defence Percent")), false)
                    .ToDictionary(x => x.Key, x => x.Value);
                }
            }

            else
            {
                if(isAscending)
                {
                    return DefaultInventorySortingChain.GetChain(
                    data.OrderBy(x => x.Value.getItem().zone), true
                    ).ToDictionary(x => x.Key, x => x.Value);
                }
                else
                {
                    return DefaultInventorySortingChain.GetChain(
                    data.OrderByDescending(x => x.Value.getItem().zone), false
                    ).ToDictionary(x => x.Key, x => x.Value);
                }
            }

        }


        public override string GetName()
        {
            return "Stat%";
        }

        public override bool IsApplicable(string currentInventoryChoice)
        {
            return true;
        }
    }
}
