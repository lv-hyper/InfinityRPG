using System.Collections.Generic;
using System.Linq;
using InGame.Data.Mob;

namespace Main.UI.Statistics.Collection
{
    public interface ICollectionSortingStrategy
    {
        public IOrderedEnumerable<AbstractMob> Sort(IEnumerable<AbstractMob> items);
    }
}