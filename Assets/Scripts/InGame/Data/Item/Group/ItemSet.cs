using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Item.Group
{
    [Serializable]
    public class ItemSetEffectElement
    {
        [SerializeField] int itemCount;
        [SerializeField] List<ItemAbility> itemAbilities;
        [SerializeField] List<string> tagResists;

        public int GetItemCount() { return itemCount; }
        public List<ItemAbility> GetAbilities() { return itemAbilities; }
        public List<string> GetTagResists() {return tagResists;}
    }

    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item Set")]
    public class ItemSet : ItemGroup
    {
        [SerializeField] List<ItemSetEffectElement> itemSetEffects;
        public override void EnrollToCollection()
        {
            ItemSetCollection.GetInstance().AddItemSet(this);
        }

        public List<ItemAbility> GetItemSetAbility(List<Item> itemList)
        {
            int matchingCount = itemList.FindAll(item => items.Contains(item)).Count();

            List<ItemAbility> itemAbilities = new List<ItemAbility>();


            foreach(var setEffect in itemSetEffects)
            {
                if(setEffect.GetItemCount() <= matchingCount)
                {
                    itemAbilities.AddRange(setEffect.GetAbilities());
                }
            }

            return itemAbilities;
        }

        public List<ItemAbility> GetItemSetAbilityWhichNotEnough(List<Item> itemList)
        {
            int matchingCount = itemList.FindAll(item => items.Contains(item)).Count();

            List<ItemAbility> itemAbilities = new List<ItemAbility>();


            foreach(var setEffect in itemSetEffects)
            {
                if(matchingCount >= 1 && setEffect.GetItemCount() > matchingCount)
                {
                    itemAbilities.AddRange(setEffect.GetAbilities());
                }
            }

            return itemAbilities;            
        }

        public string GetItemSetEffectString(List<Item> itemList)
        {
            int minCount = itemSetEffects.Select(e => e.GetItemCount()).Min();
            int maxCount = itemSetEffects.Select(e => e.GetItemCount()).Max();
            int matchingCount = itemList.FindAll(item => items.Contains(item)).Count();

            if (matchingCount < minCount) return "";

            var setItemEffect = GetItemSetAbility(itemList);
            var setTagList = GetTagResists(itemList);

            string result = string.Format("Item Set : {0} ({1}/{2})\n", name, matchingCount, maxCount);

            result += GetTotalAbilityString(setItemEffect);

            /*
            foreach(var ability in setItemEffect)
            {
                result += ability.GetAbilityInString() + "\n";
            }
            */

            foreach (var ability in setTagList)
            {
                result += "Resist : "+ability + "\n";
            }

            result += "\n";

            return result;
        }

        public string GetItemSetEffectStringIncludingNotEnough(List<Item> itemList)
        {
            int maxCount = itemSetEffects.Select(e => e.GetItemCount()).Max();
            int matchingCount = itemList.FindAll(item => items.Contains(item)).Count();
            if (matchingCount == 0) return "";
            string result = string.Format("{0} Set({1} / {2} equipped)\n", name, matchingCount, maxCount);

            foreach (ItemSetEffectElement setEffect in itemSetEffects)
            {
                int reqParts = setEffect.GetItemCount();
                if (matchingCount < reqParts) result += "<color=grey>";

                var setItemEffect = GetItemSetAbility(itemList);
                var setTagList = GetTagResists(itemList);

                result += string.Format("[{0} Set Bonus]\n", reqParts);

                result += GetAbilityString(setEffect);
                if (matchingCount < reqParts) result += "</color>";

                /*
                foreach(var ability in setItemEffect)
                {
                    result += ability.GetAbilityInString() + "\n";
                }
                */

                foreach (var ability in setTagList)
                {
                    result += "Resist : "+ability + "\n";
                }
            }

            return result + "\n";            
        }

        public string GetItemSetNameString(List<Item> itemList)
        {
            int minCount = itemSetEffects.Select(e => e.GetItemCount()).Min();
            int maxCount = itemSetEffects.Select(e => e.GetItemCount()).Max();
            int matchingCount = itemList.FindAll(item => items.Contains(item)).Count();

            if (matchingCount < minCount) return "";

            var setItemEffect = GetItemSetAbility(itemList);
            var setTagList = GetTagResists(itemList);

            string result = string.Format("{0} Set: ({1} / {2} equipped)\n", name, matchingCount, maxCount);

            /*
            foreach(var ability in setItemEffect)
            {
                result += ability.GetAbilityInString() + "\n";
            }
            */

            foreach (var ability in setTagList)
            {
                result += "Resist : "+ability + "\n";
            }

            result += "\n";

            return result;
        }

        public List<string> GetTagResists(List<Item> itemList)
        {
            int matchingCount = itemList.FindAll(item => items.Contains(item)).Count();

            List<string> result = new List<string>();


            foreach (var setEffect in itemSetEffects)
            {
                if (setEffect.GetItemCount() <= matchingCount)
                {
                    result.AddRange(setEffect.GetTagResists());
                }
            }

            return result;
        }

        public static string GetTotalAbilityString(List<ItemAbility> itemAbilities)
        {
            var abilities = itemAbilities.Select(itemAbility => { return itemAbility.GetAbility(); });
            var groupedAbilities = abilities.GroupBy(ability => ability.GetAbility().name).Select(group =>
            {
                var aggregatedAbility = group.ToList().Aggregate((originalAbility, newAbility) =>
                {
                    return originalAbility.GetAbility().AggregateAbility(originalAbility, newAbility);
                });

                return new KeyValuePair<string, Ability.AbilityData>(group.Key, aggregatedAbility);
            }).ToList();
            string result = "";

            foreach (var groupedAbility in groupedAbilities)
            {

                string description = string.Format(
                    groupedAbility.Value.GetAbility().GetDescription(),
                    groupedAbility.Value.GetRawAmount()
                );

                result += description + "\n";
            }

            return result;
        }

        public static string GetAbilityString(ItemSetEffectElement setEffect)
        {
            string result = "";
            foreach (var ability in setEffect.GetAbilities())
            {
                result += string.Format(ability.GetAbility().GetAbility().GetDescription(), ability.GetAbility().GetRawAmount()) + "\n";
            }
            /*
            var abilities = .Select(itemAbility => { return itemAbility.GetAbility(); });
            var groupedAbilities = abilities.GroupBy(ability => ability.GetAbility().name).Select(group =>
            {
                var aggregatedAbility = group.ToList().Aggregate((originalAbility, newAbility) =>
                {
                    return originalAbility.GetAbility().AggregateAbility(originalAbility, newAbility);
                });

                return new KeyValuePair<string, Ability.AbilityData>(group.Key, aggregatedAbility);
            }).ToList();
            string result = "";

            foreach (var groupedAbility in abilities)
            {

                string description = string.Format(
                    groupedAbility.GetAbility().GetDescription(),
                    groupedAbility.GetRawAmount()
                );

                result += description + "\n";
            }
            */
            return result;   
                     
        }
    }

    public class ItemSetCollection
    {
        static ItemSetCollection instance = null;

        List<ItemSet> itemSets;
        Dictionary<string, List<ItemSet>> itemIDIndex;
        private bool loaded = false;

        ItemSetCollection()
        {
            itemSets = new List<ItemSet>();
            itemIDIndex = new Dictionary<string, List<ItemSet>>();
        }

        public static ItemSetCollection GetInstance()
        {
            if (instance == null)
                instance = new ItemSetCollection();

            return instance;
        }

        public void AddItemSet(ItemSet itemSet)
        {
            itemSets.Add(itemSet);

            foreach (var item in itemSet.GetItems())
            {
                if(item == null)
                {
                    Debug.LogError(itemSet.name);
                    Debug.Assert(item != null);
                }
                if (!itemIDIndex.ContainsKey(item.itemID))
                {
                    itemIDIndex[item.itemID] = new List<ItemSet>();
                }
                itemIDIndex[item.itemID].Add(itemSet);
            }
        }

        public List<ItemSet> GetItemSetByItemID(string itemID)
        {
            if (itemIDIndex.ContainsKey(itemID))
                return itemIDIndex[itemID];

            else
                return null;
        }

        public List<ItemSet> GetItemSetByItemIDList(List<string> itemIDList)
        {
            List<ItemSet> itemSets = new List<ItemSet>();
            foreach (var itemID in itemIDList)
            {
                var itemSet = GetItemSetByItemID(itemID);
                if (itemSet != null)
                    itemSets.AddRange(itemSet);
            }

            return itemSets;
        }

        public List<ItemAbility> GetItemSetAbility(List<Item> items)
        {
            List<ItemAbility> itemAbilities = new List<ItemAbility>();

            foreach (var itemSet in itemSets)
            {
                itemAbilities.AddRange(itemSet.GetItemSetAbility(items));
            }

            return itemAbilities;
        }

        public string GetItemSetEffectString(List<Item> itemList)
        {
            string result = "";
            foreach (var itemSet in itemSets)
            {
                //result += itemSet.GetItemSetEffectString(itemList);
                result += itemSet.GetItemSetEffectStringIncludingNotEnough(itemList);
            }
            return result;
        }

        public List<string> GetItemSetTagResists(List<Item> itemList)
        {
            List<string> _tagResists = new List<string>();

            foreach (var itemSet in itemSets)
            {
                _tagResists.AddRange(itemSet.GetTagResists(itemList));
            }

            return _tagResists;
        }

        public string GetItemSetName(List<Item> itemList)
        {
            string result = "";
            foreach (var itemSet in itemSets)
            {
                //result += itemSet.GetItemSetEffectString(itemList);
                result += itemSet.GetItemSetNameString(itemList);
            }
            return result;
        }        

        public string FindSetName(Item item)
        {
            //strange mecahism because of optimization
            string result = "";

            foreach (ItemSet itemSet in itemSets)
            {
                if (itemSet.GetItems().Contains(item))
                {
                    result += "and " + itemSet.name;
                }
            }

            if(result != "") return result.Substring(3);
            else return "";
        }

        public bool Loaded()
        {
            return loaded;
        }

        public void SetLoadFinished()
        {
            loaded = true;
        }
    }
}
