using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InGame.UI
{
    public abstract class AbstractInventoryFilteringStrategy
    {
        public abstract Dictionary<string, Data.Item.ItemCollection.ItemStatus>
            FilterItemCollection(Dictionary<string, Data.Item.ItemCollection.ItemStatus> data);

        public abstract string GetName();

        public abstract bool IsApplicable(string currentInventoryChoice);
    }
}
