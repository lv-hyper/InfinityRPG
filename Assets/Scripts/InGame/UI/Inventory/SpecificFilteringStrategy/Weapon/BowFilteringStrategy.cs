using InGame.Data.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace InGame.UI
{
    public class BowFilteringStrategy : AbstractInventoryFilteringStrategy
    {
        public override Dictionary<string, ItemCollection.ItemStatus> FilterItemCollection(Dictionary<string, ItemCollection.ItemStatus> data)
        {
            var filtered = data.Where(x => x.Value.getItem().GetAdditionalInfo().Contains("DEX")).ToDictionary(x => x.Key, x => x.Value);
            return filtered;
        }

        public override string GetName()
        {
            return "Bow";
        }

        public override bool IsApplicable(string currentInventoryChoice)
        {
            return true;
        }
    }
}
