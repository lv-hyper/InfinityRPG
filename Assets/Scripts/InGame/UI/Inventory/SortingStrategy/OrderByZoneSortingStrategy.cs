using InGame.Data.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InGame.UI
{
    public class OrderByZoneSortingStrategy : AbstractInventorySortingStrategy
    {
        public override Dictionary<string, ItemCollection.ItemStatus> SortItemCollection(Dictionary<string, ItemCollection.ItemStatus> data, string itemType, bool isAscending = true)
        {
            if (isAscending)
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

        public override string GetName()
        {
            return "Zone";
        }

        public override bool IsApplicable(string currentInventoryChoice)
        {
            return true;
        }
    }
}
