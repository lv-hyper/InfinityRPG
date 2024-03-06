using InGame.Data.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InGame.UI
{
    public class DefaultInventoryFilteringStrategy : AbstractInventoryFilteringStrategy
    {
        public override Dictionary<string, ItemCollection.ItemStatus> FilterItemCollection(Dictionary<string, ItemCollection.ItemStatus> data)
        {
            return data;
        }

        public override string GetName()
        {
            return "Default";
        }

        public override bool IsApplicable(string currentInventoryChoice)
        {
            return true;
        }
    }
}
