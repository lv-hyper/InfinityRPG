using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InGame.UI
{
    public class DefaultInventorySortingChain
    {
        public static IOrderedEnumerable<KeyValuePair<string, Data.Item.ItemCollection.ItemStatus>>
            GetChain(IOrderedEnumerable<KeyValuePair<string, Data.Item.ItemCollection.ItemStatus>> data, bool isAscending)
        {
            if(isAscending)
            {
                return data
                    .ThenBy(x => x.GetType().ToString())
                    .ThenBy(x =>
                    {
                        long modifiedLevel = x.Value.getItem().level;
                        if (modifiedLevel == 0) modifiedLevel = long.MaxValue;
                        return modifiedLevel;
                    })
                    .ThenBy(x => x.Value.getItem().zone)
                    .ThenBy(x => x.Value.getItem().order)
                    .ThenBy(x =>
                    {
                        long modifiedPrice = x.Value.getItem().price;
                        if (modifiedPrice == 0) modifiedPrice = long.MaxValue;
                        return modifiedPrice;
                    })
                    .ThenBy(x => x.Value.getItem().name);
            }

            else
            {
                return data
                    .ThenByDescending(x => x.GetType().ToString())
                    .ThenByDescending(x =>
                    {
                        long modifiedLevel = x.Value.getItem().level;
                        if (modifiedLevel == 0) modifiedLevel = long.MaxValue;
                        return modifiedLevel;
                    })
                    .ThenByDescending(x => x.Value.getItem().zone)
                    .ThenByDescending(x => x.Value.getItem().order)
                    .ThenByDescending(x =>
                    {
                        long modifiedPrice = x.Value.getItem().price;
                        if (modifiedPrice == 0) modifiedPrice = long.MaxValue;
                        return modifiedPrice;
                    })
                    .ThenByDescending(x => x.Value.getItem().name);
            }
        }
    }
}
