using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InGame.UI
{
    public abstract class AbstractInventorySortingStrategy
    {
            public abstract Dictionary<string, Data.Item.ItemCollection.ItemStatus>
            SortItemCollection(Dictionary<string, Data.Item.ItemCollection.ItemStatus> data, string itemType, bool isAscending = true);

        public abstract string GetName();

        public abstract bool IsApplicable(string currentInventoryChoice);
    }
}
